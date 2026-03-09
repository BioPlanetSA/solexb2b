using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FakeItEasy;
using ServiceStack.Common;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.BLL.Testy;
using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Zamowienia;
using Xunit;
namespace SolEx.Hurt.Core.BLL.Tests
{
    public class TestyKonfiguracjiTests
    {
        [Fact(DisplayName = "Test Sprawdzajacy konfiguracje skrzynki do wysylania maili")]
        public void TestKonfiguracjiSkrzynkiPocztowejTest()
        {
            tSkrzynkiPocztowej("", "", "", "", false);
            tSkrzynkiPocztowej("aaa", "aaa", "aaa", "aaa", true);
            tSkrzynkiPocztowej("aaa", "", "", "", false);
            tSkrzynkiPocztowej("", "aaa", "", "", false);
            tSkrzynkiPocztowej("aaa", "aaa", "aaa", "", false);
            tSkrzynkiPocztowej("", "aaa", "aaaa", "", false);

        }

        [Fact(DisplayName = "Test sprawdzajacy konfiguracje skrzynki newslleterow")]
        public void TestKonfiguracjiSkrzynkiNewsletterTest()
        {

            tSkrzynkiNewsletter(false, "", "", "", "");
            tSkrzynkiNewsletter(true, "aaaa", "aaa", "aaa", "aaa");
            tSkrzynkiNewsletter(false, "a", "", "", "");
            tSkrzynkiNewsletter(false, "", "a", "", "");
            tSkrzynkiNewsletter(false, "aaa", "aaa", "a", "");
            tSkrzynkiNewsletter(false, "", "aaaa", "aaa", "");
        }

        [Fact(DisplayName = "Test sprawdzajacy dzialanie metody SprawdzKatalogPdf - sprwadzanie dostepu do katalogu - przechowywanie pdf")]
        public void TestKataloguPdfTest()
        {
            tKataloguPdf(true);

        }
        [Fact(DisplayName = "Test sprawdzajacy dzialanie metody SprawdzKatalogZdjec - sprwadzanie dostepu do katalogu - przechowywanie zdjęć")]
        public void TestKataloguPdfZdjec()
        {
            tKataloguZdjec(true);

        }

        [Fact(DisplayName = "Test sprawdzajacy poprawnosc dzialania funkcji TestSlabychHasel - sprawdzenie czy wystepuje slabe haslo u uzytkownikow z rola rozna od klienta ")]
        public void TestSlabychHaselTest()
        {
            //1-haslo uzytkownik, 2-oczekiwana, 3-role
            tSlabychHasel(Tools.PobierzInstancje.GetMd5Hash("123"), false, "1;2;0");
            tSlabychHasel(Tools.PobierzInstancje.GetMd5Hash("123"), true, "0");
            tSlabychHasel(Tools.PobierzInstancje.GetMd5Hash("1234"), true, "2");
        }

        [Fact(DisplayName = "Sprawdzenie czy poprawnie dziala metoda PrzechwytywanieMaili - sprawdzenie czy włączone jest przechwytywanie emaila")]
        public void PrzechwytywanieMailiTest()
        {
            tPrzechwytywanieMaila(true, false);
            tPrzechwytywanieMaila(false, true);
        }

        [Fact(DisplayName = "Test sprawdzajacy czy wartosci domysne cen hurtowej oraz detalicznej zostaly zmienione")]
        public void SprawdzenieDomyslnychCenTest()
        {
            tDomysneCeny(true, 1, 1);
            tDomysneCeny(false, 0, 0);
            tDomysneCeny(false, 1, 0);
            tDomysneCeny(false, 0, 1);
        }

        [Fact(DisplayName = "Test sprawdząjacy czy liczba aktynych klientów posiadających tylko role Klient jest większa niż 10")]
        public void SprawdzenieIkoscAktywnychKlientowTest()
        {
            sIlosciAktywnychKlientow(true, true, 15, RoleType.Klient);
            sIlosciAktywnychKlientow(false, false, 15, RoleType.Klient);
            sIlosciAktywnychKlientow(true, false, 8, RoleType.Klient);
        }

