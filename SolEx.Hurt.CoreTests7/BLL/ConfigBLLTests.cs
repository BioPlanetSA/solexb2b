using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using Xunit;
namespace SolEx.Hurt.Core.BLL.Tests
{
    public class ConfigBLLTests
    {
        [Fact(DisplayName = "Wyliczanie indywidualnej stawki vat")]
        public void SynchronizacjaPobierzPoleIndywidualnaStawaVatTest()
        {
            string domyslne = "";
            Dictionary<string, string> pars = new Dictionary<string, string>();
            ConfigBLL c = A.Fake<ConfigBLL>();
            A.CallTo(() => c.PobierzSynchronizacjaPoleUstawienie("IndywidualnaStawaVat", domyslne, pars, ustawieniaGrupa.Klienci, A<string>.Ignored, null)).Returns("");

            Klient k = new Klient();
            c.SynchronizacjaPobierzPoleIndywidualnaStawaVat(k, domyslne, pars, true, true);

            Assert.Equal(k.IndywidualnaStawaVat, 0);
            c.SynchronizacjaPobierzPoleIndywidualnaStawaVat(k, domyslne, pars, false, true);

            Assert.Equal(k.IndywidualnaStawaVat, null);
            c.SynchronizacjaPobierzPoleIndywidualnaStawaVat(k, domyslne, pars, true, false);

            Assert.Equal(k.IndywidualnaStawaVat, 0);
            c.SynchronizacjaPobierzPoleIndywidualnaStawaVat(k, domyslne, pars, false, false);

            Assert.Equal(k.IndywidualnaStawaVat, 0);
        }

        private Dictionary<string, List<string>> _plikiIntegracji = new Dictionary<string, List<string>>
        {
            {"1_Produkty_Podstawowy-plik-z-listą-produktów_XML", new List<string> { "6.cshtml", "1.cshtml","2.cshtml"}  },
            {"2_Produkty_Podstawowy-plik-z-listą-produktów_CSV", new List<string> {"1.cshtml","2.cshtml"}  },
            {"9_Sklepy_Lista-sklepow_XML", new List<string> {"1.cshtml","2.cshtml","6.cshtml", "-2.cshtml"}  },
            {"4_ProduktyKatalogDrukowanie_Podstawowy-plik-danych_JSON", new List<string> { "2.cshtml", "1.cshtml" }  }
        };


        private IDictionary<TypDanychIntegracja, List<PlikIntegracjiSzablon>> _spodziewanyWynikPlikiIntegracji = null;


        private string sciezkaPlikowIntegracji = AppDomain.CurrentDomain.BaseDirectory + @"\Views\Integracja\";

        private void przygotujPlikiIntegracji()
        {
            if (System.IO.Directory.Exists(sciezkaPlikowIntegracji))
            {
                System.IO.Directory.Delete(sciezkaPlikowIntegracji, true);
            }

            foreach (var katalog in _plikiIntegracji)
            {
                string sciezkaKatalog = sciezkaPlikowIntegracji + katalog.Key;
                if (System.IO.Directory.Exists(sciezkaKatalog))
                {
                    System.IO.Directory.Delete(sciezkaKatalog, true);
                }

                System.IO.Directory.CreateDirectory(sciezkaKatalog);

                foreach (var plik in katalog.Value)
                {
                    var f = System.IO.File.Create(sciezkaKatalog + "/" + plik);
                    f.Close();
                }
            }

            _spodziewanyWynikPlikiIntegracji = new Dictionary<TypDanychIntegracja, List<PlikIntegracjiSzablon>>();


            List<PlikIntegracjiSzablon> listaSzablonowProdukty = new List<PlikIntegracjiSzablon>();
            listaSzablonowProdukty.Add(new PlikIntegracjiSzablon() { Format = "CSV", Szablon = "Podstawowy-plik-z-listą-produktów", typDanych = TypDanychIntegracja.Produkty, Wersja = new List<int> { 2, 1 } });
            listaSzablonowProdukty.Add(new PlikIntegracjiSzablon() { Format = "XML", Szablon = "Podstawowy-plik-z-listą-produktów", typDanych = TypDanychIntegracja.Produkty, Wersja = new List<int> { 6, 2, 1 } });
            _spodziewanyWynikPlikiIntegracji.Add(TypDanychIntegracja.Produkty, listaSzablonowProdukty);

            List<PlikIntegracjiSzablon> listaSzablonowKatalog = new List<PlikIntegracjiSzablon>();
            listaSzablonowKatalog.Add(new PlikIntegracjiSzablon() { Format = "JSON", Szablon = "Podstawowy-plik-danych", typDanych = TypDanychIntegracja.ProduktyKatalogDrukowanie, Wersja = new List<int> { 2, 1 } });
            _spodziewanyWynikPlikiIntegracji.Add(TypDanychIntegracja.ProduktyKatalogDrukowanie, listaSzablonowKatalog);

            List<PlikIntegracjiSzablon> listaSzablonowSklepy = new List<PlikIntegracjiSzablon>();
            listaSzablonowSklepy.Add(new PlikIntegracjiSzablon() { Format = "XML", Szablon = "Lista-sklepow", typDanych = TypDanychIntegracja.Sklepy, Wersja = new List<int> { 6, 2, 1, -2 } });
            _spodziewanyWynikPlikiIntegracji.Add(TypDanychIntegracja.Sklepy, listaSzablonowSklepy);
        }

