//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using FakeItEasy;
//using SolEx.Hurt.Core.BLL;
//using SolEx.Hurt.Core.BLL.Interfejsy;
//using SolEx.Hurt.Core.Importy;
//using SolEx.Hurt.Core.Pomocnicze;
//using SolEx.Hurt.DAL;
//using SolEx.Hurt.Model;using SolEx.Hurt.Model.Interfaces.Modele;
//using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci;
//using Xunit;
//namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci.Tests
//{
//    public class PrzypisaniePrzedstawicieliOpiekunowDoKlientowTests
//    {
//        [Fact()]
//        public void PrzetworzMainTest()
//        {
//            List<kategorie_klientow> katkl = new List<kategorie_klientow>();
//            katkl.Add(new kategorie_klientow() { grupa = "aaa", nazwa = "dupa1", Id = 1 });

//            kategorie_klientow kategoriaopiekun = new kategorie_klientow() { grupa = "opiekun", nazwa = "P1", Id = 2 };
//            katkl.Add(kategoriaopiekun);
//            katkl.Add(new kategorie_klientow() { grupa = "", nazwa = "dupa2", Id = 3 });
//            katkl.Add(new kategorie_klientow() { grupa = "ad;flkjdf", nazwa = "%KL1", Id = 4 });

//            kategorie_klientow kategoriap1 = new kategorie_klientow() { grupa = ConfigBLL.PobierzInstancje.CechaAuto, nazwa = "%P1", Id = 11 };
//            katkl.Add(kategoriap1);

//            kategorie_klientow kategoriap2 = new kategorie_klientow() { grupa = "Grupa główna", nazwa = "drugi przedstawiciel", Id = 33 };
//            katkl.Add(kategoriap2);

//            klienci k = new klienci();
//            k.nazwa = "mietek";
//            k.klient_id = 69;

//            klienci przedstawiciel1 = new klienci();
//            przedstawiciel1.nazwa = "pierwszy przedstawiciel klienta";
//            przedstawiciel1.symbol = "P1";
//            przedstawiciel1.email = "p1@p1.pl";
//            przedstawiciel1.klient_id = 111;

//            klienci przedstawiciel2 = new klienci();
//            przedstawiciel2.nazwa = "drugi przedstawiciel klienta";
//            przedstawiciel2.symbol = "P2";
//            przedstawiciel2.email = "p2@p2.pl";
//            przedstawiciel2.klient_id = 222;

//            List<klienci> przedstawiciele = new List<klienci>() { przedstawiciel1, przedstawiciel2 };

//            List<klienci_kategorie> kliencikategorielaczniki = new List<klienci_kategorie>();
//            kliencikategorielaczniki.Add(new klienci_kategorie() {kategoria_klientow_id = 2, klient_id = 3 });
//            kliencikategorielaczniki.Add(new klienci_kategorie() { kategoria_klientow_id = 2, klient_id = 4 });
//            kliencikategorielaczniki.Add(new klienci_kategorie() {  kategoria_klientow_id = kategoriap1.Id, klient_id = k.klient_id });
//            kliencikategorielaczniki.Add(new klienci_kategorie() { kategoria_klientow_id = kategoriap2.Id, klient_id = k.klient_id });
//            kliencikategorielaczniki.Add(new klienci_kategorie() { kategoria_klientow_id = kategoriaopiekun.Id, klient_id = k.klient_id });

//            Dictionary<int, klienci> klienci = new Dictionary<int, klienci>();
//            klienci.Add(k.klient_id, k);


//            var klienciDostetp = A.Fake<IKlienciWyszukiwanie>();
//            A.CallTo(() => klienciDostetp.PobierzKategorieKlienta(A<IKlienci>.Ignored, A<List<kategorie_klientow>>.Ignored, A<List<klienci_kategorie>>.Ignored, kategoriap2.grupa.ToLower(), true)).Returns(kategoriap2);
//            A.CallTo(() => klienciDostetp.PobierzKategorieKlienta(A<IKlienci>.Ignored, A<List<kategorie_klientow>>.Ignored, A<List<klienci_kategorie>>.Ignored, kategoriaopiekun.grupa.ToLower(), true)).Returns(kategoriaopiekun);
//            A.CallTo(() => klienciDostetp.PobierzKategorieKlienta(A<IKlienci>.Ignored, A<List<kategorie_klientow>>.Ignored, A<List<klienci_kategorie>>.Ignored, "%", true)).Returns(kategoriap1);
//            A.CallTo(() => klienciDostetp.PobierzKategorieKlienta(A<IKlienci>.Ignored, A<List<kategorie_klientow>>.Ignored, A<List<klienci_kategorie>>.Ignored, "opidfssdfekun", true)).Returns(null);
//            PrzypisaniePrzedstawicieliOpiekunowDoKlientow ppodk = new PrzypisaniePrzedstawicieliOpiekunowDoKlientow();
//            ppodk.KlienciDostep = klienciDostetp;

//            ppodk.Grupa = "%";
//            ppodk.TypPracownika = TypyPracownikow.Przedstawiciel;
//            ppodk.SposobPolaczenia = SposobyPolaczenia.Symbol;
//            ppodk.PrzetworzMain(ref klienci, przedstawiciele, katkl, kliencikategorielaczniki);

//            Assert.Equal(przedstawiciel1.klient_id, klienci.First().Value.przedstawiciel_id);

//            ppodk.Grupa = "Grupa główna";
//            ppodk.SposobPolaczenia = SposobyPolaczenia.TylkoDwaPierwszeSlowa_npImieNazwisko;
//            ppodk.PrzetworzMain(ref klienci, przedstawiciele, katkl, kliencikategorielaczniki);

//            Assert.Equal(przedstawiciel2.klient_id, klienci.First().Value.przedstawiciel_id);

//            ppodk.Grupa = "opiekun";
//            ppodk.SposobPolaczenia = SposobyPolaczenia.Symbol;
//            ppodk.PrzetworzMain(ref klienci, przedstawiciele, katkl, kliencikategorielaczniki);

//            Assert.Equal(przedstawiciel1.klient_id, klienci.First().Value.przedstawiciel_id);

//        }


//    }
//}
