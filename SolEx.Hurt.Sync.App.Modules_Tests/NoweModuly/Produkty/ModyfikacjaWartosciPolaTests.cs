using System.Collections.Generic;
using SolEx.Hurt.Model;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty.Tests
{
    public class ModyfikacjaWartosciPolaTests
    {
        [Fact()]
        public void PrzetworzTest()
        {
            ModyfikacjaWartosciPola modul=new ModyfikacjaWartosciPola();
            modul.Pola = "nazwa";
            modul.NowaWartoscPola = "nowa_{0}";

            string nazwa = "nazwa";
            string wynik = string.Format(modul.NowaWartoscPola, nazwa);
            Dictionary<long, Produkt> produktyB2B = new Dictionary<long, Produkt>();
            List<ProduktUkryty> pu = new List<ProduktUkryty>();
           List< ProduktKategoria> produkty_kategorie = new List< ProduktKategoria>();

            List<ProduktyZamienniki> zamienniki = new List<ProduktyZamienniki>();
            List<Produkt> produkty = new List<Produkt>();
            produkty.Add(new Produkt { Id = 1, Nazwa = nazwa });

            List<JednostkaProduktu> jp = new List<JednostkaProduktu>();
            List<Tlumaczenie> slowniki = new List<Tlumaczenie>();
            Dictionary<long, ProduktCecha> cechyprodkty = new Dictionary<long, ProduktCecha>();
            List<Cecha> cechy = new List<Cecha>();
            List<Atrybut> atrybuty = new List<Atrybut>();
            modul.Przetworz(ref produkty, ref slowniki, produktyB2B, ref jp, ref cechyprodkty, ref produkty_kategorie, null, ref pu, ref zamienniki, new Dictionary<long, KategoriaProduktu>(),ref cechy, ref atrybuty);
            Assert.True(produkty[0].Nazwa==wynik, "not implemented yet");
        }
    }
}
