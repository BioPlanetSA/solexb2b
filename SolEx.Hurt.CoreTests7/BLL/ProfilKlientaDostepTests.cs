using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using ServiceStack.OrmLite;
using ServiceStack.Text;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.ObiektyMaili;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using Xunit;

namespace SolEx.Hurt.CoreTests.BLL
{
    public class ProfilKlientaDostepTest
    {
        private string baza = ":memory:";
        private OrmLiteConnectionFactory polaczenie;

        public ProfilKlientaDostepTest()
        {
            polaczenie = new OrmLiteConnectionFactory(baza, SqliteDialect.Provider)
            {
                DialectProvider = {UseUnicode = true},
                AutoDisposeConnection = false
            };
        }

        [Fact(DisplayName = "Metoda testująca czy prawidłowo wyciagamy klientów którzy maja określona wartośc w odpowiednim typie ustawienia oraz czy prawidłowo zwracamy wartosc domyslna profilu")]
        public void PobierzKlientowZOkreslonaWartosciaUstawieniaTest()
        {
            ProfilKlienta pf1 = new ProfilKlienta(TypUstawieniaKlienta.PowiadomieniaMailowe, false, "PrzeterminowanePlatnosci", AccesLevel.Zalogowani) { KlientId = 2};
            ProfilKlienta pf2 = new ProfilKlienta(TypUstawieniaKlienta.PowiadomieniaMailowe, true, "PrzeterminowanePlatnosci", AccesLevel.Zalogowani) { KlientId = null};
            ProfilKlienta pf3 = new ProfilKlienta(TypUstawieniaKlienta.DokumentyTylkoNiezrealizowane, true, "PrzeterminowanePlatnosci", AccesLevel.Zalogowani) { KlientId = 3 };
            ProfilKlienta pf4 = new ProfilKlienta(TypUstawieniaKlienta.PowiadomieniaMailowe, true, "PrzeterminowanePlatnosci", AccesLevel.Zalogowani) { KlientId = 4 };

            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.DbFactory = polaczenie;
            A.CallTo(() => solexBllCalosc.DostepDane).Returns(dostep);
            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<ProfilKlienta>();
                db.Insert(pf1);
                db.Insert(pf2);
                db.Insert(pf3);
                db.Insert(pf4);
            }

            ProfilKlientaDostep pkd = new ProfilKlientaDostep(solexBllCalosc);
            var wynik = pkd.PobierzKlientowZWartosciaUstawienia<bool>(TypUstawieniaKlienta.PowiadomieniaMailowe, "PrzeterminowanePlatnosci", AccesLevel.Zalogowani);

            Assert.True(wynik.Count()==3);
            Assert.False(wynik[2]);
            Assert.True(wynik[4]);
            Assert.True(wynik[0]);
        }



        [Fact(DisplayName = "Test sprawdzający poprawne iniciowanie wartości domyślnych profili klienta dla powiadomien mailowych ")]
        public void InicjalizujPowiadomieniaMailoweTest()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            ICacheBll cache = A.Fake<ICacheBll>();
            A.CallTo(() => solexBllCalosc.Cache).Returns(cache);
            A.CallTo(() => cache.PobierzObiekt<IList<Model.Klient>>(A<string>.Ignored)).Returns(null);

            var ustawienie1 = new UstawieniePowiadomienia();
            ustawienie1.Id = -100496004004814508;
            ustawienie1.ParametryWysylania = new List<ParametryWyslania>()
            {
                new ParametryWyslania() {DoKogo = TypyPowiadomienia.Klient, Aktywny = true},
                new ParametryWyslania() {DoKogo = TypyPowiadomienia.Opiekun, Aktywny = false},
                new ParametryWyslania() {DoKogo = TypyPowiadomienia.Przedstawiciel, Aktywny = false},
                new ParametryWyslania() {DoKogo = TypyPowiadomienia.DrugiOpiekun, Aktywny = false}
            };
            ustawienie1.ZgodaNaZmianyPrzezKlienta = true;
            var ustawienie2 = new UstawieniePowiadomienia();
            ustawienie2.Id = 5555555;
            ustawienie2.ParametryWysylania = new List<ParametryWyslania>()
            {
                new ParametryWyslania() {DoKogo = TypyPowiadomienia.Klient, Aktywny = true},
                new ParametryWyslania() {DoKogo = TypyPowiadomienia.Opiekun, Aktywny = false},
                new ParametryWyslania() {DoKogo = TypyPowiadomienia.Przedstawiciel, Aktywny = false},
                new ParametryWyslania() {DoKogo = TypyPowiadomienia.DrugiOpiekun, Aktywny = false}
            };
            ustawienie2.ZgodaNaZmianyPrzezKlienta = false;

