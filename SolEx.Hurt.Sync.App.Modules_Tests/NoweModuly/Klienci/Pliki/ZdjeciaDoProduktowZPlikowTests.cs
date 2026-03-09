using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Pliki;

using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Pliki.Tests
{
    public class ZdjeciaDoProduktowZPlikowTests
    {
        [Fact(DisplayName = "Moduł do zdjęć z plików - test wyciągania identyfikatorów")]
        public void WyciagnijIdentyfikatorPlikuZNazwyTest()
        {
            ZdjeciaDoProduktowZPlikow modul = new ZdjeciaDoProduktowZPlikow();
            string separator = "_";

            string zdjecieLuzem1 = "9104501S";
            string zdjecieLuzem2 = "9104501S";
            string zdjecieLuzem2full = zdjecieLuzem2 + "_1";

            List<string> identyfikatory = modul.WyciagnijIdentyfikatorPlikuZNazwy(zdjecieLuzem1, separator);

            //pierwszy identyfikator nie może zostać zmieniony
            Assert.Equal(identyfikatory.First(), zdjecieLuzem1);

            List<string> identyfikatory2 = modul.WyciagnijIdentyfikatorPlikuZNazwy(zdjecieLuzem2full, separator);

            //identyfikator powinien mieć uciętą końcówkę z jego kolejnością
            Assert.Equal(identyfikatory2.First(), zdjecieLuzem2);

            List<string> identyfikatory3 = modul.WyciagnijIdentyfikatorPlikuZNazwy(zdjecieLuzem1, separator);

            //pierwszy identyfikator nie może zostać zmieniony
            Assert.Equal(identyfikatory3.First(), zdjecieLuzem1);

            List<string> identyfikatory4 = modul.WyciagnijIdentyfikatorPlikuZNazwy(zdjecieLuzem2full, separator);

            //identyfikator powinien mieć uciętą końcówkę z jego kolejnością
            Assert.Equal(identyfikatory4.First(), zdjecieLuzem2);


            string zdjecieSubiekt1 = "30022246";
            string zdjecieSubiekt1full = zdjecieSubiekt1 + "_2560_0";

            List<string> identyfikatory5 = modul.WyciagnijIdentyfikatorPlikuZNazwy(zdjecieSubiekt1full, separator);

            //identyfikator powinien mieć uciętą końcówkę z jego kolejnością
            Assert.Equal(identyfikatory5.First(), zdjecieSubiekt1);

            string z = "86154";
            string z1 = "86154_b";

            List<string> identyfikatory6 = modul.WyciagnijIdentyfikatorPlikuZNazwy(z1, separator);

            Assert.Equal(identyfikatory6.First(), z);

        }

        [Fact(DisplayName = "Moduł zdjęć z plików - test głównej metody, szuka po symbolu")]
        public void ZdjeciaDoProduktowZPlikowPelnyTest()
        {
            string sciezka = "C:\\fotki";
            var nazwy = PrzygodujDaneTestoweNazwyPlikowTest1();

            var pliki = PrzygotujDaneTestoweTest1(sciezka, nazwy);

            LogiFormatki logi = A.Fake<LogiFormatki>();
            PlikiBLL plikiBLL = A.Fake<PlikiBLL>();
            ZdjeciaDoProduktowZPlikow modul = A.Fake<ZdjeciaDoProduktowZPlikow>();
            modul.Sciezka = sciezka;
            modul.Logi = logi;
            modul.PoCzymSzukacPlikow = TypyPolDoDopasowaniaZdjecia.Kod;
            A.CallTo(() => modul.CzySciezkaIstnieje(sciezka)).Returns(true);
            A.CallTo(() => modul.PobierzPlikiZKatalogu(sciezka)).Returns(pliki);

            Produkt produkt1 = new Produkt();
            produkt1.Kod = "86154";
            produkt1.Id = 666;

            Produkt produkt5 = new Produkt();
            produkt5.Kod = ".CE285A(1)";
            produkt5.Id = 555;

            modul.PlikBll = plikiBLL;
            int licznik = modul.licznikMinimalny;

            Plik plik = modul.UtworzPlik(pliki[0], nazwy[0], ref licznik);
            plik.Rozmiar = 1000;
            plik.NazwaBezRozszerzenia = nazwy[0];
            plik.Id = 1;
            plik.Sciezka = sciezka + "\\";

            A.CallTo(() => modul.UtworzPlik(pliki[0], nazwy[0], ref licznik)).Returns(plik);
            A.CallTo(() => plikiBLL.CzyPlikToZdjecie(plik)).Returns(true);


            Plik plik2 = modul.UtworzPlik(pliki[1], nazwy[3], ref licznik);
            plik2.Rozmiar = 1000;
            plik2.NazwaBezRozszerzenia = nazwy[3];
            plik2.Id = 2;
            plik2.Sciezka = sciezka + "\\";

            A.CallTo(() => modul.UtworzPlik(pliki[1], nazwy[3], ref licznik)).Returns(plik2);
            A.CallTo(() => plikiBLL.CzyPlikToZdjecie(plik2)).Returns(true);

            Plik plik3 = modul.UtworzPlik(pliki[3], nazwy[4], ref licznik);
            plik3.Rozmiar = 1000;
            plik3.NazwaBezRozszerzenia = nazwy[4];
            plik3.Id = 3;
            plik3.Sciezka = sciezka + "\\";

            Plik plik5 = modul.UtworzPlik(pliki[5], nazwy[5], ref licznik);
            plik5.Rozmiar = 1000;
            plik5.NazwaBezRozszerzenia = nazwy[5];
            plik5.Id = 3;
            plik5.Sciezka = sciezka + "\\";

            A.CallTo(() => modul.UtworzPlik(pliki[3], nazwy[4], ref licznik)).Returns(plik3);
            A.CallTo(() => plikiBLL.CzyPlikToZdjecie(plik3)).Returns(true);

            A.CallTo(() => modul.UtworzPlik(pliki[5], nazwy[5], ref licznik)).Returns(plik5);
            A.CallTo(() => plikiBLL.CzyPlikToZdjecie(plik5)).Returns(true);

            Dictionary<long, Produkt> listaproduktow = new List<Produkt> { produkt1, produkt5 }.ToDictionary(a => a.Id, a => a);

            A.CallTo(() => modul.ZweryfikujTypPliku(plik)).Returns(true);
            A.CallTo(() => modul.ZweryfikujTypPliku(plik2)).Returns(true);
            A.CallTo(() => modul.ZweryfikujTypPliku(plik3)).Returns(true);
            A.CallTo(() => modul.ZweryfikujTypPliku(plik5)).Returns(true);

            foreach (string nazwapliku in pliki)
            {
                string nowanazwa = Path.GetFileNameWithoutExtension(nazwapliku).ToLower();
                A.CallTo(() => modul.WyciagnijIdentyfikatorPlikuZNazwy(nowanazwa, modul.Separator)).Returns(new List<string>() { nowanazwa });
            }

            List<ProduktPlik> plikiproduktow = new List<ProduktPlik>();
            List<Plik> plikilista = new List<Plik>();
            List<Cecha> cechylista = new List<Cecha>();
            List<KategoriaProduktu> kategorielista = new List<KategoriaProduktu>();
            List<Klient> kliencilista = new List<Klient>();

            modul.Przetworz(listaproduktow, ref plikiproduktow, ref plikilista, null, ref cechylista, ref kategorielista, ref kliencilista);

            var pierwszy = plikilista.First(a => a.Nazwa == nazwy[4]);
            //powinno znaleźć jeden plik
            Assert.NotNull(plikiproduktow.FirstOrDefault(a => a.Glowny && a.PlikId == pierwszy.Id));

            //powinno znaleść powiązanie dla tego jednego pliku wyżej i dla jedynego produktu
            Assert.True(plikiproduktow.Any(a => a.PlikId == pierwszy.Id && a.ProduktId == produkt1.Id));
        }

        [Fact(DisplayName = "Moduł zdjęć z plików - test głównej metody, szuka po symbolu. zdjęcia trec")]
        public void ZdjeciaDoProduktowZPlikowPelnyTestTrec()
        {
            string sciezka = "C:\\fotki";
            var nazwy = PrzygodujDaneTestoweNazwyPlikowTestTrec();

            var pliki = PrzygotujDaneTestoweTest1(sciezka, nazwy);

            LogiFormatki logi = A.Fake<LogiFormatki>();
            PlikiBLL plikiBLL = A.Fake<PlikiBLL>();
            ZdjeciaDoProduktowZPlikow modul = A.Fake<ZdjeciaDoProduktowZPlikow>();
            modul.Sciezka = sciezka;
            modul.Logi = logi;
            modul.PoCzymSzukacPlikow = TypyPolDoDopasowaniaZdjecia.Kod;
            A.CallTo(() => modul.CzySciezkaIstnieje(sciezka)).Returns(true);
            A.CallTo(() => modul.PobierzPlikiZKatalogu(sciezka)).Returns(pliki);

            Produkt produkt1 = new Produkt();
            produkt1.Kod = "86154";
            produkt1.Id = 666;

            Produkt produkt5 = new Produkt();
            produkt5.Kod = "BCAA H-S 130. W-G";
            produkt5.Id = 555;

            modul.PlikBll = plikiBLL;
            int licznik = modul.licznikMinimalny;

            Plik plik = modul.UtworzPlik(pliki[0], nazwy[0], ref licznik);
            plik.Rozmiar = 1000;
            plik.NazwaBezRozszerzenia = nazwy[0];
            plik.Id = 1;
            plik.Sciezka = sciezka + "\\";

            A.CallTo(() => modul.UtworzPlik(pliki[0], nazwy[0], ref licznik)).Returns(plik);
            A.CallTo(() => plikiBLL.CzyPlikToZdjecie(plik)).Returns(true);


            Plik plik2 = modul.UtworzPlik(pliki[2], nazwy[1], ref licznik);
            plik2.Rozmiar = 1000;
            plik2.NazwaBezRozszerzenia = nazwy[1];
            plik2.Id = 2;
            plik2.Sciezka = sciezka + "\\";

            A.CallTo(() => modul.UtworzPlik(pliki[2], nazwy[1], ref licznik)).Returns(plik2);
            A.CallTo(() => plikiBLL.CzyPlikToZdjecie(plik2)).Returns(true);

            Plik plik3 = modul.UtworzPlik(pliki[3], nazwy[4], ref licznik);
            plik3.Rozmiar = 1000;
            plik3.NazwaBezRozszerzenia = nazwy[4];
            plik3.Id = 3;
            plik3.Sciezka = sciezka + "\\";

            Plik plik5 = modul.UtworzPlik(pliki[5], nazwy[5], ref licznik);
            plik5.Rozmiar = 1000;
            plik5.NazwaBezRozszerzenia = nazwy[5];
            plik5.Id = 3;
            plik5.Sciezka = sciezka + "\\";

            A.CallTo(() => modul.UtworzPlik(pliki[3], nazwy[4], ref licznik)).Returns(plik3);
            A.CallTo(() => plikiBLL.CzyPlikToZdjecie(plik3)).Returns(true);

            A.CallTo(() => modul.UtworzPlik(pliki[5], nazwy[5], ref licznik)).Returns(plik5);
            A.CallTo(() => plikiBLL.CzyPlikToZdjecie(plik5)).Returns(true);

            Dictionary<long, Produkt> listaproduktow = new List<Produkt> { produkt1, produkt5 }.ToDictionary(a => a.Id, a => a);

            A.CallTo(() => modul.ZweryfikujTypPliku(plik)).Returns(true);
            A.CallTo(() => modul.ZweryfikujTypPliku(plik2)).Returns(true);
            A.CallTo(() => modul.ZweryfikujTypPliku(plik3)).Returns(true);
            A.CallTo(() => modul.ZweryfikujTypPliku(plik5)).Returns(true);

            foreach (string nazwapliku in pliki)
            {
                string nowanazwa = Path.GetFileNameWithoutExtension(nazwapliku).ToLower();
                string zwracana = nowanazwa.Split('_').First();
                A.CallTo(() => modul.WyciagnijIdentyfikatorPlikuZNazwy(nowanazwa, modul.Separator))
                    .Returns(new List<string>() { zwracana });
            }

            List<ProduktPlik> plikiproduktow = new List<ProduktPlik>();
            List<Plik> plikilista = new List<Plik>();
            List<Cecha> cechylista = new List<Cecha>();
            List<KategoriaProduktu> kategorielista = new List<KategoriaProduktu>();
            List<Klient> kliencilista = new List<Klient>();

            modul.Przetworz(listaproduktow, ref plikiproduktow, ref plikilista, null, ref cechylista, ref kategorielista, ref kliencilista);

            var pierwszy = plikilista.First(a => a.Nazwa == nazwy[1]);
            //powinno znaleźć jeden plik
            Assert.NotNull(plikiproduktow.FirstOrDefault(a => a.Glowny && a.PlikId == pierwszy.Id));

            //powinno znaleść powiązanie dla tego jednego pliku wyżej i dla jedynego produktu
            Assert.True(plikiproduktow.Any(a => a.PlikId == pierwszy.Id && a.ProduktId == produkt5.Id));
        }

        private static string[] PrzygodujDaneTestoweNazwyPlikowTest1()
        {
            string[] nazwy = new string[]
            {
                "86154_a.PNG",
                "86154_b.PNG",
                "TESTOWY_a.PNG",
                "86154_1.PNG",
                "86154.PNG",
                "-CE285A-1-_3586_1.jpg"
            };
            return nazwy;
        }

        private static string[] PrzygodujDaneTestoweNazwyPlikowTestTrec()
        {
            string[] nazwy = new string[]
            {
                "BCAA-H-S-130-LE_1.PNG",
                "BCAA-H-S-130-W-G_1.PNG",
                "BCAA-H-S-300-LE_1.PNG",
                "BCAA-H-S-300-W-G_1.PNG",
                "BCAA-H-S-600-LE_1.PNG",
                "BCAA-H-S-600-W-G_1.jpg"
            };
            return nazwy;
        }

        private static string[] PrzygotujDaneTestoweTest1(string sciezka, string[] nazwy)
        {
            string[] pliki = new string[]
            {
                sciezka + "\\" + nazwy[0],
                sciezka + "\\" + nazwy[3],
                sciezka + "\\" + nazwy[1],
                sciezka + "\\" + nazwy[4],
                sciezka + "\\" + nazwy[2],
                sciezka + "\\" + nazwy[5]
            };
            return pliki;
        }

        [Fact(DisplayName = "Moduł zdjęć z plików - test głównej metody, szuka po pole_tekst1")]
        public void ZdjeciaDoProduktowZPlikowPelnyTest2()
        {
            string sciezka = "C:\\fotki";
            var nazwy = PrzygodujDaneTestoweNazwyPlikowTest2();

            var pliki = PrzygotujDaneTestoweSciezkiPlikowTest2(sciezka, nazwy);

            LogiFormatki logi = A.Fake<LogiFormatki>();
            PlikiBLL plikiBLL = A.Fake<PlikiBLL>();
            ZdjeciaDoProduktowZPlikow modul = A.Fake<ZdjeciaDoProduktowZPlikow>();
            modul.Sciezka = sciezka;
            modul.Logi = logi;

            modul.PoCzymSzukacPlikow = TypyPolDoDopasowaniaZdjecia.PoleTekst1;
            modul.Separator = "#";
            A.CallTo(() => modul.CzySciezkaIstnieje(sciezka)).Returns(true);
            A.CallTo(() => modul.PobierzPlikiZKatalogu(sciezka)).Returns(pliki);

            Produkt produkt1 = new Produkt();
            produkt1.Kod = produkt1.PoleTekst1 = "shootfighters-lublin-rashguard2-projekty-graficzne-visual12_3";
            produkt1.Id = 666;

            modul.PlikBll = plikiBLL;
            int licznik = modul.licznikMinimalny;

            Plik plik = modul.UtworzPlik(pliki[0], nazwy[0], ref licznik);
            plik.Rozmiar = 1000;
            plik.NazwaBezRozszerzenia = nazwy[0];
            plik.Id = 1;
            plik.Sciezka = sciezka + "\\";

            A.CallTo(() => modul.UtworzPlik(pliki[0], nazwy[0], ref licznik)).Returns(plik);
            A.CallTo(() => plikiBLL.CzyPlikToZdjecie(plik)).Returns(true);


            Plik plik2 = modul.UtworzPlik(pliki[1], nazwy[3], ref licznik);
            plik2.Rozmiar = 1000;
            plik2.NazwaBezRozszerzenia = nazwy[3];
            plik2.Id = 2;
            plik2.Sciezka = sciezka + "\\";

            A.CallTo(() => modul.UtworzPlik(pliki[1], nazwy[3], ref licznik)).Returns(plik2);
            A.CallTo(() => plikiBLL.CzyPlikToZdjecie(plik2)).Returns(true);

            Plik plik3 = modul.UtworzPlik(pliki[3], nazwy[4], ref licznik);
            plik3.Rozmiar = 1000;
            plik3.NazwaBezRozszerzenia = nazwy[4];
            plik3.Id = 3;
            plik3.Sciezka = sciezka + "\\";

            A.CallTo(() => modul.UtworzPlik(pliki[3], nazwy[4], ref licznik)).Returns(plik3);
            A.CallTo(() => plikiBLL.CzyPlikToZdjecie(plik3)).Returns(true);

            foreach (string nazwapliku in pliki)
            {
                string nowanazwa = Path.GetFileNameWithoutExtension(nazwapliku).ToLower();
                A.CallTo(() => modul.WyciagnijIdentyfikatorPlikuZNazwy(nowanazwa, modul.Separator))
                    .Returns(new List<string>() { nowanazwa });
            }

            Dictionary<long, Produkt> listaproduktow = new List<Produkt> { produkt1 }.ToDictionary(a => a.Id, a => a);

            A.CallTo(() => modul.ZweryfikujTypPliku(plik)).Returns(true);
            A.CallTo(() => modul.ZweryfikujTypPliku(plik2)).Returns(true);
            A.CallTo(() => modul.ZweryfikujTypPliku(plik3)).Returns(true);

            List<ProduktPlik> plikiproduktow = new List<ProduktPlik>();
            List<Plik> plikilista = new List<Plik>();
            List<Cecha> cechylista = new List<Cecha>();
            List<KategoriaProduktu> kategorielista = new List<KategoriaProduktu>();
            List<Klient> kliencilista = new List<Klient>();

            modul.Przetworz(listaproduktow, ref plikiproduktow, ref plikilista, null, ref cechylista, ref kategorielista, ref kliencilista);

            var pierwszy = plikilista.First(a => a.Nazwa == nazwy[0]);
            //powinno znaleźć jeden plik
            Assert.NotNull(plikiproduktow.FirstOrDefault(a => a.Glowny && a.PlikId == pierwszy.Id));

            //powinno znaleść powiązanie dla tego jednego pliku wyżej i dla jedynego produktu
            Assert.True(plikiproduktow.Any(a => a.PlikId == pierwszy.Id && a.ProduktId == produkt1.Id));
        }

        private static string[] PrzygotujDaneTestoweSciezkiPlikowTest2(string sciezka, string[] nazwy)
        {
            string[] pliki = new string[]
            {
                sciezka + "\\" + nazwy[0],
                sciezka + "\\" + nazwy[3],
                sciezka + "\\" + nazwy[1],
                sciezka + "\\" + nazwy[4],
                sciezka + "\\" + nazwy[2]
            };
            return pliki;
        }

        private static string[] PrzygodujDaneTestoweNazwyPlikowTest2()
        {
            string[] nazwy = new string[]
            {
                "shootfighters-lublin-rashguard2-projekty-graficzne-visual12_3.PNG",
                "86154_b.PNG",
                "TESTOWY_a.PNG",
                "86154_1.PNG",
                "86154.PNG"
            };
            return nazwy;
        }

        [Fact(DisplayName = "Moduł zdjęć z plików - dodawanie zdjęć dla danego produktu do list")]
        public void DodajPlikiDoProduktowTest()
        {
            int licznikMinimalny = 100;
            Dictionary<long, ProduktPlik> plikiLokalnePowiazania = new Dictionary<long, ProduktPlik>();
            List<Plik> plikiLokalne = new List<Plik>();
            string Separator = "_";


            string sciezkaZdjec = "C:\\fotki\\";
            string nazwaPliku1 = "zdjecie1_1.jpg";
            string sciezkaPliku1 = sciezkaZdjec + nazwaPliku1;
            int idProduktu = 69;

            LogiFormatki logi = A.Fake<LogiFormatki>();
            PlikiBLL plikiBLL = A.Fake<PlikiBLL>();
            ZdjeciaDoProduktowZPlikow modul = A.Fake<ZdjeciaDoProduktowZPlikow>();
            modul.PlikBll = plikiBLL;
            modul.Logi = logi;

            Plik plik = modul.UtworzPlik(sciezkaPliku1, nazwaPliku1, ref licznikMinimalny);
            plik.Rozmiar = 1000;
            plik.NazwaBezRozszerzenia = nazwaPliku1;
            plik.Id = 3;
            plik.Sciezka = sciezkaZdjec;

            A.CallTo(() => modul.UtworzPlik(sciezkaPliku1, nazwaPliku1, ref licznikMinimalny)).Returns(plik);
            A.CallTo(() => plikiBLL.CzyPlikToZdjecie(plik)).Returns(true);
            A.CallTo(() => modul.ZweryfikujTypPliku(plik)).Returns(true);

            string innyPlik = "zdjecie1_2.jpg";
            string sciezkaPliku2 = "C:\\fotki\\" + innyPlik;

            Plik plik2 = modul.UtworzPlik(sciezkaPliku2, innyPlik, ref licznikMinimalny);
            plik2.Rozmiar = 1000;
            plik2.NazwaBezRozszerzenia = innyPlik;
            plik2.Id = 4;
            plik2.Sciezka = sciezkaZdjec;

            A.CallTo(() => modul.UtworzPlik(sciezkaPliku2, innyPlik, ref licznikMinimalny)).Returns(plik2);
            A.CallTo(() => plikiBLL.CzyPlikToZdjecie(plik2)).Returns(true);
            A.CallTo(() => modul.ZweryfikujTypPliku(plik2)).Returns(true);

            HashSet<string> paryZdjecProduktu = new HashSet<string>();
            paryZdjecProduktu.Add(sciezkaPliku1);
            paryZdjecProduktu.Add(sciezkaPliku2);

            //musi zwrócić tylko 1 zdjęcie dla produktu i musi to być zdjęcie główne
            modul.DodajPlikiDoProduktow(plikiLokalnePowiazania, plikiLokalne, sciezkaPliku1, licznikMinimalny, Separator, idProduktu, paryZdjecProduktu);

            Assert.Equal(1, plikiLokalne.Count);
            Assert.Equal(1, plikiLokalnePowiazania.Count);

            Assert.Equal(nazwaPliku1, plikiLokalne.First().Nazwa);
            Assert.Equal(idProduktu, plikiLokalnePowiazania.First().Value.ProduktId);
            Assert.Equal(plik.Id, plikiLokalnePowiazania.First().Value.PlikId);
            Assert.True(plikiLokalnePowiazania.First().Value.Glowny);

            //musi zwrócić 2 zdjęcia (pierwsze dodane było wyżej) i drugie zdjęcie nie może być zdjęciem głównym
            modul.DodajPlikiDoProduktow(plikiLokalnePowiazania, plikiLokalne, sciezkaPliku2, licznikMinimalny, Separator, idProduktu, paryZdjecProduktu);

            Assert.Equal(2, plikiLokalne.Count);
            Assert.Equal(2, plikiLokalnePowiazania.Count);

            Assert.Equal(innyPlik, plikiLokalne[1].Nazwa);
            Assert.Equal(idProduktu, plikiLokalnePowiazania.Values.Last().ProduktId);
            Assert.Equal(plik2.Id, plikiLokalnePowiazania.Values.Last().PlikId);
            Assert.False(plikiLokalnePowiazania.Values.Last().Glowny);

        }

        string sciezka = "C:\\gdziesgleboko\\bylysobie\\fotki\\";
        private List<string> PrzygodujDaneTestoweDlaSortowaniaZdjec1()
        {

            return new List<string>()
            {
                sciezka + "kllklklk_3.jpg",
                sciezka +  "kllklklk_2.jpg",
                sciezka + "kllklklk_4.jpg",
                sciezka + "kllklklk_1.jpg"
            };
        }

        private List<string> PrzygodujDaneTestoweDlaSortowaniaZdjec2()
        {
            return new List<string>()
            {
                sciezka + "3_asdasd.jpg",
                sciezka + "7_asdasd.jpg",
                sciezka + "2_asdasd.jpg"
            };
        }




        [Fact(DisplayName = "Moduł zdjęć z plików - sortowanie zdjęć wg nazwy pliku")]
        public void PosortujZdjeciaTestSortowanieWgNazwy()
        {
            string separator = "_";
            List<string> pliki = PrzygodujDaneTestoweDlaSortowaniaZdjec1();

            ZdjeciaDoProduktowZPlikow modul = A.Fake<ZdjeciaDoProduktowZPlikow>();

            TextHelper texthelper = A.Fake<TextHelper>();

            foreach (string plik in pliki)
            {
                string plik1 = new FileInfo(plik).Name;
                A.CallTo(() => texthelper.OczyscNazwePliku(plik1)).Returns(plik1);
            }

            modul.Texthelper = texthelper;

            List<string> posortowaneZdjecia = modul.PosortujZdjecia(pliki, SortowaniePlikow.NazwaAsc, separator);

            Assert.Equal(pliki[3], posortowaneZdjecia[0]);
            Assert.Equal(pliki[1], posortowaneZdjecia[1]);
            Assert.Equal(pliki[0], posortowaneZdjecia[2]);
            Assert.Equal(pliki[2], posortowaneZdjecia[3]);
        }

        [Fact(DisplayName = "Moduł zdjęć z plików - sortowanie zdjęć wg pierwszego elementu")]
        public void PosortujZdjeciaTestSortowanieWgPierwszegoElementu()
        {
            string separator = "_";
            List<string> pliki = PrzygodujDaneTestoweDlaSortowaniaZdjec2();

            ZdjeciaDoProduktowZPlikow modul = A.Fake<ZdjeciaDoProduktowZPlikow>();

            TextHelper texthelper = A.Fake<TextHelper>();

            foreach (string plik in pliki)
            {
                FileInfo fi = new FileInfo(plik);
                string plik1 = fi.Name;
                A.CallTo(() => texthelper.OczyscNazwePliku(plik1)).Returns(plik1);
                A.CallTo(() => modul.InformacjeOPliku(plik)).Returns(fi);
                A.CallTo(() => modul.WyciagnijIdentyfikatorPlikuZNazwy(plik1, separator))
                    .Returns(plik1.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries).ToList());
            }

            modul.Texthelper = texthelper;

            List<string> posortowaneZdjecia = modul.PosortujZdjecia(pliki, SortowaniePlikow.PierwszyElement, separator);

            Assert.Equal(pliki[2], posortowaneZdjecia[0]);
            Assert.Equal(pliki[0], posortowaneZdjecia[1]);
            Assert.Equal(pliki[1], posortowaneZdjecia[2]);
        }

        [Fact(DisplayName = "Moduł zdjęć z plików - sortowanie zdjęć wg ostatniego elementu")]
        public void PosortujZdjeciaTestSortowanieWgOstatniegoElementu()
        {
            string separator = "_";
            List<string> pliki = PrzygodujDaneTestoweDlaSortowaniaZdjec1();

            ZdjeciaDoProduktowZPlikow modul = A.Fake<ZdjeciaDoProduktowZPlikow>();

            TextHelper texthelper = A.Fake<TextHelper>();

            foreach (string plik in pliki)
            {
                FileInfo fi = new FileInfo(plik);
                string plik1 = fi.Name;
                A.CallTo(() => texthelper.OczyscNazwePliku(plik1)).Returns(plik1);
                A.CallTo(() => modul.InformacjeOPliku(plik)).Returns(fi);
                A.CallTo(() => modul.WyciagnijIdentyfikatorPlikuZNazwy(plik1, separator))
                    .Returns(plik1.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries).ToList());
            }

            modul.Texthelper = texthelper;

            List<string> posortowaneZdjecia = modul.PosortujZdjecia(pliki, SortowaniePlikow.OstatniEement, separator);

            Assert.Equal(pliki[3], posortowaneZdjecia[0]);
            Assert.Equal(pliki[1], posortowaneZdjecia[1]);
            Assert.Equal(pliki[0], posortowaneZdjecia[2]);
            Assert.Equal(pliki[2], posortowaneZdjecia[3]);
        }

        [Fact(DisplayName = "Moduł zdjęć z plików - sortowanie zdjęć wg daty pliku (najstarszego)")]
        public void PosortujZdjeciaTestSortowanieWgDatyUtworzeniaOdNajstarszego()
        {

            List<string> pliki = PrzygodujDaneTestoweDlaSortowaniaZdjec1();

            ZdjeciaDoProduktowZPlikow modul = A.Fake<ZdjeciaDoProduktowZPlikow>();

            TextHelper texthelper = A.Fake<TextHelper>();

            int dzien = 14;
            //w pętli pliki mają nadawaną datę malejąco
            foreach (string plik in pliki)
            {
                FileInfo fi = new FileInfo(plik);
                string plik1 = fi.Name;
                A.CallTo(() => texthelper.OczyscNazwePliku(plik1)).Returns(plik1);
                A.CallTo(() => modul.InformacjeOPliku(plik)).Returns(fi);
                A.CallTo(() => modul.PobierzDateUtworzeniaPliku(fi)).Returns(new DateTime(2014, 6, dzien--));
            }

            modul.Texthelper = texthelper;

            //metoda powinna dla najstarszych plików zwrócić odwrotną listę niż na wejściu do metody
            List<string> posortowaneZdjecia = modul.PosortujZdjecia(pliki, SortowaniePlikow.DataUtworzeniaAsc, "_");

            Assert.Equal(pliki[3], posortowaneZdjecia[0]);
            Assert.Equal(pliki[2], posortowaneZdjecia[1]);
            Assert.Equal(pliki[1], posortowaneZdjecia[2]);
            Assert.Equal(pliki[0], posortowaneZdjecia[3]);
        }

        [Fact(DisplayName = "Moduł zdjęć z plików - sortowanie zdjęć wg daty pliku (najmłodszego)")]
        public void PosortujZdjeciaTestSortowanieWgDatyUtworzeniaOdNajmlodszego()
        {
            List<string> pliki = PrzygodujDaneTestoweDlaSortowaniaZdjec1();

            ZdjeciaDoProduktowZPlikow modul = A.Fake<ZdjeciaDoProduktowZPlikow>();

            TextHelper texthelper = A.Fake<TextHelper>();

            int dzien = 14;
            //w pętli pliki mają nadawaną datę rosnąco
            foreach (string plik in pliki)
            {
                FileInfo fi = new FileInfo(plik);
                string plik1 = fi.Name;
                A.CallTo(() => texthelper.OczyscNazwePliku(plik1)).Returns(plik1);
                A.CallTo(() => modul.InformacjeOPliku(plik)).Returns(fi);
                A.CallTo(() => modul.PobierzDateUtworzeniaPliku(fi)).Returns(new DateTime(2014, 6, dzien++));
            }

            modul.Texthelper = texthelper;

            //metoda powinna dla najmłodszych plików zwrócić odwrotną listę niż na wejściu do metody
            List<string> posortowaneZdjecia = modul.PosortujZdjecia(pliki, SortowaniePlikow.DataUtworzeniaDesc, "_");

            Assert.Equal(pliki[3], posortowaneZdjecia[0]);
            Assert.Equal(pliki[2], posortowaneZdjecia[1]);
            Assert.Equal(pliki[1], posortowaneZdjecia[2]);
            Assert.Equal(pliki[0], posortowaneZdjecia[3]);
        }

        [Fact(DisplayName = "generowanie nazwa zdjęć ze ścieżki")]
        public void DodajPlikiDoProduktowTest1()
        {

            TextHelper texthelper = A.Fake<TextHelper>();
            ZdjeciaDoProduktowZPlikow modul = A.Fake<ZdjeciaDoProduktowZPlikow>();

            modul.NazwaMapowanie = true;
            Dictionary<long, ProduktPlik> plikiLokalnePowiazania = new Dictionary<long, ProduktPlik>();
            List<Plik> plikiLokalne = new List<Plik>();
            string sciezkaPliku = "c:\\judo-koch\\rashguard2\\projekty-graficzne\\visual9.jpg";
            int licznikMinimalny = 0;
            string Separator = "";
            int produkt_id = 10;
            HashSet<string> pliki = new HashSet<string>();
            pliki.Add(sciezkaPliku);
            modul.Texthelper = texthelper;


            string nazwa = "judo-koch\\rashguard2\\projekty-graficzne\\visual9";
            string nazwaPoprawiona = "judo-koch-rashguard2-projekty-graficzne-visual9";
            string nazwaPoprawionaZRozszerzeniem = "judo-koch-rashguard2-projekty-graficzne-visual9.jpg";

            FileInfo fi = new FileInfo(sciezkaPliku);
            string plik1 = fi.Name;
            int licznik = 0;
            Plik plik = modul.UtworzPlik(sciezkaPliku, nazwaPoprawionaZRozszerzeniem, ref licznik);
            plik.Rozmiar = 1000;
            plik.Nazwa = nazwaPoprawionaZRozszerzeniem;
            plik.NazwaBezRozszerzenia = nazwaPoprawiona;
            plik.Id = 1;
            plik.Sciezka = sciezka + "\\";

            A.CallTo(() => texthelper.OczyscNazwePliku(nazwa)).Returns(nazwaPoprawiona);

            A.CallTo(() => modul.UtworzPlik(sciezkaPliku, nazwaPoprawionaZRozszerzeniem, ref licznik)).Returns(plik);
            A.CallTo(() => texthelper.OczyscNazwePliku(plik1)).Returns(plik1);
            A.CallTo(() => modul.InformacjeOPliku(sciezkaPliku)).Returns(fi);
            A.CallTo(() => modul.PobierzDateUtworzeniaPliku(fi)).Returns(new DateTime(2014, 6, 1));
            A.CallTo(() => modul.ZweryfikujTypPliku(A<Plik>.Ignored)).Returns(true);
            //metoda powinna dla najmłodszych plików zwrócić odwrotną listę niż na wejściu do metody
            var wynik = modul.DodajPlikiDoProduktow(plikiLokalnePowiazania, plikiLokalne, sciezkaPliku,
                    licznikMinimalny, Separator, produkt_id, pliki);
            Assert.True(plikiLokalne[0].Nazwa.Equals(nazwaPoprawionaZRozszerzeniem));
        }
    }
}
