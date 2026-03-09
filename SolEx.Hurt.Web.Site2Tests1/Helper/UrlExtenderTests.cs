using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FakeItEasy;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Web.Site2;
using SolEx.Hurt.Web.Site2.Controllers;
using SolEx.Hurt.Web.Site2.Helper;
using SolEx.Hurt.Web.Site2.Models.DaneDoWidokow.Koszyk;
using SolEx.Hurt.Web.Site2.PageBases;
using Xunit;

namespace SolEx.Hurt.Web.Site2Tests.Helper
{
    public class UrlExtenderTests
    {
        private Jezyk jezykTestowyEn = A.Fake<Jezyk>();
        private Jezyk jezykTestowyPl = A.Fake<Jezyk>();

        IKlient klient = A.Fake<IKlient>();
        private ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
        private IDaneDostep dane = A.Fake<IDaneDostep>();

        IConfigBLL config = A.Fake<IConfigBLL>();

        private Controller testowyKontroler = null;

        private Dictionary<int, Jezyk> slownikJezykow = null;
        private Dictionary<string, Jezyk> slownikJezykowSymbolach = null;

        protected HttpContextBase GetContextFake(string url = "/")
        {
            HttpRequestBase request = A.Fake<HttpRequestBase>();
            var response = A.Fake<HttpResponseBase>();
            var httpWorkerRequest = A.Fake<HttpWorkerRequest>();

            HttpContextBase contextFake = A.Fake<HttpContextBase>();

            A.CallTo(() => request.ApplicationPath).Returns("/");
            A.CallTo(() => request.RawUrl).Returns(url);

            Uri uri = new Uri("http://localhost/" + url.TrimStart('/'), UriKind.Absolute);

            A.CallTo(() => request.Url).Returns(uri);
            A.CallTo(() => request.ServerVariables).Returns(new System.Collections.Specialized.NameValueCollection());
            A.CallTo(() => request.AppRelativeCurrentExecutionFilePath).Returns(url);
            A.CallTo(() => request.PathInfo).Returns(String.Empty);

            A.CallTo(() => response.ApplyAppPathModifier(A<string>.Ignored)).ReturnsLazily(x => "" + x.Arguments[0]);

            A.CallTo(() => contextFake.Request).Returns(request);
            A.CallTo(() => contextFake.Response).Returns(response);
            A.CallTo(() => contextFake.GetService(typeof(HttpWorkerRequest))).Returns(httpWorkerRequest);
            A.CallTo(() => contextFake.Session).Returns(null);
            A.CallTo(() => contextFake.Items).Returns(new Dictionary<string, string>());

            return contextFake;
        }

        protected T GetController<T>(string url ="/") where T : Controller, new()
        {
            HttpContextBase contextFake = GetContextFake(url);
            RouteTable.Routes.Clear();
            SolEx.Hurt.Web.Site2.RouteConfig.RegisterRoutes(RouteTable.Routes, calosc);

            var controller = new T();
            controller.ControllerContext = new ControllerContext(contextFake, new RouteData(), controller);
            controller.Url = new UrlHelper(new RequestContext(contextFake, new RouteData()), RouteTable.Routes);

            (controller as SolexControler).Calosc = calosc;

            return controller;
        }

        protected void konfiguracjaDlaWieluJezykow()
        {
            slownikJezykowSymbolach = new Dictionary<string, Jezyk> { { jezykTestowyEn.Symbol, jezykTestowyEn }, { jezykTestowyPl.Symbol, jezykTestowyPl } };
            slownikJezykow = new Dictionary<int, Jezyk> { { jezykTestowyEn.Id, jezykTestowyEn }, { jezykTestowyPl.Id, jezykTestowyPl } };
            A.CallTo(() => config.WieleJezykowWSystemie).Returns(true);
            A.CallTo(() => config.JezykIDDomyslny).Returns(jezykTestowyEn.Id);
            A.CallTo(() => config.JezykiWSystemie).Returns(slownikJezykow);
            A.CallTo(() => config.AdresStronyZProduktem).Returns("p");
            A.CallTo(() => config.LinkAlternatywnyStronyProduktow).Returns("superFajnyLink/46s");

            A.CallTo(() => calosc.Konfiguracja).Returns(config);
            A.CallTo(() => calosc.DostepDane).Returns(dane);

            A.CallTo(() => calosc.Konfiguracja.JezykiWSystemieSlownikPoSymbolu).Returns(slownikJezykowSymbolach);

            A.CallTo(() => calosc.AdresyDostep.PobierzAdresyKlienta(A<IKlient>.Ignored)).ReturnsLazily((IList<IAdres> a) => a);

            A.CallTo( () => calosc.Cache.SlownikPrywatny_PobierzObiekt<string>( A<Func<long,string>>.Ignored, A<int>.Ignored, A<long>.Ignored, A<string>.Ignored ) )
                .ReturnsLazily( call => call.GetArgument<Func<long,string>>(0).Invoke(0)  );

            UrlExtender.Config = config;
            UrlExtender.Calosc = calosc;

            testowyKontroler = this.GetController<TresciController>();
        }

        protected void konfiguracjaDlaJednegoJezyka()
        {
            slownikJezykowSymbolach = new Dictionary<string, Jezyk> { { jezykTestowyEn.Symbol, jezykTestowyEn } };
            slownikJezykow = new Dictionary<int, Jezyk> { { jezykTestowyEn.Id, jezykTestowyEn } };
            A.CallTo(() => config.WieleJezykowWSystemie).Returns(false);
            A.CallTo(() => config.JezykIDDomyslny).Returns(jezykTestowyEn.Id);
            A.CallTo(() => config.JezykiWSystemie).Returns(slownikJezykow);
            A.CallTo(() => config.AdresStronyZProduktem).Returns("p");
            A.CallTo(() => config.LinkAlternatywnyStronyProduktow).Returns("superFajnyLink/46s");

            A.CallTo(() => calosc.Konfiguracja).Returns(config);
            A.CallTo(() => calosc.DostepDane).Returns(dane);
            A.CallTo(() => calosc.Konfiguracja.JezykiWSystemieSlownikPoSymbolu).Returns(slownikJezykowSymbolach);

            A.CallTo(() => calosc.Cache.SlownikPrywatny_PobierzObiekt<string>(A<Func<long,string>>.Ignored, A<int>.Ignored, A<long>.Ignored, A<string>.Ignored))
               .ReturnsLazily(call => call.GetArgument<Func<string>>(0).Invoke());

            UrlExtender.Config = config;
            UrlExtender.Calosc = calosc;

            testowyKontroler = this.GetController<TresciController>();
        }

