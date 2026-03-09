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
    public class DoklejeniePolaDoPolaTests
    {
        [Fact(DisplayName = "Moduł DoklejeniePolaDoPola - test doklejania pola symbol do pola email")]
        public void PrzetworzTest()
        {

            Dictionary<long, Klient> kliencidotestu = WygenerujKlientow();
            Dictionary<long, Klient> kliencidoporownania = WygenerujKlientow();
            List<KupowaneIlosci> kupowaneilosci = new List<KupowaneIlosci>(0);
            Dictionary<Adres, KlientAdres> adresy = new Dictionary<Adres, KlientAdres>(0);
            List<Sklep> sklepy = new List<Sklep>(0);
            List<SklepKategoriaSklepu> sklepykategoriepolaczenia = new List<SklepKategoriaSklepu>(0);
            List<KategoriaSklepu> sklepykategorie = new List<KategoriaSklepu>(0);

            List<Kraje> kraje=new List<Kraje>();
            List<Region> regiony=new List<Region>();
            DoklejeniePolaDoPola modul = new DoklejeniePolaDoPola();

            List<Magazyn> magazyny = new List<Magazyn>();
            ISyncProvider provider = A.Fake<ISyncProvider>();
            modul.PoleZrodlowe = "Symbol";
            modul.PoleDocelowe = "WiadomoscEmail";

            modul.Przetworz(ref kliencidotestu, new Dictionary<long, Produkt>(), ref adresy,
                new List<KategoriaKlienta>(), new List<KlientKategoriaKlienta>(), ref sklepy, ref sklepykategoriepolaczenia,
                ref sklepykategorie,ref kraje,ref regiony, ref magazyny, provider);

            foreach (KeyValuePair<long, Klient> klient in kliencidotestu)
            {
                string ciagDoPorownania = string.Format("{0}{1}", kliencidoporownania[klient.Key].Email,
                    kliencidoporownania[klient.Key].Symbol);
               Assert.Equal(ciagDoPorownania, klient.Value.Email);
            }
        }

        [Fact(DisplayName = "Moduł DoklejeniePolaDoPola - test doklejania pola symbol do pola email z separatorem")]
        public void PrzetworzTestZSeparatorem()
        {

            Dictionary<long, Klient> kliencidotestu = WygenerujKlientow();
            Dictionary<long, Klient> kliencidoporownania = WygenerujKlientow();
            List<KupowaneIlosci> kupowaneilosci = new List<KupowaneIlosci>(0);
            Dictionary<Adres, KlientAdres> adresy = new Dictionary<Adres, KlientAdres>();
            List<Sklep> sklepy = new List<Sklep>(0);
            List<SklepKategoriaSklepu> sklepykategoriepolaczenia = new List<SklepKategoriaSklepu>(0);
            List<KategoriaSklepu> sklepykategorie = new List<KategoriaSklepu>(0);


            DoklejeniePolaDoPola modul = new DoklejeniePolaDoPola();
            modul.PoleZrodlowe = "Symbol";
            modul.PoleDocelowe = "WiadomoscEmail";
            modul.Separator = "###";
            List<Kraje> kraje = new List<Kraje>();
            List<Region> regiony = new List<Region>();
            List<Magazyn> magazyny = new List<Magazyn>();
            ISyncProvider provider = A.Fake<ISyncProvider>();
            modul.Przetworz(ref kliencidotestu, new Dictionary<long, Produkt>(), ref adresy,
                new List<KategoriaKlienta>(), new List<KlientKategoriaKlienta>(), ref sklepy, ref sklepykategoriepolaczenia,
                ref sklepykategorie, ref kraje, ref regiony, ref magazyny, provider);

            foreach (KeyValuePair<long, Klient> klient in kliencidotestu)
            {
                string ciagDoPorownania = string.Format("{0}{2}{1}", kliencidoporownania[klient.Key].Email,
                    kliencidoporownania[klient.Key].Symbol, modul.Separator);
                Assert.Equal(ciagDoPorownania, klient.Value.Email);
            }
        }

        private Dictionary<long, Klient> WygenerujKlientow(int ile = 10)
        {
            Dictionary<long, Klient> slownikklientow = new Dictionary<long, Klient>(ile);
            for (int i = 0; i < ile; i++)
            {
                Klient nowy = new Klient(i);
                nowy.Email = string.Format("fakemail{0}@kiss.com", i);
                nowy.Symbol = string.Format("klient{0}", i);

                slownikklientow.Add(nowy.Id, nowy);
            }

            return slownikklientow;
        }

    }
}
