using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using FakeItEasy;
using ServiceStack.OrmLite;
using ServiceStack.ServiceClient.Web;
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
using AuthenticationException = System.Security.Authentication.AuthenticationException;
using Klient = SolEx.Hurt.Core.Klient;


namespace SolEx.Hurt.CoreTests.BLL
{    

    public class KlienciDostepTests
    {

        [Fact(DisplayName = "Test dla walidatora klientów")]
        public void WalidatorKlientowTests_testyPrzedstawicieliIOpieknow()
        {
            IKlient zadajacy = new Klient();
            zadajacy.Role = new HashSet<RoleType> {RoleType.Przedstawiciel};
            zadajacy.Id = 6;
            zadajacy.WidziWszystkich = false;

            //testy tylko dla przypadkow ktore maja nie przejsc
            Klient klient_InnegoPrzedstawiciela = new Klient() { Id = 56, Role = new HashSet<RoleType> { RoleType.Klient }, OpiekunId =  5};

            Assert.True(klient_InnegoPrzedstawiciela.OpiekunId != zadajacy.Id && klient_InnegoPrzedstawiciela.PrzedstawicielId == null && klient_InnegoPrzedstawiciela.DrugiOpiekunId == null);

            var walidatorSkompilowany = SolexBllCalosc.PobierzInstancje.Klienci.WalidatorKlientow.Compile();

            bool testWynik = walidatorSkompilowany(klient_InnegoPrzedstawiciela, zadajacy);
            Assert.True(testWynik == false, "Klient należy do innego przedstawiciela - nie moze byc pobrany");

            //teraz robimy zapytanie z zadajacym NULLem - ma wszystko sie pobrać bez znaczenia na warunki  - tego testu juz nie robimy bo walidatro nie ma juz funckji sprawdzenia jak jest NULL - po prostu walidator nie jest uruchamiany przy pobiernaiu danych
            //testWynik = walidatorSkompilowany(klient_InnegoPrzedstawiciela, null);
            //Assert.True(testWynik == true, "zadajacy == null - ma sie pobrac wszystko");

            //zadajcy admin
            zadajacy.Role = new HashSet<RoleType> { RoleType.Przedstawiciel, RoleType.Administrator };
            testWynik = walidatorSkompilowany(klient_InnegoPrzedstawiciela, zadajacy);
            Assert.True(zadajacy.CzyAdministrator);
            Assert.True(testWynik == true, "zadajacy == ADMIN - ma sie pobrac wszystko");

            //reset dla przedstawiciela
            zadajacy.Role = new HashSet<RoleType> { RoleType.Przedstawiciel };

            //jesli sam sie klient pobiera to też ma być OK
            zadajacy.Id = klient_InnegoPrzedstawiciela.Id;
            testWynik = walidatorSkompilowany(klient_InnegoPrzedstawiciela, zadajacy);
            Assert.True(zadajacy.Id == klient_InnegoPrzedstawiciela.Id);
            Assert.True(testWynik == true, "zadajacy == ten sam klient ID - ma sie pobrac wszystko");
            zadajacy.Id = klient_InnegoPrzedstawiciela.Id + 345;


            //a teraz juz ma byc pobrany bo juz widzi wszystkich
            zadajacy.WidziWszystkich = true;
            testWynik = walidatorSkompilowany(klient_InnegoPrzedstawiciela, zadajacy);
            Assert.True(testWynik == true, "Klient należy do innego przedstawiciela - ale ma byc pobrany bo przedstawiciel widzi wszystkich klientow");

            //po zmienie przedstawiciela ma byc ok
            zadajacy.WidziWszystkich = false;
            klient_InnegoPrzedstawiciela.OpiekunId = zadajacy.Id;
            testWynik = walidatorSkompilowany(klient_InnegoPrzedstawiciela, zadajacy);
            Assert.True(testWynik == true, "Klient należy do tego przedstawiciela - ma byc pobrany");
        }



