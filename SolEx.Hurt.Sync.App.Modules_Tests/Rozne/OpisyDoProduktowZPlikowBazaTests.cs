using System.Collections.Generic;
using FakeItEasy;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.AktualizacjaProduktow;

using Xunit;

namespace SolEx.Hurt.Sync.App.Modules_.Rozne.Tests
{
    public class OpisyDoProduktowZPlikowBazaTests
    {
        [Fact()]
        public void PrzetworzBazaTestDopasowaniaPlikuDoKonkretnegoPolaOpisu()
        {
            OpisyDoProduktowZPlikowBaza o = new OpisyDoProduktowZPlikow();

            //przypadek 1 kiedy plik z opisem nie ma żadnych końcówek i moduł ma wrzucić opis z pliku do pierwszego pola z opisem
            string opis = o.ZnajdzKolejnoscOpisu("plik1.txt", new string[] {}, "_");

            Assert.Equal("", opis);

            //przypadek 2 kiedy plik z opisem ma końcówkę B i ma być wrzucony do drugiego opisu
            string opis2 = o.ZnajdzKolejnoscOpisu("plik1_B.txt", new string[] {"A", "B", "C"}, "_");

            Assert.Equal("opis2", "opis" + opis2);

            //błąd
            string opis3 = o.ZnajdzKolejnoscOpisu("plik1_B.txt", new string[] {"C"}, "_");
            Assert.Equal("", opis3);

            //błąd #2
            string opis4 = o.ZnajdzKolejnoscOpisu("plik1.txt", new string[] {"C"}, "_");
            Assert.Equal("", opis4);

        }

        [Fact()]
        public void PrzetworzBazaTestPelnyTestOpisow()
        {
            OpisyDoProduktowZPlikow opis = A.Fake<OpisyDoProduktowZPlikow>();
            opis.Sciezka = "C:\\pliki";
            opis.Separator = "_";
            Produkt produkt1 = new Produkt();
            produkt1.Kod = "FAJNYKOD1";

            Produkt produkt2 = new Produkt();
            produkt2.Kod = "TAJNY111KOD222";

            List<Produkt> listaproduktow = new List<Produkt>(2) { produkt1, produkt2 };
            List<Tlumaczenie> slowniki = new List<Tlumaczenie>(0);

            string sciezkaPliku1 = "C:\\pliki\\FAJNYKOD1.txt";
            string opisPliku1 = "fajny opis";

            string sciezkaPliku2 = "C:\\pliki\\TAJNY111KOD222_jakiessmieci_2.txt";
            string opisPliku2 = "nie pacz tutaj";

            string sciezkaPliku3 = "C:\\pliki\\TAJNY111KOD222_jakiesinnesmieci_1.txt";
            string opisPliku3 = "masakrujemy lewaków!";

            string[] listaPlikowZOpisami =
            {
                "C:\\pliki\\asdfsadf.txt",
                "C:\\pliki\\asdar3324rtert.asd",
                sciezkaPliku1,
                "C:\\pliki\\asdkkkkkk.txt",
                sciezkaPliku2,
                sciezkaPliku3
            };

            //TypWSystemie typy = new TypWSystemie();
            //typy.Id = 796786173;
            //typy.Nazwa = "SolEx.Hurt.Model.produkty";

            //List<TypWSystemie> systemtypylista = new List<TypWSystemie>(){typy};

            ConfigBLL config = A.Fake<ConfigBLL>();
            //A.CallTo(() => config.SystemTypes).Returns(systemtypylista);
            opis._ConfigBLL = config;

            A.CallTo(() => opis.PobierzPliki(opis.Sciezka)).Returns(listaPlikowZOpisami);

            Tools tools = A.Fake<Tools>();
            opis._Tools = tools;

            A.CallTo(() => tools.PobierzZawartoscPlikuTekstowegoZFormatowaniem(sciezkaPliku1, opis.Kodowanie, true))
                .Returns(opisPliku1);

            A.CallTo(() => tools.PobierzZawartoscPlikuTekstowegoZFormatowaniem(sciezkaPliku2, opis.Kodowanie, true))
                .Returns(opisPliku2);

            A.CallTo(() => tools.PobierzZawartoscPlikuTekstowegoZFormatowaniem(sciezkaPliku3, opis.Kodowanie, true))
                .Returns(opisPliku3);

            LogiFormatki logiFormatki = A.Fake<LogiFormatki>();
            opis._LogiFormatki = logiFormatki;

            A.CallTo(() => logiFormatki.LogujInfo("Plików z opisami: 6"));

            Jezyk jezykpl = new Jezyk();
            jezykpl.Id = 1;
            jezykpl.Nazwa = "pl";
            jezykpl.Symbol = "pl";
            jezykpl.Domyslny = true;
            Dictionary<int, Jezyk> JezykiWSystemie = new Dictionary<int, Jezyk>(1);
            JezykiWSystemie.Add(jezykpl.Id, jezykpl);


            A.CallTo(() => config.JezykiWSystemie).Returns(JezykiWSystemie);

            opis.Przetwarzaj(ref listaproduktow, ref slowniki);

            //produkt 1 ma tylko 1 opis, bez śmieci w nazwie
            Assert.Equal(opisPliku1, produkt1.Opis);

            //produkt 2 ma 2 pliki, które na liście są w innej kolejności, tutaj zadziała sortowanie i opisPliku2 które jest pierwsze na liście ma być w polu opis2
            Assert.Equal(opisPliku2, produkt2.Opis2);

            //a tutaj drugi plik z opisem dla produktu 2 który na liście plików był dalej ale po sortowaniu ma być jako pierwszy dlatego jest dodany do pola opis
            Assert.Equal(opisPliku3, produkt2.Opis);
        }

