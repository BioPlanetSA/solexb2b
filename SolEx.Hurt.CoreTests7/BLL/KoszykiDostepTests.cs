using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces;
using Xunit;

namespace SolEx.Hurt.CoreTests.BLL
{
    public class KoszykiDostepTests
    {
        [Fact(DisplayName = "Sprawdzenie wymaganego OZ")]
        public void UzupelnijJednostkiOKrokDodaniaTest()
        {
            IProduktKlienta pb = A.Fake<IProduktKlienta>();
            pb.IloscWOpakowaniu = 1;
            pb.WymaganeOz = true;
            A.CallTo(() => pb.JednostkaPodstawowa.Id).Returns(1);

            IProduktKlienta pb2 = A.Fake<IProduktKlienta>();
            pb2.IloscWOpakowaniu = 6;
            pb2.WymaganeOz = true;
            A.CallTo(() => pb2.JednostkaPodstawowa.Id).Returns(1);

            IProduktKlienta pb3 = A.Fake<IProduktKlienta>();
            pb3.IloscWOpakowaniu = 1;
            pb3.WymaganeOz = false;
            A.CallTo(() => pb3.JednostkaPodstawowa.Id).Returns(1);

            IProduktKlienta pb4 = A.Fake<IProduktKlienta>();
            pb4.IloscWOpakowaniu = 6;
            pb4.WymaganeOz = false;
            A.CallTo(() => pb4.JednostkaPodstawowa.Id).Returns(1);

            List<Tuple<JednostkaProduktu, IProduktKlienta, decimal>> jednostkiTestowe = new List<Tuple<JednostkaProduktu, IProduktKlienta, decimal>>
            {
                //wymaganeOz 
                new Tuple<JednostkaProduktu, IProduktKlienta, decimal>(new JednostkaProduktu() {Id = 1, Przelicznik = 1, Calkowitoliczowa = false, Zaokraglenie = 2}, pb, 1),
                new Tuple<JednostkaProduktu, IProduktKlienta, decimal>(new JednostkaProduktu() {Id = 2, Przelicznik = 0.5m, Calkowitoliczowa = true, Zaokraglenie = 2}, pb, 2),
                new Tuple<JednostkaProduktu, IProduktKlienta, decimal>(new JednostkaProduktu() {Id = 3, Przelicznik = 0.5m, Calkowitoliczowa = false, Zaokraglenie = 2}, pb, 2),
                new Tuple<JednostkaProduktu, IProduktKlienta, decimal>(new JednostkaProduktu() {Id = 4, Przelicznik = 0.4m, Calkowitoliczowa = false, Zaokraglenie = 2}, pb, 2.5m),
                new Tuple<JednostkaProduktu, IProduktKlienta, decimal>(new JednostkaProduktu() {Id = 5, Przelicznik = 12, Calkowitoliczowa = true, Zaokraglenie = 2}, pb, 1),
                new Tuple<JednostkaProduktu, IProduktKlienta, decimal>(new JednostkaProduktu() {Id = 6, Przelicznik = 24, Calkowitoliczowa = false, Zaokraglenie = 2}, pb, 0.04m),

                //new Tuple<JednostkaProduktu, IProduktKlienta, decimal>(new JednostkaProduktu() {Id = 7, Przelicznik = 24, Calkowitoliczowa = false, Zaokraglenie = 2}, pb2, 4), // nie potrzebujemy 4 tylko 6/24 czyli 0.25 bo wymagane jest 6 a nie 144
                new Tuple<JednostkaProduktu, IProduktKlienta, decimal>(new JednostkaProduktu() {Id = 7, Przelicznik = 24, Calkowitoliczowa = false, Zaokraglenie = 2}, pb2, 0.25m),
                new Tuple<JednostkaProduktu, IProduktKlienta, decimal>(new JednostkaProduktu() {Id = 8, Przelicznik = 24, Calkowitoliczowa = true, Zaokraglenie = 2}, pb2, 1),
                new Tuple<JednostkaProduktu, IProduktKlienta, decimal>(new JednostkaProduktu() {Id = 9, Przelicznik = 25, Calkowitoliczowa = false, Zaokraglenie = 2}, pb2, 0.24m),
                new Tuple<JednostkaProduktu, IProduktKlienta, decimal>(new JednostkaProduktu() {Id = 10, Przelicznik = 26, Calkowitoliczowa = false, Zaokraglenie = 4}, pb2, 0.2308m),

                //nie wymaganeOz
                new Tuple<JednostkaProduktu, IProduktKlienta, decimal>(new JednostkaProduktu() {Id = 11, Przelicznik = 0.5m, Calkowitoliczowa = true, Zaokraglenie = 2}, pb3, 1),
                new Tuple<JednostkaProduktu, IProduktKlienta, decimal>(new JednostkaProduktu() {Id = 12, Przelicznik = 0.5m, Calkowitoliczowa = false, Zaokraglenie = 2}, pb3, 0.5m),
                new Tuple<JednostkaProduktu, IProduktKlienta, decimal>(new JednostkaProduktu() {Id = 13, Przelicznik = 1, Calkowitoliczowa = false, Zaokraglenie = 2}, pb3, 1),
                new Tuple<JednostkaProduktu, IProduktKlienta, decimal>(new JednostkaProduktu() {Id = 14, Przelicznik = 0.4m, Calkowitoliczowa = false, Zaokraglenie = 2}, pb3, 0.4m),
                new Tuple<JednostkaProduktu, IProduktKlienta, decimal>(new JednostkaProduktu() {Id = 15, Przelicznik = 12, Calkowitoliczowa = true, Zaokraglenie = 2}, pb3, 1),
                new Tuple<JednostkaProduktu, IProduktKlienta, decimal>(new JednostkaProduktu() {Id = 16, Przelicznik = 24, Calkowitoliczowa = false, Zaokraglenie = 2}, pb3, 0.04m),

                new Tuple<JednostkaProduktu, IProduktKlienta, decimal>(new JednostkaProduktu() {Id = 17, Przelicznik = 24, Calkowitoliczowa = false, Zaokraglenie = 2}, pb4, 0.25m),
                new Tuple<JednostkaProduktu, IProduktKlienta, decimal>(new JednostkaProduktu() {Id = 18, Przelicznik = 24, Calkowitoliczowa = true, Zaokraglenie = 2}, pb4, 1),
                new Tuple<JednostkaProduktu, IProduktKlienta, decimal>(new JednostkaProduktu() {Id = 19, Przelicznik = 25, Calkowitoliczowa = false, Zaokraglenie = 2}, pb4, 0.24m),
                new Tuple<JednostkaProduktu, IProduktKlienta, decimal>(new JednostkaProduktu() {Id = 20, Przelicznik = 26, Calkowitoliczowa = false, Zaokraglenie = 4}, pb4, 0.2308m),
            };

            var koszykTestowy = new KoszykiDostep(SolexBllCalosc.PobierzInstancje);

            foreach (var test in jednostkiTestowe)
            {
                var wynik = koszykTestowy.UzupelnijJednostkiOKrokDodania(new List<JednostkaProduktu>() {test.Item1}, test.Item2);
                Assert.True(wynik.First().Krok == test.Item3,string.Format("Id: {0}, całkowitoliczbowa: {3}, Krok: {1}, Wymagane: {2}",wynik.First().Id, wynik.First().Krok.DoLadnejCyfry(), test.Item3, wynik.First().Calkowitoliczowa));
            }
        }