        [Fact(DisplayName = "Test dla walidatora klientów - klient niezalogowany ma byc pobrany TYLKO dla admina albo jak null klient - nie moze byc pobrany")]
        public void WalidatorKlientowTests_testyKlientNiezalogowany()
        {
            IKlient zadajacy = new Klient();
            zadajacy.Role = new HashSet<RoleType> { RoleType.Przedstawiciel, RoleType.Administrator };
            zadajacy.Id = 6;
            zadajacy.WidziWszystkich = false;

            //testy tylko dla przypadkow ktore maja nie przejsc
            Klient klientTestowy = new Klient() { Id = 0, Role = new HashSet<RoleType> { RoleType.Klient } };
            Assert.True(klientTestowy.Dostep == AccesLevel.Niezalogowani);

            var walidatorSkompilowany = SolexBllCalosc.PobierzInstancje.Klienci.WalidatorKlientow.Compile();

            //zadajacy jako admina wyciagamy - ma przejsc
            bool testWynik = walidatorSkompilowany(klientTestowy, zadajacy);
            Assert.True(testWynik == true, "Zadajacy jest adminem - ma byc wyciągnięty niezalogowany");

            //zadajacy jako NULL - ma przejsc - tego testu juz nie robimy bo walidatro nie ma juz funckji sprawdzenia jak jest NULL - po prostu walidator nie jest uruchamiany przy pobiernaiu danych
            //testWynik = walidatorSkompilowany(klientTestowy, null);
            //Assert.True(testWynik == true, "Zadajacy jest NULLem - ma byc wyciągnięty niezalogowany");

            //zadajcy normalnym klientem - NIE moze przejsc
            zadajacy.Role = new HashSet<RoleType>() {RoleType.Klient};
            Assert.False(zadajacy.CzyAdministrator);
            testWynik = walidatorSkompilowany(klientTestowy, zadajacy);
            Assert.True(testWynik == false, "Zadajacy jest KLIENTEM normalnym - nie widzi niezalogowanego");
        }

        [Fact(DisplayName = "Test dla walidatora klientów - klient nadrzedny widzi wszystkie swoje dzieci")]
        public void WalidatorKlientowTests_testyKlientNadrzedny()
        {
            IKlient zadajacy = new Klient();
            zadajacy.Role = new HashSet<RoleType> { RoleType.Klient };
            zadajacy.Id = 6;

            //testy tylko dla przypadkow ktore maja nie przejsc
            Klient klientTestowy = new Klient() { Id = 56, Role = new HashSet<RoleType> { RoleType.Klient }, KlientNadrzednyId = 6};
            Assert.True(klientTestowy.KlientNadrzednyId == zadajacy.Id);

            var walidatorSkompilowany = SolexBllCalosc.PobierzInstancje.Klienci.WalidatorKlientow.Compile();

            bool testWynik = walidatorSkompilowany(klientTestowy, zadajacy);
            Assert.True(testWynik, "Zadajacy jest klientem nadrzednym - ma widziec swoje dziecko");
        }


        private ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
        private OrmLiteConnectionFactory polaczenie;
        private string baza = ":memory:";

        public KlienciDostepTests()
        {
            polaczenie = new OrmLiteConnectionFactory(baza, SqliteDialect.Provider)
            {
                DialectProvider = {UseUnicode = true},
                AutoDisposeConnection = false
            };
        }

        //bartek ma to napisac
        [Fact()]
        public void PobierzSzablonyWidoczneDlaKlientaTest()
        {
            ISolexBllCalosc Calosc = A.Fake<ISolexBllCalosc>();
            KlienciDostep klienciDostep = new KlienciDostep(Calosc);

            ICacheBll cache = A.Fake<ICacheBll>();
            A.CallTo(() => Calosc.Cache).Returns(cache);
            A.CallTo(() => cache.PobierzObiekt<IList<KatalogSzablonModelBLL>>(A<string>.Ignored)).Returns(null);

            IKlient klient = A.Fake<IKlient>();
            klient.Role = new HashSet<RoleType> { RoleType.Klient };

            Jezyk jezyk = new Jezyk();

            IList<KatalogSzablonModelBLL> lista = new List<KatalogSzablonModelBLL>
                {
                    new KatalogSzablonModelBLL(klient, 1) {Aktywny = true, Id = 1, DostepnyDla = new HashSet<RoleType> {RoleType.Klient, RoleType.Administrator}},
                    new KatalogSzablonModelBLL(klient, 1) {Aktywny = true, Id = 2, DostepnyDla = new HashSet<RoleType> {RoleType.Klient, RoleType.Administrator}},
                    new KatalogSzablonModelBLL(klient, 1) {Aktywny = true, Id = 3, DostepnyDla = new HashSet<RoleType> {RoleType.Administrator}}
                };
            
            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<KatalogSzablonModelBLL>();

                foreach (KatalogSzablonModelBLL szablon in lista)
                {
                    db.Insert(szablon);
                }
            }

