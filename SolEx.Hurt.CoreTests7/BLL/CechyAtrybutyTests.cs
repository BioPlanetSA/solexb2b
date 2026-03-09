using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using FakeItEasy;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using Xunit;

namespace SolEx.Hurt.CoreTests.BLL
{
    public class CechyAtrybutyTests: CechyAtrybuty
    {
        private string baza = ":memory:";
        private OrmLiteConnectionFactory polaczenie;

        public CechyAtrybutyTests():base(null)
        {
            base.Calosc = A.Fake<ISolexBllCalosc>();
            polaczenie = new OrmLiteConnectionFactory(baza, SqliteDialect.Provider) { DialectProvider = { UseUnicode = true }, AutoDisposeConnection = false };

        }
        
        [Fact(DisplayName = "Test sprawdzajacy działanie metody zwracajacej atrybuty dla okreslonej cechy o okreslonym symbolu")]
        public void WyciagnijAtrybutZCechyTest()
        {
            string separator = ":";
            string symbol = "kolejność:758";
            string nazwa = "758";

            string symbol2 = "cos";
            string nazwa2 = "234";
            string symbolAuto = "--auto--";

            var config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.AtrybutZCechy).Returns(true);
            A.CallTo(() => config.SeparatorAtrybutowWCechach).Returns(separator.ToArray());
            A.CallTo(() => config.CechaAuto).Returns(symbolAuto);


            AtrybutyWyszukiwanie ca = new AtrybutyWyszukiwanie();

            var wynik = ca.WyciagnijAtrybutZCechy(ref symbol, ref nazwa, separator.ToCharArray(), symbolAuto);
            var wynik2 = ca.WyciagnijAtrybutZCechy(ref symbol2, ref nazwa2, separator.ToCharArray(), symbolAuto);

            int id1 = Math.Abs("kolejność".Trim().GetHashCode());
            int id2 = Math.Abs("--auto--".Trim().GetHashCode());

            Assert.Equal(wynik.Nazwa, "kolejność");
            Assert.True(wynik.Id == id1);
            Assert.Equal(wynik2.Nazwa, symbolAuto);
            Assert.True(wynik2.Id == id2);
        }

