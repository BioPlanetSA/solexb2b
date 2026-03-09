using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using Xunit;
using Klient = SolEx.Hurt.Core.Klient;

namespace SolEx.Hurt.CoreTests.BLL
{
    public class SposobyPokazywaniaStanowBllTests
    {
       

        [Fact(DisplayName = "Test sprawdzania czy prawidłowo wyciągamy sposoby pokazywania stanówdla różnych klientów")]
        public void PobierzSposobyKlientaTest()
        {
            int jezyk = 1;
            //klient 1 - zwykły klient
            IKlient klient1 = A.Fake<IKlient>();
            klient1.Id = 1;
            klient1.Role = new HashSet<RoleType>() {RoleType.Klient};

            //klient 2 - pracownik
            IKlient klient2 = A.Fake<IKlient>();
            klient2.Id = 2;
            klient2.Role = new HashSet<RoleType>() { RoleType.Klient, RoleType.Pracownik };

            //klient3 Administrator
            IKlient klient3 = A.Fake<IKlient>();
            klient3.Id = 3;
            klient3.Role = new HashSet<RoleType>() { RoleType.Klient, RoleType.Pracownik, RoleType.Administrator };

            //klient 4 przedstawiciel

            IKlient klient4 = A.Fake<IKlient>();
            klient4.Id = 4;
            klient4.Role = new HashSet<RoleType>() { RoleType.Klient, RoleType.Pracownik, RoleType.Przedstawiciel };
            
            //klient 5 niezalogowany
            IKlient klient5 = A.Fake<IKlient>();
            klient5.Id = 0;
            A.CallTo(() => klient5.Dostep).Returns(AccesLevel.Niezalogowani);


            SposobPokazywaniaStanow s1 = new SposobPokazywaniaStanow();
            s1.Id = 1;
            s1.Nazwa = "Nazwa 1";
            s1.Dostep=AccesLevel.Zalogowani;
            s1.PozycjaLista = PozycjaLista.Kolumna;
            s1.DozwolonaRolaKlienta = new List<RoleType>() {RoleType.Administrator, RoleType.Klient, RoleType.Pracownik};

            SposobPokazywaniaStanow s2 = new SposobPokazywaniaStanow();
            s2.Id = 2;
            s2.Nazwa = "Nazwa 2";
            s2.Dostep = AccesLevel.Niezalogowani;
            s2.PozycjaLista = PozycjaLista.Kolumna;

            SposobPokazywaniaStanow s3 = new SposobPokazywaniaStanow();
            s3.Id = 3;
            s3.Nazwa = "Nazwa 3";
            s3.Dostep = AccesLevel.Zalogowani;
            s3.PozycjaLista = PozycjaLista.Brak;
            s3.DozwolonaRolaKlienta = new List<RoleType>() { RoleType.Administrator, RoleType.Klient, RoleType.Pracownik };

            SposobPokazywaniaStanow s4 = new SposobPokazywaniaStanow();
            s4.Id = 4;
            s4.Nazwa = "Nazwa 4";
            s4.Dostep = AccesLevel.Zalogowani;
            s4.PozycjaLista = PozycjaLista.Kolumna;
            s4.DozwolonaRolaKlienta = new List<RoleType>() { RoleType.Pracownik };


            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
           
            Dictionary<long,SposobPokazywaniaStanow>slownikStanow = new Dictionary<long, SposobPokazywaniaStanow>();
            slownikStanow.Add(s1.Id,s1);
            slownikStanow.Add(s2.Id, s2);
            slownikStanow.Add(s3.Id, s3);
            slownikStanow.Add(s4.Id, s4);

            A.CallTo(() =>solexBllCalosc.WidocznosciTypowBll.KlientMaDostepDoObiektu(A<IKlient>.Ignored,A<SposobPokazywaniaStanow>.Ignored)).Returns(true);
            
            SposobyPokazywaniaStanowBLL stany = new SposobyPokazywaniaStanowBLL(solexBllCalosc);
            var wynik1 = stany.PobierzSposobyKlienta(klient1, klient4, jezyk, slownikStanow);
            //Z przedstawicielem pracownikiem
            Assert.True(wynik1.Count==3);
            Assert.True(wynik1.First().Id == 1);
            Assert.True(wynik1.Last().Id == 4);

            //bez przedstawiciela
            var wynik2 = stany.PobierzSposobyKlienta(klient1, null, jezyk, slownikStanow);
            Assert.True(wynik2.First().Id == 1);
            Assert.True(wynik2.Last().Id == 3);
            Assert.True(wynik2.Count == 2);

            //dla niezalogowanych nie pokazywać
            var wynik3 = stany.PobierzSposobyKlienta(klient3, null, jezyk, slownikStanow);
            Assert.True(wynik3.First().Id == 1);
            Assert.True(wynik3.Last().Id == 4);
            Assert.True(wynik3.Count == 3);

            var wynik4 = stany.PobierzSposobyKlienta(klient2, null, jezyk, slownikStanow);
            Assert.True(wynik4.First().Id == 1);
            Assert.True(wynik4.Last().Id == 4);
            Assert.True(wynik4.Count == 3);

            var wynik5 = stany.PobierzSposobyKlienta(klient5, null, jezyk, slownikStanow);
            Assert.True(wynik5.First().Id == 2);
            Assert.True(wynik5.Count == 1);
            
        }


