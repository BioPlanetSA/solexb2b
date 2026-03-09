using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using log4net;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Core;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.SolexPay.Model;

namespace SolEx.Hurt.SolexPay
{
    public class SolexPayLogic : IPaymentProvider
    {
        private readonly ILog _log;

        private RestClient restClient { get; set; }

      
        private readonly SolexPayConfig solexPayConfig;

        private string _pragmaKey;
        private string _pragmaSecret;
        private string _baseUrl;

        public SolexPayLogic(ILog log, SolexPayConfig configSolexPay)
        {           
            this.solexPayConfig = configSolexPay;

            string baseUrl;

            if (configSolexPay.UseTestGateway)
            {
                _pragmaKey = "02a93843-fe98-4284-b4b1-ad4cd3d55625";
                _baseUrl = "https://api-partner.box.pragmago.tech";
                _pragmaSecret = "6b70303e-83f2-45d1-92d8-58d5538fa84a";
            }
            else
            {
                //production
                _pragmaKey = "59f03c67-f675-44d3-9592-e2d5b0309cdb";
                _baseUrl = "https://onlineapi.pragmago.pl";
                _pragmaSecret = "de69d610-9dde-4ed8-aedf-9c7945952fb3";
            }

            string token = this.LoginToPragma();
            this.restClient = new RestClient(_baseUrl) { Authenticator = new JwtAuthenticator(token) };
            
            //default QS
            this.restClient.AddDefaultParameter("utm_source", "www", ParameterType.QueryString);
            this.restClient.AddDefaultParameter("utm_medium", "koszyk", ParameterType.QueryString);
            this.restClient.AddDefaultParameter("utm_campaign", "odroczonaplatnosc", ParameterType.QueryString);
            this.restClient.AddDefaultParameter("utm_term", "p-152577", ParameterType.QueryString);
            this.restClient.AddDefaultParameter("utm_content", "=c-zaplac", ParameterType.QueryString);
            this.restClient.AddDefaultParameter("recommendation_code", "152577", ParameterType.QueryString);
        }


        public string LoginToPragma()
        {
            var request = new RestRequest("api/partner/authorize", Method.POST);

            Dictionary<string, string> parametry = new Dictionary<string, string>()
                {
                    {"key", this._pragmaKey},
                    {"secret",  this._pragmaSecret}
                };

            request.AddJsonBody(parametry);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var client = new RestClient(this._baseUrl);
            var result = client.Post<SolexPayAuth>(request);

            return result.Data.token;
        }

        public WartoscLiczbowa CalculateDefermentCost(decimal valueToPay, string currency, int defermentDays = 30)
        {
            var request = new RestRequest(calculatedDefermentUrl, Method.GET);
            request.AddQueryParameter("value[amount]", valueToPay.ToString(CultureInfo.InvariantCulture));
            request.AddQueryParameter("value[currency]", currency);
            request.AddQueryParameter("defermentDays", defermentDays.ToString() );
            request.AddQueryParameter("networkCode", "152577");

           var result = this.restClient.Get<SolexPayCalculatedValue>(request);

            return new WartoscLiczbowa(result.Data.value.amount, result.Data.value.currency);
        }

        public virtual ProviderPlatnosciOnline Provider => ProviderPlatnosciOnline.SolexPay;


        public string SendApplicationBase(SolexPayRootObject paymentModel)
        {
            RestRequest request = new RestRequest(applicationUrl, Method.POST);
            request.AddJsonBody(paymentModel);

            try
            {
                var result = this.restClient.Post(request);

                if (result.StatusCode != HttpStatusCode.Created )
                {
                    _log.Error($"Błędny wniosek do SolexPay - komunikat z pragmy: {result.Content}, zrzut wniosku: {JSonHelper.Serialize(paymentModel)} ");
                    throw new Exception(result.Content);
                }

                return result.Content;
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                throw new Exception("Problem z wyslanie wniosku o odroczoną płatność");
            }
        }

