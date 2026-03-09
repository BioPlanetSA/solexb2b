using Xunit;
using SolEx.Hurt.Core.Sync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Common.Extensions;
using SolEx.Hurt.Model;
using FakeItEasy;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;

namespace SolEx.Hurt.Core.Sync.Tests
{
    public class KategorieKlientowWyszukiwanieTests
    {
        [Fact(DisplayName = "Testowanie wyszukiwania kategorii klientów")]
        public void PobierzWszystkieKategorieKlientaTest()
        {
            List<KategoriaKlienta> kategorie = new List<KategoriaKlienta>()
            {
                new KategoriaKlienta() {Grupa = "--auto-", Id = 1, Nazwa = "czerwony"},
                new KategoriaKlienta() {Grupa = "--auto-", Id = 2, Nazwa = "zielony"},
                new KategoriaKlienta() {Grupa = "--auto-", Id = 3, Nazwa = "ZOlty"},
                new KategoriaKlienta() {Grupa = "--auto-", Id = 4, Nazwa = "bialy"},
                new KategoriaKlienta() {Grupa = "kolor", Id = 5, Nazwa = "czerwony"},
                new KategoriaKlienta() {Grupa = "kolor", Id = 6, Nazwa = "zielony"},
                new KategoriaKlienta() {Grupa = "rozmiar", Id = 7, Nazwa = "XXl"},
                new KategoriaKlienta() {Grupa = "rozmiar", Id = 8, Nazwa = "xL"},
                new KategoriaKlienta() {Grupa = "rozmiar", Id = 9, Nazwa = "Xs"},
                new KategoriaKlienta() {Grupa = "--auto--", Id = 10, Nazwa = "&.aaa"},
                new KategoriaKlienta() {Grupa = "--auto--", Id = 91, Nazwa = "&.bbbXs"},
                new KategoriaKlienta() {Grupa = "--auto--", Id = 92, Nazwa = "&.dfgdfXs"},
                new KategoriaKlienta() {Grupa = "--auto--", Id = 93, Nazwa = "&.sdfsfXs"},
            };

            List<KlientKategoriaKlienta> laczniki = new List<KlientKategoriaKlienta>()
            {
                new KlientKategoriaKlienta(4, 1),
                new KlientKategoriaKlienta(7, 1),
                new KlientKategoriaKlienta(9, 1),
                new KlientKategoriaKlienta(4, -3),
                new KlientKategoriaKlienta(4, 3),
                new KlientKategoriaKlienta(4, 5),
                new KlientKategoriaKlienta(4, 6),
                new KlientKategoriaKlienta(4, 9)
            };

            KategorieKlientowWyszukiwanie.PobierzInstancje.Konfiguracja = A.Fake<IConfigBLL>();
            A.CallTo(() => KategorieKlientowWyszukiwanie.PobierzInstancje.Konfiguracja.SeparatorGrupKlientow).Returns(":_?".ToArray());

            //filtracja
            var wynik = KategorieKlientowWyszukiwanie.PobierzInstancje.FiltrujKategorieWgGrupyLubCechy(kategorie, "dfggdf");
            Assert.True(wynik.IsEmpty());

            wynik = KategorieKlientowWyszukiwanie.PobierzInstancje.FiltrujKategorieWgGrupyLubCechy(kategorie, "--auto--:&", false);
            Assert.True(wynik.Count == 4);

            wynik = KategorieKlientowWyszukiwanie.PobierzInstancje.FiltrujKategorieWgGrupyLubCechy(kategorie, "dfggdf:");
            Assert.True(wynik.IsEmpty());

            wynik = KategorieKlientowWyszukiwanie.PobierzInstancje.FiltrujKategorieWgGrupyLubCechy(kategorie, "auto");
            Assert.True(wynik.IsEmpty());

            wynik = KategorieKlientowWyszukiwanie.PobierzInstancje.FiltrujKategorieWgGrupyLubCechy(kategorie, "--auto----");
            Assert.True(wynik.IsEmpty());

            wynik = KategorieKlientowWyszukiwanie.PobierzInstancje.FiltrujKategorieWgGrupyLubCechy(kategorie, "--auto");
            Assert.True(wynik.Count == 8);

            wynik = KategorieKlientowWyszukiwanie.PobierzInstancje.FiltrujKategorieWgGrupyLubCechy(kategorie, "--auto", true);
            Assert.True(wynik.IsEmpty());

            wynik = KategorieKlientowWyszukiwanie.PobierzInstancje.FiltrujKategorieWgGrupyLubCechy(kategorie, "xl");
            Assert.True(wynik.Count == 0);

            wynik = KategorieKlientowWyszukiwanie.PobierzInstancje.FiltrujKategorieWgGrupyLubCechy(kategorie, "RoZMIAR");
            Assert.True(wynik.Count == 3, wynik.Count.ToString());

            wynik = KategorieKlientowWyszukiwanie.PobierzInstancje.FiltrujKategorieWgGrupyLubCechy(kategorie, "RoZMIAR:");
            Assert.True(wynik.Count == 3);

            wynik = KategorieKlientowWyszukiwanie.PobierzInstancje.FiltrujKategorieWgGrupyLubCechy(kategorie, "RoZMIAR:x");
            Assert.True(wynik.Count == 3);

            wynik = KategorieKlientowWyszukiwanie.PobierzInstancje.FiltrujKategorieWgGrupyLubCechy(kategorie, "RoZMIAR:x", true);
            Assert.True(wynik.Count == 0);

            wynik = KategorieKlientowWyszukiwanie.PobierzInstancje.FiltrujKategorieWgGrupyLubCechy(kategorie, "RoZMIAR:xx");
            Assert.True(wynik.Count == 1);

            wynik = KategorieKlientowWyszukiwanie.PobierzInstancje.FiltrujKategorieWgGrupyLubCechy(kategorie, "--auto-:bialy");
            Assert.True(wynik.Count == 1);

            // Klient klient = new Klient() {Id = 4};

        }
    }
}