        [Fact(DisplayName = "Test sprawdzamy czy klient będzie widział stan na podstawie ról które posiada")]
        public void CzyPokazacStanNaPodstawieRoliKlientaTsst()
        {
            IKlient klient = new Klient();
            klient.Role = new HashSet<RoleType>() {RoleType.Klient};

            IKlient przedstawiciel = new Klient();
            przedstawiciel.Role=new HashSet<RoleType>(){RoleType.Klient,RoleType.Przedstawiciel};

            ISolexBllCalosc calosc =new SolexBllCalosc();
            SposobyPokazywaniaStanowBLL sposoby = new SposobyPokazywaniaStanowBLL(calosc);

            Assert.True(sposoby.CzyPokazacStanNaPodstawieRoliKlienta(klient,null,przedstawiciel));
            //Nie bedzie widziane gdyż klient przejmie role przedstawiciela i bedzie mial roe klienta oraz przedstawiciela
            Assert.False(sposoby.CzyPokazacStanNaPodstawieRoliKlienta(klient, new List<RoleType>() {RoleType.Klient}, przedstawiciel));
            Assert.True(sposoby.CzyPokazacStanNaPodstawieRoliKlienta(klient, new List<RoleType>() { RoleType.Przedstawiciel }, przedstawiciel));
            Assert.True(sposoby.CzyPokazacStanNaPodstawieRoliKlienta(klient, new List<RoleType>() { RoleType.Klient }, null));
            Assert.False(sposoby.CzyPokazacStanNaPodstawieRoliKlienta(klient, new List<RoleType>() { RoleType.Przedstawiciel }, null));
        }
        [Fact(DisplayName = "Test sprawdzamy czy poprawnie sprawdzany jest warunek dostawy cyklicznej")]
        public void SpelniaWarunekCyklicznejDostawyTest()
        {
            IProduktKlienta pk = A.Fake<IProduktKlienta>();
            A.CallTo(() => pk.NajblizszaDostawa).Returns(DateTime.Now);

            IProduktKlienta pkBezDostawy = A.Fake<IProduktKlienta>();
            A.CallTo(() => pkBezDostawy.NajblizszaDostawa).Returns(null);

            SposobPokazywaniaStanowRegula sp1 = new SposobPokazywaniaStanowRegula();
            sp1.CyklicznaDostawa=CyklkicznaDostawa.NiePosiada;

            SposobPokazywaniaStanowRegula sp2 = new SposobPokazywaniaStanowRegula();
            sp2.CyklicznaDostawa = CyklkicznaDostawa.NieWplywa;

            SposobPokazywaniaStanowRegula sp3 = new SposobPokazywaniaStanowRegula();
            sp3.CyklicznaDostawa = CyklkicznaDostawa.Posiada;


            ISolexBllCalosc calosc = new SolexBllCalosc();
            SposobyPokazywaniaStanowBLL stany = new SposobyPokazywaniaStanowBLL(calosc);

            Assert.False(stany.SpelniaWarunekCyklicznejDostawy(pk,sp1));
            Assert.True(stany.SpelniaWarunekCyklicznejDostawy(pk, sp2));
            Assert.True(stany.SpelniaWarunekCyklicznejDostawy(pk, sp3));


            Assert.True(stany.SpelniaWarunekCyklicznejDostawy(pkBezDostawy, sp1));
            Assert.True(stany.SpelniaWarunekCyklicznejDostawy(pkBezDostawy, sp2));
            Assert.False(stany.SpelniaWarunekCyklicznejDostawy(pkBezDostawy, sp3));
        }