        public UrlExtenderTests()
        {
            jezykTestowyEn.Symbol = "en";
            jezykTestowyEn.Id = 5;

            jezykTestowyPl.Symbol = "pl";
            jezykTestowyPl.Id = 9;

            klient.Email = "test.fd@emfd.com";

            konfiguracjaDlaWieluJezykow();
        }

        [Fact(DisplayName = "Tworzenie linku do cechy")]
        public void ZbudujLinkTest_Cecha()
        {
            konfiguracjaDlaWieluJezykow();
            CechyBll cecha = A.Fake<CechyBll>();
            AtrybutBll atrybut = A.Fake<AtrybutBll>();
            atrybut.Symbol = "rozmiar";
            atrybut.PoleDoBudowyLinkow = atrybut.Symbol.Trim();
            atrybut.PoleDoBudowyLinkow = Tools.OczyscCiagDoLinkuURL(atrybut.PoleDoBudowyLinkow);


            cecha.Nazwa = "XL";

            A.CallTo(() => cecha.PobierzAtrybut()).Returns(atrybut);
            string link = UrlExtender.ZbudujLink(testowyKontroler.Url, cecha, jezykTestowyEn);

            Assert.True("/en/p?filtry=rozmiar[XL]" == link);
            
            cecha.Nazwa = "XL - XXL";
            link = UrlExtender.ZbudujLink(testowyKontroler.Url, cecha, jezykTestowyEn);

            Assert.True("/en/p?filtry=rozmiar[XL+-+XXL]" == link);
        }

        [Fact(DisplayName = "Tworzenie linków do stron")]
        public void ZbudujLinkTest_Strona()
        {
            konfiguracjaDlaWieluJezykow();
            TrescBll strona = new TrescBll(calosc);
            strona.Aktywny = true;
            strona.Id = 55;
            strona.Symbol = "logowanie";

            //TEST bez linku alternatywnego
            string linkWynik = UrlExtender.ZbudujLink(testowyKontroler.Url, strona, klient, jezykTestowyEn);
            Assert.True(linkWynik == "/en/logowanie", linkWynik);

            //TEST bez linku alternatywnego ale z returnURL
            string returnURL = "/jakislink/takm.html?cos=ffd&opcja=2";
            Dictionary<string, string> dodatkoweParam = new Dictionary<string, string> {{"ReturnUrl", returnURL}};
            linkWynik = UrlExtender.ZbudujLink(testowyKontroler.Url, strona, klient, jezykTestowyEn, dodatkoweParam);

            string spodziewanyLink = "/en/logowanie?ReturnUrl=" + HttpUtility.UrlEncode(returnURL);

            Assert.True(linkWynik.ToUpper() == spodziewanyLink.ToUpper());
        }

 
        public void ZbudujLinkTest_ZmianaIP()
        {
            klient.GidIp = "634534534";
            string linkWynik = UrlExtender.ZbudujLinkZmianaAdresuIpKlienta(testowyKontroler.Url, klient);
            Assert.True(linkWynik == "/en/Klienci/ZmianaIp/634534534", linkWynik);
        }

        [Fact(DisplayName = "Tworzenie linków do stron - linki HTTP")]
        public void ZbudujLinkTest_StronaHttp()
        {
            konfiguracjaDlaWieluJezykow();
            TrescBll strona = new TrescBll(calosc);
            strona.Aktywny = true;
            strona.Id = 55;
            strona.Symbol = "logowanie";
            strona.LinkAlternatywny = "http://wp.pl";

            //TEST bez linku alternatywnego
            string linkWynik = UrlExtender.ZbudujLink(testowyKontroler.Url, strona, klient, jezykTestowyEn);
            Assert.True(linkWynik.ToUpper() == strona.LinkAlternatywny.ToUpper());

            //TEST bez linku alternatywnego ale z returnURL
            string returnURL = "/jakislink/takm.html?cos=ffd&opcja=2";
            Dictionary<string, string> dodatkoweParam = new Dictionary<string, string> {{"ReturnUrl", returnURL}};
            linkWynik = UrlExtender.ZbudujLink(testowyKontroler.Url, strona, klient, jezykTestowyEn, dodatkoweParam);

            Assert.True(linkWynik.ToUpper() == strona.LinkAlternatywny.ToUpper());
        }

        [Fact(DisplayName = "Tworzenie linków do stron - linki MailTo")]
        public void ZbudujLinkTest_StronaMailTo()
        {
            konfiguracjaDlaWieluJezykow();
            TrescBll strona = new TrescBll(calosc);
            strona.Aktywny = true;
            strona.Id = 55;
            strona.Symbol = "logowanie";
            strona.LinkAlternatywny = "mailto:spam@solex.net.pl";

            //TEST bez linku alternatywnego
            string linkWynik = UrlExtender.ZbudujLink(testowyKontroler.Url, strona, klient, jezykTestowyEn);
            Assert.True(linkWynik.ToUpper() == strona.LinkAlternatywny.ToUpper());

            //TEST bez linku alternatywnego ale z returnURL
            string returnURL = "/jakislink/takm.html?cos=ffd&opcja=2";
            Dictionary<string, string> dodatkoweParam = new Dictionary<string, string> {{"ReturnUrl", returnURL}};
            linkWynik = UrlExtender.ZbudujLink(testowyKontroler.Url, strona, klient, jezykTestowyEn, dodatkoweParam);

            Assert.True(linkWynik.ToUpper() == strona.LinkAlternatywny.ToUpper());
        }

        [Fact(DisplayName = "Tworzenie linków do stron - modalne")]
        public void ZbudujLinkTest_StronaModalne()
        {
            konfiguracjaDlaWieluJezykow();
            TrescBll strona = new TrescBll(calosc);
            strona.Aktywny = true;
            strona.Id = 55;
            strona.Symbol = "logowanie";
            strona.SposobOtwierania = Model.Enums.SposobOtwieraniaModal.ZalogowaniModalNiezalogowaniModal;

            //TEST bez linku alternatywnego
            string linkWynik = UrlExtender.ZbudujLink(testowyKontroler.Url, strona, klient, jezykTestowyEn);
            Assert.True(HttpUtility.UrlDecode(linkWynik).ToUpper() == "/en/logowanie/m".ToUpper());

            //TEST bez linku alternatywnego ale z returnURL
            string returnURL = "smieciReturn";
            Dictionary<string, string> dodatkoweParam = new Dictionary<string, string> {{"ReturnUrl", returnURL}};
            linkWynik = UrlExtender.ZbudujLink(testowyKontroler.Url, strona, klient, jezykTestowyEn, dodatkoweParam);

            Assert.True(HttpUtility.UrlDecode(linkWynik).ToUpper() == "/en/logowanie/m?ReturnUrl=smieciReturn".ToUpper());
        }