        [Fact(DisplayName = "Sprawdzanie wyliczonej ilości")]
        public void SprawdzIloscTest()
        {
            SprawdzIlosc(2, 0, false, 3, true, false, 3, 2);
            SprawdzIlosc(2, 0, false, 3, true, false, 0, 4);
            SprawdzIlosc(2, 0, false, 3, true, false, 0, 3);
            SprawdzIlosc(2, 0, false, 1, true, false, 2, 2);
            SprawdzIlosc(2, 2, true, 1, true, false, 2, 2);
            SprawdzIlosc(2, 2, true, 1, true, false, 2, 3);
            SprawdzIlosc(1, 2, true, 1, true, false, 0, 2);
            SprawdzIlosc(2, 2, true, 3, true, false, 4, 2);
            SprawdzIlosc(2.55M, 2, true, 2, true, false, 4, 2);//tu jest błąd opakowanie jest 2 sztuki i jest wymagane więć wynik p[owienien być 4 a nie 2.55
            SprawdzIlosc(0.3M, 1, true, 1, true, true, 1, 2);
            SprawdzIlosc(4, 6, true, 6, true, true, 6, 2);
            SprawdzIlosc(2, 0, false, 3, false, false, 2, 2);
            SprawdzIlosc(2, 0, false, 1, false, false, 2, 2);
            SprawdzIlosc(2, 2, true, 1, false, false, 2, 2);
            SprawdzIlosc(2, 2, true, 3, false, false, 2, 2);
            SprawdzIlosc(2.55M, 2, true, 2, false, true, 4, 2);
            SprawdzIlosc(0.3M, 1, true, 1, false, true, 1, 2);
            SprawdzIlosc(4, 6, true, 6, false, false, 6, 2);
            SprawdzIlosc(34444, 43, true, 43, true, true, 9999, 2);

            SprawdzIlosc(3,6,true,1,true,false,6,0);
            SprawdzIlosc(3,0.4m,true,0.4m,true,false,3.2m,0);
            SprawdzIlosc(3.6m, 0.4m, true, 0.4m, true, false, 3.6m, 0);
            //SprawdzIlosc(3, 0.4m, false, 0.4m, true, false, 3.2m, 0);
            SprawdzIlosc(0.2m,0.4m,false,0.5m,true,false,0.5m,0);

        
            //sprawdzanie zaokrąglenia względem jednostki
            SprawdzIlosc(0.44444444m,0,false,0,true,false,0.45m,0);
            SprawdzIlosc(0.44444444m, 0.5m, true, 0, true, false, 0.5m, 0);

        }

