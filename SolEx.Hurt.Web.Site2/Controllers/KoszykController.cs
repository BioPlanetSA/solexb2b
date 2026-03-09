using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.BLL.Web;
using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;
using SolEx.Hurt.Core.Importy;
using SolEx.Hurt.Core.Importy.Koszyk;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Web;
using SolEx.Hurt.Web.Site2.Helper;
using SolEx.Hurt.Web.Site2.Models;
using SolEx.Hurt.Web.Site2.Models.DaneDoWidokow;
using SolEx.Hurt.Web.Site2.Models.DaneDoWidokow.Koszyk;
using SolEx.Hurt.Web.Site2.Modules;
using SolEx.Hurt.Web.Site2.PageBases;
using System.Collections;
using ServiceStack.Common.Extensions;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.Koszyk;
using Klient = SolEx.Hurt.Core.Klient;

namespace SolEx.Hurt.Web.Site2.Controllers
{

    [RoutePrefix("koszyk")]
    public class KoszykController : SolexControler
    {
        private readonly IConfigBLL _config = SolexBllCalosc.PobierzInstancje.Konfiguracja;

        public IKoszykiDostep KoszykiDostep { get; set; } = SolexBllCalosc.PobierzInstancje.Koszyk;

        [Route("KoszykWybor")]
        public PartialViewResult KoszykWybor(bool pokazujusuwanie = false, bool pokazujdaty = false, bool pokazujhistoriezmian = false, TypKoszyka typ = TypKoszyka.Koszyk)
        {
            //jak nie ma licencji to nic nie pokazujemy
            if (!SolexBllCalosc.PobierzInstancje.Konfiguracja.GetLicense(Licencje.Wielokoszykowosc))
            {
                return null;
            }

            int aktualny = SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzWartosc<int>(SolexHelper.AktualnyKlient, TypUstawieniaKlienta.WybranyKoszyk, typ.ToString());
            IList<KoszykBll> koszyki = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<KoszykBll>(SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient, x => x.Typ == typ);
            
            Dictionary<long, KeyValuePair<string, KoszykBll>> slownikKoszykow = new Dictionary<long, KeyValuePair<string, KoszykBll>>();
            foreach (KoszykBll k in koszyki)
            {
                string nazwa;
                if (k.KlienciMogacyAkceptowacKoszyk!=null && k.KlienciMogacyAkceptowacKoszyk.Any())
                {
                    nazwa = $"{SolexBllCalosc.PobierzInstancje.Konfiguracja.PobierzTlumaczenie(k.Klient.JezykId, "Do akceptacji: ")} {k.Klient.Nazwa}";
                }
                else
                {
                    nazwa = string.IsNullOrEmpty(k.Nazwa) ? SolexBllCalosc.PobierzInstancje.Konfiguracja.PobierzTlumaczenie(k.Klient.JezykId,"Domyślny") : k.Nazwa;
                }
                if (k.HistoriaZmianStatusow!=null && k.HistoriaZmianStatusow.Any() && (k.KlienciMogacyAkceptowacKoszyk==null || !k.KlienciMogacyAkceptowacKoszyk.Any()))
                {
                    var nazwaK = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(k.HistoriaZmianStatusow.Last().KlientId);
                    nazwa += $" {SolexBllCalosc.PobierzInstancje.Konfiguracja.PobierzTlumaczenie(k.Klient.JezykId, "Odrzucony przez")} {nazwaK.Nazwa}";
                }
                slownikKoszykow.Add(k.Id, new KeyValuePair<string, KoszykBll>(nazwa,k));
            }
            return PartialView("_KoszykWybor",new DanewyborKoszyka{IdWybranego = aktualny,Typ = typ,PokazujZarzadzanie=pokazujusuwanie,PokazujDaty=pokazujdaty,SlownikKoszykow = slownikKoszykow});
        }

        [Route("WybierzKoszyk/{idKoszyka}")]
        public ActionResult WybranieKoszyka(long idKoszyka)
        {           
            SolexBllCalosc.PobierzInstancje.ProfilKlienta.DodajWartosc(SolexHelper.AktualnyKlient, TypUstawieniaKlienta.WybranyKoszyk, idKoszyka, TypKoszyka.Koszyk.ToString());
            return AkcjaPowrotu();
        }

        [Route("UsunKoszyk/{idKoszyka}")]
        public ActionResult UsunKoszyk(long idKoszyka)
        {
            SolexBllCalosc.PobierzInstancje.DostepDane.UsunPojedynczy<KoszykBll>(idKoszyka);

            //kontrolowane pobierani nowego koszyka -zeby profil sie odswierzyl
            //var a = SolexHelper.AktualnyKoszyk;
            //SolexBllCalosc.PobierzInstancje.Koszyk.PobierzKoszykWgTypu(SolexHelper.AktualnyKlient, TypKoszyka.Koszyk);

            return AkcjaPowrotu();
        }

        [Route("DodajKoszyk")]
        public ActionResult DodajKoszyk(string nazwa)
        {
            long idnowego = SolexBllCalosc.PobierzInstancje.Koszyk.StworzNowyKoszyk(SolexHelper.AktualnyKlient, nazwa);
            SolexBllCalosc.PobierzInstancje.ProfilKlienta.DodajWartosc(SolexHelper.AktualnyKlient, TypUstawieniaKlienta.WybranyKoszyk, idnowego, TypKoszyka.Koszyk.ToString());
            return AkcjaPowrotu();
        }

        [Route("Calosc")]
        public PartialViewResult Calosc(int Id)
        {
            string PoprzedniaStrona = "";
            if (Request.UrlReferrer != null && Request.UrlReferrer.OriginalString.IndexOf("pk",StringComparison.InvariantCultureIgnoreCase)>=0)
            {
                PoprzedniaStrona = Request.UrlReferrer.OriginalString;

            }
            if (string.IsNullOrEmpty(PoprzedniaStrona))
            {
                //powrot do listy produktow
                PoprzedniaStrona = Url.LinkProdukty(SolexHelper.AktualnyJezyk);
            }
            //param.PoprzedniaStrona = url;
            return PartialView("_Calosc", new Tuple<int,string>(Id, PoprzedniaStrona));
        }

