using System.Collections.Generic;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces.Sync;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty.Tests
{
    public class FiltrujProduktyNaKatalogBezLogowaniaTests
    {
        [Fact()]
        public void PrzetworzTest()
        {
            Produkt p1 = new Produkt() { Id = 1, Widocznosc = AccesLevel.Niezalogowani };
            Produkt p2 = new Produkt() { Id = 2, Widocznosc = AccesLevel.Niezalogowani };
            List<Produkt> listaProduktow = new List<Produkt>() { p1, p2 };

            ProduktCecha cp1 = new ProduktCecha { ProduktId = 1, CechaId = 1 };
            ProduktCecha cp2 = new ProduktCecha { ProduktId = 1, CechaId = 2 };

            ProduktCecha cp3 = new ProduktCecha { ProduktId = 2, CechaId = 1 };
            ProduktCecha cp4 = new ProduktCecha { ProduktId = 2, CechaId = 3 };

            Dictionary<long, ProduktCecha>  lacznikiCech = new Dictionary<long, ProduktCecha>() { {cp1.Id, cp1}, {cp2.Id,cp2}, {cp3.Id,cp3}, {cp4.Id,cp4} };


            List<Tlumaczenie> produktyTlumaczenia = new List<Tlumaczenie>();
            List<JednostkaProduktu> jednostki = new List<JednostkaProduktu>();
            List<ProduktKategoria> lacznikiKategorii = new List<ProduktKategoria>();
            ISyncProvider provider = null;
            List<ProduktUkryty> produktuUkryteErp = new List<ProduktUkryty>();
            List<ProduktyZamienniki> zamienniki = new List<ProduktyZamienniki>();
               List<Cecha> cechy = new List<Cecha>();
            List<Atrybut> atrybuty = new List<Atrybut>();
           

            FiltrujProduktyNaKatalogBezLogowania filtrowanie = new FiltrujProduktyNaKatalogBezLogowania();
            filtrowanie.ListaCech = new List<string>() { "1", "2" };
            filtrowanie.WidocznoscKtorykolwiek = (int)AccesLevel.Zalogowani;
            filtrowanie.WidocznoscWszystkie = (int)AccesLevel.Wszyscy;
            filtrowanie.Przetworz(ref listaProduktow, ref produktyTlumaczenia, new Dictionary<long, Produkt>(), ref jednostki, ref lacznikiCech, ref lacznikiKategorii, provider, ref produktuUkryteErp, ref zamienniki, new Dictionary<long, KategoriaProduktu>() ,ref cechy,ref atrybuty);
            Assert.True(listaProduktow[0].Widocznosc == AccesLevel.Wszyscy);
            Assert.True(listaProduktow[1].Widocznosc == AccesLevel.Zalogowani);

        }
        [Fact()]
        public void PrzetworzTest2()
        {
            Produkt p1 = new Produkt() { Id = 1, Widocznosc = AccesLevel.Wszyscy };
            Produkt p2 = new Produkt() { Id = 2, Widocznosc = AccesLevel.Wszyscy };
            List<Produkt> listaProduktow = new List<Produkt>() { p1, p2 };

            ProduktCecha cp1 = new ProduktCecha { ProduktId = 1, CechaId = 1 };
          

            ProduktCecha cp3 = new ProduktCecha { ProduktId = 2, CechaId = 1 };
            ProduktCecha cp4 = new ProduktCecha { ProduktId = 2, CechaId = 3 };

            Dictionary<long, ProduktCecha> lacznikiCech = new Dictionary<long, ProduktCecha>();


            List<Tlumaczenie> produktyTlumaczenia = new List<Tlumaczenie>();
            List<JednostkaProduktu> jednostki = new List<JednostkaProduktu>();
            List<ProduktKategoria> lacznikiKategorii = new List<ProduktKategoria>();
            ISyncProvider provider = null;
            List<ProduktUkryty> produktuUkryteErp = new List<ProduktUkryty>();
            List<ProduktyZamienniki> zamienniki = new List<ProduktyZamienniki>();
               List<Cecha> cechy = new List<Cecha>();
            List<Atrybut> atrybuty = new List<Atrybut>();
           

            FiltrujProduktyNaKatalogBezLogowania filtrowanie = new FiltrujProduktyNaKatalogBezLogowania();
            filtrowanie.ListaCech = new List<string>() { "1", "2" };
            filtrowanie.WidocznoscWszystkie = (int)AccesLevel.Niezalogowani;
            filtrowanie.Przetworz(ref listaProduktow, ref produktyTlumaczenia, new Dictionary<long, Produkt>(), ref jednostki, ref lacznikiCech, ref lacznikiKategorii, provider, ref produktuUkryteErp, ref zamienniki, new Dictionary<long, KategoriaProduktu>() ,ref cechy,ref atrybuty);
            Assert.True(listaProduktow[0].Widocznosc == AccesLevel.Wszyscy);
            Assert.True(listaProduktow[1].Widocznosc == AccesLevel.Wszyscy);

        }
      


    }
}
