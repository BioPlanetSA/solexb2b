using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Interfaces.Modele;
using Xunit;
using SolEx.Hurt.Core.BLL;

namespace SolEx.Hurt.Core.BLL.Tests
{
    public class SzukanieTests
    {
        //[Fact(DisplayName = "Test sprawdzajacy poprawnosc metody sortujacej obiekty wzgledem okreslonego propertisa")]
        //public void SortujObiektyTest()
        //{
        //    Szukanie sz = new Szukanie();
        //    var k1 = A.Fake<IKlienci>();
        //    A.CallTo(() => k1.Id).Returns(1);
        //    k1.email = "JakisEmail@gmail.com";
        //    k1.BlokadaPowod = BlokadaPowod.ZmianaAdresuIp;
        //    k1.IndywidualnaStawaVat = 19;
        //    k1.adres_wysylki_id = 2;

        //    var k2 = A.Fake<IKlienci>();
        //    A.CallTo(() => k2.Id).Returns(2);
        //    k2.email = "DrugiKlientEmail@gmail.pl";
        //    k2.BlokadaPowod = BlokadaPowod.BrakFaktur;
        //    k2.IndywidualnaStawaVat = 23;
        //    k2.adres_wysylki_id = 18;
        //    List<IKlienci> listaKlientow = new List<IKlienci>(){k1,k2};

        //    List<IKlienci> wynik1 = sz.SortujObiekty(listaKlientow, "BlokadaPowod", KolejnoscSortowania.asc).ToList();
        //    List<IKlienci> wynik2 = sz.SortujObiekty(listaKlientow, "IndywidualnaStawaVat", KolejnoscSortowania.asc).ToList();
        //    List<IKlienci> wynik3 = sz.SortujObiekty(listaKlientow, "adres_wysylki_id", KolejnoscSortowania.asc).ToList();
        //    List<IKlienci> wynik4 = sz.SortujObiekty(listaKlientow, "email", KolejnoscSortowania.asc).ToList();

        //    List<IKlienci> wynik5 = sz.SortujObiekty(listaKlientow, "BlokadaPowod", KolejnoscSortowania.desc).ToList();
        //    List<IKlienci> wynik6 = sz.SortujObiekty(listaKlientow, "IndywidualnaStawaVat", KolejnoscSortowania.desc).ToList();
        //    List<IKlienci> wynik7 = sz.SortujObiekty(listaKlientow, "adres_wysylki_id", KolejnoscSortowania.desc).ToList();
        //    List<IKlienci> wynik8 = sz.SortujObiekty(listaKlientow, "email", KolejnoscSortowania.desc).ToList();

        //    Assert.True(wynik1[0].Id == 2);
        //    Assert.True(wynik2[0].Id == 1);
        //    Assert.True(wynik3[0].Id == 1);
        //    Assert.True(wynik4[0].Id == 2);

        //    Assert.True(wynik5[0].Id == 1);
        //    Assert.True(wynik6[0].Id == 2);
        //    Assert.True(wynik7[0].Id == 2);
        //    Assert.True(wynik8[0].Id == 1);
        //}

