using System.Collections.Generic;
using SolEx.Hurt.Model;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty;
using Xunit;

namespace SolEx.Hurt.Sync.App.Modules_Tests.NoweModuly.Produkty
{
    public class OczyszczanieNazwTests
    {
        [Fact(DisplayName = "test modułu - Usun lub podmie wybrane znaki w polu")]
        public void PrzetworzTest()
        {
            OczyszczanieNazw modul =new OczyszczanieNazw();
            modul.Pola = new List<string>() {"Nazwa"};
            modul.CzyRegex = false;
            modul.ZnakDocelowy = "<";
            modul.ZnakiDoPodmiany = "{[";
            
            Dictionary<long, Produkt> produktyB2B = new Dictionary<long, Produkt>();
            List<ProduktUkryty> pu = new List<ProduktUkryty>();
            List< ProduktKategoria> produkty_kategorie = new List< ProduktKategoria>();

            List<ProduktyZamienniki> zamienniki = new List<ProduktyZamienniki>();
            List<Produkt> produkty = new List<Produkt>();
            produkty.Add(new Produkt { Id = 1, Nazwa = "Ala{h2 " });
            produkty.Add(new Produkt { Id = 1, Nazwa = "Ala[h2 " });
            produkty.Add(new Produkt { Id = 1, Nazwa = "Ala{[h2 " });
            produkty.Add(new Produkt { Id = 1, Nazwa = "[img src = \"http://basston.pl/img/cms/DD_Audio/dd_features-Spiders-20161.jpg\"]" });

            List<JednostkaProduktu> jp = new List<JednostkaProduktu>();
            List<Tlumaczenie> slowniki = new List<Tlumaczenie>();
            Dictionary<long, ProduktCecha> cechyprodkty = new Dictionary<long, ProduktCecha>();
            List<Cecha> cechy = new List<Cecha>();
            List<Atrybut> atrybuty = new List<Atrybut>();
            modul.Przetworz(ref produkty, ref slowniki, produktyB2B, ref jp, ref cechyprodkty, ref produkty_kategorie, null, ref pu, ref zamienniki, new Dictionary<long, KategoriaProduktu>(),ref cechy, ref atrybuty);
            Assert.True(produkty[0].Nazwa.Equals("Ala<h2 "));
            Assert.True(produkty[1].Nazwa.Equals("Ala<h2 "));
            Assert.True(produkty[2].Nazwa.Equals("Ala<<h2 "));
            Assert.True(produkty[3].Nazwa.Equals("<img src = \"http://basston.pl/img/cms/DD_Audio/dd_features-Spiders-20161.jpg\"]"));
        }
    }
}
