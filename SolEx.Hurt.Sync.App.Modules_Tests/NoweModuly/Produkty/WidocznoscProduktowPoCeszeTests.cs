using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces.Sync;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty.Tests
{
    public class WidocznoscProduktowPoCeszeTests
    {
        [Fact(DisplayName = "Test sprawdzajacy poprawność działania modułu ustawiającego widoczność produktu ze wzgledu na posiadane cechy")]
        public void PrzetworzTest()
        {
            Cecha cecha1 = new Cecha("tak", "cecha1") { Id = 1 };
            Cecha cecha2 = new Cecha("nie", "cecha2") { Id = 2 };
            Cecha cecha3 = new Cecha("adfasdfasdf", "cecha3") { Id = 3 };
            Cecha cecha4 = new Cecha("sghfghdfgh", "cecha4") { Id = 4 };

            Dictionary<long, Cecha> SlownikCech = new Dictionary<long, Cecha>();
            SlownikCech.Add(cecha1.Id,cecha1);
            SlownikCech.Add(cecha2.Id, cecha2);
            SlownikCech.Add(cecha3.Id, cecha3);
            SlownikCech.Add(cecha4.Id, cecha4);

            var api = A.Fake<IAPIWywolania>();
            A.CallTo(() => api.PobierzCechy()).Returns(SlownikCech);

            Produkt p1 = new Produkt() { Id = 1, Widoczny = false };
            Produkt p2 = new Produkt() { Id = 3, Widoczny = false };
            Produkt p4 = new Produkt() { Id = 4, Widoczny = false, };
            List<Produkt> listaWejsciowa = new List<Produkt>(){p1,p2,p4};

            ProduktCecha cp1= new ProduktCecha(){CechaId = 1, ProduktId = 1};
            ProduktCecha cp2= new ProduktCecha(){CechaId = 1, ProduktId = 2};
            ProduktCecha cp3= new ProduktCecha(){CechaId = 2, ProduktId = 3};
            Dictionary<long,ProduktCecha> listaLacznikow = new Dictionary<long, ProduktCecha> { {cp1.Id, cp1}, {cp2.Id, cp2}, {cp3.Id, cp3}};


            var prov = A.Fake<ISyncProvider>();
            A.CallTo(() => prov.PobierzCechyProduktow_Polaczenia(new []{1})).Returns(listaLacznikow.Values.ToList());

            List<Tlumaczenie> produktyTlumaczenia = new List<Tlumaczenie>();
            Dictionary<long, Produkt> produktyNaB2B = new Dictionary<long, Produkt>(); 
            List<JednostkaProduktu> jednostki = new List<JednostkaProduktu>();
            List<ProduktKategoria> lacznikiKategorii = new List<ProduktKategoria>();
            List<ProduktUkryty> produktuUkryteErp = new List<ProduktUkryty>();
            List<ProduktyZamienniki> zamienniki = new List<ProduktyZamienniki>();
            List<Cecha> cechy = new List<Cecha>();
            List<Atrybut> atrybuty = new List<Atrybut>();
           

            DezaktywacjaLubAktywacjaProduktuPoCesze mod = new DezaktywacjaLubAktywacjaProduktuPoCesze();
            mod.Widocznosc = true;
           mod.ApiWywolanie = api;
            mod.Cechy = new List<int>(){1,6};
            //mod.Kolejnosc = -11;
            mod.Przetworz(ref listaWejsciowa, ref produktyTlumaczenia, produktyNaB2B, ref jednostki, ref listaLacznikow, ref lacznikiKategorii, prov, ref produktuUkryteErp, ref zamienniki, new Dictionary<long, KategoriaProduktu>() ,ref cechy,ref atrybuty);
            Assert.True(listaWejsciowa[0].Widoczny);


            Assert.False(listaWejsciowa[1].Widoczny);


            Assert.False(listaWejsciowa[2].Widoczny );

        }
        [Fact(DisplayName = "Test sprawdzajacy poprawność działania modułu ustawiającego widoczność produktu ze wzgledu na posiadane cechy, ukrywamy produkty modułem")]
        public void PrzetworzTest2()
        {
            Cecha cecha1 = new Cecha("tak", "cecha1") { Id = 1 };
            Cecha cecha2 = new Cecha("nie", "cecha2") { Id = 2 };
            Cecha cecha3 = new Cecha("adfasdfasdf", "cecha3") { Id = 3 };
            Cecha cecha4 = new Cecha("sghfghdfgh", "cecha4") { Id = 4 };

            Dictionary<long, Cecha> SlownikCech = new Dictionary<long, Cecha>();
            SlownikCech.Add(cecha1.Id, cecha1);
            SlownikCech.Add(cecha2.Id, cecha2);
            SlownikCech.Add(cecha3.Id, cecha3);
            SlownikCech.Add(cecha4.Id, cecha4);

            var api = A.Fake<IAPIWywolania>();
            A.CallTo(() => api.PobierzCechy()).Returns(SlownikCech);

            Produkt p1 = new Produkt() { Id = 1, Widoczny = true };
            Produkt p2 = new Produkt() { Id = 3, Widoczny = true};
            Produkt p4 = new Produkt() { Id = 4, Widoczny = true };
            List<Produkt> listaWejsciowa = new List<Produkt>() { p1, p2, p4 };

            ProduktCecha cp1 = new ProduktCecha() { CechaId = 1, ProduktId = 1 };
            ProduktCecha cp2 = new ProduktCecha() { CechaId = 1, ProduktId = 2 };
            ProduktCecha cp3 = new ProduktCecha() { CechaId = 2, ProduktId = 3 };
            Dictionary<long, ProduktCecha> listaLacznikow = new Dictionary<long, ProduktCecha> { { cp1.Id, cp1 }, { cp2.Id, cp2 }, { cp3.Id, cp3 } };


            var prov = A.Fake<ISyncProvider>();
       //     A.CallTo(() => prov.PobierzCechyProduktow_Polaczenia()).Returns(listaLacznikow);

            List<Tlumaczenie> produktyTlumaczenia = new List<Tlumaczenie>();
            Dictionary<long, Produkt> produktyNaB2B = new Dictionary<long, Produkt>();
            List<JednostkaProduktu> jednostki = new List<JednostkaProduktu>();
            List<ProduktKategoria> lacznikiKategorii = new List<ProduktKategoria>();
            List<ProduktUkryty> produktuUkryteErp = new List<ProduktUkryty>();
            List<ProduktyZamienniki> zamienniki = new List<ProduktyZamienniki>();   List<Cecha> cechy = new List<Cecha>();
            List<Atrybut> atrybuty = new List<Atrybut>();
            

            DezaktywacjaLubAktywacjaProduktuPoCesze mod = new DezaktywacjaLubAktywacjaProduktuPoCesze();
            mod.Widocznosc = false;
            mod.ApiWywolanie = api;
            mod.Cechy = new List<int>() { 2 };
           // mod.Kolejnosc = -11;
            mod.Przetworz(ref listaWejsciowa, ref produktyTlumaczenia, produktyNaB2B, ref jednostki, ref listaLacznikow, ref lacznikiKategorii, prov, ref produktuUkryteErp, ref zamienniki, new Dictionary<long, KategoriaProduktu>(),ref cechy,ref atrybuty);
            Assert.True(listaWejsciowa[0].Widoczny);

            Assert.False(listaWejsciowa[1].Widoczny);

            Assert.True(listaWejsciowa[2].Widoczny);
        }
    }
}