        [Fact(DisplayName = "Test sprawdzamy czy poprawnie sprawdzany jest warunek typu stanu")]
        public void SpelniaWarunekTypuStanuTest()
        {
            IProduktKlienta pk = A.Fake<IProduktKlienta>();
            A.CallTo(() => pk.PobierzTypStany).Returns(TypStanu.cykliczna_dostawa);

          
            SposobPokazywaniaStanowRegula sp1 = new SposobPokazywaniaStanowRegula();
            sp1.TypStanu=TypStanu.cykliczna_dostawa;
            sp1.WarunekStany = WarunekStanu.Rowny;

            SposobPokazywaniaStanowRegula sp2 = new SposobPokazywaniaStanowRegula();
            sp2.TypStanu=TypStanu.dropshiping;
            sp2.WarunekStany = WarunekStanu.NieWplywa;

            SposobPokazywaniaStanowRegula sp3 = new SposobPokazywaniaStanowRegula();
            sp3.TypStanu=TypStanu.cykliczna_dostawa;
            sp3.WarunekStany = WarunekStanu.Rozny;

            SposobPokazywaniaStanowRegula sp4 = new SposobPokazywaniaStanowRegula();
            sp4.TypStanu = TypStanu.na_stanie;
            sp4.WarunekStany = WarunekStanu.Rozny;


            SposobPokazywaniaStanowRegula sp5 = new SposobPokazywaniaStanowRegula();
            sp5.TypStanu = TypStanu.dropshiping;
            sp5.WarunekStany = WarunekStanu.Rowny;
            

            ISolexBllCalosc calosc = new SolexBllCalosc();
            SposobyPokazywaniaStanowBLL stany = new SposobyPokazywaniaStanowBLL(calosc);

            Assert.True(stany.SpelniaWarunekTypuStanu(pk, sp1));
            Assert.True(stany.SpelniaWarunekTypuStanu(pk, sp2));
            Assert.False(stany.SpelniaWarunekTypuStanu(pk, sp3));


            Assert.True(stany.SpelniaWarunekTypuStanu(pk, sp4));
            Assert.False(stany.SpelniaWarunekTypuStanu(pk, sp5));


        }

        [Fact(DisplayName = "Test sprawdzamy czy poprawnie sprawdzany jest warunek ilości produktu")]
        public void SpelniaWarunekIlosciuTest()
        {
            IProduktKlienta pk = A.Fake<IProduktKlienta>();
            A.CallTo(() => pk.StanMin).Returns(1.5m);
          
            SposobPokazywaniaStanowRegula sp1 = new SposobPokazywaniaStanowRegula();
            sp1.WarunekIlosci = Warunek.Mniejszy;
            sp1.RazyStanMinimalny = true;
            sp1.IloscProduktu = 10;
            SposobPokazywaniaStanowRegula sp2 = new SposobPokazywaniaStanowRegula();
            sp2.WarunekIlosci = Warunek.MniejszyRowny;
            sp2.IloscProduktu = 6;
            sp2.RazyStanMinimalny = false;
            SposobPokazywaniaStanowRegula sp3 = new SposobPokazywaniaStanowRegula();
            sp3.WarunekIlosci = Warunek.Wiesze;
            sp3.IloscProduktu = 1;
            sp3.RazyStanMinimalny = true;
            SposobPokazywaniaStanowRegula sp4 = new SposobPokazywaniaStanowRegula();
            sp4.WarunekIlosci = Warunek.Rowna;
            sp4.RazyStanMinimalny = false;
            sp4.IloscProduktu = 8;
            SposobPokazywaniaStanowRegula sp5 = new SposobPokazywaniaStanowRegula();
            sp5.WarunekIlosci = Warunek.WiekszaRowna;
            sp5.RazyStanMinimalny = true;
            sp5.IloscProduktu = 14;


            ISolexBllCalosc calosc = new SolexBllCalosc();
            SposobyPokazywaniaStanowBLL stany = new SposobyPokazywaniaStanowBLL(calosc);

            Assert.False(stany.SpelniaWarunekIlosci(pk, sp1,15));
            Assert.True(stany.SpelniaWarunekIlosci(pk, sp1, 14));
            Assert.True(stany.SpelniaWarunekIlosci(pk, sp2, 6));
            Assert.False(stany.SpelniaWarunekIlosci(pk, sp2, 7));
            Assert.True(stany.SpelniaWarunekIlosci(pk, sp3, 2));
            Assert.False(stany.SpelniaWarunekIlosci(pk, sp3, 0));
            Assert.True(stany.SpelniaWarunekIlosci(pk, sp4, 8));
            Assert.False(stany.SpelniaWarunekIlosci(pk, sp5, 20));
            Assert.True(stany.SpelniaWarunekIlosci(pk, sp5, 21));
        }

