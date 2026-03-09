using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using SolEx.Hurt.Model;
using Xunit;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty.Tests
{
    public class PrzypisanieCechyDlaNowychProduktowTests
    {
        [Fact()]
        public void PrzetworzTest()
        {
           Test1();
           Test2();
        }

        private void Test2()
        {
            var modul = A.Fake<PrzypisanieCechyDlaNowychProduktow>();
            Dictionary<long, Cecha> cechy = new Dictionary<long, Cecha>();
            cechy.Add(1, new Cecha() { Id = 1, Symbol = "dododania" });
            cechy.Add(2, new Cecha() { Id = 2, Symbol = "pomijana" });
            A.CallTo(() => modul.CechyNaB2B).Returns(cechy);
            modul.ListaCechWykluczenia = new List<string>(){"2"};
            modul.CechaDoDodania = 1;
            //modul.SymbolCechy = "dododania";
            //modul.Wykluczenia = "pomijana";
            modul.IloscDniWstecz = 3;
            //modul.IleProduktow = 50;
            List<Produkt> produkty = new List<Produkt>();
            produkty.Add(new Produkt(1){DataDodania = DateTime.Now, Kod="Produkt 1"});
            produkty.Add(new Produkt(2) { DataDodania = DateTime.Now, Kod = "Produkt 2" });
            produkty.Add(new Produkt(3) { DataDodania = DateTime.Now.AddDays(-10), Kod="Produkt 3" });
            List<Tlumaczenie> slowniki = new List<Tlumaczenie>();
            Dictionary<long, Produkt> produktyB2B = new Dictionary<long, Produkt>();
            List<JednostkaProduktu> jednostki = new List<JednostkaProduktu>();
            Dictionary<long, ProduktCecha> lacznikiCech = new Dictionary<long, ProduktCecha>();

            var cp1 = new ProduktCecha {CechaId = 2, ProduktId = 2};
            lacznikiCech.Add(cp1.Id, cp1);
            var cp2 = new ProduktCecha {CechaId = 1, ProduktId = 3};
            lacznikiCech.Add(cp2.Id, cp2);
           List< ProduktKategoria> produkty_kategorie = new List< ProduktKategoria>();
            List<ProduktUkryty> pu = new List<ProduktUkryty>();
            List<ProduktyZamienniki> ProduktyZamienniki = new List<ProduktyZamienniki>();
               List<Cecha> cechy1 = new List<Cecha>();
            List<Atrybut> atrybuty = new List<Atrybut>();
            
            modul.Przetworz(ref produkty, ref slowniki, produktyB2B, ref jednostki, ref lacznikiCech, ref produkty_kategorie, null, ref pu, ref ProduktyZamienniki, new Dictionary<long, KategoriaProduktu>(),ref cechy1,ref atrybuty);
            Assert.True(lacznikiCech.Count(x => x.Value.CechaId == 1) == 1);
        }

        private void Test1()
        {
            var modul = A.Fake<PrzypisanieCechyDlaNowychProduktow>();
            Dictionary<long, Cecha> cechy = new Dictionary<long, Cecha>();
            cechy.Add(1, new Cecha() { Id = 1, Symbol = "dododania" });
            cechy.Add(2, new Cecha() { Id = 2, Symbol = "pomijana" });
            A.CallTo(() => modul.CechyNaB2B).Returns(cechy);
            modul.SymbolCechy = "dododania";
           // modul.IleProduktow = 50;
            modul.IloscDniWstecz = 3;
            List<Produkt> produkty = new List<Produkt>();
            produkty.Add(new Produkt(1) { DataDodania = DateTime.Now, Kod = "Produkt 1" });
            produkty.Add(new Produkt(2) { DataDodania = DateTime.Now, Kod = "Produkt 2" });
            produkty.Add(new Produkt(3) { DataDodania = DateTime.Now.AddDays(-10), Kod = "Produkt 3" });
            List<Tlumaczenie> slowniki = new List<Tlumaczenie>();
            Dictionary<long, Produkt> produktyB2B = new Dictionary<long, Produkt>();
            List<JednostkaProduktu> jednostki = new List<JednostkaProduktu>();
            Dictionary<long, ProduktCecha> lacznikiCech = new Dictionary<long, ProduktCecha>();

            var cp = new ProduktCecha {CechaId = 2, ProduktId = 2};
            lacznikiCech.Add(cp.Id, cp);
           List< ProduktKategoria> produkty_kategorie = new List< ProduktKategoria>();
            List<ProduktUkryty> pu = new List<ProduktUkryty>();
            List<ProduktyZamienniki> ProduktyZamienniki = new List<ProduktyZamienniki>();
               List<Cecha> cechy1 = new List<Cecha>();
            List<Atrybut> atrybuty = new List<Atrybut>();
           
            modul.Przetworz(ref produkty, ref slowniki, produktyB2B, ref jednostki, ref lacznikiCech, ref produkty_kategorie, null, ref pu, ref ProduktyZamienniki, new Dictionary<long, KategoriaProduktu>() ,ref cechy1,ref atrybuty);
            Assert.True(lacznikiCech.Count(x => x.Value.CechaId == 1) == 2);
        }
    }
}
