using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.BLL;
using FakeItEasy;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces;
using Xunit;
namespace SolEx.Hurt.Core.BLL.Tests
{
    public class ZadaniaBLLTests
    {

        private readonly ISolexBllCalosc _calosc = A.Fake<ISolexBllCalosc>();
        //private OrmLiteConnectionFactory _polaczenie;
        //private string baza = ":memory:";

        public ZadaniaBLLTests()
        {
            //_polaczenie = new OrmLiteConnectionFactory(baza, SqliteDialect.Provider)
            //{
            //    DialectProvider = { UseUnicode = true },
            //    AutoDisposeConnection = false
            //};
        }
        //[Fact()]
        //public void BindingPoSelecieTest()
        //{
        //    var lista = ListaZadan();
        //    ZadaniaBLL zadania = new ZadaniaBLL(_calosc);
        //    var wynik = zadania.BindingPoSelecie(1, null, lista);

        //    Assert.True(wynik.All(x => x.Typ != null), "Wszystkie zadania muszą mieć wypełniony typ");
        //}
        [Fact(DisplayName = "Test sprawdzajacy poprawność pobierania zadań dla koszyka ")]
        public void PobierzZadaniaCalegoKoszykaKtorePasujaTest()
        {
            
        }

        private static IList<ZadanieBll> ListaZadan()
        {
            ZadanieBll z1 = new ZadanieBll() {Id = 1, ModulFullTypeName = "SolEx.Hurt.Core.BLL.ZadaniaKoszyka.KosztDostawy,SolEx.Hurt.Core"};
            ZadanieBll z2 = new ZadanieBll() {Id = 2, ModulFullTypeName = "SolEx.Hurt.Core.BLL.ZadaniaKoszyka.Platnosc,SolEx.Hurt.Core"};
            ZadanieBll z3 = new ZadanieBll() {Id = 3, ModulFullTypeName = "SolEx.Hurt.Core.BLL.RegulyKoszyka.SposobPlatnosci,SolEx.Hurt.Core"};
            ZadanieBll z4 = new ZadanieBll() {Id = 4, ModulFullTypeName = "SolEx.Hurt.Sync.App.Modules_.NoweModuly.Dokumenty.InformacjaOPrzeterminowanych,SolEx.Hurt.Sync.App.Modules_"};
            ZadanieBll z5 = new ZadanieBll() {Id = 5, ModulFullTypeName = "SolEx.Hurt.Core.BLL.RegulyKoszyka.AdresRecznieDodany,SolEx.Hurt.Core"};
            ZadanieBll z6 = new ZadanieBll() {Id = 5, ModulFullTypeName = "SolEx.Hurt.Core.BLL.ZadaniaKoszyka.BlokadaFinalizacjiKoszykaZaPunkty,SolEx.Hurt.Core" };
            IList<ZadanieBll> lista = new List<ZadanieBll>() {z1, z2, z3, z4, z5, z6};
            return lista;
        }

        [Fact()]
        public void PobierzZadaniaWyfiltrowaneTest()
        {
            var lista = ListaZadan();
            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            IDaneDostep dostep = A.Fake<IDaneDostep>();
            A.CallTo(() => calosc.DostepDane).Returns(dostep);
            ZadaniaBLL zadania = new ZadaniaBLL(calosc);
            IList<ZadanieBll> listaPoBindingu = zadania.BindingPoSelecie(1, null, lista);
            A.CallTo(dostep).Where(x => x.Method.Name == "Pobierz").WithReturnType<IList<ZadanieBll>>().WithAnyArguments().Returns(listaPoBindingu);
             

            var wynik = zadania.PobierzZadaniaWyfiltrowane<ISposobPlatnosci, ZadanieCalegoKoszyka>(1,null);
            Assert.True(wynik != null, $"Wynik jest nullem");
            Assert.True(wynik.Count() == 1, $"Wynik zawiera {wynik.Count()}, a powinien 1");

            wynik = zadania.PobierzZadaniaWyfiltrowane<ISposobDostawy, ZadanieCalegoKoszyka>(1, null);
            Assert.True(wynik != null, $"Wynik jest nullem");
            Assert.True(wynik.Count() == 1, $"Wynik zawiera {wynik.Count()}, a powinien 1");

            wynik = zadania.PobierzZadaniaWyfiltrowane<IModulStartowy, ZadanieCalegoKoszyka>(1, null);
            Assert.True(wynik != null, $"Wynik jest nullem");
            Assert.True(wynik.Count() == 1, $"Wynik zawiera {wynik.Count()}, a powinien 1");
        }

