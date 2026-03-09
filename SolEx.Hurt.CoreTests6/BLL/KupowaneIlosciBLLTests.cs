using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FakeItEasy;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using Xunit;
namespace SolEx.Hurt.Core.BLL.Tests
{
    public class KupowaneIlosciBLLTests
    {
        private List<KupowaneIlosci> WygenerujKupowaneIlosci(int ile)
        {
            List<KupowaneIlosci> kupowaneIlosci = new List<KupowaneIlosci>();

            for (int i = 0; i < ile; i++)
            {
                KupowaneIlosci k = new  KupowaneIlosci();
                k.KlientId = i;
                k.ProduktId = i;
                k.RodzajDokumentu = RodzajDokumentu.Faktura;
                k.Od = DateTime.Now.AddDays(-10).Date;
                k.Do = DateTime.Now.Date;
             
                kupowaneIlosci.Add(k);
            }

            return kupowaneIlosci;
        }

        [Fact()]
        public void ZnajdzPasujacyTestSprawdzajacy()
        {
            KupowaneIlosciBLL kupowanie = A.Fake<KupowaneIlosciBLL>();

            Dictionary<string, KupowaneIlosci> slownik =
                WygenerujKupowaneIlosci(100).ToDictionary(x => x.Id, x => x);

            A.CallTo(() => kupowanie.PobierzLimity()).Returns(slownik);

            var kupowanieilosci = kupowanie.ZnajdzPasujacy(19, 19, RodzajDokumentu.Faktura, DateTime.Now.AddDays(-10).Date, DateTime.Now.Date);

       
            Assert.Equal(19, kupowanieilosci.KlientId);
            Assert.Equal(19, kupowanieilosci.ProduktId);
        }

        [Fact()]
        public void ZnajdzPasujacyTestWydajnosciowy()
        {
            KupowaneIlosciBLL kupowanie = A.Fake<KupowaneIlosciBLL>();

            Dictionary<string, KupowaneIlosci> slownik =
                WygenerujKupowaneIlosci(1000001).ToDictionary(x => x.Id, x => x);

            A.CallTo(() => kupowanie.PobierzLimity()).Returns(slownik);

            int ileIteracji = 10000;
            Stopwatch stoper = new Stopwatch();
            stoper.Start();
            for (int i = 0; i < ileIteracji; i++)
            {
                var kupowanieilosci = kupowanie.ZnajdzPasujacy(1000000, 1000000, RodzajDokumentu.Faktura,
                    DateTime.Now.AddDays(-10).Date, DateTime.Now.Date);
            }
            stoper.Stop();

            Assert.True(stoper.Elapsed.TotalSeconds/ileIteracji < 1);
        }

        private List<historia_dokumenty> WygenerujDokumenty(int ile)
        {
            List<historia_dokumenty> lista = new List<historia_dokumenty>();
            for (int i = 1; i <= ile; i++)
            {
                historia_dokumenty hd = new historia_dokumenty();
                hd.DataWyslaniaDokumentu = DateTime.Now.AddDays(i);
                hd.data_dodania = DateTime.Now.AddDays(i);
                hd.data_utworzenia = DateTime.Now.AddDays(i);
                hd.NumerObcy = i+"ASDASD";
                hd.id = i;
                hd.klient_id = i%2 == 0 ? i : 3;
                hd.Rodzaj = i%2 == 0 ? RodzajDokumentu.Faktura : RodzajDokumentu.Zamowienie;

                lista.Add(hd);
            }
            return lista;
        }

