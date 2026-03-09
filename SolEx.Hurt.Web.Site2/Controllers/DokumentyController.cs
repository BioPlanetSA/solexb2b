using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Web.Site2.AtrybutyMvc;
using SolEx.Hurt.Web.Site2.Helper;
using SolEx.Hurt.Web.Site2.Models;
using SolEx.Hurt.Web.Site2.Models.DaneDoWidokow;
using SolEx.Hurt.Web.Site2.PageBases;
using Klient = SolEx.Hurt.Core.Klient;

namespace SolEx.Hurt.Web.Site2.Controllers
{
    [RoutePrefix("dokumenty")]
    public class DokumentyController : SolexControler
    {
        [Route("Pobierz/{id}/{format}/{parametry?}")]
        [AtrybutyMvc.Authorize]
        public void Pobierz(int id, string format, string parametry = null)
        {
            DokumentyBll dok = SolexBllCalosc.PobierzInstancje.DokumentyDostep.PobierzDokumentIDUwzgledniajacSztuczneZamowienia(id, SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient);
            Encoding enc;
            string nazwa;
            var dane = SolexBllCalosc.PobierzInstancje.DokumentyDostep.PobierzPlik(dok, format, SolexHelper.AktualnyKlient, out enc, out nazwa, parametry);
            WyslijPlik(dane, enc, nazwa);
        }

        [Route("Pokaz/{id}")]
        [AtrybutyMvc.Authorize]
        public PartialViewResult Pokaz(int id)
        {
            DokumentyBll dok = SolexBllCalosc.PobierzInstancje.DokumentyDostep.PobierzDokumentIDUwzgledniajacSztuczneZamowienia(id, SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient);
            return PartialView("_Dokument_" + dok.Rodzaj, dok);
        }

        [Route("PobierzListe")]
        [Route("PobierzListe/{rodzaj}/{odKiedy}/{doKiedy}/{pokazstatus:bool}/{pokazKolumneZrealizowane?}")]
        public PartialViewResult PobierzListe( RodzajDokumentu rodzaj, DateTime odKiedy, DateTime doKiedy, bool pokazstatus=false, bool pokazKolumneZrealizowane = false, string szukanieFraza = "", bool mozliwaPlatnosc = false, bool czysadane = false)
        {
            if (odKiedy == DateTime.MinValue ||  odKiedy > DateTime.Now)
            {
                int zIluDni = 0;
                if (rodzaj == RodzajDokumentu.Zamowienie)
                {
                     zIluDni = SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzWartosc<int>(SolexHelper.AktualnyKlient, TypUstawieniaKlienta.DokumentyZIluDniDomyslniePokazywacZamowienie);
                }
                if (rodzaj == RodzajDokumentu.Faktura)
                {
                     zIluDni = SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzWartosc<int>(SolexHelper.AktualnyKlient, TypUstawieniaKlienta.DokumentyZIluDniDomyslniePokazywacFaktura);
                }
                if (rodzaj == RodzajDokumentu.Oferta)
                {
                     zIluDni = SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzWartosc<int>(SolexHelper.AktualnyKlient, TypUstawieniaKlienta.DokumentyZIluDniDomyslniePokazywacOferta);
                }

                odKiedy = DateTime.Now.AddDays(-zIluDni);
            }
            if (doKiedy == DateTime.MinValue || doKiedy > DateTime.Now)
            {
                doKiedy = DateTime.Now;
            }

            if (SolexHelper.AktualnyKlient.Id == 0)
            {
                throw new Exception("Brak podanego klienta");
            }

            List<DokumentyBll> dok = SolexBllCalosc.PobierzInstancje.DokumentyDostep.PobierzWyfiltrowaneDokumenty(SolexHelper.AktualnyKlient, SolexHelper.AktualnyKlient, rodzaj, odKiedy, doKiedy, szukanieFraza,true, true);

            if (dok.Any())
            {
                DocumentSummary danedoWykresu = rodzaj == RodzajDokumentu.Faktura ? SolexBllCalosc.PobierzInstancje.DokumentyDostep.WygenerujeDaneDoWykresuFaktur(dok) : SolexBllCalosc.PobierzInstancje.DokumentyDostep.WygenerujeDaneDoWykresuZamowien(dok);
                return PartialView("_Dokumenty_" + rodzaj, new ParametryDoSzczegolowejListyDokumentow(dok, danedoWykresu, mozliwaPlatnosc,czysadane,pokazstatus, pokazKolumneZrealizowane));
            }
            return PartialView("_Dokumenty_brak");
        }

