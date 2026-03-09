using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.BLL.RegulyKoszyka;
using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.ListyPrzewozowe;
using Xunit;
namespace SolEx.Hurt.Core.BLL.Tests
{
    public class ZadaniaBLLTests
    {
        [Fact(DisplayName = "Sprawdzenie czy klient ma dostęp do wybranego zadania")]
        public void JestDostepneDlaKlientaTest()
        {
            Testuj(false, null, 0, true);
            Testuj(false, 1, 0, false);
            Testuj(true, null, 0, true);
            Testuj(true, 1, 0, true);
            Testuj(true, 1, 1, false);
            Testuj(true, null, 1, false);
            Testuj(false, 1, 1, true);
        }

        void Testuj(bool centralne, int? zadanieOddzial, int odzialKlienta,bool wynik)
        {
             IKlient klient = A.Fake<IKlient>();
            A.CallTo(() => klient.OddzialDoJakiegoNalezyKlient).Returns(odzialKlienta);
            Zadania testowe=new Zadania();
            testowe.Centralne = centralne;
            testowe.OddzialId = zadanieOddzial;
            BlokadaFinalizacjiKoszyka mod=new BlokadaFinalizacjiKoszyka();
            mod.powiazaneZadanie = testowe;
            bool result= mod.JestDostepneDlaKlienta( klient);
            Assert.True(wynik==result,string.Format("Porównywanie zadanie centralne {0} odzial {1}, klient odzlail {2}, oczekiwane {3} wynik {4}",centralne,zadanieOddzial,odzialKlienta,wynik,result));
        }

        

        [Fact(DisplayName = "Test sprawdzajacy poprawność pobrania aktywnego zadania o określonym typie zadania")]
        public void PobierzZadaniaTest()
        {
            throw new Exception("Do przeróbki");
            //Zadania z1 = new Zadania() { id = 1, ModulKolejnosc = 1, ModulNazwa = "Jakas nazwa", Aktywne = false, TypZadania = TypZadania.RegulaKoszyka};
            //Zadania z2 = new Zadania() { id = 2, ModulKolejnosc = 2, ModulNazwa = "Jakas nazwa2", Aktywne = true,TypZadania = TypZadania.RegulaKoszyka };
            //Zadania z3 = new Zadania() { id = 3, ModulKolejnosc = 1, ModulNazwa = "Jakas nazwa3", Aktywne = true, TypZadania = TypZadania.RegulaPunktowa };
            //Zadania z4 = new Zadania() { id = 4, ModulKolejnosc = 2, ModulNazwa = "Jakas nazwa4", Aktywne = false, TypZadania = TypZadania.RegulaPunktowa };
            //List<Zadania> listaZadan = new List<Zadania>(){z1,z2,z3,z4};

            //var zad = A.Fake<ZadaniaBLL>();
            //A.CallTo(() => zad.WszystkieZadania()).Returns(listaZadan);

            //List<Zadania> wynik = zad.PobierzZadania(true, TypZadania.RegulaKoszyka);
            //List<Zadania> wynik2 = zad.PobierzZadania(false, TypZadania.RegulaKoszyka);
            //List<Zadania> wynik3 = zad.PobierzZadania(true, TypZadania.RegulaPunktowa);
            //List<Zadania> wynik4 = zad.PobierzZadania(false, TypZadania.RegulaPunktowa);

            //Assert.True(wynik.Count == 1, string.Format("Oczekiwano: 1, otrzymano: {0}", wynik.Count));
            //Assert.True(wynik2.Count == 2, string.Format("Oczekiwano: 2, otrzymano: {0}", wynik2.Count));
            //Assert.True(wynik3.Count == 1, string.Format("Oczekiwano: 1, otrzymano: {0}", wynik3.Count));
            //Assert.True(wynik4.Count == 2, string.Format("Oczekiwano: 2, otrzymano: {0}", wynik4.Count));

        }

        [Fact(DisplayName = "Test sprawdzajacy poprawność działania metody sprawdzajacej czy Zadanie jest aktywe")]
        public void JestAktywneZadanieTest()
        {
            Zadania z1 = new Zadania() { id = 1, ModulFullTypeName = "SolEx.Hurt.Core.BLL.ZadaniaKoszyka.PrzekroczoneStany" };
            Zadania z2 = new Zadania() { id = 2, ModulFullTypeName = "SolEx.Hurt.Core.BLL.ZadaniaKoszyka.PrzypisanieZamowieniuKategorii" };
            Zadania z3 = new Zadania() { id = 3, ModulFullTypeName = "SolEx.Hurt.Core.BLL.ZadaniaKoszyka.Platnosc" };

            List<Zadania> listaZadan = new List<Zadania>() { z1, z2, z3 };
            ZadaniaBLL zad = A.Fake<ZadaniaBLL>();
            A.CallTo(() => zad.Wszystykie()).Returns(listaZadan);

            Assert.True(zad.JestAktywneZadanie<PrzekroczoneStany>());
            Assert.True(zad.JestAktywneZadanie<PrzypisanieZamowieniuKategorii>());
            Assert.True(zad.JestAktywneZadanie<Platnosc>());
            Assert.False(zad.JestAktywneZadanie<KosztDostawy>());
        }

