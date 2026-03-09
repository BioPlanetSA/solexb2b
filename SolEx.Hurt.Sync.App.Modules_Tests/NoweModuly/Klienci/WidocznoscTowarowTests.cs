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
    public class WidocznoscTowarowTests
    {
        [Fact(DisplayName = "Widocznosc towaru dla klientow")]
        public void PrzetworzTest()
        {
         TestW(false,true,WidocznoscTowarow.Widocznosc.PelnaOferta);
         TestW(true, true, WidocznoscTowarow.Widocznosc.PelnaOferta);
         TestW(false, false, WidocznoscTowarow.Widocznosc.TylkoMojKatalog);
         TestW(true, false, WidocznoscTowarow.Widocznosc.TylkoMojKatalog); 
        }

        void TestW(bool poczatkowy, bool oczekiwany, WidocznoscTowarow.Widocznosc rodzaj)
        {
            WidocznoscTowarow wid = new WidocznoscTowarow();
            wid.TypWidocznosci = rodzaj;

            Klient klient = new Klient(1);
            Dictionary<long, Klient> slowKlient = new Dictionary<long, Klient>();
            Dictionary<long, Produkt> produktyB2B = new Dictionary<long, Produkt>();
            List<KupowaneIlosci> iloscii = new List<KupowaneIlosci>();
            Dictionary<Adres, KlientAdres> adresyWErp = new Dictionary<Adres, KlientAdres>();
            List<KategoriaKlienta> kategorie = new List<KategoriaKlienta>();
            List<KlientKategoriaKlienta> laczniki = new List<KlientKategoriaKlienta>();
            List<Sklep> sklepy = new List<Sklep>();
            List<SklepKategoriaSklepu> sklepylaczniki = new List<SklepKategoriaSklepu>();
            List<KategoriaSklepu> sklepyKategorie = new List<KategoriaSklepu>();

            List<Kraje> kraje = new List<Kraje>();
            List<Region> regiony = new List<Region>();

            List<Magazyn> magazyny = new List<Magazyn>();
            ISyncProvider provider = A.Fake<ISyncProvider>();
            klient.PelnaOferta = poczatkowy;

            slowKlient.Add(1, klient);

            wid.Przetworz(ref slowKlient, produktyB2B, ref adresyWErp, kategorie, laczniki, ref sklepy, ref sklepylaczniki, ref sklepyKategorie, ref kraje, ref regiony, ref magazyny, provider);

            Assert.True(oczekiwany==klient.PelnaOferta, "not implemented yet");
        }
    }
}