        [Route("Lista")]
        public PartialViewResult Lista(RodzajDokumentu typ, string platnoscdane, bool platnoscionline, bool pokazstatus, bool pokaztylkoniezrealizowane = false, bool pokazkolumnezrealizowane = false)
        {
            //Potrzeban mi w tym miejscu tylko i wyłącznie ilość dokumentów jakie posiada klient 
            bool saDokumentyDlaKlienta = SolexBllCalosc.PobierzInstancje.DokumentyDostep.CzySaDokumentyDlaKlienta(typ, SolexHelper.AktualnyKlient);
            if (!saDokumentyDlaKlienta)
            {
                return PartialView("_Dokumenty_brak");
            }
            ParametryDoListyDokumentow parametry = ParametryDoListyDokumentow(typ, pokaztylkoniezrealizowane);
            parametry.DaneDoPrzelewu = platnoscdane;
            parametry.PlatnoscOpline = platnoscionline;
            parametry.PokazujStatus = pokazstatus;
            parametry.PokazujKolumneZrealizowane = pokazkolumnezrealizowane;
            parametry.PokazTylkoNiezrealizowane = pokaztylkoniezrealizowane;

            return PartialView("_Lista", parametry);
        }

        [NonAction]
        private ParametryDoListyDokumentow ParametryDoListyDokumentow(RodzajDokumentu typ, bool pokaztylkoniezrealizowane)
        {
            ParametryDoListyDokumentow parametry = new ParametryDoListyDokumentow();
            parametry.Rodzaj = typ;
            if (typ == RodzajDokumentu.Faktura)
            {
                List<DaneDoTabow> taby = new List<DaneDoTabow>();
                DateTime maxDataDlaTrybuPlatnosciFaktury = DateTime.Now.AddYears(-1);
                taby = new List<DaneDoTabow>
                {
                    new DaneDoTabow("#TrybPlatnosci", "Tryb płatności", false, "Pokazuje tylko dokumenty niezapłacone, od najstarszego"),
                    new DaneDoTabow("#TrybPrzegladania", "Tryb Przeglądania", false, "Pokazuje wszystkie dokumenty, od najnowszego")
                };

                //ustalenie maksymalnej daty jaka mozna pokazac w fakturach NIEZAPLACONYCH
                DateTime data = SolexBllCalosc.PobierzInstancje.DostepDane.DbORM.QuerySingle<DateTime>("SELECT Max(DataUtworzenia) FROM HistoriaDokumentu WHERE Zaplacono = 0");
                if (data < maxDataDlaTrybuPlatnosciFaktury)
                {
                    maxDataDlaTrybuPlatnosciFaktury = data;
                }
                parametry.OdKiedyTrybuPlatnosci = maxDataDlaTrybuPlatnosciFaktury;
                parametry.OdKiedyTrybuPrzegladania = DateTime.Now.AddDays(-30);
                parametry.Taby = taby;
                parametry.WybraneTylkoNiezaplacone = SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzWartosc<bool>(SolexHelper.AktualnyKlient, TypUstawieniaKlienta.DokumentyTylkoNiezaplacone);
                parametry.WybraneTylkoPrzeterminowane = SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzWartosc<bool>(SolexHelper.AktualnyKlient, TypUstawieniaKlienta.DokumentyTylkoPrzeterminowane);
            }
            if (typ == RodzajDokumentu.Zamowienie && pokaztylkoniezrealizowane)
            {
                SolexBllCalosc.PobierzInstancje.ProfilKlienta.DodajWartosc(SolexHelper.AktualnyKlient, TypUstawieniaKlienta.DokumentyTylkoNiezrealizowane, true);
                parametry.WybraneTylkoNiezrealizowane = SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzWartosc<bool>(SolexHelper.AktualnyKlient, TypUstawieniaKlienta.DokumentyTylkoNiezrealizowane);
            }



            int zIluDniPobierac = 0;
            switch (typ)
            {
                case RodzajDokumentu.Faktura: zIluDniPobierac = SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzWartosc<int>(SolexHelper.AktualnyKlient, TypUstawieniaKlienta.DokumentyZIluDniDomyslniePokazywacFaktura); break;
                case RodzajDokumentu.Zamowienie: zIluDniPobierac = SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzWartosc<int>(SolexHelper.AktualnyKlient, TypUstawieniaKlienta.DokumentyZIluDniDomyslniePokazywacZamowienie); break;
                case RodzajDokumentu.Oferta: zIluDniPobierac = SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzWartosc<int>(SolexHelper.AktualnyKlient, TypUstawieniaKlienta.DokumentyZIluDniDomyslniePokazywacOferta); break;
            }

            parametry.OdKiedyPobieracDokumenty = DateTime.Now.AddDays(-zIluDniPobierac);
            return parametry;
        }
        [Route("Podsumowanie")]
        public PartialViewResult Podsumowanie(bool ukryjkredytkupiecki)
        {
            IKlient k = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Core.Klient>(SolexHelper.AktualnyKlient.Id);
            DocumentSummary suma = SolexBllCalosc.PobierzInstancje.DokumentyDostep.PobierzPodsumowanieFakturKlient(k);
            if ((suma.Niezaplacone!=null && suma.Niezaplacone.IloscPozycji > 0) || (suma.Przeterminowane!=null && suma.Przeterminowane.IloscPozycji> 0)
                || (SolexHelper.AktualnyKlient.LimitKredytu >0 &&  !ukryjkredytkupiecki))
            {
                return PartialView("_Podsumowanie", new ParametryDoPodsumowaniaDokumentow(k, ukryjkredytkupiecki, suma));
               // return PartialView("_Podsumowanie", new Tuple<DocumentSummary, IKlient>(suma, SolexHelper.AktualnyKlient));
            }
            return null;
        }

