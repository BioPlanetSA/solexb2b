using System;
using System.Collections.Generic;
using SolEx.Hurt.Model;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty.Tests
{
    public class UsunStareDniDostawyTests
    {
        [Fact()]
        public void UsunStareDniDostawyTest()
        {
            List<Tlumaczenie> slowniki = new List<Tlumaczenie>(0);
            List<JednostkaProduktu> jednostki = new List<JednostkaProduktu>(0);
            Dictionary<long, ProduktCecha> cechyprodukty = new Dictionary<long, ProduktCecha>();
            List<ProduktUkryty> produktuUkryteErp = new List<ProduktUkryty>(0);
            List<ProduktKategoria> lacznikiKategorii = new List<ProduktKategoria>(0);   List<Cecha> cechy = new List<Cecha>();
            List<Atrybut> atrybuty = new List<Atrybut>();
            

            UsunStareDniDostawy modul = new UsunStareDniDostawy();
            modul.FormatDaty = "dd.MM.yyyy";

            Produkt produkt = new  Produkt();
            produkt.Dostawa = "01.05.2014";
         List<ProduktyZamienniki> TODO=new List<ProduktyZamienniki>();
            List<Produkt> listawejsciowa = new List<Produkt>(1);
            listawejsciowa.Add(produkt);

            modul.Przetworz(ref listawejsciowa, ref slowniki, new Dictionary<long, Produkt>(), ref jednostki,
                ref cechyprodukty, ref lacznikiKategorii, null, ref produktuUkryteErp, ref TODO, new Dictionary<long, KategoriaProduktu>(),ref cechy,ref atrybuty);

            //data w produkcie jest starsza niż obecna data więc dostawa powinna być nullem
            Assert.Null(produkt.Dostawa);

            string dataPrzyszlosci = DateTime.Now.AddDays(1).ToString(modul.FormatDaty);
            produkt.Dostawa = dataPrzyszlosci;

            modul.Przetworz(ref listawejsciowa, ref slowniki, new Dictionary<long, Produkt>(), ref jednostki,
                ref cechyprodukty, ref lacznikiKategorii, null, ref produktuUkryteErp, ref TODO, new Dictionary<long, KategoriaProduktu>(),ref cechy,ref atrybuty);

            //dla daty większej od obecnej moduł nie powinien usuwać daty dostawy
            Assert.Equal(produkt.Dostawa, dataPrzyszlosci);

            string dzisiejszaData = DateTime.Now.ToString(modul.FormatDaty);
            produkt.Dostawa = dzisiejszaData;

            modul.Przetworz(ref listawejsciowa, ref slowniki, new Dictionary<long, Produkt>(), ref jednostki,
                ref cechyprodukty, ref lacznikiKategorii, null, ref produktuUkryteErp, ref TODO, new Dictionary<long, KategoriaProduktu>(),ref cechy,ref atrybuty);

            //dzisiejsza data dostawy na produkcie nie może zostać usunięta
            Assert.Equal(produkt.Dostawa, dzisiejszaData);
        }
    }
}
