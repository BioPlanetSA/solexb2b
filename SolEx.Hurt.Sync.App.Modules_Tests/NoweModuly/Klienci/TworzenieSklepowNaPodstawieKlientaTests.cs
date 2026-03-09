using System.Collections.Generic;
using FakeItEasy;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Interfaces.Sync;

using Xunit;
using Adres = SolEx.Hurt.Model.Adres;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci.Tests
{
    public class TworzenieSklepowNaPodstawieKlientaTests
    {
        [Fact(DisplayName = "Test modułu tworzenia sklepów na podstawie klienta")]
        public void PrzetworzTest()
        {
            IConfigBLL config = A.Fake<IConfigBLL>();
            ILogiFormatki logi = A.Fake<ILogiFormatki>();
            var modul = A.Fake<TworzenieSklepowNaPodstawieKlienta>();
            modul.Config = config;
            modul.LogiFormatki = logi;
            modul.KategoriaSklepow = "1";
            modul.KategoriaKlienta = new HashSet<int>() { 1 };
            A.CallTo(() => modul.KategorieB2B).Returns(new List<KategoriaSklepu> { new KategoriaSklepu() { Id = 1, Nazwa = modul.KategoriaSklepow } });
            Klient klient1 = new Klient(1) { Nazwa = "n1", Aktywny = true };
            Klient klient2 = new Klient(2) { Nazwa = "n2", Aktywny = true };
            Klient klient3 = new Klient(3) { Nazwa = "n3", Aktywny = true };
            Dictionary<Adres, KlientAdres> adresy = new Dictionary<Adres, KlientAdres>();
            adresy.Add(new Model.Adres { Id = 1,  Miasto = "miasto1", Telefon = "" }, new KlientAdres(){KlientId = 1, AdresId = 1, TypAdresu = TypAdresu.Glowny});
            adresy.Add(new Model.Adres { Id = 2, Miasto = "miasto1", Telefon = "" }, new KlientAdres() { KlientId = 1, AdresId = 1, TypAdresu = TypAdresu.Glowny });
            adresy.Add(new Model.Adres { Id = 3, Miasto = "miasto1", Telefon = "" }, new KlientAdres() { KlientId = 1, AdresId = 1, TypAdresu = TypAdresu.Glowny });
            List<KupowaneIlosci> ilosci = new List<KupowaneIlosci>();
            List<Sklep> sklepy = new List<Sklep>();
            List<SklepKategoriaSklepu> sklepylaczniki = new List<SklepKategoriaSklepu>();
            List<KategoriaSklepu> sklepykategorie = new List<KategoriaSklepu>();

            Dictionary<long, Klient> slownikKlienci = new Dictionary<long, Klient>();
            slownikKlienci.Add(klient1.Id, klient1);
            slownikKlienci.Add(klient2.Id, klient2);
            slownikKlienci.Add(klient3.Id, klient3);
            List<Kraje> kraje = new List<Kraje>();
            List<Region> regiony = new List<Region>();
            List<Magazyn> magazyny = new List<Magazyn>();
            ISyncProvider provider = A.Fake<ISyncProvider>();
            List<KlientKategoriaKlienta> lacznik = new List<KlientKategoriaKlienta>();
            lacznik.Add(new KlientKategoriaKlienta{KlientId = 1,KategoriaKlientaId = 1});
            lacznik.Add(new KlientKategoriaKlienta{KlientId = 2,KategoriaKlientaId = 1});
            lacznik.Add(new KlientKategoriaKlienta { KlientId = 2, KategoriaKlientaId =2 });
            lacznik.Add(new KlientKategoriaKlienta { KlientId =3, KategoriaKlientaId = 2 });
            List<KategoriaKlienta> kateorie = new List<KategoriaKlienta>();
            kateorie.Add(new KategoriaKlienta() { Nazwa = "kat", Id = 1, Grupa = "pierwsza" });
            kateorie.Add(new KategoriaKlienta() { Nazwa = "kat", Id = 2, Grupa = "druga" });
            modul.Przetworz(ref slownikKlienci, new Dictionary<long, Produkt>(), ref adresy, kateorie, lacznik, ref sklepy, ref sklepylaczniki, ref sklepykategorie, ref kraje, ref regiony,ref magazyny,provider);

            Assert.True(sklepy.Count==2,"Liczba sklpeów");
            Assert.True(sklepylaczniki.Count == 2, "Liczba łączników");
        }
        [Fact(DisplayName = "Test modułu tworzenia sklepów na podstawie klienta kiedy są aktywne 2 takie moduły")]
        public void PrzetworzTestGdyDwaModuly()
        {
            IConfigBLL config = A.Fake<IConfigBLL>();
            ILogiFormatki logi = A.Fake<ILogiFormatki>();

            List<Magazyn> magazyny = new List<Magazyn>();
            ISyncProvider provider = A.Fake<ISyncProvider>();
            var modul1 = A.Fake<TworzenieSklepowNaPodstawieKlienta>();
            modul1.Config = config;
            modul1.LogiFormatki = logi;
            modul1.KategoriaSklepow = "1";
            modul1.KategoriaKlienta = new HashSet<int>() {1};
            A.CallTo(() => modul1.KategorieB2B).Returns(new List<KategoriaSklepu> { new KategoriaSklepu() { Id = 1, Nazwa = modul1.KategoriaSklepow } });
            Klient klient1 = new Klient(1) { Nazwa = "n1", Aktywny = true };
            Klient klient2 = new Klient(2) { Nazwa = "n2", Aktywny = true };
            Klient klient3 = new Klient(3) { Nazwa = "n3", Aktywny = true };
            Dictionary<Adres, KlientAdres> adresy = new Dictionary<Adres, KlientAdres>();
            adresy.Add(new Model.Adres { Id = 1, Miasto = "miasto1", Telefon = "" }, new KlientAdres() { KlientId = 1, AdresId = 1, TypAdresu = TypAdresu.Glowny });
            adresy.Add(new Model.Adres { Id = 2, Miasto = "miasto1", Telefon = "" }, new KlientAdres() { KlientId = 1, AdresId = 1, TypAdresu = TypAdresu.Glowny });
            adresy.Add(new Model.Adres { Id = 3, Miasto = "miasto1", Telefon = "" }, new KlientAdres() { KlientId = 1, AdresId = 1, TypAdresu = TypAdresu.Glowny });
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
            lacznik.Add(new KlientKategoriaKlienta { KlientId = 1, KategoriaKlientaId = 1 });
            lacznik.Add(new KlientKategoriaKlienta { KlientId = 2, KategoriaKlientaId = 1 });
            lacznik.Add(new KlientKategoriaKlienta { KlientId = 2, KategoriaKlientaId = 2 });
            lacznik.Add(new KlientKategoriaKlienta { KlientId = 3, KategoriaKlientaId = 2 });
            List<KategoriaKlienta> kateorie = new List<KategoriaKlienta>();
            kateorie.Add(new KategoriaKlienta() { Nazwa = "kat", Id = 1, Grupa = "pierwsza" });
            kateorie.Add(new KategoriaKlienta() { Nazwa = "kat", Id = 2, Grupa = "druga" });
            modul1.Przetworz(ref slownikKlienci, new Dictionary<long, Produkt>(), ref adresy, kateorie, lacznik, ref sklepy, ref sklepylaczniki, ref sklepykategorie ,ref kraje,ref regiony,ref magazyny,provider);

            var modul2 = A.Fake<TworzenieSklepowNaPodstawieKlienta>();
            modul2.KategoriaSklepow = "2";
            modul2.KategoriaKlienta = new HashSet<int>() { 2 };
            modul2.Config = config;
            modul2.LogiFormatki = logi;
            A.CallTo(() => modul2.KategorieB2B).Returns(new List<KategoriaSklepu> { new KategoriaSklepu() { Id = 2, Nazwa = modul2.KategoriaSklepow } });
            
            modul2.Przetworz(ref slownikKlienci, new Dictionary<long, Produkt>(), ref adresy, kateorie, lacznik, ref sklepy, ref sklepylaczniki, ref sklepykategorie, ref kraje, ref regiony, ref magazyny,provider);
            Assert.True(sklepy.Count == 3, "Liczba sklpeów");
            Assert.True(sklepylaczniki.Count == 4, "Liczba łączników");
        }

        [Fact(DisplayName = "Test metody pobierającej koordynaty z api google dla wybranego sklepu")]
        public void SprawdzKoordynaty()
        {
            SklepyBll sklep = new SklepyBll();
            Model.Adres adres = new Model.Adres() { Id = 1, UlicaNr = "verviers", KodPocztowy = "4800", Miasto = "verviers" };
            sklep.AdresId = adres.Id;
            A.CallTo(() => sklep.UlicaNr).Returns(adres.UlicaNr);
            A.CallTo(() => sklep.KodPocztowy).Returns(adres.KodPocztowy);
            A.CallTo(() => sklep.Miasto).Returns(adres.Miasto);
            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();

            Sklepy s = new Sklepy(calosc);

            decimal lat;
            decimal lon;

            s.LocationGeoCode(adres, out lat, out lon);
            Assert.True(lat != 0 && lon != 0);
        }
    }
}
