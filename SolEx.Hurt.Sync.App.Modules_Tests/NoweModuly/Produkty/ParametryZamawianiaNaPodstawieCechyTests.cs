using System.Collections.Generic;
using FakeItEasy;
using SolEx.Hurt.Model;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty.Tests
{
    public class ParametryZamawianiaNaPodstawieCechyTests
    {
        [Fact(DisplayName = "Ustawianie minimu logistycznego i ilości w opakowaniu na podstawie cechy")]
        public void PrzetworzTest()
        {
          Test(2,3,"2/3");
          Test(2.3M, 3.3M, "2.3/3.3");
          Test(2.3M, 3.3M, "2,3/3,3");
          Test(0.3M,0.3M, "0.3/0.3");
          Test(0.3M, 0.3M, "0,3/0,3");
          Test(0.3M, 0.3M, "0.3 / 0.3");
          Test(0.3M, 0.3M, "0,3 / 0,3");
          Test(4, 3, "4/3");
          Test(0, 1, "4 kg/3 kg");
          Test(0, 3, "4 kg/3");
          Test(4, 1, "4/3 kg");
          Test(0, 1, "4 3");
          Test(0, 1, "4");
        }

        void Test(decimal oczekiwanemin, decimal oczekiwaneop, string cecha)
        {
            var pprodukt = new Produkt { Id = 1 };
            List<Cecha> cechy = new List<Cecha>();
            cechy.Add(new Cecha { Id = 1, Symbol = "minima:" + cecha, Nazwa = cecha });
            var modul = A.Fake<ParametryZamawianiaNaPodstawieCechy>();
            modul.PoczatekCechy = "minima:";

            A.CallTo(() => modul.Cechy).Returns(cechy);

            Dictionary<long, Produkt> produktyB2B = new Dictionary<long, Produkt>();
            List<ProduktUkryty> pu = new List<ProduktUkryty>();
           List< ProduktKategoria> produkty_kategorie = new List< ProduktKategoria>();

            List<ProduktyZamienniki> zamienniki = new List<ProduktyZamienniki>();
            List<Produkt> produkty = new List<Produkt>();
            produkty.Add(pprodukt);


            List<JednostkaProduktu> jp = new List<JednostkaProduktu>();
            List<Tlumaczenie> slowniki = new List<Tlumaczenie>();
            Dictionary<long, ProduktCecha> cechyprodkty = new Dictionary<long, ProduktCecha>();
            var cp = new ProduktCecha() {ProduktId = 1, CechaId = 1};
            cechyprodkty.Add(cp.Id, cp);

            List<Atrybut> atrybuty = new List<Atrybut>();
            modul.Przetworz(ref produkty, ref slowniki, produktyB2B, ref jp, ref cechyprodkty, ref produkty_kategorie, null, ref pu, ref zamienniki, new Dictionary<long, KategoriaProduktu>(), ref cechy, ref atrybuty);
            Assert.True(pprodukt.IloscMinimalna == oczekiwanemin,string.Format("Otrzymane min {0}, oczekiwane {1} nazwa {2}",pprodukt.IloscMinimalna,oczekiwanemin,cecha));
            Assert.True(pprodukt.IloscWOpakowaniu == oczekiwaneop, string.Format("Otrzymane op {0}, oczekiwane {1} nazwa {2}", pprodukt.IloscWOpakowaniu, oczekiwaneop, cecha));
        }
    }
}