        [Fact(DisplayName = "Test sprawdzajacy czy liczba aktywnych produktów jest wieksza niz 10")]
        public void SprawdzenieIlosciAktywnychProduktowTest()
        {
            tIloscAktywnychProduktow(true, 11);
            tIloscAktywnychProduktow(false, 4);
        }

        [Fact(DisplayName = "Test sprawdzajacy czy ilosc zamowien jest wieksza niz 5")]
        public void SprawdzenieIlosciZamowienTest()
        {
            tIloscZamowien(true, 6);
            tIloscZamowien(false, 5);
        }

        [Fact(DisplayName = "Test sprawdzajacy czy ilosc dokumentow jest wieksza niz 10")]
        public void SprawdzenieIlosciDokumentowTest()
        {
            tIloscDokumentow(true, 11);
            tIloscDokumentow(false, 10);
            tIloscDokumentow(false, 3);
        }

        [Fact(DisplayName = "Test sprawdzajacy czy sa minimum dwie skonfigurowae metki")]
        public void CzyMetkiSaSkonfigurowaneTest()
        {
            tMetkiSaSkonfigurowane(true, 3, MetkaPozycjaLista.NadNazwa, MetkaPozycjaRodziny.NadNazwa, MetkaPozycjaSzczegoly.NadNazwa, MetkaPozycjaSzczegolyWarianty.NadNazwa, "opis", "katalog");
            tMetkiSaSkonfigurowane(false, 1, MetkaPozycjaLista.NadNazwa, MetkaPozycjaRodziny.NadNazwa, MetkaPozycjaSzczegoly.NadNazwa, MetkaPozycjaSzczegolyWarianty.NadNazwa, "opis", "katalog");
            tMetkiSaSkonfigurowane(false, 3, MetkaPozycjaLista.Brak, MetkaPozycjaRodziny.Brak, MetkaPozycjaSzczegoly.Brak, MetkaPozycjaSzczegolyWarianty.Brak, "opis", "katalog");
            tMetkiSaSkonfigurowane(false, 3, MetkaPozycjaLista.NadNazwa, MetkaPozycjaRodziny.NadNazwa, MetkaPozycjaSzczegoly.NadNazwa, MetkaPozycjaSzczegolyWarianty.NadNazwa, null, null);
            tMetkiSaSkonfigurowane(true, 3, MetkaPozycjaLista.Brak, MetkaPozycjaRodziny.Brak, MetkaPozycjaSzczegoly.Brak, MetkaPozycjaSzczegolyWarianty.NadNazwa, "opis", null);


        }
        [Fact(DisplayName = "Test sprawdzający czy jest przynajmniej jedna rejestracja")]
        public void CzyJestJednaRejestracjaTest()
        {
            tCzyJestJednaRejestracja(false, 0);
            tCzyJestJednaRejestracja(true, 1);
        }

        [Fact(DisplayName = "Test sprawdzajacy czy ilość aktywnych modulów koszyka jest większa niż 5")]
        public void IloscAktynycgModulowKoszykaTest()
        {
            tIloscAktywnosciModuluKoszyka(true, 5, true);
            tIloscAktywnosciModuluKoszyka(false, 2, true);
            tIloscAktywnosciModuluKoszyka(false, 2, false);
        }

        [Fact(DisplayName = "Test sprawdzający czy ilość aktynych automatycznych aktualizacji jest wiekszy niz 1")]
        public void CzyAktualizacjaSaAktywneTest()
        {
            tCzyAtualizacjeSaAktywe(true, 1);
            tCzyAtualizacjeSaAktywe(false, 0);
        }

        [Fact(DisplayName = "Test spradzajacy czy w ostanich 24h odbyla sie synchronizacja ")]
        public void CzySynchronizacjaWCiaguDobyTest()
        {
            tCzySynchronizacjaWCiaguDoby(true, 7);
            tCzySynchronizacjaWCiaguDoby(false, 30);
        }