        [Route("CzescDynamiczna")]
        public PartialViewResult CzescDynamiczna(ParametryKoszyka parametry)
        {
            ModelState.Clear();
            ParametryKoszyka pars = PobierzParametryKoszyka(SolexHelper.AktualnyKlient, SolexHelper.AktualnyJezyk.Id, parametry, SolexHelper.AktualnyKoszyk);

            SzablonAkceptacjiPoziomy poziomAkceptacji;
            var czyKoszykDoAkceptacji = pars.KoszykObiekt.CzyKoszykDoAkceptacji(SolexHelper.AktualnyKlient, out poziomAkceptacji);
            pars.Akceptacja = czyKoszykDoAkceptacji;
            if (czyKoszykDoAkceptacji)
            {
                pars.TekstButtonaFinalizacji = "Wyślij koszyk do akceptacji";
                if (poziomAkceptacji.Klienci.Count == 1)
                {
                    string pole = parametry.KontrolkaKoszyka.PoleKontaAkceptujacego;
                    if (!string.IsNullOrEmpty(pole))
                    {
                        var klient = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(poziomAkceptacji.Klienci.First());
                        var prop = typeof(Klient).GetProperty(pole);
                        pars.TekstButtonaFinalizacji = "Wyślij koszyk w celu akceptacji do: " + prop.GetValue(klient);
                    }
                }
            }
            else
            {
                pars.TekstButtonaFinalizacji = "Finalizuj";
            }
            pars.WidoczneCeny = !SolexHelper.AktualnyKlient.StaleUkrywanieCen;
            return PartialView(pars.KoszykObiekt.PobierzPozycje.Any() ? "_CzescDynamiczna" : "_CzescDynamicznaKoszykPusty", pars);
        }

        [Route("AktualizujWymuszonaCenePrzedstawiciela")]
        public void AktualizujWymuszonaCenePrzedstawiciela(decimal? cena, int idPozycji)
        {
            var aktualnyKoszyk = SolexHelper.AktualnyKoszyk;
            if (!SolexBllCalosc.PobierzInstancje.Konfiguracja.GetLicense(Licencje.ZmianaCenPrzedstawiciel) || aktualnyKoszyk.PrzedstawicielKtoryEdytujeKoszykWImieniuKlienta == null)
            {
                throw new Exception("Nie masz uprawnień do tej czynności");
            }
            var poz = aktualnyKoszyk.PobierzPozycje.FirstOrDefault(x => x.Id == idPozycji);
            if (poz == null)
            {
                return;
            }
            if (cena != null && !cena.Equals(poz.WymuszonaCenaNettoPrzedstawiciel))
            {
                poz.WymuszonaCenaNettoPrzedstawiciel = cena;
                SolexBllCalosc.PobierzInstancje.Koszyk.UaktualnijKoszyk(aktualnyKoszyk, true);
            }
        }
        [Route("UstawDodatkoweParametryKoszyka")]
        public PartialViewResult UstawDodatkoweParametryKoszyka(int idKontrolki, object wartosc, bool czyElementJestDodatkowymParametrem, string modul, string symbol)
        {
            var aktualnyKoszyk = SolexHelper.AktualnyKoszyk;
            if (czyElementJestDodatkowymParametrem)
            {
                int idModulu;
                if (int.TryParse(modul, out idModulu))
                {
                    aktualnyKoszyk.DodajDodatkowyParametr(idModulu, symbol, (wartosc as IEnumerable).Cast<string>().ToArray());
                }
            }
            if (wartosc != null)
            {
                DodatkowePolaWKoszyku mod = (DodatkowePolaWKoszyku)Enum.Parse(typeof(DodatkowePolaWKoszyku), modul);
                string wartoscDoUstawienia = (wartosc as IEnumerable).First().ToString();
                switch (mod)
                {
                    case DodatkowePolaWKoszyku.Platnosci:
                        aktualnyKoszyk.PlatnoscId = !string.IsNullOrEmpty(wartoscDoUstawienia) ? (int?) int.Parse(wartoscDoUstawienia) : null;
                        break;
                    case DodatkowePolaWKoszyku.Adresy:
                        aktualnyKoszyk.AdresId = !string.IsNullOrEmpty(wartoscDoUstawienia)  ? (long?)Int64.Parse(wartoscDoUstawienia) : null;
                        break;
                    case DodatkowePolaWKoszyku.DostepneMagazyny:
                        aktualnyKoszyk.MagazynRealizujacy = wartoscDoUstawienia;
                        break;
                    case DodatkowePolaWKoszyku.Dostawy:
                        aktualnyKoszyk.KosztDostawyId = !string.IsNullOrEmpty(wartoscDoUstawienia) ? (int?)int.Parse(wartoscDoUstawienia) : null;
                        break;
                }
            }

            SolexBllCalosc.PobierzInstancje.Koszyk.UaktualnijKoszyk(aktualnyKoszyk, true);
            return CzescDynamiczna(new ParametryKoszyka() { Id = idKontrolki });
        }


        [Route("Przelicz")]
        public PartialViewResult Przelicz(ParametryDoPrzeladowaniaKoszyka parametry)
        {
            ZaktualizujKoszyk(parametry);
            return CzescDynamiczna(new ParametryKoszyka() {Id = parametry.IdKontrolki});

        }

        [NonAction]
        private void ZaktualizujKoszyk(ParametryDoPrzeladowaniaKoszyka parametry)
        {
            var aktualnyKoszyk = SolexHelper.AktualnyKoszyk;
            bool bylaZmiana = false;
            foreach (var par in parametry.Pozycje)
            {
                var poz = aktualnyKoszyk.PobierzPozycje.FirstOrDefault(x => x.Id == par.Id);
                if (poz == null) { continue; }
                if ((poz.Ilosc != par.Ilosc || poz.JednostkaId != par.JednostkaId))
                {
                    poz.Ilosc = par.Ilosc;
                    poz.JednostkaId = par.JednostkaId;
                    bylaZmiana = true;
                }
                if (par.Indywidualizacja != null)
                {
                    foreach (var indywidualizacjaWartosc in par.Indywidualizacja)
                    {
                        var indywidualizacjaPozycji = poz.Indywidualizacja.FirstOrDefault(x => x.IndywidualizacjaID == indywidualizacjaWartosc.IndywidualizacjaID);
                        if (indywidualizacjaPozycji == null)
                        {
                            throw new Exception("Proba dodania indywidualizacji której nie było na pozycji. W razie problemu proszę o kontakt z Pawłem.");
                        }
                        var wartosc = (indywidualizacjaWartosc.Wartosc as IEnumerable).First();
                        if (indywidualizacjaPozycji.Wartosc==null|| !indywidualizacjaPozycji.Wartosc.Equals(wartosc))
                        {
                            indywidualizacjaPozycji.Wartosc = wartosc;
                            bylaZmiana = true;
                        }
                    }
                }
            }
            if (bylaZmiana)
            {
                SolexBllCalosc.PobierzInstancje.Koszyk.UaktualnijKoszyk(aktualnyKoszyk, true);
            }
        }

