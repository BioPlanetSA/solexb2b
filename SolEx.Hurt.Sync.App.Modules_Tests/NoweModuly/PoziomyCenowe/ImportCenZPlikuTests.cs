using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.PoziomyCenowe;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.PoziomyCenowe.Tests
{
    public class ImportCenZPlikuTests
    {
        StreamReader Dane()
        {
            string plik = @"EAN;CENA DETALICZNA BRUTTO
5907814662682;12.3";

            MemoryStream ms = new MemoryStream();
            byte[] dane = Encoding.UTF8.GetBytes(plik);
            ms.Write(dane, 0, dane.Length);
            ms.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(ms);
            return sr;
        }
        [Fact()]
        public void ImportCenZPlikuTest()
        {
            Test(RodzajCeny.Netto,12.3M);
            Test(RodzajCeny.Brutto, 10);
        }

        private void Test(RodzajCeny rodzajCeny,decimal oczekiwana)
        {
            List<Produkt> produkty = new List<Produkt>();
            produkty.Add(new Produkt(1) { KodKreskowy = "5907814662682",Vat=23 });
            produkty.Add(new Produkt(2) { KodKreskowy = "5907814668929", Vat = 23 });
            var modul = A.Fake<ImportCenZPliku>();
            A.CallTo(() => modul.StrumienDanych()).Returns(Dane());
            A.CallTo(() => modul.Produkty).Returns(produkty);
            modul.Sciezka = @"C:\testy\grupy-kraje.csv";
            modul.NazwaKulumnyCena = "CENA DETALICZNA BRUTTO";
            modul.NazwaKulumnyIdentyfikator = "EAN";
            modul.Pola = "kod_kreskowy";
            modul.WalutaPoziomuCenowego = "PLN";
            modul.NazwaPoziomuCenowego = "Imporotwany";
            modul.RodzajCeny = rodzajCeny;
            Dictionary<int, PoziomCenowy> pc = new Dictionary<int, PoziomCenowy>();
            Dictionary<int, CenaPoziomu> cp = new Dictionary<int, CenaPoziomu>();
            Dictionary<int,PoziomCenowy> pcb2b=new Dictionary<int, PoziomCenowy>();
            List<CenaPoziomu> wynik = new List<CenaPoziomu>();
            Dictionary<long, CenaPoziomu> cpb2b = new Dictionary<long, CenaPoziomu>();
            modul.Przetworz(ref pc, ref wynik, pcb2b, cpb2b);
            Assert.True(pc.Values.Count(x => x.Nazwa == modul.NazwaPoziomuCenowego && x.WalutaId == modul.WalutaPoziomuCenowego.ToLower().WygenerujIDObiektuSHAWersjaLong()) == 1);
            Assert.True(wynik.Count == 1);
            Assert.True(wynik.Any(x => x.ProduktId == 1 && x.WalutaId == modul.WalutaPoziomuCenowego.ToLower().WygenerujIDObiektuSHAWersjaLong() && x.Netto == oczekiwana), string.Format("Rodzaj {0} wartosc {1}", rodzajCeny, oczekiwana));
        }
    }
}