        [Fact(DisplayName = "Test sprawdzajacy poprawność dzialania metody wyszukujacej obiekty")]
        public void WyszukajObiektyTest()
        {
            Szukanie sz = new Szukanie();
            var k1 = A.Fake<IKlienci>();
            A.CallTo(() => k1.Id).Returns(1);
            k1.Email = "JakisEmail@gmail.com";
            k1.PowodBlokady = BlokadaPowod.ZmianaAdresuIp;
            k1.IndywidualnaStawaVat = 19;
           // k1.AdresWysylkiId = 2;

            var k2 = A.Fake<IKlienci>();
            A.CallTo(() => k2.Id).Returns(2);
            k2.Email = "DrugiKlientEmail@gmail.pl";
            k2.PowodBlokady = BlokadaPowod.BrakFaktur;
            k2.IndywidualnaStawaVat = 19;
          //  k2.AdresWysylkiId = 18;


            var k3 = A.Fake<IKlienci>();
            A.CallTo(() => k3.Id).Returns(3);
            k3.Email = "DrugiKlientEmail@o2.pl";
            k3.PowodBlokady = BlokadaPowod.Brak;
            k3.IndywidualnaStawaVat = 19;
          //  k3.AdresWysylkiId = 10;
            List<IKlienci> listaKlientow = new List<IKlienci>() { k1, k2,k3 };

            var config = A.Fake<ConfigBLL>();
            A.CallTo(() => config.PodczasWyszukiwaniaZmienPolskeZnaki).Returns(true);
            A.CallTo(() => config.SzumyWyszukiwania).Returns("- ");
            sz.Config = config;

            var wynik = sz.WyszukajObiekty(listaKlientow, "gmail.com 19", new[] { "email", "IndywidualnaStawaVat" }).ToList();
            Assert.True(wynik.Count==1 && wynik[0].Id==1);

            var wynik2 = sz.WyszukajObiekty(listaKlientow, "gmail 19", new[] { "email", "IndywidualnaStawaVat" }).ToList();
            Assert.True(wynik2.Count == 2 && wynik2[1].Id == 2);

            //var wynik3 = sz.WyszukajObiekty(listaKlientow, "gmail 18 19", new[] { "email", "IndywidualnaStawaVat", "adres_wysylki_id" }).ToList();
            //Assert.True(wynik3.Count == 1 && wynik3[0].Id == 2);
        }
        private List<Klient> TworzDane(int ile)
        {
            List<Klient> klieni = new List<Klient>();
            for (int i = 0; i < ile; i++)
            {

                Klient k2 = A.Fake<Klient>();
                A.CallTo(() => k2.Id).Returns(ile-i);
                k2.Nazwa = "klient " + (i+1);
                k2.DataDodatnia = DateTime.Now.AddDays(i % 2 == 0 ? -i : i);
                klieni.Add(k2);


            }
            return klieni;

        }
            [Fact(DisplayName = "Test dynamiczego zapytania ")]
        public void StworzWhereTest()
        {
           Szukanie s=new Szukanie();
            IConfigBLL config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.PodczasWyszukiwaniaZmienPolskeZnaki).Returns(false);
            A.CallTo(() => config.SzumyWyszukiwania).Returns("- ");
            s.Config = config;
            var dane = TworzDane(10);
            var warunek = s.StworzWhere<IKlient>(new[] { "nazwa" },new []{ "klient1"});
            var wynik = dane.Where((Func<IKlient, bool>) warunek).ToList();
            Assert.Equal(2,wynik.Count);
        }
            [Fact(DisplayName = "Test dynamiczego zapytania - jedna z fraz pusta ")]
            public void StworzWhereJednaPustaTest()
            {
                Szukanie s = new Szukanie();
                IConfigBLL config = A.Fake<IConfigBLL>();
                A.CallTo(() => config.PodczasWyszukiwaniaZmienPolskeZnaki).Returns(false);
                A.CallTo(() => config.SzumyWyszukiwania).Returns("- ");
                s.Config = config;
                var dane = TworzDane(10);
                var warunek = s.StworzWhere<IKlient>(new[] { "nazwa" }, new[] { "klient1","" });
                var wynik = dane.Where((Func<IKlient, bool>)warunek).ToList();
                Assert.Equal(2, wynik.Count);
            }
            [Fact(DisplayName = "Test dynamiczego zapytania pole liczba")]
            public void StworzWhereTestPoleLiczba()
            {
                Szukanie s = new Szukanie();
                IConfigBLL config = A.Fake<IConfigBLL>();
                A.CallTo(() => config.PodczasWyszukiwaniaZmienPolskeZnaki).Returns(false);
                A.CallTo(() => config.SzumyWyszukiwania).Returns("- ");
                s.Config = config;
                var dane = TworzDane(10);
                var warunek = s.StworzWhere<Klient>(new[] { "Id" }, new[] { "1" });
                var wynik = dane.Where((Func<Klient, bool>)warunek).ToList();
                Assert.Equal(1, wynik.Count);
            }
            [Fact(DisplayName = "Test dynamiczego zapytania - wersja generyczna")]
            public void StworzWhereTestGeneric()
            {
                Szukanie s = new Szukanie();
                IConfigBLL config = A.Fake<IConfigBLL>();
                A.CallTo(() => config.PodczasWyszukiwaniaZmienPolskeZnaki).Returns(false);
                A.CallTo(() => config.SzumyWyszukiwania).Returns("- ");
                s.Config = config;
                var dane = TworzDane(10);
                var warunek = s.StworzWhere<IKlient>( new[] { "nazwa" }, new[] { "klient1" });
                var wynik = dane.Where(warunek).ToList();
                Assert.Equal(2, wynik.Count);
            }
            [Fact(DisplayName = "Test dynamiczego zapytania - dwa parametry")]
            public void StworzWhereTest2()
            {
                Szukanie s = new Szukanie();
                IConfigBLL config = A.Fake<IConfigBLL>();
                A.CallTo(() => config.PodczasWyszukiwaniaZmienPolskeZnaki).Returns(false);
                A.CallTo(() => config.SzumyWyszukiwania).Returns("- ");
                s.Config = config;
                var dane = TworzDane(10);
                var warunek = s.StworzWhere<IKlient>( new[] { "nazwa" }, new[] { "klient","1" });
                var wynik = dane.Where((Func<IKlient, bool>)warunek).ToList();
                Assert.Equal(2, wynik.Count);
            }
        [Fact(DisplayName = "Test dynamiczego zapytania - szuaknie daty pomiędzy")]
        public void StworzWhereTestDaty()
        {
            Szukanie s = new Szukanie();
            IConfigBLL config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.PodczasWyszukiwaniaZmienPolskeZnaki).Returns(false);
            A.CallTo(() => config.SzumyWyszukiwania).Returns("- ");
            s.Config = config;
            var dane = TworzDane(10);
            string[] szukanie = new[] { DateTime.Now.Date.AddDays(-3)+";"+ DateTime.Now.Date.AddDays(3)};
            var warunek = s.StworzWhere<Klient>( new[] { "DataDodatnia" }, szukanie);
            var wynik = dane.Where((Func<Klient, bool>)warunek).ToList();
            Assert.Equal(3, wynik.Count);
        }
        [Fact(DisplayName = "Test dynamiczego zapytania - szuaknie daty od")]
        public void StworzWhereTestDatyOd()
        {
            Szukanie s = new Szukanie();
            IConfigBLL config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.PodczasWyszukiwaniaZmienPolskeZnaki).Returns(false);
            A.CallTo(() => config.SzumyWyszukiwania).Returns("- ");
            s.Config = config;
            var dane = TworzDane(10);
            string[] szukanie = new[] { DateTime.Now.Date.AddDays(-3) + ";"};
            var warunek = s.StworzWhere<Klient>( new[] { "DataDodatnia" }, szukanie);
            var wynik = dane.Where((Func<Klient, bool>)warunek).ToList();
            Assert.Equal(7, wynik.Count);
        }
        [Fact(DisplayName = "Test dynamiczego zapytania - szuaknie daty Do")]
        public void StworzWhereTestDatyDd()
        {
            Szukanie s = new Szukanie();
            IConfigBLL config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.PodczasWyszukiwaniaZmienPolskeZnaki).Returns(false);
            A.CallTo(() => config.SzumyWyszukiwania).Returns("- ");
            s.Config = config;
            var dane = TworzDane(10);
            string[] szukanie = new[] { ";" + DateTime.Now.Date.AddDays(3) };
            var warunek = s.StworzWhere<Klient>( new[] { "DataDodatnia" }, szukanie);
            var wynik = dane.Where((Func<Klient, bool>)warunek).ToList();
            Assert.Equal(6, wynik.Count);
        }
        [Fact(DisplayName = "Test dynamiczego zapytania - szuaknie daty pomiędzy, oraz wg pola")]
        public void StworzWhereTLaczie()
        {
            Szukanie s = new Szukanie();
            IConfigBLL config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.PodczasWyszukiwaniaZmienPolskeZnaki).Returns(false);
            A.CallTo(() => config.SzumyWyszukiwania).Returns("- ");
            s.Config = config;
            var dane = TworzDane(10);
            string[] szukanie = { DateTime.Now.Date.AddDays(-3) + ";" + DateTime.Now.Date.AddDays(3), "klient1" };
            var warunek = s.StworzWhere<Klient>( new[] { "nazwa", "DataDodatnia" }, szukanie);
            var wynik = dane.Where((Func<Klient, bool>)warunek).ToList();
            Assert.Equal(1, wynik.Count);
        }
        [Fact(DisplayName = "Test dynamiczego zapytania - szuaknie daty kontrketnej")]
        public void StworzWhereTestDatyKontrketna()
        {
            Szukanie s = new Szukanie();
            IConfigBLL config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.PodczasWyszukiwaniaZmienPolskeZnaki).Returns(false);
            A.CallTo(() => config.SzumyWyszukiwania).Returns("- ");
            s.Config = config;
            var dane = TworzDane(10);
            dane[0].DataDodatnia = DateTime.Now.Date.AddHours(1);
            string []szukanie =new []{ DateTime.Now.Date.AddHours(1).ToString()};
            var warunek = s.StworzWhere<Klient>( new[] { "DataDodatnia" }, szukanie);
            var wynik = dane.Where((Func<Klient, bool>)warunek).ToList();
            Assert.Equal(1, wynik.Count);
        }
        [Fact(DisplayName = "Test dynamiczego zapytania -konkretna fraza w kontetnym polu")]
        public void StworzWhereTestDatyKontrketnaFrazaKonkretnePole()
        {
            Szukanie s = new Szukanie();
            IConfigBLL config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.PodczasWyszukiwaniaZmienPolskeZnaki).Returns(false);
            A.CallTo(() => config.SzumyWyszukiwania).Returns("- ");
            s.Config = config;
            var dane = TworzDane(10);
         
            string[] szukanie =  {"klient","8" };
            var warunek = s.StworzWhere<Klient>( new[] { "nazwa","Id" }, szukanie,true);
            var wynik = dane.Where((Func<Klient, bool>)warunek).ToList();
            Assert.Equal(1, wynik.Count);
        }
        [Fact(DisplayName = "Test dynamiczego zapytania -konkretna fraza w kontetnym polu,zła liczba parametrów")]
        public void StworzWhereFrazaKonkretnePoleWyjatek()
        {
            Szukanie s = new Szukanie();
            IConfigBLL config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.PodczasWyszukiwaniaZmienPolskeZnaki).Returns(false);
            A.CallTo(() => config.SzumyWyszukiwania).Returns("- ");
            s.Config = config;
            string[] szukanie = { "klient", "8" };
           Assert.Throws<InvalidOperationException>(()=> s.StworzWhere< IKlient>(new[] { "nazwa",  }, szukanie, true));
          
        }
        [Fact(DisplayName = "Test dynamiczego zapytania - pusta liczba parametrów")]
        public void StworzWherePusteListaParametrow()
        {
            Szukanie s = new Szukanie();
            IConfigBLL config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.PodczasWyszukiwaniaZmienPolskeZnaki).Returns(false);
            A.CallTo(() => config.SzumyWyszukiwania).Returns("- ");
            s.Config = config;
            var dane = TworzDane(10);

            string[] szukanie = {  };
            var warunek = s.StworzWhere<IKlient>(new[] { "nazwa", "Id" }, szukanie);
            var wynik = dane.Where((Func<IKlient, bool>)warunek).ToList();
            Assert.Equal(10, wynik.Count);

        }
        [Fact(DisplayName = "Test dynamiczego zapytania -szukanie pustego stringa")]
        public void StworzWhereSzukaniePustegoStringa()
        {
            Szukanie s = new Szukanie();
            IConfigBLL config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.PodczasWyszukiwaniaZmienPolskeZnaki).Returns(false);
            A.CallTo(() => config.SzumyWyszukiwania).Returns("- ");
            s.Config = config;
            var dane = TworzDane(10);

            string[] szukanie = {"" };
            var warunek = s.StworzWhere<IKlient>( new[] { "nazwa", "Id" }, szukanie);
            var wynik = dane.Where((Func<IKlient, bool>)warunek).ToList();
            Assert.Equal(10, wynik.Count);

        }
        [Fact(DisplayName = "Test dynamiczego zapytania -konkretna fraza w kontetnym polu wersja 2")]
        public void StworzWhereTestDatyKontrketnaFrazaKonkretnePole2()
        {
            Szukanie s = new Szukanie();
            IConfigBLL config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.PodczasWyszukiwaniaZmienPolskeZnaki).Returns(false);
            A.CallTo(() => config.SzumyWyszukiwania).Returns("- ");
            s.Config = config;
            var dane = TworzDane(10);

            string[] szukanie = { "klient", "8" };
            var warunek = s.StworzWhere<Klient>(new[] { "Id", "nazwa" }, szukanie, true);
            var wynik = dane.Where((Func<Klient, bool>)warunek).ToList();
            Assert.Equal(0, wynik.Count);
        }
        [Fact(DisplayName = "Test dynamiczego zapytania - sortowanie")]
        public void WygenerujSortowanieTest()
        {
            Szukanie s = new Szukanie();
            IConfigBLL config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.PodczasWyszukiwaniaZmienPolskeZnaki).Returns(false);
            A.CallTo(() => config.SzumyWyszukiwania).Returns("- ");
            s.Config = config;
            var dane = TworzDane(10);

            var warunek = s.WygenerujSortowanie(typeof (IKlient), "nazwa");
            var wynik = dane.OrderByDescending((Func<IKlient,object>)warunek).ToList();
            Assert.Equal("klient 9", wynik[0].Nazwa);
        }

        [Fact(DisplayName = "Test dynamiczego zapytania - sortowanie generyczne")]
        public void WygenerujSortowanieTestGeneric()
        {
            Szukanie s = new Szukanie();
            IConfigBLL config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.PodczasWyszukiwaniaZmienPolskeZnaki).Returns(false);
            A.CallTo(() => config.SzumyWyszukiwania).Returns("- ");
            s.Config = config;
            var dane = TworzDane(10);

            var warunek = s.WygenerujSortowanie<IKlient>( "nazwa");
            var wynik = dane.OrderByDescending(warunek).ToList();
            Assert.Equal("klient 9", wynik[0].Nazwa);
        }
        [Fact(DisplayName = "Test dynamiczego zapytania - sortowanie generyczne, wg liczby")]
        public void WygenerujSortowanieTestGenericLiczba()
        {
            Szukanie s = new Szukanie();
            IConfigBLL config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.PodczasWyszukiwaniaZmienPolskeZnaki).Returns(false);
            A.CallTo(() => config.SzumyWyszukiwania).Returns("- ");
            s.Config = config;
            var dane = TworzDane(10);

            var warunek = s.WygenerujSortowanie<Klient>("Id");
            var wynik = dane.OrderByDescending(warunek).ToList();
            Assert.Equal(10, wynik[0].Id);
        }
    }
}