        public HistoriaDokumentuPlatnosciOnline GenerateDocumentOnlinePayment(byte[] plik, DokumentyBll dokument, string mimeType, string fileName, ISolexHelper solex, string adresId, out string hashDodanegoPliku)
        {
            string adresIp = adresId;
            hashDodanegoPliku = SendFile(plik, fileName, mimeType).id;
            return RegisterTransactionInDatabase(hashDodanegoPliku, dokument, adresIp, solex);
        }

        public string SendApplication(DokumentyBll dokument, string shopUrl, string notificationUrl, string fileName, ISolexHelper solex, string hashDodanegoPliku, int defermentDays, out string urlToRedirectClient)
        {
            SolexPayRootObject paymentModel = GenerateSolexPayModel(dokument, shopUrl, notificationUrl, hashDodanegoPliku, fileName, solex, defermentDays);
       
            var basketId = SendApplicationBase(paymentModel).Trim('"');

            urlToRedirectClient = this.generateUrlTORedirectClient(basketId);

            return basketId;
        }

        private string generateUrlTORedirectClient(string basketId)
        {
            if (this.solexPayConfig.UseTestGateway)
            {
            return $"https://form-partner.box.pragmago.tech/odroczone-terminy-platnosci/krok1?utm_source=www&utm_medium=koszyk&utm_campaign=odroczonaplatnosc&utm_term=p-152577&utm_content=c-zaplac&recommendation_code=152577&basketDraftId={basketId}";
            }

            return $"https://online.pragmago.pl/odroczone-terminy-platnosci/krok1?utm_source=www&utm_medium=koszyk&utm_campaign=odroczonaplatnosc&utm_term=p-152577&utm_content=c-zaplac&recommendation_code=152577&basketDraftId={basketId}";
        }


        public SolexPayRootObject GenerateSolexPayModel(DokumentyBll dokument, string shopUrl, string notificationUrl, string token, string fileName, ISolexHelper solexHelper, int defermentDays)
        {
            if (dokument.WalutaId == null)
            {
                throw new Exception($"Obiekt dokumentu nie ma uzupełnionej waluty.");
            }

            var walutaObiekt = SolexBllCalosc.PobierzInstancje.Konfiguracja.SlownikWalut[dokument.WalutaId.Value];

            SolexPayRootObject rootObject = new SolexPayRootObject();
            rootObject.partnerOrderId = $"{token}";
            rootObject.comment = $"{dokument.NazwaDokumentu}";
            rootObject.transferTitle = $"{dokument.NazwaDokumentu}";
            rootObject.notificationUrl = notificationUrl;
            rootObject.shopUrl = shopUrl;
            rootObject.value = new SolexPayValue
            {
                amount = dokument.DokumentWartoscBrutto.Wartosc,
                currency = dokument.DokumentWartoscBrutto.Waluta
            };

            rootObject.defermentDays = defermentDays;

            rootObject.applicant = new SolexPayApplicant()
            {
                nip = solexHelper.AktualnyKlient.Nip.Replace("-", ""),
                email = solexHelper.AktualnyKlient.Email,
            };

            if (!string.IsNullOrEmpty(solexHelper.AktualnyKlient.Telefon) && ConvertPhoneNumberToString(solexHelper.AktualnyKlient.Telefon, out string telefon) && telefon.Length >= 9 && telefon.Length <= 15)
            {
                rootObject.applicant.phone = telefon;
            }

            if (dokument.Id < 0)
            {
                rootObject.invoicePaymentTerm = DateTime.Now.AddDays(3).ToString("yyyy-mm-dd");
            }
            else
            {
                rootObject.invoicePaymentTerm = dokument.TerminPlatnosci?.ToString("yyyy-mm-dd") ?? dokument.DataUtworzenia.AddDays(3).ToString("yyyy-mm-dd");
            }

            rootObject.receiver = new SolexPayReceiver
            {
                nip = this.solexPayConfig.SellerNIP.Replace("-", ""),
                email = this.solexPayConfig.SellerEmail
            };

            if (!string.IsNullOrEmpty(this.solexPayConfig.SellerPhone) && ConvertPhoneNumberToString(this.solexPayConfig.SellerPhone, out telefon) && telefon.Length >= 9 && telefon.Length <= 15)
            {
                rootObject.receiver.phone = telefon;
            }

            rootObject.receiverBankAccount = new SolexPayReceiverBankAccount
            {
                iban = walutaObiekt.NrKonta,
            };

            if (!rootObject.receiverBankAccount.iban.StartsWith("PL", StringComparison.InvariantCultureIgnoreCase))
            {
                rootObject.receiverBankAccount.iban = $"PL{rootObject.receiverBankAccount.iban}";
            }

            rootObject.attachments = new List<SolexPayAttachment>
            {
                new SolexPayAttachment()
                {
                    fileId = token,
                    filename = fileName,
                    type = "PURCHASE_FILE_TYPE"
                }
            };

            return rootObject;
        }