        private Dictionary<int, List<historia_dokumenty_produkty>> WygenerujProdukty(int iledokumentow, int ileproduktowNaDokument)
        {
            Dictionary<int, List<historia_dokumenty_produkty>> lista = new Dictionary<int, List<historia_dokumenty_produkty>>();
            for (int i = 1; i <= iledokumentow; i++)
            {
                List<historia_dokumenty_produkty> listaProduktow = new List<historia_dokumenty_produkty>(ileproduktowNaDokument);

                for (int p = 1; p <= ileproduktowNaDokument; p++)
                {
                    historia_dokumenty_produkty hpd = new historia_dokumenty_produkty();
                    hpd.CenaBruttoPoRabacie = p;
                    hpd.CenaNettoPoRabacie = p;
                    hpd.dokument_id = i;
                    hpd.ilosc = (i*2) + p;
                    hpd.produkt_id = p;
                    listaProduktow.Add(hpd);
                }
                lista.Add(i, listaProduktow);
            }
            return lista;
        }


        [Fact()]
        public void WyliczKupowanaIloscTestSprwadzajacy()
        {
            //KupowaneIlosciBLL kupowanie = A.Fake<KupowaneIlosciBLL>();

            //int ileDokumentow = 10;
            //int ileProduktownadokument = 5;
            //int idproduktu = 3;

            //var dokumenty = WygenerujDokumenty(ileDokumentow);
            //var produktydokumenty = WygenerujProdukty(ileDokumentow, ileProduktownadokument);

            //decimal ile = kupowanie.WyliczKupowanaIlosc(dokumenty, produktydokumenty, idproduktu, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(10));
            //int wyliczoneIle = wyliczIle(ileDokumentow, idproduktu);
            //Assert.Equal(wyliczoneIle, ile);
        }

        public int wyliczIle(int iledokumentow, int idProduktu)
        {
            int ile = 0;
            for (int i = 1; i <= iledokumentow; i++)
                ile += i*2 + idProduktu;

            return ile;
        }

        [Fact()]
        public void WyliczKupowanaIloscTestWydajnosciowy()
        {
        //    KupowaneIlosciBLL kupowanie = A.Fake<KupowaneIlosciBLL>();

        //    int ileDokumentow = 10000;
        //    int ileProduktownadokument = 200;
        //    int idproduktu = 199;

        //    var dokumenty = WygenerujDokumenty(ileDokumentow);
        //    var produktydokumenty = WygenerujProdukty(ileDokumentow, ileProduktownadokument);

        //    int ileIteracji = 200;
        //    Stopwatch stoper = Stopwatch.StartNew();
        //    for (int i = 0; i < ileIteracji; i++)
        //    {
        //        decimal ile = kupowanie.WyliczKupowanaIlosc(dokumenty, produktydokumenty, idproduktu,
        //            DateTime.Now.AddDays(-1), DateTime.Now.AddDays(10));
        //    }
        //    stoper.Stop();

        //    Assert.True(stoper.Elapsed.TotalMilliseconds / ileIteracji < 1);
        }

        [Fact(DisplayName = "Pobieranie kupionych ilości z dokumentów")]
        public void PobierzKupowanaIloscTest()
        {
            IConfigBLL config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.JezykIDPolski).Returns(1);
            A.CallTo(() => config.GetLicense(Licencje.katalog_klienta)).Returns(true);
            KupowaneIlosciBLL ki = A.Fake<KupowaneIlosciBLL>();
            ki.Configbll = config;

            IKlienciDostep kliencidostep = A.Fake<IKlienciDostep>();
            int idklienta = 2;
            IKlient klient1 = A.Fake<IKlient>();
            A.CallTo(() => klient1.klient_id).Returns(idklienta);


            A.CallTo(() => kliencidostep.Pobierz(idklienta)).Returns(klient1);

            IProduktyKlienta pu = A.Fake<IProduktyKlienta>();

            A.CallTo(() => ki.ProduktyKlienta).Returns(pu);
    
            ki.Configbll = config;
         
            
            //IDokumentPozycja dok1pozycja1 = A.Fake<IDokumentPozycja>();
            //A.CallTo(() => dok1pozycja1.PozycjaDokumentuIdProduktu).Returns(101);
            //A.CallTo(() => dok1pozycja1.PozycjaDokumentuIlosc).Returns(10);
            
