using System;
using System.Collections.Generic;
using FakeItEasy;
using log4net;
using SolEx.Hurt.Model;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.Rozne.Tests
{
    public class DomyslnaWartoscPolaBaseTests
    {
        [Fact(DisplayName = "Test sprawdzający poprawność nadpisywania okreśonych pól przez wartość domyślną")]
        public void PrzetworzTest()
        {
            string wartoscDomyslna = "Wartosc domyslna";
            Klient k1 = new Klient(null) { Id = 1, Email = "jakkis1@solex.net.pl", Nazwa = "jaka1", Symbol = "jakisSymbol1" };
            Klient k2 = new Klient(null) { Id = 2, Email = "jakkis2@solex.net.pl", Nazwa = "jaka2", Symbol = "jakisSymbol1" };
            Klient k3 = new Klient(null) { Id = 3, Email = "jakkis3@solex.net.pl", Nazwa = "jaka3", Symbol = "jakisSymbol1" };

            List<Klient> listaKlientow = new List<Klient>() { k1, k2, k3 };

            string pole1 = "email";
            string pole2 = "nazwa";
            List<string> listaPol = new List<string>(){pole1,pole2};

            DomyslnaWartoscPolaBase dwpb = new DomyslnaWartoscPola();
            dwpb.NullZamiastPustegoStringa = false;
            dwpb.WartoscDomyslna = wartoscDomyslna;
            dwpb.Przetworz(listaKlientow, listaPol);

            Assert.True(listaKlientow[0].Email==wartoscDomyslna);
            Assert.True(listaKlientow[0].Nazwa == wartoscDomyslna);
            Assert.True(listaKlientow[1].Email == wartoscDomyslna);
            Assert.True(listaKlientow[1].Nazwa == wartoscDomyslna);
            Assert.True(listaKlientow[2].Email == wartoscDomyslna);
            Assert.True(listaKlientow[2].Nazwa == wartoscDomyslna);

        }

        [Fact(DisplayName = "Test sprawdzający poprawność nadpisywania okreśonych pól gdy nie ma wartosci domyslnej")]
        public void PrzetworzTest2()
        {
            string wartoscDomyslna = string.Empty;
            Klient k1 = new Klient(null) { Id = 1, Email = "jakkis1@solex.net.pl", Nazwa = "jaka1", Telefon = "jakisTelefon1" };
            Klient k2 = new Klient(null) { Id = 2, Email = "jakkis2@solex.net.pl", Nazwa = "jaka2", Telefon = "jakisTelefon2" };
            Klient k3 = new Klient(null) { Id = 3, Email = "jakkis3@solex.net.pl", Nazwa = "jaka3", Telefon = "jakisTelefon3" };

            List<Klient> listaKlientow = new List<Klient>() { k1, k2, k3 };

            string pole1 = "telefon";
            List<string> listaPol = new List<string>() { pole1};

            DomyslnaWartoscPolaBase dwpb = new DomyslnaWartoscPola();
            dwpb.NullZamiastPustegoStringa = true;
            dwpb.WartoscDomyslna = wartoscDomyslna;
            dwpb.Przetworz(listaKlientow, listaPol);

            Assert.True(listaKlientow[0].Telefon == wartoscDomyslna);
            

        }
        [Fact(DisplayName = "Test sprawdzajacy poprawnosc nadpisywania dla pola nullable bez wartosci domyslnej")]
        public void PrzetworzTest3()
        {
            string wartoscDomyslna = string.Empty;
            Klient k1 = new Klient(null) { Id = 1, Email = "jakkis1@solex.net.pl", Nazwa = "jaka1", Telefon = "jakisTelefon1" };
            Klient k2 = new Klient(null) { Id = 2, Email = "jakkis2@solex.net.pl", Nazwa = "jaka2"};
            Klient k3 = new Klient(null) { Id = 3, Email = "jakkis3@solex.net.pl", Nazwa = "jaka3", Telefon = "jakisTelefon3" };

            List<Klient> listaKlientow = new List<Klient>() { k1, k2, k3 };

            string pole1 = "konto_potwierdzajace_id";
            List<string> listaPol = new List<string>() { pole1 };

            DomyslnaWartoscPolaBase dwpb = new DomyslnaWartoscPola();
            dwpb.NullZamiastPustegoStringa = true;
            dwpb.WartoscDomyslna = wartoscDomyslna;
            dwpb.Przetworz(listaKlientow, listaPol);

           // Assert.True(listaKlientow[0].KontoPotwierdzajaceId == null);
        }
        [Fact(DisplayName = "Test wyłapujący wyjątek")]
        public void PrzetworzTest4()
        {
            string wartoscDomyslna = "jakas wartosc";
            Klient k1 = new Klient(null) { Id = 1, Email = "jakkis1@solex.net.pl", Nazwa = "jaka1", Telefon = "jakisTelefon1" };
            Klient k2 = new Klient(null) { Id = 2, Email = "jakkis2@solex.net.pl", Nazwa = "jaka2"};
            Klient k3 = new Klient(null) { Id = 3, Email = "jakkis3@solex.net.pl", Nazwa = "jaka3", Telefon = "jakisTelefon3" };

            List<Klient> listaKlientow = new List<Klient>() { k1, k2, k3 };

            string pole1 = "konto_potwierdzajace_id";
            List<string> listaPol = new List<string>() { pole1 };

            DomyslnaWartoscPolaBase dwpb = new DomyslnaWartoscPola();
            dwpb.NullZamiastPustegoStringa = true;
            dwpb.WartoscDomyslna = wartoscDomyslna;
            var log = A.Fake<ILog>();
            A.CallTo(() => log.Error(A<string>.Ignored)).Throws(new Exception("Błąd parsowania"));

            try
            {
                dwpb.Przetworz(listaKlientow, listaPol);
                Assert.True(false, "Nie powinien sie dodac, proba nadpisania inta stringiem");
            }
            catch 
            {
            }



        }

    }
}