            DostepDoDanych dostep = A.Fake<DostepDoDanych>();
            dostep.DbFactory = polaczenie;
            A.CallTo(() => Calosc.DostepDane).Returns(dostep);

            A.CallTo(dostep).Where(x => x.Method.Name == "Pobierz").WithReturnType< IList<KatalogSzablonModelBLL> >().WithAnyArguments().Returns(lista);

            //jak sa katalogi
            klienciDostep = new KlienciDostep(Calosc);
            var wynik = klienciDostep.PobierzSzablonyWidoczneDlaKlienta(klient);

            Assert.True(wynik.Count == 2);

            //BRAK katalogów
            klienciDostep = new KlienciDostep(Calosc);
            using (var db = polaczenie.OpenDbConnection())
            {
                db.DeleteAll<KatalogSzablonModelBLL>();
            }

            wynik = klienciDostep.PobierzSzablonyWidoczneDlaKlienta(klient);

            Assert.True(wynik == null, "Powinien być byll bo nie ma zadnych katalogow");
        }

        [Fact()]
        public void WyliczOdKiedyTest()
        {
            TestWyliczOdKiedy(6, null, new DateTime(2016, 1, 1), new DateTime(2016, 1, 1));
            TestWyliczOdKiedy(6, null, new DateTime(2016, 7, 2), new DateTime(2016, 7, 1));
            TestWyliczOdKiedy(6, null, new DateTime(2016, 7, 1), new DateTime(2016, 7, 1));
            TestWyliczOdKiedy(6, new DateTime(2016, 3, 1), new DateTime(2016, 7, 1), new DateTime(2016, 3, 1));
            TestWyliczOdKiedy(6, new DateTime(2016, 3, 1), new DateTime(2016, 10, 1), new DateTime(2016, 9, 1));
        }

        private void TestWyliczOdKiedy(int iloscMiesiecy, DateTime? odkiedy, DateTime aktualna, DateTime oczekiwana)
        {
            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            KlienciDostep kd = new KlienciDostep(calosc);
            SzablonLimitow sl = new SzablonLimitow();
            sl.IloscMiesiecy = iloscMiesiecy;
            sl.OdKiedy = odkiedy;
            DateTime data = kd.WyliczOdKiedy(sl);
            Assert.Equal(oczekiwana, data);
        }