            //IDokumentPozycja dok1pozycja2 = A.Fake<IDokumentPozycja>();
            //A.CallTo(() => dok1pozycja2.PozycjaDokumentuIdProduktu).Returns(102);
            //A.CallTo(() => dok1pozycja2.PozycjaDokumentuIlosc).Returns(11);

            //IDokumentPozycja dok1pozycja3 = A.Fake<IDokumentPozycja>();
            //A.CallTo(() => dok1pozycja3.PozycjaDokumentuIdProduktu).Returns(103);
            //A.CallTo(() => dok1pozycja3.PozycjaDokumentuIlosc).Returns(12);
            
            //List<IDokumentPozycja> dok1pozycje = new List<IDokumentPozycja>(){ dok1pozycja1, dok1pozycja2, dok1pozycja3 };


            //IDokument dok1 = A.Fake<IDokument>();
            //A.CallTo(() => dok1.PobierzPozycjeDokumentu()).Returns(dok1pozycje);
            //A.CallTo(() => dok1.DokummentRodzaj).Returns(RodzajDokumentu.Faktura);
            //A.CallTo(() => dok1.DokumentOdbiorcaId).Returns(1);
            //A.CallTo(() => dok1.DokumentId).Returns(1);

            //IDokumentPozycja dok2pozycja1 = A.Fake<IDokumentPozycja>();
            //A.CallTo(() => dok2pozycja1.PozycjaDokumentuIdProduktu).Returns(102);
            //A.CallTo(() => dok2pozycja1.PozycjaDokumentuIlosc).Returns(69);

            //List<IDokumentPozycja> dok2pozycje = new List<IDokumentPozycja>() { dok2pozycja1 };


            //IDokument dok2 = A.Fake<IDokument>();
            //A.CallTo(() => dok1.PobierzPozycjeDokumentu()).Returns(dok2pozycje);
            //A.CallTo(() => dok1.DokummentRodzaj).Returns(RodzajDokumentu.Faktura);
            //A.CallTo(() => dok1.DokumentOdbiorcaId).Returns(1);
            //A.CallTo(() => dok1.DokumentId).Returns(2);

            Dictionary<int, IDokument> slownikdokumenty = new Dictionary<int, IDokument>();
            //slownikdokumenty.Add(1, dok1);
            //slownikdokumenty.Add(2, dok2);

            Dokumenty dokumenty = A.Fake<Dokumenty>();
            A.CallTo(() => dokumenty.PobierzDokumenty(RodzajDokumentu.Faktura, 1)).Returns(slownikdokumenty);
    
            IDokumentPozycja dok3pozycja1 = A.Fake<IDokumentPozycja>();
            A.CallTo(() => dok3pozycja1.PozycjaDokumentuIdProduktu).Returns(101);
            A.CallTo(() => dok3pozycja1.PozycjaDokumentuIlosc).Returns(10);

            IDokumentPozycja dok3pozycja2 = A.Fake<IDokumentPozycja>();
            A.CallTo(() => dok3pozycja2.PozycjaDokumentuIdProduktu).Returns(102);
            A.CallTo(() => dok3pozycja2.PozycjaDokumentuIlosc).Returns(11);

            IDokumentPozycja dok3pozycja3 = A.Fake<IDokumentPozycja>();
            A.CallTo(() => dok3pozycja3.PozycjaDokumentuIdProduktu).Returns(103);
            A.CallTo(() => dok3pozycja3.PozycjaDokumentuIlosc).Returns(99);

            List<IDokumentPozycja> dok3pozycje = new List<IDokumentPozycja>() { dok3pozycja1, dok3pozycja2, dok3pozycja3 };


