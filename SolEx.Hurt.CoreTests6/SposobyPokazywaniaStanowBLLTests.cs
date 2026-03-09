//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using FakeItEasy;
//using Fasterflect;
//using ServiceStack.Validation;
//using SolEx.Hurt.Core;
//using SolEx.Hurt.Core.BLL;
//using SolEx.Hurt.Core.BLL.Interfejsy;
//using SolEx.Hurt.Core.ModelBLL.Interfejsy;
//using SolEx.Hurt.Model;
//using SolEx.Hurt.Model.Enums;
//using Xunit;
//namespace SolEx.Hurt.Core.Tests
//{
//    public class SposobyPokazywaniaStanowBLLTests
//    {
//        [Fact(DisplayName = "Test pokazywania stanów z magazynu o symbolu zawartym w wybranej kategorii klienta")]
//        public void PokazStanProduktuWgSposobuTest()
//        {
//            //test pokazywania stanów dla magazynu o symbolu SZYBY z pominięciem wyrzucania wyjątków jeśli dla pierwszego przypadku nie znajdzie magazynu
//            PokazStanProduktuWgSposobu("SZYBY", false);
//            //test pokazywania stanów dla magazynu o symbolu MILICZ z pominięciem wyrzucania wyjątków jeśli dla pierwszego przypadku nie znajdzie magazynu
//            PokazStanProduktuWgSposobu("MILICZ", false);
//            //test pokazywania stanów dla magazynu o symbolu LIPNY (którego nie ma) z wyrzuceniem wyjątku jeśli dla pierwszego przypadku nie znajdzie magazynu
//            PokazStanProduktuWgSposobu("LIPNY", true);
//        }
//        public void PokazStanProduktuWgSposobu(string magazynZKategorii, bool mozesiewywalic)
//        {
//            int klientID = 69;

//            var sesja = A.Fake<SesjaHelper>();
//            A.CallTo(() => sesja.KlientID).Returns(klientID);

//            var kliencidostep = A.Fake<KlienciDostep>();
//            A.CallTo(() => kliencidostep.PobierzWszystkieKategorieKlienta(klientID, "B2B_MAGAZYN")).Returns(new List<kategorie_klientow>(){  new kategorie_klientow() { grupa = "B2B_MAGAZYN", nazwa = magazynZKategorii }});

//            var stany = A.Fake<ProduktyStanBll>();
//            var magazyny = WygenerujMagazyny();
//            A.CallTo(() => stany.WszystkieSlownik()).Returns(magazyny);

//            var sposoby = A.Fake<SposobyPokazywaniaStanowBLL>();
//            sposoby.sesjaHelper = sesja;
//            sposoby.klienciDostep = kliencidostep;
//            sposoby.produktyStanBll = stany;
//            var slownikSposobow = WygenerujListeSposobow();
//            A.CallTo(() => sposoby.WszystkieSposoby(1)).Returns(slownikSposobow);

//            string kluczcache = "stan_prod0_sposob_10__mag2_lang3";
//            A.CallTo(() => sposoby.GetCacheName(1, 10, 2, 1)).Returns(kluczcache);
//            A.CallTo(() => sposoby.GetCacheName(2, 10, 2, 1)).Returns(kluczcache);
//            A.CallTo(() => sposoby.GetCacheName(3, 10, 2, 1)).Returns(kluczcache);
//            A.CallTo(() => sposoby.GetCacheName(1, 10, 3, 1)).Returns(kluczcache);
//            A.CallTo(() => sposoby.GetCacheName(2, 10, 3, 1)).Returns(kluczcache);
//            A.CallTo(() => sposoby.GetCacheName(3, 10, 3, 1)).Returns(kluczcache);
//            A.CallTo(() => sposoby.GetCacheName(1, 10, 5, 1)).Returns(kluczcache);
//            A.CallTo(() => sposoby.GetCacheName(2, 10, 5, 1)).Returns(kluczcache);
//            A.CallTo(() => sposoby.GetCacheName(3, 10, 5, 1)).Returns(kluczcache);
//            A.CallTo(() => sposoby.GetCacheName(1, 10, 4, 1)).Returns(kluczcache);
//            A.CallTo(() => sposoby.GetCacheName(2, 10, 4, 1)).Returns(kluczcache);
//            A.CallTo(() => sposoby.GetCacheName(3, 10, 4, 1)).Returns(kluczcache);