        [Fact(DisplayName = "Tworzenie linków do stron - NIE modalne")]
        public void ZbudujLinkTest_StronaNieModalne()
        {
            konfiguracjaDlaWieluJezykow();
            TrescBll strona = new TrescBll(calosc);
            strona.Aktywny = true;
            strona.Id = 55;
            strona.Symbol = "logowanie";
            strona.SposobOtwierania = Model.Enums.SposobOtwieraniaModal.ZalogowaniNieModalNiezalogowaniNieModal;

            //TEST bez linku alternatywnego
            string linkWynik = UrlExtender.ZbudujLink(testowyKontroler.Url, strona, klient, jezykTestowyEn);
            Assert.True(linkWynik.ToUpper() == "/en/logowanie".ToUpper());

            //TEST bez linku alternatywnego ale z returnURL
            string returnURL = "smieciReturn";
            Dictionary<string, string> dodatkoweParam = new Dictionary<string, string> {{"ReturnUrl", returnURL}};
            linkWynik = UrlExtender.ZbudujLink(testowyKontroler.Url, strona, klient, jezykTestowyEn, dodatkoweParam);

            Assert.True(linkWynik.ToUpper() == "/en/logowanie?ReturnUrl=smieciReturn".ToUpper());
        }


        [Fact(DisplayName = "Tworzenie linków do stron z linku alternatywnego - ale nie zewnatrzenego - proste przypadki NIE modalne")]
        public void ZbudujLinkTest_StronaLinkiALternatywne()
        {
            konfiguracjaDlaWieluJezykow();
            TrescBll strona = new TrescBll(calosc);
            strona.Aktywny = true;
            strona.Id = 55;
            strona.Symbol = "logowanie";
            strona.LinkAlternatywny = "login";
            strona.SposobOtwierania = SposobOtwieraniaModal.ZalogowaniNieModalNiezalogowaniNieModal;

            //TEST bez linku alternatywnego
            string linkWynik = UrlExtender.ZbudujLink(testowyKontroler.Url, strona, klient, jezykTestowyEn);
            Assert.True(linkWynik == "/en/login");

            //TEST bez linku alternatywnego ale z returnURL
            string returnURL = "/jakislink/takm.html?cos=ffd&opcja=2";
            Dictionary<string, string> dodatkoweParam = new Dictionary<string, string> {{"ReturnUrl", returnURL}};
            linkWynik = UrlExtender.ZbudujLink(testowyKontroler.Url, strona, klient, jezykTestowyEn, dodatkoweParam);

            string spodziewanyLink = "/en/login?ReturnUrl=" + HttpUtility.UrlEncode(returnURL);
            Assert.True(linkWynik.ToUpper() == spodziewanyLink.ToUpper());
        }

        [Fact(DisplayName = "Tworzenie linków do stron z linku alternatywnego - ale nie zewnatrzenego - proste przypadki MODALNE")]
        public void ZbudujLinkTest_StronaLinkiALternatywne_modalne()
        {
            konfiguracjaDlaWieluJezykow();
            TrescBll strona = new TrescBll(calosc);
            strona.Aktywny = true;
            strona.Id = 55;
            strona.Symbol = "logowanie";
            strona.LinkAlternatywny = "logowanie";
            strona.SposobOtwierania = SposobOtwieraniaModal.ZalogowaniModalNiezalogowaniModal;

            //TEST bez linku alternatywnego
            string linkWynik = UrlExtender.ZbudujLink(testowyKontroler.Url, strona, klient, jezykTestowyEn);
            Assert.True(linkWynik == "/en/logowanie/m");

            //TEST bez linku alternatywnego ale z returnURL
            string returnURL = "/jakislink/takm.html?cos=ffd&opcja=2";
            Dictionary<string, string> dodatkoweParam = new Dictionary<string, string> { { "ReturnUrl", returnURL } };
            linkWynik = UrlExtender.ZbudujLink(testowyKontroler.Url, strona, klient, jezykTestowyEn, dodatkoweParam);

            string spodziewanyLink = "/en/logowanie/m?ReturnUrl=" + HttpUtility.UrlEncode(returnURL);
            Assert.True(linkWynik.ToUpper() == spodziewanyLink.ToUpper());
        }

        [Fact(DisplayName = "Tworzenie linków do stron z linku alternatywnego - ale nie zewnatrzenego z specjalnymi znakami - trudne przypadki")]
        public void ZbudujLinkTest_StronaLinkiALternatywneTrudnePrzypadki()
        {
            konfiguracjaDlaWieluJezykow();
            TrescBll strona = new TrescBll(calosc);
            strona.Aktywny = true;
            strona.Id = 55;
            strona.Symbol = "logowanie";
            strona.LinkAlternatywny = "login/produkt2/4,m?opcja1=3&filtr4=rozmiar[xl]";

            //TEST bez return url - dziwne znaki w linu alternatywnym
            string linkWynik = UrlExtender.ZbudujLink(testowyKontroler.Url, strona, klient, jezykTestowyEn);
            Assert.True(linkWynik == "/en/login/produkt2/4,m?opcja1=3&filtr4=rozmiar[xl]");

            //TEST bez linku alternatywnego ale z returnURL
            string returnURL = "/jakislink/takm.html?cos=ffd&opcja=2";
           
            Dictionary<string, string> dodatkoweParam = new Dictionary<string, string> {{"ReturnUrl", returnURL}};
            linkWynik  = UrlExtender.ZbudujLink(testowyKontroler.Url, strona, klient, jezykTestowyEn, dodatkoweParam);
            Assert.True(linkWynik == "/en/login/produkt2/4,m?opcja1=3&filtr4=rozmiar[xl]&ReturnUrl=" + HttpUtility.UrlEncode(returnURL));
        }


