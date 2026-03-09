using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ServiceStack.Common;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Web;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Web;
using SolEx.Hurt.Web.Site2.Helper;
using SolEx.Hurt.Web.Site2.Models;
using SolEx.Hurt.Web.Site2.Models.DaneDoWidokow;
using SolEx.Hurt.Web.Site2.Models.DaneDoWidokow.ProfilKlienta;
using SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.Bazowe;
using SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.SzczegolyProduktu;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Controllers
{
    [RoutePrefix("produkty")]
    public class ProduktyController : SolexControler
    {
        [ChildActionOnly]
        public PartialViewResult UkryjCeny()
        {
            if (SolexHelper.AktualnyKlient.Dostep == AccesLevel.Niezalogowani || SolexHelper.AktualnyKlient.StaleUkrywanieCen)
            {
                return null;
            }
            var ukryjCenyHurtowe = SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzWartosc<bool>(SolexHelper.AktualnyKlient, TypUstawieniaKlienta.UkryjCenyHurtowe);
            ZmianaDanychBool zmianaDanych = new ZmianaDanychBool
            {
                Typ = TypUstawieniaKlienta.UkryjCenyHurtowe,
                Ikona = ukryjCenyHurtowe ? "fa-eye-slash" : "fa-eye",
                Tooltip = ukryjCenyHurtowe ? "Pokaż ceny" : "Ukryj ceny",
                Wartosc = !ukryjCenyHurtowe
            };
            return PartialView("../ProfilKlienta/ZmianaUstawieniaBool", zmianaDanych);
        }
        [ChildActionOnly]
        public PartialViewResult SortowanieJednostek()
        {
            if (SolexHelper.AktualnyKlient.Dostep==AccesLevel.Niezalogowani|| !SolexBllCalosc.PobierzInstancje.Konfiguracja.PokazywanieSortowaniaJednostek || SolexBllCalosc.PobierzInstancje.Konfiguracja.UkryjJednostkiMiary)
            {
                return null;
            }
            var sposobSortowania = SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzWartosc<bool>(SolexHelper.AktualnyKlient,TypUstawieniaKlienta.SposobSortowaniaJednostek);
            ZmianaDanychBool zmianaDanych = new ZmianaDanychBool
            {
                Typ = TypUstawieniaKlienta.SposobSortowaniaJednostek, Ikona = sposobSortowania ? "fa-cube":"fa-cubes" ,
                Tooltip = sposobSortowania ? "Małe opakowania" : "Duże opakowania",Wartosc = !sposobSortowania
            };
            return PartialView("../ProfilKlienta/ZmianaUstawieniaBool", zmianaDanych);
        }

        [ChildActionOnly]
        public PartialViewResult Cena(CenaDoPokazania cena)
        {
            return PartialView("_Cena", cena);
        }

        [ChildActionOnly]
        public PartialViewResult Szukanie(string placeholder)
        {
            var fraza = Request["szukane"];

            long katId;
            return PartialView("_Szukanie", new Tuple<string,bool, string>(fraza, Url.AktualnaStronaToStronaProduktow(out katId), placeholder));
        }

        [Route("dymek")]
        [Route("dymek/{produkt:long}/{ilosc:decimal}/{jednostka:long?}/{typPozycji}/{poprzednio:decimal}/{tryb:int}")]
        public JsonResult Dymek(long produkt, decimal ilosc, long? jednostka, TypPozycjiKoszyka typPozycji, decimal poprzednio = 0, int tryb = 0)
        {
            if(SolexHelper.AktualnyKlient.Id == 0)
            {
                //niezalogowanemu nic nie dajemy - czasem tu wejdzie bo sesje wygasly czy cos
                return null;
            }


            ProduktKlienta p;
            if (typPozycji == TypPozycjiKoszyka.Gratis || typPozycji == TypPozycjiKoszyka.ZaPunkty)
            {
                ProduktBazowy prod = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktBazowy>(produkt);
                p = prod.Widoczny ? new ProduktKlienta(prod, SolexHelper.AktualnyKlient) : null;
            }
            else
            {
                p = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktKlienta>(produkt, SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient);
            }

            if (p == null)
            {
                //nic nie zwracamy - czasem jest uruchaminy dymek przez jakies cache albo gratisy i nie ma dymkow - powinno byc inne zabezpieczenie - ale nie na teraz prosciej zwracac null
                return null;
                //throw  new Exception($"Brak produktu dla id: {produkt}.");
            }

            if (p.Jednostki == null || p.Jednostki.IsEmpty() || p.JednostkaPodstawowa == null)
            {
                throw new Exception($"Brak jednostek dla produktu id: {produkt}.");
            }
            
            var uzupelnioneJednostki = SolexBllCalosc.PobierzInstancje.Koszyk.UzupelnijJednostkiOKrokDodania(p.Jednostki, p);

            JednostkaProduktu jednostkaWybrana = null;
            if (jednostka == null)
            {
                jednostkaWybrana = uzupelnioneJednostki.FirstOrDefault(x=>x.Id == p.JednostkaPodstawowa.Id);
                if (jednostkaWybrana == null)
                {
                   throw new Exception($"Nie można pobrać jednostki podstawowej dla produktu id: {p.Id}, jednostka podstawowa id: {p.JednostkaPodstawowa.Id}. Tabalica jednostek produktu: {uzupelnioneJednostki.Select(x=> x.Id.ToString()).ToList().Join(",")}");
                }
            }
            else
            {
                jednostkaWybrana = uzupelnioneJednostki.First(x => x.Id == jednostka);
                if (jednostkaWybrana == null)
                {
                    throw new Exception($"Nie można pobrać wymaganej jednostki dla produktu id: {p.Id}, jednostka wymagana id: {jednostka}. Tabalica jednostek produktu: {uzupelnioneJednostki.Select(x => x.Id.ToString()).ToList().Join(",")}");
                }
            }
            if (tryb != 0)
            {
                if (tryb > 0 && tryb != 9)
                {
                    ilosc = poprzednio + jednostkaWybrana.Krok;
                }
                if (tryb < 0)
                {
                    ilosc = poprzednio - jednostkaWybrana.Krok;
                }
                // B2B-947 umożliwienie podania wartości do dodania
                if (tryb == 9 && ilosc > 0)
                {
                    ilosc = poprzednio + ilosc;
                }
                if (ilosc < 0) ilosc=0;
            }


            string pw = "";
            decimal iloscStara = ilosc;
            decimal iloscWKoszyku=0;
         
            Dictionary<long, decimal> ilosciKoszyka = SolexHelper.AktualnyKoszyk.PobierzIlosciProduktowWKoszyku();
            if (ilosciKoszyka.ContainsKey(p.Id))
            {
                iloscWKoszyku = ilosciKoszyka[p.Id];
            }
            
            ilosc = SolexBllCalosc.PobierzInstancje.Koszyk.SprawdzIlosc(p, jednostkaWybrana.Id, ilosc, poprzednio, iloscWKoszyku);
            
            if (SolexBllCalosc.PobierzInstancje.Konfiguracja.PokazujDymek)
            {
                decimal? iloscMaksymalnaMozliwa = null;

                if (ilosc > 0 && SolexBllCalosc.PobierzInstancje.Koszyk.CzyJestWlaczonyModulPrzekroczonychStanow)
                {
                    iloscMaksymalnaMozliwa = p.IloscLaczna;

                    //pokazujemy ograniczenie tylko jesli klient przekracza stany
                    if (iloscMaksymalnaMozliwa >= ilosc)
                    {
                        iloscMaksymalnaMozliwa = null;
                    }
                    else
                    {
                        string cacheKlientaInfoOProdukcieZapisWstatystykach = "k{0}p{1}";
                        //prosty cache zabezpieczenie czy w ostanim czasie juz klientowi bylo wysylane info
                        if (!SolexBllCalosc.PobierzInstancje.Cache.PobierzObiekt<bool?>(cacheKlientaInfoOProdukcieZapisWstatystykach, SolexHelper.AktualnyKlient.Id, p.Id).HasValue)
                        {
                            //odnowanie w statach
                            SolexBllCalosc.PobierzInstancje.Statystyki.DodajInfoOPobraniuStanuProduktu(SolexHelper.AktualnyKlient, ilosc, p, iloscMaksymalnaMozliwa.Value);
                            //dodanie cache
                            SolexBllCalosc.PobierzInstancje.Cache.DodajObiekt(cacheKlientaInfoOProdukcieZapisWstatystykach, true, SolexHelper.AktualnyKlient.Id, p.Id);
                        }
                    }
                }

                Tuple<IProduktBazowy, decimal, JednostkaProduktu, decimal?> daneWidoku = new Tuple<IProduktBazowy, decimal, JednostkaProduktu, decimal?>(p, ilosc, jednostkaWybrana, iloscMaksymalnaMozliwa);
                pw = this.PartialViewToString("_Dymek", daneWidoku);
            }

            string dodatkoweInfo = "";
            if (ilosc != iloscStara)
            {
                dodatkoweInfo = SolexBllCalosc.PobierzInstancje.Konfiguracja.PobierzTlumaczenie(SolexHelper.AktualnyJezyk.Id, "Zaokrąglono ilość produktu {0} z {1:0.########} {3} do {2:0.########} {3}", p.Nazwa, iloscStara, ilosc, jednostkaWybrana.Nazwa);
            }

            return Json(new Tuple<string, decimal, string>(pw, ilosc, dodatkoweInfo), JsonRequestBehavior.AllowGet);
        }

        [Route("Gradacje")]
        [ChildActionOnly]
        public PartialViewResult Gradacje(int id)
        {
            GradacjaProduktu kontrolkaGradacja = this.PobierzKontrolke<GradacjaProduktu>(id);
            //czy mamy po czym liczyć gradacje w ogole - czy gradacja jest aktywna
            if (kontrolkaGradacja.SposobPokazywania == GradacjaSposobPokazywania.Brak)
            {
                return null;
            }

            ProduktKlienta p = Calosc.DostepDane.PobierzPojedynczy<ProduktKlienta>(kontrolkaGradacja.ProduktId, SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient);
            //jesli nie ma gradacji do pokazania - bo np. klient juz ma lepsze ceny niz to wynika z gradacji to pokazujemy jako normalna cena
            if (p == null || p.GradacjePosortowane == null || p.FlatCeny.GradacjaUzytaDoLiczeniaCeny_Poziomy == null)
            {
                return null;
            }
            
            var parametryWidoku = new ParametryDoGradacji(p.FlatCeny.GradacjaUzytaDoLiczeniaCeny_Poziomy, p, kontrolkaGradacja);
            parametryWidoku.WyliczonaIloscProduktu = p.FlatCeny.GradacjaUzytaDoLiczeniaCeny_KupioneIlosci;

            if (kontrolkaGradacja.SposobPokazywania == GradacjaSposobPokazywania.Zlozona)
            {
                return PartialView("Produkty/_Gradacje", parametryWidoku);
            }

            if (kontrolkaGradacja.SposobPokazywania == GradacjaSposobPokazywania.IleBrakujeDoNajlepszejCeny)
            {
                return PartialView("Produkty/_GradacjeIleBrakujeDoNajlepszejCeny", parametryWidoku);
            }

            return PartialView("Produkty/_GradacjeProste", parametryWidoku);
        }

        /// <summary>
        /// Generowanie przyciskow do zmiany widokow
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public PartialViewResult TrybyListyProduktow()
        {
            List<TrybListyProduktow> tryby = Core.BLL.Web.TrybyListyProduktow.PobierzInstancje.WszystkieTryby(SolexHelper.AktualnyKlient);
            if (tryby.Count < 2)
            {
                return null;
            }

            return PartialView("_TrybyListyProduktow",tryby);
        }

        [ChildActionOnly]
        public PartialViewResult Sortowanie()
        {
            List<Sortowanie> sortowanie = SolexBllCalosc.PobierzInstancje.Konfiguracja.DostepneSortowanieListyProduktow;
            Dictionary<string, Sortowanie> sort = SortowanieHelper.PobierzInstancje.PobierzSortowanieZOpisem<ProduktKlienta>(sortowanie);
            if (sort.Count <2)
            {
                return null;
            }
            Sortowanie wybrane = SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzSortowanie(SolexHelper.AktualnyKlient, TypUstawieniaKlienta.KolumnaSortowaniaListyProduktow);
           
            return PartialView("_Sortowanie", new DaneDoWidokuSortowania(sort, wybrane, "produkty"));
        }


        /// <summary>
        /// Akcja która renderuje parametry dla produktów - które później ajaxem są czytane i renderowane
        /// </summary>
        [ChildActionOnly]
        [FriendlyName("Kontrolka listy produktów")]
        [Route("Lista")]
        public PartialViewResult Lista(ParametryPrzekazywaneDoListyProduktow parametry)
        {
            return PartialView("_Lista", parametry);
        }


        /// <summary>
        /// Główne pokazywanie produktów
        /// </summary>
        /// <param name="parametry"></param>
        /// <returns></returns>
        [Route("ListaWczytaj")]
        //[ChildActionOnly]
        public PartialViewResult ListaWczytaj(ParametryPrzekazywaneDoListyProduktow parametry)
        {
            if (parametry == null)
            {
                throw new HttpException(401, "Brak parametrów do pokazania listy produktów");
            }


            var staleFiltry = StaleFiltryAktualnieWybrane();
            IList<ProduktKlienta> produktyWybraneZWarunkow = SolexBllCalosc.PobierzInstancje.ProduktyKlienta.ProduktySpelniajaceKryteria(parametry.kategoria, parametry.szukane,
                SolexHelper.AktualnyKlient, SolexHelper.AktualnyJezyk.Id, parametry.filtry, staleFiltry, parametry.szukanaWewnetrzne);

            Sortowanie sortowanie = SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzSortowanie(SolexHelper.AktualnyKlient, TypUstawieniaKlienta.KolumnaSortowaniaListyProduktow);
            int lacznie;
            HashSet<long> rodzinowe;
            int rozmiarStrony = SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzWartosc<int>(SolexHelper.AktualnyKlient, TypUstawieniaKlienta.RozmiarStronyListaProduktow);
            int ilepominac = (parametry.strona - 1) * rozmiarStrony;

            Dictionary<IProduktKlienta, KategorieBLL> produkty;
            
            //ToDo Trzeba dodać obsługę przypadku gdy produkt nie będzie posiadał cechy z atrybutem zaznaczonym w kontrolce
            if ((parametry.KontrolkaProduktowJakoListaProduktow.IdAtrybutow!= null && parametry.KontrolkaProduktowJakoListaProduktow.IdAtrybutow.Any()) && string.IsNullOrEmpty(parametry.szukane))
            {
                produkty = null;
                rodzinowe = null;
                lacznie = 0;
                if (parametry.filtry != null)
                {
                    int a = parametry.KontrolkaProduktowJakoListaProduktow.IdAtrybutow.Count(x => parametry.filtry.ContainsKey(x));
                    if ((parametry.KontrolkaProduktowJakoListaProduktow.SposoobSprawdzeniaFiltrow == SposoobSprawdzeniaFiltrow.PrzynajmniejJeden && a>0) || (parametry.KontrolkaProduktowJakoListaProduktow.SposoobSprawdzeniaFiltrow == SposoobSprawdzeniaFiltrow.Wszystkie && parametry.KontrolkaProduktowJakoListaProduktow.IdAtrybutow.Count == a))
                    {
                        produkty = SolexBllCalosc.PobierzInstancje.ProduktyKlienta.WybierzProduktyDoPokazaniaWgStronyISortowania(parametry.kategoria, 
                            produktyWybraneZWarunkow, SolexHelper.AktualnyKlient,SolexHelper.AktualnyJezyk.Id, sortowanie.Pola, false, ilepominac, rozmiarStrony,out lacznie, out rodzinowe);
                    }
                }
            }
            else
            {
                produkty = SolexBllCalosc.PobierzInstancje.ProduktyKlienta.WybierzProduktyDoPokazaniaWgStronyISortowania
                    (parametry.kategoria, produktyWybraneZWarunkow, SolexHelper.AktualnyKlient, SolexHelper.AktualnyJezyk.Id, sortowanie.Pola, false,ilepominac, rozmiarStrony, out lacznie, out rodzinowe);
            }

            if (this.SolexHelper.AktualnyPrzedstawiciel == null && !this.SolexHelper.AktualnyKlient.Role.Contains(RoleType.Pracownik) && (!string.IsNullOrEmpty(parametry.szukane) || !string.IsNullOrEmpty(parametry.szukanaWewnetrzne)))
            {
                string szukanie = $"{parametry.szukane} {parametry.szukanaWewnetrzne}";
                szukanie = Calosc.Szukanie.UsunZnakiZakazaneZSzukania(szukanie);
                Calosc.Statystyki.LogujDzialanieUzytkownikowAsync(this.SolexHelper.AktualnyKlient, szukanie, ZdarzenieGrupa.Produkty, ZdarzenieGlowne.WyszukiwanieProduktu);
            }

            TrescBll opis = null,kategoriaOpis=null;
            if (parametry.kategoria.HasValue)
            {
                if (parametry.KategoriaObiekt != null)
                {
                    if (!string.IsNullOrEmpty(parametry.KategoriaObiekt.KategoriaTresciSymbol))
                    {
                        opis = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<TrescBll>(x => parametry.KategoriaObiekt.KategoriaTresciSymbol.Equals(x.Symbol, StringComparison.InvariantCultureIgnoreCase), SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient, false);
                    }
                    string symbolKategorii = null;
                    if (!string.IsNullOrEmpty(parametry.KontrolkaProduktowJakoListaProduktow.SymbolOpisKategorii))
                    {
                        symbolKategorii = parametry.KontrolkaProduktowJakoListaProduktow.SymbolOpisKategorii;
                    }
                    else
                    {
                        var s = parametry.KategoriaObiekt.Grupa.SymbolTresciOpisuKategorii;
                        if (!string.IsNullOrEmpty(s))
                        {
                            symbolKategorii = s;
                        }
                    }
                    if (!string.IsNullOrEmpty(symbolKategorii))
                    {
                        kategoriaOpis = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<TrescBll>(x => symbolKategorii.Equals(x.Symbol, StringComparison.InvariantCultureIgnoreCase), SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient, false);
                    }
                }
            }

            var widoki = SolexBllCalosc.PobierzInstancje.Konfiguracja.AktywneWidokiListyProduktow(SolexHelper.AktualnyKlient.Dostep == AccesLevel.Zalogowani);
                string uklad = SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzWartosc<string>(SolexHelper.AktualnyKlient, TypUstawieniaKlienta.SzablonListy);
                if (!widoki.Contains(uklad))
                {
                    uklad = widoki.First();
                    SolexBllCalosc.PobierzInstancje.ProfilKlienta.DodajWartosc(SolexHelper.AktualnyKlient, TypUstawieniaKlienta.SzablonListy, uklad);
                }

            //pobieramy ustawienie czy klient powinien widziec ceny hurtowe
            bool pokazywacCene = !SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzWartosc<bool>(SolexHelper.AktualnyKlient, TypUstawieniaKlienta.UkryjCenyHurtowe);

            //pobieramy ustawienie sortowania jednostek
            bool sortowanieJednostek = SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzWartosc<bool>(SolexHelper.AktualnyKlient, TypUstawieniaKlienta.SposobSortowaniaJednostek);


            //absolutnie nie wolno zmianiac koljenosci kategorii i produktów - obie listy musza być w identycznej kolejności
            ListaProduktowZKategoriam ust = produkty==null?null:new ListaProduktowZKategoriam(produkty.Keys.ToArray(), produkty.Values.ToArray(), rodzinowe);
            ListaDane listaDane = new ListaDane(lacznie, ust, parametry, SolexHelper.AktualnyKlient, opis, kategoriaOpis, parametry.KontrolkaProduktowJakoListaProduktow.OpisProduktowZamiastOpisuKategorii,uklad);
            listaDane.KlientWidziCeneHurtowa = pokazywacCene;
            listaDane.SortowanieJednostek = sortowanieJednostek;
            listaDane.PokazujJednostki = !SolexBllCalosc.PobierzInstancje.Konfiguracja.UkryjJednostkiMiary;
            listaDane.WszystkieSposobyPokazywaniaStanowKlienta = SolexBllCalosc.PobierzInstancje.SposobyPokazywaniaStanowBll.WszystkieSposobyKlienta(SolexHelper.AktualnyKlient, SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyPrzedstawiciel);
            return PartialView("_ListaDane", listaDane);
        }

        /// <summary>
        /// Losowa lista produktów - akcja wykonywana przez kontrolke. Renderuje odrazu liste produktów
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        [Route("WybraneProdukty")]
        public PartialViewResult WybraneProdukty(int id)
        {
            LosowaListaProduktowWybraneIdProduktow kontrolkaWyzwalajaca = Calosc.DostepDane.PobierzPojedynczy<TrescKolumnaBll>(id,SolexHelper.AktualnyJezyk.Id).Kontrolka() as LosowaListaProduktowWybraneIdProduktow;

            var listaproduktowid = kontrolkaWyzwalajaca.ListaProduktowId;

            if (listaproduktowid == null || !listaproduktowid.Any())
            {
                return null;
            }

            Dictionary<long, ProduktKlienta> Produkty = Calosc.DostepDane.Pobierz<ProduktKlienta>(SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient, x => listaproduktowid.Contains(x.Id)).ToDictionary(x => x.Id, x => x);

            ListaDane ld = new ListaDane();
            ld.AktualneParametry = new ParametryPrzekazywaneDoListyProduktow();
            ld.AktualneParametry.Zaladujkontrolke(kontrolkaWyzwalajaca);

            ld.KlientWidziCeneHurtowa = !SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzWartosc<bool>(SolexHelper.AktualnyKlient, TypUstawieniaKlienta.UkryjCenyHurtowe); ;
            ld.SortowanieJednostek = SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzWartosc<bool>(SolexHelper.AktualnyKlient, TypUstawieniaKlienta.SposobSortowaniaJednostek);
            ld.PokazujJednostki = !SolexBllCalosc.PobierzInstancje.Konfiguracja.UkryjJednostkiMiary;
            ld.WszystkieSposobyPokazywaniaStanowKlienta = SolexBllCalosc.PobierzInstancje.SposobyPokazywaniaStanowBll.WszystkieSposobyKlienta(SolexHelper.AktualnyKlient, SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyPrzedstawiciel);
            ld.UstawieniaListyProduktow = new ListaProduktowZKategoriam(Produkty.Values.ToArray(), null, new HashSet<long>( Produkty.Keys )); 

            if (kontrolkaWyzwalajaca.Przesuwanie)
            {
                ld.slajder = new Slajder(Produkty.Count);
                ld.slajder.IleElementowWWierszu = kontrolkaWyzwalajaca.IleProduktowWWierszu;
                ld.slajder.IleWierszy = kontrolkaWyzwalajaca.PrzesuwanieIleWierszy;
                ld.slajder.CzasPrzeskoku = 200;
            }

            ViewEngineResult result = ViewEngines.Engines.FindView(this.ControllerContext, kontrolkaWyzwalajaca.Szablon, null);
            return result.View!=null? PartialView(kontrolkaWyzwalajaca.Szablon, ld) : PartialView("Widoki/" + kontrolkaWyzwalajaca.Szablon, ld);
        }

    }
    
}