        [Fact(DisplayName = "Test sprawdzajacy czy jest jakis rotator banerow")]
        public void CzyJestRotatorBanerowTest()
        {
            tCzyJestRotatorBanerow(true, 1, true);
            tCzyJestRotatorBanerow(false, 0, true);
            tCzyJestRotatorBanerow(false, 2, false);
            tCzyJestRotatorBanerow(false, 1, false);
        }

        [Fact(DisplayName = "test sprawdzjący czy jest jakis sposob pokazywania stanow")]
        public void CzyJestSposobPokazywaniaStanowTest()
        {
            tCzyJestSposobPokazywaniaStanow(true, 1, true);
            tCzyJestSposobPokazywaniaStanow(false, 0, true);
            tCzyJestSposobPokazywaniaStanow(false, 2, false);
        }

        [Fact(DisplayName = "Test sprawdzajacy czy jest minimum 2 Pracowników")]
        public void IloscPracownikowTest()
        {
            tIloscPracownikow(true, 4, RoleType.Pracownik);
            tIloscPracownikow(false, 2, RoleType.Pracownik);
            tIloscPracownikow(false, 4, RoleType.Klient);
        }

        [Fact(DisplayName = "Test sprawdzający czy klient posiada opiekunów")]
        public void CzyKlientMaOpiekunowTest()
        {
            tzyKlientMaOpiekunow(true, 2, 1, 2, 3);
            tzyKlientMaOpiekunow(true, 2, null, null, 3);
            tzyKlientMaOpiekunow(true, 2, null, 2, null);
            tzyKlientMaOpiekunow(true, 2, 1, null, null);
            tzyKlientMaOpiekunow(false, 0, 1, 2, 3);
            tzyKlientMaOpiekunow(false, 2, null, null, null);
        }

        [Fact(DisplayName = "Test sprawdzajacy czy został wysłany minimum jeden newsletter")]
        public void CzyWyslanyNewsletterTest()
        {
            tCzyWyslanyNewsletter(true, 2);
            tCzyWyslanyNewsletter(false, 0);
        }

    

        [Fact(DisplayName = "Test sprawdzający czy jest włączona integoracja dla klientów")]
        public void CzyIntegracjaDlaKlientaTest()
        {
            tCzyIntegracjaDlaKlienta(true, true, true, Kierunek.Export);
            tCzyIntegracjaDlaKlienta(false, false, true, Kierunek.Export);
            tCzyIntegracjaDlaKlienta(true, false, false, Kierunek.Export);
            tCzyIntegracjaDlaKlienta(true, false, true, Kierunek.Import);
        }

        [Fact(DisplayName = "Test sprawdzający czy poziom cen ma walute PLN")]
        public void CzyWalutaPlnTest()
        {
            tCzyWalutaPln(false, false, true);
            tCzyWalutaPln(true, false, false);
            tCzyWalutaPln(false, true, false);
        }

        [Fact(DisplayName = "Test Sprawdzajacy czy nie ma dubli kategorii")]
        public void SprawdzenieDubliKategoriiTest()
        {
            AtrybutyBLL a1 = new AtrybutyBLL(){nazwa = "Nazwa1", atrybut_id = 1};
            AtrybutyBLL a2 = new AtrybutyBLL() { nazwa = "Nazwa2", atrybut_id = 2 };
            AtrybutyBLL a3 = new AtrybutyBLL() { nazwa = "Nazwa1", atrybut_id = 3 };
            List<AtrybutyBLL> listaAtrybutow = new List<AtrybutyBLL>(){a1,a2,a3};
            SprawdzenieDubliAtrybutow duble= new SprawdzenieDubliAtrybutow();
            var c = A.Fake<ICechyAtrybuty>();
            A.CallTo(() => c.Pobierz(A<AtrybutySearchCriteria>.Ignored, A<int>.Ignored)).Returns(listaAtrybutow);
            var config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.JezykIDPolski).Returns(1);
            duble.cechyAtrybuty = c;
            duble.config = config;
            List<string> wynik = duble.Test();
            Assert.True(wynik.Count==1);
            Assert.True(wynik[0] == string.Format("Są przynajmniej dwa atrybuty o nazwie: {0}", a1.nazwa));


        }