        [Fact()]
        public void PrzetworzBazaTestPelnyTestOpisowInneRozszerzenie()
        {
            OpisyDoProduktowZPlikow opis = A.Fake<OpisyDoProduktowZPlikow>();
            opis.Sciezka = "C:\\pliki";
            opis.Separator = "_";
            opis.Rozszerzenie = ".htm";
            opis.PoCzymSzukacPlikow = TypyPolDoDopasowaniaZdjecia.PoleTekst1;
            Produkt produkt1 = new Produkt();
            produkt1.PoleTekst1 = "FAJNYKOD1";
            produkt1.Kod = "kod";

            Produkt produkt2 = new Produkt();
            produkt2.Kod = "TAJNY111KOD222";

            List<Produkt> listaproduktow = new List<Produkt>(2) { produkt1, produkt2 };
            List<Tlumaczenie> slowniki = new List<Tlumaczenie>(0);

            string sciezkaPliku1 = "C:\\pliki\\FAJNYKOD1.htm";
            string opisPliku1 = "fajny opis";

            string sciezkaPliku2 = "C:\\pliki\\TAJNY111KOD222_jakiessmieci_2.txt";
            string opisPliku2 = "nie pacz tutaj";

            string sciezkaPliku3 = "C:\\pliki\\TAJNY111KOD222_jakiesinnesmieci_1.txt";
            string opisPliku3 = "masakrujemy lewaków!";

            string[] listaPlikowZOpisami =
            {
                "C:\\pliki\\asdfsadf.txt",
                "C:\\pliki\\asdar3324rtert.asd",
                sciezkaPliku1,
                "C:\\pliki\\asdkkkkkk.txt",
                sciezkaPliku2,
                sciezkaPliku3
            };

            ConfigBLL config = A.Fake<ConfigBLL>();
            
            opis._ConfigBLL = config;

            A.CallTo(() => opis.PobierzPliki(opis.Sciezka)).Returns(listaPlikowZOpisami);

            Tools tools = A.Fake<Tools>();
            opis._Tools = tools;

            A.CallTo(() => tools.PobierzZawartoscPlikuTekstowegoZFormatowaniem(sciezkaPliku1, opis.Kodowanie, true))
                .Returns(opisPliku1);

            A.CallTo(() => tools.PobierzZawartoscPlikuTekstowegoZFormatowaniem(sciezkaPliku2, opis.Kodowanie, true))
                .Returns(opisPliku2);

            A.CallTo(() => tools.PobierzZawartoscPlikuTekstowegoZFormatowaniem(sciezkaPliku3, opis.Kodowanie, true))
                .Returns(opisPliku3);

            LogiFormatki logiFormatki = A.Fake<LogiFormatki>();
            opis._LogiFormatki = logiFormatki;

            A.CallTo(() => logiFormatki.LogujInfo("Plików z opisami: 6"));

            var jezykpl = new Jezyk();
            jezykpl.Id = 1;
            jezykpl.Nazwa = "pl";
            jezykpl.Symbol = "pl";
            jezykpl.Domyslny = true;
            Dictionary<int, Jezyk> JezykiWSystemie = new Dictionary<int, Jezyk>(1);
            JezykiWSystemie.Add(jezykpl.Id, jezykpl);


            A.CallTo(() => config.JezykiWSystemie).Returns(JezykiWSystemie);

            opis.Przetwarzaj(ref listaproduktow, ref slowniki);

            //produkt 1 ma tylko 1 opis, bez śmieci w nazwie
            Assert.Equal(opisPliku1, produkt1.Opis);

            opis.PoCzymSzukacPlikow = TypyPolDoDopasowaniaZdjecia.Kod;
            opis.Przetwarzaj(ref listaproduktow, ref slowniki);

            //produkt 2 ma 2 pliki, które na liście są w innej kolejności, tutaj zadziała sortowanie i opisPliku2 które jest pierwsze na liście ma być w polu opis2
            Assert.Equal(opisPliku2, produkt2.Opis2);

            //a tutaj drugi plik z opisem dla produktu 2 który na liście plików był dalej ale po sortowaniu ma być jako pierwszy dlatego jest dodany do pola opis
            Assert.Equal(opisPliku3, produkt2.Opis);
        }