        public PartialViewResult Przeterminowane()
        {
           bool saprzeterminowane = SolexBllCalosc.PobierzInstancje.DokumentyDostep.CzyKlientPosiadaJakiesPrzeterminowaneFaktury(SolexHelper.AktualnyKlient.Id);
           return PartialView("_Przeterminowane", saprzeterminowane);
        }
        [Route("ZglosBledneDane")]
        public PartialViewResult ZglosBledneDane(string dokument)
        {
            string email = "";
            string naglowek = string.Empty;
            string stopka = "Dołożyliśmy wszelkich starań, aby powyższe dane były poprawne, jednak nie gwarantujemy, że publikowane informacje nie zawierają błędów, które nie mogę jednak stanowić podstawy do jakichkoliwek roszczeń.";

            
                if (SolexHelper.AktualnyKlient.Opiekun != null && !string.IsNullOrEmpty(SolexHelper.AktualnyKlient.Opiekun.Email))
                {
                    email = SolexHelper.AktualnyKlient.Opiekun.Email;
                }
                if (string.IsNullOrEmpty(email) && SolexHelper.AktualnyKlient.Przedstawiciel != null &&
                    !string.IsNullOrEmpty(SolexHelper.AktualnyKlient.Przedstawiciel.Email))
                {
                    email = SolexHelper.AktualnyKlient.Przedstawiciel.Email;
                }
            
            if (string.IsNullOrEmpty(email)) return null;
            return PartialView("ZglosBlad", new ParametryDoZglosBlad(email, "Błędne dane dokumentu", dokument.Replace("\\", " ").Replace("/", " "), "Zgłoś błędne dane dokumentu",naglowek,stopka));
        }
    }
}