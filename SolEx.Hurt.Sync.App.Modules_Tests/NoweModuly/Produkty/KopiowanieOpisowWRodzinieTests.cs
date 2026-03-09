using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty;
using Xunit;

namespace SolEx.Hurt.Sync.App.Modules_Tests.NoweModuly.Produkty
{
    public class KopiowanieOpisowWRodzinieTests
    {
        [Fact(DisplayName = "Test sprawdzający poprawność kopiowania pola stringowego w rodzinie - wymuszone kopiowanie")]
        public void PrzetworzTest()
        {
            Produkt p1 = new Produkt()
            {
                Id = 1,
                //opis = "",
                Rodzina = "Rodzina 1"
            };
            Produkt p2 = new Produkt()
            {
                Id = 2,
                Opis = "Opis 21",
                Rodzina = "Rodzina 1"
            };
            Produkt p3 = new Produkt()
            {
                Id = 3,
                Opis = "Opis 31",
                Rodzina = "Rodzina 2"
            };
            Produkt p4 = new Produkt()
            {
                Id = 4,
                Opis = "Opis 4",
                Rodzina = "Rodzina 2"
            };
            List<Produkt> listaProduktow = new List<Produkt>(){p1,p2,p3,p4};




            Model.Tlumaczenie s1 = new Tlumaczenie()
            {
                ObiektId = 1,
                Wpis = "Opis produktu 1 po engliszu",
                Typ = "SolEx.Hurt.Model.Produkt",
                Pole = "opis",
                JezykId = 2
            };
            Model.Tlumaczenie s2 = new Tlumaczenie()
            {
                ObiektId = 1,
                Typ = "SolEx.Hurt.Model.Produkt",
                Pole = "opis",
                JezykId = 1
            };
            Model.Tlumaczenie s3 = new Tlumaczenie()
            {
                ObiektId = 2,
                Wpis = "Opis produktu 2 po engliszu",
                Typ = "SolEx.Hurt.Model.Produkt",
                Pole = "opis",
                JezykId = 2
            };
            Model.Tlumaczenie s4 = new Tlumaczenie()
            {
                ObiektId = 2,
                Typ = "SolEx.Hurt.Model.Produkt",
                Wpis = "Opis 21",
                Pole = "opis",
                JezykId = 1
            };


            //Model.slowniki s5 = new slowniki()
            //{
            //    id = 5,
            //    obiekt_id = 3,
            //    wpis = "Opis produktu 3 po engliszu",
            //    typ_id = 1,
            //    pole = "opis",
            //    jezyk_id = 2
            //};
            Model.Tlumaczenie s6 = new Tlumaczenie()
            {
                ObiektId = 3,
                Wpis = "Opis 31",
                Typ = "SolEx.Hurt.Model.Produkt",
                Pole = "opis",
                JezykId = 1
            };
            Model.Tlumaczenie s7 = new Tlumaczenie()
            {
                ObiektId = 4,
                Wpis = "Opis produktu 4 po engliszu",
                Typ = "SolEx.Hurt.Model.Produkt",
                Pole = "opis",
                JezykId = 2
            };
            Model.Tlumaczenie s8 = new Tlumaczenie()
            {
                ObiektId = 4,
                Typ = "SolEx.Hurt.Model.Produkt",
                Wpis = "Opis 4",
                Pole = "opis",
                JezykId = 1
            };

            List<Tlumaczenie> slowniki = new List<Tlumaczenie>(){s1,s2,s3,s4,s7,s6,s8};


            Dictionary<long, Produkt> produktyNaB2B = new Dictionary<long, Produkt>();
            List<JednostkaProduktu> jednostki = new List<JednostkaProduktu>();
            Dictionary<long, ProduktCecha> lacznikiCech = new Dictionary<long, ProduktCecha>();
            List<ProduktKategoria> lacznikiKategorii = new List<ProduktKategoria>();
            Model.Interfaces.Sync.ISyncProvider provider = null;
            List<ProduktUkryty> produktuUkryteErp = new List<ProduktUkryty>();
            List<ProduktyZamienniki> zamienniki=new List<ProduktyZamienniki>();
            Dictionary<long, KategoriaProduktu> kategorie = new Dictionary<long, KategoriaProduktu>();




            Dictionary<int, Jezyk> jezykiWSystemie = new Dictionary<int, Jezyk>();
            Jezyk j1 = new Jezyk() { Id = 1, Nazwa = "Polski" };
            Jezyk j2 = new Jezyk() { Id = 2, Nazwa = "Angielski" };
            jezykiWSystemie.Add(j1.Id, j1);
            jezykiWSystemie.Add(j2.Id, j2);

            
            var config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.JezykiWSystemie).Returns(jezykiWSystemie);