        [Fact(DisplayName = "test pobierania szablon dla integracji - bez szablonow wlasnych klienta")]
        public void PobierzListePlikowIntegracjiTest()
        {
            this.przygotujPlikiIntegracji();

            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();

            A.CallTo(() => calosc.Konfiguracja.SzablonNiestandardowyNazwa).Returns(null);

            ConfigBLL conf = new ConfigBLL(calosc, null);

            IDictionary<TypDanychIntegracja, List<PlikIntegracjiSzablon>> wynik = conf.PobierzListePlikowIntegracji;

            Assert.True(wynik.Count == _spodziewanyWynikPlikiIntegracji.Count);
            for (int i = 0; i < wynik.Count; ++i)
            {
                TypDanychIntegracja klucz = wynik.Keys.ToList()[i];
                Assert.True(klucz == _spodziewanyWynikPlikiIntegracji.Keys.ToList()[i], " zły klucz dla indeks: " + i);

                for (int ii = 0; ii < wynik[klucz].Count; ++ii)
                {
                    Assert.True(wynik[klucz][ii].Format == _spodziewanyWynikPlikiIntegracji[klucz][ii].Format, string.Format("zły format dla klucza: {0} i indeksu ii: {1}", i, ii));
                    Assert.True(wynik[klucz][ii].Szablon == _spodziewanyWynikPlikiIntegracji[klucz][ii].Szablon, string.Format("zły Szablon dla klucza: {0} i indeksu ii: {1}", i, ii));
                    Assert.True(wynik[klucz][ii].typDanych == _spodziewanyWynikPlikiIntegracji[klucz][ii].typDanych, string.Format("zły typDanych dla klucza: {0} i indeksu ii: {1}", i, ii));

                    for (int iii = 0; iii < wynik[klucz][ii].Wersja.Count; ++iii)
                    {
                        Assert.True(wynik[klucz][ii].Wersja[iii] == _spodziewanyWynikPlikiIntegracji[klucz][ii].Wersja[iii], string.Format("zła wersja dla klucza: {0} i indeksu ii: {1} / iii: {2}", i, ii, iii));
                    }

                    //sciezka do szablonu czy jest ok
                    foreach (var wersja in wynik[klucz][ii].Wersja)
                    {
                        string sciezka = sciezkaPlikowIntegracji + @"\" + wynik[klucz][ii].PobierzWidokPliku(wersja) + ".cshtml";
                        Assert.True(System.IO.File.Exists(sciezka));
                    }
                }
            }
        }

        [Fact(DisplayName = "Sprawdzanie poprawności wyliczania kursu między walutami.")]
        public void PobierzKursWalutTest()
        {
            Waluta waluta1 = new Waluta(1, "PLN", "PLN");
            Waluta waluta2 = new Waluta(2, "EUR", "EUR", 5);
            ConfigBLL c = A.Fake<ConfigBLL>();

            Assert.Throws(typeof(Exception), () =>
            {
                c.PobierzKursWalut(waluta1, waluta2);
            });
            waluta1.Kurs = 0;
            Assert.Throws(typeof(Exception), () =>
            {
                c.PobierzKursWalut(waluta1, waluta2);
            });
            waluta1.Kurs = 1;

            decimal kurs = c.PobierzKursWalut(waluta1, waluta2);

            var kursCalkowity = Math.Round(waluta1.Kurs.Value * (1M / waluta2.Kurs.Value), 4);

            Assert.True(kurs == kursCalkowity, $"Kurs wynosi: {kurs} a powienien :{kursCalkowity}");
        }

        [Fact(DisplayName = "Sprawdzanie poprawności wyliczania kursu między walutami za pomocą symboli.")]
        public void PobierzKursWalutTest1()
        {
            ConfigBLL c = A.Fake<ConfigBLL>();

            string waluta1 = "PLN";
            string waluta2 = "EUR";

            Dictionary<long, Waluta> slownikWalut = new Dictionary<long, Waluta> {{2, new Waluta(2, "EUR", "EUR", 5)}};

            A.CallTo(() => c.SlownikWalut).Returns( slownikWalut);
            Assert.Throws(typeof(Exception), () =>
            {
                c.PobierzKursWalut(waluta1, waluta2);
            });
            slownikWalut.Add(1, new Waluta(1, "PLN", "PLN", 1));

            A.CallTo(() => c.SlownikWalut).Returns(slownikWalut);
            decimal kurs = c.PobierzKursWalut(waluta1, waluta2);

            var kursCalkowity = Math.Round(1 * (1M / 5), 4);

            Assert.True(kurs == kursCalkowity, $"Kurs wynosi: {kurs} a powienien :{kursCalkowity}");
        }
    }
}
