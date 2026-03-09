using System.Collections.Generic;
using System.Linq;
using System.Xml;
using FakeItEasy;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Zamowienia;
using Xunit;

namespace SolEx.Hurt.Sync.App.Modules_Tests.NoweModuly.Zamowienia
{
    public class ComarchConnectorTests
    {
        private string zamowienieXML1numer = "Zamowienie t5";
        private string zamowienieXML1data = "2014-06-06";
        private string zamowienieXML1ilnklienta = "CHLEMAR 2_B2B";
        private string zamowienieXML1produkt1ean = "5901885212562";
        private string zamowienieXML1produkt2ean = "5901885212883";

        private int zamowienieXML1produkt1ilosc = 14;
        private int zamowienieXML1produkt2ilosc = 69;

        private decimal zamowienieXML1produkt1cena = 23.45M;
        private decimal zamowienieXML1produkt2cena = 10.99M;

        private string zamowienieXML1produkt1jednostka = "PCE";
        private string zamowienieXML1produkt2jednostka = "PCE";

        private string drugiNumerILN = "INNYSYMBOL";

        private string zamowienieXML = "<?xml version='1.0' encoding='UTF-8'?>" +
                                       "<Document-Order>" +
                                       "<Order-Header><OrderNumber>528100184</OrderNumber><OrderDate>2015-10-08</OrderDate><ExpectedDeliveryDate>2015-10-12</ExpectedDeliveryDate><DocumentFunctionCode>O</DocumentFunctionCode></Order-Header>" +
                                       "<Order-Parties><Buyer><ILN>5900012299001</ILN></Buyer><Seller><ILN>5907814660008</ILN><CodeByBuyer>34273</CodeByBuyer></Seller><DeliveryPoint><ILN>5900012299278</ILN></DeliveryPoint><ShipFrom /></Order-Parties>" +
                                       "<Order-Lines><Line><Line-Item><LineNumber>1</LineNumber><EAN>5907464587038</EAN><BuyerItemCode>89666</BuyerItemCode><SupplierItemCode></SupplierItemCode>" +
                                       "<ItemDescription><![CDATA[FL BIO DAKTYLE B/PESTEK 150G]]></ItemDescription><OrderedQuantity>12</OrderedQuantity><UnitOfMeasure>PCE</UnitOfMeasure></Line-Item></Line>" +
                                       "<Line><Line-Item><LineNumber>2</LineNumber><EAN>5907464587083</EAN><BuyerItemCode>89744</BuyerItemCode><SupplierItemCode></SupplierItemCode>" +
                                       "<ItemDescription><![CDATA[FL BIO SLON.LUSK. 250G]]></ItemDescription><OrderedQuantity>36</OrderedQuantity><UnitOfMeasure>PCE</UnitOfMeasure></Line-Item></Line>" +
                                       "<Line><Line-Item><LineNumber>3</LineNumber><EAN>5907464587144</EAN><BuyerItemCode>89719</BuyerItemCode><SupplierItemCode></SupplierItemCode>" +
                                       "<ItemDescription><![CDATA[FL BIO PESTKI DYNI 150G]]></ItemDescription><OrderedQuantity>24</OrderedQuantity><UnitOfMeasure>PCE</UnitOfMeasure></Line-Item></Line></Order-Lines>" +
                                       "<Order-Summary><TotalLines>3</TotalLines><TotalOrderedAmount>72</TotalOrderedAmount></Order-Summary></Document-Order>";

        public string WypelnijXmlZzamowieniem(bool dodajDrugiNumerIln = true)
        {
            return zamowienieXML;
        }

