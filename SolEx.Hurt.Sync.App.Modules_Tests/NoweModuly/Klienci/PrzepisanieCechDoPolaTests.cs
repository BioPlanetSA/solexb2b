using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Web;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci;

using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci.Tests
{
    public class PrzepisanieCechDoPolaTests
    {
        [Fact()]
        public void PrzetworzTest()
        {
            PrzepisanieCechDoPola modul = new PrzepisanieCechDoPola();

            modul.Atrybut = "GLN";
            

            Klient k1 = new Klient();
            k1.Nazwa = "fajny klient";
            k1.Id = 1;

            Klient k2 = new Klient();
            k2.Nazwa = "inny mniej fajny klient";
            k2.Id = 2;

            modul.Pola = new List<string>() { "PoleTekst2" };

            Dictionary<long, Klient> listaKLientow = new Dictionary<long, Klient>();
            listaKLientow.Add(k1.Id, k1);
            listaKLientow.Add(k2.Id, k2);

            List<KupowaneIlosci> iloscii = new List<KupowaneIlosci>(0);

            Dictionary<Adres, KlientAdres> adresy = new Dictionary<Adres, KlientAdres>();

            KategoriaKlienta kategoria1 = new KategoriaKlienta();
            kategoria1.Grupa = "GRUPA1";
            kategoria1.Nazwa = "ASDASDAS";
            kategoria1.Id = 1;

            KategoriaKlienta kategoria2 = new KategoriaKlienta();
            kategoria2.Grupa = "GLN";
            kategoria2.Nazwa = "DUPECZKA";
            kategoria2.Id = 2;

            KategoriaKlienta kategoria3 = new KategoriaKlienta();
            kategoria3.Grupa = "SDFSDFSDFSDF";
            kategoria3.Nazwa = "LKJHLJKHKJH";
            kategoria3.Id = 3;
            List<Kraje> kraje = new List<Kraje>();
            List<Region> regiony = new List<Region>();
            List<KategoriaKlienta> kategorieKlientow = new List<KategoriaKlienta>();
            kategorieKlientow.Add(kategoria1);
            kategorieKlientow.Add(kategoria2);
            kategorieKlientow.Add(kategoria3);

            List<KlientKategoriaKlienta> laczniki = new List<KlientKategoriaKlienta>();
            laczniki.Add(new KlientKategoriaKlienta() { KlientId = k2.Id, KategoriaKlientaId = kategoria1.Id });
            laczniki.Add(new KlientKategoriaKlienta() { KlientId = k2.Id, KategoriaKlientaId = kategoria2.Id });
            laczniki.Add(new KlientKategoriaKlienta() { KlientId = k1.Id, KategoriaKlientaId = kategoria3.Id });
            List<Sklep> sklepy = new List<Sklep>();
            List<SklepKategoriaSklepu> sklepylaczniki = new List<SklepKategoriaSklepu>();
            List<KategoriaSklepu> sklepykategorie = new List<KategoriaSklepu>();

            List<Magazyn> magazyny = new List<Magazyn>();
            ISyncProvider provider = A.Fake<ISyncProvider>();

            ConfigBLL konfiguracjaFake = A.Fake<ConfigBLL>();
            A.CallTo(() => konfiguracjaFake.SeparatorGrupKlientow).Returns("_".ToCharArray());
            A.CallTo(() => konfiguracjaFake.CechaAuto).Returns("auto");

            modul.SyncManager = A.Fake<SyncManager>();
            A.CallTo( () => modul.SyncManager.Konfiguracja).Returns(konfiguracjaFake);

            modul.Przetworz(ref listaKLientow, new Dictionary<long, Produkt>(0), ref adresy, kategorieKlientow, laczniki, ref sklepy, ref sklepylaczniki, ref sklepykategorie, ref kraje, ref regiony, ref magazyny, provider);

            Assert.Null(k1.PoleTekst2);
            Assert.Equal(kategoria2.Nazwa, k2.PoleTekst2);
        }
    }
}