        public void tCzyWalutaPln(bool czyJestPozimCen, bool czyJestCenPoziom, bool oczekiwana)
        {
            IList<poziomy_cen> poziomyCen = new List< poziomy_cen>();
           List<ceny_poziomy> cenyPoziom = new List<ceny_poziomy>();
            if (czyJestCenPoziom)
            {
                ceny_poziomy cp = new ceny_poziomy(1, 1, 1, "PLN");
                cenyPoziom.Add(cp);
            }
            if (czyJestPozimCen)
            {
                poziomy_cen pc = new poziomy_cen(1, "a", "PLN", "PLN");
                poziomyCen.Add(pc);

            }
            var cePo = A.Fake<IPoziomyCenBll>();
            A.CallTo(() => cePo.Wszystykie()).Returns(cenyPoziom);
            var config = A.Fake<IPoziomyCenDostep>();
            A.CallTo(() => config.Wszystykie()).Returns(poziomyCen);
            CzyWalutaPln cwp = new CzyWalutaPln();
            cwp.PoziomyCenDostep = config;
            cwp.PoziomyCen = cePo;
            List<string> w = cwp.Test();
            if (oczekiwana)
            {
                Assert.False(w.Any());
            }
            else
            {
                Assert.True(w.Any());
            }
        }

        public void tCzyIntegracjaDlaKlienta(bool czyApiAktywne, bool oczekiwana, bool czyWidoczny, Kierunek rodzaj)
        {
            var config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.ApiAktywneDlaKlientow).Returns(czyApiAktywne);
            A.CallTo(() => config.JezykIDPolski).Returns(1);

            List<ImportyBll> importy = new List<ImportyBll>();
            ImportyBll imp = new ImportyBll() { };
            imp.kierunek = (int)rodzaj;
            imp.Widoczny = czyWidoczny;
            importy.Add(imp);

            var import = A.Fake<IImportyDostep>();
            A.CallTo(() => import.Pobierz(1)).Returns(importy);

