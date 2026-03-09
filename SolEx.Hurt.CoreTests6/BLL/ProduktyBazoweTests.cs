using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FakeItEasy;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.Pomocnicze;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using Xunit;

namespace SolEx.Hurt.CoreTests.BLL
{
    public class ProduktyBazoweTests
    {
        [Fact()]
        public void PobierzTypStanuTest()
        {
            //TODO sprawdzić czy jest ok w testach jeśli jest odpalana metoda Reset() - dodałem ją żeby produktybazowe miała nullowe wartości na wejściu ale nie wiem jak jest w realnych warunkach
            //mock configa
            var ConfigBllFake = A.Fake<IConfigBLL>();
            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            A.CallTo(() => calosc.Konfiguracja).Returns(ConfigBllFake);
            ProduktyBazowe modul = A.Fake<ProduktyBazowe>(x => x.WithArgumentsForConstructor(new[] { calosc }));
            
            //ustawienie cech specjalnych
            A.CallTo(() => ConfigBllFake.ProduktyNaWyczerpaniu_procentStanuMinimalnego).Returns((decimal)0.50);
            A.CallTo(() => ConfigBllFake.ProduktyNieDostepnePrzezDluzszyCzas_iloscDni).Returns(10);
               A.CallTo(() => ConfigBllFake.ProduktyNieDostepnePrzezDluzszyCzas_iloscDni).Returns(10);
            var produkt = A.Fake<ProduktBazowy>(a => a.WithArgumentsForConstructor(new Object[]{1} ));

            //cykliczna dostawa - najblizsza dostawa inna niz null
            A.CallTo(() => produkt.NajblizszaDostawa).Returns(DateTime.Now);
            var wynik = modul.WyliczTypStanu(produkt);
            Assert.Equal(wynik, TypStanu.cykliczna_dostawa);

            //cykliczna dostawa - wpisana data dostawy dluga - czyli towar niedostpeny dluzszy czas
            A.CallTo(() => produkt.DostawaData).Returns(DateTime.Now.AddYears(99)); //kosmiczna data przyszlosci
            A.CallTo(() => produkt.NajblizszaDostawa).Returns(DateTime.Now);
            produkt.Dostawa=  "2020-10-10";
            wynik = modul.WyliczTypStanu(produkt);
            Assert.Equal(wynik, TypStanu.niedostepny_dluzszy_czas);

            // dostepny > 0 - NaStanie
            A.CallTo(() => produkt.NajblizszaDostawa).Returns(null);
            A.CallTo(() => produkt.NaStanie).Returns(true);
            A.CallTo(() => produkt.DostawaData).Returns(null);
            produkt.Dostawa = null;
            wynik = modul.WyliczTypStanu(produkt);
            Assert.Equal(wynik, TypStanu.na_stanie);

            //w dostawie - jesli jest BRAK i jest dostawa wpisana. czuli tutaj powinno byc DOStepne bo jest na stanie, ale ma dostawe wpisana
            produkt.Dostawa = "testowe cokolwiek";
            wynik = modul.WyliczTypStanu(produkt);
            Assert.Equal(wynik, TypStanu.na_stanie);


            //brak i jest wpisan dostawa - czyli ze jest w dostawie
            A.CallTo(() => produkt.NaStanie).Returns(false);
            wynik = modul.WyliczTypStanu(produkt);
            Assert.Equal(wynik, TypStanu.w_dostawie);
            
            //niedostepny dluzszy czas - tak samo jak wyzej, ale musi byc data wieksz niz x dni
            A.CallTo(() => produkt.DostawaData).Returns(DateTime.Now.AddYears(99)); //kosmiczna data przyszlosci
            wynik = modul.WyliczTypStanu(produkt);
            Assert.Equal(wynik, TypStanu.niedostepny_dluzszy_czas);

            //wywalamy w dostawie - tylko BRAK
            produkt.Dostawa = "";
            wynik = modul.WyliczTypStanu(produkt);
            Assert.Equal(wynik, TypStanu.brak);
            //na wyczerpaniu - jeśli ilość produktu jest mniejsza od ustawienia produkty-na-wyczerpaniu-procentstanuminimalnego 
            produkt.StanMin = 100;
            A.CallTo(() => produkt.NaStanie).Returns(true);    //stan minimlany to 0.5 - wiec jak jest ponizej 49 ma pokazac ponizej minimum
            wynik = modul.WyliczTypStanu(produkt);
            Assert.Equal(wynik, TypStanu.na_wyczerpaniu);
            //na zamowienie - jesli NIE ma go na stanie i ma ceche
            A.CallTo(() => ConfigBllFake.ProduktyNaZamowienieCechaID).Returns(1);
            A.CallTo(() => produkt.Cechy).Returns(new Dictionary<long, CechyBll> {{1, new CechyBll()}, {2, new CechyBll()}});
            A.CallTo(() => produkt.NaStanie).Returns(false);  //nie moze byc go na stanie
            wynik = modul.WyliczTypStanu(produkt);
            Assert.Equal(wynik, TypStanu.na_zamowienie);
            //dropshiping
            A.CallTo(() => ConfigBllFake.ProduktyDropshipingCechaID).Returns(2);
            A.CallTo(() => ConfigBllFake.ProduktyDropshipingPokazujNaStanieJesliJest).Returns(false);
            wynik = modul.WyliczTypStanu(produkt);
            Assert.Equal(wynik, TypStanu.dropshiping);
            //na stanie jeśli jest cecha dla dropshipingu ale dropshiping jest nadpisany ustawieniem ProduktyDropshipingPokazujNaStanieJesliJest
            A.CallTo(() => ConfigBllFake.ProduktyDropshipingCechaID).Returns(2);
            A.CallTo(() => ConfigBllFake.ProduktyDropshipingPokazujNaStanieJesliJest).Returns(true);
            A.CallTo(() => produkt.NaStanie).Returns(true);
            wynik = modul.WyliczTypStanu(produkt);
            Assert.Equal(wynik, TypStanu.na_stanie);




            string symbolnd = "nd";
           // A.CallTo(() => ConfigBllFake.ProduktyNieDostepnePrzezDluzszyCzasCecha).Returns(symbolnd);
            var c = new Dictionary<long, CechyBll>();
            c.Add(1, new CechyBll() { Id = 1, Symbol = symbolnd });
            A.CallTo(() => produkt.Cechy).Returns(c);
            wynik = modul.WyliczTypStanu(produkt);
            Assert.Equal(wynik, TypStanu.niedostepny_dluzszy_czas);
        }