        [Fact()]
        public void DodajPozycjeTest2()
        {
            JednostkaProduktu jp = new JednostkaProduktu(true, 1, 1, "szt.", 1);
            JednostkaProduktu joz = new JednostkaProduktu(false, 1, 1, "opz.", 12);
            JednostkaProduktu jkart = new JednostkaProduktu(false, 1, 1, "karton.", 56);
            DodajPozycjeTest3(1,1,jp,jp,jp,2);
        }
        private void DodajPozycjeTest3(decimal liczbaWKoszyku, decimal liczbaDoDodania, JednostkaProduktu jednostkaPodstawowa, JednostkaProduktu jednostkaKoszyka, JednostkaProduktu jednostkaDodawania, decimal liczbaOczekiwana)
        {
            IKoszykiBLL item = A.Fake<IKoszykiBLL>();
            IKlient klient = A.Fake<IKlient>();
            IKoszykPozycja wzor = A.Fake<IKoszykPozycja>();
            wzor.Id = 1;
            wzor.ProduktId = 1;
            wzor.Ilosc = liczbaDoDodania;
            wzor.Hash = 1;
            wzor.TypPozycji = TypPozycjiKoszyka.Zwykly;
            wzor.JednostkaId = jednostkaDodawania.Id;
            A.CallTo(() => wzor.Jednostka()).Returns(jednostkaDodawania);
            A.CallTo(() => wzor.Id).Returns(wzor.Id);
            A.CallTo(() => wzor.ProduktId).Returns(wzor.Id);
            A.CallTo(() => wzor.Ilosc).Returns(wzor.Ilosc);
            A.CallTo(() => wzor.Hash).Returns(wzor.Hash);
            A.CallTo(() => wzor.TypPozycji).Returns(TypPozycjiKoszyka.Zwykly);
            A.CallTo(() => wzor.JednostkaId).Returns(jednostkaDodawania.Id);

            KoszykiDostep koszykiDostep = new KoszykiDostep(calosc);
            IDaneDostep dostep = A.Fake<IDaneDostep>();
            A.CallTo(() => calosc.DostepDane).Returns(dostep);
            ProduktBazowy pb = new ProduktBazowy();
            pb.Id = 1;
            A.CallTo(dostep).Where(x => x.Method.Name == "PobierzPojedynczy").WithReturnType<ProduktBazowy>().WithAnyArguments().Returns(pb);
            
            IProduktKlienta pk = A.Fake<IProduktKlienta>();

            A.CallTo(() => pk.Jednostki).Returns(new List<JednostkaProduktu>() {jednostkaPodstawowa, jednostkaDodawania });

            KoszykPozycje poz1 = A.Fake<KoszykPozycje>();
            poz1.ProduktId = 1;
            poz1.Hash = wzor.Hash;
            poz1.TypPozycji = wzor.TypPozycji;
            poz1.JednostkaId = jednostkaKoszyka.Id;
            poz1.Ilosc = liczbaWKoszyku;
            A.CallTo(() => poz1.Produkt).Returns(pk);
            //A.CallTo(() => poz1.Jednostka()).Returns(jednostkaKoszyka);

            List<KoszykPozycje> lista = new List<KoszykPozycje> {poz1};
            A.CallTo(() => item.PobierzPozycje).Returns(lista);
            var wynik = koszykiDostep.DodajPozycje(item, klient, wzor);

            Assert.True(wynik.Ilosc == liczbaOczekiwana,
                $"Liczba w koszyku: {liczbaWKoszyku}, liczba dodana:{liczbaDoDodania}, iczba oczekiwana: {liczbaOczekiwana}, jednosta koszyka:{jednostkaKoszyka.Nazwa}, jednostka dodawania:{jednostkaDodawania.Nazwa}");

        }

        [Fact()]
        public void DokumentyDoLiczeniaLimitowTest()
        {
            TestPobieraniaDokumentow1();
            TestPobieraniaDokumentow2();
            TestPobieraniaDokumentow3();
        }

        private void TestPobieraniaDokumentow1()
        {
            DateTime odkiedy = new DateTime(2016, 1, 1);
            IKlient tmp = new Klient();
            tmp.Id = 1;
            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            IDokumentyDostep dok = A.Fake<IDokumentyDostep>();
            A.CallTo(() => calosc.DokumentyDostep).Returns(dok);
            List<DokumentyBll> dokfake = new List<DokumentyBll>();
            dokfake.Add(A.Fake<DokumentyBll>());
            A.CallTo(
                () =>
                    dok.PobierzWyfiltrowaneDokumenty(tmp, tmp, RodzajDokumentu.Zamowienie, odkiedy, A<DateTime>.Ignored,
                        A<string>.Ignored, A<bool>.Ignored, A<bool>.Ignored)).Returns(dokfake);

            HashSet<long> klienci = new HashSet<long>();
            klienci.Add(tmp.Id);
            KlienciDostep kd = A.Fake<KlienciDostep>(x => x.WithArgumentsForConstructor(new object[] {calosc}));
            A.CallTo(() => kd.PobierzWgIdLubZalogowanyAktualnie(1)).Returns(tmp);
            //A.CallTo(() => kd.Calosc).Returns(calosc);

            var wynik = kd.DokumentyDoLiczeniaLimitow(tmp, odkiedy, klienci);
            Assert.Equal(wynik.Count, 1);
        }

