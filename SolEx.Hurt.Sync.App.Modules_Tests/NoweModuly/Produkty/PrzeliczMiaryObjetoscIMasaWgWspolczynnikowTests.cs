using System.Collections.Generic;
using SolEx.Hurt.Model;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty.Tests
{
    public class PrzeliczMiaryObjetoscIMasaWgWspolczynnikowTests
    {
        [Fact(DisplayName = "Test sprawdzający poprawne działanie modulu Przeliczania miar objętości oraz wag produktów")]
        public void PrzetworzTest()
        {
            Produkt p1 = new Produkt()
            {
                Id = 1,
                Waga = 1,
                Objetosc = 2261.3m,
            };
            Produkt p2 = new Produkt()
            {
                Id = 2,
                Waga = 0.3m,
                Objetosc = 0.00m,
            };
            Produkt p3 = new Produkt()
            {
                Id = 3,
                Waga = 0.00m,
                Objetosc = 3279.96m,
            };
            Produkt p4 = new Produkt()
            {
                Id = 4,
                Waga = 0.00m,
                Objetosc =0.00m,
            };
            List<Produkt> listaProduktow = new List<Produkt>(){p1,p2,p3,p4};

            PrzeliczMiaryObjetoscIMasaWgWspolczynnikow modul = new PrzeliczMiaryObjetoscIMasaWgWspolczynnikow();
            modul.MnoznikDlaObjetosci = 0.01m;
            modul.MnoznikDlaWag = 1;
            modul.Zaokraglenie = 4;


            List<Tlumaczenie> produktyTlumaczenia=new List<Tlumaczenie>();
            Dictionary<long, Produkt> produktyNaB2B=new Dictionary<long, Produkt>();
            List<JednostkaProduktu> jednostki=new List<JednostkaProduktu>();
            Dictionary<long, ProduktCecha> lacznikiCech = new Dictionary<long, ProduktCecha>();

            List<ProduktKategoria> lacznikiKategorii=new List<ProduktKategoria>();
            Model.Interfaces.Sync.ISyncProvider provider = null;
            List<ProduktUkryty> produktuUkryteErp=new List<ProduktUkryty>();
            List<ProduktyZamienniki> zamienniki=new List<ProduktyZamienniki>();
            Dictionary<long, KategoriaProduktu> kategorie = new Dictionary<long, KategoriaProduktu>();
               List<Cecha> cechy = new List<Cecha>();
            List<Atrybut> atrybuty = new List<Atrybut>();
            


            modul.Przetworz(ref listaProduktow,ref produktyTlumaczenia, produktyNaB2B, ref jednostki, ref lacznikiCech, ref lacznikiKategorii,provider,ref produktuUkryteErp, ref zamienniki,kategorie,ref cechy,ref atrybuty);

            Assert.True(p1.Waga==(1m));
            Assert.True(p2.Waga == 0.3m);
            Assert.True(p3.Waga == 0.000m);
            Assert.True(p4.Waga == 0.000m);


            Assert.True(p1.Objetosc == (0.0023m));
            Assert.True(p2.Objetosc == 0.000m);
            Assert.True(p3.Objetosc == 0.0033m);
            Assert.True(p4.Objetosc == 0.000m);

            
        }
    }
}