        [Fact(DisplayName = "Test wydajnościowy metody pobierajacej typ stanu")]
        public void PobierzTypStanuTestWydajnosc()
        {
            //mock configa
            var ConfigBllFake = A.Fake<ConfigBLL>();
            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            A.CallTo(() => calosc.Konfiguracja).Returns(ConfigBllFake);
            ProduktyBazowe modul = A.Fake<ProduktyBazowe>(x => x.WithArgumentsForConstructor(new[] { calosc }));
            var produkt = A.Fake<ProduktBazowy>(a => a.WithArgumentsForConstructor(new Object[] { 1 }));

            //200 cech do produktu
            var slownikCech = new Dictionary<long, CechyBll>(200);
            for (int i = 0; i < 200; ++i)
            {
              slownikCech.Add(i, new CechyBll());  
            }
            A.CallTo(() => produkt.Cechy).Returns(slownikCech);

           // A.CallTo(() => ConfigBllFake.ProduktyNieDostepnePrzezDluzszyCzasCecha).Returns("");
            A.CallTo(() => ConfigBllFake.ProduktyDropshipingCechaID).Returns(5663);    //dropshiping
            A.CallTo(() => ConfigBllFake.ProduktyNaWyczerpaniu_procentStanuMinimalnego).Returns((decimal)0.50);
            A.CallTo(() => ConfigBllFake.ProduktyNieDostepnePrzezDluzszyCzas_iloscDni).Returns(10);
            A.CallTo(() => ConfigBllFake.ProduktyNaZamowienieCechaID).Returns(458);
            
            A.CallTo(() => produkt.NaStanie).Returns(false);
            A.CallTo(() => produkt.NajblizszaDostawa).Returns(null);

            var timer = Stopwatch.StartNew();
            for (int i = 0; i < 1000; ++i)
            {
                modul.WyliczTypStanu(produkt);
            }
            timer.Stop();
            Assert.True(timer.Elapsed.Seconds < 1);  //mniej niz 1 sekund dla calej paetli 1000 iteracji

            timer = Stopwatch.StartNew();

            A.CallTo(() => produkt.DostawaData).Returns(DateTime.Now);
            
            for (int i = 0; i < 500; ++i)
            {
                modul.WyliczTypStanu(produkt);
            }

            timer.Stop();
            Assert.True( timer.Elapsed.Seconds < 1);  //mniej niz 5 sekund dla calej paetli 1000 iteracji
        }