        private void TestPobieraniaDokumentow2()
        {
            DateTime odkiedy = new DateTime(2016, 1, 1);
            IKlient tmp = new Klient();
            tmp.Id = 1;
            IKlient idk2 = new Klient() {Id = 2};
            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            IDokumentyDostep dok = A.Fake<IDokumentyDostep>();
            A.CallTo(() => calosc.DokumentyDostep).Returns(dok);
            List<DokumentyBll> dokfake = new List<DokumentyBll>();
            dokfake.Add(A.Fake<DokumentyBll>());
            A.CallTo(
                () =>
                    dok.PobierzWyfiltrowaneDokumenty(tmp, idk2, RodzajDokumentu.Zamowienie, odkiedy, A<DateTime>.Ignored,
                        A<string>.Ignored, A<bool>.Ignored, A<bool>.Ignored)).Returns(dokfake);

            HashSet<long> klienci = new HashSet<long>();
            klienci.Add(idk2.Id);
            KlienciDostep kd = A.Fake<KlienciDostep>(x => x.WithArgumentsForConstructor(new object[] {calosc}));
            A.CallTo(() => kd.PobierzWgIdLubZalogowanyAktualnie(2)).Returns(idk2);
            //A.CallTo(() => kd.Calosc).Returns(calosc);

            var wynik = kd.DokumentyDoLiczeniaLimitow(tmp, odkiedy, klienci);
            Assert.Equal(wynik.Count, 1);
        }

        private void TestPobieraniaDokumentow3()
        {
            DateTime odkiedy = new DateTime(2016, 1, 1);
            IKlient tmp = new Klient();
            tmp.Id = 1;
            IKlient idk2 = new Klient() {Id = 2};
            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            IDokumentyDostep dok = A.Fake<IDokumentyDostep>();
            A.CallTo(() => calosc.DokumentyDostep).Returns(dok);
            List<DokumentyBll> dokfake = new List<DokumentyBll>();
            dokfake.Add(A.Fake<DokumentyBll>());
            A.CallTo(
                () =>
                    dok.PobierzWyfiltrowaneDokumenty(tmp, idk2, RodzajDokumentu.Zamowienie, odkiedy, A<DateTime>.Ignored,
                        A<string>.Ignored, A<bool>.Ignored, A<bool>.Ignored)).Returns(dokfake);
            A.CallTo(
                () =>
                    dok.PobierzWyfiltrowaneDokumenty(tmp, tmp, RodzajDokumentu.Zamowienie, odkiedy, A<DateTime>.Ignored,
                        A<string>.Ignored, A<bool>.Ignored, A<bool>.Ignored)).Returns(dokfake);

            HashSet<long> klienci = new HashSet<long>();
            klienci.Add(tmp.Id);
            klienci.Add(idk2.Id);
            KlienciDostep kd = A.Fake<KlienciDostep>(x => x.WithArgumentsForConstructor(new object[] {calosc}));
            //A.CallTo(() => kd.Calosc).Returns(calosc);
            A.CallTo(() => kd.PobierzWgIdLubZalogowanyAktualnie(1)).Returns(tmp);
            A.CallTo(() => kd.PobierzWgIdLubZalogowanyAktualnie(2)).Returns(idk2);
            var wynik = kd.DokumentyDoLiczeniaLimitow(tmp, odkiedy, klienci);

            Assert.Equal(wynik.Count, 2);
        }

