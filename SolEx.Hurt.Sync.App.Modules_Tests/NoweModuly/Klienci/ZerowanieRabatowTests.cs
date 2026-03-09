using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci.Tests
{
    public class ZerowanieRabatowTests
    {
        [Fact(DisplayName = "Zerowanie rabatów klientów na podstawie wartości pola")]
        public void PrzetworzTest()
        {
            ZerowanieRabatow modul = new ZerowanieRabatow();
         modul.Pole = "pole_tekst1";
         modul.Wartosc = "PROMOCJE";
         modul.Porowanie=Wartosc.Rowne;
         Klient klient1 = new Klient(1) { Nazwa = "n1", Aktywny = true, Rabat = 5, PoleTekst1 = "5%" };
         Klient klient2 = new Klient(2) { Nazwa = "n2", Aktywny = true, Rabat = 5, PoleTekst1 = "PROMOCJE" };
         Klient klient3 = new Klient(3) { Nazwa = "n3", Aktywny = true, Rabat = 5, PoleTekst1 = "Promocje" };
         Dictionary<Adres, KlientAdres> adresy = new Dictionary<Adres, KlientAdres>();
         
            List<KupowaneIlosci> ilosci = new List<KupowaneIlosci>();
            List<Sklep> sklepy = new List<Sklep>();
            List<SklepKategoriaSklepu> sklepylaczniki = new List<SklepKategoriaSklepu>();
            List<KategoriaSklepu> sklepykategorie = new List<KategoriaSklepu>();
            List<Kraje> kraje = new List<Kraje>();
            List<Region> regiony = new List<Region>();
            Dictionary<long, Klient> slownikKlienci = new Dictionary<long, Klient>();
            slownikKlienci.Add(klient1.Id, klient1);
            slownikKlienci.Add(klient2.Id, klient2);
            slownikKlienci.Add(klient3.Id, klient3);

            List<KlientKategoriaKlienta> lacznik = new List<KlientKategoriaKlienta>();
            List<KategoriaKlienta> kateorie = new List<KategoriaKlienta>();

            List<Magazyn> magazyny = new List<Magazyn>();
            ISyncProvider provider = A.Fake<ISyncProvider>();
            modul.Przetworz(ref slownikKlienci, new Dictionary<long, Produkt>(), ref adresy, kateorie, lacznik, ref sklepy, ref sklepylaczniki, ref sklepykategorie, ref kraje, ref regiony, ref magazyny, provider);

            Assert.Equal(5, klient1.Rabat);
            Assert.Equal(0, klient2.Rabat);
            Assert.Equal(5, klient3.Rabat);
        }
    }
}