        [Route("AktualizacjaUwag")]
        public void AktualizacjaUwag(string uwagi)
        {
            var aktualnyKoszyk = SolexHelper.AktualnyKoszyk;
            if (!string.IsNullOrEmpty(aktualnyKoszyk.Uwagi) && aktualnyKoszyk.Uwagi.Equals(uwagi))
            {
                return;
            }
            aktualnyKoszyk.Uwagi = uwagi;
            SolexBllCalosc.PobierzInstancje.Koszyk.UaktualnijKoszyk(aktualnyKoszyk, false);
        }

        [Route("Finalizuj")]
        public PartialViewResult Finalizuj(ParametryDoPrzeladowaniaKoszyka parametry)
        {
            ZaktualizujKoszyk(parametry);

            //dla bezpieczensta jescze raz polcizmy moduly - zeby sie nie okazalo ze nagle blokada finalizacji jest 
            SolexHelper.AktualnyKoszyk.PrzeliczModulyKoszykowe_PobierzKomunikaty();

            if (SolexHelper.AktualnyKoszyk.MoznaFinalizowacKoszyk)
            {
                bool akceptacja;
                KoszykiDostep.FinalizacjaKoszyka(SolexHelper.AktualnyKoszyk, SolexHelper.AktualnyKlient, out akceptacja, SolexHelper.AktualnyKlient, SolexHelper.AktualnyPrzedstawiciel);
                string komunikat = "Twoje zamówienie zostało wysłane";
                if (akceptacja)
                {
                    komunikat = "Twoje zamówienie zostało wysłane do akceptacji";
                }
                return PartialView("_CzescDynamicznaFinalizacja", komunikat);
            }
            return CzescDynamiczna(new ParametryKoszyka() { Id = parametry.IdKontrolki });
        }

        [Route("Odrzuc")]
        public PartialViewResult Odrzuc(ParametryDoPrzeladowaniaKoszyka parametry)
        {
            ZaktualizujKoszyk(parametry);
            KoszykiDostep.Odrzuc(SolexHelper.AktualnyKoszyk, SolexHelper.AktualnyKlient);
            return PartialView("_CzescDynamicznaOdrzucono");
        }

        [ChildActionOnly]
        public PartialViewResult Koszyk(KoszykPodglad kontrolka)
        {
            if (Request.Url != null && Request.Url.LocalPath.Equals(Url.ZbudujLinkKoszyka(SolexHelper.AktualnyJezyk), StringComparison.OrdinalIgnoreCase)) { return null; }
            return PartialView("_Koszyk", kontrolka);
        }

        [Route("podglad/{iloscPozycji}")]
        public ActionResult Podglad(int iloscPozycji)
        {
            if (SolexHelper.AktualnyKoszyk == null || !SolexHelper.AktualnyKoszyk.PobierzPozycje.Any() || iloscPozycji == 0)
            {
                return new EmptyResult();
            }

            List<KoszykPozycje> listaKoszykPozycjaBlls = SolexHelper.AktualnyKoszyk.PobierzPozycje.Take(iloscPozycji).ToList();
            List<ElementyKoszykaPodglad> listaElementyKoszykaPodglads = SolexBllCalosc.PobierzInstancje.Konfiguracja.PolaKoszykaPodglad.ToList();
            bool widoczneceny = !SolexHelper.AktualnyKlient.StaleUkrywanieCen && !SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzWartosc<bool>(SolexHelper.AktualnyKlient,TypUstawieniaKlienta.UkryjCenyHurtowe);

            Tuple<IKoszykiBLL, List<KoszykPozycje>, List<ElementyKoszykaPodglad>,bool> daneDoWidoku =
                new Tuple<IKoszykiBLL, List<KoszykPozycje>, List<ElementyKoszykaPodglad>, bool>(SolexHelper.AktualnyKoszyk, listaKoszykPozycjaBlls, listaElementyKoszykaPodglads, widoczneceny);
            return PartialView("_Podglad", daneDoWidoku);
        }



        [Route("UlubioneZmien/{produkt}")]
        public JsonResult UlubioneZmien(long produkt)
        {
            bool ist;
            OdpowiedzKoszykaDlaPozycji odp = KoszykiDostep.ZmienStatusPozycjiUlubione(SolexHelper.AktualnyKlient, produkt, SolexHelper.AktualnyJezyk.Id, out ist);
            return Json(new OdpowiedzKoszyk() {Odpowiedzi = new List<OdpowiedzKoszykaDlaPozycji>() {odp}, CzyModal = ist}, JsonRequestBehavior.AllowGet);
        }
        
        [Route("InfoDostepnoscZmien/{produkt}")]
        public JsonResult InfoDostepnoscZmien(long produkt)
        {
            bool ist;
            OdpowiedzKoszykaDlaPozycji odp = KoszykiDostep.ZmienStatusPozycjiInfoDostepnosc(SolexHelper.AktualnyKlient, produkt, SolexHelper.AktualnyJezyk.Id, out ist);
            return Json(new OdpowiedzKoszyk() {Odpowiedzi = new List<OdpowiedzKoszykaDlaPozycji>() {odp}, CzyModal = ist}, JsonRequestBehavior.AllowGet);
        }
        
        [Route("UsunPozycjeZKoszyka")]
        public PartialViewResult UsunPozycjeZKoszyka(HashSet<int> pozycje, int idKontrolki)
        {
            KoszykBll koszyk = SolexHelper.AktualnyKoszyk;
            if (koszyk != null)
            {
                if (pozycje == null)
                {
                    koszyk.PobierzPozycje.ForEach(x=>x.Ilosc=0);

                }
                else
                {
                    foreach (int idp in pozycje)
                    {
                        var poz = koszyk.PobierzPozycje.FirstOrDefault(x => x.Id == idp);
                        if (poz != null)
                        {
                            poz.Ilosc = 0;
                        }
                    }
                }
               
                KoszykiDostep.UaktualnijKoszyk(koszyk);
            }
            return CzescDynamiczna(new ParametryKoszyka() { Id = idKontrolki });
        }
        private bool OdwrotnaKolejnoscJednostek
        {
            get
            {
                string klucz = "OdwrotnaKolejnoscJednostek";
                bool? id = SolexBllCalosc.PobierzInstancje.Cache.PobierzChwilowy<bool?>(klucz);
                if (id==null)
                {
                    id = SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzWartosc<bool>(SolexHelper.AktualnyKlient, TypUstawieniaKlienta.SposobSortowaniaJednostek);
                    SolexBllCalosc.PobierzInstancje.Cache.DodajChwilowy(klucz, id);
                }
                return id.Value;
            }
        }
        [ChildActionOnly]
        public PartialViewResult DodawanieProduktuProdukt(ProduktKlienta produkt, bool ukryjJednostki = false, bool ukryjdodawanie = false, bool? dodawanieTekstowe = null)
        {
            if (SolexHelper.AktualnyKlient.Dostep == AccesLevel.Niezalogowani)
            {
                return null;
            }

            string info = "Produkt spoza Twojej oferty standardowej";
            bool pokazujCene;
            if (produkt == null || !SolexBllCalosc.PobierzInstancje.ProduktyBazowe.MoznaDodacDoKoszyka(produkt,out info, out pokazujCene))
            {
                return PartialView("_DodawanieNiemozliwe",info);
            }

            bool pokazjednostki = !SolexBllCalosc.PobierzInstancje.Konfiguracja.UkryjJednostkiMiary && !ukryjJednostki;

            var przyciski = KoszykiDostep.PobierzParametryPrzyciskowDodawania(SolexHelper.AktualnyKoszyk, produkt.Id, TypPozycjiKoszyka.Zwykly, SolexHelper.AktualnyKlient, SolexHelper.AktualnyJezyk.Id, dodawanieTekstowe);
            ParametryIlosciProduktu pi = new ParametryIlosciProduktu(produkt, OdwrotnaKolejnoscJednostek, TypPozycjiKoszyka.Zwykly, ukryjdodawanie ? null : przyciski, pokazjednostki);

            return PartialView("EditorTemplates/ParametryIlosciProduktu", pi);
        }