        [Fact(DisplayName = "Test sprawdzający poprawność tworzenia sesji")]
        public void LogowanieKlientaTest()
        {
            //ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            IConfigBLL config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.GetLicense(Licencje.ZmianaIp)).Returns(true);
            A.CallTo(() => config.KategoriaKlientaNieBlokujZmianaIp).Returns(0);
            A.CallTo(() => calosc.Konfiguracja).Returns(config);

            KlienciDostep kd = A.Fake<KlienciDostep>(x => x.WithArgumentsForConstructor(new object[] {calosc}));
            A.CallTo(() => kd.SprawdzAdresIpKlienta(A<Klient>.Ignored)).DoesNothing();
            string login = "test";
            string haslo = "haslo";
            string ipKlienta = "ipKlienta";


            Klient klient = A.Fake<Klient>();
            klient.Login = login;
            klient.HasloOdkryte = haslo;
            klient.Aktywny = false;

            Klient k;
            //Jedno z pol puste -powinien wywalić wyjatek
            try
            {
                kd.CzyMoznaZalogowacKlienta("", haslo, ipKlienta, out k);
                Assert.True(false);
            }
            catch (InvalidDataException)
            {

            }
            A.CallTo(() => kd.PobierzPologinie(login, haslo, null)).Returns(klient);
            A.CallTo(() => kd.PobierzPologinie("test1", haslo, null)).Returns(null);

            //Brak klienta o określonych danych
            try
            {
                kd.CzyMoznaZalogowacKlienta("test1", haslo, ipKlienta, out k);
                Assert.True(false);
            }
            catch (InvalidOperationException)
            {
            }

            //Klient znalezione nie aktywny
            try
            {
                kd.CzyMoznaZalogowacKlienta(login, haslo, ipKlienta, out k);
                Assert.True(false);
            }
            catch (InvalidDataException)
            {
            }
            klient.Aktywny = true;
            klient.BlokadaZamowien = true;
            klient.PowodBlokady = BlokadaPowod.ZmianaAdresuIp;
            A.CallTo(() => klient.Kategorie).Returns(null);
            klient.Kategorie = null;

            //Klient nie moze sie zalogować zablokowane logowanie 
            try
            {
                kd.CzyMoznaZalogowacKlienta(login, haslo, ipKlienta, out k);
                Assert.True(false);
            }
            catch (InvalidOperationException)
            {
            }

            //Tworzymy sobie tabelke z sesjami
            ICacheBll cache = A.Fake<ICacheBll>();
            A.CallTo(() => calosc.Cache).Returns(cache);
            A.CallTo(() => cache.PobierzObiekt<IList<Sesja>>(A<string>.Ignored)).Returns(null);

            DostepDoDanych dostep = new DostepDoDanych(calosc);
            dostep.DbFactory = polaczenie;
            dostep.BindSelect<Sesja>(SposobCachowania.Brak, null, null, null);
            A.CallTo(() => calosc.DostepDane).Returns(dostep);
            klient.PowodBlokady = BlokadaPowod.Brak;
            klient.BlokadaZamowien = false;

           
            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<Sesja>();
            }

            var sesje = dostep.Pobierz<Sesja>(null);
            Assert.True(sesje.Count==0);

            throw new Exception("wentur porpaw to ");
            //Generujemy guidId oraz wprowadzamy do bazy wpis sesji
           // Guid guidId = kd.CzyMoznaZalogowacKlienta(login, haslo, ipKlienta, out k);