        private void SprawdzIlosc(decimal iloscObecna, decimal oz, bool wymaganeOz, decimal iloscminimalna, bool jednostkaPodstowa, bool calkowitoliczbowa, decimal iloscOczekiwana, decimal poprzednia, decimal? limit = null)
        {
            ProduktBazowy pb = A.Fake<ProduktBazowy>();
            pb.Id = 1;
            pb.IloscWOpakowaniu = oz;
            pb.WymaganeOz = wymaganeOz;
            pb.IloscMinimalna = iloscminimalna;
            pb.JezykId = 1;
            int idjednostki = 1;
            IKlient klient = A.Fake<IKlient>();
            ProduktKlienta pk = A.Fake<ProduktKlienta>(a => a.WithArgumentsForConstructor(new Object[] { pb, klient }));
            A.CallTo(() => pk.IloscLaczna).Returns(20);
            A.CallTo(() => pk.DostepnyLimit).Returns(limit);

            JednostkaProduktu jednostka = new JednostkaProduktu { Przelicznik = 1, Calkowitoliczowa = calkowitoliczbowa, Podstawowa = jednostkaPodstowa, Id = 1, Zaokraglenie = 2};
            List<JednostkaProduktu> jednostki = new List<JednostkaProduktu>();
            jednostki.Add(jednostka);
            if (!jednostkaPodstowa)
            {
                var jednostka2 = new JednostkaProduktu { Przelicznik = 1, Calkowitoliczowa = false, Podstawowa = true, Id = 2, Zaokraglenie = 2};

                jednostki.Add(jednostka2);
            }

            A.CallTo(() => pk.Jednostki).Returns(jednostki);
            A.CallTo(() => pk.JednostkaPodstawowa).Returns(jednostki.FirstOrDefault(x => x.Podstawowa));

            ISolexBllCalosc sol = A.Fake<ISolexBllCalosc>();
            A.CallTo(() => sol.ProduktyJednostkiDostep.PobierzJednostkiProduktuWgProduktu(1)).Returns(null);
           // A.CallTo(() => sol.Konfiguracja.MinimumLogistyczneWymagane).Returns(true);

            KoszykiDostep kd = new KoszykiDostep(sol);
            decimal po = kd.SprawdzIlosc(pk, idjednostki, iloscObecna, poprzednia);

            bool wynik = iloscOczekiwana == po;
            Assert.True(wynik, string.Format("Calkowitoliczbowa={0},limity {6},ilosc_w_opakownaiu={4}, wymaganie_oz={5},wejsciowa {3} oczekiwana {1}, otrzymana{2},podstawowa {7} poprzednia {8} "
                 , calkowitoliczbowa, iloscOczekiwana, po, iloscObecna, oz, wymaganeOz, limit, jednostkaPodstowa, poprzednia));
        }

