using System;
using System.Collections.Generic;
using FakeItEasy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces.Sync;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci.Tests
{
    public class RoleKlientowTests
    {
        [Fact()]
        public void RoleKlientowPrzetworzTest()
        {
            Klient klient1 = new Klient(1);
            Klient klient2 = new Klient(2);
            Klient klient3 = new Klient(3);

            RoleKlientow modul = new RoleKlientow();
            modul.KategoriaKlienta= new HashSet<int>(){1,2};


            Dictionary<Adres, KlientAdres> adresy = new Dictionary<Adres, KlientAdres>();
            List<KupowaneIlosci> ilosci = new List<KupowaneIlosci>();
            List<Sklep> sklepy = new List<Sklep>();
            List<SklepKategoriaSklepu> sklepylaczniki = new List<SklepKategoriaSklepu>();
            List<KategoriaSklepu> sklepykategorie = new List<KategoriaSklepu>();

            KategoriaKlienta k1 = new KategoriaKlienta() { Id = 1, Nazwa = "nazwa1" };
            KategoriaKlienta k2 = new KategoriaKlienta() { Id = 2, Nazwa = "nazwa2" };
            KategoriaKlienta k3 = new KategoriaKlienta() { Id = 3, Nazwa = "nazwa3" };
            List<KategoriaKlienta> listaKategorii = new List<KategoriaKlienta>() { k1, k2,k3 };


            KlientKategoriaKlienta kk1 = new KlientKategoriaKlienta() { KlientId = 1, KategoriaKlientaId = 1};
            KlientKategoriaKlienta kk2 = new KlientKategoriaKlienta() { KlientId = 1, KategoriaKlientaId = 2 };

            KlientKategoriaKlienta kk3 = new KlientKategoriaKlienta() {  KlientId = 2, KategoriaKlientaId = 1 };
            KlientKategoriaKlienta kk4 = new KlientKategoriaKlienta() {  KlientId = 2, KategoriaKlientaId = 5 };

            KlientKategoriaKlienta kk5 = new KlientKategoriaKlienta() {  KlientId = 3, KategoriaKlientaId = 7 };
            KlientKategoriaKlienta kk6 = new KlientKategoriaKlienta() { KlientId = 3, KategoriaKlientaId = 8 };


            List<KlientKategoriaKlienta> listaKlienciKategorie = new List<KlientKategoriaKlienta>(){kk1,kk2,kk3,kk4,kk5,kk6};


            Dictionary<long, Klient> slownikKlienci = new Dictionary<long, Klient>();
            slownikKlienci.Add(klient1.Id, klient1);
            slownikKlienci.Add(klient2.Id, klient2);
            slownikKlienci.Add(klient3.Id, klient3);

            List<Kraje> kraje = new List<Kraje>();
            List<Region> regiony = new List<Region>();

            List<Magazyn> magazyny = new List<Magazyn>();
            ISyncProvider provider = A.Fake<ISyncProvider>();
            modul.RolaKlienta = RoleType.Pracownik;

            modul.Przetworz(ref slownikKlienci, new Dictionary<long, Produkt>(), ref adresy,
                listaKategorii, listaKlienciKategorie, ref sklepy, ref sklepylaczniki, ref sklepykategorie, ref kraje, ref regiony, ref magazyny, provider);

            Assert.True(slownikKlienci[klient1.Id].Role.Contains(RoleType.Pracownik));
            Assert.True(slownikKlienci[klient2.Id].Role.Contains(RoleType.Pracownik));
            Assert.True(!slownikKlienci[klient3.Id].Role.Contains(RoleType.Pracownik));

            modul.KategoriaKlienta = null;
            modul.RolaKlienta = RoleType.Przedstawiciel;
            try
            {
                modul.Przetworz(ref slownikKlienci, new Dictionary<long, Produkt>(), ref adresy,listaKategorii, listaKlienciKategorie, ref sklepy, ref sklepylaczniki, ref sklepykategorie, ref kraje, ref regiony, ref magazyny, provider);
                Assert.True(false);
            }catch(NullReferenceException)
            {
              
            }
            modul.KategoriaKlienta = new HashSet<int>() { 20 };
            modul.RolaKlienta = RoleType.Przedstawiciel;
            try
            {
                modul.Przetworz(ref slownikKlienci, new Dictionary<long, Produkt>(), ref adresy, listaKategorii, listaKlienciKategorie, ref sklepy, ref sklepylaczniki, ref sklepykategorie, ref kraje, ref regiony, ref magazyny, provider);
                Assert.True(false);
            }
            catch (InvalidOperationException)
            {

            }

            

        }
    }
}