        [Fact(DisplayName = "Tworzenie linków do stron z linku alternatywnego - ale nie zewnatrzenego - proste przypadki NIE modalne - BEZ jezykow")]
        public void ZbudujLinkTest_StronaLinkiALternatywne_bez_jezykow()
        {
            konfiguracjaDlaJednegoJezyka();
            TrescBll strona = new TrescBll(calosc);
            strona.Aktywny = true;
            strona.Id = 55;
            strona.Symbol = "logowanie";
            strona.LinkAlternatywny = "login";
            strona.SposobOtwierania = SposobOtwieraniaModal.ZalogowaniNieModalNiezalogowaniNieModal;

            //TEST bez linku alternatywnego
            string linkWynik = UrlExtender.ZbudujLink(testowyKontroler.Url, strona, klient, jezykTestowyEn);
            Assert.True(linkWynik == "/login");

            //TEST bez linku alternatywnego ale z returnURL
            string returnURL = "/jakislink/takm.html?cos=ffd&opcja=2";
            Dictionary<string, string> dodatkoweParam = new Dictionary<string, string> { { "ReturnUrl", returnURL } };
            linkWynik = UrlExtender.ZbudujLink(testowyKontroler.Url, strona, klient, jezykTestowyEn, dodatkoweParam);

            string spodziewanyLink = "/login?ReturnUrl=" + HttpUtility.UrlEncode(returnURL);
            Assert.True(linkWynik.ToUpper() == spodziewanyLink.ToUpper());
        }

        [Fact(DisplayName = "Tworzenie linków do stron z linku alternatywnego - ale nie zewnatrzenego - proste przypadki MODALNE - BEZ jezykow")]
        public void ZbudujLinkTest_StronaLinkiALternatywne_modalne_bez_jezykow()
        {
            konfiguracjaDlaJednegoJezyka();
            TrescBll strona = new TrescBll(calosc);
            strona.Aktywny = true;
            strona.Id = 55;
            strona.Symbol = "logowanie";
            strona.LinkAlternatywny = "logowanie";
            strona.SposobOtwierania = SposobOtwieraniaModal.ZalogowaniModalNiezalogowaniModal;

            //TEST bez linku alternatywnego
            string linkWynik = UrlExtender.ZbudujLink(testowyKontroler.Url, strona, klient, jezykTestowyEn);
            Assert.True(linkWynik == "/logowanie/m", linkWynik);

            //TEST bez linku alternatywnego ale z returnURL
            string returnURL = "/jakislink/takm.html?cos=ffd&opcja=2";
            Dictionary<string, string> dodatkoweParam = new Dictionary<string, string> { { "ReturnUrl", returnURL } };
            linkWynik = UrlExtender.ZbudujLink(testowyKontroler.Url, strona, klient, jezykTestowyEn, dodatkoweParam);

            string spodziewanyLink = "/logowanie/m?ReturnUrl=" + HttpUtility.UrlEncode(returnURL);
            Assert.True(linkWynik.ToUpper() == spodziewanyLink.ToUpper());
        }

        [Fact(DisplayName = "Tworzenie linków do produktu - NIE modalne")]
        public void ZbudujLinkTest_Produkt()
        {
            konfiguracjaDlaWieluJezykow();
            TrescBll stronaZProduktem = new TrescBll(calosc);
            stronaZProduktem.Aktywny = true;
            stronaZProduktem.Id = 55;
            stronaZProduktem.Symbol = "p";
            stronaZProduktem.SposobOtwierania = SposobOtwieraniaModal.ZalogowaniNieModalNiezalogowaniNieModal;

            UrlExtender.StronaZProduktem = stronaZProduktem;

            ProduktKlienta pk = A.Fake<ProduktKlienta>();
            pk.Nazwa = "produkt nazwa ,jaks, 56 - nowość";
            pk.FriendlyLinkURL = "produkt-3434-f-ddd";
            A.CallTo(() => pk.Id).Returns(54);

            //TEST klient zalogowany
            IKlient klient = A.Fake<IKlient>();
            A.CallTo(() => klient.Dostep).Returns(AccesLevel.Zalogowani);
            pk.Klient = klient;

            string linkWynik = UrlExtender.ZbudujLink(testowyKontroler.Url, pk, jezykTestowyEn, false);
            Assert.True(linkWynik == "/en/produkt-3434-f-ddd/p54");

            //test NIE zalogowni
            A.CallTo(() => klient.Dostep).Returns(AccesLevel.Niezalogowani);

            linkWynik = UrlExtender.ZbudujLink(testowyKontroler.Url, pk, jezykTestowyEn, false);
            Assert.True(linkWynik == "/en/produkt-3434-f-ddd/p54");
        }

        [Fact(DisplayName = "Tworzenie linków do produktu - MODALNE")]
        public void ZbudujLinkTest_ProduktModalne()
        {
            konfiguracjaDlaWieluJezykow();
            TrescBll stronaZProduktem = new TrescBll(calosc);
            stronaZProduktem.Aktywny = true;
            stronaZProduktem.Id = 55;
            stronaZProduktem.Symbol = "p";
            stronaZProduktem.SposobOtwierania = SposobOtwieraniaModal.ZalogowaniModalNiezalogowaniModal;
            UrlExtender.StronaZProduktem = stronaZProduktem;

            ProduktKlienta pk = A.Fake<ProduktKlienta>();
            pk.Nazwa = "produkt nazwa ,jaks, 56 - nowość";
            pk.FriendlyLinkURL = "produkt-3434-f-ddd";
            A.CallTo(() => pk.Id).Returns(54);

            //TEST klient zalogowany
            IKlient klient = A.Fake<IKlient>();
            A.CallTo(() => klient.Dostep).Returns(AccesLevel.Zalogowani);
            pk.Klient = klient;

            string linkWynik = UrlExtender.ZbudujLink(testowyKontroler.Url, pk, jezykTestowyEn, false);
            Assert.True(linkWynik == "/en/produkt-3434-f-ddd/p54/m");

            //test NIE zalogowni
            A.CallTo(() => klient.Dostep).Returns(AccesLevel.Niezalogowani);

            linkWynik = UrlExtender.ZbudujLink(testowyKontroler.Url, pk, jezykTestowyEn, false);
            Assert.True(linkWynik == "/en/produkt-3434-f-ddd/p54/m");
        }