            IDokument dok3 = A.Fake<IDokument>();
            A.CallTo(() => dok3.PobierzPozycjeDokumentu()).Returns(dok3pozycje);
            A.CallTo(() => dok3.DokummentRodzaj).Returns(RodzajDokumentu.Faktura);
            A.CallTo(() => dok3.DokumentOdbiorcaId).Returns(2);
            A.CallTo(() => dok3.DokumentId).Returns(3);
            A.CallTo(() => dok3.DokumentDataWystawienia).Returns(new DateTime(2012, 6, 10));

            IDokumentPozycja dok4pozycja1 = A.Fake<IDokumentPozycja>();
            A.CallTo(() => dok4pozycja1.PozycjaDokumentuIdProduktu).Returns(103);
            A.CallTo(() => dok4pozycja1.PozycjaDokumentuIlosc).Returns(1);

            IDokumentPozycja dok4pozycja2 = A.Fake<IDokumentPozycja>();
            A.CallTo(() => dok4pozycja2.PozycjaDokumentuIdProduktu).Returns(12);
            A.CallTo(() => dok4pozycja2.PozycjaDokumentuIlosc).Returns(113);


            List<IDokumentPozycja> dok4pozycje = new List<IDokumentPozycja>() { dok4pozycja1, dok4pozycja2 };

            IKlient klient = A.Fake<IKlient>();
            A.CallTo(() => klient.klient_id).Returns(2);
            IDokument dok4 = A.Fake<IDokument>();
            A.CallTo(() => dok4.PobierzPozycjeDokumentu()).Returns(dok4pozycje);
            A.CallTo(() => dok4.DokummentRodzaj).Returns(RodzajDokumentu.Faktura);
            A.CallTo(() => dok4.DokumentOdbiorcaId).Returns(2);
            A.CallTo(() => dok4.DokumentId).Returns(4);
            A.CallTo(() => dok4.DokumentDataWystawienia).Returns(new DateTime(2013,6,10));

            slownikdokumenty = new Dictionary<int, IDokument>();
            slownikdokumenty.Add(3, dok3);
            slownikdokumenty.Add(4, dok4);

            A.CallTo(() => dokumenty.PobierzDokumenty(RodzajDokumentu.Faktura, 2)).Returns(slownikdokumenty);

            Dictionary<string, KupowaneIlosci> slownik =
                WygenerujKupowaneIlosci(100).ToDictionary(x => x.Id, x => x);

            A.CallTo(() => ki.PobierzLimity()).Returns(slownik);
            A.CallTo(() => ki.PobierzDokumentyDoLiczenia(2, RodzajDokumentu.Faktura)).Returns(slownikdokumenty);
            //sprawdzamy wszystkie faktury klienta
            //zwraca 100 sztuk z 2 dokumentów dla proiduktu o id 103
            A.CallTo(() => pu.PobierzIdProduktowRzeczywiscieDostepnychDlaKlienta(klient)).Returns(new HashSet<int>() { 103 });

            decimal ilosc2 = ki.PobierzKupowanaIlosc(103, klient, RodzajDokumentu.Faktura);
            Assert.Equal(100, ilosc2);

            //sprawdzamy wszystkie faktury klienta od 2012-06-10
            decimal ilosc3 = ki.PobierzKupowanaIlosc(103, klient, RodzajDokumentu.Faktura, new DateTime(2012, 6, 10));
            Assert.Equal(100, ilosc3);

            //sprawdzamy ilość dla wszystkich faktur klienta od 2012-06-11
            //powinno zwrócić 1 bo tylko 1 pozycja dokumentu pasuje do takich danych
            decimal ilosc4 = ki.PobierzKupowanaIlosc(103, klient, RodzajDokumentu.Faktura, new DateTime(2012, 6, 11));
            Assert.Equal(1, ilosc4);

            //sprawdzamy wszystkie faktury klienta od 2012-06-10 do 2013-06-10
            //powinno zwrócić 100 sztuk z 2 dokumentów
            decimal ilosc5 = ki.PobierzKupowanaIlosc(103, klient, RodzajDokumentu.Faktura, new DateTime(2012, 6, 10), new DateTime(2013, 6, 10));
            Assert.Equal(100, ilosc5);