        /// <summary>
        /// NIE korzystać z tej metody! Przekazywać cały produkt klienta a nie ID longowe!
        /// </summary>
        /// <param name="produktid"></param>
        /// <param name="ukryjJednostki"></param>
        /// <param name="ukryjdodawanie"></param>
        /// <param name="dodawanieTekstowe"></param>
        /// <returns></returns>
        public PartialViewResult DodawanieProduktu(long produktid, bool ukryjJednostki = false, bool ukryjdodawanie = false, bool? dodawanieTekstowe = null)
        {
            ProduktKlienta pr = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktKlienta>(produktid, SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient);
            return DodawanieProduktuProdukt(pr, ukryjJednostki, ukryjdodawanie, dodawanieTekstowe);
        }

        //public PartialViewResult DodawanieProduktuZaPunkty(int produkt, bool ukryjJednostki = false, bool ukryjdodawanie = false, bool? dodawanieTekstowe = null)
        //{
        //    const string tekstprzyciskbrak = "Dodaj do koszyka";
        //    const string tekstprzyciskjest = "W koszyku";
        //    if (SolexHelper.AktualnyKlient.Dostep == AccesLevel.Niezalogowani)
        //    {
        //        return null;
        //    }
        //    ProduktBazowy pb = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktBazowy>(produkt);
        //    ProduktKlienta pr = pb!=null?new ProduktKlienta(pb,SolexHelper.AktualnyKlient):null;
        //    string info;
        //    bool pokazujCene;
        //    SolexBllCalosc.PobierzInstancje.ProduktyBazowe.MoznaDodacDoKoszyka(pr, out info, out pokazujCene);
        //    if (pr == null || !pokazujCene)
        //    {
        //        return PartialView("_DodawanieNiemozliwe");
        //    }
        //    bool pokazjednostki = !SolexBllCalosc.PobierzInstancje.Konfiguracja.UkryjJednostkiMiary && !ukryjJednostki;
        //    var przyciski = KoszykiDostep.PobierzParametryPrzyciskowDodawania(SolexHelper.AktualnyKoszyk,produkt, TypPozycjiKoszyka.ZaPunkty, SolexHelper.AktualnyKlient, SolexHelper.AktualnyJezyk.Id, dodawanieTekstowe, tekstprzyciskbrak, tekstprzyciskjest);
        //    bool odwrotnaKolejnoscJednostek = SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzWartosc<bool>(pr.Klient, TypUstawieniaKlienta.SposobSortowaniaJednostek);
        //    ParametryIlosciProduktu pi = new ParametryIlosciProduktu(pr, odwrotnaKolejnoscJednostek,TypPozycjiKoszyka.ZaPunkty, ukryjdodawanie ? null : przyciski, pokazjednostki);

        //    return PartialView("EditorTemplates/ParametryIlosciProduktu", pi);
        //}

        [HttpPost]
        [Route("DodajDokument")]
        public JsonResult DodajDokument(int id)
        {
            DokumentyBll dok = SolexBllCalosc.PobierzInstancje.DokumentyDostep.PobierzDokumentIDUwzgledniajacSztuczneZamowienia(id, SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient);

            List<KoszykPozycje> pozycje = new List<KoszykPozycje>();
            int idKoszyka = SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzWartosc<int>(SolexHelper.AktualnyKlient, TypUstawieniaKlienta.WybranyKoszyk, TypKoszyka.Koszyk.ToString());
            foreach (var pozycja in dok.PobierzPozycjeDokumentu())
            {
                ProduktKlienta pk = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktKlienta>(pozycja.ProduktId, SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient);
                if (pk == null || !pk.Widoczny || pk.Kategorie.IsEmpty())
                {
                    continue;
                }
                var tmp = new KoszykPozycje
                {
                    ProduktId = pozycja.ProduktId,
                    Ilosc = pozycja.PozycjaDokumentuIlosc,
                    JednostkaId = pk.JednostkaPodstawowa.Id,
                    KoszykId = idKoszyka,
                    ProduktBazowyId = pozycja.ProduktIdBazowy==0?pozycja.ProduktId: pozycja.ProduktIdBazowy
                };
                pozycje.Add(tmp);
            }
            List<IKoszykPozycja> przekroczoneStany, przekroczoneLimity, zmienioneilosci, dodanepozycje;
            List<IProduktKlienta> nowe;
            KoszykBll result = KoszykiDostep.ZmienPozycjeKoszyka(pozycje, SolexHelper.AktualnyKlient, out przekroczoneStany, out przekroczoneLimity, out nowe, out zmienioneilosci, out dodanepozycje,SolexHelper.AktualnyKoszyk);
           OdpowiedzKoszyk response = KoszykiDostep.WygenerujKomunikaty(SolexHelper.AktualnyJezyk.Id,result, przekroczoneStany,przekroczoneLimity, nowe, zmienioneilosci, dodanepozycje, SolexHelper.AktualnyKlient);
            return Json(response);
        }