        public bool ConvertPhoneNumberToString(string telefon, out string value)
        {
            char[] chars = new char[] { ' ', '-', '+' };
            foreach (char charToRemove in chars)
            {
                while (true)
                {
                    int index = telefon.IndexOf(charToRemove);
                    if (index == -1)
                    {
                        break;
                    }
                    telefon = telefon.Remove(index, 1);
                }
            }

            value = telefon;
            return long.TryParse(telefon, out _);
        }

        public SolexPayUploadResult SendFile(byte[] plik, string fileName, string mime)
        {
            RestRequest request = new RestRequest(fileUploadUrl, Method.POST);
            request.AddFile("upload[file]", plik, fileName, mime);
            request.AddParameter("type", "PURCHASE_FILE_TYPE");
            request.AddParameter("ocr", $"{true}");

            var result = this.restClient.Post<SolexPayUploadResult>(request);

            return result.Data;
        }

        protected HistoriaDokumentuPlatnosciOnline RegisterTransactionInDatabase(string token, DokumentyBll document, string ipAddress, ISolexHelper solexHelper)
        {
            HistoriaDokumentuPlatnosciOnline historia = new HistoriaDokumentuPlatnosciOnline();
            historia.DataOperacji = DateTime.Now;
            historia.IdDokumentu = document.Id;
            historia.Kwota = document.WartoscNalezna;
            historia.MetodaPlatnosci = (int)ProviderPlatnosciOnline.SolexPay;
            historia.NazwaDokumentu = document.NazwaDokumentu;
            historia.IpOperacji= ipAddress;
            historia.PlatnikId = solexHelper.AktualnyKlient.Id;
            historia.Status = StatusPlatnosci.Rozpoczeta;
            //historia.PaymentDescription = document.Name;
            historia.TytulPlatnosci= document.NazwaDokumentu;
            //historia.UniquePaymentKeyForTransaction = document.Name.WygenerujIDObiektuSHAWersjaLong();

            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(historia);

            //     historia. = token;

            //   _documentPaymentProvider.AddPayments(new List<DocumentOnlinePaymentDto>() { historia });
            return historia;
        }

        public SolexPayNotification DecodeNotification(string powiadomienie, string secretKey = null)
        {
            if(secretKey == null)
            {
                secretKey = this._pragmaSecret;
            }

            byte[] hash = GetSha256Code(secretKey);

            // Instantiate a new Aes object to perform string symmetric encryption
            Aes encryptor = Aes.Create();

            encryptor.Mode = CipherMode.ECB;

            // Set key and IV
            byte[] aesKey = new byte[32];
            Array.Copy(hash, 0, aesKey, 0, 32);
            encryptor.Key = aesKey;
            encryptor.IV = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };

            // Instantiate a new MemoryStream object to contain the encrypted bytes
            MemoryStream memoryStream = new MemoryStream();

            // Instantiate a new encryptor from our Aes object
            ICryptoTransform aesDecryptor = encryptor.CreateDecryptor();

            // Instantiate a new CryptoStream object to process the data and write it to the
            // memory stream
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptor, CryptoStreamMode.Write);

            // Will contain decrypted plaintext
            string plainText = String.Empty;

            try
            {
                // Convert the ciphertext string into a byte array
                byte[] cipherBytes = Convert.FromBase64String(powiadomienie);

                // Decrypt the input ciphertext string
                cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);

                // Complete the decryption process
                cryptoStream.FlushFinalBlock();

                // Convert the decrypted data from a MemoryStream to a byte array
                byte[] plainBytes = memoryStream.ToArray();