            KopiowanieOpisowWRodzinie modul = new KopiowanieOpisowWRodzinie();
            modul.Pola = new List<string>(){"opis"};
            modul.WymuszenieKopiowania = true;
            modul.config = config;
               List<Cecha> cechy = new List<Cecha>();
            List<Atrybut> atrybuty = new List<Atrybut>();
          
            modul.Przetworz(ref listaProduktow, ref slowniki, produktyNaB2B, ref jednostki, ref lacznikiCech, ref lacznikiKategorii, provider, ref produktuUkryteErp, ref zamienniki,kategorie  ,ref cechy,ref atrybuty);

            Assert.True(listaProduktow.First(x => x.Id == 1).Opis == p2.Opis);
            Assert.True(listaProduktow.First(x => x.Id == 4).Opis == p3.Opis);

            Assert.True(slowniki.First(x => x.ObiektId == 1 && x.JezykId == 2).Wpis == s3.Wpis);
            Assert.True(slowniki.First(x => x.ObiektId == 1 && x.JezykId == 1).Wpis == s4.Wpis);

            Assert.True(slowniki.First(x => x.ObiektId == 3 && x.JezykId == 2).Wpis == s6.Wpis);
            Assert.True(slowniki.Count == jezykiWSystemie.Count * listaProduktow.Count);
            Assert.True(slowniki.First(x => x.ObiektId == 3 && x.JezykId == 1).Wpis == s6.Wpis);


        }