        [HttpPost]
        [Route("DodajPozycje")]
        public JsonResult DodajPozycje(IList<ParametryIlosciProduktu> pozycje)
        {
            const decimal cena = 1m;
            int iloscMaxPozycjiWKoszyku = SolexBllCalosc.PobierzInstancje.Konfiguracja.MaksymalnaIloscPozycjiWKoszyku;
            List<KoszykPozycje> dane = new List<KoszykPozycje>();
            var iloscPozycji = SolexHelper.AktualnyKoszyk.PobierzPozycje.Count;
            List<OdpowiedzKoszykaDlaPozycji> komunikaty = new List<OdpowiedzKoszykaDlaPozycji>();
         
            foreach (ParametryIlosciProduktu d in pozycje)
            {
                if (iloscPozycji >= iloscMaxPozycjiWKoszyku)
                {
                    var odpowiedz = new OdpowiedzKoszykaDlaPozycji()
                    {
                        Tekst = $"W koszyku można mieć maksymalnie {iloscMaxPozycjiWKoszyku} produktów",
                        Typ = "error"
                    };
                    return Json(new OdpowiedzKoszyk()
                    {
                        Odpowiedzi = new List<OdpowiedzKoszykaDlaPozycji>() { odpowiedz },
                        CzyModal = true
                    });
                }

                if (d.TypPozycji == TypPozycjiKoszyka.ZaPunkty)
                {
                    d.WymuszonaCenaNettoModul = cena;
                }

                KoszykPozycje pozycja = new KoszykPozycje(d);
                pozycja.KoszykId = SolexHelper.AktualnyKoszyk.Id;
                pozycja.Klient = SolexHelper.AktualnyKlient;

                string kom = pozycja.Produkt.PopupKomunikat;

                if (!string.IsNullOrEmpty(kom))
                {
                    komunikaty.Add(new OdpowiedzKoszykaDlaPozycji() {Tekst = kom});
                }

                kom = pozycja.Produkt.PopupTekst;

                if (!string.IsNullOrEmpty(kom))
                {
                    komunikaty.Add(new OdpowiedzKoszykaDlaPozycji() {Tekst = kom});
                }

                //if (pozycja.Indywidualizacja != null && pozycja.Indywidualizacja.Any())
                //{
                //    foreach (IndywidualizacjaWartosc t in pozycja.Indywidualizacja)
                //    {
                //        if (t.Indywidualizacja == null)
                //        {
                //            t.Indywidualizacja =
                //                pozycja.Produkt.IndiwidualizacjeProduktu.First(x => x.Id == t.IndywidualizacjaID);
                //        }
                //    }
                //}

                dane.Add(pozycja);
                iloscPozycji++;
            }

            List<IKoszykPozycja> przekroczoneStany;
            List<IKoszykPozycja> przekroczoneLimity;
            List<IKoszykPozycja> zmienioneilosci;
            List<IKoszykPozycja> dodane;
            List<IProduktKlienta> nowe;
            KoszykBll result = KoszykiDostep.ZmienPozycjeKoszyka(dane, SolexHelper.AktualnyKlient, out przekroczoneStany, out przekroczoneLimity, out nowe, out zmienioneilosci, out dodane, SolexHelper.AktualnyKoszyk);
            OdpowiedzKoszyk odp = KoszykiDostep.WygenerujKomunikaty(SolexHelper.AktualnyJezyk.Id, result, przekroczoneStany, przekroczoneLimity, nowe, zmienioneilosci, dodane, SolexHelper.AktualnyKlient);

            if (!komunikaty.Any())
            {
                odp.CzyModal = false;
                return Json(odp);
            }
            odp.Odpowiedzi = komunikaty;
            odp.CzyModal = true;
            return Json(odp);
    }
        

        [HttpPost]
        [Route("PobierzIloscPozycjiWKoszyku")]
        public JsonResult PobierzIloscPozycjiWKoszyku()
        {
            OdpowiedzKoszyk odp = new OdpowiedzKoszyk()
            {
                Brutto = Math.Round(SolexHelper.AktualnyKoszyk.PobierzWartoscBrutto(),2),
                Netto = Math.Round(SolexHelper.AktualnyKoszyk.PobierzWartoscNetto(),2),
                Waluta = SolexHelper.AktualnyKoszyk.WalutaKoszyka().WalutaB2b,
                IloscPozycji = SolexHelper.AktualnyKoszyk.PobierzPozycje.Count
            };
            return Json(odp);
        }


        [HttpPost]
        [Route("Indywidualizacja")]
        public PartialViewResult Indywidualizacja(IList<ParametryIlosciProduktu> pozycje)
        {
            string wiadomosc=null;
            string rozmiarZdjecia = SolexBllCalosc.PobierzInstancje.Konfiguracja.RozmiarZdjeciaIndywidualizacja;
            List<ParametrIndywidualizacji> produktydozmiany = new List<ParametrIndywidualizacji>();
            decimal iloscIndywidualizacji = 0;
            //Dodałem sortowanie ze względu na fakt że jak bedziemy chcieli dodac dwie pozycjie jednej 50 sztuk i drugiej 2 to zeby nie było tak ze tej co jest 50 beda wszystkie a tej 2 nie bedzie wogole
            foreach (var p in pozycje.OrderBy(x=>x.Ilosc))
            {
                ProduktKlienta pr = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktKlienta>(p.ProduktId, SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient);
                if (pr == null)
                {
                    continue;
                }
                string info;
                bool pokazujCene;
                if (!SolexBllCalosc.PobierzInstancje.ProduktyBazowe.MoznaDodacDoKoszyka(pr,out info, out pokazujCene))
                {
                    continue;
                }

                iloscIndywidualizacji += p.Ilosc;
                ParametrIndywidualizacji produktInd;
                if (iloscIndywidualizacji > 50)
                {
                    decimal ilosc = p.Ilosc -  Math.Abs(50 - iloscIndywidualizacji);
                    produktInd = new ParametrIndywidualizacji(pr, ilosc, p.JednostkaId.Value, p.TypPozycji);
                    produktydozmiany.Add(produktInd);
                    wiadomosc = "Ilość produktów została ograniczona. Maksymalna ilość produktów indywidualizowanych przy jednokrotnym dodawniu do koszyka wynosi 50";
                    break;
                }
                produktInd = new ParametrIndywidualizacji(pr, p.Ilosc, p.JednostkaId.Value, p.TypPozycji);
                produktydozmiany.Add(produktInd);
            }
            if (produktydozmiany.Count == 0)
            {
                return null;
            }
            return PartialView("_Indywidualizacja", new Tuple<List<ParametrIndywidualizacji>, string,string>(produktydozmiany, wiadomosc, rozmiarZdjecia));
        }

        [ChildActionOnly]
        public PartialViewResult Ulubione(long ProduktId, bool TrybTekstowy = false)
        {
            if (SolexHelper.AktualnyKlient.Dostep == AccesLevel.Niezalogowani)
            {
                return null;
            }

            return PartialView("_Ulubione", new Tuple<long, HashSet<long>, bool>(ProduktId, SolexHelper.AktualnyKlient.IdUlubionych,TrybTekstowy));
        }

