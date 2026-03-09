using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci.Tests
{
    public class PoziomCenowyNaPodstawieKategoriiTests
    {
        [Fact(DisplayName = "Test sprawdzający poprawność dziłania modułu przypisującego określony poziom cenowy do klientów posiadających wybraną kategorię")]
        public void PrzetworzTest()
        {
            Klient k1 = new Klient() { Id = 1, PoziomCenowyId = 1 };
            Klient k2 = new Klient() { Id = 2, PoziomCenowyId = 1 };
            Klient k3 = new Klient() { Id = 3, PoziomCenowyId = 1 };
            Klient k4 = new Klient() { Id = 4, PoziomCenowyId = 1 };
            Klient k5 = new Klient() { Id = 5, PoziomCenowyId = 1 };
            Klient k6 = new Klient() { Id = 6, PoziomCenowyId = 1 };

            Dictionary<long, Klient> slownikKlientow = new Dictionary<long, Klient>();
            slownikKlientow.Add(k1.Id, k1);
            slownikKlientow.Add(k2.Id, k2);
            slownikKlientow.Add(k3.Id, k3);
            slownikKlientow.Add(k4.Id, k4);
            slownikKlientow.Add(k5.Id, k5);
            slownikKlientow.Add(k6.Id, k6);

            KlientKategoriaKlienta kk1 = new KlientKategoriaKlienta(){KategoriaKlientaId = 1, KlientId = 1};
            KlientKategoriaKlienta kk2 = new KlientKategoriaKlienta() { KategoriaKlientaId = 1, KlientId = 2 };
            KlientKategoriaKlienta kk3 = new KlientKategoriaKlienta() { KategoriaKlientaId = 2, KlientId = 3 };
            KlientKategoriaKlienta kk4 = new KlientKategoriaKlienta() { KategoriaKlientaId = 2, KlientId = 4 };
            KlientKategoriaKlienta kk5 = new KlientKategoriaKlienta() { KategoriaKlientaId = 3, KlientId = 5 };
            KlientKategoriaKlienta kk6 = new KlientKategoriaKlienta() { KategoriaKlientaId = 3, KlientId = 6 };
            List<KlientKategoriaKlienta> laczniki = new List<KlientKategoriaKlienta>(){kk1,kk2,kk3,kk4,kk5,kk6};


            List<KupowaneIlosci> iloscii = new List<KupowaneIlosci>();
            Dictionary<Adres, KlientAdres> adresyWErp = new Dictionary<Adres, KlientAdres>();
            List<KategoriaKlienta> kategorie=new List<KategoriaKlienta>();
            List<Sklep> sklepy=new List<Sklep>();
            List<SklepKategoriaSklepu> sklpeylaczniki = new List<SklepKategoriaSklepu>();
            List<KategoriaSklepu> sklepyKategorie = new List<KategoriaSklepu>();
            List<Kraje> kraje = new List<Kraje>();
            List<Region> regiony= new List<Region>();



            PoziomCenowyNaPodstawieKategorii modul = new PoziomCenowyNaPodstawieKategorii();
            modul.ListaKategoriiKlienta = new List<int>(){1,3};
            modul.PoziomCenowy = 69;

            List<Magazyn> magazyny = new List<Magazyn>();
            ISyncProvider provider = A.Fake<ISyncProvider>();
            modul.Przetworz(ref slownikKlientow, new Dictionary<long, Produkt>(), ref adresyWErp,kategorie,laczniki,ref sklepy,ref sklpeylaczniki,ref sklepyKategorie, ref kraje, ref regiony, ref magazyny, provider );

            Assert.True(slownikKlientow[k1.Id].PoziomCenowyId.Value == 69);
            Assert.True(slownikKlientow[k2.Id].PoziomCenowyId.Value == 69);
            Assert.True(slownikKlientow[k3.Id].PoziomCenowyId.Value == 1);
            Assert.True(slownikKlientow[k4.Id].PoziomCenowyId.Value == 1);
            Assert.True(slownikKlientow[k5.Id].PoziomCenowyId.Value == 69);
            Assert.True(slownikKlientow[k6.Id].PoziomCenowyId.Value == 69);

        }
    }
}