        [Fact(DisplayName = "Test sprawdzający poprawność kopiowania pola stringowego w rodzinie - tylko nullowe")]
        public void PrzetworzTest2()
        {
            Produkt p1 = new Produkt()
            {
                Id = 1,
                //opis = "",
                Rodzina = "Rodzina 1"
            };
            Produkt p2 = new Produkt()
            {
                Id = 2,
                Opis = "Opis 21",
                Rodzina = "Rodzina 1"
            };
            Produkt p3 = new Produkt()
            {
                Id = 3,
                Opis = "Opis 31",
                Rodzina = "Rodzina 2"
            };
            Produkt p4 = new Produkt()
            {
                Id = 4,
                Opis = "Opis 4",
                Rodzina = "Rodzina 2"
            };
            List<Produkt> listaProduktow = new List<Produkt>() { p1, p2, p3, p4 };




            Model.Tlumaczenie s1 = new Tlumaczenie()
            {

                ObiektId = 1,
                Wpis = "Opis produktu 1 po engliszu",
                Typ = "SolEx.Hurt.Model.Produkt",
                Pole = "opis",
                JezykId = 2
            };
            Model.Tlumaczenie s2 = new Tlumaczenie()
            {

                ObiektId = 1,
                Typ = "SolEx.Hurt.Model.Produkt",
                Pole = "opis",
                JezykId = 1
            };
            Model.Tlumaczenie s3 = new Tlumaczenie()
            {

                ObiektId = 2,
                Wpis = "Opis produktu 2 po engliszu",
                Typ = "SolEx.Hurt.Model.Produkt",
                Pole = "opis",
                JezykId = 2
            };
            Model.Tlumaczenie s4 = new Tlumaczenie()
            {

                ObiektId = 2,
                Typ = "SolEx.Hurt.Model.Produkt",
                Wpis = "Opis 21",
                Pole = "opis",
                JezykId = 1
            };


            Model.Tlumaczenie s5 = new Tlumaczenie()
            {

                ObiektId = 3,
                Wpis = "Opis produktu 3 po engliszu",
                Typ = "SolEx.Hurt.Model.Produkt",
                Pole = "opis",
                JezykId = 2
            };
            Model.Tlumaczenie s6 = new Tlumaczenie()
            {

                ObiektId = 3,
                Wpis = "Opis 31",
                Typ = "SolEx.Hurt.Model.Produkt",
                Pole = "opis",
                JezykId = 1
            };
            Model.Tlumaczenie s7 = new Tlumaczenie()
            {

                ObiektId = 4,
                Wpis = "Opis produktu 4 po engliszu",
                Typ = "SolEx.Hurt.Model.Produkt",
                Pole = "opis",
                JezykId = 2
            };
            Model.Tlumaczenie s8 = new Tlumaczenie()
            {

                ObiektId = 4,
                Typ = "SolEx.Hurt.Model.Produkt",
                Wpis = "Opis 4",
                Pole = "opis",
                JezykId = 1
            };

            List<Tlumaczenie> slowniki = new List<Tlumaczenie>() { s1, s2, s3, s4, s5,s7, s6, s8 };


            Dictionary<long, Produkt> produktyNaB2B = new Dictionary<long, Produkt>();
            List<JednostkaProduktu> jednostki = new List<JednostkaProduktu>();
            Dictionary<long, ProduktCecha> lacznikiCech = new Dictionary<long, ProduktCecha>();
            List<ProduktKategoria> lacznikiKategorii = new List<ProduktKategoria>();
            Model.Interfaces.Sync.ISyncProvider provider = null;
            List<ProduktUkryty> produktuUkryteErp = new List<ProduktUkryty>();
            List<ProduktyZamienniki> zamienniki = new List<ProduktyZamienniki>();
            Dictionary<long, KategoriaProduktu> kategorie = new Dictionary<long, KategoriaProduktu>();




            Dictionary<int, Jezyk> jezykiWSystemie = new Dictionary<int, Jezyk>();
            Jezyk j1 = new Jezyk() { Id = 1, Nazwa = "Polski" };
            Jezyk j2 = new Jezyk() { Id = 2, Nazwa = "Angielski" };
            jezykiWSystemie.Add(j1.Id, j1);
            jezykiWSystemie.Add(j2.Id, j2);



          

            var config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.JezykiWSystemie).Returns(jezykiWSystemie);

            KopiowanieOpisowWRodzinie modul = new KopiowanieOpisowWRodzinie();
               List<Cecha> cechy = new List<Cecha>();
            List<Atrybut> atrybuty = new List<Atrybut>();
           
            modul.Pola = new List<string>() { "opis" };
            modul.WymuszenieKopiowania = false;
            modul.config = config;
            modul.Przetworz(ref listaProduktow, ref slowniki, produktyNaB2B, ref jednostki, ref lacznikiCech, ref lacznikiKategorii, provider, ref produktuUkryteErp, ref zamienniki, kategorie ,ref cechy,ref atrybuty);
            Assert.True(slowniki.Count == jezykiWSystemie.Count * listaProduktow.Count);

            Assert.True(listaProduktow.First(x => x.Id == 1).Opis == p2.Opis);
            Assert.True(listaProduktow.First(x => x.Id == 2).Opis == p2.Opis);
            Assert.True(listaProduktow.First(x => x.Id == 3).Opis == p3.Opis);
            Assert.True(listaProduktow.First(x => x.Id == 4).Opis == p4.Opis);

            Assert.True(slowniki.First(x => x.ObiektId == 1 && x.JezykId == 2).Wpis == s3.Wpis);
            Assert.True(slowniki.First(x => x.ObiektId == 1 && x.JezykId == 1).Wpis == s4.Wpis);



        }





    }
}