        //[Fact(DisplayName = "Sprawdzenie przekroczonego stanu")]
        //public void CzyJestPrzektoczonyStanTest()
        //{
        //    TestPrzekroczone1();
        //    TestPrzekroczone2();
        //    TestPrzekroczone3();
        //    TestPrzekroczone4();
        //}
        //private void TestPrzekroczone1()
        //{
        //var ConfigBllFake = A.Fake<IConfigBLL>();
        //var stany = A.Fake<ISposobyPokazywaniaStanowBLL>();
        //var zadania = A.Fake<IZadaniaBLL>();
        //A.CallTo(() => stany.SaStany(null, null)).Returns(true);
        //var kd = A.Fake<KoszykiDostep>();
        //kd.Konfiguracja = ConfigBllFake;
        //kd.Stany = stany;
        //kd.Zadania = zadania;
        //var pk = A.Fake<ProduktBazowy>(a => a.WithArgumentsForConstructor(new Object[] { new ProduktBazowy(1), 1 }));
        //A.CallTo(() => pk.IloscLaczna).Returns(20);

        //var jednostka = new JednostkaProduktu() { Przelicznik = 1, Calkowitoliczowa = true, Podstawowa = true, JednostkaId = 1 };

        //var kp_baza = new koszyki_pozycje() { jednostka = jednostka.JednostkaId, ilosc = 5 };
        //KoszykPozycje pozycjaBLL = A.Fake<KoszykPozycje>(a => a.WithArgumentsForConstructor(new Object[] { kp_baza, 1, new Klient(null) { klient_id = 1 }, jednostka, null }));

        //A.CallTo(() => pozycjaBLL.Produkt).Returns(pk);

        //var koszyk = new KoszykiBLL(null, 1, new List<IKoszykPozycja> { pozycjaBLL });
        //A.CallTo(() => kd.CzyJestWlaczonyModulPrzekroczonychStanow).Returns(false);

        //var wynik = kd.CzyJestPrzekroczonyStan(koszyk, pozycjaBLL);

        //Assert.False(wynik, "stany nie przekroczone, bez zadania  i zdefiniowanymi sposoobami pokazywania stanów");
        //}
        //private void TestPrzekroczone2()
        //{
        //var ConfigBllFake = A.Fake<IConfigBLL>();
        //var stany = A.Fake<ISposobyPokazywaniaStanowBLL>();
        //var zadania = A.Fake<IZadaniaBLL>();
        //A.CallTo(() => stany.SaStany(null, null)).Returns(true);
        //var kd = A.Fake<KoszykiDostep>();
        //kd.Konfiguracja = ConfigBllFake;
        //kd.Stany = stany;
        //kd.Zadania = zadania;
        //var pk = A.Fake<ProduktBazowy>(a => a.WithArgumentsForConstructor(new Object[] { new ProduktBazowy(1), 1 }));
        //A.CallTo(() => pk.IloscLaczna).Returns(20);

        //var jednostka = new JednostkaProduktu() { Przelicznik = 1, Calkowitoliczowa = true, Podstawowa = true, JednostkaId = 1 };

        //var kp_baza = new koszyki_pozycje() { jednostka = jednostka.JednostkaId, ilosc = 30 };
        //KoszykPozycje pozycjaBLL = A.Fake<KoszykPozycje>(a => a.WithArgumentsForConstructor(new Object[] { kp_baza, 1, new Klient(null) { klient_id = 1 }, jednostka, null }));

        //A.CallTo(() => pozycjaBLL.Produkt).Returns(pk);

        //var koszyk = new KoszykiBLL(null, 1, new List<IKoszykPozycja> { pozycjaBLL });
        //A.CallTo(() => kd.CzyJestWlaczonyModulPrzekroczonychStanow).Returns(false);