        [Fact()]
        public void PrzetworzBazaTest1()
        {
            OpisyDoProduktowZPlikowBaza o = new OpisyDoProduktowZPlikow();
            o.PoCzymSzukacPlikow = TypyPolDoDopasowaniaZdjecia.Kod;

            string[] pliki = new string[2] {"C:\\pliki\\12345.txt", "C:\\pliki\\231231_1.txt"};

            var slownikPlikow = o.StworzSlownikPlikow(pliki);

            Produkt p = new Produkt();
            p.Kod = "12345";

            Produkt p2 = new Produkt();
            p2.Kod = "231231";

            
            List<Produkt> produktyList = new List<Produkt>(){p, p2};


            Dictionary<string, List<string>> plikiProduktow = o.DopasujPlikiDoProduktow(produktyList, slownikPlikow);
        }

        [Fact()]
        public void PrzetworzBazaTest2()
        {
            OpisyDoProduktowZPlikowBaza o = new OpisyDoProduktowZPlikow();
            o.PoCzymSzukacPlikow = TypyPolDoDopasowaniaZdjecia.PoleTekst1;
            o.Rozszerzenie = ".htm";
            string[] pliki = new string[2] {"C:\\pliki\\12345.htm", "C:\\pliki\\231231.htm"};

            var slownikPlikow = o.StworzSlownikPlikow(pliki);

            Produkt p = new Produkt();
            p.PoleTekst1 = "12345";

            Produkt p2 = new Produkt();
            p2.PoleTekst1 = "231231";


            List<Produkt> produktyList = new List<Produkt>() { p, p2 };


            Dictionary<string, List<string>> plikiProduktow = o.DopasujPlikiDoProduktow(produktyList, slownikPlikow);
        }

         [Fact()]
         public void PrzetworzBazaTest()
         {
             Produkt p1 = new Produkt { Id = 1, Kod = "41060.green.s", KodKreskowy = "5901947804070" };
             Produkt p2 = new Produkt { Id = 2, Kod = "JakisKod2", KodKreskowy = "JakisKodKreskowy" };
             List<Produkt> produktyNaB2B = new List<Produkt> { p1, p2 };
             List<Tlumaczenie> slowniki = new List<Tlumaczenie>();

             //TypWSystemie typy = new TypWSystemie();
             //typy.Id = 796786173;
             //typy.Nazwa = "SolEx.Hurt.Model.produkty";
             //List<TypWSystemie> listaTypow = new List<TypWSystemie>() { typy };

             var config = A.Fake<ConfigBLL>();
             //A.CallTo(() => config.SystemTypes).Returns(listaTypow);

            OpisyDoProduktowZPlikow op = new OpisyDoProduktowZPlikow();
            //op.Kodowanie=Opisy.KodowanieOpisow.Dopasuj;
            op._ConfigBLL = config;
            op.Separator = "_";
            op.Sciezka = "c:\\pliki";
            op.KluczOpisow = "opisMarket;opisTechniczny";
            op.PoCzymSzukacPlikow = TypyPolDoDopasowaniaZdjecia.Kod;
            op.PrzetworzBaza(ref produktyNaB2B, ref slowniki);

            Assert.Equal(produktyNaB2B[0].Opis,"To jest opis testowy<br/>Druga linia");
         }