            CzyIntegracjaDlaKlienta tk = new CzyIntegracjaDlaKlienta();
            tk.Konfiguracja = config;
            tk.Import = import;
            List<string> w = tk.Test();
            if (oczekiwana)
            {
                Assert.False(w.Any());
            }
            else
            {
                Assert.True(w.Any());
            }
        }

     
        public void tCzyWyslanyNewsletter(bool oczekiwana, int iloscMaili)
        {
            IList<mailing_kampanie> kampania = new List<mailing_kampanie>();
            for (int i = 0; i < iloscMaili; i++)
            {
                mailing_kampanie mk = new mailing_kampanie();
                kampania.Add(mk);
            }

            var maile = A.Fake<IMaileBLL>();
            A.CallTo(() => maile.Wszystykie()).Returns(kampania);
            CzyWyslanyNewsletter tk = new CzyWyslanyNewsletter();
            tk.Maile = maile;
            List<string> w = tk.Test();
            if (oczekiwana)
            {
                Assert.False(w.Any());
            }
            else
            {
                Assert.True(w.Any());
            }
        }

        public void tzyKlientMaOpiekunow(bool oczekiwana, int iloscKlientow, int? id_opiekuna, int? id_przedstawiciela, int? id_drugiego_opiekuna)
        {
            List<IKlient> listaKlientow = new List<IKlient>();
            for (int i = 0; i < iloscKlientow; i++)
            {
                IKlient klient = new Klient(null) { opiekun_id = id_opiekuna, przedstawiciel_id = id_przedstawiciela, drugi_opiekun = id_drugiego_opiekuna };
                listaKlientow.Add(klient);
            }
            var klienci = A.Fake<IKlienciDostep>();
            A.CallTo(() => klienci.PobierzWszystkich(null)).Returns(listaKlientow);
            CzyKlientMaOpiekunow tk = new CzyKlientMaOpiekunow();
            tk.Klienci = klienci;
            List<string> w = tk.Test();
            if (oczekiwana)
            {
                Assert.False(w.Any());
            }
            else
            {
                Assert.True(w.Any());
            }
        }
        public void tIloscPracownikow(bool oczekiwana, int iloscPracownikow, RoleType rola)
        {
            List<IKlient> listaPracownikow = new List<IKlient>();
            for (int i = 0; i < iloscPracownikow; i++)
            {
                IKlient klient = new Klient(null) { role = ((int)rola).ToString() };
                listaPracownikow.Add(klient);
            }

            var klienci = A.Fake<IKlienciDostep>();
            A.CallTo(() => klienci.PobierzWszystkich(null)).Returns(listaPracownikow);
            IloscPracownikow tk = new IloscPracownikow();
            tk.Klienci = klienci;
            List<string> w = tk.Test();
            if (oczekiwana)
            {
                Assert.False(w.Any());
            }
            else
            {
                Assert.True(w.Any());
            }

        }

        public void tCzyJestSposobPokazywaniaStanow(bool oczekiwana, int iloscSposobow, bool jestRegula)
        {
            List<sposoby_pokazywania_stanow_reguly> reguly = new List<sposoby_pokazywania_stanow_reguly>();
            Dictionary<int, sposoby_pokazywania_stanow> slownikSposobow = new Dictionary<int, sposoby_pokazywania_stanow>();

            if (jestRegula)
            {
                sposoby_pokazywania_stanow_reguly regula = new sposoby_pokazywania_stanow_reguly();
                reguly.Add(regula);
            }

            for (int i = 0; i < iloscSposobow; i++)
            {
                sposoby_pokazywania_stanow sposob = new sposoby_pokazywania_stanow() { reguly = reguly };
                slownikSposobow.Add(i + 1, sposob);
            }

            var config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.JezykIDPolski).Returns(1);

            var sposoby = A.Fake<ISposobyPokazywaniaStanowBLL>();
            A.CallTo(() => sposoby.WszystkieSposoby(1)).Returns(slownikSposobow);

            CzyJestSposobPokazywaniaStanow tk = new CzyJestSposobPokazywaniaStanow();
            tk.Konfiguracja = config;
            tk.SposobyPokazwaniaStanow = sposoby;
            List<string> w = tk.Test();
            if (oczekiwana)
            {
                Assert.False(w.Any());
            }
            else
            {
                Assert.True(w.Any());
            }
        }

        public void tCzyJestRotatorBanerow(bool oczekiwana, int iloscBanerow, bool czyWidoczny)
        {

            Dictionary<int, Banery> listaBanerow = new Dictionary<int, Banery>();
            for (int i = 0; i < iloscBanerow; i++)
            {
                Banery b = new Banery() { Id = i + 1, Widoczny = czyWidoczny };
                listaBanerow.Add(i + 1, b);
            }

            var config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.JezykIDPolski).Returns(1);

            var banery = A.Fake<IBanerBLL>();
            A.CallTo(() => banery.WszystkieBanery(1)).Returns(listaBanerow);

            CzyJestRotatorBanerow tk = new CzyJestRotatorBanerow();
            tk.Konfiguracja = config;
            tk.Banery = banery;
            List<string> w = tk.Test();
            if (oczekiwana)
            {
                Assert.False(w.Any());
            }
            else
            {
                Assert.True(w.Any());
            }
        }

        public void tCzySynchronizacjaWCiaguDoby(bool oczekiwana, int ileGodzin)
        {
            Zadania zad = new Zadania() { NumerElementuSynchronizacji = 1, ostatnieUruchomienieKoniec = DateTime.Now.AddHours(-ileGodzin) };
            List<Zadania> zadania = new List<Zadania>();
            zadania.Add(zad);

            var config = A.Fake<IZadaniaBLL>();
            A.CallTo(() => config.PobierzZadania(true, A<TypZadania>.Ignored)).Returns(zadania);
            CzySynchronizacjaWCiaguDoby tk = new CzySynchronizacjaWCiaguDoby();
            tk.Zadania = config;

            List<string> w = tk.Test();
            if (oczekiwana)
            {
                Assert.False(w.Any());
            }
            else
            {
                Assert.True(w.Any());
            }

        }

        public void tCzyAtualizacjeSaAktywe(bool oczekiwane, int iloscZadan)
        {
            List<Zadania> zadaniaAutomatycznejSynchronizacji = new List<Zadania>();
            for (int i = 0; i < iloscZadan; i++)
            {
                Zadania zad = new Zadania() { NumerElementuSynchronizacji = i + 1 };
                zadaniaAutomatycznejSynchronizacji.Add(zad);
            }

            var zadania = A.Fake<IZadaniaBLL>();
            A.CallTo(() => zadania.PobierzZadania(true, A<TypZadania>.Ignored)).Returns(zadaniaAutomatycznejSynchronizacji);
            CzySynchronizacjeAutomatyczneSaAktywne tk = new CzySynchronizacjeAutomatyczneSaAktywne();
            tk.Zadania = zadania;
            List<string> w = tk.Test();
            if (oczekiwane)
            {
                Assert.False(w.Any());
            }
            else
            {
                Assert.True(w.Any());
            }

        }
        public void tIloscAktywnosciModuluKoszyka(bool oczekiwana, int iloscModulow, bool czyAktywne)
        {
            List<Zadania> zadaniaKoszyka = new List<Zadania>();
            for (int i = 0; i < iloscModulow; i++)
            {
                var zad = A.Fake<Zadania>();
                zad.Aktywne = czyAktywne;
                zadaniaKoszyka.Add(zad);
            }

            var config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.JezykIDPolski).Returns(1);

            var lista = A.Fake<IZadaniaBLL>();
            A.CallTo(() => lista.PobierzZadania(true, TypZadania.RegulaKoszyka)).Returns(zadaniaKoszyka);

            IloscAktynycgModulowKoszyka tk = new IloscAktynycgModulowKoszyka();
            tk.Konfiguracja = config;
            tk.Zadania = lista;

            List<string> w = tk.Test();
            if (oczekiwana)
            {
                Assert.False(w.Any());
            }
            else
            {
                Assert.True(w.Any());
            }
        }
        public void tCzyJestJednaRejestracja(bool oczekiwana, int iloscRejestracji)
        {
            List<RejestracjeModel> listarejestracji = new List<RejestracjeModel>();
            for (int i = 0; i < iloscRejestracji; i++)
            {
                RejestracjeModel rm = new RejestracjeModel();
                listarejestracji.Add(rm);
            }

            var config = A.Fake<IRejestracjeBll>();
            A.CallTo(() => config.GetRegistrationList(null)).Returns(listarejestracji);

            CzyJestJednaRejestracja tk = new CzyJestJednaRejestracja();
            tk.Rejestracja = config;

            List<string> w = tk.Test();
            if (oczekiwana)
            {
                Assert.False(w.Any());
            }
            else
            {
                Assert.True(w.Any());
            }
            

        }


        public void tMetkiSaSkonfigurowane(bool oczekiwana, int iloscMetek, MetkaPozycjaLista pozycjaLista, MetkaPozycjaRodziny pozycjaRodziny, MetkaPozycjaSzczegoly pozycjaSzczegoly, MetkaPozycjaSzczegolyWarianty pozycjaSzczegolyWarianty, string metkaOpis, string metkaKatalog)
        {

            Dictionary<int, CechyBll> metki = new Dictionary<int, CechyBll>();
            for (int i = 0; i < iloscMetek; i++)
            {
                CechyBll cecha = new CechyBll() { cecha_id = i + 1, MetkaPozycjaLista = pozycjaLista, MetkaPozycjaRodziny = pozycjaRodziny, MetkaPozycjaSzczegoly = pozycjaSzczegoly, MetkaPozycjaSzczegolyWarianty = pozycjaSzczegolyWarianty, metka_katalog = metkaKatalog, metka_opis = metkaOpis };
                metki.Add(i + 1, cecha);
            }

            var konfiguracja = A.Fake<IConfigBLL>();
            A.CallTo(() => konfiguracja.JezykIDPolski).Returns(1);

            var config = A.Fake<ICechyAtrybuty>();
            A.CallTo(() => config.PobierzWszystkieCechy(1)).Returns(metki);


            CzyMetkiSaSkonfigurowane tk = new CzyMetkiSaSkonfigurowane();
            tk.CechyAtrybuty = config;
            tk.Konfiguracja = konfiguracja;

            List<string> w = tk.Test();
            if (oczekiwana)
            {
                Assert.False(w.Any());
            }
            else
            {
                Assert.True(w.Any());
            }
        }

        public void tIloscDokumentow(bool oczekiwana, int iloscDokumentow)
        {
            Dictionary<int, IDokument> dokumenty = new Dictionary<int, IDokument>();
            historia_dokumenty histDokument = new historia_dokumenty();
            IDokument dok = new DokumentyBll(histDokument);
            for (int i = 0; i < iloscDokumentow; i++)
            {
                dokumenty.Add(i, dok);

            }

            var config = A.Fake<IDokumenty>();
            A.CallTo(() => config.WszystkieDokumenty()).Returns(dokumenty);

            SprawdzenieIlosciDokumentow tk = new SprawdzenieIlosciDokumentow();
            tk.Dokumenty = config;
            List<string> w = tk.Test();
            if (oczekiwana)
            {
                Assert.False(w.Any());
            }
            else
            {
                Assert.True(w.Any());
            }
        }

        public void tIloscZamowien(bool oczekiwana, int iloscZamowien)
        {
            Dictionary<int, ZamowieniaBLL> slownik = new Dictionary<int, ZamowieniaBLL>();
            zamowienia zamowienie = new zamowienia();
            ZamowieniaBLL zam = new ZamowieniaBLL(zamowienie);

            for (int i = 0; i < iloscZamowien; i++)
            {

                slownik.Add(i, zam);

            }
            var config = A.Fake<IZamowieniaDostep>();
            A.CallTo(() => config.PobierzZamowienia(null)).Returns(slownik);

            SprawdzenieIlosciZamowien tk = new SprawdzenieIlosciZamowien();
            tk.Zamowienia = config;
            List<string> w = tk.Test();
            if (oczekiwana)
            {
                Assert.False(w.Any());
            }
            else
            {
                Assert.True(w.Any());
            }


        }
        public void tIloscAktywnychProduktow(bool oczekiwana, int iloscProduktow)
        {
            List<ProduktBazowy> listaProduktow = new List<ProduktBazowy>();
            ProduktBazowy pb = new ProduktBazowy(1);

            for (int i = 0; i < iloscProduktow; i++)
            {

                listaProduktow.Add(pb);
            }

            var config = A.Fake<IProduktyBazowe>();
            A.CallTo(() => config.Pobierz(A<ProduktySearchCriteria>.Ignored, 1, null)).Returns(listaProduktow);

            SprawdzenieIlosciAktywnychProduktow tk = new SprawdzenieIlosciAktywnychProduktow();
            tk.ProduktyBazowe = config;
            List<string> w = tk.Test();
            if (oczekiwana)
            {
                Assert.False(w.Any());
            }
            else
            {
                Assert.True(w.Any());
            }
        }

        public void sIlosciAktywnychKlientow(bool aktywny, bool oczekiwana, int iloscKlientow, RoleType rola)
        {
            List<IKlient> listaKlientow = new List<IKlient>();
            IKlient klient = new Klient(null) { aktywny = aktywny, role = ((int)rola).ToString() };
            for (int i = 0; i < iloscKlientow; i++)
            {
                listaKlientow.Add(klient);
            }

            var config = A.Fake<IKlienciDostep>();
            A.CallTo(() => config.PobierzWszystkich(null)).Returns(listaKlientow);


            SprawdzenieIkoscAktywnychKlientow tk = new SprawdzenieIkoscAktywnychKlientow();
            tk.Klienci = config;
            List<string> w = tk.Test();
            if (oczekiwana)
            {
                Assert.False(w.Any());
            }
            else
            {
                Assert.True(w.Any());
            }
        }

        public void tDomysneCeny(bool oczekiwana, int cenaHurtowa, int cenaDetaliczna)
        {
            var config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.GetPriceLevelHurt).Returns(cenaHurtowa);
            A.CallTo(() => config.GetPriceLevelDetal).Returns(cenaDetaliczna);

            SprawdzenieDomyslnychCen tk = new SprawdzenieDomyslnychCen();
            tk.Konfiguracja = config;

            List<string> w = tk.Test();
            if (oczekiwana)
            {
                Assert.False(w.Any());
            }
            else
            {
                Assert.True(w.Any());
            }
        }

        public void tPrzechwytywanieMaila(bool oczekiwane, bool przechwycony)
        {
            var config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.MaileTylkoSolex).Returns(przechwycony);

            TestPrzechwytywanieMaili tk = new TestPrzechwytywanieMaili();
            tk.Konfiguracja = config;

            List<string> w = tk.Test();
            if (oczekiwane)
            {
                Assert.False(w.Any());
            }
            else
            {
                Assert.True(w.Any());
            }

        }

        public void tSlabychHasel(string haslo, bool oczekiwane, string rola)
        {
            List<IKlient> listaKlientow = new List<IKlient>();
            IKlient klient = new Klient(null) { haslo_klienta = haslo, role = rola};

            listaKlientow.Add(klient);

            var config = A.Fake<IKlienciDostep>();
            A.CallTo(() => config.PobierzWszystkich(null)).Returns(listaKlientow);

            TestSlabychHasel tk = new TestSlabychHasel();
            tk.Klienci = config;
            List<string> w = tk.Test();
            if (oczekiwane)
            {
                Assert.False(w.Any());
            }
            else
            {
                Assert.True(w.Any());
            }

        }

        public void tKataloguZdjec(bool wartoscOczekiwana)
        {
            string dir = "kataalo";
            Directory.CreateDirectory(dir);
            SprawdzKatalogZdjec tk = new SprawdzKatalogZdjec();
            List<string> w = tk.sprawdzKatalog(dir);
            if (wartoscOczekiwana)
            {
                Assert.False(w.Any());
            }
            else
            {
                Assert.True(w.Any());
            }
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }
        }


        public void tKataloguPdf(bool wartoscOczekiwana)
        {
            string dir = "kataalo";
            Directory.CreateDirectory(dir);
            SprawdzKatalogPdf tk = new SprawdzKatalogPdf();
            List<string> w = tk.sprawdzKatalog(dir);
            if (wartoscOczekiwana)
            {
                Assert.False(w.Any());
            }
            else
            {
                Assert.True(w.Any());
            }
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }
        }


        public void tSkrzynkiPocztowej(string emailFrom, string nazwaUzytkownika, string nazwaHosta, string haslo, bool wynik)
        {
            var config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.EmailFrom).Returns(emailFrom);
            A.CallTo(() => config.EmailNazwaUzytkownika).Returns(nazwaUzytkownika);
            A.CallTo(() => config.EmailHaslo).Returns(haslo);
            A.CallTo(() => config.EmailHost).Returns(nazwaHosta);

            TestSkrzynki tk = new TestSkrzynki();
            tk.Konfiguracja = config;
            List<string> w = tk.Test();
            if (wynik)
            {
                Assert.False(w.Any());
            }
            else
            {
                Assert.True(w.Any());
            }

        }

        public void tSkrzynkiNewsletter(bool wynik, string emailFrom, string nazwaUzytkownika, string nazwaHosta, string haslo)
        {
            var config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.MailingEmailFrom).Returns(emailFrom);
            A.CallTo(() => config.MailingEmailNazwaUzytkownika).Returns(nazwaUzytkownika);
            A.CallTo(() => config.MailingEmailHaslo).Returns(haslo);
            A.CallTo(() => config.MailingEmailHost).Returns(nazwaHosta);

            TestKonfiguracjiSkrzynkiNewsletterow tk = new TestKonfiguracjiSkrzynkiNewsletterow();
            tk.Konfiguracja = config;
            List<string> w = tk.Test();
            if (wynik)
            {
                Assert.False(w.Any());
            }
            else
            {
                Assert.True(w.Any());
            }

        }





    }
}