        [Fact(DisplayName = "Test sprawdzajacy generowanie słownika przechowującego jako klucz numeru id atrybutu oraz jako wartosci zbior numerow id cech")]
        public void SlownikFiltrowTest()
        {
            ICechyBll a = A.Fake<ICechyBll>();
            CechyBll cecha1 = new CechyBll(){ Id = 1, Nazwa = "agd i rtv\\lodówki"};
            CechyBll cecha2 = new CechyBll(){ Id = 2, Nazwa = "sprzęt sportowy\\narty"};
            CechyBll cecha3 = new CechyBll(){ Id = 3, Nazwa = "agd i rtv\\kuchenki mikrofalowe"};
            CechyBll cecha4 = new CechyBll(){ Id = 4, Nazwa = "452"};
            CechyBll cecha5 = new CechyBll(){ Id = 5, Nazwa = "361"};
            CechyBll cecha6 = new CechyBll(){ Id = 6, Nazwa = "1000 mm"};
            CechyBll cecha7 = new CechyBll(){ Id = 7, Nazwa = "2000 mm"};
            CechyBll cecha8 = new CechyBll(){ Id = 8, Nazwa = "3000 mm"};
            CechyBll cecha9 = new CechyBll(){ Id = 9, Nazwa = "4000 mm"};
            List<CechyBll> listacech1 = new List<CechyBll>(){cecha1,cecha2,cecha3};
            List<CechyBll> listacech2 = new List<CechyBll>(){cecha4,cecha5};
            List<CechyBll> listacech3 = new List<CechyBll>(){cecha6,cecha7,cecha8,cecha9};


            string filtr = @"kategorie[agd i rtv\lodówki;sprzęt sportowy\narty;agd i rtv\kuchenki mikrofalowe]kolejność[452;361]długość[1000 mm;2000 mm;3000 mm; 4000 mm]";
            AtrybutBll atrybut1 = new AtrybutBll( new Atrybut(), listacech1);
            atrybut1.Id = 1;
            atrybut1.Nazwa = atrybut1.PoleDoBudowyLinkow = "kategorie";
            


            AtrybutBll atrybut2 = new AtrybutBll(new Atrybut(), listacech2);
            atrybut2.Id = 2;
            atrybut2.Nazwa = atrybut2.PoleDoBudowyLinkow = "kolejność";

            AtrybutBll atrybut3 = new AtrybutBll(new Atrybut(), listacech3);
            atrybut3.Id = 3;
            atrybut3.Nazwa = atrybut3.PoleDoBudowyLinkow = "długość";

            List<AtrybutBll> listaAtrybutow = new List<AtrybutBll>() { atrybut1, atrybut2, atrybut3 };

            this._wszystkieAtrybutyWgJezykow = listaAtrybutow;

            //test pierwszej metody

            Dictionary<string, HashSet<string>> wynik1 = this.ZbudujSlownikFiltrow(filtr);

            Assert.True(wynik1.Count == 3 );
            Assert.True(wynik1.ElementAt(0).Key == "kategorie" );
            Assert.True(wynik1.ElementAt(1).Key == "kolejność");
            Assert.True(wynik1.ElementAt(2).Key == "długość");

            Assert.True(wynik1.ElementAt(0).Value.Count  == 3 );
            Assert.True(wynik1.ElementAt(0).Value.Contains( @"agd i rtv\lodówki" ) );
            Assert.True(wynik1.ElementAt(0).Value.Contains(@"sprzęt sportowy\narty"));
            Assert.True(wynik1.ElementAt(0).Value.Contains(@"agd i rtv\kuchenki mikrofalowe"));
            
            //test 2 metody
            Dictionary<int, HashSet<long>> wynik = this.SlownikFiltrow(filtr);

            Assert.True(wynik.Count == 3);

            Assert.True(wynik.ElementAt(0).Value.Count == 3);
            Assert.True(wynik.ElementAt(1).Value.Count == 2);
            Assert.True(wynik.ElementAt(2).Value.Count == 4);

            Assert.True(wynik.ElementAt(0).Key == 1 );
            Assert.True(wynik.ElementAt(1).Key == 2);
            Assert.True(wynik.ElementAt(2).Key == 3);

            Assert.True((wynik[1].Contains(cecha1.Id)));
            Assert.True((wynik[1].Contains(cecha2.Id)));
            Assert.True((wynik[1].Contains(cecha3.Id)));
            Assert.True((wynik[2].Contains(cecha4.Id)));
            Assert.True((wynik[2].Contains(cecha5.Id)));
            Assert.True((wynik[3].Contains(cecha6.Id)));
            Assert.True((wynik[3].Contains(cecha7.Id)));
            Assert.True((wynik[3].Contains(cecha8.Id)));
            Assert.True((wynik[3].Contains(cecha9.Id)));
        }

        private const string MetkaCechy = "Testowa metka cecha";
        private const string MetkaAtrybut = "testowy opis";

