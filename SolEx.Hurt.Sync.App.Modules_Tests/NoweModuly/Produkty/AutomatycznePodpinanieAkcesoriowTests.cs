using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FakeItEasy;
using SolEx.Hurt.Model;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty.Tests
{
    public class AutomatycznePodpinanieAkcesoriowTests
    {
        [Fact()]
        public void PrzetworzTest()
        {
            Cecha c1 = new Cecha() { AtrybutId = 1, Id = 1, Symbol = "cb:123" };
            Cecha c2 = new Cecha() { AtrybutId = 2, Id = 2, Symbol = "kolor:zielony" };
            Cecha c3 = new Cecha() { AtrybutId = 3, Id = 3, Symbol = "kat:felgi" };
            Cecha c4 = new Cecha() { AtrybutId = 3, Id = 4, Symbol = "kat:nakretki" };

            List<Cecha> listaCech = new List<Cecha>() { c1, c2, c3, c4 };

            Produkt p1 = new Produkt() { Id = 1, Nazwa = "felga a" };
            Produkt p2 = new Produkt() { Id = 2, Nazwa = "nakretka z" };

            AutomatycznePodpinanieAkcesoriow auto = A.Fake<AutomatycznePodpinanieAkcesoriow>();
            auto.WspolneAtrybuty = new List<string>() { "1", "2", "3" }; 
            auto.CechyProduktowNadrzednych = new List<string>() { "3", "4" };
            auto.CechyAkcesoriow = new List<string>() { "4" };                                                   
            A.CallTo(() => auto.CechyNaB2B).Returns(listaCech);

            ProduktCecha cp1 = new ProduktCecha() {  CechaId = 1, ProduktId = 1 };
            ProduktCecha cp2 = new ProduktCecha() { CechaId = 2, ProduktId = 1 };
            ProduktCecha cp3 = new ProduktCecha() { CechaId = 3, ProduktId = 1 };
            ProduktCecha cp4 = new ProduktCecha() { CechaId = 4, ProduktId = 2 };
            ProduktCecha cp5 = new ProduktCecha() {CechaId = 1, ProduktId = 2 };
            ProduktCecha cp6 = new ProduktCecha() {CechaId = 6, ProduktId = 2 };

            Dictionary<long, ProduktCecha> lacznikiCech = new Dictionary<long, ProduktCecha> { { cp1.Id, cp1 }, { cp2.Id, cp2 }, { cp3.Id, cp3 }, { cp4.Id, cp4 }, { cp5.Id, cp5 }, { cp6.Id, cp6 } };

            List<Produkt> listaWejsciowa = new List<Produkt>();
            List<Tlumaczenie> produktyTlumaczenia = new List<Tlumaczenie>();
            Dictionary<long, Produkt> produktyNaB2B = new Dictionary<long, Produkt>();
            List<JednostkaProduktu> jednostki = new List<JednostkaProduktu>();

            List<ProduktKategoria> lacznikiKategorii = new List<ProduktKategoria>();
            Model.Interfaces.Sync.ISyncProvider provider = null;
            List<ProduktUkryty> produktuUkryteErp = new List<ProduktUkryty>();
            List<ProduktyZamienniki> zamienniki = new List<ProduktyZamienniki>();
               List<Cecha> cechy = new List<Cecha>();
            List<Atrybut> atrybuty = new List<Atrybut>();
            
            auto.Przetworz(ref listaWejsciowa, ref produktyTlumaczenia, produktyNaB2B, ref  jednostki, ref lacznikiCech, ref lacznikiKategorii, provider, ref produktuUkryteErp, ref zamienniki, new Dictionary<long, KategoriaProduktu>(),ref cechy,ref atrybuty);
            Assert.True(zamienniki.Count() == 2);
            Assert.True(zamienniki[0].ProduktId == p1.Id && zamienniki[0].ZamiennikId == p2.Id);
        }
        [Fact()]
        public void PrzetworzTest2()
        {
            Cecha c1 = new Cecha() { AtrybutId = 1, Id = 1, Symbol = "cb:123" };
            Cecha c2 = new Cecha() { AtrybutId = 2, Id = 2, Symbol = "kolor:zielony" };
            Cecha c3 = new Cecha() { AtrybutId = 3, Id = 3, Symbol = "kat:felgi" };
            Cecha c4 = new Cecha() { AtrybutId = 3, Id = 4, Symbol = "kat:nakretki" };

            List<Cecha> ListaCech = new List<Cecha>() { c1, c2, c3, c4 };

            Produkt p1 = new Produkt() { Id = 1, Nazwa = "felga a" };
            Produkt p2 = new Produkt() { Id = 2, Nazwa = "nakretka z" };

            AutomatycznePodpinanieAkcesoriow auto = A.Fake<AutomatycznePodpinanieAkcesoriow>();
            auto.WspolneAtrybuty = new List<string>() { "1", "2", "3" };
            auto.CechyProduktowNadrzednych = new List<string>() { "3" };
            auto.CechyAkcesoriow = new List<string>() { "4" };  
            A.CallTo(() => auto.CechyNaB2B).Returns(ListaCech);


            ProduktCecha cp1 = new ProduktCecha() {CechaId = 1, ProduktId = 1 };
            ProduktCecha cp2 = new ProduktCecha() {CechaId = 2, ProduktId = 1 };
            ProduktCecha cp3 = new ProduktCecha() {  CechaId = 3, ProduktId = 1 };
            ProduktCecha cp4 = new ProduktCecha() { CechaId = 4, ProduktId = 2 };
            ProduktCecha cp5 = new ProduktCecha() { CechaId = 5, ProduktId = 2 };
            ProduktCecha cp6 = new ProduktCecha() {  CechaId = 6, ProduktId = 2 };

            Dictionary<long,ProduktCecha> lacznikiCech = new Dictionary<long, ProduktCecha> { {cp1.Id,cp1}, {cp2.Id, cp2}, {cp3.Id, cp3}, {cp4.Id, cp4}, {cp5.Id,cp5}, {cp6.Id, cp6} };

            List<Produkt> listaWejsciowa = new List<Produkt>();
            List<Tlumaczenie> produktyTlumaczenia = new List<Tlumaczenie>();
            Dictionary<long, Produkt> produktyNaB2B = new Dictionary<long, Produkt>();
            List<JednostkaProduktu> jednostki = new List<JednostkaProduktu>();

            List<ProduktKategoria> lacznikiKategorii = new List<ProduktKategoria>();
            Model.Interfaces.Sync.ISyncProvider provider = null;
            List<ProduktUkryty> produktuUkryteErp = new List<ProduktUkryty>();
            List<ProduktyZamienniki> zamienniki = new List<ProduktyZamienniki>();
               List<Cecha> cechy = new List<Cecha>();
            List<Atrybut> atrybuty = new List<Atrybut>();
           
            auto.Przetworz(ref listaWejsciowa, ref produktyTlumaczenia, produktyNaB2B, ref  jednostki, ref lacznikiCech, ref lacznikiKategorii, provider, ref produktuUkryteErp, ref zamienniki, new Dictionary<long, KategoriaProduktu>() ,ref cechy,ref atrybuty);
            Assert.True(!zamienniki.Any());
            //Assert.True(zamienniki[0].ProduktId == p1.produkt_id && zamienniki[0].ZamiennikId == p2.produkt_id);
        }
        [Fact(DisplayName = "Test wydajnosciowy")]
        public void PrzetworzTest3()
        {

            Random rd = new Random();
            List<Cecha> ListaCech = new List<Cecha>();
            for (int i = 0; i < 1000; i++)
            {
                
                int idAtrybutu = rd.Next(1, 10);
                
                ListaCech.Add(new Cecha(){Id = i+1, AtrybutId = idAtrybutu});
            }

            AutomatycznePodpinanieAkcesoriow auto = A.Fake<AutomatycznePodpinanieAkcesoriow>();
            List<string> listaWspolna = new List<string>() { "1", "2", "3", "4", "5", "6" };
            auto.WspolneAtrybuty = listaWspolna;
            auto.CechyProduktowNadrzednych = new List<string>() { "3" };
            auto.CechyAkcesoriow = new List<string>() { "4" };  
            A.CallTo(() => auto.CechyNaB2B).Returns(ListaCech);
            //Random rd = new Random();
            Dictionary<long,ProduktCecha> lacznikiCech = new Dictionary<long,ProduktCecha>();
            for (int i = 0; i < 1000; i++)
            {
                
                int cechaId = rd.Next(1, 6);
                int produktId = rd.Next(1, 10);
                var cp = new ProduktCecha() {CechaId = cechaId, ProduktId = produktId};
                lacznikiCech.Add(cp.Id, cp);
            }


            List<Produkt> listaWejsciowa = new List<Produkt>();
            List<Tlumaczenie> produktyTlumaczenia = new List<Tlumaczenie>();
            Dictionary<long, Produkt> produktyNaB2B = new Dictionary<long, Produkt>();
            List<JednostkaProduktu> jednostki = new List<JednostkaProduktu>();

            List<ProduktKategoria> lacznikiKategorii = new List<ProduktKategoria>();
            Model.Interfaces.Sync.ISyncProvider provider = null;
            List<ProduktUkryty> produktuUkryteErp = new List<ProduktUkryty>();
            List<ProduktyZamienniki> zamienniki = new List<ProduktyZamienniki>();

            Stopwatch stoper = Stopwatch.StartNew();
               List<Cecha> cechy = new List<Cecha>();
            List<Atrybut> atrybuty = new List<Atrybut>();
          
            auto.Przetworz(ref listaWejsciowa, ref produktyTlumaczenia, produktyNaB2B, ref  jednostki, ref lacznikiCech, ref lacznikiKategorii, provider, ref produktuUkryteErp, ref zamienniki, new Dictionary<long, KategoriaProduktu>()  ,ref cechy,ref atrybuty);
            stoper.Stop();
            Assert.True(stoper.Elapsed.TotalSeconds < 1, string.Format("za długi czas przydzielania akcesoriów: {0} sekund zamiast poniżej 1 sekundy", stoper.Elapsed.TotalSeconds));
        }
    }
}