        //[Fact(DisplayName = "Sprawdzenie czy klient ma dostęp do wybranego Zadanie")]
        //public void JestDostepneDlaKlientaTest()
        //{
        //    Testuj(false, null, 0, true);
        //    Testuj(false, 1, 0, false);
        //    Testuj(true, null, 0, true);
        //    Testuj(true, 1, 0, true);
        //    Testuj(true, 1, 1, false);
        //    Testuj(true, null, 1, false);
        //    Testuj(false, 1, 1, true);
        //}

        //void Testuj(bool centralne, int? zadanieOddzial, int odzialKlienta,bool wynik)
        //{
        //     IKlient klient = A.Fake<IKlient>();
        //    A.CallTo(() => klient.OddzialDoJakiegoNalezyKlient).Returns(odzialKlienta);
        //    Zadanie testowe=new Zadanie();
        //    testowe.Centralne = centralne;
        //    testowe.OddzialId = zadanieOddzial;
        //    BlokadaFinalizacjiKoszyka mod=new BlokadaFinalizacjiKoszyka();
        //    mod.powiazaneZadanie = testowe;
        //    bool result= mod.JestDostepneDlaKlienta( klient);
        //    Assert.True(wynik==result,string.Format("Porównywanie zadanie centralne {0} odzial {1}, klient odzlail {2}, oczekiwane {3} wynik {4}",centralne,zadanieOddzial,odzialKlienta,wynik,result));
        //}



        //[Fact(DisplayName = "Test sprawdzajacy poprawność pobrania aktywnego Zadanie o określonym typie Zadanie")]
        //public void PobierzZadanieTest()
        //{
        //    throw new Exception("Do przeróbki");
        //    Zadanie z1 = new Zadanie() { Id = 1, ModulKolejnosc = 1, ModulNazwa = "Jakas nazwa", Aktywne = false, TypZadanie = TypZadanie.RegulaKoszyka };
        //    Zadanie z2 = new Zadanie() { Id = 2, ModulKolejnosc = 2, ModulNazwa = "Jakas nazwa2", Aktywne = true, TypZadanie = TypZadanie.RegulaKoszyka };
        //    Zadanie z3 = new Zadanie() { Id = 3, ModulKolejnosc = 1, ModulNazwa = "Jakas nazwa3", Aktywne = true, TypZadanie = TypZadanie.RegulaPunktowa };
        //    Zadanie z4 = new Zadanie() { Id = 4, ModulKolejnosc = 2, ModulNazwa = "Jakas nazwa4", Aktywne = false, TypZadanie = TypZadanie.RegulaPunktowa };
        //    List<Zadanie> listaZadan = new List<Zadanie>() { z1, z2, z3, z4 };

        //    var zad = A.Fake<ZadanieBLL>();
        //    A.CallTo(() => zad.WszystkieZadanie()).Returns(listaZadan);

        //    List<Zadanie> wynik = zad.PobierzZadanie(true, TypZadanie.RegulaKoszyka);
        //    List<Zadanie> wynik2 = zad.PobierzZadanie(false, TypZadanie.RegulaKoszyka);
        //    List<Zadanie> wynik3 = zad.PobierzZadanie(true, TypZadanie.RegulaPunktowa);
        //    List<Zadanie> wynik4 = zad.PobierzZadanie(false, TypZadanie.RegulaPunktowa);