        [Fact(DisplayName = "Test sprawdzamy czy poprawnie sprawdzany jest warunek stanu minimalnego")]
        public void SpelniaWarunekStanuMinimalnego()
        {
            IProduktKlienta pk = A.Fake<IProduktKlienta>();
            A.CallTo(() => pk.StanMin).Returns(1.5m);

            SposobPokazywaniaStanowRegula sp1 = new SposobPokazywaniaStanowRegula();
            sp1.WarunekIlosciMinimalnej = Warunek.Mniejszy;
            sp1.IloscMinimalna = 1;
            SposobPokazywaniaStanowRegula sp2 = new SposobPokazywaniaStanowRegula();
            sp2.WarunekIlosciMinimalnej = Warunek.MniejszyRowny;
            sp2.IloscMinimalna = 1;
            SposobPokazywaniaStanowRegula sp3 = new SposobPokazywaniaStanowRegula();
            sp3.WarunekIlosciMinimalnej = Warunek.Wiesze;
            sp3.IloscMinimalna = 2;
            SposobPokazywaniaStanowRegula sp4 = new SposobPokazywaniaStanowRegula();
            sp4.WarunekIlosciMinimalnej = Warunek.Rowna;
            sp4.IloscMinimalna = 2;
            SposobPokazywaniaStanowRegula sp5 = new SposobPokazywaniaStanowRegula();
            sp5.WarunekIlosciMinimalnej = Warunek.WiekszaRowna;
            sp5.IloscMinimalna = 2;


            SposobPokazywaniaStanowRegula sp6 = new SposobPokazywaniaStanowRegula();
            sp6.WarunekIlosciMinimalnej = Warunek.Mniejszy;
            sp6.IloscMinimalna = 2;
            SposobPokazywaniaStanowRegula sp7 = new SposobPokazywaniaStanowRegula();
            sp7.WarunekIlosciMinimalnej = Warunek.MniejszyRowny;
            sp7.IloscMinimalna = 1.5m;
            SposobPokazywaniaStanowRegula sp8 = new SposobPokazywaniaStanowRegula();
            sp8.WarunekIlosciMinimalnej = Warunek.Wiesze;
            sp8.IloscMinimalna = 1;
            SposobPokazywaniaStanowRegula sp9 = new SposobPokazywaniaStanowRegula();
            sp9.WarunekIlosciMinimalnej = Warunek.Rowna;
            sp9.IloscMinimalna = 1.5m;
            SposobPokazywaniaStanowRegula sp10 = new SposobPokazywaniaStanowRegula();
            sp10.WarunekIlosciMinimalnej = Warunek.WiekszaRowna;
            sp10.IloscMinimalna = 1;

            ISolexBllCalosc calosc = new SolexBllCalosc();
            SposobyPokazywaniaStanowBLL stany = new SposobyPokazywaniaStanowBLL(calosc);

            Assert.False(stany.SpelniaWarunekStanuMinimalnego(pk,sp1));
            Assert.False(stany.SpelniaWarunekStanuMinimalnego(pk, sp2));
            Assert.False(stany.SpelniaWarunekStanuMinimalnego(pk, sp3));
            Assert.False(stany.SpelniaWarunekStanuMinimalnego(pk, sp4));
            Assert.False(stany.SpelniaWarunekStanuMinimalnego(pk, sp5));

            Assert.True(stany.SpelniaWarunekStanuMinimalnego(pk, sp6));
            Assert.True(stany.SpelniaWarunekStanuMinimalnego(pk, sp7));
            Assert.True(stany.SpelniaWarunekStanuMinimalnego(pk, sp8));
            Assert.True(stany.SpelniaWarunekStanuMinimalnego(pk, sp9));
            Assert.True(stany.SpelniaWarunekStanuMinimalnego(pk, sp10));
        }