        [Fact(DisplayName = "Tworzenie linków do kategori")]
        public void ZbudujLinkTest_Kategoria()
        {
            konfiguracjaDlaWieluJezykow();
            KategorieBLL kat = new KategorieBLL();
            kat.Id = 37;
            kat.Nazwa = "dzika kategoria";
            kat.FriendlyLinkURL = "linkowana-nazwa";

            string linkWynik = UrlExtender.ZbudujLink(testowyKontroler.Url, kat, jezykTestowyEn);
            Assert.True(linkWynik == "/en/linkowana-nazwa/pk37", linkWynik);
        }


        [Fact(DisplayName = "Test linka do danych tstow do katalogu")]
        public void ZbudujLinkDoTestowychDanychKataloguProduktowTest()
        {
            konfiguracjaDlaWieluJezykow();

            PlikIntegracjiSzablon zrodlo = new PlikIntegracjiSzablon() {IdSzablonu = 6, Format = "JSON", Szablon = "Podstawowy-plik-danych", typDanych = TypDanychIntegracja.ProduktyKatalogDrukowanie, Wersja = new List<int> {2, 1}};

            string linkWynik = UrlExtender.ZbudujLinkDoTestowychDanychKataloguProduktow(testowyKontroler.Url, zrodlo);
            Assert.True(linkWynik == "/xmlapi/test-catalog/2/6", linkWynik);

            konfiguracjaDlaJednegoJezyka();
            linkWynik = UrlExtender.ZbudujLinkDoTestowychDanychKataloguProduktow(testowyKontroler.Url, zrodlo);
            Assert.True(linkWynik == "/xmlapi/test-catalog/2/6", linkWynik);
        }


        [Fact(DisplayName = "Wypisanie z newslettera")]
        public void ZbudujLinkTest_WypisanieNewsletter()
        {
            konfiguracjaDlaWieluJezykow();

            string linkWynik = UrlExtender.WypiszZNewslettera(testowyKontroler.Url, klient);
            string klucz = SolexBllCalosc.PobierzInstancje.Klienci.KluczDoKlientaWypisanieZapisaniaZNewsletera(klient);
            string linkDocelowy = ("http://localhost/newsletter/wypisz/" + klient.Email + "/" + klucz).ToUpper();
            Assert.True(HttpUtility.UrlDecode(linkWynik).ToUpper() == linkDocelowy, linkWynik);
        }

        [Fact(DisplayName = "Budowa linku do Importu")]
        public void ZbudujLinkDoImportuTresciAdminTest()
        {
            konfiguracjaDlaWieluJezykow();

            string linkWynik = UrlExtender.ZbudujLinkDoImportuAdmin(testowyKontroler.Url, typeof(TlumaczeniePole), jezykTestowyEn );
            string linkDocelowy = ("/en/Admin/Import");
            Assert.True(linkWynik == linkDocelowy, linkWynik);

            konfiguracjaDlaJednegoJezyka();

            linkWynik = UrlExtender.ZbudujLinkDoImportuAdmin(testowyKontroler.Url, typeof(TlumaczeniePole), jezykTestowyEn);
            linkDocelowy = ("/Admin/Import");
            Assert.True(linkWynik == linkDocelowy, linkWynik);
        }


        [Fact(DisplayName = "Link do listy blogów - wielojezykowosc")]
        public void ZbudujLinkTest_BlogWpis()
        {
            BlogWpisBll blog = A.Fake<BlogWpisBll>();
            blog.Id = 765;
            blog.Aktywny = true;            
            blog.Tytul = "Super fajny blog o czyms ciekawym!";
            blog.LinkURL = "super-fajny-blog-o-czyms-ciekawym";

            string symbolStronyUkladuBloga = "przepisy";

            string linkWynik = UrlExtender.ZbudujLink(testowyKontroler.Url,  blog, symbolStronyUkladuBloga, jezykTestowyEn);

            Assert.True(linkWynik == "/en/przepisy/super-fajny-blog-o-czyms-ciekawym/b765", linkWynik);
        }


        [Fact(DisplayName = "Linki do listy produktow")]
        public void ZbudujLinkProduktyListaTest()
        {
            A.CallTo(() => config.AdresStronyZProduktami).Returns("p?czyscstalefiltry");
            konfiguracjaDlaWieluJezykow();
            string link = UrlExtender.LinkProdukty(testowyKontroler.Url, jezykTestowyEn);

            Assert.True(link == "/en/p?czyscstalefiltry", link);

            konfiguracjaDlaJednegoJezyka();
            link = UrlExtender.LinkProdukty(testowyKontroler.Url, jezykTestowyEn);

            Assert.True(link == "/p?czyscstalefiltry", link);
        }


        [Fact(DisplayName = "Linki do ZbudujLinkPowrotu - bez podawanie linku powrotu")]
        public void ZbudujLinkPowrotuTest()
        {
            //Z ajaxa
            konfiguracjaDlaWieluJezykow();
            bool ajax = true;
            string link = UrlExtender.ZbudujLinkPowrotu(testowyKontroler.Url, ajax, null, null);

            Assert.True(link == null && ajax == true, link);

            konfiguracjaDlaJednegoJezyka();
            link = UrlExtender.ZbudujLinkPowrotu(testowyKontroler.Url, ajax, null, null);

            Assert.True(link == null && ajax == true, link);

            //bez AJAXEM
            konfiguracjaDlaWieluJezykow();
            ajax = false;
            link = UrlExtender.ZbudujLinkPowrotu(testowyKontroler.Url, ajax, null, jezykTestowyPl);

            Assert.True(link == "/pl" && ajax == false, link);

            konfiguracjaDlaJednegoJezyka();
            link = UrlExtender.ZbudujLinkPowrotu(testowyKontroler.Url, ajax, null, jezykTestowyEn);

            Assert.True(link == "/" && ajax == false, link);
        }