        //[Fact()]
        //public void MoznaDodacDoKoszykaTest()
        //{
        //    var ConfigBllFake = A.Fake<ConfigBLL>();
        //    ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
        //    A.CallTo(() => calosc.Konfiguracja).Returns(ConfigBllFake);
        //    ProduktyBazowe modul = A.Fake<ProduktyBazowe>(x => x.WithArgumentsForConstructor(new[] { calosc }));
        //    var produkt = A.Fake<ProduktBazowy>(a => a.WithArgumentsForConstructor(new Object[] { 1 }));

        //    A.CallTo(() => ConfigBllFake.BlokujDodawanieDoKoszykaDlaBrakujacychProduktow).Returns(false);

        //    //normalny produkt - mozna dodac do koszyka
        //    bool wynik = modul.MoznaDodacDoKoszyka( produkt);
        //    Assert.Equal(wynik, true);

        //    //nie mozna dodac jesli typ niedostepny dluzszy czas
        //    A.CallTo(() => modul.PobierzTypStanu(produkt)).Returns(TypStanu.niedostepny_dluzszy_czas);
        //    wynik = modul.MoznaDodacDoKoszyka(produkt);
        //    Assert.Equal(wynik, false);

        //    //nie mozna dodac jesli jest brak i ustawieni UkryjDodawanieDoKoszykaDlaBrakujacychProduktow
        //    A.CallTo(() => ConfigBllFake.BlokujDodawanieDoKoszykaDlaBrakujacychProduktow).Returns(true);
        //    A.CallTo(() => modul.PobierzTypStanu(produkt)).Returns(TypStanu.w_dostawie);
        //    wynik = modul.MoznaDodacDoKoszyka(produkt);
        //    Assert.Equal(wynik, false);

        //    A.CallTo(() => modul.PobierzTypStanu(produkt)).Returns(TypStanu.brak);
        //    wynik = modul.MoznaDodacDoKoszyka(produkt);
        //    Assert.Equal(wynik, false);

        //    //a jak bedzie na satnie to mozna
        //    A.CallTo(() => modul.PobierzTypStanu(produkt)).Returns(TypStanu.na_stanie);
        //    wynik = modul.MoznaDodacDoKoszyka(produkt);
        //    Assert.Equal(wynik, true);

        //}

        //[Fact()]
        //public void MoznaDodacDoPoinformujODostepnosciTest()
        //{
        //    var ConfigBllFake = A.Fake<ConfigBLL>();
        //    ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
        //    A.CallTo(() => calosc.Konfiguracja).Returns(ConfigBllFake);
        //    ProduktyBazowe ProduktyBazoweFake = A.Fake<ProduktyBazowe>(x => x.WithArgumentsForConstructor(new[] { calosc }));
        //    var produkt = A.Fake<ProduktBazowy>(a => a.WithArgumentsForConstructor(new Object[] { 1 }));

        //    A.CallTo(() => ProduktyBazoweFake.PobierzTypStanu(produkt)).Returns(TypStanu.brak);
        //    bool wynik = ProduktyBazoweFake.MoznaDodacDoPoinformujODostepnosci(produkt);
        //    Assert.Equal(wynik, true);

        //    A.CallTo(() => ProduktyBazoweFake.PobierzTypStanu(produkt)).Returns(TypStanu.cykliczna_dostawa);
        //    wynik = ProduktyBazoweFake.MoznaDodacDoPoinformujODostepnosci(produkt);
        //    Assert.Equal(wynik, false);

        //    A.CallTo(() => ProduktyBazoweFake.PobierzTypStanu(produkt)).Returns(TypStanu.dropshiping);
        //    wynik = ProduktyBazoweFake.MoznaDodacDoPoinformujODostepnosci(produkt);
        //    Assert.Equal(wynik, false);

        //    A.CallTo(() => ProduktyBazoweFake.PobierzTypStanu(produkt)).Returns(TypStanu.na_stanie);
        //    wynik = ProduktyBazoweFake.MoznaDodacDoPoinformujODostepnosci(produkt);
        //    Assert.Equal(wynik, false);

        //    A.CallTo(() => ProduktyBazoweFake.PobierzTypStanu(produkt)).Returns(TypStanu.na_wyczerpaniu);
        //    wynik = ProduktyBazoweFake.MoznaDodacDoPoinformujODostepnosci(produkt);
        //    Assert.Equal(wynik, false);

