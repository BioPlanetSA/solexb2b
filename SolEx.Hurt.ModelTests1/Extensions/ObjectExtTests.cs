using System.Collections.Generic;
using System.Diagnostics;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Core;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using Xunit;
using System;
using System.Linq;
using FakeItEasy;
using ServiceStack.Logging;

namespace System.Tests
{
    public class ObjectExtTests
    {
        [Fact]
        public void TestPobieraniaPorpertisowIUsuwaniaZbednychPol()
        {
            Cecha cecha = new Cecha();

            Assert.True(cecha.GetType().Properties().ContainsKey("JezykId"));
            Assert.False(cecha.GetType().Properties(null, true).ContainsKey("JezykId"));
        }


        [Fact]
        public void KopiujPolaTest()
        {
            Klient klientZrodlo = new Klient() {Nazwa = "testowa nazwa do kopiowania", Symbol = "testowy symbol", Opis = "zły"};
            Klient klientCel = new Klient() {Nazwa = "zła", Symbol = "zły", Opis = "dobry"};

            Assert.True(klientCel.Nazwa != klientZrodlo.Nazwa && klientCel.Symbol != klientZrodlo.Symbol && klientCel.Opis != klientZrodlo.Opis, "Muszą być inne wartości do testów");

            klientCel.KopiujPola( klientZrodlo, new {klientCel.Opis} );

            Assert.True(klientCel.Nazwa == klientZrodlo.Nazwa);
            Assert.True(klientCel.Symbol == klientZrodlo.Symbol);
            Assert.True(klientCel.Opis != klientZrodlo.Opis);
        }
        [Fact]
        public void KopiujPolaIstniejaceObiektyTest()
        {
            Produkt produktCel = new Produkt(1) {Nazwa = "Nazwa Cel", Abstrakcyjny = true, Kod = "Sylbol cel", WyslanoMailNowyProdukt = false, };
            Produkt produktZrodlo = new Produkt(1) { Nazwa = "Nazwa zrodlo", Abstrakcyjny = false, Kod = "Sylbol zrodlo", WyslanoMailNowyProdukt = true};

            Produkt prod = new Produkt();

            List<Produkt> listaZrodlowa = new List<Produkt>() {produktZrodlo};
            List<Produkt> listaCel = new List<Produkt>() { produktCel};
            var slownikProduktow = listaCel.ToDictionary(x => x.Id, x => x);
            
            slownikProduktow.KopiujPolaIstniejaceObiekty( listaZrodlowa.ToDictionary(x=>x.Id, x=>x), new { prod.Nazwa, prod.CenaWPunktach });

            Assert.True(listaCel[0].Nazwa.Equals("Nazwa zrodlo"), $"Nazwa powinna być: Nazwa zrodlo a jest: {listaCel[0].Nazwa}.");
            Assert.False(listaCel[0].Kod.Equals(listaZrodlowa[0].Kod, StringComparison.InvariantCultureIgnoreCase), $"Kod powinna być: {produktCel.Kod} a jest: {listaCel[0].Kod}.");
            Assert.True(listaCel[0].Abstrakcyjny, $"Produkt powinnien być abstrakcyjny a nie jest.");


            produktCel = new Produkt(1) { Nazwa = "Nazwa Cel", Abstrakcyjny = true, Kod = "Sylbol cel", WyslanoMailNowyProdukt = false, };
            listaCel = new List<Produkt>() { produktCel };
            slownikProduktow = listaCel.ToDictionary(x => x.Id, x => x);

            slownikProduktow.KopiujPolaIstniejaceObiekty(listaZrodlowa.ToDictionary(x => x.Id, x => x),null, new { prod.Nazwa });

            Assert.True(listaCel[0].Nazwa.Equals("Nazwa Cel"), $"Nazwa powinna być: Nazwa Cel a jest: {listaCel[0].Nazwa}.");
            Assert.True(listaCel[0].Kod.Equals(listaZrodlowa[0].Kod, StringComparison.InvariantCultureIgnoreCase), $"Kod powinna być: {produktZrodlo.Kod} a jest: {listaCel[0].Kod}.");
            Assert.False(listaCel[0].Abstrakcyjny, $"Produkt nie powinnien być abstrakcyjny a jest.");
        }