        [Fact(DisplayName = "Linki do ZbudujLinkPowrotu - bez podawanie linku powrotu, z refereme")]
        public void ZbudujLinkPowrotuTest_referer()
        {
            string referer = "http://localhost/pl/jedzida/pk114227?szukanaWewnetrzne=jezdzidd?";
            
            //Z ajaxa
            konfiguracjaDlaWieluJezykow();
            bool ajax = true;
            string link = UrlExtender.ZbudujLinkPowrotu(testowyKontroler.Url, ajax, referer, null);

            Assert.True(link == null && ajax == true, link);

            konfiguracjaDlaJednegoJezyka();
            link = UrlExtender.ZbudujLinkPowrotu(testowyKontroler.Url, ajax, referer, null);

            Assert.True(link == null && ajax == true, link);

            //BEZ AJAXEM - przyrost przypadek
            konfiguracjaDlaWieluJezykow();
            ajax = false;
            link = UrlExtender.ZbudujLinkPowrotu(testowyKontroler.Url, ajax, referer, jezykTestowyPl);
            Assert.True(link == referer.ToString() && ajax == false, link);

            konfiguracjaDlaJednegoJezyka();
            link = UrlExtender.ZbudujLinkPowrotu(testowyKontroler.Url, ajax, referer, jezykTestowyEn);
            Assert.True(link == referer.ToString() && ajax == false, link);

            //bez ajaxa trudne przypadki - lista produktow
            konfiguracjaDlaWieluJezykow();
            link = UrlExtender.ZbudujLinkPowrotu(testowyKontroler.Url, ajax, referer, jezykTestowyPl, "/p");
            Assert.True("/pl/p" == link, link);

            konfiguracjaDlaJednegoJezyka();
            link = UrlExtender.ZbudujLinkPowrotu(testowyKontroler.Url, ajax, referer, jezykTestowyEn, "/p");
            Assert.True(link == "/p" && ajax == false, link);
        }

        [Fact(DisplayName = "Linki powrotu z jezykami")]
        public void ZbudujLinkPowrotuTest_referer_z_jezykiem()
        {
            string referer = "http://localhost/pl/jedzida/pk114227?szukanaWewnetrzne=jezdzidd?";
            konfiguracjaDlaWieluJezykow();
            //nigdzie nie uzywane
            //bool ajax = true;
            string link = UrlExtender.ZbudujLinkPowrotu(testowyKontroler.Url, false, referer, jezykTestowyPl, "/pl/p/");

            Assert.True(link == "/pl/p", link);
        }

        [Fact(DisplayName = "Link do drukowania katalogu z koszyka")]
        public void ZbudujLinkDoDrukowaniaKataloguKoszykaTest()
        {
            konfiguracjaDlaWieluJezykow();
            string link = UrlExtender.ZbudujLinkDoDrukowaniaKataloguKoszyka(testowyKontroler.Url, jezykTestowyPl);
            Assert.True(link == "/pl/xmlapi/print", link);

            konfiguracjaDlaJednegoJezyka();
            link = UrlExtender.ZbudujLinkDoDrukowaniaKataloguKoszyka(testowyKontroler.Url, jezykTestowyPl);
            Assert.True(link == "/xmlapi/print", link);
        }

        public class TestowyRouting
        {
            public string urlTestowy { get; set; }
            public string controller { get; set; }
            public string action { get; set; }
            public Dictionary<string, string> parametry { get; set; }

            public TestowyRouting(string url, string controler, string action, Dictionary<string, string> parametry)
            {
                this.urlTestowy = url;
                this.controller = controler;
                this.action = action;
                this.parametry = parametry;
            }
        }

        //w linkach nie moze byc QS bo nie ma tego obslugi w testach
        private readonly List<TestowyRouting> listaTestowaRoutingow = new List<TestowyRouting>
        {
            new TestowyRouting("~/przepisy/super-fajny-blog-o-czyms-ciekawym/b765", "tresci", "WpisyBloga", new Dictionary<string, string> {{ "blogWpisId", "765" }, { "symbol", "przepisy"}  }),
            new TestowyRouting("~/Produkty/ListaWczytaj", "Produkty", "ListaWczytaj", null),
            new TestowyRouting("~/p", "Tresci", "StronaSymbol", new Dictionary<string, string> {{"symbol", "p"}}),
            new TestowyRouting("~/amarantus-bezglutenowy-bio-1000g/p291/m", "Tresci", "SzczegolyProduktu", new Dictionary<string, string> {{"modal", "m"}, {"produktId", "291"}}),
            new TestowyRouting("~/KategorieProduktowe/DrzewkoKategorie", "KategorieProduktowe", "DrzewkoKategorie", null),
            new TestowyRouting("~/Produkty/Dymek", "Produkty", "Dymek", null),
            new TestowyRouting("~/denverfood/pk48", "Tresci", "KategoriaProduktu", new Dictionary<string, string> {{"nazwa", "denverfood"}}),

            new TestowyRouting("~/dokumenty/PobierzListe/Faktura/2016-09-06/2016-11-04/False/True", "Dokumenty", "PobierzListe", new Dictionary<string, string>
            {
                { "rodzaj", "Faktura" }, { "pokazKolumneZrealizowane", "True" }, { "pokazstatus", "False"},
                { "odKiedy", "2016-09-06"}, { "doKiedy", "2016-11-04"}
            }),

            new TestowyRouting("~/ProfilKlienta/UstawWartoscUstawienia", "ProfilKlienta", "UstawWartoscUstawienia", null ),
            new TestowyRouting("~/dokumenty/Pobierz/7553/SolEx.Hurt.Core.Importy.Eksporty.PcMarket/", "dokumenty", "Pobierz", new Dictionary<string, string> {{"id", "7553" } , {"format", "SolEx.Hurt.Core.Importy.Eksporty.PcMarket" }    }),
            new TestowyRouting("~/koszyk/podglad/5", "koszyk", "podglad", new Dictionary<string, string> {{ "iloscPozycji", "5"}}),
            new TestowyRouting("~/dokumenty/Pokaz/8707", "dokumenty", "Pokaz", new Dictionary<string, string> {{"id", "8707" } }),

            new TestowyRouting("~/zmiana-hasla", "Tresci", "StronaSymbol", new Dictionary<string, string> {{"symbol", "zmiana-hasla" } }),
            new TestowyRouting("~/Klienci/ZmianaIp/4563534", "Klienci", "ZmianaIp", new Dictionary<string, string> {{"hash", "4563534" } }),

            new TestowyRouting("~/newsletter/Wypisz/spam@sdfsf.com/453453453v453454", "Newsletter", "Wypisz", new Dictionary<string, string> {{"email", "spam@sdfsf.com" }, { "klucz", "453453453v453454" } }),
            new TestowyRouting("~/newsletter/Zapisz/spam@sdfsf.com/453453453v4534547", "Newsletter", "Zapisz", new Dictionary<string, string> {{ "email", "spam@sdfsf.com" }, { "klucz", "453453453v4534547" } }),

            new TestowyRouting("~/newsletter/ZapiszDoNewslettera", "Newsletter", "ZapiszDoNewslettera",  null ),

            new TestowyRouting("~/tresci/menu", "tresci", "Menu", null),
            new TestowyRouting("~/tresci/menu/66", "Tresci", "Menu", new Dictionary<string, string> {{ "id", "66" } }),

            new TestowyRouting("~/xmlapi/gen-client-key", "integracja", "GenerowanieKluczaAktualnegoKlienta", null),
            new TestowyRouting("~/xmlapi/print", "integracja", "DrukujKatalogProduktow", null),
            new TestowyRouting("~/xmlapi/test-catalog/2/42", "Integracja", "GenerujTestowyPlikDanychKatalogu", new Dictionary<string, string> {{ "szablonId", "42" }, { "wersja", "2" } }),
            
            new TestowyRouting("~/admin/Import","Admin","Import",null)
        };