        //    Assert.True(wynik.Count == 1, string.Format("Oczekiwano: 1, otrzymano: {0}", wynik.Count));
        //    Assert.True(wynik2.Count == 2, string.Format("Oczekiwano: 2, otrzymano: {0}", wynik2.Count));
        //    Assert.True(wynik3.Count == 1, string.Format("Oczekiwano: 1, otrzymano: {0}", wynik3.Count));
        //    Assert.True(wynik4.Count == 2, string.Format("Oczekiwano: 2, otrzymano: {0}", wynik4.Count));

        //}

        //[Fact(DisplayName = "Test sprawdzajacy poprawność działania metody sprawdzajacej czy Zadanie jest aktywe")]
        //public void JestAktywneZadanieTest()
        //{
        //    Zadanie z1 = new Zadanie() { Id = 1, ModulFullTypeName = "SolEx.Hurt.Core.BLL.ZadanieKoszyka.PrzekroczoneStany" };
        //    Zadanie z2 = new Zadanie() { Id = 2, ModulFullTypeName = "SolEx.Hurt.Core.BLL.ZadanieKoszyka.PrzypisanieZamowieniuKategorii" };
        //    Zadanie z3 = new Zadanie() { Id = 3, ModulFullTypeName = "SolEx.Hurt.Core.BLL.ZadanieKoszyka.Platnosc" };

        //    List<Zadanie> listaZadan = new List<Zadanie>() { z1, z2, z3 };
        //    ZadaniaBLL zad = A.Fake<ZadaniaBLL>();
        //    A.CallTo(() => zad.Wszystykie()).Returns(listaZadan);

        //    Assert.True(zad.JestAktywneZadanie<PrzekroczoneStany>());
        //    Assert.True(zad.JestAktywneZadanie<PrzypisanieZamowieniuKategorii>());
        //    Assert.True(zad.JestAktywneZadanie<Platnosc>());
        //    Assert.False(zad.JestAktywneZadanie<KosztDostawy>());
        //}

        //[Fact(DisplayName = "Test sprawdzajacy dzialanie metody pobierajacej Zadanie podrzedne")]
        //public void PobierzZadaniePodzedneTest()
        //{
        //    Zadanie z1 = new Zadanie() { Id = 1, ModulFullTypeName = "SolEx.Hurt.Core.BLL.ZadanieKoszyka.PrzekroczoneStany" };
        //    Zadanie z2 = new Zadanie() { Id = 2, ZadanieNadrzedne = 1};
        //    Zadanie z3 = new Zadanie() { Id = 3, ZadanieNadrzedne = 1};
        //    Zadanie z4 = new Zadanie() { Id = 4, ZadanieNadrzedne = 2 };
        //    List<Zadanie> listaZadan = new List<Zadanie>(){z1,z2,z3,z4};

        //    ZadanieBLL zad = A.Fake<ZadanieBLL>();
        //    A.CallTo(() => zad.Wszystykie()).Returns(listaZadan);
        //    List<Zadanie> wynik = zad.PobierzZadaniePodzedne(1).ToList();

        //    Assert.True(wynik.Count==2);
        //    Assert.True(wynik.Any(x=>x.id==2));
        //    Assert.True(wynik.Any(x => x.Id == 3));
        //}

        //[Fact(DisplayName = "Test sprawdzajacy czy metoda pobierajaca zadanie o określonym id działa poprawnie")]
        //public void PobierzZadanieByIDTest()
        //{
        //    Zadanie z1 = new Zadanie() { Id = 1, ModulKolejnosc = 1};
        //    Zadanie z2 = new Zadanie() { Id = 2, ZadanieNadrzedne = 1, ModulKolejnosc = 2 };
        //    Zadanie z3 = new Zadanie() { Id = 3, ZadanieNadrzedne = 1, ModulKolejnosc = 3 };
        //    Zadanie z4 = new Zadanie() { Id = 4, ZadanieNadrzedne = 2, ModulKolejnosc = 3 };
        //    List<Zadanie> listaZadan = new List<Zadanie>() { z1, z2, z3, z4 };

