using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web.Http;
using System.Web.Mvc;
using ExcelLibrary.CompoundDocumentFormat;
using ServiceStack.Common;
using ServiceStack.Common.Utils;
using ServiceStack.OrmLite;
using ServiceStack.ServiceHost;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.Api;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.Importy.Eksporty;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.SolexPay;
using SolEx.Hurt.SolexPay.Interfaces;
using SolEx.Hurt.SolexPay.Model;
using SolEx.Hurt.Web.Site2.Helper;
using SolEx.Hurt.Web.Site2.Models;
using SolEx.Hurt.Web.Site2.Models.DaneDoWidokow.Dokumenty;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Controllers
{
    [System.Web.Mvc.RoutePrefix("PlatnosciOnline")]
    public class PlatnosciOnlineController : SolexControler
    {
        public PlatnosciOnlineController()
        {
            SolexPayConfig config = new SolexPayConfig()
            {
                SellerEmail = "solexpay@bioplanet.pl",
                SellerNIP = "5862160738",
                SellerPhone = "+48227312870",
                UseTestGateway = false
            };
            solexPayLogic = new SolexPayLogic(SolexBllCalosc.PobierzInstancje.Log, config );
        }

        private SolexPayLogic solexPayLogic;



        [System.Web.Mvc.Route("SolexPayZaplac14/{idDokument}")]
        public ActionResult SolexPayZaplac14(int idDokument)
        {
            return this.SolexPayZaplac(idDokument, 14);
        }


        [System.Web.Mvc.Route("SolexPayZaplac30/{idDokument}")]
        public ActionResult SolexPayZaplac30(int idDokument)
        {
            return this.SolexPayZaplac(idDokument, 30);
        }


        private ActionResult SolexPayZaplac(int idDokument, int ileDni)
        {
           var document = Calosc.DostepDane.PobierzPojedynczy<DokumentyBll>(idDokument, SolexHelper.AktualnyKlient);

            if (document == null)
            {
                throw new Exception("Problem z pobraniem dokumentu do zapłacenia.");
            }

            //wracamy do referera
            if (Request == null || Request.UrlReferrer == null)
            {
                throw new Exception("Próba nielegalnego płacenia SolEx Pay");
            }

            string returnUrl = Request.UrlReferrer.ToString();

            bool czyJestPDF = Calosc.DokumentyDostep.IstniejeZalacznik(document.Id, "pdf");
            string mimeType = "application/pdf";
            string fileName = null;
            byte[] plikDoWyslania = null;

            if (czyJestPDF)
            {
                //wysalnie PDF do pragmy
                string sciezka = Calosc.DokumentyDostep.PobierzSciezkePliku(document.Id, "pdf");
                fileName = System.IO.Path.GetFileName(sciezka);
                plikDoWyslania = System.IO.File.ReadAllBytes(sciezka);
            }
            else
            {
                //plik CSV z dokumentu
                Csv generatorCSV = new Csv();
                plikDoWyslania = generatorCSV.PobierzDokumentDlaKlienta(document, SolexHelper.AktualnyKlient);
                fileName = generatorCSV.PobierzNazwePliku(document);
                mimeType = "text/csv";
            }

            HistoriaDokumentuPlatnosciOnline onlinePaymentDocument = this.solexPayLogic.GenerateDocumentOnlinePayment(plikDoWyslania, document, mimeType, fileName, this.SolexHelper, this.HttpContext.Request.UserHostAddress, out string hashOfTheAddedDocument);
            string urlNotificationStatus = $"https://bioplanet.pl/platnosciOnline/solexPayNotif/";

            //wracamy do aktualnej strony
            var tokenPragmaId = this.solexPayLogic.SendApplication(document, returnUrl, urlNotificationStatus, fileName, this.SolexHelper, hashOfTheAddedDocument, ileDni, out string urlRedirect);

            //aktualizacja placenia jak dostaniemy ID od pragmy
            onlinePaymentDocument.NumerPlatnosci = tokenPragmaId;
            onlinePaymentDocument.KluczPlatnosci = tokenPragmaId;

            switch (ileDni)
            {
                case 14: onlinePaymentDocument.MetodaPlatnosci = (int)ProviderPlatnosciOnline.SolexPay14; break;
                case 30: onlinePaymentDocument.MetodaPlatnosci = (int)ProviderPlatnosciOnline.SolexPay; break;
            }
            Calosc.DostepDane.AktualizujPojedynczy(onlinePaymentDocument);
         
            return View("SolexPayRedirect", model: urlRedirect);
        }



        // GET: PlatnosciOnline
        [System.Web.Mvc.Route("DanePrzelew")]
        public PartialViewResult DanePrzelew(DokumentPlatnosc[] dokumenty,bool online,string symbolopis)
        {
            if(dokumenty == null || !dokumenty.Any())
            {
                throw new Exception("Brak dokumentów do płacenia");
            }

            TrescBll opis = null;
            if (!string.IsNullOrEmpty(symbolopis))
            {
                opis = Calosc.DostepDane.PobierzPojedynczy<TrescBll>(x => x.Symbol == symbolopis, SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient, false);
            }

            //domyslnie online true
            online = true;

            var model = new ParametryDoPlatnosci(opis, dokumenty.ToList(),  SolexHelper.AktualnyKlient );

            if(dokumenty.Length > 1)
            {
                online = false; //nie mozna placic za kilka dokumentow jednoczesnie
                model.errory = new string[] { "Aby płacić usługą Solex Pay musisz wybrać tylko jeden dokument do zapłaty" };
            }            

            DokumentyBll dokumentPlacony = null;

            foreach (var d in dokumenty)
            {
                dokumentPlacony = Calosc.DostepDane.PobierzPojedynczy<DokumentyBll>(d.Dokument, SolexHelper.AktualnyKlient);

                if(dokumentPlacony == null)
                {
                    throw new Exception($"Nie mozna pobrać dokumentu do płacenia id: {d.Dokument} dla klienta id: {SolexHelper.AktualnyKlient.Id}");
                }

                d.DokumentObiekt = dokumentPlacony;
                if (dokumentPlacony.DokumentWartoscNalezna != d.Wartosc)
                {
                    online = false;
                    model.errory = new string[] { "Aby zapłacić Solex Pay kwotą należności musi być równa należności do zapłaty za cały dokument" };
                }
            }

                //waluta z pierwszego dokumentu zczytujemy
                if (!Calosc.Konfiguracja.SlownikWalut.TryGetValue(dokumentPlacony.WalutaId.Value, out Waluta w))
                {
                    throw new Exception($"Nie mozna pobrać waluty z dokumentu: {dokumentPlacony.Id}, waluta id: {dokumentPlacony.WalutaId.Value}");
                }
                model.Waluta = w;
                            
                //dodawanie sposobow platnosci
                model.juzWTrakciePlacenia = Calosc.DostepDane.Pobierz<HistoriaDokumentuPlatnosciOnline>(null, x => Sql.In( x.IdDokumentu, dokumenty.Select(y=> y.Dokument) ) ).ToArray();

                //czy juz jest w trakcie placenia
                if (online)
                {
                model.ListaFormPlatnosci = new List<FirmaPlaceniaOnline>();
               
                //SOLEX Pay
                var solexPay14 = new FirmaPlaceniaOnline()
                {
                    Nazwa = "SolEx Pay",
                    PodTytul = "14 dni odroczenia",
                    Akcja = nameof(this.SolexPayZaplac14),
                    Aktywna = true,
                    LinkDoPomocy = "https://solexb2b.com/solexPay"
                };

                var solexPay30 = new FirmaPlaceniaOnline()
                {
                    Nazwa = "SolEx Pay",
                    PodTytul = "30 dni odroczenia",
                    Akcja = nameof(this.SolexPayZaplac30),
                    Aktywna = true,
                    LinkDoPomocy = "https://solexb2b.com/solexPay"
                };

                //jesli tylko jeden dokument jest to mozna placic go SolexPayem
                if (!solexPayLogic.CheckDocumentIfCanBePaid(dokumentPlacony.DokumentWartoscNalezna, SolexHelper.AktualnyKlient, out string[] errors))
                {
                    solexPay14.Aktywna = false;
                    solexPay14.Error = errors.First();

                    solexPay30.Aktywna = false;
                    solexPay30.Error = errors.First();
                }

                //koszty
                solexPay14.Koszt = this.solexPayLogic.CalculateDefermentCost(dokumentPlacony.DokumentWartoscNalezna, dokumentPlacony.DokumentWartoscNalezna.Waluta, 14);
                solexPay30.Koszt = this.solexPayLogic.CalculateDefermentCost(dokumentPlacony.DokumentWartoscNalezna, dokumentPlacony.DokumentWartoscNalezna.Waluta, 30);

                model.ListaFormPlatnosci.Add(solexPay14);
                model.ListaFormPlatnosci.Add(solexPay30);
            }


            if (opis == null && !online)
            {
                return null;
            }

            return PartialView("_DanePrzelew", model);
        }

        [System.Web.Mvc.Route("solexPayNotif")]
        [System.Web.Mvc.HttpPost]
        public void solexPayNotif([FromBody] string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                throw new Exception("Brak contentu - coś nie tak");
            }

            try
            {
                    //rozkodowywujemy powiadomienie
                    SolexPayNotification pragmaPowiadomienie = this.solexPayLogic.DecodeNotification(content);
                    DateTime dataAktualizacji = DateTime.Now;

                    //Pobieramy historie platnosci online
                    var historiaOnline = Calosc.DostepDane.DbORM.FirstOrDefault<HistoriaDokumentuPlatnosciOnline>(x => x.KluczPlatnosci == pragmaPowiadomienie.@object.order_id);

                    if (historiaOnline == null)
                    {
                        throw new Exception($"Problem z pobraniem platności dla transakcji: {pragmaPowiadomienie.@object.order_id}");
                    }


                switch (pragmaPowiadomienie.@object.status)
                {
                    case SolexPayStatus.NEW: historiaOnline.Status = StatusPlatnosci.Rozpoczeta; break;
                    case SolexPayStatus.PENDING: historiaOnline.Status = StatusPlatnosci.WToku; break;
                    case SolexPayStatus.ACCEPTED: historiaOnline.Status = StatusPlatnosci.Potwierdzona; break;
                    case SolexPayStatus.REJECTED: historiaOnline.Status = StatusPlatnosci.Odrzucona; break;
                    case SolexPayStatus.AGREED: historiaOnline.Status = StatusPlatnosci.Potwierdzona; break;
                }

                Calosc.DostepDane.AktualizujPojedynczy(historiaOnline);
            }
            catch (Exception ex)
            {
                Calosc.Log.Error($"Problem z aktualizacją statusu płatności dla providera SolexPay. Błąd: {ex.Message}. Content: { content }", ex);
                throw;
            }
        }












        //[System.Web.Mvc.Route("ZaplacOnlineZaDokumenty")]
        //public PartialViewResult ZaplacOnlineZaDokumenty(int[] ids, string suma)
        //{
        //    List<DokumentPlatnosc> dokumenty = new List<DokumentPlatnosc>();
        //    IList<HistoriaDokumentuPlatnosciOnline> historiaPlatnosci = new List<HistoriaDokumentuPlatnosciOnline>();
        //    foreach (var i in ids)
        //    {
        //        DokumentyBll dok = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<DokumentyBll>(i, SolexHelper.AktualnyKlient);
        //        decimal wartosc = Math.Round(dok.DokumentWartoscBrutto.Wartosc,2);
        //        dokumenty.Add(new DokumentPlatnosc { Dokument = dok.Id, DokumentObiekt = dok, Wartosc = wartosc});
        //        DokumentPlatnosciOnline docs = new DokumentPlatnosciOnline(dok) {KluczPlatnosci = suma,Status = StatusPlatnosci.Rozpoczeta};
        //        historiaPlatnosci.Add(docs);
        //    }
        //    string tmp = SumaKontrolna(dokumenty);
        //    if(suma != SumaKontrolna(dokumenty)) throw new Exception("Nielegana próba podjęcia płatności");
        //    var waluty = new HashSet<string>( dokumenty.Select(x => x.DokumentObiekt.walutaB2b) );
        //    if(!waluty.Any()) throw new Exception("Dokumenty nie mają waluty");
        //    if(waluty.Count>1)throw new Exception("Dokumenty mają różne waluty");
        //    SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe(historiaPlatnosci);

        //    if (!Platnosc.TestujPolaczenie()) throw new Exception("Błąd połaczenia z serwisem płatności");
        //    string tytul = dokumenty.Select(x => x.DokumentObiekt.NazwaDokumentu + " (" + x.DokumentObiekt.DokumentWartoscBrutto + ")").Aggregate("", (current, pair) => current +" "+ pair);
        //    string token = Platnosc.RejestrujTransakcie(suma, dokumenty.Sum(x => x.Wartosc), tytul, waluty.FirstOrDefault());
        //    //return Redirect(Platnosc.StronaPlatnosci(token));
        //    return PartialView("_Weryfikacja", new ParametryDoWeryfikacji(suma,dokumenty.Sum(x => x.Wartosc), null, StatusPlatnosci.Rozpoczeta, token));

        //}

        //[System.Web.Mvc.Route("PrzekierowanieDoPlatnosci/{token}")]
        //public ActionResult PrzekierowanieDoPlatnosci(string token)
        //{
        //    return Redirect(Platnosc.StronaPlatnosci(token));
        //}

        //[System.Web.Mvc.Route("ZweryfikujPlatnosc/{kluczDokumentu}")]
        //public PartialViewResult ZweryfikujPlatnosc(string kluczDokumentu)
        //{
        //    string error = "";
        //    var dokumenty = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<HistoriaDokumentuPlatnosciOnline>(null,
        //        x => x.KluczPlatnosci == kluczDokumentu);
        //    int? numerPlatnosci = dokumenty.FirstOrDefault().NumerPlatnosci;
        //    decimal kwotaTr = dokumenty.Sum(x => x.Kwota);
        //   ;
        //    string waluta = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<DokumentyBll>(dokumenty.FirstOrDefault().IdDokumentu, SolexHelper.AktualnyKlient).walutaB2b;

        //    if (numerPlatnosci == null)
        //        return PartialView("_Weryfikacja", new ParametryDoWeryfikacji(kluczDokumentu,kwotaTr, null, StatusPlatnosci.Rozpoczeta, error));

        //    StatusPlatnosci status = Platnosc.SprawdzStatus(kluczDokumentu, kwotaTr, (int)numerPlatnosci, out error, waluta);
        //    return PartialView("_Weryfikacja", new ParametryDoWeryfikacji(kluczDokumentu,kwotaTr, numerPlatnosci, status, error));
        //}

        //[System.Web.Mvc.Route("PotwierdzPlatnosc")]
        //public void PotwierdzPlatnosc(int p24_merchant_id, int p24_pos_id, string p24_session_id, int p24_amount, string p24_currency, int p24_order_id, int p24_method, string p24_statement, string p24_sign)
        //{
        //    string sumaKon =
        //        Platnosc.Klucz(new List<string>
        //        {
        //            p24_session_id,
        //            p24_order_id.ToString(),
        //            p24_amount.ToString(),
        //            p24_currency
        //        });
        //    if(p24_sign!=sumaKon) return;
        //    Platnosc.ZmienStatusPlatnosci(p24_session_id,p24_order_id,p24_statement, p24_method,StatusPlatnosci.Potwierdzona);
        //}

        //[NonAction]
        //private string SumaKontrolna(List<DokumentPlatnosc> docs)
        //{
        //    string sumaId = docs.Sum(x => x.Dokument).ToString();
        //    string data = DateTime.Now.ToString("ddMMyyyy");
        //    string wartosc = docs.Sum(x => x.Wartosc).DoLadnejCyfry()+docs[0].DokumentObiekt.walutaB2b;
        //    string tmp = sumaId + data + SolexHelper.AktualnyKlient.Id + wartosc;
        //    return Tools.PobierzInstancje.GetMd5Hash(sumaId + data + SolexHelper.AktualnyKlient.Id + wartosc);
        //}

    }
}