using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Core.BLL.WirtualneProduktyProvidery;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces.Sync;
using Xunit;
using Klient = SolEx.Hurt.Core.Klient;

namespace SolEx.Hurt.CoreTests.BLL
{

   
    public class SmmashTests
    {
        [Fact(DisplayName = "Poprawne wyciaganie plikow")]
        //Do prawidłowego działania testu trzeba stworzyć katalog C:\Smmash\Adamusik\RSO2\projekty-graficzne
        public void SynchronizatorPrzetwarzajWirtualneProduktySmmashTest()
        {
            Smmash sm = new Smmash();

            Model.Klient k1 = A.Fake<Model.Klient>();
            k1.Symbol = "ADAMUSIK";
            k1.Id = 1;

            Model.Klient k2 = A.Fake<Model.Klient>();
            k2.Symbol = "ALPHA-TEAM";
            k2.Id = 2;

            Atrybut atrybut = new Atrybut() {Id=1, Nazwa = "NAZWA FOLDERU PROJEKTU" };

            Cecha c1= new Cecha()  { Symbol = "nazwa folderu projektu:shc2/projekty-graficzne", AtrybutId = 1,Nazwa = "SHC2/projekty-graficzne", Id = 1 };
            Cecha c2 = new Cecha() { Symbol = "nazwa folderu projektu:rso2/projekty-graficzne", AtrybutId = 1, Nazwa = "RSO2/projekty-graficzne", Id = 2};
            Cecha c3 = new Cecha() { Symbol = "nazwa folderu projektu:rso3/projekty-graficzne", AtrybutId = 1, Nazwa = "RSO3/projekty-graficzne", Id = 3 };
            Cecha c4 = new Cecha() { Symbol = "nazwa folderu projektu:rso5/projekty-graficzne", AtrybutId = 1, Nazwa = "RSO5/projekty-graficzne", Id = 4 };



            Produkt p1 = new Produkt(1) {Rodzina = "Testowa rodzina 1", Kod = "SHC2XL" };
            Produkt p2 = new Produkt(2) { Rodzina = "Testowa rodzina 2", Kod = "RSO2XL" };
            Produkt p3 = new Produkt(3) {Kod= "RSO3S" };

            List<Produkt>listaProduktow = new List<Produkt>() {p1,p2,p3};

            ProduktCecha pc1 = new ProduktCecha(1,1);
            ProduktCecha pc2 = new ProduktCecha(2,2);
            ProduktCecha pc3 = new ProduktCecha(3,3);
            ProduktCecha pc4 = new ProduktCecha(1,4);

            List<Tlumaczenie> tlumaczenia=new List<Tlumaczenie>();
            List<JednostkaProduktu>jed=new List<JednostkaProduktu>();
            Dictionary<long, ProduktCecha> produktyCechyErp = new Dictionary<long, ProduktCecha>();
            produktyCechyErp.Add(pc1.Id,pc1);
            produktyCechyErp.Add(pc2.Id, pc2);
            produktyCechyErp.Add(pc3.Id, pc3);
            produktyCechyErp.Add(pc4.Id, pc4);
            List<ProduktKategoria> pkzErp = new List<ProduktKategoria>();
            ISyncProvider aktualnyProvider = null;
            List<ProduktUkryty> produktuUkryteErp = new List<ProduktUkryty>();
            List<ProduktyZamienniki> zamiennikierp = new List<ProduktyZamienniki>();
            List<Cecha> cechytmp=new List<Cecha>() {c1,c2,c3,c4};
            List<Atrybut> atrybuty=new List<Atrybut>() {atrybut};
            Dictionary<long, Model.Klient>klienci = new Dictionary<long, Model.Klient>();
            klienci.Add(k1.Id,k1);
            klienci.Add(k2.Id, k2);
            sm.SciezkaFolderNaDysku = "C:\\Smmash";
            sm.SynchronizatorPrzetwarzajWirtualneProdukty(ref listaProduktow,ref tlumaczenia, new Dictionary<long, Produkt>(),ref jed, ref produktyCechyErp, ref pkzErp, aktualnyProvider,
                ref produktuUkryteErp,ref zamiennikierp,new Dictionary<long, KategoriaProduktu>(), ref cechytmp,ref atrybuty,ref klienci );

            Assert.True(klienci.Count==2);
            Assert.False(string.IsNullOrEmpty(klienci.First().Value.PoleTekst5));
            Assert.True(string.IsNullOrEmpty(klienci.Last().Value.PoleTekst5));
        }