        [Fact(DisplayName = "Interpretacja routingow - WIELOjezykowosc")]
        public void InterpretacjaRoutingow_wieloJezykowosc()
        {
            konfiguracjaDlaWieluJezykow();
            System.Web.Routing.RouteCollection routeCollection = System.Web.Routing.RouteTable.Routes;

            var listaTestowaRoutingowPrzepisanaWieloJezyczna = new List<TestowyRouting>();
            foreach (TestowyRouting r in listaTestowaRoutingow)
            {
                TestowyRouting nowy = new TestowyRouting(r.urlTestowy.Replace("~/", "~/en/"), r.controller, r.action, r.parametry);
                listaTestowaRoutingowPrzepisanaWieloJezyczna.Add(nowy);
            }


            foreach (TestowyRouting test in listaTestowaRoutingowPrzepisanaWieloJezyczna)
            {
                var context = this.GetContextFake(test.urlTestowy);

                //czy jest routing wymagany w ogole
                bool znalezionoRouting = false;
                foreach (RouteBase x in routeCollection)
                {
                    Route r = x as Route;
                    if (r == null)
                    {
                        continue;
                    }

                    if (r.Defaults == null)
                    {
                        continue;
                    }

                    string contr = r.Defaults["controller"].ToString().ToUpper();

                    if (contr == test.controller.ToUpper())
                    {
                        string action = r.Defaults["action"].ToString().ToUpper();

                        if (action == test.action.ToUpper())
                        {
                            znalezionoRouting = true;
                            break;
                        }
                    }
                }

                if (!znalezionoRouting)
                {
                    throw new Exception("Brak routingu dla URLa: " + test.urlTestowy);
                }

                RouteData routeData = RouteTable.Routes.GetRouteData(context);

                if (routeData == null)
                {
                    throw new Exception("Błąd routingu - obiekt pusty");
                }

                if (routeData.Values.ContainsKey("MS_DirectRouteMatches"))
                {
                    routeData = ((IEnumerable<RouteData>)routeData.Values["MS_DirectRouteMatches"]).First();
                }
                this.AssertRouteData(routeData, test.controller, test.action, test.parametry, test.urlTestowy);
            }
        }




        [Fact(DisplayName = "Interpretacja routingow - JEDEN JEZYK")]
        public void InterpretacjaRoutingow()
        {
            konfiguracjaDlaJednegoJezyka();
            System.Web.Routing.RouteCollection routeCollection = System.Web.Routing.RouteTable.Routes;

            foreach (TestowyRouting test in listaTestowaRoutingow)
            {
                var context = this.GetContextFake(test.urlTestowy);

                //czy jest routing wymagany w ogole
                bool znalezionoRouting = false;
                foreach (RouteBase x in routeCollection)
                {
                    Route r = x as Route;
                    if (r == null)
                    {
                        continue;
                    }

                    if (r.Defaults == null)
                    {
                        continue;
                    }

                    string contr = r.Defaults["controller"].ToString().ToUpper();

                    if (contr == test.controller.ToUpper())
                    {
                        string action = r.Defaults["action"].ToString().ToUpper();

                        if (action == test.action.ToUpper())
                        {
                            znalezionoRouting = true;
                            break;
                        }
                    }
                }

                if (!znalezionoRouting)
                {
                    throw new Exception("Brak routingu dla URLa: " + test.urlTestowy);
                }

                RouteData routeData = RouteTable.Routes.GetRouteData(context);

                if (routeData == null)
                {
                    throw new Exception("Błąd routingu - obiekt pusty dla URLa: " + test.urlTestowy);
                }

                if (routeData.Values.ContainsKey("MS_DirectRouteMatches"))
                {
                    routeData = ((IEnumerable<RouteData>) routeData.Values["MS_DirectRouteMatches"]).First();
                }
                this.AssertRouteData(routeData, test.controller, test.action, test.parametry, test.urlTestowy);
            }
        }

        protected void AssertRouteData(RouteData routeData, string controller, string action, Dictionary<string, string> parametry, string URL)
        {
            string controlerWybrany, actionWybrany;
            Assert.NotNull(routeData);

            try
            {
                controlerWybrany = routeData.Values["controller"].ToString();
                actionWybrany = routeData.Values["action"].ToString();
            }
            catch (Exception)
            {
                throw new Exception("Nie można pobrać kontrolera lub akcji dla URL: " + URL);
            }

            Assert.NotNull(controlerWybrany);
            Assert.NotNull(actionWybrany);

            Assert.True(controller.ToUpper() == controlerWybrany.ToUpper(), string.Format("dla URL: {0} ma być controler: {1} a jest: {2}", URL, controller, controlerWybrany) );
            Assert.True(action.ToUpper() == actionWybrany.ToUpper(), "action zwrocony w tescie: " + actionWybrany + " a powinno być: " + action + ". URL testowy: " + URL);

            if (parametry != null)
            {
                foreach (var p in parametry)
                {
                    if (!routeData.Values.ContainsKey(p.Key))
                    {
                        throw new Exception("Brak parametru: " + p.Key + " dla URLa: " + URL);
                    }

                    string rutValue = routeData.Values[p.Key].ToString();

                    if (rutValue == null)
                    {
                        continue;
                    }

                    Assert.True(p.Value.ToUpper() == rutValue.ToUpper(), string.Format("zła wartość parametru: {0} dla URLa: {1}", p.Key, URL));
                }
            }
        }
        [Fact(DisplayName = "Tworzenie linków do plików użytkownika")]
        public void PobierzSciezkePlikUseraTest()
        {
            HttpContext.Current = new HttpContext(new HttpRequest(null, "http://localhost", null), new HttpResponse(null));


            UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            Type typ = typeof(Rejestracja);
            long idObiektu = 1;
            string nazwaPoprawna = "plik.png";
            string nazwaNieporawna = "pli/k.png";
           //nigdzie nie uzywane 
            // string aktualnySciezka = "AppDomain.CurrentDomain.BaseDirectory";

            string katalog = "Zasoby\\" + typ.Name + "\\" + idObiektu + "\\" + nazwaPoprawna;

            string spodziewanyWynik1 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, katalog);
            string spodziewanyWynik2 = Path.Combine("http://localhost/", katalog.Replace("\\", "/"));