        [Fact(DisplayName = "Test sprawdzamy poprawne pobieranie magazynu dla sposobu pokazywania stanow")]
        public void PobierzMagazynyDlaSposobuPokazyywaniaStanowTest()
        {
            Magazyn m1 = new Magazyn() {Id=1, MagazynRealizujacy = false, Nazwa = "Magazn 1"};
            Magazyn m2 = new Magazyn() { Id = 2, MagazynRealizujacy = true, Nazwa = "Magazn 2" };

            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            IConfigBLL config = A.Fake<IConfigBLL>();
            IProduktyStanBll produktStany = A.Fake<IProduktyStanBll>();
            A.CallTo(() => calosc.Konfiguracja).Returns(config);
            A.CallTo(() => calosc.ProduktyStanBll).Returns(produktStany);

            A.CallTo(() => config.SlownikMagazynowPoId).Returns(new Dictionary<int, Magazyn>() {{m1.Id, m1}, {m2.Id, m2}});

            A.CallTo(() => produktStany.MagazynDomyslny()).Returns(m2);

            //Sposob z zdefiniowanym domyslnym magazynem
            SposobPokazywaniaStanow s1 = new SposobPokazywaniaStanow();
            s1.DomyslnyMagazynId = 1;

            //bez domyslnego magazynu
            SposobPokazywaniaStanow s2 = new SposobPokazywaniaStanow();
            s2.Nazwa = "s2";

            IKlient klient = new Klient(null);
            klient.Nazwa = "k1";
            klient.Id = 1;
            klient.DostepneMagazynyDlaKlienta=new List<Magazyn>() {m1,m2};
            klient.DostepneMagazyny=new HashSet<string>() {"m1","m2"};


            SposobyPokazywaniaStanowBLL sposoby = new SposobyPokazywaniaStanowBLL(calosc);
            //Pobieramy magazyny dla sposoby z domyslnym magazynem - ten magazyn powinien byćzwrocony
            var wynik = sposoby.PobierzMagazynyDlaSposobuPokazyywaniaStanow(s1, klient);
            Assert.True(wynik.Count==1);
            Assert.True(wynik.First().Id == 1);

            //Pobieramy magazyn ze spsobu ktory nie ma domyslnego magazynu ale magazyny powinny być pobrane z klienta
            wynik = sposoby.PobierzMagazynyDlaSposobuPokazyywaniaStanow(s2, klient);
            Assert.True(wynik.Count==2);
            Assert.True(wynik.First().Id == 1);
            Assert.True(wynik.Last().Id == 2);

            //Pobieramy magazyny dla sposoby bez zdefiniowanych magazynow, klient równieżnie ma zdefiniowwanych dostepnych klientow, zwrócony powinienn być magazyn podstawowy
            klient.DostepneMagazynyDlaKlienta = null;
            klient.DostepneMagazyny = null;
            wynik = sposoby.PobierzMagazynyDlaSposobuPokazyywaniaStanow(s2, klient);
            Assert.True(wynik.Count == 1);
            Assert.True(wynik.First().Id == 2);

        }