            //Wartosc domyslna juz dodana
            var ustawienie3 = new UstawieniePowiadomienia();
            ustawienie3.Id = -7367565678842455244;
            ustawienie3.ParametryWysylania = new List<ParametryWyslania>()
            {
                new ParametryWyslania() {DoKogo = TypyPowiadomienia.Klient, Aktywny = true},
                new ParametryWyslania() {DoKogo = TypyPowiadomienia.Opiekun, Aktywny = false},
                new ParametryWyslania() {DoKogo = TypyPowiadomienia.Przedstawiciel, Aktywny = false},
                new ParametryWyslania() {DoKogo = TypyPowiadomienia.DrugiOpiekun, Aktywny = false}
            };
            ustawienie3.ZgodaNaZmianyPrzezKlienta = true;

            ProfilKlienta pk1 = new ProfilKlienta()
            {
                Dodatkowe = "NowyDokument",
                Dopisek = "Zalogowani",
                TypUstawienia = TypUstawieniaKlienta.PowiadomieniaMailowe,
                Wartosc = "true",
                KlientId = null
            };

            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<UstawieniePowiadomienia>();
                db.Insert(ustawienie1);
                db.Insert(ustawienie2);
                db.Insert(ustawienie3);

                db.CreateTable<ProfilKlienta>();
                db.Insert(pk1);
            }

            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.DbFactory = polaczenie;
            A.CallTo(() => solexBllCalosc.DostepDane).Returns(dostep);


            IMaileBLL mail = A.Fake<IMaileBLL>();
            SzablonMailaBaza obiekt = (SzablonMailaBaza) Activator.CreateInstance(typeof(NoweProduktyWSystemie));
            SzablonMailaBaza obiekt2 = (SzablonMailaBaza) Activator.CreateInstance(typeof(NowyDokument));
            List<SzablonMailaBaza> lista = new List<SzablonMailaBaza>() {obiekt, obiekt2};
            A.CallTo(() => mail.PobierzListeWszystkichPowiadomienMailowych()).Returns(lista);
            A.CallTo(() => solexBllCalosc.MaileBLL).Returns(mail);


            ProfilKlientaDostep pkd = new ProfilKlientaDostep(solexBllCalosc);
            var profilePRzedInicjalizacja = solexBllCalosc.DostepDane.Pobierz<ProfilKlienta>(null);
            Assert.True(profilePRzedInicjalizacja.Count == 1);
            pkd.InicjalizujPowiadomieniaMailowe();
            var profilePoInicjalizacja = solexBllCalosc.DostepDane.Pobierz<ProfilKlienta>(null);
            Assert.True(profilePoInicjalizacja.Count == 2);

            pk1.Dodatkowe = null;

            //dodanie domyslnej wartosci
            using (var db = polaczenie.OpenDbConnection())
            {
                db.DropAndCreateTable<ProfilKlienta>();
                db.Insert(pk1);
            }
            profilePoInicjalizacja = solexBllCalosc.DostepDane.Pobierz<ProfilKlienta>(null);
            pkd.InicjalizujPowiadomieniaMailowe();
            Assert.True(profilePoInicjalizacja.Count == 1);
        }

        [Fact(DisplayName = "Test sprawdzający poprawne iniciowanie wartości domyślnych profili klienta oraz odczyt domyślnych wartości ")]
        public void PobierzWartoscDomyslnaTest()
        {
            var solexBllCalosc = KonfiguracjaDomyslna();
            ProfilKlientaDostep pkd = new ProfilKlientaDostep(solexBllCalosc);
            pkd.InicjalizujDomyslneWartosci();
            //szablon listy
            var szablon = pkd.PobierzWartoscDomyslna<string>(TypUstawieniaKlienta.SzablonListy, null);
            Assert.True(szablon.Equals("widok1"));
            szablon = pkd.PobierzWartoscDomyslna<string>(TypUstawieniaKlienta.SzablonListy, null, AccesLevel.Zalogowani);
            Assert.True(szablon.Equals("widok2"));

            szablon = pkd.PobierzWartoscDomyslna<string>(TypUstawieniaKlienta.SzablonListy, "");
            Assert.True(szablon.Equals("widok1"));
            szablon = pkd.PobierzWartoscDomyslna<string>(TypUstawieniaKlienta.SzablonListy, "", AccesLevel.Zalogowani);
            Assert.True(szablon.Equals("widok2"));

            //rozmiar strony listy produktow
            int rozmiarStrony = pkd.PobierzWartoscDomyslna<int>(TypUstawieniaKlienta.RozmiarStronyListaProduktow, null);
            Assert.True(rozmiarStrony == 100);
            rozmiarStrony = pkd.PobierzWartoscDomyslna<int>(TypUstawieniaKlienta.RozmiarStronyListaProduktow, null, AccesLevel.Zalogowani);
            Assert.True(rozmiarStrony == 100);

            //rozmiar strony listy w adminie  
            rozmiarStrony = pkd.PobierzWartoscDomyslna<int>(TypUstawieniaKlienta.ListaKolumnWAdminieRozmiarStrony, null);
            Assert.True(rozmiarStrony == 0);
            rozmiarStrony = pkd.PobierzWartoscDomyslna<int>(TypUstawieniaKlienta.ListaKolumnWAdminieRozmiarStrony, "SolEx.Hurt.Core.Klient", AccesLevel.Zalogowani);
            Assert.True(rozmiarStrony == 99);

            rozmiarStrony = pkd.PobierzWartoscDomyslna<int>(TypUstawieniaKlienta.ListaKolumnWAdminieRozmiarStrony, null);
            Assert.True(rozmiarStrony == 0);
            rozmiarStrony = pkd.PobierzWartoscDomyslna<int>(TypUstawieniaKlienta.RozmiarStronyListaProduktow, null, AccesLevel.Zalogowani);
            Assert.True(rozmiarStrony == 100);

        }

        private ISolexBllCalosc KonfiguracjaDomyslna()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            ICacheBll cache = A.Fake<ICacheBll>();
            IConfigBLL konfiguracja = A.Fake<IConfigBLL>();
            A.CallTo(() => solexBllCalosc.Cache).Returns(cache);
            A.CallTo(() => cache.PobierzObiekt<IList<Model.Klient>>(A<string>.Ignored)).Returns(null);
            A.CallTo(() => solexBllCalosc.Konfiguracja).Returns(konfiguracja);
            A.CallTo(() => konfiguracja.AktywneWidokiListyProduktow(false)).Returns(new HashSet<string>() {"widok1", "widok2"});
            A.CallTo(() => konfiguracja.AktywneWidokiListyProduktow(true)).Returns(new HashSet<string>() {"widok2", "widok1"});
            A.CallTo(() => konfiguracja.IleProduktowPokazacNaStronie).Returns("100;20;10");
            A.CallTo(() => konfiguracja.ZIleDniDomyslniePokazywacDokumenty).Returns(50);
            A.CallTo(() => konfiguracja.DostepneSortowanieListyProduktow)
                .Returns(new List<Sortowanie>() {new Sortowanie(new List<SortowaniePole>() {new SortowaniePole("Pole1", KolejnoscSortowania.asc)}, "Pole1", "Pole1 asc|Pole1")});
            A.CallTo(() => konfiguracja.DostepneSortowanieKoszyka)
                .Returns(new List<Sortowanie>() {new Sortowanie(new List<SortowaniePole>() {new SortowaniePole("Pole1", KolejnoscSortowania.asc)}, "Pole1", "Pole1 asc|Pole1")});
            IMaileBLL mail = A.Fake<IMaileBLL>();
            SzablonMailaBaza obiekt = (SzablonMailaBaza) Activator.CreateInstance(typeof(NoweProduktyWSystemie));
            SzablonMailaBaza obiekt2 = (SzablonMailaBaza) Activator.CreateInstance(typeof(NowyDokument));
            List<SzablonMailaBaza> lista = new List<SzablonMailaBaza>() {obiekt, obiekt2};
            A.CallTo(() => mail.PobierzListeWszystkichPowiadomienMailowych()).Returns(lista);
            A.CallTo(() => solexBllCalosc.MaileBLL).Returns(mail);
            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.DbFactory = polaczenie;
            A.CallTo(() => solexBllCalosc.DostepDane).Returns(dostep);
            ProfilKlienta pk1 = new ProfilKlienta()
            {
                Dodatkowe = "SolEx.Hurt.Core.Klient",
                Dopisek = AccesLevel.Zalogowani.ToString(),
                TypUstawienia = TypUstawieniaKlienta.ListaKolumnWAdminieRozmiarStrony,
                Wartosc = "99",
                KlientId = null
            };
            ProfilKlienta pk2 = new ProfilKlienta()
            {
                Dodatkowe = "SolEx.Hurt.Core.Klient",
                Dopisek = AccesLevel.Zalogowani.ToString(),
                TypUstawienia = TypUstawieniaKlienta.ListaKolumnWAdminieRozmiarStrony,
                Wartosc = "77",
                KlientId = 1
            };
            var ustawienie1 = new UstawieniePowiadomienia();
            ustawienie1.Id = -100496004004814508;
            ustawienie1.ParametryWysylania = new List<ParametryWyslania>()
            {
                new ParametryWyslania() {DoKogo = TypyPowiadomienia.Klient, Aktywny = true},
                new ParametryWyslania() {DoKogo = TypyPowiadomienia.Opiekun, Aktywny = false},
                new ParametryWyslania() {DoKogo = TypyPowiadomienia.Przedstawiciel, Aktywny = false},
                new ParametryWyslania() {DoKogo = TypyPowiadomienia.DrugiOpiekun, Aktywny = false}
            };
            ustawienie1.ZgodaNaZmianyPrzezKlienta = true;
            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<UstawieniePowiadomienia>();
                db.Insert(ustawienie1);
                db.CreateTable<ProfilKlienta>();
                db.Insert(pk1);
                db.Insert(pk2);
            }
            return solexBllCalosc;
        }

        public void DodajWartoscTest()
        {
            var solexBllCalosc = KonfiguracjaDomyslna();
            IKlient klient = A.Fake<IKlient>();
            klient.Id = 1;

            ProfilKlientaDostep pkd = new ProfilKlientaDostep(solexBllCalosc);

            pkd.DodajWartosc(klient, TypUstawieniaKlienta.ListaKolumnWAdminieRozmiarStrony, 8556);

            var wynik = pkd.PobierzWartosc<int>(klient, TypUstawieniaKlienta.ListaKolumnWAdminieRozmiarStrony);
            Assert.True(wynik == 8556);
        }


        [Fact(DisplayName = "Test poprawności sprawdzania stałych filtrów")]
        public void SprawdzStalyFiltrTest()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            IKlient klient = A.Fake<IKlient>();
            klient.Id = 1;

            Dictionary<int, long[]> slownikAtr = new Dictionary<int, long[]>();
            slownikAtr.Add(1,new long[]  { 11,12,13,14});
            slownikAtr.Add(2, new long[] { 21, 22, 23, 24 });
            slownikAtr.Add(3, new long[] { 31, 32, 33, 34 });
            slownikAtr.Add(4, new long[] { 41, 42, 43, 44 });
            slownikAtr.Add(5, new long[] { 51, 52, 53, 54 });

            A.CallTo(() => solexBllCalosc.CechyAtrybuty.SlownikIdAtrybutowIIdCech).Returns(slownikAtr);
            ProfilKlientaDostep pkd = new ProfilKlientaDostep(solexBllCalosc);

            Dictionary<int, long[]> wartosc = new Dictionary<int, long[]>();
            wartosc.Add(1, new long[] {11});
            wartosc.Add(2, new long[] {21});

            string wartoscJson = JsonSerializer.SerializeToString(wartosc);
            ProfilKlienta pk1 = new ProfilKlienta()
            {
                Dopisek = AccesLevel.Zalogowani.ToString(),
                TypUstawienia = TypUstawieniaKlienta.StalyFiltr,
                Wartosc = wartoscJson,
                KlientId = klient.Id
            };
            bool wynik = pkd.SprawdzStalyFiltr(pk1);
            Assert.False(wynik, "Profil nie powinien się zmienić");

            Dictionary<int, long[]> wartosc2 = new Dictionary<int, long[]>();
            wartosc2.Add(1, new long[] { 11, 19 });
            wartosc2.Add(2, new long[] { 21 });

            string wartoscJson2 = JsonSerializer.SerializeToString(wartosc);
            ProfilKlienta pk2 = new ProfilKlienta()
            {
                Dopisek = AccesLevel.Zalogowani.ToString(),
                TypUstawienia = TypUstawieniaKlienta.StalyFiltr,
                Wartosc = wartoscJson2,
                KlientId = klient.Id
            };
            wynik = pkd.SprawdzStalyFiltr(pk2);
            Assert.False(wynik, "Profil powinien się zmienić");
            var warWynikowa = JsonSerializer.DeserializeFromString<Dictionary<int, HashSet<long>>>(pk2.Wartosc);
            Assert.True(warWynikowa.Count()==2, $"Wartośc powinna mieć 2 atrybuty a ma {warWynikowa.Count}");
            Assert.True(warWynikowa.First().Value.Count==1, $"Pierwsza wartośc powinna mieć 1 cechę a ma {warWynikowa.First().Value.Count}");
            Assert.True(warWynikowa.Last().Value.Count==1, $"Druga wartośc powinna mieć 1 cechę a ma {warWynikowa.Last().Value.Count}");

            Dictionary<int, long[]> wartosc3 = new Dictionary<int, long[]>();
            wartosc3.Add(1, new long[] { 11, 19 });
            wartosc3.Add(2, new long[] { 21 });
            wartosc3.Add(9, new long[] { 91 });

            string wartoscJson3 = JsonSerializer.SerializeToString(wartosc);
            ProfilKlienta pk3 = new ProfilKlienta()
            {
                Dopisek = AccesLevel.Zalogowani.ToString(),
                TypUstawienia = TypUstawieniaKlienta.StalyFiltr,
                Wartosc = wartoscJson3,
                KlientId = klient.Id
            };
            wynik = pkd.SprawdzStalyFiltr(pk3);
            Assert.False(wynik, "Profil powinien się zmienić");
            warWynikowa = JsonSerializer.DeserializeFromString<Dictionary<int, HashSet<long>>>(pk3.Wartosc);
            Assert.True(warWynikowa.Count() == 2, $"Wartośc powinna mieć 2 atrybuty a ma {warWynikowa.Count}");
            Assert.True(warWynikowa.First().Value.Count == 1, $"Pierwsza wartośc powinna mieć 1 cechę a ma {warWynikowa.First().Value.Count}");
            Assert.True(warWynikowa.Last().Value.Count == 1, $"Druga wartośc powinna mieć 1 cechę a ma {warWynikowa.Last().Value.Count}");
        }

        [Fact(DisplayName = "Sprawdzanie poprawności listy profili")]
        public void SprawdzProfileTest()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            IKlient klient = A.Fake<IKlient>();
            klient.Id = 1;

            Dictionary<int, long[]> slownikAtr = new Dictionary<int, long[]>();
            slownikAtr.Add(1, new long[] { 11, 12, 13, 14 });
            slownikAtr.Add(2, new long[] { 21, 22, 23, 24 });
            slownikAtr.Add(3, new long[] { 31, 32, 33, 34 });
            slownikAtr.Add(4, new long[] { 41, 42, 43, 44 });
            slownikAtr.Add(5, new long[] { 51, 52, 53, 54 });

            A.CallTo(() => solexBllCalosc.CechyAtrybuty.SlownikIdAtrybutowIIdCech).Returns(slownikAtr);
            ProfilKlientaDostep pkd = new ProfilKlientaDostep(solexBllCalosc);

            Dictionary<int, long[]> wartosc = new Dictionary<int, long[]>();
            wartosc.Add(1, new long[] { 11 });
            wartosc.Add(2, new long[] { 21 });

            string wartoscJson = JsonSerializer.SerializeToString(wartosc);
            ProfilKlienta pk1 = new ProfilKlienta()
            {
                Dopisek = AccesLevel.Zalogowani.ToString(),
                TypUstawienia = TypUstawieniaKlienta.StalyFiltr,
                Wartosc = wartoscJson,
                KlientId = klient.Id
            };
            ProfilKlienta pk2 = new ProfilKlienta()
            {
                Dopisek = AccesLevel.Zalogowani.ToString(),
                TypUstawienia = TypUstawieniaKlienta.RozmiarStronyListaProduktow,
                Wartosc = "10",
                KlientId = klient.Id
            };
            ProfilKlienta pk3 = new ProfilKlienta()
            {
                Dopisek = AccesLevel.Zalogowani.ToString(),
                TypUstawienia = TypUstawieniaKlienta.DokumentyTylkoNiezaplacone,
                Wartosc = "True",
                KlientId = klient.Id
            };
            var wynik = pkd.SprawdzProfile(new List<ProfilKlienta>() {pk2, pk3});
            Assert.False(wynik.Any(), $"Do aktualizacji nie powinno być profili a jest {wynik.Count}");

            wynik = pkd.SprawdzProfile(new List<ProfilKlienta>() {pk1, pk2, pk3 });
            Assert.True(wynik.Count == 0, $"Do aktualizacji powinnien być jeden profil a jest {wynik.Count}");
        }
    }
}