            //sprawdzamy wszystkie faktury klienta od 2013-06-10 do 2013-06-10
            //powinno zwrócić 1 sztukę z 1 dokumentu
            decimal ilosc6 = ki.PobierzKupowanaIlosc(103, klient, RodzajDokumentu.Faktura, new DateTime(2013, 6, 10), new DateTime(2013, 6, 10));
            Assert.Equal(1, ilosc6);

            //sprawdzamy wszystkie faktury klienta od 2013-06-10 do 2014-06-10
            //powinno zwrócić 1 sztukę z 1 dokumentu
            decimal ilosc7 = ki.PobierzKupowanaIlosc(103, klient, RodzajDokumentu.Faktura, new DateTime(2013, 6, 10), new DateTime(2014, 6, 10));
            Assert.Equal(1, ilosc7);

            //sprawdzamy wszystkie faktury klienta od 2012-06-01 do 2012-06-09
            //powinno zwrócić 0 sztuk
            decimal ilosc8 = ki.PobierzKupowanaIlosc(103, klient, RodzajDokumentu.Faktura, new DateTime(2012, 6, 1), new DateTime(2012, 6, 9));
            Assert.Equal(0, ilosc8);

            //sprawdzamy wszystkie faktury klienta od 2013-06-11 do 2014-06-09
            //powinno zwrócić 0 sztuk
            decimal ilosc9 = ki.PobierzKupowanaIlosc(103, klient, RodzajDokumentu.Faktura, new DateTime(2013, 6, 11), new DateTime(2014, 6, 9));
            Assert.Equal(0, ilosc9);
            
        }

        //[Fact()]
        //public void PrzeliczIlosciKlientaTestSprawdzajacy()
        //{
        //    KupowaneIlosciBLL kupowanie = A.Fake<KupowaneIlosciBLL>();

          
        //    int ileDokumentow = 10;
        //    int ileProduktownadokument = 5;
        //    int idproduktu = 3;
        //    int idKlienta = 2;



        //    RabatyBll rabaty = A.Fake<RabatyBll>();
        //    A.CallTo(() => rabaty.Odswierz(null, idKlienta));
        //    kupowanie.Rabaty = rabaty;

        //    HashSet<int> idproduktow = new HashSet<int>();
        //    for (int i = 1; i <= ileProduktownadokument; i++)
        //        idproduktow.Add(i);
        //    A.CallTo(() => kupowanie.PobierzIdProduktow()).Returns(idproduktow);



        //    var dokumenty = WygenerujDokumenty(ileDokumentow);
        //    var produktydokumenty = WygenerujProdukty(ileDokumentow, ileProduktownadokument);

        //    var dokumentyKlienta = dokumenty.Where(a => a.klient_id == idKlienta && a.Rodzaj == RodzajDokumentu.Faktura).ToList();
        //    var produktyKlientaLista =
        //        produktydokumenty.Select(a => a.Value.Where(b => dokumentyKlienta.Any(c => c.id == b.dokument_id)))
        //            .ToList();
        //    Dictionary<int, List<historia_dokumenty_produkty>> produktyKlienta = new Dictionary<int, List<historia_dokumenty_produkty>>();
        //    foreach (var pk in produktyKlientaLista)
        //    {
        //        foreach (var historia_Dokumenty_Produkty in pk)
        //        {
        //            if(!produktyKlienta.ContainsKey(historia_Dokumenty_Produkty.dokument_id))
        //                produktyKlienta.Add(historia_Dokumenty_Produkty.dokument_id, new List<historia_dokumenty_produkty>(){ historia_Dokumenty_Produkty});    

        //            else produktyKlienta[historia_Dokumenty_Produkty.dokument_id].Add(historia_Dokumenty_Produkty);
        //        }
                
        //    }