        /// <summary>
        /// Głowna metoda sprawdzijąca poprawne ustawianie slowników metek przy starcie aplikacji
        /// </summary>
        /// <param name="cechaMaMetka">Czy cecha ma metka</param>
        /// <param name="atrybutMaMetke">Czy atrybut ma metkę</param>
        /// <param name="atrybutwidoczny">Czy atrybut jest widoczny</param>
        private void SprawdzMetki(bool cechaMaMetka, bool atrybutMaMetke, bool atrybutwidoczny)
        {
            var dane = A.Fake<IDaneDostep>();
            A.CallTo(() => Calosc.DostepDane).Returns(dane);

            AtrybutBll atrybut1 = new AtrybutBll();
            atrybut1.Widoczny = atrybutwidoczny;
            atrybut1.Nazwa = "Producenci";
            atrybut1.Id = 1;
            atrybut1.MetkaPozycjaKoszykGratisyPopUp = MetkaPozycjaKoszykGratisyPopUp.Brak;
            if (atrybutMaMetke)
            {
                atrybut1.UniwersalnaMetkaOpis = MetkaAtrybut;
            }
            atrybut1.UniwersalnaMetkaKatalog = "Tescik";
            CechyBll c = new CechyBll();
            if (cechaMaMetka)
            {
                c.MetkaOpis = MetkaCechy;
            }
            c.Nazwa = "Producenci ";
            c.Id = 1;
            c.AtrybutId = 1;
            c.JezykId = 1;
            c.MetkaPozycjaKafle = MetkaPozycjaKafle.PodNazwa;
            c.MetkaPozycjaRodziny = MetkaPozycjaRodziny.PodNazwa;
            c.MetkaPozycjaKoszykAutomatyczne = MetkaPozycjaKoszykAutomatyczne.PodNazwa;
            c.MetkaPozycjaKoszykGratisy = MetkaPozycjaKoszykGratisy.PodNazwa;
            c.MetkaPozycjaKoszykGratisyPopUp = MetkaPozycjaKoszykGratisyPopUp.PodNazwa;
            c.MetkaPozycjaLista = MetkaPozycjaLista.PodNazwa;
            c.MetkaPozycjaSzczegolyWarianty = MetkaPozycjaSzczegolyWarianty.PodNazwa;
            c.MetkaPozycjaSzczegoly = MetkaPozycjaSzczegoly.NaZdjeciuDol;
            c.MetkaPozycjaKoszykProdukty = MetkaPozycjaKoszykProdukty.PodNazwa;

            List<AtrybutBll> listaAtrybutow = new List<AtrybutBll>();
            List<CechyBll> listaCech = new List<CechyBll>();
            //Jezeli atrybut ma ceche i jest ona widoczna maja być zwórcona cecha i atrybut podobnie w sytuacji gdy nie ma matki cecha nie ma metki atrybut ale atrybut jest widoczny -sprawdzamy czy poprawnie doda się metkatylko dla niezalogowanych
            if ((atrybutMaMetke && atrybutwidoczny) || (!cechaMaMetka && atrybutwidoczny))
            {
                listaAtrybutow.Add(atrybut1);
                listaCech.Add(c);
            }
            else if (cechaMaMetka)
            {
                listaCech.Add(c);
            }

            A.CallTo(dane).Where(x => x.Method.Name == "Pobierz").WithReturnType<IList<AtrybutBll>>().WithAnyArguments().Returns(listaAtrybutow);
            A.CallTo(dane).Where(x => x.Method.Name == "Pobierz").WithReturnType<IList<CechyBll>>().WithAnyArguments().Returns(listaCech);
            A.CallTo(dane).Where(x => x.Method.Name == "Pobierz").WithReturnType<IList<Cecha>>().WithAnyArguments().Returns(new List<Cecha>() {c});
            this.UstawSlownikiMetek();

            if (!cechaMaMetka && (!atrybutwidoczny || !atrybutMaMetke))
            {
                Assert.True(this._slownikMetekZalogowani.Count==0);
            }
            if ((atrybutMaMetke && atrybutwidoczny) || cechaMaMetka)
            {
                string pozycja = MetkaPozycjaLista.PodNazwa.ToString();
                Assert.True(this._slownikMetekZalogowani.ContainsKey(typeof(MetkaPozycjaLista).Name) && _slownikMetekZalogowani[typeof(MetkaPozycjaLista).Name][pozycja].Contains(1));
            }
            if (!cechaMaMetka && atrybutMaMetke && atrybutwidoczny)
            {
                //Nie powinno byc MetkaPozycjaKoszykGratisyPopUp
                Assert.True(this._slownikMetekZalogowani.Count == 8);
                Assert.False(this._slownikMetekZalogowani.ContainsKey(typeof(MetkaPozycjaKoszykGratisyPopUp).Name));
            }
            if (cechaMaMetka)
            {
                Assert.True(this._slownikMetekZalogowani.Count == 9);
            }
            if (!cechaMaMetka && atrybutwidoczny && !atrybutMaMetke)
            {
                Assert.True(this._slownikMetekZalogowani.Count == 0);
                Assert.True(this._slownikMetekNiezalogowani.Count == 8);
            }
        }


