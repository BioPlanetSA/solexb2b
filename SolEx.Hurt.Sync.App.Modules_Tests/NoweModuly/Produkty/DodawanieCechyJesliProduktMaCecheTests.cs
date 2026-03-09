using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using SolEx.Hurt.Model;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty.Tests
{
    public class DodawanieCechyJesliProduktMaCecheTests
    {
        [Fact(DisplayName = "Moduł DodawanieCechyJesliProduktMaCeche")]
        public void PrzetworzTest()
        {
            Test1();
            Test2();
            Test3();
            Test4();
        }
        private void Test4()
        {
            List<ProduktyZamienniki> TODO = new List<ProduktyZamienniki>();
            Produkt produkt = new Produkt();
            produkt.Id = 1;
            produkt.Kod = "S0784450";

            List<Produkt> listaproduktow = new List<Produkt>() { produkt };
            Cecha nowacecha = new Cecha();
            nowacecha.Nazwa = "40K028X";
            nowacecha.Symbol = "nowa:40K028X";
            nowacecha.Id = 100;
            Cecha wymagana = new Cecha();
            wymagana.Nazwa = "40K028X";
            wymagana.Symbol = "wymagana:40K028X";
            wymagana.Id = 200;
            Dictionary<long, Cecha> listaCech = new List<Cecha>() { nowacecha, wymagana }.ToDictionary(x => x.Id, x => x);

            ProduktCecha cechyProdukty = new ProduktCecha();
            cechyProdukty.ProduktId = produkt.Id;
            cechyProdukty.CechaId = wymagana.Id;
            ProduktCecha cechyProdukty2 = new ProduktCecha();

            cechyProdukty2.ProduktId = produkt.Id;
            cechyProdukty2.CechaId = nowacecha.Id;
            Dictionary<long, ProduktCecha>  lacznikicech = new Dictionary<long, ProduktCecha>() { {cechyProdukty2.Id, cechyProdukty2} };


            List<Tlumaczenie> slowniki = new List<Tlumaczenie>(0);
            List<JednostkaProduktu> jednostki = new List<JednostkaProduktu>(0);
           List< ProduktKategoria> lacznikikategorii = new List< ProduktKategoria>(0);
            List<ProduktUkryty> produktyukryte = new List<ProduktUkryty>(0);
            var modul = A.Fake<DodawanieCechyJesliProduktMaCeche>();
               List<Cecha> cechy = new List<Cecha>();
            List<Atrybut> atrybuty = new List<Atrybut>();
           
            A.CallTo(() => modul.CechyNaB2B).Returns(listaCech);
            modul.CechaWymagana = wymagana.Symbol;
            modul.CechaDodawanej = nowacecha.Symbol;
            modul.Przetworz(ref listaproduktow, ref slowniki, new Dictionary<long, Produkt>(), ref jednostki, ref lacznikicech, ref lacznikikategorii, null, ref produktyukryte, ref TODO, new Dictionary<long, KategoriaProduktu>() ,ref cechy,ref atrybuty);

            bool wynik = lacznikicech.Count == 1 && lacznikicech.Any(x => x.Value.CechaId == nowacecha.Id && x.Value.ProduktId == produkt.Id);
            Assert.True(wynik, " produkt nie mna cechy wymaganej, ma nową cechę");
        }
        private void Test3()
        {
            List<ProduktyZamienniki> TODO = new List<ProduktyZamienniki>();
            Produkt produkt = new Produkt();
            produkt.Id = 1;
            produkt.Kod = "S0784450";

            List<Produkt> listaproduktow = new List<Produkt>() { produkt };
            Cecha nowacecha = new Cecha();
            nowacecha.Nazwa = "40K028X";
            nowacecha.Symbol = "nowa:40K028X";
            nowacecha.Id = 100;
            Cecha wymagana = new Cecha();
            wymagana.Nazwa = "40K028X";
            wymagana.Symbol = "wymagana:40K028X";
            wymagana.Id = 200;
            Dictionary<long, Cecha> listaCech = new List<Cecha>() { nowacecha, wymagana }.ToDictionary(x => x.Id, x => x);

            ProduktCecha cechyProdukty = new ProduktCecha();
            cechyProdukty.ProduktId = produkt.Id;
            cechyProdukty.CechaId = wymagana.Id;
            ProduktCecha cechyProdukty2 = new ProduktCecha();
            cechyProdukty2.ProduktId = produkt.Id;
            cechyProdukty2.CechaId = nowacecha.Id;
            Dictionary<long,ProduktCecha> lacznikicech = new Dictionary<long, ProduktCecha>();


            List<Tlumaczenie> slowniki = new List<Tlumaczenie>(0);
            List<JednostkaProduktu> jednostki = new List<JednostkaProduktu>(0);
           List< ProduktKategoria> lacznikikategorii = new List< ProduktKategoria>(0);
            List<ProduktUkryty> produktyukryte = new List<ProduktUkryty>(0);
            var modul = A.Fake<DodawanieCechyJesliProduktMaCeche>();
               List<Cecha> cechy = new List<Cecha>();
            List<Atrybut> atrybuty = new List<Atrybut>();
           
            A.CallTo(() => modul.CechyNaB2B).Returns(listaCech);
            modul.CechaWymagana = wymagana.Symbol;
            modul.CechaDodawanej = nowacecha.Symbol;
            modul.Przetworz(ref listaproduktow, ref slowniki, new Dictionary<long, Produkt>(), ref jednostki, ref lacznikicech, ref lacznikikategorii, null, ref produktyukryte, ref TODO, new Dictionary<long, KategoriaProduktu>() ,ref cechy,ref atrybuty);

            bool wynik = lacznikicech.Count == 0;
            Assert.True(wynik, " produkt nie ma cechy wymaganej, nie ma nowej cechy");
        }

        private void Test1()
        {
            List<ProduktyZamienniki> TODO = new List<ProduktyZamienniki>();
            Produkt produkt = new Produkt();
            produkt.Id = 1;
            produkt.Kod = "S0784450";

            List<Produkt> listaproduktow = new List<Produkt>() { produkt };
            Cecha nowacecha = new Cecha();
            nowacecha.Nazwa = "40K028X";
            nowacecha.Symbol = "nowa:40K028X";
            nowacecha.Id = 100;
            Cecha wymagana = new Cecha();
            wymagana.Nazwa = "40K028X";
            wymagana.Symbol = "wymagana:40K028X";
            wymagana.Id = 200;
            Dictionary<long, Cecha> listaCech = new List<Cecha>() { nowacecha, wymagana }.ToDictionary(x => x.Id, x => x);

            ProduktCecha cechyProdukty = new ProduktCecha();
            cechyProdukty.ProduktId = produkt.Id;
            cechyProdukty.CechaId = wymagana.Id;

            Dictionary<long, ProduktCecha> lacznikicech = new Dictionary<long, ProduktCecha>() { {cechyProdukty.Id, cechyProdukty} };


            List<Tlumaczenie> slowniki = new List<Tlumaczenie>(0);
            List<JednostkaProduktu> jednostki = new List<JednostkaProduktu>(0);
           List< ProduktKategoria> lacznikikategorii = new List< ProduktKategoria>(0);
            List<ProduktUkryty> produktyukryte = new List<ProduktUkryty>(0);
            var modul = A.Fake<DodawanieCechyJesliProduktMaCeche>();
               List<Cecha> cechy = new List<Cecha>();
            List<Atrybut> atrybuty = new List<Atrybut>();
         
            A.CallTo(() => modul.CechyNaB2B).Returns(listaCech);
            modul.CechaWymagana = wymagana.Symbol;
            modul.CechaDodawanej = nowacecha.Symbol;
            modul.Przetworz(ref listaproduktow, ref slowniki, new Dictionary<long, Produkt>(), ref jednostki, ref lacznikicech, ref lacznikikategorii, null, ref produktyukryte, ref TODO, new Dictionary<long, KategoriaProduktu>()   ,ref cechy,ref atrybuty);

            bool wynik = lacznikicech.Count == 2 && lacznikicech.Any(x => x.Value.CechaId == nowacecha.Id && x.Value.ProduktId == produkt.Id);
            Assert.True(wynik, " produkt ma cechę wymaganą, nie ma nowej cechy");
        }
        private void Test2()
        {
            List<ProduktyZamienniki> TODO = new List<ProduktyZamienniki>();
            Produkt produkt = new Produkt();
            produkt.Id = 1;
            produkt.Kod = "S0784450";

            List<Produkt> listaproduktow = new List<Produkt>() { produkt };
            Cecha nowacecha = new Cecha();
            nowacecha.Nazwa = "40K028X";
            nowacecha.Symbol = "nowa:40K028X";
            nowacecha.Id = 100;
            Cecha wymagana = new Cecha();
            wymagana.Nazwa = "40K028X";
            wymagana.Symbol = "wymagana:40K028X";
            wymagana.Id = 200;
            Dictionary<long, Cecha> listaCech = new List<Cecha>() { nowacecha, wymagana }.ToDictionary(x => x.Id, x => x);

            ProduktCecha cechyProdukty = new ProduktCecha();
            cechyProdukty.ProduktId = produkt.Id;
            cechyProdukty.CechaId = wymagana.Id;
            ProduktCecha cechyProdukty2 = new ProduktCecha();
            cechyProdukty2.ProduktId = produkt.Id;
            cechyProdukty2.CechaId = nowacecha.Id;
            Dictionary<long, ProduktCecha> lacznikicech = new Dictionary<long, ProduktCecha>();


            List<Tlumaczenie> slowniki = new List<Tlumaczenie>(0);
            List<JednostkaProduktu> jednostki = new List<JednostkaProduktu>(0);
           List< ProduktKategoria> lacznikikategorii = new List< ProduktKategoria>(0);
            List<ProduktUkryty> produktyukryte = new List<ProduktUkryty>(0);
            var modul = A.Fake<DodawanieCechyJesliProduktMaCeche>();
               List<Cecha> cechy = new List<Cecha>();
            List<Atrybut> atrybuty = new List<Atrybut>();
         
            A.CallTo(() => modul.CechyNaB2B).Returns(listaCech);
            modul.CechaWymagana = wymagana.Symbol;
            modul.CechaDodawanej = nowacecha.Symbol;
            modul.Przetworz(ref listaproduktow, ref slowniki, new Dictionary<long, Produkt>(), ref jednostki, ref lacznikicech, ref lacznikikategorii, null, ref produktyukryte, ref TODO, new Dictionary<long, KategoriaProduktu>()   ,ref cechy,ref atrybuty);

            bool wynik = lacznikicech.Count == 2 && lacznikicech.Any(x => x.Value.CechaId == nowacecha.Id && x.Value.ProduktId == produkt.Id);
            Assert.True(wynik, " produkt ma cechę wymaganą, ma nową cechę");
        }
    }
}