        [Fact(DisplayName = "Test sprawdzamy poprawne pobieranie sposobów dla klienta")]
        public void WszystkieSposobyKlientaTest()
        {
            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            IDaneDostep dostepDodanych = A.Fake<IDaneDostep>();
            A.CallTo(() => calosc.DostepDane).Returns(dostepDodanych);

            //Sposob z zdefiniowanym domyslnym magazynem
            SposobPokazywaniaStanow s1 = new SposobPokazywaniaStanow();
            s1.Id = 1;
            s1.PozycjaLista=PozycjaLista.Kolumna;
            s1.DomyslnyMagazynId = 1;

            //bez domyslnego magazynu
            SposobPokazywaniaStanow s2 = new SposobPokazywaniaStanow();
            s2.Id = 2;
            s2.PozycjaLista = PozycjaLista.Kolumna;
            s2.Nazwa = "s2";

            SposobPokazywaniaStanow s3 = new SposobPokazywaniaStanow();
            s3.Id = 3;
            s3.PozycjaLista = PozycjaLista.Brak;
            s3.Nazwa = "s3";

            IKlient przedstawiciel = new Klient(null);
            przedstawiciel.Id = 2;

            IKlient klient = new Klient(null);
            klient.Id = 1;


            A.CallTo(dostepDodanych).Where(x => x.Method.Name == "Pobierz").WithReturnType<IList<SposobPokazywaniaStanow>>().WithAnyArguments().Returns(new List<SposobPokazywaniaStanow>() {s1,s2,s3});

            SposobyPokazywaniaStanowBLL sposoby = A.Fake<SposobyPokazywaniaStanowBLL>(x=>x.WithArgumentsForConstructor(new object[] {calosc}));
            A.CallTo(() => sposoby.PobierzSposobyKlienta(A<IKlient>.Ignored, null, A<int>.Ignored, A<Dictionary<long, SposobPokazywaniaStanow>>.Ignored)).Returns(new List<SposobPokazywaniaStanow>() {s1});
            A.CallTo( () =>sposoby.PobierzSposobyKlienta(A<IKlient>.Ignored, przedstawiciel, A<int>.Ignored, A<Dictionary<long, SposobPokazywaniaStanow>>.Ignored)).Returns(new List<SposobPokazywaniaStanow>() {s1, s2, s3});


            //Pobieramy bez sposobow rozpatrywanych bez przedstawiciela
            var wynik = sposoby.WszystkieSposobyKlienta(klient,1, null, null, false);
            Assert.True(wynik.Count()==1);
            Assert.True(wynik.First().Key==PozycjaLista.Kolumna);
            Assert.True(wynik.First().Value.Count == 1);
            Assert.True(wynik.First().Value.First().Key == s1.Id);

            //Pobieramy bez sposobow rozpatrywanych z przedstawicielem 
            wynik = sposoby.WszystkieSposobyKlienta(klient, 1, przedstawiciel, null, false);
            Assert.True(wynik.Count() == 2);
            Assert.True(wynik.Keys.Contains(PozycjaLista.Brak) );
            Assert.True(wynik.Keys.Contains(PozycjaLista.Kolumna));
            Assert.True(wynik[PozycjaLista.Kolumna].Count==2);
            Assert.True(wynik[PozycjaLista.Kolumna].ContainsKey(1));
            Assert.True(wynik[PozycjaLista.Kolumna].ContainsKey(2));
            Assert.True(wynik[PozycjaLista.Brak].Count==1);
            Assert.True(wynik[PozycjaLista.Brak].ContainsKey(3));

            //Pobieramy ze sposobami rozpatrywanymi i z przedstawicielem 
            wynik = sposoby.WszystkieSposobyKlienta(klient, 1, przedstawiciel, new List<long>() {1,3}, false);
            Assert.True(wynik.Count() == 2);
            Assert.True(wynik.Keys.Contains(PozycjaLista.Brak));
            Assert.True(wynik.Keys.Contains(PozycjaLista.Kolumna));
            Assert.True(wynik[PozycjaLista.Brak].ContainsKey(3));
            Assert.True(wynik[PozycjaLista.Kolumna].ContainsKey(1));

            //Pobieramy ze sposobami rozpatrywanymi, z przedstawicielem oraz tylko te które mają pozycję na liscie
            wynik = sposoby.WszystkieSposobyKlienta(klient, 1, przedstawiciel, new List<long>() { 1, 3 }, true);
            Assert.True(wynik.Count() == 1);
            Assert.False(wynik.Keys.Contains(PozycjaLista.Brak));
            Assert.True(wynik.Keys.Contains(PozycjaLista.Kolumna));
            Assert.True(wynik[PozycjaLista.Kolumna].ContainsKey(1));
        }