//            string wynik = "STAN DO POKAZANIA";;

//            var cache = A.Fake<CacheBll>();
//            A.CallTo(() => cache.PobierzObiekt<string>(kluczcache)).Returns(wynik);

//            sposoby.cacheBll = cache;

//            //brakuje testu - jesli jest magazyn podany, ale jest kategoria mag.

//            try
//            {
//                string wynikSposobu1 = sposoby.PokazStanProduktuWgSposobu(2, 10, 0, 1);

//                Assert.Equal(wynik, wynikSposobu1);
//            }
//            catch
//            {
//                Assert.True(mozesiewywalic, "Metoda nie powinna wyrzucić wyjątku");
//            }

//            try
//            {
//                string wynikSposobu2 = sposoby.PokazStanProduktuWgSposobu(1, 10, 0, 1);
//                Assert.True(false, "Nie wyrzuciło wyjątku z brakiem magazynu o wybranym symbolu");
//            }
//            catch
//            {
                
//            }

//            try
//            {
//                string wynikSposobu2 = sposoby.PokazStanProduktuWgSposobu(3, 10, 0, 1);
//                Assert.True(false, "Nie wyrzuciło wyjątku z brakiem magazynu o wybranym symbolu");
//            }
//            catch
//            {
                
//            }
//        }

//        [Fact(DisplayName = "Test pobierania ID magazynu dla wybranego sposobu pokazywania stanów")]
//        public void PobierzIDMagazynuTest()
//        {

//            var sposoby = WygenerujListeSposobow();
//            var kliencidostep = A.Fake<KlienciDostep>();
//            int idklienta = 10;
            
//            A.CallTo(() => kliencidostep.PobierzWszystkieKategorieKlienta(idklienta, "B2B_MAGAZYN")).Returns(new List<kategorie_klientow>() { new kategorie_klientow() { grupa = "B2B_MAGAZYN", nazwa = "SZYBY" }});
//            PobierzIDMagazynu(idklienta, 0, 2, sposoby[2], false, kliencidostep);

//            A.CallTo(() => kliencidostep.PobierzWszystkieKategorieKlienta(idklienta, A<string>.Ignored)).Returns(null);
//            PobierzIDMagazynu(idklienta, 0, null, sposoby[1], false, kliencidostep);

//            PobierzIDMagazynu(idklienta, 0, null, sposoby[3], true, kliencidostep);

//            PobierzIDMagazynu(idklienta, 1, 1, sposoby[3], true, kliencidostep);
//        }

//        public void PobierzIDMagazynu(int idklienta, int magazynid, int? oczekiwaneIDmagazynu, sposoby_pokazywania_stanow sposob, bool czyMozeWyrzucicWyjatek, KlienciDostep kliencidostep)
//        {
//            var stany = A.Fake<ProduktyStanBll>();
//            var magazyny = WygenerujMagazyny();
//            A.CallTo(() => stany.WszystkieSlownik()).Returns(magazyny);

//            var sesja = A.Fake<SesjaHelper>();
//            A.CallTo(() => sesja.KlientID).Returns(idklienta);
//            var sposoby = A.Fake<SposobyPokazywaniaStanowBLL>();
//            sposoby.sesjaHelper = sesja;
//            sposoby.klienciDostep = kliencidostep;
//            sposoby.produktyStanBll = stany;
//            var slownikSposobow = WygenerujListeSposobow();
//            A.CallTo(() => sposoby.WszystkieSposoby(1)).Returns(slownikSposobow);

//            try
//            {
//                HashSet<int> pobraneIDmagazynu = sposoby.PobierzIDMagazynow(sposob, magazynid);

//                if(!oczekiwaneIDmagazynu.HasValue)
//                    Assert.Null(pobraneIDmagazynu);

//                else Assert.True(pobraneIDmagazynu.First() == oczekiwaneIDmagazynu);
//            }
//            catch(Exception ex)
//            {
//                if (!czyMozeWyrzucicWyjatek)
//                    throw ex;
//            }

//        }

//        private Dictionary<int,sposoby_pokazywania_stanow> WygenerujListeSposobow()
//        {
//            Dictionary<int, sposoby_pokazywania_stanow> slownik = new Dictionary<int, sposoby_pokazywania_stanow>();