        [Fact(DisplayName = "Test sprawdzajacy poprawnosc pobierania metek")]
        public void PobierzListeMetekTest()
        {
            SprawdzMetki(true,true,true);
            var cechy = PobierzListeMetek(MetkaPozycjaKoszykGratisyPopUp.PodNazwa, new HashSet<long>() {1}, 1, true);
            Assert.True(cechy.Count==1);
            Assert.True(cechy.First().MetkaOpis == MetkaCechy);
            _slownikCechJezyki = null;

            SprawdzMetki(true, false, true);
            cechy = PobierzListeMetek(MetkaPozycjaKoszykGratisyPopUp.PodNazwa, new HashSet<long>() { 1 }, 1, true);
            Assert.True(cechy.Count == 1);
            Assert.True(cechy.First().MetkaOpis == MetkaCechy);
            _slownikCechJezyki = null;

            SprawdzMetki(false, true, true);
            cechy = PobierzListeMetek(MetkaPozycjaKoszykGratisyPopUp.PodNazwa, new HashSet<long>() { 1 }, 1, true);
            Assert.True(cechy == null);
            _slownikCechJezyki = null;
            cechy = PobierzListeMetek(MetkaPozycjaLista.PodNazwa, new HashSet<long>() { 1 }, 1, true);
            Assert.True(cechy.Count == 1);
            Assert.True(cechy.First().MetkaOpis == MetkaAtrybut);

            SprawdzMetki(false, true, false);
            cechy = PobierzListeMetek(MetkaPozycjaLista.PodNazwa, new HashSet<long>() { 1 }, 1, true);
            Assert.True(cechy==null);
        }


        [Fact(DisplayName = "Test sprawdzajacy poprawnosc ustawiania metek")]
        public void UstawSlownikiMetekTest()
        {
            SprawdzMetki(true, true, true);
            SprawdzMetki(true, false, true);
            SprawdzMetki(false, true, true);
            SprawdzMetki(false, true, false);
            SprawdzMetki(false, false, true);
        }

        [Fact(DisplayName = "Test wydajnosciowy metek")]
        public void TestWydajnosciowyPobieraniaMetek()
        {
            var dane = A.Fake<IDaneDostep>();
            A.CallTo(() => Calosc.DostepDane).Returns(dane);

            var listaAtrybutow = WygenerujAtrybuty();
            var listaCech = WygenerujCechy();


            A.CallTo(dane).Where(x => x.Method.Name == "Pobierz").WithReturnType<IList<AtrybutBll>>().WithAnyArguments().Returns(listaAtrybutow);
            A.CallTo(dane).Where(x => x.Method.Name == "Pobierz").WithReturnType<IList<CechyBll>>().WithAnyArguments().Returns(listaCech);
            A.CallTo(dane).Where(x => x.Method.Name == "Pobierz").WithReturnType<IList<Cecha>>().WithAnyArguments().Returns(listaCech.Select(x=>x as Cecha).ToList());
            this.UstawSlownikiMetek();


            //Wszystkie metki sa nie ma zadnych brakow
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            for (int i = 0; i < 50000; i++)
            {
                switch (i % 3)
                {
                    case 0:
                        PobierzListeMetek(MetkaPozycjaLista.PodNazwa, new HashSet<long>() { 1 }, 1, true);
                        break;
                    case 1:
                        PobierzListeMetek(MetkaPozycjaKoszykAutomatyczne.PodNazwa, new HashSet<long>() { 1 }, 1, true);
                        break;
                    case 2:
                        PobierzListeMetek(MetkaPozycjaKafle.PodNazwa, new HashSet<long>() { 1 }, 1, true);
                        break;
                    
                }
            }
            stopWatch.Stop();
            TimeSpan czas = stopWatch.Elapsed;
            Assert.True(czas.Seconds<1);

            //Nie bedzie wogole metek
            listaCech.ForEach(x => x.MetkaPozycjaKoszykGratisyPopUp = MetkaPozycjaKoszykGratisyPopUp.Brak);
            A.CallTo(dane).Where(x => x.Method.Name == "Pobierz").WithReturnType<IList<CechyBll>>().WithAnyArguments().Returns(listaCech);
            stopWatch.Restart();

            stopWatch.Start();
            for (int i = 0; i < 100000; i++)
            {
                PobierzListeMetek(MetkaPozycjaKafle.Brak, new HashSet<long>() { 1 }, 1, true);
            }
            stopWatch.Stop();
            czas = stopWatch.Elapsed;
            Assert.True(czas.TotalSeconds < 0.1);

        }