            //var sesja = dostep.PobierzPojedynczy<Sesja>(guidId);
            ////DataZakonczenia - null, przed wylogowaniem
            //Assert.True(!sesja.DataZakonczenia.HasValue);
            //sesja = dostep.PobierzPojedynczy<Sesja>(guidId);
            //Assert.True(sesja.DataZakonczenia.HasValue);


        }
        
        [Fact(DisplayName = "Test sprawdzający poprawne pobieranie wykorzystanego limitu przez klienta")]
        public void PobierzWykorzystanyLimitTest()
        {
            IKlient klient = new Klient();
            klient.SzablonLimitowId = 1;
            klient.Id = 1;
            
            SzablonLimitow szablon = new SzablonLimitow();
            szablon.Id = 1;
            szablon.IloscMiesiecy = 5;
            szablon.IloscMiesiecy = 2;
            //Dokumenty będa brane z ostatniego miesiaca
            szablon.OdKiedy = DateTime.Now.AddMonths(-1);
            szablon.WartoscZamowien = 5000;

            //Dokument prawidłowu utworzony 2 dni temu, powinien znalesc sie w doumentach branych pod uwagę liczenia
            HistoriaDokumentu h1 = new HistoriaDokumentu();
            h1.Id = 1;
            h1.DataUtworzenia = DateTime.Now.AddDays(-2);
            h1.WartoscNetto = 100;
            h1.KlientId = 1;

            //Dokument powinien być pominięty gdyż data utworzenia jest 1.5 misiace temu
            HistoriaDokumentu h2 = new HistoriaDokumentu();
            h2.Id = 2;
            h2.DataUtworzenia = DateTime.Now.AddDays(-45);
            h2.WartoscNetto = 200;
            h2.KlientId = 1;

            //Dokument powinien być pominiety inny klient niż jest żadanie
            HistoriaDokumentu h3 = new HistoriaDokumentu();
            h3.Id = 3;
            h3.DataUtworzenia = DateTime.Now.AddDays(-3);
            h3.WartoscNetto = 300;
            h3.KlientId = 2;


            //Prawidłowe zamówienie
            Zamowienie z1 = new Zamowienie();
            z1.KlientId = 1;
            z1.DataUtworzenia = DateTime.Now.AddDays(-1);
            z1.WartoscNetto = 500;
            z1.Id=1;
            z1.StatusId = StatusImportuZamowieniaDoErp.Złożone;

            //Zmowienie powinno byc pominiete jest w przedziale nie ma go w dokumentach ale ma status niewidoczny
            Zamowienie z2 = new Zamowienie();
            z2.KlientId = 1;
            z2.DataUtworzenia = DateTime.Now.AddDays(-1);
            z2.WartoscNetto = 600;
            z2.Id = 2;
            z2.StatusId = StatusImportuZamowieniaDoErp.Anulowane;

            
            //Prawidłowe jeszcze nie zaciągniete jako dokument
            Zamowienie z3 = new Zamowienie();
            z3.KlientId = 1;
            z3.DataUtworzenia = DateTime.Now.AddDays(-1);
            z3.WartoscNetto = 700;
            z3.Id = 3;
            z3.StatusId = StatusImportuZamowieniaDoErp.Zaimportowane;

            //zam pomiete jest juz zaciagniete jako dokument
            Zamowienie z4 = new Zamowienie();
            z4.KlientId = 1;
            z4.DataUtworzenia = DateTime.Now.AddDays(-1);
            z4.WartoscNetto = 800;
            z4.Id = 4;
            z4.StatusId = StatusImportuZamowieniaDoErp.Zaimportowane;

            ZamowienieDokumenty zd = new ZamowienieDokumenty();
            zd.IdZamowienia = 4;
            zd.IdDokumentu = 3;

            StatusZamowienia sz = new StatusZamowienia();
            sz.Id = 1;
            sz.Widoczny = true;

            StatusZamowienia sz2 = new StatusZamowienia();
            sz2.Id = 2;
            sz2.Widoczny = true;

            StatusZamowienia sz3 = new StatusZamowienia();
            sz3.Id =3;
            sz3.Widoczny = false;

            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.DbFactory = polaczenie;

            ICacheBll cache = A.Fake<ICacheBll>();
            A.CallTo(() => solexBllCalosc.Cache).Returns(cache);

            A.CallTo(() => solexBllCalosc.DostepDane).Returns(dostep);


            A.CallTo(cache).Where(w => w.Method.Name == "PobierzObiekt").WithReturnType<decimal?>().WithAnyArguments().Returns(null);
            A.CallTo(cache).Where(w => w.Method.Name == "PobierzObiekt").WithReturnType<int?>().WithAnyArguments().Returns(null);
            //A.CallTo(() => cache.PobierzObiekt<decimal?>(A<Func<decimal?>>.Ignored, A<object>.Ignored, A<string>.Ignored, A<object[]>.Ignored)).Returns(null);
            //A.CallTo(() => cache.PobierzObiekt<int?>(A<Func<int?>>.Ignored, A<object>.Ignored, A<string>.Ignored, A<object[]>.Ignored)).Returns(null);

            KlienciDostep klienciDostep = new KlienciDostep(solexBllCalosc);

            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<SzablonLimitow>();
                db.CreateTable<HistoriaDokumentu>();
                db.CreateTable<Zamowienie>();
                db.CreateTable<ZamowienieDokumenty>();
                db.CreateTable<StatusZamowienia>();

                db.Insert(szablon);
                db.Insert(h1);
                db.Insert(h2);
                db.Insert(h3);
                db.Insert(z1);
                db.Insert(z2);
                db.Insert(z3);
                db.Insert(z4);
                db.Insert(zd);

                db.Insert(sz2); db.Insert(sz);
                db.Insert(sz3);
            }
            //todo: Zapytaj Bartka jak mozna zfejkowac Pobieranie cache dla limitow 
            Assert.True(false);
            var ilosc = klienciDostep.PobierzWykorzystanyLimit<int?>(klient, szablon, RodzajLimitu.LimitIlosciZamowien);
            //Powinny byc dok h1 oraz zamowienie z3 i z1
            Assert.True(ilosc==3);
            var wartosc = klienciDostep.PobierzWykorzystanyLimit<decimal?>(klient, szablon, RodzajLimitu.LimitWartosciZamowien);
            Assert.True(wartosc == 1300);

        }


    }
}