        [Route("GratisyWybor/{idKontrolki}")]
        public PartialViewResult GratisyWybor(int idKontrolki)
        {
            KoszykCalosc kontrolkaKoszyka = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<TrescKolumnaBll>(idKontrolki).Kontrolka() as KoszykCalosc;
            if (kontrolkaKoszyka == null)
            {
                throw new Exception($"Kontrolka o id: {idKontrolki}, nie jest kontrolką koszyka");
            }

            var gratisy = KoszykiDostep.PobierzDostepneGratisy(SolexHelper.AktualnyKoszyk, SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient);

            WidocznoscKolumnKoszyk widGratisyPopUp =  new WidocznoscKolumnKoszyk();
            widGratisyPopUp.PokazywacMetkeRodzinowa = kontrolkaKoszyka.PokazywacMetkeRodzinowaKoszykProduktyGratisyPopUp;
            widGratisyPopUp.PokazywacZdjecieProduktu = kontrolkaKoszyka.PokazywacZdjecieProduktuKoszykProduktyGratisyPopUp;
            widGratisyPopUp.PokazywacNazweProduktu = kontrolkaKoszyka.PokazywacNazweProduktuKoszykProduktyGratisyPopUp;
            widGratisyPopUp.PokazywacSymbolProduktu = kontrolkaKoszyka.PokazywacSymbolProduktuKoszykProduktyGratisyPopUp;
            widGratisyPopUp.PokazywacKodKreskowy = kontrolkaKoszyka.PokazywacKodKreskowyProduktuKoszykProduktyGratisyPopUp;
            widGratisyPopUp.PokazywacVat = kontrolkaKoszyka.PokazywacVatCenyKoszykowejGratisPopUp;
            widGratisyPopUp.UkrywacJednoskeMiaryIIlosc = _config.UkryjJednostkiMiary;
            widGratisyPopUp.CenaHurtowa = kontrolkaKoszyka.PokazywacCenaKatalogowaProduktyGratisyPopUp;
            widGratisyPopUp.RozmiarZdjecia = kontrolkaKoszyka.RozmiarZdjeciaWKoszyku;
            ViewBag.GratisyKolumny = widGratisyPopUp;
            ViewBag.IdKontrolki = idKontrolki;
            return PartialView("_GratisyWybor", gratisy);
        }


        [ChildActionOnly]
        [Route("ImportowaniePozycjiKoszyka")]
        public PartialViewResult ImportowaniePozycjiKoszyka(int id)//string[] dostepneimporty, string naglowek="", string stopka="")
        {
            ImportPozycjiKoszyka kontrolka = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<TrescKolumnaBll>(id).Kontrolka() as ImportPozycjiKoszyka;
            var dostepneimporty = kontrolka.DostepneImporty;

            if (dostepneimporty == null || !dostepneimporty.Any())
            {
                return null;
            }

            ImportowaniePozycjiKoszykaModel model = new ImportowaniePozycjiKoszykaModel();
            model.NaglowekStopka = new DaneNaglowekStopka(kontrolka.Naglowek, kontrolka.Stopka);

            string wybrany = SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzWartosc<string>(SolexHelper.AktualnyKlient,TypUstawieniaKlienta.KoszykOstatnioWybranyImportPozycji);
            if (string.IsNullOrEmpty(wybrany))
            {
                wybrany = dostepneimporty.FirstOrDefault();
            }
            var aktywneFormty = _config.AktywneFormatyImportowaniaPlikow;
            foreach (string m in dostepneimporty)
            {
                OpisImportera opisImportera;
                if (aktywneFormty.TryGetValue(m, out opisImportera))
                {
                    string nazwa = _config.PobierzTlumaczenie(SolexHelper.AktualnyJezyk.Id, opisImportera.Nazwa);
                    opisImportera.CzyWybrany = m == wybrany;
                    model.Lista.Add(nazwa, opisImportera);//new OpisImportera { CzyWybrany = m == wybrany, OpisTypu = m, PlikWzorcowy = imp.Item2});
                }
                else return PartialView("Tekst", $"Brak pliku wzorcowego dla importera: {m}");
            }
            return PartialView("_ImportowaniePozycjiKoszyka", model);
        }
        [Route("ImportPozycjiZPolaTextowewgo")]
        public PartialViewResult ImportPozycjiZPolaTextowewgo(string naglowek = "", string stopka = "")
        {
            ImportZPolaTextowego imp = new ImportZPolaTextowego();
            ImportowaniePozycjiKoszykaModel model = new ImportowaniePozycjiKoszykaModel();
            model.NaglowekStopka = new DaneNaglowekStopka(naglowek, stopka);

            string nazwa = _config.PobierzTlumaczenie(SolexHelper.AktualnyJezyk.Id, imp.LadnaNazwa);
            model.Lista.Add(nazwa, new OpisImportera {  OpisTypu = imp.GetType().PobierzOpisTypu()});
            return PartialView("_ImportPozycjiKoszykaZPolaTextowego", model );
        }

        /// <summary>
        /// Metoda javascript służąca do uploadu plików w module importu pozycji do koszyka
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Upload")]
        public JsonResult Upload()
        {
            HttpPostedFileBase file = Request.Files[0];
            string provider = Request.Form["DropDownSelect"];
            Type typ = Type.GetType(provider);
            List<Komunikat> result = new List<Komunikat>();
            OdpowiedzKoszyk odpKoszyka = null;
            if (file != null)
            {
                //zapisujemy plik w katalogu
                string katalogZapisu = Url.PobierzSciezkePlikUsera(typ, DateTime.Now.ToString("yy-MM-dd HH-mm-ss") + SolexHelper.AktualnyKlient.Id, file.FileName, false);
                new UploadPlikow().ZapiszPlik(file, katalogZapisu);
                string sciezkaZewnetrzna = Url.PobierzSciezkePlikUsera(typ, DateTime.Now.ToString("yy-MM-dd HH-mm-ss") + SolexHelper.AktualnyKlient.Id, file.FileName, true);
                SolexBllCalosc.PobierzInstancje.Statystyki.DodajZdarzenie(ZdarzenieGlowne.ImportPliku, "Link do pliku", sciezkaZewnetrzna, SolexHelper.AktualnyKlient);

                string zawartosc = Tools.PobierzInstancje.GetContent(file.InputStream);
                result = WczytajPozycje(typ, zawartosc, file, out odpKoszyka);
            }
            //return PartialView("WynikImportu", new DaneDoWynikuImportu(result, file!=null?file.FileName:"", DateTime.Now));

            return Json(new { data = this.PartialViewToString("WynikImportu", new DaneDoWynikuImportu(result, file != null ? file.FileName : "", DateTime.Now)), pozycje = odpKoszyka });
        }

