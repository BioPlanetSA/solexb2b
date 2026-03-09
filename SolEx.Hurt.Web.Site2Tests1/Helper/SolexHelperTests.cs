using Xunit;
using SolEx.Hurt.Web.Site2.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Web.Site2.Helper.Tests
{
    public class SolexHelperTests
    {
        private Jezyk jezykTestowyEn = A.Fake<Jezyk>();
        private Jezyk jezykTestowyPl = A.Fake<Jezyk>();

        private ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
        private IDaneDostep dane = A.Fake<IDaneDostep>();

        IConfigBLL config = A.Fake<IConfigBLL>();

        private Dictionary<string, Jezyk> slownikJezykowSymbolach = null;
        private Dictionary<int, Jezyk> slownikJezykow = null;

        public SolexHelperTests()
        {
            jezykTestowyEn.Id = 45;
            jezykTestowyEn.Symbol = "en";

            jezykTestowyPl.Id = 34;
            jezykTestowyPl.Symbol = "pl";
        }

        protected void konfiguracjaDlaWieluJezykow()
        {
            slownikJezykowSymbolach = new Dictionary<string, Jezyk> {{jezykTestowyEn.Symbol, jezykTestowyEn}, {jezykTestowyPl.Symbol, jezykTestowyPl}};
            slownikJezykow = new Dictionary<int, Jezyk> {{jezykTestowyEn.Id, jezykTestowyEn}, {jezykTestowyPl.Id, jezykTestowyPl}};
            A.CallTo(() => config.WieleJezykowWSystemie).Returns(true);
            A.CallTo(() => config.JezykIDDomyslny).Returns(jezykTestowyEn.Id);
            A.CallTo(() => config.JezykiWSystemie).Returns(slownikJezykow);
            A.CallTo(() => config.AdresStronyZProduktem).Returns("p");

            A.CallTo(() => calosc.Konfiguracja).Returns(config);
            A.CallTo(() => calosc.DostepDane).Returns(dane);

            A.CallTo(() => calosc.Konfiguracja.JezykiWSystemieSlownikPoSymbolu).Returns(slownikJezykowSymbolach);

            UrlExtender.Config = config;
            UrlExtender.Calosc = calosc;
        }

        protected void konfiguracjaDlaJednegoJezyka()
        {
            slownikJezykowSymbolach = new Dictionary<string, Jezyk> {{jezykTestowyEn.Symbol, jezykTestowyEn}};
            slownikJezykow = new Dictionary<int, Jezyk> {{jezykTestowyEn.Id, jezykTestowyEn}};
            A.CallTo(() => config.WieleJezykowWSystemie).Returns(false);
            A.CallTo(() => config.JezykIDDomyslny).Returns(jezykTestowyEn.Id);
            A.CallTo(() => config.JezykiWSystemie).Returns(slownikJezykow);
            A.CallTo(() => config.AdresStronyZProduktem).Returns("p");

            A.CallTo(() => calosc.Konfiguracja).Returns(config);
            A.CallTo(() => calosc.DostepDane).Returns(dane);
            A.CallTo(() => calosc.Konfiguracja.JezykiWSystemieSlownikPoSymbolu).Returns(slownikJezykowSymbolach);
        }


        [Fact(DisplayName = "Ładowanie właściwego języka - ajaxy - wile jezykow - klient ZALOGOWANY")]
        public void UstawAktualnyJezykTest()
        {
            konfiguracjaDlaWieluJezykow();

            IKlient klient = A.Fake<IKlient>();
            A.CallTo(() => klient.Id).Returns(56);
            A.CallTo(() => klient.JezykId).Returns(jezykTestowyEn.Id);

            IDaneDostep dane = A.Fake<IDaneDostep>();
            A.CallTo(() => calosc.DostepDane).Returns(dane);

            //AJAXowe  zapytanie - czyli zwracamy zawsze w jezuki KLIENTA - nigdy nie robmy zapisu
            SolexHelper sesja = A.Fake<SolexHelper>();

            sesja.Calosc = calosc;
            A.CallTo(() => sesja.AktualnyKlient).Returns(klient);
            Assert.True(sesja.AktualnyJezyk == null);

            sesja.UstawAktualnyJezyk(null, true);
            Assert.True(sesja.AktualnyJezyk.Id == klient.JezykId);

            A.CallTo(() => calosc.DostepDane.AktualizujPojedynczy(A<Core.Klient>.Ignored)).MustNotHaveHappened();

            //AJAXowe zapytanie - zle jezyk wyslany - musi byc podmiana na klienta jezyk
            sesja = A.Fake<SolexHelper>();
            sesja.Calosc = calosc;
            A.CallTo(() => sesja.AktualnyKlient).Returns(klient);
            Assert.True(sesja.AktualnyJezyk == null);
            Assert.True(klient.JezykId != jezykTestowyPl.Id, "inny język wysylamy do sprawdzenia, niż go ma klient");

            sesja.UstawAktualnyJezyk(jezykTestowyPl, true);
            Assert.True(sesja.AktualnyJezyk.Id == klient.JezykId);

            A.CallTo(() => calosc.DostepDane.AktualizujPojedynczy(A<Core.Klient>.Ignored)).MustNotHaveHappened();
        }

        [Fact(DisplayName = "Ładowanie właściwego języka - klient zalogowany")]
        public void UstawAktualnyJezykTest_wielejezyk_nieajax()
        {
            konfiguracjaDlaWieluJezykow();

            IKlient klient = A.Fake<IKlient>();
            A.CallTo(() => klient.Id).Returns(56);
            A.CallTo(() => klient.JezykId).Returns(jezykTestowyEn.Id);

            IDaneDostep dane = A.Fake<IDaneDostep>();
            A.CallTo(() => calosc.DostepDane).Returns(dane);

            //nieAJAXOWE zapytanie - podmiana jezyka
            SolexHelper sesja = A.Fake<SolexHelper>();
            sesja.Calosc = calosc;
            A.CallTo(() => sesja.AktualnyKlient).Returns(klient);
            Assert.True(sesja.AktualnyJezyk == null);
            Assert.True(sesja.AktualnyKlient.JezykId != jezykTestowyPl.Id, "inny język wysylamy do sprawdzenia, niż go ma klient");

            sesja.UstawAktualnyJezyk(jezykTestowyPl, false);
            Assert.True(sesja.AktualnyJezyk.Id == klient.JezykId);

            A.CallTo(() => calosc.DostepDane.AktualizujPojedynczy(A<Core.Klient>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact(DisplayName = "Ładowanie właściwego języka - klient zalogowany - nie podany jezyk")]
        public void UstawAktualnyJezykTest_wielejezyk_nieajax_zalogowany_niepodanyjezyk()
        {
            konfiguracjaDlaWieluJezykow();

            IKlient klient = A.Fake<IKlient>();
            A.CallTo(() => klient.Id).Returns(56);
            A.CallTo(() => klient.JezykId).Returns(jezykTestowyPl.Id);

            IDaneDostep dane = A.Fake<IDaneDostep>();
            A.CallTo(() => calosc.DostepDane).Returns(dane);

            //nieAJAXOWE zapytanie - podmiana jezyka
            SolexHelper sesja = A.Fake<SolexHelper>();
            sesja.Calosc = calosc;
            A.CallTo(() => sesja.AktualnyKlient).Returns(klient);
            Assert.True(sesja.AktualnyJezyk == null);
            Assert.True(sesja.AktualnyKlient.JezykId != calosc.Konfiguracja.JezykIDDomyslny, "klient jezyk musi byc inny niz domyslny jezyk");

            sesja.UstawAktualnyJezyk(null, false);
            Assert.True(sesja.AktualnyJezyk.Id == klient.JezykId);

            A.CallTo(() => calosc.DostepDane.AktualizujPojedynczy(A<Core.Klient>.Ignored)).MustNotHaveHappened();
        }


        [Fact(DisplayName = "Ładowanie właściwego języka -wersja wielo jezykowa - klient zalogowany")]
        public void UstawAktualnyJezykTest_jedenJezyk_nieajax()
        {
            konfiguracjaDlaWieluJezykow();

            IKlient klient = A.Fake<IKlient>();
            A.CallTo(() => klient.Id).Returns(56);
            A.CallTo(() => klient.JezykId).Returns(jezykTestowyPl.Id);

            IDaneDostep dane = A.Fake<IDaneDostep>();
            A.CallTo(() => calosc.DostepDane).Returns(dane);

            //nieAJAXOWE zapytanie - podmiana jezyka
            SolexHelper sesja = A.Fake<SolexHelper>();
            sesja.Calosc = calosc;
            A.CallTo(() => sesja.AktualnyKlient).Returns(klient);
            Assert.True(sesja.AktualnyJezyk == null);
            Assert.True(sesja.AktualnyKlient.JezykId != jezykTestowyEn.Id, "inny język wysylamy do sprawdzenia, niż go ma klient");

            sesja.UstawAktualnyJezyk(jezykTestowyEn, false);
            Assert.True(sesja.AktualnyJezyk.Id == klient.JezykId);

            A.CallTo(() => calosc.DostepDane.AktualizujPojedynczy(A<Core.Klient>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact(DisplayName = "Ładowanie właściwego języka - klient niezalogowany, bez cookisa")]
        public void UstawAktualnyJezykTest_klientniezalogowany()
        {
            konfiguracjaDlaWieluJezykow();

            IKlient klient = A.Fake<IKlient>();
            A.CallTo(() => klient.Id).Returns(0);   //niezalogowany
            //A.CallTo(() => klient.JezykId).Returns(jezykTestowyPl.Id); //klient ma PL

            IDaneDostep dane = A.Fake<IDaneDostep>();
            A.CallTo(() => calosc.DostepDane).Returns(dane);

            //nieAJAXOWE zapytanie - podmiana jezyka
            SolexHelper sesja = A.Fake<SolexHelper>();
            sesja.Calosc = calosc;
            A.CallTo(() => sesja.AktualnyKlient).Returns(klient);
            Assert.True(sesja.AktualnyJezyk == null);

            sesja.UstawAktualnyJezyk(null, false);
            Assert.True(sesja.AktualnyJezyk.Id == calosc.Konfiguracja.JezykIDDomyslny, "dla klienta niezalogowanego bierzemy zawsze jezyk domyslna systemu jesli nie podany jezyk, jesli podany jezyk to ten jezyk");


            sesja = A.Fake<SolexHelper>();
            sesja.Calosc = calosc;
            A.CallTo(() => sesja.AktualnyKlient).Returns(klient);
            Assert.True(sesja.AktualnyJezyk == null);
            Assert.True(jezykTestowyPl.Id != calosc.Konfiguracja.JezykIDDomyslny);

            sesja.UstawAktualnyJezyk(jezykTestowyPl, false);
            Assert.True(sesja.AktualnyJezyk.Id == jezykTestowyPl.Id, "dla klienta niezalogowanego bierzemy zawsze jezyk domyslna systemu jesli nie podany jezyk, jesli podany jezyk to ten jezyk");

            A.CallTo(() => calosc.DostepDane.AktualizujPojedynczy(A<Core.Klient>.Ignored)).MustNotHaveHappened();
        }

        [Fact(DisplayName = "Ładowanie właściwego języka - klient niezalogowany z cookisem")]
        public void UstawAktualnyJezykTest_klientniezalogowany_cookis()
        {
            konfiguracjaDlaWieluJezykow();

            IKlient klient = A.Fake<IKlient>();
            A.CallTo(() => klient.Id).Returns(0);   //niezalogowany
            //A.CallTo(() => klient.JezykId).Returns(jezykTestowyPl.Id); //klient ma PL

            IDaneDostep dane = A.Fake<IDaneDostep>();
            A.CallTo(() => calosc.DostepDane).Returns(dane);

            //nieAJAXOWE zapytanie - podmiana jezyka
            SolexHelper sesja = A.Fake<SolexHelper>();
            sesja.Calosc = calosc;
            sesja.CookieHelper = A.Fake<ICookieHelper>();
            A.CallTo(() => sesja.AktualnyKlient).Returns(klient);
            Assert.True(sesja.AktualnyJezyk == null);

            //jezyk w cookisie = PL (czyli inny niz domyslny konfiga)
            A.CallTo(() => sesja.CookieHelper.GetCookieValue(null)).WithAnyArguments().Returns(jezykTestowyPl.Id.ToString());
            Assert.True(sesja.CookieHelper.GetCookieValue(null) != calosc.Konfiguracja.JezykIDDomyslny.ToString(), "jezyk domyslny ma byc inny niz jezyk cookisa zeby test mial sens");

            sesja.UstawAktualnyJezyk(null, false);
            Assert.True(sesja.AktualnyJezyk.Id.ToString() == sesja.CookieHelper.GetCookieValue(null), "klient niezalogowany powinno isc z cookisa jezyk bo cookis ma jezyk ustawiony!");

            //nie powinno byc zapisu zadnego
            A.CallTo(() => sesja.CookieHelper.SetCookie(A<string>.Ignored, jezykTestowyEn.Id)).WithAnyArguments().MustNotHaveHappened();

            //drug test - podanie jezyka do metody INNEGO niz klient ma w cookisie - powinno nadpisac cookisa
            Assert.True(sesja.AktualnyJezyk != jezykTestowyEn);

            sesja.UstawAktualnyJezyk(jezykTestowyEn, false);
            Assert.True(sesja.AktualnyJezyk.Id == jezykTestowyEn.Id, "dla klienta niezalogowanego bierzemy zawsze jezyk domyslna systemu jesli nie podany jezyk, jesli podany jezyk to ten jezyk");

            //zmiana cookisa na wlasciwy jezyk nastapiloa

            A.CallTo(() => sesja.CookieHelper.SetCookie(A<string>.Ignored, jezykTestowyEn.Id)).MustHaveHappened(Repeated.Exactly.Once);


            A.CallTo(() => calosc.DostepDane.AktualizujPojedynczy(A<Core.Klient>.Ignored)).MustNotHaveHappened();
        }
    }
}