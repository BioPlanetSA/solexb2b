using System.Collections.Generic;
using SolEx.Hurt.Model;
using SolEx.Hurt.Sync.App.Modules_.Rozne;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty.Tests
{
    public class OczyszczanieWarosciPolaTests
    {
        [Fact(DisplayName = "Test Oczyszczanie Wartosci pola")]
        public void PrzetworzTest()
        {
            
            testOczyszczanie("test/taa", "/","test","nazwa", JakPobieramy.Przed);
            testOczyszczanie("test/aaa","/","test/aaa","kod",JakPobieramy.Przed);

            testOczyszczanie("test/aaa", "/", "test/aaa", "kod", JakPobieramy.Za);
            testOczyszczanie("test/aaa", "/", "aaa", "nazwa", JakPobieramy.Za);
            
        }

        void testOczyszczanie(string wejscie,string separator, string oczekiwana, string poleZrodlowe, JakPobieramy rodzaj)
        {
            Produkt produkt = new Produkt();
            produkt.Nazwa = wejscie;

            OczyszczanieWarosciPola modul = new OczyszczanieWarosciPola();
            modul.JakPobieramy = rodzaj;
            
            modul.Separator = separator;
            modul.PoleZrodlowe = poleZrodlowe;

             List<Produkt> listaWejsciowa = new List<Produkt>();
            listaWejsciowa.Add(produkt);
            
            List<Tlumaczenie> produktyTlumaczenia = new List<Tlumaczenie>();
            List<JednostkaProduktu> jednostki = new List<JednostkaProduktu>();
            Dictionary<long, ProduktCecha> lacznikiCech = new Dictionary<long, ProduktCecha>();
           List< ProduktKategoria> lacznikiKategorii=new List< ProduktKategoria>();
            List<ProduktUkryty> produktuUkryteErp = new List<ProduktUkryty>();
            List<ProduktyZamienniki> zamienniki = new List<ProduktyZamienniki>();
               List<Cecha> cechy = new List<Cecha>();
            List<Atrybut> atrybuty = new List<Atrybut>();
           

            modul.Przetworz(ref listaWejsciowa, ref produktyTlumaczenia, new Dictionary<long, Produkt>(), ref jednostki, ref lacznikiCech, ref lacznikiKategorii, null, ref produktuUkryteErp, ref zamienniki, new Dictionary<long, KategoriaProduktu>() ,ref cechy,ref atrybuty);
            Assert.Equal(oczekiwana,produkt.Nazwa);
        }
        
    }
}