        //    ZadanieBLL zad = A.Fake<ZadanieBLL>();
        //    A.CallTo(() => zad.Wszystykie()).Returns(listaZadan);
        //    Zadanie wynik = zad.Pobierz(1);
        //    Zadanie wynik2 = zad.Pobierz(2);
        //    Zadanie wynik3 = zad.Pobierz(3);
        //    Zadanie wynik4 = zad.Pobierz(4);

        //    Assert.True(wynik.ModulKolejnosc == z1.ModulKolejnosc);
        //    Assert.True(wynik2.ModulKolejnosc == z2.ModulKolejnosc);
        //    Assert.True(wynik3.ModulKolejnosc == z3.ModulKolejnosc);
        //    Assert.True(wynik4.ModulKolejnosc == z4.ModulKolejnosc);
        //}

        //[Fact(DisplayName = "Test sprawdzajacy poprawność działania metody zwracajacej date ostatniego uruchomienia")]
        //public void TerminOstatniegoUruchomieniaTest()
        //{
        //    DateTime dataUruchomienieKoniec = new DateTime(2015, 1, 30);
        //    Zadanie z1 = new Zadanie() { Id = 1, ModulNazwa = "JakisModul", ostatnieUruchomienieKoniec = dataUruchomienieKoniec };
        //    Zadanie z2 = new Zadanie() { Id = 2, ModulNazwa = "JakisModul2" };
        //    List<Zadanie> listaZadan = new List<Zadanie>() { z1, z2};

        //    ZadanieBLL zad = A.Fake<ZadanieBLL>();
        //    A.CallTo(() => zad.Wszystykie()).Returns(listaZadan);

        //    DateTime data = zad.TerminOstatniegoUruchomienia("JakisModul");
        //    DateTime data2 = zad.TerminOstatniegoUruchomienia("JakisModul2");
        //    DateTime data3 = zad.TerminOstatniegoUruchomienia("JakisModul3");

        //    Assert.True(data == dataUruchomienieKoniec);
        //    Assert.True(data2 == DateTime.MinValue);
        //    Assert.True(data3 == DateTime.MaxValue);

        //}

        //[Fact(DisplayName = "Metoda wyliczająca typy Zadanie")]
        //public void PobierzGrupyDoJakichPasujeZadanieTest()
        //{

        //  TestTyp(typeof(KrajKlienta), new[] { TypZadania.WarunekRegulyKoszyka });
        //  TestTyp(typeof(ProduktyZCechaPozycje), new[] { TypZadania.WarunekRegulyKoszyka, TypZadania.WarunekRegulyPunktowej });
        //  TestTyp(typeof(ListyPrzewozoweDPDZagraniczne), new[] { TypZadania.Synchronizacja });
        //  TestTyp(typeof(DodajDoUwag), new[] { TypZadania.RegulaKoszyka });
        //}

        //void TestTyp(Type testowany, IEnumerable<TypZadania> oczekiwane)
        //{
        //    var ConfigBllFake = A.Fake<ConfigBLL>();
        //    ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
        //    A.CallTo(() => calosc.Konfiguracja).Returns(ConfigBllFake);
        //    ZadaniaBLL a = A.Fake<ZadaniaBLL>(x => x.WithArgumentsForConstructor(new[] { calosc }));
        //    var wynik = a.PobierzGrupyDoJakichPasujeZadanie(testowany);
        //    Assert.Equal(wynik.Count,oczekiwane.Count());
        //    foreach (TypZadania t in oczekiwane)
        //    {
        //        Assert.True(wynik.Contains(t), "wynik nie zawiera "+t);
        //    }
        //    foreach (TypZadania t in wynik)
        //    {
        //        Assert.True(oczekiwane.Contains(t), "oczekiwane nie zawiera " + t);
        //    }
        //}
    }
}