        //    Dictionary<int, List<historia_dokumenty_produkty>> ilosci;
        //    A.CallTo(() => kupowanie.PobierzDokumentyDoLiczenia(idKlienta, RodzajDokumentu.Faktura, out ilosci))
        //        .Returns(dokumentyKlienta).AssignsOutAndRefParameters(produktyKlienta);
            
                    

        //    var przeliczoneIlosci = kupowanie.PrzeliczIlosciKlienta(idKlienta, RodzajDokumentu.Faktura, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(10), new Dictionary<int, decimal>(), true);
        //    foreach (var kupowaneIlosci in przeliczoneIlosci)
        //    {
        //        Assert.Equal(idKlienta, kupowaneIlosci.KlientId);
        //        Assert.Equal(kupowaneIlosci.ProduktId+4, kupowaneIlosci.WyliczonaIlosc);
        //    }
        //}

        //[Fact()]
        //public void PrzeliczIlosciKlientaTestWydajnosciowy()
        //{
        //    KupowaneIlosciBLL kupowanie = A.Fake<KupowaneIlosciBLL>();


        //    int ileDokumentow = 10;
        //    int ileProduktownadokument = 5;
        //    int idproduktu = 3;
        //    int idKlienta = 2;



        //    RabatyBll rabaty = A.Fake<RabatyBll>();
        //    A.CallTo(() => rabaty.Odswierz(null, idKlienta));
        //    kupowanie.Rabaty = rabaty;

        //    HashSet<int> idproduktow = new HashSet<int>();
        //    for (int i = 1; i <= ileProduktownadokument; i++)
        //        idproduktow.Add(i);
        //    A.CallTo(() => kupowanie.PobierzIdProduktow()).Returns(idproduktow);



        //    var dokumenty = WygenerujDokumenty(ileDokumentow);
        //    var produktydokumenty = WygenerujProdukty(ileDokumentow, ileProduktownadokument);

        //    var dokumentyKlienta = dokumenty.Where(a => a.klient_id == idKlienta && a.Rodzaj == RodzajDokumentu.Faktura).ToList();
        //    var produktyKlientaLista =
        //        produktydokumenty.Select(a => a.Value.Where(b => dokumentyKlienta.Any(c => c.id == b.dokument_id)))
        //            .ToList();
        //    Dictionary<int, List<historia_dokumenty_produkty>> produktyKlienta = new Dictionary<int, List<historia_dokumenty_produkty>>();
        //    foreach (var pk in produktyKlientaLista)
        //    {
        //        foreach (var historia_Dokumenty_Produkty in pk)
        //        {
        //            if (!produktyKlienta.ContainsKey(historia_Dokumenty_Produkty.dokument_id))
        //                produktyKlienta.Add(historia_Dokumenty_Produkty.dokument_id, new List<historia_dokumenty_produkty>() { historia_Dokumenty_Produkty });

        //            else produktyKlienta[historia_Dokumenty_Produkty.dokument_id].Add(historia_Dokumenty_Produkty);
        //        }

        //    }

        //    Dictionary<int, List<historia_dokumenty_produkty>> ilosci;
        //    A.CallTo(() => kupowanie.PobierzDokumentyDoLiczenia(idKlienta, RodzajDokumentu.Faktura, out ilosci))
        //        .Returns(dokumentyKlienta).AssignsOutAndRefParameters(produktyKlienta);

        //    int ileIteracji = 10000;
        //    Stopwatch stoper = Stopwatch.StartNew();
        //    for (int i = 0; i < ileIteracji; i++)
        //    {
        //        var przeliczoneIlosci = kupowanie.PrzeliczIlosciKlienta(idKlienta, RodzajDokumentu.Faktura,
        //            DateTime.Now.AddDays(-1), DateTime.Now.AddDays(10), new Dictionary<int, decimal>(), true);

        //    }
        //    stoper.Stop();

        //    Assert.True(stoper.Elapsed.TotalMilliseconds/ileIteracji < 0.7);

        //}
    }
}