                // Convert the decrypted byte array to string
                plainText = Encoding.ASCII.GetString(plainBytes, 0, plainBytes.Length);
            }
            finally
            {
                // Close both the MemoryStream and the CryptoStream
                memoryStream.Close();
                cryptoStream.Close();
            }

            // Return the decrypted data as a string
            if (string.IsNullOrEmpty(plainText))
            {
                throw new Exception("Problem podczas rozkodowywania powiadomienia");
            }

            return JSonHelper.Deserialize<SolexPayNotification>(plainText);
        }

        protected byte[] GetSha256Code(string secretKey)
        {
            SHA256 mySHA256 = SHA256Managed.Create();
            return mySHA256.ComputeHash(Encoding.ASCII.GetBytes(secretKey));
        }

        private int minValuePLN = 100;
        private int minValueEUR = 22;

        private string[] suportedCurrecies = new[] { "PLN", "EUR" };


        public bool CheckIfClientIsBlocked(IKlient client)
        {
            if (string.IsNullOrEmpty(client.Nip))
            {
                return true;
            }

            return false;

        //    string nipClient = client.Nip.Replace("-", "");

         //   SolexBllCalosc.PobierzInstancje.DostepDane.DbORM.

            //TODO: zmienic na check prosty
       //     return _solexPayLimit.CheckIfClientIsBlockedSolexPay(nipClient);
        }

        public bool CheckIfClientIsFromPoland(IKlient client)
        {
            var clientCountry = client.DomyslnyAdres?.KrajSymbol;
            if (!string.IsNullOrEmpty(clientCountry))
            {
                return clientCountry.Equals("PL", StringComparison.InvariantCultureIgnoreCase);
            }

            if (!client.Eksport && !client.KlientEu && client.IndywidualnaStawaVat == null)
            {
                return true;
            }

            return false;
        }

        public bool CheckDocumentIfCanBePaid(WartoscLiczbowa valueToPay, IKlient client, out string[] errors)
        {
            if (!suportedCurrecies.Contains(valueToPay.Waluta) || !CheckIfClientIsFromPoland(client) || CheckIfClientIsBlocked(client))
            {
                //wentur tak zaprojketowal ze tutaj sprawdzamy duze ograniczenia dla klienta - bez zwracanego bledu sprawdzamy wszystkie dokumenty wstepnie po prostu
                errors = Array.Empty<string>();
                return false;
            }

            List<string> bledy = new List<string>();
            var limit = valueToPay.Waluta == "PLN" ? minValuePLN : minValueEUR;
            if (valueToPay < limit)
            {
                bledy.Add($"Wybrany sposób płatności ma minimalną wartość zamówienia która wynosi: {limit} {valueToPay.Waluta}");
            }

            bool bladDanych = false;

            List<string> bledyDoLogow = new List<string>();
            if (string.IsNullOrEmpty(this.solexPayConfig.SellerNIP))
            {
                bledyDoLogow.Add("Nie uzupełniony nip właściciela");
                bladDanych = true;
            }

            if (string.IsNullOrEmpty(this.solexPayConfig.SellerEmail))
            {
                bledyDoLogow.Add("Nie uzupełniony email właściciela");
                bladDanych = true;
            }


            if (string.IsNullOrEmpty(this.solexPayConfig.SellerPhone) || !ConvertPhoneNumberToString(this.solexPayConfig.SellerPhone, out _))
            {
                bledyDoLogow.Add("Nie uzupełniony numer telefonu właściciela lub jest on niepoprawny");
                bladDanych = true;
            }

            if (bledyDoLogow.Any())
            {
                _log.Error($"Błędne dane dla providera SolexPay:\r\n {string.Join(".\r\n", bledyDoLogow)}");
            }

            if (bladDanych)
            {
                bledy.Add("Nie poprawne konfiguracjne, dokładne dane w logach platformy");
            }

            errors = bledy.ToArray();
            return true;
        }

        private string calculatedDefermentUrl = "/api/calculator/deferred_payment";
        private string fileUploadUrl = "api/partner/attachment/upload";
        private string applicationUrl = "api/basket_integration/DP";
    }
}