            string wynik = helper.PobierzSciezkePlikUsera(typ, idObiektu, nazwaPoprawna, false);
            Assert.Equal(spodziewanyWynik1, wynik);

            try
            {
                helper.PobierzSciezkePlikUsera(typ, idObiektu, nazwaNieporawna, false);
                Assert.True(false);
            }
            catch (InvalidOperationException) { }
            try
            {
                helper.PobierzSciezkePlikUsera(typ, 0, nazwaPoprawna, false);
                Assert.True(false);
            }
            catch (InvalidOperationException) { }
            wynik = helper.PobierzSciezkePlikUsera(typ, 1, nazwaPoprawna, true);
            Assert.Equal(spodziewanyWynik2, wynik);
        }

        [Fact( DisplayName = "Budowanie linku do importu")]
        public void BudowanieLinkuDOImportowAdminCSV()
        {
            konfiguracjaDlaJednegoJezyka();

            string typ = typeof(Model.Klient).PobierzOpisTypu();

            RouteValueDictionary parametry = new RouteValueDictionary() { { "typ", typ }, { "modal", "m" } };
            string link = testowyKontroler.Url.Action("importForm", "Admin", parametry);

            Assert.True(link == HttpUtility.UrlEncode($"/Admin/importForm/m?typ={typ}"), link);
        }

        [Fact(DisplayName = "Testowanie global asax odczytanie jezyka z requestu")]
        public void ProbojPobracJezykZLinkaRequestuTest()
        {
            konfiguracjaDlaWieluJezykow();
            Global global = A.Fake<Global>();
            global.Calosc = calosc;

            Uri url = new Uri("http://localhost/pl/costam/inne/sdfs?rfrr5%dd&d=2");
            Jezyk jezyk = global.ProbojPobracJezykZLinkaRequestu(url);

            Assert.True(jezyk == slownikJezykowSymbolach["pl"]);

            url = new Uri("http://localhost/en/costam/inne/sdfs?rfrr5%dd&d=2");
            jezyk = global.ProbojPobracJezykZLinkaRequestu(url);
            Assert.True(jezyk == slownikJezykowSymbolach["en"]);


            url = new Uri("http://localhost/en5/costam/inne/sdfs?rfrr5%dd&d=2");
            jezyk = global.ProbojPobracJezykZLinkaRequestu(url);
            Assert.True(jezyk == null);

            url = new Uri("http://localhost/costam/inne/sdfs?rfrr5%dd&d=2");
            jezyk = global.ProbojPobracJezykZLinkaRequestu(url);
            Assert.True(jezyk == null);

            url = new Uri("http://localhost/erew/costam/inne/sdfs?rfrr5%dd&d=2");
            jezyk = global.ProbojPobracJezykZLinkaRequestu(url);
            Assert.True(jezyk == null);

            url = new Uri("http://localhost/");
            jezyk = global.ProbojPobracJezykZLinkaRequestu(url);
            Assert.True(jezyk == null);

            url = new Uri("http://localhost/en/p");
            jezyk = global.ProbojPobracJezykZLinkaRequestu(url);
            Assert.True(jezyk == slownikJezykowSymbolach["en"]);
        }

   List<TestowyRouting> listaTestowychLinkow = new List<TestowyRouting>
       {
           new TestowyRouting("/8090/pk100787", "", "",new Dictionary<string, string> { {"katID", "100787" } }),
           new TestowyRouting("~/napoje/pk221", "", "",new Dictionary<string, string> { {"katID", "221" } }),
           new TestowyRouting("~/napoje/pk221", "", "",new Dictionary<string, string> { {"katID", "221" } }),
           new TestowyRouting("/dfgddgd-gd-wer-er-pk-sd4/pkfdfpk34sd", "", "", null),
           new TestowyRouting("/superFajnyLink/46s", "", "", new Dictionary<string, string> { {"katID", "0" } }),
           new TestowyRouting("/alce-nero-woskie-produkty/pk100096", "", "", new Dictionary<string, string> { {"katID", "100096" } } )
       };

        [Fact(DisplayName = "czy podane linki sa stronami produktow")]
        public void AktualnaStronaToStronaProduktowTest()
        {
            konfiguracjaDlaJednegoJezyka();
            //System.Web.Routing.RouteCollection routeCollection = System.Web.Routing.RouteTable.Routes;

            var kontrolerTest = this.GetController<TresciController>(listaTestowychLinkow.First().urlTestowy);

            Assert.True(kontrolerTest.Calosc.Konfiguracja.LinkAlternatywnyStronyProduktow == "superFajnyLink/46s", "Testy operaja sie o ustawienie LinkAlternatywnyStronyProduktow, ale jest zła wartość =" + kontrolerTest.Calosc.Konfiguracja.LinkAlternatywnyStronyProduktow);

            foreach (TestowyRouting test in listaTestowychLinkow)
            {
                kontrolerTest = this.GetController<TresciController>(test.urlTestowy);
                RouteData routeData = RouteTable.Routes.GetRouteData(kontrolerTest.HttpContext);
                if (routeData.Values.ContainsKey("MS_DirectRouteMatches"))
                {
                    routeData = ((IEnumerable<RouteData>)routeData.Values["MS_DirectRouteMatches"]).First();
                }

                long katID = 0;
                bool wynik = UrlExtender.AktualnaStronaToStronaProduktow(kontrolerTest.Url, out katID, routeData.Values);

                if (test.parametry == null)
                {
                    Assert.False(wynik);
                }
                else
                {
                    Assert.True(wynik, $"Dla URL: {test.urlTestowy} wyrabny został kontroler: {routeData.Values["controller"]} i akcja {routeData.Values["action"]}");
                    Assert.True(katID == long.Parse(test.parametry["katID"]), test.urlTestowy);
                }
            }
        }

    }
}
