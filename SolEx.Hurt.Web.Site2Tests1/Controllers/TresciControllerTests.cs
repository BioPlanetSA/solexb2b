using Xunit;
using SolEx.Hurt.Web.Site2.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Controllers.Tests
{
    public class TresciControllerTests
    {

        public class TestPliki
        {
            public string NazwaNaDysku { get; set; }
            public string LinkOczekiwany { get; set; }
            public string NazwaLadnaOczekiwana { get; set; }
        }

        [Fact(DisplayName = "Pobieranie plikow z dysku do kontrolki plikow")]
        public void PobierzListePlikowDoKontroliPlikowTest()
        {
            string sciezkaBazowaPlikowMock = "pliki2/test/";
            string sciekzUzytkownika = @"pliki super fajne\drug podkatalog";

            string aktualanSciezkaZasobowMockBezwzgledna = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "zasoby", sciezkaBazowaPlikowMock);
            string aktualnaSciezkaUzytkownikaBezwzgledna = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, aktualanSciezkaZasobowMockBezwzgledna, sciekzUzytkownika);

            if (Directory.Exists(aktualanSciezkaZasobowMockBezwzgledna))
            {
                this.DeleteDirectory(aktualanSciezkaZasobowMockBezwzgledna);
            }

            Directory.CreateDirectory(aktualanSciezkaZasobowMockBezwzgledna);
            Directory.CreateDirectory(aktualnaSciezkaUzytkownikaBezwzgledna);
        

            List<TestPliki> testowePliki = new List<TestPliki>
            {
                new TestPliki {NazwaNaDysku = "test_etr34_35 - 53 d,txt.txt", NazwaLadnaOczekiwana = "test etr34 35 53 d,txt", LinkOczekiwany = $"/zasoby/{sciezkaBazowaPlikowMock}/test_etr34_35 - 53 d,txt.txt"},
                new TestPliki {NazwaNaDysku = "plik.png", NazwaLadnaOczekiwana = "plik", LinkOczekiwany = $"/zasoby/{sciezkaBazowaPlikowMock}/plik.png"},
                new TestPliki {NazwaNaDysku = "pli-1k.png", NazwaLadnaOczekiwana = "pli 1k", LinkOczekiwany = $"/zasoby/{sciezkaBazowaPlikowMock}/pli-1k.png"},
                new TestPliki {NazwaNaDysku = "---__--plik__---1.png", NazwaLadnaOczekiwana = "plik 1", LinkOczekiwany = $"/zasoby/{sciezkaBazowaPlikowMock}/---__--plik__---1.png"},
                new TestPliki {NazwaNaDysku = "---__------pl----i___k  1.png", NazwaLadnaOczekiwana = "pl i k 1", LinkOczekiwany = $"/zasoby/{sciezkaBazowaPlikowMock}/---__------pl----i___k  1.png"},
            };

            List<TestPliki> testoweFoldery = new List<TestPliki>
            {
                new TestPliki {NazwaNaDysku = "pliki super fajne/drug podkatalog", NazwaLadnaOczekiwana = "drug podkatalog", LinkOczekiwany ="http://wp.pl/tresc/super/strona?path=" + new UrlHelper().Encode( "pliki super fajne/drug podkatalog")},
                new TestPliki {NazwaNaDysku = "pliki super fajne/---drug podkat__alog--1", NazwaLadnaOczekiwana = "drug podkat alog 1", LinkOczekiwany ="http://wp.pl/tresc/super/strona?path=" + new UrlHelper().Encode( "pliki super fajne/---drug podkat__alog--1")},
                new TestPliki {NazwaNaDysku = "pliki super fajne/drug podkatalog/n---owy katalog", NazwaLadnaOczekiwana = "n owy katalog", LinkOczekiwany =  "http://wp.pl/tresc/super/strona?path=" + new UrlHelper().Encode( "pliki super fajne\\drug podkatalog\\n---owy katalog")},
                new TestPliki {NazwaNaDysku = "pliki super fajne/---drug podkat__alog--1/vdddd    ", NazwaLadnaOczekiwana = "vdddd", LinkOczekiwany = "http://wp.pl/tresc/super/strona?path=" + new UrlHelper().Encode("pliki super fajne/---drug podkat__alog--1/vdddd") },
            };

          
            foreach (var plikTest in testowePliki)
            {
                //pliki
                System.IO.File.Create(Path.Combine(aktualanSciezkaZasobowMockBezwzgledna, plikTest.NazwaNaDysku));
                System.IO.File.Create(Path.Combine(aktualnaSciezkaUzytkownikaBezwzgledna, plikTest.NazwaNaDysku));
            }

            foreach (var plikTest in testoweFoldery)
            {
                //katalogi bez rozszerzen
                string path = Path.Combine(aktualanSciezkaZasobowMockBezwzgledna, plikTest.NazwaNaDysku);
                Directory.CreateDirectory(path);
            }

            string urlDlaKatalogow = "http://wp.pl/tresc/super/strona";

            TresciController tresci = new TresciController();

            var wynik = tresci.PobierzListePlikowDoKontroliPlikow(sciezkaBazowaPlikowMock, sciekzUzytkownika, urlDlaKatalogow, true);

            Assert.True( wynik.Item1.Any(x=> x.Typ == TypZasobu.Katalog) && wynik.Item1.Any(x => x.Typ == TypZasobu.Plik), "powinny być zarówno katalogi jak i pliki");

            foreach (ListaPlikowModel w in wynik.Item1)
            {
                if (w.Typ == TypZasobu.Plik)
                {
                    string klucz = Path.GetFileName(w.Link);
                    var oczekiwanaWartosc = testowePliki.First(x => x.NazwaNaDysku == klucz);

                    Assert.True(oczekiwanaWartosc.NazwaLadnaOczekiwana == w.NazwaBezRoszerzenia, $"Oczekiwano: {oczekiwanaWartosc.NazwaLadnaOczekiwana} a wyszło: {w.NazwaBezRoszerzenia}");

                    string linkOczekiwany = Path.Combine("zasoby", sciezkaBazowaPlikowMock, sciekzUzytkownika, klucz);
                    Assert.True(w.Link == linkOczekiwany);
                }

                if (w.Typ == TypZasobu.Katalog)
                {
                    var wartoscZSlownika = testoweFoldery.First(x => x.NazwaNaDysku.EndsWith(w.NazwaOryginalna) );
                    Assert.True(w.Link == wartoscZSlownika.LinkOczekiwany, $"oczekiwany link:\n '{wartoscZSlownika.LinkOczekiwany}' a otrzymany:\n '{w.Link}'");
                }
            }

            string spodziewanyLink = urlDlaKatalogow + "?path=" + new UrlHelper().Encode( "pliki super fajne" );
            Assert.True(wynik.Item2 == spodziewanyLink, $"Wynik otrzymano: {wynik.Item2} a spodziewano: '{spodziewanyLink}' ");

            //wywolanie bez sciekiz klienta BZ katalogow
            wynik = tresci.PobierzListePlikowDoKontroliPlikow(sciezkaBazowaPlikowMock, null, urlDlaKatalogow, false);

            Assert.True(wynik.Item2 == null);
            Assert.True( wynik.Item1.All( x=> x.Typ == TypZasobu.Plik), "tylko pliki powinny byc" );

            //spradzenie sciezki do rodzica jak jeden poziom tylko katalogu izytkownia podanego

            wynik = tresci.PobierzListePlikowDoKontroliPlikow(sciezkaBazowaPlikowMock, "pliki super fajne",  urlDlaKatalogow, false);
            Assert.True(wynik.Item2 == urlDlaKatalogow); 
        }

        private void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                System.IO.File.SetAttributes(file, FileAttributes.Normal);
                System.IO.File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, true);
        }
    }
}