        private List<AtrybutBll> WygenerujAtrybuty()
        {
            List<AtrybutBll>wynik = new List<AtrybutBll>();
            for (int i = 1; i < 4; i++)
            {
                AtrybutBll atrybut1 = new AtrybutBll();
                atrybut1.Widoczny = true;
                atrybut1.Nazwa = "Producenci - "+i;
                atrybut1.Id = i;
                atrybut1.MetkaPozycjaKafle = MetkaPozycjaKafle.PodNazwa;
                atrybut1.MetkaPozycjaRodziny = MetkaPozycjaRodziny.PodNazwa;
                atrybut1.MetkaPozycjaKoszykAutomatyczne = MetkaPozycjaKoszykAutomatyczne.PodNazwa;
                atrybut1.MetkaPozycjaKoszykGratisy = MetkaPozycjaKoszykGratisy.PodNazwa;
                atrybut1.MetkaPozycjaKoszykGratisyPopUp = MetkaPozycjaKoszykGratisyPopUp.Brak;
                atrybut1.MetkaPozycjaLista = MetkaPozycjaLista.PodNazwa;
                atrybut1.MetkaPozycjaSzczegolyWarianty = MetkaPozycjaSzczegolyWarianty.PodNazwa;
                atrybut1.MetkaPozycjaSzczegoly = MetkaPozycjaSzczegoly.NaZdjeciuDol;
                atrybut1.MetkaPozycjaKoszykProdukty = MetkaPozycjaKoszykProdukty.PodNazwa;
                atrybut1.UniwersalnaMetkaOpis = "testowy opis - " + i;
                atrybut1.UniwersalnaMetkaKatalog = "testowy opis - " + i;
                wynik.Add(atrybut1);
            }
            return wynik;
        }

        private List<CechyBll> WygenerujCechy(int ilosc=1000)
        {
            List<CechyBll> wynik = new List<CechyBll>();
            for (int i = 1; i < ilosc; i++)
            {
                CechyBll c = new CechyBll();
                if (i%2 == 0)
                {
                    c.MetkaOpis = "TEstowa metka cecha: " + i;
                }
                c.Nazwa = "Producenci - "+i;
                c.Id = i;
                switch (i%3)
                {
                    case 0:
                        c.AtrybutId = 1;
                        break;
                    case 1:
                        c.AtrybutId = 2;
                        break;
                    case 2:
                        c.AtrybutId = 3;
                        break;
                    default:
                        c.AtrybutId = 1;
                        break;
                }

                c.JezykId = 1;
                c.MetkaPozycjaKafle = MetkaPozycjaKafle.PodNazwa;
                c.MetkaPozycjaRodziny = MetkaPozycjaRodziny.PodNazwa;
                c.MetkaPozycjaKoszykAutomatyczne = MetkaPozycjaKoszykAutomatyczne.PodNazwa;
                c.MetkaPozycjaKoszykGratisy = MetkaPozycjaKoszykGratisy.PodNazwa;
                c.MetkaPozycjaKoszykGratisyPopUp = MetkaPozycjaKoszykGratisyPopUp.PodNazwa;
                c.MetkaPozycjaLista = MetkaPozycjaLista.PodNazwa;
                c.MetkaPozycjaSzczegolyWarianty = MetkaPozycjaSzczegolyWarianty.PodNazwa;
                c.MetkaPozycjaSzczegoly = MetkaPozycjaSzczegoly.NaZdjeciuDol;
                c.MetkaPozycjaKoszykProdukty = MetkaPozycjaKoszykProdukty.PodNazwa;
                wynik.Add(c);
            }
            return wynik;
        }