        //    A.CallTo(() => ProduktyBazoweFake.PobierzTypStanu(produkt)).Returns(TypStanu.na_zamowienie);
        //    wynik = ProduktyBazoweFake.MoznaDodacDoPoinformujODostepnosci(produkt);
        //    Assert.Equal(wynik, false);

        //    A.CallTo(() => ProduktyBazoweFake.PobierzTypStanu(produkt)).Returns(TypStanu.niedostepny_dluzszy_czas);
        //    wynik = ProduktyBazoweFake.MoznaDodacDoPoinformujODostepnosci(produkt);
        //    Assert.Equal(wynik, false);

        //    A.CallTo(() => ProduktyBazoweFake.PobierzTypStanu(produkt)).Returns(TypStanu.w_dostawie);
        //    wynik = ProduktyBazoweFake.MoznaDodacDoPoinformujODostepnosci(produkt);
        //    Assert.Equal(wynik, true);
        //}

        [Fact()]
        public void PasujeDoSzukaniaTest()
        {
            List<ProduktBazowy> produkty = WygenerujProduktyBazowe(100);
            ISolexBllCalosc isc = A.Fake<ISolexBllCalosc>();
            ProduktyBazowe pb = new ProduktyBazowe(isc);
            string szukanaFraza = "";
            List<string> pola = new List<string> { "KodKreskowy", "Kod" };
            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            ConfigBLL config = A.Fake<ConfigBLL>();
            A.CallTo(() => calosc.Konfiguracja).Returns(config);
            
            foreach (ProduktBazowy bazowy in produkty)
            {
                szukanaFraza = bazowy.KodKreskowy;
                Assert.True(pb.PasujeDoSzukania(bazowy, szukanaFraza));
            }
                szukanaFraza = "5999";
            bool wynik = false;
            foreach (ProduktBazowy bazowy in produkty)
            {
                if (pb.PasujeDoSzukania(bazowy, szukanaFraza, pola))
                {
                    wynik = true;
                }
            }
            Assert.True(wynik);

        }

        [Fact()]
        public void PasujeDoSzukaniaTestWydajnosciowy()
        {

            //wersja 1 - dla 16tys produktów czas łączny (s): 3.3
            //07.07.2014 13:00 wersja 2 - dla 16tys produktów czas łączny (s): 2.4
            //07.07.2014 14:00 wersja 3 - dla 16tys produktów czas łączny (s): 2.32
            //07.07.2014 14:34 wersja 4 - dla 16tys produktów czas łączny (s): 0.56 (0.32 bez debug)

            ConfigBLL config = A.Fake<ConfigBLL>();
         
            List<ProduktBazowy> produkty = WygenerujProduktyBazowe(16000);
            string szukanaFraza = "TAK";
            A.CallTo(() => config.SzumyWyszukiwania).Returns("- ");
            A.CallTo(() => config.PodczasWyszukiwaniaZmienPolskeZnaki).Returns(true);
            ProduktyKodyDodatkoweBll kody = A.Fake<ProduktyKodyDodatkoweBll>();

            Dictionary<long, List<ProduktyKodyDodatkowe>> kodyWygenerowane = WygenerujKodyKreskoweDlaProduktow(produkty);
            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            A.CallTo(() => calosc.Konfiguracja).Returns(config);
            foreach (var produktBazowy in produkty)
            {
                //   produktBazowy.KodyDodatkoweProdukty = kody;
                A.CallTo(() => kody.PobierzKodyProduktu(produktBazowy.Id))
                    .Returns(kodyWygenerowane[produktBazowy.Id]);
            }
           // pb.Cache = cache;
            var szukanie = Szukanie.PobierzInstancje;
            szukanie.Config = config;
           
            List<string> pola =
                "nazwa;kod;kod_kreskowy;KategorieTekstowo;pole_tekst1;opis".Split(';').ToList();

            A.CallTo(() => config.ProduktyWyszukiwanie).Returns(pola);
            Stopwatch stoper = Stopwatch.StartNew();
            szukanie.WyszukajObiekty(produkty, szukanaFraza, pola);

            stoper.Stop();

            Assert.True(stoper.Elapsed.TotalSeconds < 0.4, stoper.Elapsed.TotalSeconds.ToString());
        }

        private List<string> _produktyWyszukiwanie;

        public List<string> ProduktyWyszukiwanie
        {
            get
            {
                if (_produktyWyszukiwanie == null)
                {
                    _produktyWyszukiwanie =
                        "nazwa;kod;kod_kreskowy;KategorieTekstowo;DodatkoweKodyString;pole_tekst1;opis"
                            .Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries)
                            .ToList();
                }
                return _produktyWyszukiwanie;
            }
        }


