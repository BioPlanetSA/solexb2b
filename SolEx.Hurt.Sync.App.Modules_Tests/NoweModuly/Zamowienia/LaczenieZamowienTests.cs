using System;
using System.Collections.Generic;
using FakeItEasy;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Zamowienia;
using Xunit;
using Klient = SolEx.Hurt.Core.Klient;

namespace SolEx.Hurt.Sync.App.Modules_Tests.NoweModuly.Zamowienia
{
    public class LaczenieZamowienTests : LaczenieZamowienSubiekt
    {
        [Fact(DisplayName = "Sprawdza ustawienie kategorii")]
        public void SprawdzKlientaCzyMoznaLaczycTest()
        {
            IAPIWywolania api = A.Fake<IAPIWywolania>();
            this.ApiWywolanie = api;
            var katKli = new Dictionary<long, KlientKategoriaKlienta>();
            katKli.Add(1, new KlientKategoriaKlienta(1, 1));
            katKli.Add(2, new KlientKategoriaKlienta(1, 2));
            katKli.Add(3, new KlientKategoriaKlienta(1, 3));
            katKli.Add(4, new KlientKategoriaKlienta(1, 4));
            katKli.Add(5, new KlientKategoriaKlienta(1, 5));
            katKli.Add(6, new KlientKategoriaKlienta(1, 6));
            A.CallTo(() => api.PobierzKlienciKategorie(A<Dictionary<string, object>>.Ignored)).Returns(katKli);
            Klient klient = new Klient { Id = 1 };

            this.WymaganeKategorieKlienta = new List<int>() { 3, 7, 13 };
            Assert.True(this.SprawdzKlientaCzyMoznaLaczyc(klient.Id), "Powinno być true");

            this.WykluczoneKategorieKlienta = new List<int>() { 4, 9, 12 };
            Assert.False(this.SprawdzKlientaCzyMoznaLaczyc(klient.Id), "Powinno być false");

            this.WykluczoneKategorieKlienta = null;
            this.WymaganeKategorieKlienta = null;

            Assert.True(this.SprawdzKlientaCzyMoznaLaczyc(klient.Id));
        }

        [Fact(DisplayName = "Sprawdzanie poprawności odczytu cechy rozbicia z nr zamówienia")]
        public void PobierzCecheRozbiciaTest()
        {
            LaczenieZamowienSubiekt laczenie = new LaczenieZamowienSubiekt();
            string wynik = laczenie.PobierzPowodRozbicia("B2B 149220/1/CHLO z 2");
            Assert.Equal(wynik, "CHLO");
            wynik = laczenie.PobierzPowodRozbicia("B2B 1492ddfds20/1/CHLO z 254");
            Assert.Equal(wynik, "CHLO");
            wynik = laczenie.PobierzPowodRozbicia("B2B 1492ddfds20/1/CHeeeeLO z 254");
            Assert.Equal(wynik, "CHeeeeLO");

            wynik = laczenie.PobierzPowodRozbicia("B21492ddfds20/1/CHeeeeLO z 254");
            Assert.Equal(wynik, string.Empty);

            wynik = laczenie.PobierzPowodRozbicia("IMPORT 149220/1/CHLO z 2");
            Assert.Equal(wynik, "CHLO");
            wynik = laczenie.PobierzPowodRozbicia("Import 1492ddfds20/1/CHLO z 254");
            Assert.Equal(wynik, "CHLO");
            wynik = laczenie.PobierzPowodRozbicia("import 1492ddfds20/1/CHeeeeLO z 254");
            Assert.Equal(wynik, "CHeeeeLO");

            wynik = laczenie.PobierzPowodRozbicia("import1492ddfds20/1/CHeeeeLO z 254");
            Assert.Equal(wynik, string.Empty);
        }