        [Fact(DisplayName = "Weryfikacja Poprawności Wczytania Zamówienia z Xml")]
        public void WeryfikacjaPoprawnosciWczytaniaZamowieniaXmlDoObiektu()
        {
            string plik1 = "C:\\test1.xml";
            List<string> listaPlikow = new List<string>() { plik1 };

            ComarchConnector cc = A.Fake<ComarchConnector>();

            A.CallTo(() => cc.WyszukajPlikDoWczytania()).Returns(listaPlikow);
            string zamowienieXML1 = WypelnijXmlZzamowieniem();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(zamowienieXML1);

            A.CallTo(() => cc.PobierzDokument(plik1)).Returns(doc);

            Produkt p1 = new Produkt { Id = 1, KodKreskowy = "5907464587038" };
            Produkt p2 = new Produkt { KodKreskowy = "5907464587083", Id = 2 };
            Produkt p3 = new Produkt { KodKreskowy = "5907464587144", Id = 3 };
            Dictionary<string, Produkt> produktywgkodukreskowego = new Dictionary<string, Produkt> { { p1.KodKreskowy, p1 }, { p2.KodKreskowy, p2 }, { p3.KodKreskowy, p3 } };
            A.CallTo(() => cc.ProduktyWgKoduKreskowego).Returns(produktywgkodukreskowego);

            Jednostka j1 = new Jednostka { Id = 1, Nazwa = "PCE" };
            Jednostka j2 = new Jednostka { Id = 2, Nazwa = "PCE" };
            Dictionary<long, Jednostka> jednostki = new Dictionary<long, Jednostka> { { j1.Id, j1 }, { j2.Id, j2 } };

            ProduktJednostka pj1 = new ProduktJednostka { Podstawowa = true, ProduktId = 3242342, JednostkaId = 1 };
            ProduktJednostka pj2 = new ProduktJednostka { Podstawowa = true, ProduktId = p1.Id, JednostkaId = pj1.JednostkaId };
            ProduktJednostka pj3 = new ProduktJednostka { Podstawowa = true, ProduktId = p2.Id, JednostkaId = pj1.JednostkaId };
            ProduktJednostka pj4 = new ProduktJednostka { Podstawowa = true, ProduktId = p3.Id, JednostkaId = pj1.JednostkaId };
            Dictionary<long, ProduktJednostka> produktyjednostki = new Dictionary<long, ProduktJednostka> { { pj1.Id, pj1 }, { pj2.Id, pj2 }, { pj3.Id, pj3 }, { pj4.Id, pj4 } };

            ConfigBLL config = A.Fake<ConfigBLL>();

            StatusZamowienia z1 = new StatusZamowienia { Id = 1, Importowac = true };
            Dictionary<int, StatusZamowienia> lista = new Dictionary<int, StatusZamowienia> { { z1.Id, z1 } };
            A.CallTo(() => config.StatusyZamowien).Returns(lista);

            cc.ConfigBll = config;

            Klient klient = new Klient { Id = 1, Symbol = "5900012299278" };
            Dictionary<long, Klient> slownikKlienci = new Dictionary<long, Klient> { { klient.Id, klient } };
            A.CallTo(() => cc.Klienci).Returns(slownikKlienci);

            var api = A.Fake<IAPIWywolania>();
            FlatCeny fc1 = new FlatCeny() { KlientId = 1, ProduktId = 1, CenaNetto = 12 };
            FlatCeny fc2 = new FlatCeny() { KlientId = 2, ProduktId = 2, CenaNetto = 55 };
            List<FlatCeny> ceny = new List<FlatCeny>() { fc1, fc2 };
            A.CallTo(() => api.PobierzCenyKlientow(A<HashSet<long>>.Ignored)).Returns(ceny);

            cc.PoleIln = "Symbol";
            cc.ApiWywolanie = api;
            ZamowieniaImport zamowieniezXML = null;
            zamowieniezXML = cc.WczytajZPlikuMain(plik1, jednostki, produktyjednostki);

            Assert.Equal(klient.Id, zamowieniezXML.KlientId);

            Assert.Equal("528100184", zamowieniezXML.NumerTymczasowyZamowienia);
            //    Assert.Equal(zamowienieXML1data, zamowieniezXML.data_utworzenia.Value.ToShortDateString());
            Assert.Equal(3, zamowieniezXML.pozycje.Count);

            Assert.Equal(12, zamowieniezXML.pozycje[0].CenaNetto);
            Assert.Equal(55, zamowieniezXML.pozycje[1].CenaNetto);

            Assert.Equal(12, zamowieniezXML.pozycje[0].Ilosc);
            Assert.Equal(36, zamowieniezXML.pozycje[1].Ilosc);
            Assert.Equal(24, zamowieniezXML.pozycje[2].Ilosc);

            Assert.Equal(j1.Nazwa, zamowieniezXML.pozycje[0].Jednostka);
            Assert.Equal(j2.Nazwa, zamowieniezXML.pozycje[1].Jednostka);
        }

        [Fact(DisplayName = "Filtrowanie niepotrzebnych plików z listy")]
        public void PrzefiltrujListePlikowTest()
        {
            string katalog = "\\\\Bioartsrv\\edi\\test";

            string plik1 = "\\\\Bioartsrv\\edi\\test\\IN_ORDER_298482\\archiwum\\2014-07\\2014-07-02-09-31-10-270_5909000787384_{fc76b829-0a0a-0a0a-0a0a-0a0a0a0a0a0a}.xml";
            string plik2 = "\\\\Bioartsrv\\edi\\test\\IN_ORDER_298482\\buffer\\2014-04\\2014-07-02-01-31-10-270_5909000787384_{fc76b829-0a0a-0a0a-0a0a-0a0a0a0a0a0a}.xml";
            string plik3 = "\\\\Bioartsrv\\edi\\test\\IN_ORDER_298482\\2014-07-02-09-31-10-270_5909000787384_{97c2b829-0a0a-0a0a-0a0a-0a0a0a0a0a0a}.xml";
            string plik4 = "\\\\Bioartsrv\\edi\\test\\IN_ORDER_298482\\2014-07-02-09-31-10-270_5909000787384_{fc76b829-0a0a-0a0a-0a0a-0a0a0a0a0a0a}.xml";
            string plik5 = "\\\\Bioartsrv\\edi\\test\\STARE\\2014-07-02-09-31-10-270_5909000787384_{fc76b829-0a0a-0a0a-0a0a-0a0a0a0a0a0a}.xml";
            string plik6 = "\\\\Bioartsrv\\edi\\test\\STARE\\2014-07-02-09-31-10-270_5909000787384_{97c2b829-0a0a-0a0a-0a0a-0a0a0a0a0a0a}.xml";
            List<string> listaPlikow = new List<string>() { plik1, plik2, plik3, plik4, plik5, plik6 };

            ComarchConnector cc = A.Fake<ComarchConnector>();

            cc.KatalogZamowien = katalog;
            var przefiltrowane = cc.PrzefiltrujListePlikow(listaPlikow).ToList();

            Assert.Equal(2, przefiltrowane.Count);
            Assert.Equal(plik3, przefiltrowane[0]);
            Assert.Equal(plik4, przefiltrowane[1]);
        }
    }
}