        [Fact(DisplayName = "Test sprawdzajacy dzialanie metody pobierajacej zadania podrzedne")]
        public void PobierzZadaniaPodzedneTest()
        {
            Zadania z1 = new Zadania() { id = 1, ModulFullTypeName = "SolEx.Hurt.Core.BLL.ZadaniaKoszyka.PrzekroczoneStany" };
            Zadania z2 = new Zadania() { id = 2, ZadanieNadrzedne = 1};
            Zadania z3 = new Zadania() { id = 3, ZadanieNadrzedne = 1};
            Zadania z4 = new Zadania() { id = 4, ZadanieNadrzedne = 2 };
            List<Zadania> listaZadan = new List<Zadania>(){z1,z2,z3,z4};

            ZadaniaBLL zad = A.Fake<ZadaniaBLL>();
            A.CallTo(() => zad.Wszystykie()).Returns(listaZadan);
            List<Zadania> wynik = zad.PobierzZadaniaPodzedne(1).ToList();

            Assert.True(wynik.Count==2);
            Assert.True(wynik.Any(x=>x.id==2));
            Assert.True(wynik.Any(x => x.id == 3));
        }

        [Fact(DisplayName = "Test sprawdzajacy czy metoda pobierajaca zadanie o określonym id działa poprawnie")]
        public void PobierzZadanieByIDTest()
        {
            Zadania z1 = new Zadania() { id = 1, ModulKolejnosc = 1};
            Zadania z2 = new Zadania() { id = 2, ZadanieNadrzedne = 1, ModulKolejnosc = 2 };
            Zadania z3 = new Zadania() { id = 3, ZadanieNadrzedne = 1, ModulKolejnosc = 3 };
            Zadania z4 = new Zadania() { id = 4, ZadanieNadrzedne = 2, ModulKolejnosc = 3 };
            List<Zadania> listaZadan = new List<Zadania>() { z1, z2, z3, z4 };

            ZadaniaBLL zad = A.Fake<ZadaniaBLL>();
            A.CallTo(() => zad.Wszystykie()).Returns(listaZadan);
            Zadania wynik = zad.Pobierz(1);
            Zadania wynik2 = zad.Pobierz(2);
            Zadania wynik3 = zad.Pobierz(3);
            Zadania wynik4 = zad.Pobierz(4);

            Assert.True(wynik.ModulKolejnosc == z1.ModulKolejnosc);
            Assert.True(wynik2.ModulKolejnosc == z2.ModulKolejnosc);
            Assert.True(wynik3.ModulKolejnosc == z3.ModulKolejnosc);
            Assert.True(wynik4.ModulKolejnosc == z4.ModulKolejnosc);
        }

        [Fact(DisplayName = "Test sprawdzajacy poprawność działania metody zwracajacej date ostatniego uruchomienia")]
        public void TerminOstatniegoUruchomieniaTest()
        {
            DateTime dataUruchomienieKoniec = new DateTime(2015, 1, 30);
            Zadania z1 = new Zadania() { id = 1, ModulNazwa = "JakisModul", ostatnieUruchomienieKoniec = dataUruchomienieKoniec };
            Zadania z2 = new Zadania() { id = 2, ModulNazwa = "JakisModul2" };
            List<Zadania> listaZadan = new List<Zadania>() { z1, z2};

            ZadaniaBLL zad = A.Fake<ZadaniaBLL>();
            A.CallTo(() => zad.Wszystykie()).Returns(listaZadan);

            DateTime data = zad.TerminOstatniegoUruchomienia("JakisModul");
            DateTime data2 = zad.TerminOstatniegoUruchomienia("JakisModul2");
            DateTime data3 = zad.TerminOstatniegoUruchomienia("JakisModul3");

            Assert.True(data == dataUruchomienieKoniec);
            Assert.True(data2 == DateTime.MinValue);
            Assert.True(data3 == DateTime.MaxValue);

        }

        [Fact(DisplayName = "Metoda wyliczająca typy zadania")]
        public void PobierzGrupyDoJakichPasujeZadanieTest()
        {
         
          TestTyp(typeof(KrajKlienta), new[] { TypZadania.WarunekRegulyKoszyka });
          TestTyp(typeof(ProduktyZCechaPozycje), new[] { TypZadania.WarunekRegulyKoszyka,TypZadania.WarunekRegulyPunktowej });
          TestTyp(typeof(ListyPrzewozoweDPDZagraniczne), new[] { TypZadania.Synchronizacja });
          TestTyp(typeof(DodajDoUwag), new[] { TypZadania.RegulaKoszyka });
        }

        void TestTyp(Type testowany, IEnumerable<TypZadania> oczekiwane)
        {
            var ConfigBllFake = A.Fake<ConfigBLL>();
            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            A.CallTo(() => calosc.Konfiguracja).Returns(ConfigBllFake);
            ZadaniaBLL a = A.Fake<ZadaniaBLL>(x => x.WithArgumentsForConstructor(new[] { calosc }));
            var wynik = a.PobierzGrupyDoJakichPasujeZadanie(testowany);
            Assert.Equal(wynik.Count,oczekiwane.Count());
            foreach (TypZadania t in oczekiwane)
            {
                Assert.True(wynik.Contains(t), "wynik nie zawiera "+t);
            }
            foreach (TypZadania t in wynik)
            {
                Assert.True(oczekiwane.Contains(t), "oczekiwane nie zawiera " + t);
            }
        }
    }
}