//            slownik.Add(1, new sposoby_pokazywania_stanow() { IdDomyslnegoMagazynu = 5, KategoriaKlientaMagazyn = "COSTAM", id = 1});
//            slownik.Add(2, new sposoby_pokazywania_stanow() { IdDomyslnegoMagazynu = 4, KategoriaKlientaMagazyn = "B2B_MAGAZYN", id = 2 });
//            slownik.Add(3, new sposoby_pokazywania_stanow() { IdDomyslnegoMagazynu = 1, id = 3 });

//            return slownik;
//        }

//        private Dictionary<int, magazyny> WygenerujMagazyny()
//        {
//            Dictionary<int, magazyny> slownik = new Dictionary<int, magazyny>();
//            slownik.Add(1, new magazyny() { Id = 1, symbol = "MAG"});
//            slownik.Add(2, new magazyny() { Id = 2, symbol = "SZYBY" });
//            slownik.Add(3, new magazyny() { Id = 3, symbol = "MILICZ" });

//            return slownik;
//        }

//        [Fact()]
//        public void CzyPokazacStanNaPodstawieRoliKlientaTest()
//        {
//            SposobyPokazywaniaStanowBLL sposob = new SposobyPokazywaniaStanowBLL();
//            ISesjaHelper sesjaFake = A.Fake<ISesjaHelper>();
//            sposob.sesjaHelper = sesjaFake;
//            A.CallTo(() => sesjaFake.PrzedstawicielID).Returns(null);

//            IKlient klient = A.Fake<IKlient>();
//            A.CallTo(() => klient.Roles).Returns( new List<RoleType>(){ RoleType.Klient} );
//            //pusta lista - ma byc widoczny
//            List<RoleType> dozwolonaLista = new List<RoleType>();
//            Assert.True(sposob.CzyPokazacStanNaPodstawieRoliKlienta(klient, dozwolonaLista), "ma widzieć a nie widzi");

//            dozwolonaLista = new List<RoleType>(){ RoleType.Klient};
//            Assert.True(sposob.CzyPokazacStanNaPodstawieRoliKlienta(klient, dozwolonaLista), "ma widzieć a nie widzi - prosty klient");

//            dozwolonaLista = new List<RoleType>() { RoleType.Klient, RoleType.Pracownik };
//            Assert.True(sposob.CzyPokazacStanNaPodstawieRoliKlienta(klient, dozwolonaLista), "ma widzieć. klient i pracownik ");

//            //przypadek pracownika - i stan tylko dla pracownika
//            dozwolonaLista = new List<RoleType>() {RoleType.Pracownik};
//            A.CallTo(() => klient.Roles).Returns(new List<RoleType>() { RoleType.Klient });
//            Assert.False(sposob.CzyPokazacStanNaPodstawieRoliKlienta(klient, dozwolonaLista), "ma nie widzieć. klient ma nie widzieć stanu pracownika ");

//            //stan dla pracownika - i klient to pracownik
//            dozwolonaLista = new List<RoleType>() { RoleType.Pracownik };
//            A.CallTo(() => klient.Roles).Returns(new List<RoleType>() { RoleType.Klient, RoleType.Pracownik });
//            Assert.True(sposob.CzyPokazacStanNaPodstawieRoliKlienta(klient, dozwolonaLista), "ma widziec ");

//            //przypadek na max - pracownik zalogowany w imieniu klienta - ma widzieć stany DLA PRACOWNIKA
//            dozwolonaLista = new List<RoleType>() { RoleType.Pracownik };
//            A.CallTo(() => klient.Roles).Returns(new List<RoleType>() { RoleType.Klient });

//            A.CallTo(() => sesjaFake.PrzedstawicielID).Returns(1);

//            KlienciDostep klieciDostep = A.Fake<KlienciDostep>();
//            IKlient lipnyPrzedstawiciel = A.Fake<IKlient>();
//            A.CallTo(() => lipnyPrzedstawiciel.Roles).Returns( new List<RoleType>(){ RoleType.Klient, RoleType.Pracownik} );
//            A.CallTo(() => klieciDostep.Pobierz(1)).WithAnyArguments().Returns(lipnyPrzedstawiciel);

//            sposob.klienciDostep = klieciDostep;

//            Assert.True(sposob.CzyPokazacStanNaPodstawieRoliKlienta(klient, dozwolonaLista), "ma widziec ");
//        }
//    }
//}