         [Fact()]
         public void PrzetworzBazaTest3()
         {
             OpisyDoProduktowZPlikow modul = A.Fake<OpisyDoProduktowZPlikow>();
             modul.Sciezka = "C:\\pliki";
             modul.Separator = "";
             modul.Rozszerzenie = ".htm";
             modul.PoCzymSzukacPlikow = TypyPolDoDopasowaniaZdjecia.PoleTekst1;
             Produkt produkt1 = new Produkt();
             produkt1.PoleTekst1 = "250";
             produkt1.Kod = "kod";

             Produkt produkt2 = new Produkt();
             produkt2.PoleTekst1 = "251";
             produkt2.Kod = "kod";

             Produkt produkt3 = new Produkt();
             produkt3.PoleTekst1 = "252";
             produkt3.Kod = "kod";

             Produkt produkt4 = new Produkt();
             produkt4.PoleTekst1 = "254";
             produkt4.Kod = "kod";

             List<Produkt> listaproduktow = new List<Produkt>(3) { produkt1, produkt2, produkt3, produkt4 };
             List<Tlumaczenie> slowniki = new List<Tlumaczenie>(0);

            string sciezkaPliku1 = "C:\\pliki\\250.htm";
            string opisPliku1 = "<!DOCTYPE HTML PUBLIC '-//W3C//DTD HTML 4.0 Transitional//EN'>";

            string sciezkaPliku2 = "C:\\pliki\\251.htm";
            string opisPliku2 = "<b>opis produktu 2 nr 251</b>";

            string sciezkaPliku3 = "C:\\pliki\\252.htm";
            string opisPliku3 = "<b>opis produktu 3 nr 252</b>";


            string sciezkaPliku4 = "C:\\pliki\\254.htm";
            string opisPliku4 = "<b>opis produktu 4 nr 254</b>";

            string sciezkaPlikuZla4 = "C:\\pliki\\smieci254.htm";
            string opisPlikuZly4 = "<b>zły opis produktu 4 nr 254</b>";

            var listaPlikowZOpisami = new []
            {
                "C:\\pliki\\smiec1.txt",
                "C:\\pliki\\smiec2.asd",
                sciezkaPliku1,
                "C:\\pliki\\smiec3.txt",
                sciezkaPliku2,
                sciezkaPliku3,
                sciezkaPlikuZla4
            };

             ConfigBLL config = A.Fake<ConfigBLL>();
             modul._ConfigBLL = config;

            A.CallTo(() => modul.PobierzPliki(modul.Sciezka)).Returns(listaPlikowZOpisami);

            Tools tools = A.Fake<Tools>();
            modul._Tools = tools;

            A.CallTo(() => tools.PobierzZawartoscPlikuTekstowegoZFormatowaniem(sciezkaPliku1, modul.Kodowanie, true))
                .Returns(opisPliku1);

            A.CallTo(() => tools.PobierzZawartoscPlikuTekstowegoZFormatowaniem(sciezkaPliku2, modul.Kodowanie, true))
                .Returns(opisPliku2);

            A.CallTo(() => tools.PobierzZawartoscPlikuTekstowegoZFormatowaniem(sciezkaPliku3, modul.Kodowanie, true))
                .Returns(opisPliku3);

            A.CallTo(() => tools.PobierzZawartoscPlikuTekstowegoZFormatowaniem(sciezkaPliku4, modul.Kodowanie, true))
              .Returns(opisPliku4);

            A.CallTo(() => tools.PobierzZawartoscPlikuTekstowegoZFormatowaniem(sciezkaPlikuZla4, modul.Kodowanie, true))
                .Returns(opisPlikuZly4);

             Jezyk jezykpl = new Jezyk();
             jezykpl.Id = 1;
             jezykpl.Nazwa = "pl";
             jezykpl.Symbol = "pl";
             jezykpl.Domyslny = true;
             Dictionary<int, Jezyk> JezykiWSystemie = new Dictionary<int, Jezyk>(1);
             JezykiWSystemie.Add(jezykpl.Id, jezykpl);

             LogiFormatki logiFormatki = A.Fake<LogiFormatki>();
            A.CallTo(() => logiFormatki.LogujInfo("Plików z opisami: 6"));

             modul.Przetwarzaj(ref listaproduktow, ref slowniki);
             Assert.Equal(opisPliku1, produkt1.Opis);

             modul.Przetwarzaj(ref listaproduktow, ref slowniki);
             Assert.Equal(opisPliku2, produkt2.Opis);

             modul.Przetwarzaj(ref listaproduktow, ref slowniki);
             Assert.Equal(opisPliku3, produkt3.Opis);
         }
    }
}