        [Fact]
        public void RownyWartosciDomyslnejTest()
        {
            Assert.Equal(false,3.RownyWartosciDomyslnej());
            Assert.Equal(true, 0.RownyWartosciDomyslnej());
            int? test = null;
            Assert.Equal(true, test.RownyWartosciDomyslnej());
            test = 3;
            Assert.Equal(false, test.RownyWartosciDomyslnej());
            Produkt p=new Produkt();
            Assert.Equal(false, p.RownyWartosciDomyslnej());

            Assert.Equal(false, (3.0).RownyWartosciDomyslnej());
            Assert.Equal(true, (0.0).RownyWartosciDomyslnej());

            Assert.Equal(false,"aaa".RownyWartosciDomyslnej());
            Assert.Equal(true, "".RownyWartosciDomyslnej());

            Assert.Equal(true, ((object)null).RownyWartosciDomyslnej());
        }

       


        [Fact(DisplayName = "test uniwersalnej metody do porównywania")]
        public void PorownajTest()
        {
            decimal d1 = 1, d2 = 2;

            Assert.True(d1.PorownajWartosc(d2, Wartosc.Dowolna));
            Assert.True(d1.PorownajWartosc(d2, Wartosc.Mniejsze));
            Assert.True(d2.PorownajWartosc(d1, Wartosc.Wieksze));
            Assert.True(d1.PorownajWartosc(d1, Wartosc.Rowne));
            Assert.True(d1.PorownajWartosc(d2, Wartosc.Rozne));


            string s1 = "1", s2 = "2";

            Assert.True(s1.PorownajWartosc(s2, Wartosc.Dowolna));
            Assert.True(s1.PorownajWartosc(s2, Wartosc.Mniejsze));
            Assert.True(s2.PorownajWartosc(s1, Wartosc.Wieksze));
            Assert.True(s1.PorownajWartosc(s1, Wartosc.Rowne));
            Assert.True(s1.PorownajWartosc(s2, Wartosc.Rozne));


            WartoscLiczbowa wl1 = new WartoscLiczbowa(1), wl2 = new WartoscLiczbowa(2);

            Assert.True(wl1.PorownajWartosc(wl2, Wartosc.Dowolna));
            Assert.True(wl1.PorownajWartosc(wl2, Wartosc.Mniejsze));
            Assert.True(wl2.PorownajWartosc(wl1, Wartosc.Wieksze));
            Assert.True(wl1.PorownajWartosc(wl1, Wartosc.Rowne));
            Assert.True(wl1.PorownajWartosc(wl2, Wartosc.Rozne));



            int i1 = 1, i2 = 2;

            Assert.True(i1.PorownajWartosc(i2, Wartosc.Dowolna));
            Assert.True(i1.PorownajWartosc(i2, Wartosc.Mniejsze));
            Assert.True(d2.PorownajWartosc(i1, Wartosc.Wieksze));
            Assert.True(i1.PorownajWartosc(i1, Wartosc.Rowne));
            Assert.True(i1.PorownajWartosc(i2, Wartosc.Rozne));
        }

        //[Fact()]
        //public void PobierzPolaSlownikTest()
        //{
        //    Klient k = new Klient(){Nazwa = "test1", Login = "Login1"};
        //    List<string> pola = new List<string>() { "Nazwa", "Login" };
        //    var a = k.PobierzPolaSlownik(pola);
        //    Assert.True(a.Count==2);
        //    Assert.True(a["Nazwa"]=="test1");
        //    Assert.True(a["Login do systemu"] == "Login1");
        //}