        [Fact(DisplayName = "Test sprawdzajacy poprawnosc generowania klucza cache ZbudujKluczCacheDlaFiltrowListyProduktow")]
        public void ZbudujKluczCacheDlaFiltrowListyProduktowTest()
        {
            IKlient k = A.Fake<IKlient>();
            k.OfertaIndywidualizowana = true;
            string tekst1 = ZbudujKluczCacheDlaFiltrowListyProduktow(null, k, null, null, null, null, 520);
            string tekst2 = ZbudujKluczCacheDlaFiltrowListyProduktow(0, k, "", "", new Dictionary<int, HashSet<long>>(), new Dictionary<int, HashSet<long>>(), 520);

            string tekst3 = ZbudujKluczCacheDlaFiltrowListyProduktow(0, k, "", "", new Dictionary<int, HashSet<long>>(), new Dictionary<int, HashSet<long>>(), 300);


            Assert.Equal(tekst1, tekst2);
            Assert.Null(tekst3);
        }

        [Fact(DisplayName = "Test sprawdzjący poprawne generowanie automatycznych atrybutów oraz cech")]
        public void WygenerujAtrybutyTest()
        {

            A.CallTo(() => Calosc.Konfiguracja.JezykIDDomyslny).Returns(1);
            A.CallTo(() => Calosc.Konfiguracja.JezykIDDomyslny).Returns(1);
            A.CallTo(() => Calosc.Konfiguracja.JezykiWSystemie).Returns(new Dictionary<int, Jezyk>() {{1, new Jezyk()}});

            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<Atrybut>();
            }
            DostepDoDanych dostep =  new DostepDoDanych(Calosc);
            dostep.DbFactory = polaczenie;
            A.CallTo(() => Calosc.DostepDane).Returns(dostep);

            var atrybuty = dostep.Pobierz<AtrybutBll>(null);
            Assert.True(atrybuty.Count==0);

            WygenerujAtrybuty("symbolAtrybutuMojKatalog","symbolAtrybutuAkcesorium","symbolAtrybutuOfertaSpecjalna","symbolAtrybutuUlubione", "symbolAtrybutuGradacja",1);

            //Po uruchomieniu powinny dodać się 5 atrybutów
            atrybuty = dostep.Pobierz<AtrybutBll>(null);
            Assert.True(atrybuty.Count()==5);

            using (var db = polaczenie.OpenDbConnection())
            {
                db.DropAndCreateTable<Atrybut>();
                db.Insert(atrybuty[0]);
                db.Insert(atrybuty[1]);
            }
            atrybuty = dostep.Pobierz<AtrybutBll>(null);
            Assert.True(atrybuty.Count == 2);

            WygenerujAtrybuty("symbolAtrybutuMojKatalog", "symbolAtrybutuAkcesorium", "symbolAtrybutuOfertaSpecjalna", "symbolAtrybutuUlubione", "symbolAtrybutuGradacja", 1);

            atrybuty = dostep.Pobierz<AtrybutBll>(null);
            Assert.True(atrybuty.Count == 5);
        }


    }
}
