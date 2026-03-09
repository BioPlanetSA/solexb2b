using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.PoziomyCenowe;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci.Tests
{
    public class DodanieKupowanychIlosciTests
    {
        StreamReader Dane()
        {
            string plik = @"Klient;Produkt;Ilosc
abc;cde;5";

            MemoryStream ms = new MemoryStream();
            byte[] dane = Encoding.UTF8.GetBytes(plik);
            ms.Write(dane, 0, dane.Length);
            ms.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(ms);
            return sr;
        }
        [Fact()]
        public void DodajDaneTest()
        {
        Test();
        }
        private void Test()
        {
          
            var modul = A.Fake<DodanieKupowanychIlosci>();
            modul.NazwaKolumnyIdentyfikator ="Produkt";
         modul.NazwaKolumnyKlient="Klient";
         modul.NazwaKolumnyIlosc="Ilosc";
            modul.PolaProdukt = "kod";
            modul.PolaKlient = "symbol";
            A.CallTo(() => modul.StrumienDanych()).Returns(Dane());
            Dictionary<long,KupowaneIlosci> ilosciKlientow=new Dictionary<long, KupowaneIlosci>();
            Dictionary<long, Klient> klienci = new Dictionary<long, Klient>();
            klienci.Add(1, new Klient { Id = 1, Symbol = "abc" });
            Dictionary<long,Produkt> produkty=new Dictionary<long, Produkt>();
              produkty.Add(1,new Produkt(1) { Kod = "xxx", Vat = 23 });
            produkty.Add(2,new Produkt(2) { Kod = "cde", Vat = 23 });
            DateTime? pocz = null, koniec = null;
            modul.Wykonaj(pocz, koniec, ilosciKlientow, klienci, produkty);
            Assert.True(ilosciKlientow.Count == 1);
            Assert.True(ilosciKlientow.Values.First().KlientId==1 );
            Assert.True(ilosciKlientow.Values.First().ProduktId ==2);
            Assert.True(ilosciKlientow.Values.First().Ilosc == 5);
        }
    }
}