        [Fact()]
        public void SplitowanieStringaTestWydajnosciowy()
        {
            //prosty test pokazujący ile czasu trwa ciągłe splitowanie stringa przy zadanej ilości iteracji
            //przy 10000 iteracji splitowanie zajmuje łącznie 18ms, kiedy kolekcja jest pobierana z cache'u to zajmuje to 1ms
            int ileIteracji = 10000;
            Stopwatch stoper = Stopwatch.StartNew();
            for (int i = 0; i < ileIteracji; i++)
            {
                List<string> pola =
                    "nazwa;kod;kod_kreskowy;KategorieTekstowo;DodatkoweKodyString;pole_tekst1;opis".Split(';').ToList();
            }
            stoper.Stop();
            int lacznyCzas = stoper.Elapsed.Milliseconds;
            stoper = Stopwatch.StartNew();
            for (int i = 0; i < ileIteracji; i++)
            {
                var t = ProduktyWyszukiwanie;
            }
            stoper.Stop();
        }

        private ProduktyKodyDodatkowe KodKreskowyDlaProduktu(long idproduktu)
        {
            ProduktyKodyDodatkowe kod = new ProduktyKodyDodatkowe();
            kod.Id = (int) idproduktu;
            kod.ProduktId = (int)idproduktu;
            kod.Kod =  string.Format("00{0}11{0}", idproduktu);

            return kod;
        }

        private Dictionary<long, List<ProduktyKodyDodatkowe>> WygenerujKodyKreskoweDlaProduktow(List<ProduktBazowy> produkty)
        {
            Dictionary<long, List<ProduktyKodyDodatkowe>> slownik = new Dictionary<long, List<ProduktyKodyDodatkowe>>();
            foreach (ProduktBazowy produktBazowy in produkty)
            {
               if(!slownik.ContainsKey(produktBazowy.Id))
                    slownik.Add(produktBazowy.Id, new List<ProduktyKodyDodatkowe>() { KodKreskowyDlaProduktu(produktBazowy.Id) });
            }

            return slownik;
        }

        private List<ProduktBazowy> WygenerujProduktyBazowe(int ile)
        {
            List<ProduktBazowy> lista = new List<ProduktBazowy>(ile);

            for (int i = 0; i < ile; i++)
            {
                ProduktBazowy produkt = new ProduktBazowy(1);
                produkt.Id = i;
                produkt.Kod = "KOD" + i;
                produkt.KodKreskowy = "59"+i;
                produkt.Opis = i%2 == 0 ? "TIK" : "TAK";
                lista.Add(produkt);
            }

            return lista;
        }

