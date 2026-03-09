using System.Collections.Generic;
using FakeItEasy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces.Sync;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci.Tests
{
    public class PoziomCenyDetalicznejNaPodstawiePoziomuHurtowegoTests
    {
        [Fact(DisplayName = "Test ustawiania poziomu cen detalicznych")]
        public void PrzetworzTest()
        {
            PoziomCenyDetalicznejNaPodstawiePoziomuHurtowego modul = new PoziomCenyDetalicznejNaPodstawiePoziomuHurtowego();

            Dictionary<long, Klient> slownikKlientow = new Dictionary<long, Klient>();
            Klient k1 = new Klient(null) { Id = 1, Aktywny = true, PoziomCenowyId = 1 };
            Klient k2 = new Klient(null) { Id = 2, Aktywny = true, PoziomCenowyId = 2 };
            Klient k3 = new Klient(null) { Id = 3, Aktywny = true };
            Klient k4 = new Klient(null) { Id = 4, Aktywny = true };
            Klient k5 = new Klient(null) { Id = 5, Aktywny = true };
            slownikKlientow.Add(k1.Id, k1);
            slownikKlientow.Add(k2.Id, k2);
            slownikKlientow.Add(k3.Id, k3);
            slownikKlientow.Add(k4.Id, k4);
            slownikKlientow.Add(k5.Id, k5);


            List<KupowaneIlosci> iloscii = new List<KupowaneIlosci>();
            Dictionary<Adres, KlientAdres> adresyWErp = new Dictionary<Adres, KlientAdres>();
            List<Sklep> sklepy = new List<Sklep>();
            List<SklepKategoriaSklepu> sklpeylaczniki = new List<SklepKategoriaSklepu>();
            List<KategoriaSklepu> sklepyKategorie = new List<KategoriaSklepu>();
            List<Kraje> kraje = new List<Kraje>();
            List<Region> regiony = new List<Region>();
            modul.PoziomHurtowy = 1;
            modul.PoziomDetaliczny = 2;

            List<Magazyn> magazyny = new List<Magazyn>();
            ISyncProvider provider = A.Fake<ISyncProvider>();
            modul.Przetworz(ref slownikKlientow, new Dictionary<long, Produkt>(), ref adresyWErp, new List<KategoriaKlienta>(), new List<KlientKategoriaKlienta>(), ref sklepy, ref sklpeylaczniki, ref sklepyKategorie, ref kraje, ref regiony, ref magazyny, provider);
            Assert.Equal(2, slownikKlientow[1].CenaDetalicznaPoziomID);
            Assert.Null( slownikKlientow[2].CenaDetalicznaPoziomID);
            Assert.Null(slownikKlientow[3].CenaDetalicznaPoziomID);
        }
    }
}