        //var wynik = kd.CzyJestPrzekroczonyStan(koszyk, pozycjaBLL);
        ////wcześniej ten test sprawdzał stany przekroczone bez zadania i ze zd. spo. pok. st. ale logika tej metody przecież tego nie obsługuje - nie ma takiego przypadku
        //Assert.False(wynik, "stany nie przekroczone, bez zadania  i zdefiniowanymi sposoobami pokazywania stanów");
        //}
        //private void TestPrzekroczone3()
        //{
        //var ConfigBllFake = A.Fake<IConfigBLL>();
        //var stany = A.Fake<ISposobyPokazywaniaStanowBLL>();
        //var zadania = A.Fake<IZadaniaBLL>();

        //A.CallTo(() => stany.SaStany(null, null)).Returns(true);
        //var kd = A.Fake<KoszykiDostep>();
        //kd.Konfiguracja = ConfigBllFake;
        //kd.Stany = stany;
        //kd.Zadania = zadania;
        //var pk = A.Fake<ProduktBazowy>(a => a.WithArgumentsForConstructor(new Object[] { new ProduktBazowy(1), 1 }));
        //A.CallTo(() => pk.IloscLaczna).Returns(20);

        //var jednostka = new JednostkaProduktu() { Przelicznik = 1, Calkowitoliczowa = true, Podstawowa = true, JednostkaId = 1 };

        //var kp_baza = new koszyki_pozycje() { jednostka = jednostka.JednostkaId, ilosc = 30 };
        //KoszykPozycje pozycjaBLL = A.Fake<KoszykPozycje>(a => a.WithArgumentsForConstructor(new Object[] { kp_baza, 1, new Klient(null) { klient_id = 1 }, jednostka, null }));

        //A.CallTo(() => pozycjaBLL.Produkt).Returns(pk);

        //A.CallTo(() => kd.CzyJestWlaczonyModulPrzekroczonychStanow).Returns(true);

        //var koszyk = new KoszykiBLL(null, 1, new List<IKoszykPozycja> { pozycjaBLL });

        //A.CallTo(() => zadania.ZnajdzZadaniaWgTypu<PrzekroczoneStany>()).Returns(new List<PrzekroczoneStany> { new PrzekroczoneStany() });
        //var wynik =kd.CzyJestPrzekroczonyStan(koszyk, pozycjaBLL);

        //Assert.True(wynik, "stany przekroczone, z zadaniem i zdefiniowanymi sposoobami pokazywania stanów");
        //}
        //private void TestPrzekroczone4()
        //{
        //    var ConfigBllFake = A.Fake<IConfigBLL>();
        //var stany = A.Fake<ISposobyPokazywaniaStanowBLL>();
        //var zadania = A.Fake<IZadaniaBLL>();

        //A.CallTo(() => stany.SaStany(null, null)).Returns(false);
        //var kd = A.Fake<KoszykiDostep>();
        //kd.Konfiguracja = ConfigBllFake;
        //kd.Stany = stany;
        //kd.Zadania = zadania;
        //var pk = A.Fake<ProduktBazowy>(a => a.WithArgumentsForConstructor(new Object[] { new ProduktBazowy(1), 1 }));
        //A.CallTo(() => pk.IloscLaczna).Returns(20);

        //var jednostka = new JednostkaProduktu() { Przelicznik = 1, Calkowitoliczowa = true, Podstawowa = true, Id = 1 };

        //var kp_baza = new KoszykPozycje() { JednostkaId= jednostka.Id, Ilosc = 30 };
        //KoszykPozycje pozycjaBLL = A.Fake<KoszykPozycje>(a => a.WithArgumentsForConstructor(new Object[] { kp_baza, 1, new Klient(null) { Id = 1 }, jednostka, null }));

        //A.CallTo(() => pozycjaBLL.Produkt).Returns(pk);
        //A.CallTo(() => kd.CzyJestWlaczonyModulPrzekroczonychStanow).Returns(true);


        //var koszyk = new KoszykiBLL(null, 1, new List<IKoszykPozycja> { pozycjaBLL });

        //A.CallTo(() => zadania.PobierzZadaniaCalegoKoszykaKtorePasuja<IModulStartowy>(koszyk)).Returns(new List<ZadanieCalegoKoszyka> { new PrzekroczoneStany() });
        //var wynik = kd.CzyJestPrzekroczonyStan(koszyk, pozycjaBLL);

        //Assert.False(wynik, "stany przekroczone, z zadaniem i bez sposoobami pokazywania stanów");
        //}

    }
}