        [Fact(DisplayName = "Poprawne wyciaganie banerow")]
        //Potrzebny katalog (C:\Smmash\JUDO-CLUB-WETZLAR\Baner) z plikiem baner#250x100_visual1.png
        public void SynchronizatorPrzetwarzajWirtualneProduktySmmashTest2()
        {
            Smmash sm = new Smmash();
            
            Model.Klient k1 = A.Fake<Model.Klient>();
            k1.Symbol = "JUDO-CLUB-WETZLAR";
            k1.Id = 1;

            Atrybut atrybut = new Atrybut() { Id = 1, Nazwa = "NAZWA FOLDERU PROJEKTU" };

            Cecha c1 = new Cecha() { Symbol = "nazwa folderu projektu:Baner", AtrybutId = 1, Nazwa = "Baner", Id = 1 };


            Produkt p1 = new Produkt(1) { Kod = "Baner" };
            Produkt p2 = new Produkt(2) { Rodzina = "Testowa rodzina 2", Kod = "Baner TEst" };

            Dictionary<string,Dictionary<string,string>>wynik = new Dictionary<string, Dictionary<string, string>>();
            wynik.Add("1",new Dictionary<string, string>());
            wynik["1"].Add("visual1", "250x100");

            List<Produkt> listaProduktow = new List<Produkt>() { p1, p2 };

            ProduktCecha pc1 = new ProduktCecha(1, 1);

            List<Tlumaczenie> tlumaczenia = new List<Tlumaczenie>();
            List<JednostkaProduktu> jed = new List<JednostkaProduktu>();
            Dictionary<long, ProduktCecha> produktyCechyErp = new Dictionary<long, ProduktCecha>();
            produktyCechyErp.Add(pc1.Id, pc1);
            List<ProduktKategoria> pkzErp = new List<ProduktKategoria>();
            ISyncProvider aktualnyProvider = null;
            List<ProduktUkryty> produktuUkryteErp = new List<ProduktUkryty>();
            List<ProduktyZamienniki> zamiennikierp = new List<ProduktyZamienniki>();
            List<Cecha> cechytmp = new List<Cecha>() { c1 };
            List<Atrybut> atrybuty = new List<Atrybut>() { atrybut };
            Dictionary<long, Model.Klient> klienci = new Dictionary<long, Model.Klient>();
            klienci.Add(k1.Id, k1);
            sm.SciezkaFolderNaDysku = "C:\\Smmash";
            sm.SynchronizatorPrzetwarzajWirtualneProdukty(ref listaProduktow, ref tlumaczenia, new Dictionary<long, Produkt>(), ref jed, ref produktyCechyErp, ref pkzErp, aktualnyProvider,
                ref produktuUkryteErp, ref zamiennikierp, new Dictionary<long, KategoriaProduktu>(), ref cechytmp, ref atrybuty, ref klienci);

            Assert.True(klienci.Count == 1);

            var test = JSonHelper.Deserialize<Dictionary<string, Dictionary<string, string>>>(klienci.First().Value.PoleTekst4);
            Assert.True(test.Keys.First()=="1");
            Assert.True(test["1"].Keys.First() == "visual1");
            Assert.True(test["1"]["visual1"] == "250x100");

            Assert.True(klienci.First().Value.PoleTekst4 == JSonHelper.Serialize(wynik));
        }

    }
}
