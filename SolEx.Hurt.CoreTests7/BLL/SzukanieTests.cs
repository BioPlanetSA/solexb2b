using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FakeItEasy;
using ServiceStack.ServiceInterface.ServiceModel;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Interfaces.Modele;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.Helpers;
using Xunit;

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

        protected ISolexBllCalosc PobierzCaloscSolex()
        {
            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            var config = A.Fake<ConfigBLL>();
            A.CallTo(() => config.PodczasWyszukiwaniaZmienPolskeZnaki).Returns(true);
            A.CallTo(() => config.SzumyWyszukiwania).Returns("- ");

            A.CallTo(() => calosc.Konfiguracja).Returns(config);

            Szukanie szukanie = new Szukanie(calosc);
            A.CallTo(() => calosc.Szukanie).Returns(szukanie);

            return calosc;
        }

        [Fact(DisplayName = "Test sprawdzajacy poprawność dzialania metody wyszukujacej obiekty")]
        public void WyszukajObiektyTest()
        {
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
            List<IKlienci> listaKlientow = new List<IKlienci>() { k1, k2, k3 };



            var wynik = PobierzCaloscSolex().Szukanie.WyszukajObiekty(listaKlientow, "gmail.com", new[] { "WiadomoscEmail", "IndywidualnaStawaVat" }).ToList();
            Assert.True(wynik.Count == 1 && wynik[0].Id == 1);

            var wynik2 = PobierzCaloscSolex().Szukanie.WyszukajObiekty(listaKlientow, "gmail", new[] { "WiadomoscEmail", "IndywidualnaStawaVat" }).ToList();
            Assert.True(wynik2.Count == 2 && wynik2[1].Id == 2);
        }

        private List<Klient> TworzDane(int ile)
        {
            List<Klient> klieni = new List<Klient>();
            for (int i = 0; i < ile; i++)
            {

                Klient k2 = A.Fake<Klient>();
                A.CallTo(() => k2.Id).Returns(ile - i);
                k2.Nazwa = "klient " + (i + 1);
                k2.DataDodatnia = DateTime.Now.AddDays(i % 2 == 0 ? -i : i);
                klieni.Add(k2);


            }
            return klieni;

        }
        //    [Fact(DisplayName = "Test dynamiczego zapytania ")]
        //public void StworzWhereTest()
        //{        
        //    var dane = TworzDane(10);
        //    var warunek = PobierzCaloscSolex().Szukanie.StworzWhere<IKlient>(new[] { "nazwa" },new []{ "klient1"});
        //    var wynik = dane.Where((Func<IKlient, bool>) warunek).ToList();
        //    Assert.Equal(2,wynik.Count);
        //}
        //    [Fact(DisplayName = "Test dynamiczego zapytania - jedna z fraz pusta ")]
        //    public void StworzWhereJednaPustaTest()
        //    {            
        //        var dane = TworzDane(10);
        //        var warunek = PobierzCaloscSolex().Szukanie.StworzWhere<IKlient>(new[] { "nazwa" }, new[] { "klient1","" });
        //        var wynik = dane.Where((Func<IKlient, bool>)warunek).ToList();
        //        Assert.Equal(2, wynik.Count);
        //    }
        //    [Fact(DisplayName = "Test dynamiczego zapytania pole liczba")]
        //    public void StworzWhereTestPoleLiczba()
        //    {
        //        var dane = TworzDane(10);
        //        var warunek = PobierzCaloscSolex().Szukanie.StworzWhere<Klient>(new[] { "Id" }, new[] { "1" });
        //        var wynik = dane.Where((Func<Klient, bool>)warunek).ToList();
        //        Assert.Equal(1, wynik.Count);
        //    }
        //    [Fact(DisplayName = "Test dynamiczego zapytania - wersja generyczna")]
        //    public void StworzWhereTestGeneric()
        //    {
        //        var dane = TworzDane(10);
        //        var warunek = PobierzCaloscSolex().Szukanie.StworzWhere<IKlient>( new[] { "nazwa" }, new[] { "klient1" });
        //        var wynik = dane.Where(warunek).ToList();
        //        Assert.Equal(2, wynik.Count);
        //    }
        //    [Fact(DisplayName = "Test dynamiczego zapytania - dwa parametry")]
        //    public void StworzWhereTest2()
        //    {
        //      var dane = TworzDane(10);
        //        var warunek = PobierzCaloscSolex().Szukanie.StworzWhere<IKlient>( new[] { "nazwa" }, new[] { "klient","1" });
        //        var wynik = dane.Where((Func<IKlient, bool>)warunek).ToList();
        //        Assert.Equal(2, wynik.Count);
        //    }
        //[Fact(DisplayName = "Test dynamiczego zapytania - szuaknie daty pomiędzy")]
        //public void StworzWhereTestDaty()
        //{
        //   var dane = TworzDane(10);
        //    string[] szukanie = new[] { DateTime.Now.Date.AddDays(-3)+";"+ DateTime.Now.Date.AddDays(3)};
        //    var warunek = PobierzCaloscSolex().Szukanie.StworzWhere<Klient>( new[] { "DataDodatnia" }, szukanie);
        //    var wynik = dane.Where((Func<Klient, bool>)warunek).ToList();
        //    Assert.Equal(3, wynik.Count);
        //}
        //[Fact(DisplayName = "Test dynamiczego zapytania - szuaknie daty od")]
        //public void StworzWhereTestDatyOd()
        //{
        // var dane = TworzDane(10);
        //    string[] szukanie = new[] { DateTime.Now.Date.AddDays(-3) + ";"};
        //    var warunek = PobierzCaloscSolex().Szukanie.StworzWhere<Klient>( new[] { "DataDodatnia" }, szukanie);
        //    var wynik = dane.Where((Func<Klient, bool>)warunek).ToList();
        //    Assert.Equal(7, wynik.Count);
        //}
        //[Fact(DisplayName = "Test dynamiczego zapytania - szuaknie daty Do")]
        //public void StworzWhereTestDatyDd()
        //{
        //     var dane = TworzDane(10);
        //    string[] szukanie = new[] { ";" + DateTime.Now.Date.AddDays(3) };
        //    var warunek = PobierzCaloscSolex().Szukanie.StworzWhere<Klient>( new[] { "DataDodatnia" }, szukanie);
        //    var wynik = dane.Where((Func<Klient, bool>)warunek).ToList();
        //    Assert.Equal(6, wynik.Count);
        //}
        //[Fact(DisplayName = "Test dynamiczego zapytania - szuaknie daty pomiędzy, oraz wg pola")]
        //public void StworzWhereTLaczie()
        //{
        //  var dane = TworzDane(10);
        //    string[] szukanie = { DateTime.Now.Date.AddDays(-3) + ";" + DateTime.Now.Date.AddDays(3), "klient1" };
        //    var warunek = PobierzCaloscSolex().Szukanie.StworzWhere<Klient>( new[] { "nazwa", "DataDodatnia" }, szukanie);
        //    var wynik = dane.Where((Func<Klient, bool>)warunek).ToList();
        //    Assert.Equal(1, wynik.Count);
        //}
        //[Fact(DisplayName = "Test dynamiczego zapytania - szuaknie daty kontrketnej")]
        //public void StworzWhereTestDatyKontrketna()
        //{
        //   var dane = TworzDane(10);
        //    dane[0].DataDodatnia = DateTime.Now.Date.AddHours(1);
        //    string []szukanie =new []{ DateTime.Now.Date.AddHours(1).ToString()};
        //    var warunek = PobierzCaloscSolex().Szukanie.StworzWhere<Klient>( new[] { "DataDodatnia" }, szukanie);
        //    var wynik = dane.Where((Func<Klient, bool>)warunek).ToList();
        //    Assert.Equal(1, wynik.Count);
        //}
        //[Fact(DisplayName = "Test dynamiczego zapytania -konkretna fraza w kontetnym polu")]
        //public void StworzWhereTestDatyKontrketnaFrazaKonkretnePole()
        //{
        //    var dane = TworzDane(10);

        //    string[] szukanie =  {"klient","8" };
        //    var warunek = PobierzCaloscSolex().Szukanie.StworzWhere<Klient>( new[] { "nazwa","Id" }, szukanie,true);
        //    var wynik = dane.Where((Func<Klient, bool>)warunek).ToList();
        //    Assert.Equal(1, wynik.Count);
        //}
        //[Fact(DisplayName = "Test dynamiczego zapytania -konkretna fraza w kontetnym polu,zła liczba parametrów")]
        //public void StworzWhereFrazaKonkretnePoleWyjatek()
        //{
        //    string[] szukanie = { "klient", "8" };
        //   Assert.Throws<InvalidOperationException>(()=> PobierzCaloscSolex().Szukanie.StworzWhere< IKlient>(new[] { "nazwa",  }, szukanie, true));

        //}
        //[Fact(DisplayName = "Test dynamiczego zapytania - pusta liczba parametrów")]
        //public void StworzWherePusteListaParametrow()
        //{
        //    var dane = TworzDane(10);

        //    string[] szukanie = {  };
        //    var warunek = PobierzCaloscSolex().Szukanie.StworzWhere<IKlient>(new[] { "nazwa", "Id" }, szukanie);
        //    var wynik = dane.Where((Func<IKlient, bool>)warunek).ToList();
        //    Assert.Equal(10, wynik.Count);

        //}
        //[Fact(DisplayName = "Test dynamiczego zapytania -szukanie pustego stringa")]
        //public void StworzWhereSzukaniePustegoStringa()
        //{
        //    var dane = TworzDane(10);

        //    string[] szukanie = {"" };
        //    var warunek = PobierzCaloscSolex().Szukanie.StworzWhere<IKlient>( new[] { "nazwa", "Id" }, szukanie);
        //    var wynik = dane.Where((Func<IKlient, bool>)warunek).ToList();
        //    Assert.Equal(10, wynik.Count);

        //}
        //[Fact(DisplayName = "Test dynamiczego zapytania -konkretna fraza w kontetnym polu wersja 2")]
        //public void StworzWhereTestDatyKontrketnaFrazaKonkretnePole2()
        //{          
        //    var dane = TworzDane(10);

        //    string[] szukanie = { "klient", "8" };
        //    var warunek = PobierzCaloscSolex().Szukanie.StworzWhere<Klient>(new[] { "Id", "nazwa" }, szukanie, true);
        //    var wynik = dane.Where((Func<Klient, bool>)warunek).ToList();
        //    Assert.Equal(0, wynik.Count);
        //}

        [Fact(DisplayName = "Test dynamiczego zapytania - sortowanie")]
        public void WygenerujSortowanieTest()
        {
            List<Klient> dane = TworzDane(10);

            var warunek = PobierzCaloscSolex().Szukanie.WygenerujSortowanie(typeof(Klient), "Nazwa");
            var wynik = dane.OrderByDescending((Func<Klient, object>)warunek).ToList();
            Assert.Equal("klient 9", wynik[0].Nazwa);
        }

        [Fact(DisplayName = "Test dynamiczego zapytania - sortowanie generyczne")]
        public void WygenerujSortowanieTestGeneric()
        {
            List<Klient> dane = TworzDane(10);

            var warunek = PobierzCaloscSolex().Szukanie.WygenerujSortowanie<Klient>("Nazwa");
            var wynik = dane.OrderByDescending(warunek).ToList();
            Assert.Equal("klient 9", wynik[0].Nazwa);
        }
        [Fact(DisplayName = "Test dynamiczego zapytania - sortowanie generyczne, wg liczby")]
        public void WygenerujSortowanieTestGenericLiczba()
        {
            var dane = TworzDane(10);

            var warunek = PobierzCaloscSolex().Szukanie.WygenerujSortowanie<Klient>("Id");
            var wynik = dane.OrderByDescending(warunek).ToList();
            Assert.Equal(10, wynik[0].Id);
        }

        [Fact(DisplayName = "Test sprawdzający poprawne budowanie warunku dlapropertisa będącego liczbą mogąca być nullem")]
        public void StworzWhereEpressionTestCalkowitaNull()
        {
            string[] szukanePola = new[] { "KlientId" };
            string[] wyszukiwanie = new[] { "123" };

            Type typ = typeof(ProfilKlienta);

            //Tworzymy parametr expression
            ParameterExpression pe = Expression.Parameter(typeof(ProfilKlienta), "m");
            //Tworzymy property info z nazwy KlientId
            PropertyInfo pi = typ.GetProperty("KlientId");
            //Tworzymy fileAccess dla propertisa klientId
            MemberExpression fieldAccess = Expression.Property(pe, pi);

            //Expression gdzie sprawdzwamy czy fileAccess(stworzony z typu propertisa) jest różny od nulla
            Expression checkNull = Expression.NotEqual(fieldAccess, Expression.Constant(null));

            //Musimy wyciagnąc podstawowy typ (trzeba zrobić bo jest nullowy propertis)
            Expression typPodstawowy = Expression.Convert(fieldAccess, typeof(Int64));

            //Expressione gdzie wartosc propertisa mabyć równa wartości szukanej
            BinaryExpression rownySzukanej = Expression.Equal(typPodstawowy, Expression.Constant(long.Parse("123")));

            //budujemy Expression który ma mieć sprawdzenie czy propertis jest != null i wartość równa wartości szukanej
            Expression ex = Expression.AndAlso(checkNull, rownySzukanej);

            var wynik = PobierzCaloscSolex().Szukanie.StworzWhereEpression<ProfilKlienta>(szukanePola, wyszukiwanie, true, null);
            Assert.Equal(wynik.Body.ToString(), ex.ToString());

        }

        [Fact()]
        public void UsunZnakiZakazaneZSzukaniaTest()
        {

            var result = PobierzCaloscSolex().Szukanie.UsunZnakiZakazaneZSzukania("bettt'r");

            //Assert
            Assert.Equal("betttr", result);

        }
    }
}