        [Fact(DisplayName = "Sprawdzanie czy dobrze jest przygotowane zapytanie sql")]
        public void PrzygotujZapytanieTest()
        {
            ZamowienieSynchronizacja zamowienie = new ZamowienieSynchronizacja
            {
                KlientId = 10,
                NumerZRozbicia = "1/CHLO",
                LacznieRozbitych = 2
            };

            string wynik = this.PrzygotujZapytanie(zamowienie);
            Assert.True(wynik.IndexOf("AND dok_OdbiorcaId = 10", StringComparison.InvariantCultureIgnoreCase) > 0, $"W wyniku:{wynik}, nie ma ciągu: AND dok_OdbiorcaId = 10");
            Assert.False(wynik.IndexOf("LEFT JOIN fl_Wartosc ON flw_IdObiektu = dok_Id LEFT JOIN fl__Flagi ON flg_Id = flw_IdFlagi", StringComparison.InvariantCultureIgnoreCase) > 0, $"W wyniku:{wynik}, jest ciąg: LEFT JOIN fl_Wartosc ON flw_IdObiektu = dok_Id LEFT JOIN fl__Flagi ON flg_Id = flw_IdFlagi ");
            Assert.False(wynik.IndexOf("AND (flg_Text is null OR flg_Text not like 'Nie tykaj')", StringComparison.InvariantCultureIgnoreCase) > 0, $"W wyniku:{wynik}, jest ciąg: AND (flg_Text is null OR flg_Text not like 'Nie tykaj')");
            Assert.False(wynik.IndexOf("AND dok_NrPelnyOryg like 'B2B %/CHLO z%'", StringComparison.InvariantCultureIgnoreCase) > 0, $"W wyniku:{wynik}, jest ciąg: AND dok_NrPelnyOryg like 'B2B %/CHLO z%'");

            this.FlagaWykluczjaca = "Nie tykaj";

            wynik = this.PrzygotujZapytanie(zamowienie);
            Assert.True(wynik.IndexOf("AND dok_OdbiorcaId = 10", StringComparison.InvariantCultureIgnoreCase) > 0, $"W wyniku:{wynik}, nie ma ciągu: AND dok_OdbiorcaId = 10");
            Assert.True(wynik.IndexOf("LEFT JOIN fl_Wartosc ON flw_IdObiektu = dok_Id LEFT JOIN fl__Flagi ON flg_Id = flw_IdFlagi", StringComparison.InvariantCultureIgnoreCase) > 0, $"W wyniku:{wynik}, nie ma ciągu: LEFT JOIN fl_Wartosc ON flw_IdObiektu = dok_Id LEFT JOIN fl__Flagi ON flg_Id = flw_IdFlagi ");
            Assert.True(wynik.IndexOf("AND (flg_Text is null OR flg_Text not like 'Nie tykaj')", StringComparison.InvariantCultureIgnoreCase) > 0, $"W wyniku:{wynik}, nie ma ciągu: AND (flg_Text is null OR flg_Text not like 'Nie tykaj')");
            Assert.False(wynik.IndexOf("AND dok_NrPelnyOryg like 'B2B %/CHLO z%'", StringComparison.InvariantCultureIgnoreCase) > 0, $"W wyniku:{wynik}, jest ciąg: AND dok_NrPelnyOryg like 'B2B %/CHLO z%'");

            zamowienie.PochodziZRozbicia = true;

            wynik = this.PrzygotujZapytanie(zamowienie);

            Assert.True(wynik.IndexOf("AND dok_NrPelnyOryg like 'B2B %/CHLO z%'", StringComparison.InvariantCultureIgnoreCase) > 0, $"W wyniku:{wynik}, nie ma ciągu: AND dok_NrPelnyOryg like 'B2B %/CHLO z%'");

            this.UwagiWykluczenie = "r:%";
            wynik = this.PrzygotujZapytanie(zamowienie);

            Assert.True(wynik.IndexOf("AND dok_Uwagi not LIKE 'r:%'", StringComparison.InvariantCultureIgnoreCase) > 0, $"W wyniku:{wynik}, nie ma ciągu: AND dok_Uwagi not LIKE 'r:%'");

            this.UwagiWykluczenie = "r:% kot% %Słoń";
            wynik = this.PrzygotujZapytanie(zamowienie);

            Assert.True(wynik.IndexOf("AND dok_Uwagi not LIKE 'r:%'", StringComparison.InvariantCultureIgnoreCase) > 0, $"W wyniku:{wynik}, nie ma ciągu: AND dok_Uwagi not LIKE 'r:%'");
            Assert.True(wynik.IndexOf("AND dok_Uwagi not LIKE 'kot%'", StringComparison.InvariantCultureIgnoreCase) > 0, $"W wyniku:{wynik}, nie ma ciągu: AND dok_Uwagi not LIKE 'kot%'");
            Assert.True(wynik.IndexOf("AND dok_Uwagi not LIKE '%Słoń'", StringComparison.InvariantCultureIgnoreCase) > 0, $"W wyniku:{wynik}, nie ma ciągu: AND dok_Uwagi not LIKE '%Słoń'");
        }

