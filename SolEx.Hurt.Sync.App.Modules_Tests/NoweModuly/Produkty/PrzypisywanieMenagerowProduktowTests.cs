using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty.Tests
{
    public class PrzypisywanieMenagerowProduktowTests
    {
        [Fact()]
        public void PrzetworzTest()
        {
            Test1();
            Test2();
        }

        private void Test2()
        {
            Produkt produkt = new Produkt();
            produkt.Id = 1;
            List<Produkt> produkty = new List<Produkt> { produkt };
            var przedstawiciel1 = new Klient();
            przedstawiciel1.Nazwa = "pierwszy przedstawiciel klienta";
            przedstawiciel1.Symbol = "P1";
            przedstawiciel1.Email = "p1@p1.pl";
            przedstawiciel1.Id = 111;
            var pracownicy = new List<Klient> { przedstawiciel1 };
            Cecha cecha = new Cecha();
            cecha.Id = 2;
            cecha.Nazwa = "$"+przedstawiciel1.Symbol;
            cecha.Symbol = "--auto--:" + cecha.Nazwa;
            List<Cecha> cechy = new List<Cecha> { cecha };
            var modul = A.Fake<PrzypisywanieMenagerowProduktow>();
            A.CallTo(() => modul.PracownicyNaPlatformie).Returns(pracownicy);
            A.CallTo(() => modul.CechyNaPlatformie).Returns(cechy);
            modul.PoczatekCechy = "--auto--:$";
            modul.Pole = "symbol";
            List<ProduktyZamienniki> TODO=new List<ProduktyZamienniki>();
            ProduktCecha cp = new ProduktCecha() { CechaId = cecha.Id, ProduktId = produkt.Id };
            Dictionary<long, ProduktCecha> lacznikcech = new Dictionary<long, ProduktCecha> { { cp.Id, cp } };
            List<Tlumaczenie> slownik = new List<Tlumaczenie>();
            List<JednostkaProduktu> jednostki = new List<JednostkaProduktu>();
            Dictionary<long, Produkt> pdic = new Dictionary<long, Produkt>();
           List< ProduktKategoria> pkatnew = new List< ProduktKategoria>();
            List<ProduktUkryty> pu = new List<ProduktUkryty>();
            List<Atrybut> atrybuty = new List<Atrybut>();
           
            modul.Przetworz(ref produkty, ref slownik, pdic, ref jednostki, ref lacznikcech, ref pkatnew, null, ref pu, ref TODO, new Dictionary<long, KategoriaProduktu>() ,ref cechy,ref atrybuty);

            Assert.Equal(produkt.MenagerId, przedstawiciel1.Id);
        }

        private void Test1()
        {
            Produkt produkt = new Produkt();
            produkt.Id = 1;
            List<Produkt> produkty = new List<Produkt> { produkt };
            var przedstawiciel1 = new Klient();
            przedstawiciel1.Nazwa = "pierwszy przedstawiciel klienta";
            przedstawiciel1.Symbol = "P1";
            przedstawiciel1.Email = "p1@p1.pl";
            przedstawiciel1.Id = 111;
            var pracownicy = new List<Klient> { przedstawiciel1 };
            Cecha cecha = new Cecha();
            cecha.Id = 2;
            cecha.Nazwa = przedstawiciel1.Symbol;
            cecha.Symbol = "$:" + cecha.Nazwa;
            List<Cecha> cechy = new List<Cecha> { cecha };
            var modul = A.Fake<PrzypisywanieMenagerowProduktow>();
            A.CallTo(() => modul.PracownicyNaPlatformie).Returns(pracownicy);
            A.CallTo(() => modul.CechyNaPlatformie).Returns(cechy);
            modul.PoczatekCechy = "$:";
            modul.Pole = "symbol";
            List<ProduktyZamienniki> TODO = new List<ProduktyZamienniki>();
            ProduktCecha cp = new ProduktCecha() { CechaId = cecha.Id, ProduktId = produkt.Id };
            Dictionary<long, ProduktCecha> lacznikcech = new Dictionary<long, ProduktCecha> { { cp.Id, cp } };
            List<Tlumaczenie> slownik = new List<Tlumaczenie>();
            List<JednostkaProduktu> jednostki = new List<JednostkaProduktu>();
            Dictionary<long, Produkt> pdic = new Dictionary<long, Produkt>();
           List< ProduktKategoria> pkatnew = new List< ProduktKategoria>();
            List<ProduktUkryty> pu = new List<ProduktUkryty>();   
            List<Atrybut> atrybuty = new List<Atrybut>();
            
            modul.Przetworz(ref produkty, ref slownik, pdic, ref jednostki, ref lacznikcech, ref pkatnew, null, ref pu, ref TODO, new Dictionary<long, KategoriaProduktu>(),ref cechy,ref atrybuty);

            Assert.Equal(produkt.MenagerId, przedstawiciel1.Id);
        }
    }
}