        private List<Cecha> DaneTestPobierania(out List<ProduktCecha> cp, string separator, int liczacech)
        {
            List<Cecha> cechy = new List<Cecha>();
            for (int i = 1; i <= liczacech; i++)
            {
                cechy.Add(new Cecha { Id = i, Symbol = string.Format("atr{2}{0}test{1}", separator, i, i % 2), Nazwa = string.Format("test{0}", i) });
            }
            cp = new List<ProduktCecha>();
            for (int i = 1; i <= liczacech * liczacech; i++)
            {
                for (int j = 1; j <= liczacech; j++)
                {
                    cp.Add(new ProduktCecha { ProduktId = i, CechaId = j });
                }

            }
            return cechy;
        }
            [Fact(DisplayName = "Wyszukiwanie cech dla produktu")]
        public void PobierzWszystkieCechyProduktuTest()
        {
            int oczekiwano = 50;
            int liczacech = 100;
            string separator = ":";
            IConfigBLL config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.SeparatorAtrybutowWCechach).Returns(separator);
            int produkt = 1;
                List<ProduktCecha> cp;
                List<Cecha> cechy = DaneTestPobierania(out cp, separator, liczacech);
                ProduktyWyszukiwanie pb = new ProduktyWyszukiwanie();
               // pb.Konfiguracja = config;
            var wynik = pb.PobierzWszystkieCechyProduktu(produkt, cechy, cp, "atr0");
            Assert.True(wynik.Count == oczekiwano, string.Format("wynik {0} oczekiwano {1}",wynik.Count,oczekiwano));

        }
            [Fact(DisplayName = "Wyszukiwanie cech dla produktu wydajnosc")]
            public void PobierzWszystkieCechyProduktuWydajnoscTest()
            {
                int liczacech = 100;
                string separator = ":";
                IConfigBLL config = A.Fake<IConfigBLL>();
                A.CallTo(() => config.SeparatorAtrybutowWCechach).Returns(separator);
                int produkt = 1;
                List<ProduktCecha> cp;
                List<Cecha> cechy = DaneTestPobierania(out cp, separator, liczacech);
            
                ProduktyWyszukiwanie pb =new ProduktyWyszukiwanie();
              //  pb.Konfiguracja = config;
                Stopwatch stoper = Stopwatch.StartNew();
                var wynik = pb.PobierzWszystkieCechyProduktu(produkt, cechy, cp, "atr0");
                stoper.Stop();
                double oczekiwany = 0.2;
                Assert.True(stoper.Elapsed.TotalSeconds < oczekiwany, string.Format("za długi czas wyszukiwania cech do produkt: {0} sekund zamiast poniżej {1} sekundy, liczba lacznikow {2}",
                    stoper.Elapsed.TotalSeconds, oczekiwany,cp.Count));
            }

            [Fact()]
            public void CechyRodzinoweTest()
            {
                //int idProduktu = 1;
                int jezyk = 1;
                string rodzina = "aaa";

                int[] idAtrybutow = { 1, 2 };

                var config = A.Fake<IConfigBLL>();
                A.CallTo(() => config.AtrybutyRodzin).Returns(idAtrybutow);

                HashSet<int> listaIDCechProduktu = new HashSet<int>();
                for (int i = 0; i < 2; i++)
                {
                    listaIDCechProduktu.Add(i + 1);
                }
                var cechyprodukty = A.Fake<ICechyProduktyDostep>();
               // A.CallTo(() => cechyprodukty.PobierzIdCechProduktu(idProduktu)).Returns(listaIDCechProduktu.ToArray());

                ProduktBazowy pb1 = new ProduktBazowy(jezyk);
                pb1.Rodzina = rodzina;
                ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
                A.CallTo(() => calosc.Konfiguracja).Returns(config);
                A.CallTo(() => calosc.CechyProduktyDostep).Returns(cechyprodukty);
               // ProduktyBazowe produktyBazowe = A.Fake<ProduktyBazowe>(x => x.WithArgumentsForConstructor(new[] { calosc }));
        
               // A.CallTo(() => produktyBazowe.Pobierz(idProduktu, jezyk)).Returns(pb1);
                HashSet<int> listaCech = new HashSet<int>();
                
                CechyBll cecha = new CechyBll();
                cecha.Id = 1;
                cecha.AtrybutId = 1;
                cecha.Kolejnosc = 2;
                CechyBll cecha2 = new CechyBll();
                cecha2.Id = 2;
                cecha2.AtrybutId = 2;
                cecha2.Kolejnosc = 3;
                CechyBll cecha3 = new CechyBll();
                cecha3.Id = 2;
                cecha3.Nazwa = "Jakas cecha";
                cecha3.AtrybutId = 2;
                cecha3.Kolejnosc = 1;

                Dictionary<int, CechyBll> slownikCech = new Dictionary<int, CechyBll>();
                slownikCech.Add(1,cecha);
                slownikCech.Add(2, cecha2);
                slownikCech.Add(3, cecha3);

                var cechyAtrybuty = A.Fake<ICechyAtrybuty>();
              //  A.CallTo(() => cechyAtrybuty.PobierzCechyID(listaIDCechProduktu, jezyk)).Returns(slownikCech.Values.ToArray());


            AtrybutBll atryb = A.Fake<AtrybutBll>();
                atryb.Id = 1;
                atryb.Nazwa = "jakis atrybut";
                atryb.Kolejnosc = 2;
            AtrybutBll atryb2 = A.Fake<AtrybutBll>();
                atryb2.Id = 2;
                atryb2.Nazwa = "jakis atrybut2";
                atryb2.Kolejnosc = 1;

                List<AtrybutBll> at = new List<AtrybutBll>();
                at.Add(atryb);
                at.Add(atryb2);
               // A.CallTo(() => cechyAtrybuty.Pobierz(A<HashSet<int>>.Ignored, jezyk)).Returns(at);
                A.CallTo(() => calosc.CechyAtrybuty).Returns(cechyAtrybuty);
                
                Assert.True(listaCech.Count==3,string.Format("otrzymano: {0}",listaCech.Count));
                Assert.True(listaCech.First() == 2, string.Format("otrzymano: {0}", listaCech.First()));
            }
    }
}