        [Fact(DisplayName = "Sprawdzanie wyszukiwani wspólnego poczatku dwóch textów")]
        public void PobierzWspolnyPoczątekTest()
        {
            string text1 = "Ala ma kota";
            string text2 = "Ala nie ma kota bo lubi psy";
            string wynik = this.PobierzWspolnyPoczatek(text1, text2);
            Assert.Equal(wynik, "Ala");

            text1 = "Alan ma kota";
            wynik = this.PobierzWspolnyPoczatek(text1, text2);
            Assert.Null(wynik);

            text2 = null;
            wynik = this.PobierzWspolnyPoczatek(text1, text2);
            Assert.Null(wynik);

            text1 = null;
            wynik = this.PobierzWspolnyPoczatek(text1, text2);
            Assert.Null(wynik);
        }

        [Fact(DisplayName = "Sprawdzanie poprawności dopisywania tektu do uwag")]
        public void DopiszWyrazenieDoUwagTest()
        {
            string uwagi1 = "r_chlo Waga 4kg";
            string uwagi2 = "r_chlo eeeeeee Waga 4kg";
            string nrZamowienia = "B2B 111/234/eee z 4";
            string dopisek = $"{this.TextDoUwag} {nrZamowienia};";

            string wynik = this.DopiszWyrazenieDoUwag(uwagi1, nrZamowienia, true, uwagi2);
            Assert.Equal(wynik, $"r_chlo {dopisek} Waga 4kg");

            wynik = this.DopiszWyrazenieDoUwag(uwagi1, nrZamowienia, false, uwagi2);
            Assert.Equal(wynik, $"{dopisek} r_chlo Waga 4kg");

            uwagi1 = "Waga 4kg r_chlo";
            uwagi2 = "eWaga eeeeeeee 4kg r_chlo";
            wynik = this.DopiszWyrazenieDoUwag(uwagi1, nrZamowienia, true, uwagi2);
            Assert.Equal(wynik, $"{dopisek} {uwagi1}");

            uwagi1 = "r_chlo ŁĄCZONE B2B/1/4 Waga 4kg";
            uwagi2 = "r_chlo eeeeeee Waga 4kg";
            wynik = this.DopiszWyrazenieDoUwag(uwagi1, nrZamowienia, true, uwagi2);
            Assert.Equal(wynik, $"r_chlo {dopisek} B2B/1/4 Waga 4kg");
        }

        [Fact(DisplayName = "Sprawdzanie poprawności dopisywania nowych uwag")]
        public void DopiszNoweUwagiTest()
        {
            string uwagi1 = "r_chlo ŁĄZONE B2B 111/234/eee z 4 {nowe_uwagi}  Waga 4kg";
            string uwagi2 = "jakieś tam sobie uwagijakieś tam sobie uwagijakieś tam sobie uwagijakieś tam sobie uwagijakieś tam sobie uwagijakieś tam sobie uwagijakieś tam sobie uwagi";

            string wynik = this.DopiszNoweUwagi(uwagi1, uwagi2);
            Assert.True(wynik.Length < 370, $"Dlugość uwag nie powinnien przekroczyć 370 znaków");

            uwagi2 =
                "jakieś tam sobie uwagijakieś tam sobie uwagijakieś tam sobie uwagijakieś tam sobie uwagijakieś tam sobie uwagijakieś tam sobie uwagijakieś tam sobie uwagi " +
                "jakieś tam sobie uwagijakieś tam sobie uwagijakieś tam sobie uwagijakieś tam sobie uwagijakieś tam sobie uwagijakieś tam sobie uwagijakieś tam sobie uwagi " +
                "jakieś tam sobie uwagijakieś tam sobie uwagijakieś tam sobie uwagijakieś tam sobie uwagijakieś tam sobie uwagijakieś tam sobie uwagijakieś tam sobie uwagi ";

            wynik = this.DopiszNoweUwagi(uwagi1, uwagi2);
            Assert.True(wynik.Length == 370, $"Dlugość uwag nie powinnien przekroczyć 370 znaków");
            Assert.Contains("...", wynik);
        }
    }
}