        [Fact()]
        public void UsunDubleTest()
        {
            List<Produkt> produkty = new List<Produkt>();
            produkty.Add(new Produkt() { Id = 1, KodKreskowy = "123456" });
            produkty.Add(new Produkt() { Id = 2, KodKreskowy = "123456" });
            produkty.Add(new Produkt() { Id = 3, KodKreskowy = "234567" });
            produkty.Add(new Produkt() { Id = 4, KodKreskowy = "345678" });
            produkty.Add(new Produkt() { Id = 5, KodKreskowy = "456789" });
            produkty.Add(new Produkt() { Id = 6, KodKreskowy = "567890" });
            produkty.Add(new Produkt() { Id = 7, KodKreskowy = "678901" });

            List<Produkt> bezDubli = new List<Produkt>(produkty);

            Produkt pTemp = new Produkt();

            //usuwanie dubli kodów kreskowych
            bezDubli.UsunDuble(new { kod_kreskowy = pTemp.KodKreskowy });
            Assert.True(produkty.Count>bezDubli.Count);
        }

        [Fact(DisplayName = "Testy logowania porównań")]
        public void PokazRozniceWStringachTest()
        {
            ObjectExt.log = A.Fake<log4net.ILog>();

            A.CallTo(() => ObjectExt.log.IsDebugEnabled).Returns(true);

            ObjectExt.PokazRozniceWStringach("", "sdfsdfwefwe");
            ObjectExt.PokazRozniceWStringach("sdgfsdfsdf", "");
            ObjectExt.PokazRozniceWStringach("", "");
            ObjectExt.PokazRozniceWStringach("df ert ertert ert5tt 45t 45t45t 45t 45t45t4 5t45t 45t 45t45t 6575676575rygr hrth 4rh 4rh hr", "df ert ertert ert5tt 45t 45t45t 45t 45t45t4 5t45t 45t 45t45t 45t45y 4y4rygr hrth 4rh 4rh hr");
            ObjectExt.PokazRozniceWStringach("df ert ertert ert5tt 45t 45t45t 45t 45t45t4 5t45t 45t 45t45t 6575676575rygr hrth 4rh 4rh hrhrth 4rh 4rh hrhrth 4rh 4rh hrhrth 4rh 4rh hr", "df ert ertert ert5tt 45t 45t45t 45t 45t45t4 5t45t 45t 45t45t 45t45y 4y4rygr hrth 4rh 4rh hr 4rh 4rh hrhrth 4rh 4rh hrhrth 4rh 4rh hr");
            ObjectExt.PokazRozniceWStringach("Brak katalogu o ścieżce: \\\\files\\PROJEKTY_KLUBOWE\\26ZDH\\BANER/projekty-graficzne ]     SolEx.Hurt.Core.BLL.BllBaza`1    SolEx.Hurt.Core.BLL.WirtualneProduktyProvidery.Smmash.SynchronizatorPrzetwarzajWirtualneProdukty \r\n2017-04-03 19:37:53,692    DEBUG    [ Brak katalogu o ścieżce: \\\\files\\PROJEKTY_KLUBOWE\\26ZDH\\RSO7/projekty-graficzne ]     SolEx.Hurt.Core.BLL.BllBaza`1    SolEx.Hurt.Core.BLL.WirtualneProduktyProvidery.Smmash.SynchronizatorPrzetwarzajWirtualneProdukty \r\n2017-04-03 19:37:53,708    DEBUG    [ Brak plików w katalogu: \\\\files\\PROJEKTY_KLUBOWE\\26ZDH\\SHC3/projekty-graficzne ]     SolEx.Hurt.Core.BLL.BllBaza`1    SolEx.Hurt.Core.BLL.WirtualneProduktyProvidery.Smmash.SynchronizatorPrzetwarzajWirtualneProdukty \r\n2017-04-03 19:37:53,724    DEBUG    [ Brak", "Brak katalogu o ścieżce: \\\\files\\PROJEKTY_KLUBOWE\\26ZDH\\BANER/projekty-graficzne ]     SolEx.Hurt.Core.BLL.BllBaza`1    SolEx.Hurt.Core.BLL.WirtualneProduktyProvidery.Smmash.SynchronizatorPrzetwarzajWirtualneProdukty \r\n2017-04-03 19:37:53,692    DEBUG    [ Brak katalogu o ścieżce: \\\\files\\PROJEKTY_KLUBOWE\\26ZDH\\RSO7/projekty-graficzne ]     SolEx.Hurt.Core.BLL.BllBaza`1    SolEx.Hurt.Core.BLL.WirtualneProduktyProvidery.Smmash.SynchronizatorPrzetwarzajWirtualneProdukty \r\n2017-04-03 19:37:53,708    DEBUG    [ Brak plików w katalogu: \\\\files\\PROJEKTY_KLUBOWE\\26ZDH\\SHC3/projekty-graficzne ]     SolEx.Hurt.Core.BLL.BllBaza`1    SolEx.Hurt.Core.BLL.WirtualneProduktyProvidery.Smmash.SynchronizatorPrzetwarzajWirtualneProdukty \r\n2017-04-03 19:37:53,724    DEBUG    [ Brak");
            ObjectExt.PokazRozniceWStringach("Brak katalogu o ścieżce: \\\\files\\PROJEKTY_KLUBOWE\\26ZDH\\BANER/projekty-graficzne ]     SolEx.Hurt.Core.BLL.BllBaza`1    SolEx.Hurt.Core.BLL.WirtualneProduktyProvidery.Smmash.SynchronizatorPrzetwarzajWirtualneProdukty \r\n2017-04-03 19:37:53,692    DEBUG    [ Brak katalogu o ścieżce: \\\\files\\PROJEKTY_KLUBOWE\\26ZDH\\RSO7/projekty-graficzne ]     SolEx.Hurt.Core.BLL.BllBaza`1    SolEx.Hurt.Core.BLL.WirtualneProduktyProvidery.Smmash.SynchronizatorPrzetwarzajWirtualneProdukty \r\n2017-04-03 19:37:53,708    DEBUG    [ Brak plików w katalogu: \\\\files\\PROJEKTY_KLUBOWE\\26ZDH\\SHC3/projekty-graficzne ]     SolEx.Hurt.Core.BLL.BllBaza`1    SolEx.Hurt.Core.BLL.WirtualneProduktyProvidery.Smmash.SynchronizatorPrzetwarzajWirtualneProdukty \r\n2017-04-03 19:37:53,724    DEBUG    [ Brak", "Brak katalogu o ścieżce: \\\\files\\PROJEKTY_KLUBOWE\\26ZDH\\BANER/projekty-graficzne ]     SolEx.Hurt.Core.BLL.BllBaza`1    SolEx.Hurt.Core.BLL.WirtualneProduktyProvidery.Smmash.SynchronizatorPrzetwarzajWirtualneProdukty \r\n2017-04-03 19:37:53,692    DEBUG    [ Brak katalogu o ścieżce: \\\\files\\PROJEKTY_KLUBOWE\\26ZDH\\RSO7/projekty-graficzne ]     SolEx.Hurt.Core.BLL.BllBaza`1    SolEx.Hurt.Core.BLL.WirtualneProduktyProvidery.Smmash.SynchronizatorPrzetwarzajWirtualneProdukty \r\n2017-04-03 19:37:53,708    DEBUG    [ Brak plików w katalogu: \\\\files\\PROJEKTY_KLUBOWE\\26ZDH\\SHC3/projekty-graficzne ]     SolEx.Hurt.Core.BLL.BllBaza`1    SolEx.Hurt.Core.BLL.WfsdfsdfsfsdfsfirtualneProduktyProvidery.Smmash.SynchronizatorPrzetwarzajWirtualneProdukty \r\n2017-04-03 19:37:53,724    DEBUG    [ Brak");
            ObjectExt.PokazRozniceWStringach("test1test2test3aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "test2test2test3aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");

        }
    }
}