        /// <summary>
        /// Metoda javascript służąca do pobierania danychw module importu pozycji do koszyka
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Import")]
        public ActionResult Import()
        {
            string dane = Request.Form["DataContext"];
            string provider = Request.Form["Provider"];
            Type typ = Type.GetType(provider);
            List<Komunikat> result = new List<Komunikat>();
            OdpowiedzKoszyk odpKoszyka =  null;
            if (!string.IsNullOrEmpty(dane))
            {
                result = WczytajPozycje(typ, dane, null, out odpKoszyka);
            }
            //return PartialView("WynikImportu", new DaneDoWynikuImportu(result, string.Empty, DateTime.Now));
            return Json(new { data = this.PartialViewToString("WynikImportu", new DaneDoWynikuImportu(result, string.Empty, DateTime.Now)), pozycje = odpKoszyka });
        }
        [NonAction]
        private List<Komunikat> WczytajPozycje(Type provider, string zawartosc, HttpPostedFileBase file, out OdpowiedzKoszyk odpKoszyka)
        {
            List<Komunikat> wynik = ManagerImportow.WczytajZamowienie(provider, zawartosc, SolexHelper.AktualnyKlient,SolexHelper.AktualnyKoszyk,out odpKoszyka, (file!=null)?file.InputStream:null);
            return wynik;
        }

        [ChildActionOnly]
        public PartialViewResult Sortowanie()
        {
            List<Sortowanie> sortowanie = SolexBllCalosc.PobierzInstancje.Konfiguracja.DostepneSortowanieKoszyka;
            Dictionary<string, Sortowanie> sort = SortowanieHelper.PobierzInstancje.PobierzSortowanieZOpisem<ProduktKlienta>(sortowanie);
            if (sort.Count < 2)
            {
                return null;
            }
            Sortowanie wybrane = SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzSortowanie(SolexHelper.AktualnyKlient, TypUstawieniaKlienta.KolumnaSortowaniaKoszykLista);

            return PartialView("_Sortowanie", new DaneDoWidokuSortowania(sort, wybrane, "koszyk"));
        }

        /// <summary>
        /// pobiera parametry aktualnego koszyka klienta
        /// </summary>
        /// <param name="aktualnyKlient"></param>
        /// <param name="aktualnyJezyk"></param>
        /// <param name="paramtery"></param>
        /// <returns></returns>
        [NonAction]
        public ParametryKoszyka PobierzParametryKoszyka(IKlient aktualnyKlient, int aktualnyJezyk, ParametryKoszyka paramtery, KoszykBll koszyk)
        {
            var pozycje = koszyk.PobierzPozycje;
            if (pozycje.Any(x => x.TypPozycji == TypPozycjiKoszyka.Gratis))
            {
                var gratisy = SolexBllCalosc.PobierzInstancje.Koszyk.PobierzDostepneGratisy(koszyk, aktualnyJezyk, aktualnyKlient).Select(x => x.Item1).ToArray();
                for (int j = 0; j < pozycje.Count; j++)
                {
                    if (pozycje[j].TypPozycji == TypPozycjiKoszyka.Gratis)
                    {
                        IProduktKlienta pk = gratisy.FirstOrDefault(x => x.Id == pozycje[j].ProduktId);
                        if (pk != null)
                        {
                            var tmp = new KoszykPozycjaWyliczonaCenaBLL(pozycje[j], pk);
                            pozycje[j] = tmp;
                        }
                    }
                }
            }

            Komunikat[] komunikaty = koszyk.PrzeliczModulyKoszykowe_PobierzKomunikaty();
            var pars = new ParametryKoszyka(paramtery) { Finalizacja = koszyk.MoznaFinalizowacKoszyk };
            pars.ModulyGratisow = PobierzModulyGratisow(koszyk);
            if (!pars.ModulyGratisow.Any() && pozycje.Any(x => x.TypPozycji == TypPozycjiKoszyka.Gratis))
            {
                foreach (var poz in pozycje)
                {
                    if (poz.TypPozycji == TypPozycjiKoszyka.Gratis)
                    {
                        poz.Ilosc = 0;
                    }
                }
                SolexBllCalosc.PobierzInstancje.Koszyk.UaktualnijKoszyk(koszyk);

                return PobierzParametryKoszyka(aktualnyKlient, aktualnyJezyk, paramtery, koszyk);
            }
var platnosci = SolexBllCalosc.PobierzInstancje.Koszyk.DostepneSposobyPlantosci(koszyk);
            var dostawy = SolexBllCalosc.PobierzInstancje.Koszyk.DostepneSposobyDostawy(koszyk);
            

            //TODO: ale syf w parametrach wysylamy wrazliwe dane! zabic wszystkich

            pars.SlownikParametrow = koszyk.DodatkoweParametry;

            //konieczny warunek sprawdzenia czy jest błąd ze względu na fakt iż podczas pobierania dostawy otrzymując błąd np z upsa to cena bedzie 0 dlatego nie możemy jej pokazac jako sposób dostawy jedynie bedzie komunikat o błędzie pobierania
            List<ParametrModulu<int, string>> dos = new List<ParametrModulu<int, string>>();
            foreach (var sposobDostawy in dostawy)
            {
                var wartosc = sposobDostawy.PobierzOpis(koszyk);
                if (sposobDostawy.CzyWystapilBlad)
                {
                    continue;
                }
                dos.Add(new ParametrModulu<int, string>() { Klucz = sposobDostawy.Id, Wartosc = wartosc });
            }
            if (dos.Count == 1)
            {
                koszyk.KosztDostawyId = dos.First().Klucz;
            }
            pars.Dostawy = dos.ToArray();

            pars.Platnosci = platnosci.Select(x => new ParametrModulu<int, string> { Klucz = x.Id, Wartosc = x.PobierzOpis(koszyk) }).ToArray();
            pars.PolaWlasneKoszyka = DostepnePolaWlasne(koszyk);
            pars.DostepneMagazyny = SolexBllCalosc.PobierzInstancje.Koszyk.PobierzDostepneMagazyny(koszyk, aktualnyKlient);
            pars.KoszykObiekt = koszyk;
            if (pars.DostepneMagazyny != null && pars.DostepneMagazyny.Count == 1)
            {
                pars.KoszykObiekt.MagazynRealizujacy = pars.DostepneMagazyny.First();
            }
            DostepneAdresy(pars, koszyk);
            SprawdzPoprawnosc(pars, koszyk);

            DodajKomunikaty(pars, komunikaty, dostawy.Where(x=>x.CzyWystapilBlad).ToList());
            return pars;
        }





