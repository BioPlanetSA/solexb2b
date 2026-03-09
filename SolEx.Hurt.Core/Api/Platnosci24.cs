using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.Api
{
    public class Platnosci24
    {
        private int IdSprzedawcy { get; set; }
        private string KluczCrc { get; set; }

        public string Klucz(List<string> dane )
        {
            dane.Add(KluczCrc);
            string tmp = dane.Aggregate("", (current, t) => current + (t + "|")).TrimEnd('|');
            return Tools.PobierzInstancje.GetMd5Hash(tmp); //IdSprzedawcy + "|" + KluczCrc);
        }

        public Platnosci24(int id, string crc)
        {
            IdSprzedawcy = id;
            KluczCrc = crc;
        }

        private string Token { get; set; }
        private const string AdresPolaczeniaTestowy = "https://sandbox.przelewy24.pl/testConnection";
        private const string AdresRejestracjiTestowy = "https://sandbox.przelewy24.pl/trnRegister";
        private const string AdresWeryfikacjiTestowy = "https://sandbox.przelewy24.pl/trnVerify";
        private const string AdresTransakcjiTestowy = "https://sandbox.przelewy24.pl/trnRequest";

        public string StronaPlatnosci(string token)
        {
            return AdresTransakcjiTestowy + "/" +((string.IsNullOrEmpty(token))?Token:token);
        }

        public bool TestujPolaczenie()
        {
            Dictionary<string, object> parametry = new Dictionary<string, object>();
            parametry.Add("p24_merchant_id", IdSprzedawcy);
            parametry.Add("p24_pos_id", IdSprzedawcy);
            parametry.Add("p24_sign", Klucz(new List<string> { IdSprzedawcy.ToString() }));
            string response = SendApiRequest(AdresPolaczeniaTestowy, parametry);
            Dictionary<string, string> wynik = Convert(response);
            if (wynik.ContainsKey("error"))
            {
                if (wynik["error"] == "0") return true;
                throw new Exception(wynik["errorMessage"]);
            }

            return false;
        }

        //public StatusPlatnosci SprawdzStatus(string kluczDokumentu, decimal kwota, int numerPlatnosci, out string error, string waluta = "PLN")
        //{
        //     if (kwota <= 0) throw new Exception("Błędna kwota tranzakcji");
        //    int doZaplaty = (int)kwota*100;
        //    Dictionary<string, object> parametry = new Dictionary<string, object>();
        //    parametry.Add("p24_merchant_id", IdSprzedawcy);
        //    parametry.Add("p24_pos_id", IdSprzedawcy);
        //    parametry.Add("p24_session_id", kluczDokumentu);
        //    parametry.Add("p24_amount", doZaplaty);
        //    parametry.Add("p24_currency", waluta);
        //  //  parametry.Add("p24_order_id", numerPlatnosci);
        //    parametry.Add("p24_sign", Klucz(new List<string> { kluczDokumentu, numerPlatnosci.ToString(), doZaplaty.ToString(), waluta }));
        //    string response = SendApiRequest(AdresWeryfikacjiTestowy, parametry);
        //    Dictionary<string, string> wynik = Convert(response);
        //    StatusPlatnosci status= StatusPlatnosci.Rozpoczeta;
        //    error = "";
        //    if (wynik.ContainsKey("error"))
        //    {
        //        if (wynik["error"] == "0")
        //        {
        //            status = StatusPlatnosci.Potwierdzona;
        //        }
        //        else
        //        {
        //            status = StatusPlatnosci.Blad;
        //            error = wynik["errorMessage"];
        //        }
        //    }
        //    ZmienStatusPlatnosci(kluczDokumentu, numerPlatnosci,"",null,status);
        //    return status;
        //}

        //public void ZmienStatusPlatnosci(string kluczPlatnosci, int numerPlatnosci, string tytulPlatnosci, int? metoda, StatusPlatnosci status)
        //{
        //    List<HistoriaDokumentuPlatnosciOnline> docs = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<HistoriaDokumentuPlatnosciOnline>(null, x => x.KluczPlatnosci == kluczPlatnosci).ToList();
        //    docs.ForEach(x => x.NumerPlatnosci = numerPlatnosci);
        //    if (string.IsNullOrEmpty(tytulPlatnosci)) docs.ForEach(x => x.TytulPlatnosci = tytulPlatnosci);
        //    if (metoda != null) docs.ForEach(x => x.MetodaPlatnosci = metoda);
        //    docs.ForEach(x => x.Status = status);
        //    SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<HistoriaDokumentuPlatnosciOnline>(docs);
        //}


        public string RejestrujTransakcie(string kluczDokumentu, decimal kwota, string tytul, string waluta="PLN")
        {
            throw new NotImplementedException("Sylwo źle napisal kod - ma to przeniesc do Weba albo do oddzielnego projektu");
           // IKlient klient = null;
           // Jezyk jezyk = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Jezyk>(klient.JezykId);
           // string domena = null;// SesjaHelper.PobierzInstancje.Domena;
           // if (kwota <= 0) throw new Exception("Błędna kwota tranzakcji");
           // int doZaplaty = (int)kwota*100;
           // //string stronaPowrotu = string.Format(domena +"PlatnosciOnline/ZweryfikujPlatnosc?kluczDokumentu={0}",kluczDokumentu);
           // string stronaStatusu = (domena.Contains("localhost")) ? "":domena + "PlatnosciOnline/PotwierdzPlatnosc";
           ////string stronaStatusu = "http://denver.solexb2b.com/PlatnosciOnline/PotwierdzPlatnosc";
           // Dictionary<string, object> parametry = new Dictionary<string, object>();
           // parametry.Add("p24_session_id", kluczDokumentu);
           // parametry.Add("p24_merchant_id", IdSprzedawcy);
           // parametry.Add("p24_pos_id", IdSprzedawcy);
           // parametry.Add("p24_amount", doZaplaty);
           // parametry.Add("p24_currency", waluta);
           // parametry.Add("p24_description", tytul);
           // parametry.Add("p24_client", klient.Nazwa);
           // parametry.Add("p24_address",  klient.DomyslnyAdres.UlicaNr);
           // parametry.Add("p24_zip", klient.DomyslnyAdres.KodPocztowy);
           // parametry.Add("p24_city", klient.DomyslnyAdres.Miasto);
           // parametry.Add("p24_country", klient.DomyslnyAdres.KrajSymbol);
           // parametry.Add("p24_email", klient.Email);
           // parametry.Add("p24_language", jezyk.Symbol.ToLower());
           // parametry.Add("p24_url_return", stronaStatusu);
           // parametry.Add("p24_url_status", stronaStatusu);
           // parametry.Add("p24_api_version", "3.2");
           // parametry.Add("p24_sign", Klucz(new List<string> { kluczDokumentu, IdSprzedawcy.ToString(), doZaplaty.ToString(), waluta }));

           // string response = SendApiRequest(AdresRejestracjiTestowy, parametry);
           // Dictionary<string, string> wynik = Convert(response);
           // if (wynik.ContainsKey("error"))
           // {
           //     if (wynik["error"] != "0") 
           //     throw new Exception(wynik["errorMessage"]);
           // } 
           // if (wynik.ContainsKey("token"))
           // {
           //     if (!string.IsNullOrEmpty(wynik["token"]))
           //     {
           //         Token = wynik["token"];
           //         return wynik["token"];
           //     }
           // }
           // return null;
        }
        private string SendApiRequest(string adres, Dictionary<string, object> postParams)
        {
            try
            {
                WebRequest request = WebRequest.Create(adres);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Timeout = 100000;   //domyslnie 100 sekund
                Stream dataStream = request.GetRequestStream();

                string param = Convert(postParams);
                UTF8Encoding encoding = new UTF8Encoding();
                byte[] byteArray = encoding.GetBytes(param);

                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                WebResponse webResponse = request.GetResponse();
                Stream responseStream = webResponse.GetResponseStream();
                if (responseStream != null)
                {
                    StreamReader reader = new StreamReader(responseStream);
               
                    return reader.ReadToEnd();
                }
                return null;
            }
            catch (Exception ex)
            {
                //dodac obsługe błedu serwera 500
               throw new Exception("Wystąpił błąd z łącznością ze sklepem: " + ex.Message, ex.InnerException);
            }
        }

        private string Convert(Dictionary<string, object> parametry)
        {
          return parametry.Where(x=>x.Value!=null).Select(x => x.Key + "=" + x.Value.ToString()).Aggregate("", (current, pair) => current +"&"+ pair).TrimStart('&');
        }

        private Dictionary<string, string> Convert(string response)
        {
            string[] tmpArray = response.Split('&');
            return tmpArray.Select(s => s.Split('=')).ToDictionary(tmpS => tmpS[0], tmpS => tmpS[1]);
        }

    }
}