        [Fact(DisplayName = "Test sprawdzamy poprawne pobieranie stanu produktu oraz reguł które są spełnione")]
        public void PobierzStanProduktuWgSposobuTest()
        {
            IKlient k = new Klient(null);
            k.Id = 1;
            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            IDaneDostep dostepDodanych = A.Fake<IDaneDostep>();
            IProduktyStanBll stan = A.Fake<IProduktyStanBll>();

            A.CallTo(() => calosc.DostepDane).Returns(dostepDodanych);
            A.CallTo(() => calosc.ProduktyStanBll).Returns(stan);
            //Dla mag o id 1 stan bedzie 2
           

            Magazyn m1 = new Magazyn() { Id = 1, MagazynRealizujacy = true, Nazwa = "Magazn 1" };


            //A.CallTo(() => stan.PobierzStanyDlaProduktu(A<HashSet<int>>.Ignored, A<long>.Ignored)).Returns(2);
            
            SposobPokazywaniaStanow s1 = A.Fake<SposobPokazywaniaStanow>();
            s1.Id = 1;
            s1.PozycjaLista = PozycjaLista.Kolumna;
            s1.DomyslnyMagazynId = 1;


            IProduktKlienta pk = A.Fake<IProduktKlienta>();
            A.CallTo(() => pk.PobierzTypStany).Returns(TypStanu.na_stanie);
            A.CallTo(() => pk.PobierzStan(A<HashSet<int>>.Ignored)).Returns(2);


            string brak = "brak";
            string dostepny = "dostepny";

            SposobPokazywaniaStanowRegula spr1 = new SposobPokazywaniaStanowRegula();
            spr1.Id = 1;
            spr1.SposobId = 1;
            spr1.IloscProduktu = 0;
            spr1.WarunekIlosci=Warunek.MniejszyRowny;
            spr1.RazyStanMinimalny = false;
            spr1.IloscMinimalna = 0;
            spr1.WarunekIlosciMinimalnej=Warunek.WiekszaRowna;
            spr1.CzyTerminDostawy = false;
            spr1.CyklicznaDostawa=CyklkicznaDostawa.NieWplywa;
            spr1.TypStanu=TypStanu.brak;
            spr1.WynikHtml = brak;

            SposobPokazywaniaStanowRegula spr2 = new SposobPokazywaniaStanowRegula();
            spr2.Id = 2;
            spr2.SposobId = 1;
            spr2.IloscProduktu = 1;
            spr2.WarunekIlosci = Warunek.WiekszaRowna;
            spr2.RazyStanMinimalny = false;
            spr2.IloscMinimalna = 0;
            spr2.WarunekIlosciMinimalnej = Warunek.WiekszaRowna;
            spr2.CzyTerminDostawy = false;
            spr2.CyklicznaDostawa = CyklkicznaDostawa.NieWplywa;
            spr2.TypStanu = TypStanu.na_stanie;
            spr2.WynikHtml =dostepny;

            A.CallTo(() => s1.Reguly).Returns(new List<SposobPokazywaniaStanowRegula>() {spr1, spr2});


            SposobyPokazywaniaStanowBLL sposoby = A.Fake<SposobyPokazywaniaStanowBLL>(x => x.WithArgumentsForConstructor(new object[] { calosc }));
            A.CallTo(() => sposoby.PobierzMagazynyDlaSposobuPokazyywaniaStanow(A<SposobPokazywaniaStanow>.Ignored, A<IKlient>.Ignored)).Returns(new HashSet<Magazyn>() { m1});
            A.CallTo(() => sposoby.SpelniaRegule(pk,2,spr2,m1)).Returns(true);
            var wynik = sposoby.PobierzStanProduktuWgSposobu(s1, pk, 1, k);
            Assert.True(wynik.Count == 1);
            Assert.True(wynik.ContainsKey(2));
            Assert.True(wynik[2].Count == 1);
            Assert.True(wynik[2].First().Stan == 2);

            //Żadna reguła nie spełnia warunków
            // A.CallTo(() => stan.PobierzStanyDlaProduktu(A<HashSet<int>>.Ignored, A<long>.Ignored)).Returns(0);
            A.CallTo(() => pk.PobierzStan(A<HashSet<int>>.Ignored)).Returns(0);

            wynik = sposoby.PobierzStanProduktuWgSposobu(s1, pk, 1, k);
            Assert.True(wynik.Count==0);
        }

    }
}
