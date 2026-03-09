using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty.Tests
{
    public class PrzypisanieCechyNapodstaiwieCechyPlikBioplanetTests
    {
        StreamReader Dane()

        {
            string plik = @"ID;Nazwa;Nowa
2;BIOVERI (sezamki Croc Crac);Polska
3;SANTINI (ksylitol);chiny
2;BIOVERI (sezamki Croc Crac);Polska";

            MemoryStream ms = new MemoryStream();
            byte[] dane = Encoding.UTF8.GetBytes(plik);
            ms.Write(dane, 0, dane.Length);
            ms.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(ms);
            return sr;
        }
        [Fact()]
        public void PrzetworzTestGenerowanieCech()
        {
          
            var   modul=A.Fake< PrzypisanieCechyNapodstaiwieCechyPlikBioplanet>();
            A.CallTo(() => modul.StrumienDanych()).Returns(Dane());
            modul.Sciezka=@"C:\testy\grupy-kraje.csv";
            modul.Atrybut = "Testowy";
            modul.NowaCechaKolumna = "Nowa";
            List<Cecha> cechy=new List<Cecha>();
            List<Atrybut> atrybuty=new List<Atrybut>();
            Dictionary<long, Produkt> produktyNaB2B = new Dictionary<long, Produkt>();
            modul.Przetworz(ref atrybuty,ref cechy, produktyNaB2B);
            Assert.True(atrybuty.Count==1);
            Assert.True(cechy.Count==2);
        }

        [Fact()]
        public void PrzetworzTestPrzypisanieProduktom()
        {
            var modul = A.Fake<PrzypisanieCechyNapodstaiwieCechyPlikBioplanet>();
            A.CallTo(() => modul.StrumienDanych()).Returns(Dane());
            modul.Sciezka = @"C:\testy\grupy-kraje.csv";
            modul.Atrybut = "Testowy";
            modul.NowaCechaKolumna = "Nowa";
            modul.IstniejacaCechaKolumna = "ID";
            Dictionary<long, Produkt> produktyB2B = new Dictionary<long, Produkt>();
            List<ProduktUkryty> pu = new List<ProduktUkryty>();
           List< ProduktKategoria> produkty_kategorie = new List< ProduktKategoria>();

            List<ProduktyZamienniki> zamienniki = new List<ProduktyZamienniki>();
            List<Produkt> produkty = new List<Produkt>();
            produkty.Add(new Produkt { Id = 1 });
            produkty.Add(new Produkt { Id = 2 });

            List<JednostkaProduktu> jp = new List<JednostkaProduktu>();
            List<Tlumaczenie> slowniki = new List<Tlumaczenie>();
            Dictionary<long, ProduktCecha> cechyprodkty = new Dictionary<long, ProduktCecha>();
            var cp = new ProduktCecha() {ProduktId = 1, CechaId = 2};
            cechyprodkty.Add(cp.Id, cp);
               List<Cecha> cechy = new List<Cecha>();
            List<Atrybut> atrybuty = new List<Atrybut>();
           

            modul.Przetworz(ref produkty, ref slowniki, produktyB2B, ref jp, ref cechyprodkty, ref produkty_kategorie, null, ref pu, ref zamienniki, new Dictionary<long, KategoriaProduktu>() ,ref cechy,ref atrybuty);
            Assert.True(cechyprodkty.Count(x => x.Value.ProduktId == 1 && x.Value.CechaId!=2)==1);
        }
        [Fact()]
        public void PrzetworzTestPrzypisanieProduktomWydajnosc()
        {
            var modul = A.Fake<PrzypisanieCechyNapodstaiwieCechyPlikBioplanet>();
            A.CallTo(() => modul.StrumienDanych()).Returns(Dane());
            modul.Sciezka = @"C:\testy\grupy-kraje.csv";
            modul.Atrybut = "Testowy";
            modul.NowaCechaKolumna = "Nowa";
            modul.IstniejacaCechaKolumna = "ID";
            Dictionary<long, Produkt> produktyB2B = new Dictionary<long, Produkt>();
            List<ProduktUkryty> pu = new List<ProduktUkryty>();
           List< ProduktKategoria> produkty_kategorie = new List< ProduktKategoria>();
            Dictionary<long, ProduktCecha> cechyprodkty = new Dictionary<long, ProduktCecha>();
            List<ProduktyZamienniki> zamienniki = new List<ProduktyZamienniki>();
            List<Produkt> produkty = new List<Produkt>();
            int ileproduktow = 100000;
            for (int i = 0; i < ileproduktow; i++)
            {
                produkty.Add(new Produkt { Id = i + 1 });
                var cp = new ProduktCecha() {ProduktId = i + 1, CechaId = 1 + i%2};
                cechyprodkty.Add(cp.Id, cp);
             
            }
           

            List<JednostkaProduktu> jp = new List<JednostkaProduktu>();
            List<Tlumaczenie> slowniki = new List<Tlumaczenie>();

               List<Cecha> cechy = new List<Cecha>();
            List<Atrybut> atrybuty = new List<Atrybut>();
            
            Stopwatch stoper = new Stopwatch();
            stoper.Start();
            modul.Przetworz(ref produkty, ref slowniki, produktyB2B, ref jp, ref cechyprodkty, ref produkty_kategorie, null, ref pu, ref zamienniki, new Dictionary<long, KategoriaProduktu>(),ref cechy,ref atrybuty);
            stoper.Stop();
      
            Assert.True(cechyprodkty.Count(x => x.Value.ProduktId == 1 && x.Value.CechaId != 2) == 1);
            Assert.True(stoper.Elapsed.TotalSeconds / ileproduktow < 1, string.Format("sredni czas {0} ms", stoper.Elapsed.TotalSeconds / ileproduktow));


        }
    }
}