        [NonAction]
        private ParametrModulu<int, Komunikat>[] PobierzModulyGratisow(IKoszykiBLL koszyk)
        {
            List<ParametrModulu<int, Komunikat>> wynik = new List<ParametrModulu<int, Komunikat>>();
            var tmp = SolexBllCalosc.PobierzInstancje.ZadaniaBLL.PobierzZadaniaCalegoKoszykaKtorePasuja<IModulKoszykGratisy>(koszyk).ToArray();
            foreach (var t in tmp)
            {
                IModulKoszykGratisy m = (IModulKoszykGratisy)t;
                var kom = m.KomunikatWarunku(koszyk);
                if (kom == null)
                {
                    continue;
                }
                wynik.Add(new ParametrModulu<int, Komunikat> { Klucz = t.Id, Wartosc = kom });
            }
            return wynik.ToArray();
        }
        [NonAction]
        private void DostepneAdresy(ParametryKoszyka pars, IKoszykiBLL koszyk)
        {
            if (!pars.KoszykObiekt.AdresId.HasValue)
            {
                var adresWysylki = koszyk.Klient.Adresy.FirstOrDefault(x => x.TypAdresu == TypAdresu.Wysylki);
                if (adresWysylki != null)
                {
                    pars.KoszykObiekt.AdresId = adresWysylki.Id;
                }
            }

            var idAdresuWKontrolce = pars.KoszykObiekt.AdresId;
            var adresy = koszyk.Klient.Adresy.Where(x => x.TypAdresu != TypAdresu.Jednorazowy).ToList(); //pobieram wszystkie nie jednorazowe
            var adres = koszyk.Klient.Adresy.FirstOrDefault(x => x.TypAdresu != TypAdresu.Jednorazowy && x.Id == idAdresuWKontrolce); //tylko jednorazowy oraz ten zaznaczony w kontrolce

            if (adres != null)
            {
                adresy.Add(adres);
            }

            pars.Adresy = adresy.Distinct().Select(x => new ParametrModulu<long, string> { Klucz = x.Id, Wartosc = x.ToString() }).ToArray();

            //pars.Adresy = koszyk.Klient.Adresy.Select(x => new ParametrModulu<int, string> { Klucz = x.Id, Wartosc = x.ToString()}).ToArray();
        }
        [NonAction]
        private DodatkowePolaKoszykaPogrupowane[] DostepnePolaWlasne(IKoszykiBLL koszyk)
        {
            var tmp = SolexBllCalosc.PobierzInstancje.ZadaniaBLL.PobierzZadaniaCalegoKoszykaKtorePasuja<IPoleWlasneKoszyka>(koszyk).ToArray();
            Dictionary<PozycjaNaWidokuKoszyka, List<DodatkowePoleKoszyka>> slownik = new Dictionary<PozycjaNaWidokuKoszyka, List<DodatkowePoleKoszyka>>();
            for (int i = 0; i < tmp.Length; i++)
            {
                IPoleWlasneKoszyka m = (IPoleWlasneKoszyka)tmp[i];
                DodatkowePoleKoszyka pole = new DodatkowePoleKoszyka();
                pole.Symbol = m.Symbol;
                pole.Opis = m.NazwaDlaKlienta;
                pole.WartosciMozliwe = m.PobierzOpcje();
                pole.IdModulu = tmp[i].Id;
                pole.TypPola = m.TypPola;
                pole.Wymagane = m.Wymagane;
                pole.MultiWybor = m.Multiwybor;
                pole.Komunikat = tmp[i].Komunikat;
                var wartosci = koszyk.PobierzDodatkowyParemetr(pole.IdModulu);
                if (wartosci != null)
                    pole.WybraneWartosci = wartosci.WybraneWartosci;


                if (!slownik.ContainsKey(m.Pozycja))
                {
                    slownik.Add(m.Pozycja, new List<DodatkowePoleKoszyka>());
                }
                slownik[m.Pozycja].Add(pole);
            }
            return slownik.Select(x => new DodatkowePolaKoszykaPogrupowane { Pozycja = x.Key, Pola = x.Value.ToArray() }).ToArray();
            // return wynik.ToDictionary(x=>x.Key,x=>x.Value.ToArray());
        }

        /// <summary>
        /// spradza czy koszyk mozna finalizowac
        /// </summary>
        /// <param name="pars"></param>
        /// <param name="koszyk"></param>
        [NonAction]
        private void SprawdzPoprawnosc(ParametryKoszyka pars, KoszykBll koszyk)
        {
            if (pars.Adresy.Length > 1 && !pars.KoszykObiekt.AdresId.HasValue)
            {
                pars.Finalizacja = false;
                return;//mozna skonczyc sprawdzanie
            }
            if (pars.Platnosci.Length > 1 && !pars.KoszykObiekt.PlatnoscId.HasValue)
            {
                pars.Finalizacja = false;
                return;//mozna skonczyc sprawdzanie
            }
            if (pars.Dostawy.Length > 1 && !pars.KoszykObiekt.KosztDostawyId.HasValue)
            {
                pars.Finalizacja = false;
                return;//mozna skonczyc sprawdzanie
            }
            if (pars.PolaWlasneKoszyka.Any(x => x.Pola.Any(y => y.Wymagane && string.IsNullOrEmpty(y.WybraneWartosciString))))
            {
                pars.Finalizacja = false;
                return;
            }
            if (!SolexBllCalosc.PobierzInstancje.Koszyk.MoznaFinalizowacKoszykPrzezLimity(koszyk))
            {
                pars.Finalizacja = false;
            }
        }
        [NonAction]
        private void DodajKomunikaty(ParametryKoszyka pars, IEnumerable<Komunikat> komunikaty, List<ISposobDostawy>dostawy)
        {
            var lista = new List<Komunikat>(komunikaty);
            if (pars.KoszykObiekt.PobierzPozycje.Any())
            {
                var kd = pars.KoszykObiekt.KosztDostawy();
                if (kd != null && !string.IsNullOrEmpty(kd.Komunikat))
                {
                    lista.Add(new Komunikat(kd.Komunikat, KomunikatRodzaj.info, "", kd.KomunikatPozycja));
                }
                var pl = pars.KoszykObiekt.PlatnoscObiekt;
                if (pl != null && !string.IsNullOrEmpty(pl.Komunikat))
                {
                    lista.Add(new Komunikat(pl.Komunikat, KomunikatRodzaj.info, "", pl.KomunikatPozycja));
                }
            }

            //dodajemy komunikaty błędów z pobierania dostawy
            if (dostawy != null && dostawy.Any())
            {
                foreach (var sposobDostawy in dostawy)
                {
                    string komunikat = $"{sposobDostawy.OpisDostawy} - {sposobDostawy.Komunikat}";
                    lista.Add(new Komunikat(komunikat, KomunikatRodzaj.danger,"",sposobDostawy.KomunikatPozycja));
                }
            }

            pars.Komunikaty = lista.ToArray();
        }

    }
}

