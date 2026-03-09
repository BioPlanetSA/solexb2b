using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FakeItEasy;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.ObiektyMaili;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using Xunit;

namespace SolEx.Hurt.CoreTests.BLL
{
    public class DokumentyDostepTests
    {
        private string baza = ":memory:";
        private OrmLiteConnectionFactory polaczenie;

        public DokumentyDostepTests()
        {
            polaczenie = new OrmLiteConnectionFactory(baza, SqliteDialect.Provider)
            {
                DialectProvider = {UseUnicode = true},
                AutoDisposeConnection = false
            };
        }

        [Fact()]
        public void PobierzElementyPoSelectTest()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            IListyPrzewozoweBll listyPrzewozowe = A.Fake<IListyPrzewozoweBll>();
            IKlienciDostep klienci = A.Fake<IKlienciDostep>();

            IDaneDostep dostepDodanych = A.Fake<IDaneDostep>();

            //Sprawdzanie czy dobrze wypełniają się listy przewozowe 
            IDokumentyDostep dd = new DokumentyDostep(solexBllCalosc);
            HistoriaDokumentuListPrzewozowy list1 = new HistoriaDokumentuListPrzewozowy();
            list1.DokumentId = 1;
            list1.NumerListu = "123";
            HistoriaDokumentuListPrzewozowy list2 = new HistoriaDokumentuListPrzewozowy();
            list2.DokumentId = 2;
            list2.NumerListu = "123";
            HistoriaDokumentuListPrzewozowy list3 = new HistoriaDokumentuListPrzewozowy();
            list3.DokumentId = 3;
            list3.NumerListu = "123";

            IKlient zadajacy = A.Fake<IKlient>();
            zadajacy.Id = 1;

            IList<HistoriaDokumentuListPrzewozowy> lista = new List<HistoriaDokumentuListPrzewozowy>() {list1, list2, list3};
            A.CallTo(() => solexBllCalosc.ListyPrzewozoweBll).Returns(listyPrzewozowe);
            A.CallTo(() => solexBllCalosc.Klienci).Returns(klienci);
            A.CallTo(() => solexBllCalosc.DostepDane).Returns(dostepDodanych);

            A.CallTo(dostepDodanych).Where(x => x.Method.Name == "Pobierz").WithReturnType<IList<HistoriaDokumentuListPrzewozowy>>().WithAnyArguments().Returns(lista);

            A.CallTo(() => klienci.Pobierz(A<long>.Ignored, A<IKlient>.Ignored)).Returns(new Core.Klient());

            DokumentyBll dok1 = new DokumentyBll();
            dok1.Id = 1;
            DokumentyBll dok2 = new DokumentyBll();
            dok2.Id = 2;
            DokumentyBll dok3 = new DokumentyBll();
            dok3.Id = 3;
            List<DokumentyBll> listaDokumentow = new List<DokumentyBll>() {dok1, dok2, dok3};

            var wynik = dd.PobierzElementyPoSelect<DokumentyBll>(1, zadajacy, listaDokumentow);
            Assert.True(wynik.Any(x => x.ListyPrzewozowe.Any()));
        }

        [Fact(DisplayName = "Test sprawdzająy czy prawidłow budujemy warunek dla klientów jeżeli sa badz nie kategorie klienta")]
        public void ZbudujWarunekDlaKategoriiTest()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            int[] kategoriaKlientaNieWysylaj = new[] {1, 2, 3};
            int[] kategoriaKlientaWysylaj = new[] {4, 5};

            //Obie lisy maja dane
            DokumentyDostep dd = new DokumentyDostep(solexBllCalosc);
            string wynik = dd.ZbudujWarunekDlaKategorii(kategoriaKlientaNieWysylaj, kategoriaKlientaWysylaj);
            Assert.Equal(wynik, "select distinct KlientId from KlientKategoriaKlienta where (KategoriaKlientaId in (1,2,3) or KategoriaKlientaId not in (4,5))");

            //Tylko nie wysylaj ma dane
            wynik = dd.ZbudujWarunekDlaKategorii(kategoriaKlientaNieWysylaj, null);
            Assert.Equal(wynik, "select distinct KlientId from KlientKategoriaKlienta where (KategoriaKlientaId in (1,2,3))");

            //Tylko wysylaj ma wartosc
            wynik = dd.ZbudujWarunekDlaKategorii(null, kategoriaKlientaWysylaj);
            Assert.Equal(wynik, "select distinct KlientId from KlientKategoriaKlienta where ( KategoriaKlientaId not in (4,5))");

            //Nie ma nic podanego
            wynik = dd.ZbudujWarunekDlaKategorii(null, null);
            Assert.Equal(wynik, string.Empty);
        }

        //[Fact(DisplayName = "Test sprawdzająy czy w przypadku pustego ustawienia kategorii klienta dokona sie jakieś filtrowanie")]
        //public void PobierzFakturyDoWyslaniaInfoTest()
        //{
        //    HistoriaDokumentu hd1 = new HistoriaDokumentu();
        //    hd1.KlientId = 1;
        //    hd1.WartoscNetto = 100m;

        //    HistoriaDokumentu hd2 = new HistoriaDokumentu();
        //    hd2.KlientId = 1;
        //    hd2.WartoscNetto = 150;

        //    HistoriaDokumentu hd3 = new HistoriaDokumentu();
        //    hd3.KlientId = 2;
        //    hd3.WartoscNetto = 200;

        //    HistoriaDokumentu hd4 = new HistoriaDokumentu();
        //    hd4.KlientId = 3;
        //    hd4.WartoscNetto = 250;

        //    Dictionary<HistoriaDokumentu,bool> przeterminowane = new Dictionary<HistoriaDokumentu, bool>() { { hd1, false }, { hd2, false }, { hd3, false }, { hd4, false } };

        //    HistoriaDokumentu hd5 = new HistoriaDokumentu();
        //    hd5.KlientId = 1;
        //    hd5.WartoscNetto = 100m;

        //    HistoriaDokumentu hd6 = new HistoriaDokumentu();
        //    hd6.KlientId = 2;
        //    hd6.WartoscNetto = 150;

        //    HistoriaDokumentu hd7 = new HistoriaDokumentu();
        //    hd7.KlientId = 3;
        //    hd7.WartoscNetto = 200;

        //    HistoriaDokumentu hd8 = new HistoriaDokumentu();
        //    hd8.KlientId = 3;
        //    hd8.WartoscNetto = 250;

        //    Dictionary<HistoriaDokumentu,bool> nieZaplacone = new Dictionary<HistoriaDokumentu, bool>() { {hd5,false}, { hd6, false }, { hd7, false }, { hd8, false } };

        //    ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
        //    IConfigBLL config = A.Fake<IConfigBLL>();
        //    IDaneDostep dostep = A.Fake<IDaneDostep>();
        //    A.CallTo(() => calosc.Konfiguracja).Returns(config);
        //    A.CallTo(() => config.KategoriaKlientaNieWysylajInfoOprzeterminowanych).Returns(new HashSet<int>());
        //    A.CallTo(() => calosc.DostepDane).Returns(dostep);

        //    DokumentyDostep dd = new DokumentyDostep(calosc);
        //    var wynik = dd.PobierzFakturyDoWyslaniaInfo(nieZaplacone, przeterminowane);
        //    Assert.True(wynik.Count==3);
        //    Assert.True(wynik.ContainsKey(1) && wynik[1].Count==3);
        //    Assert.True(wynik.ContainsKey(2) && wynik[2].Count ==2);
        //    Assert.True(wynik.ContainsKey(3) && wynik[3].Count == 3);

        //    HashSet<int>blokowani = new HashSet<int>() { 1 };
        //    A.CallTo(() => config.KategoriaKlientaNieWysylajInfoOprzeterminowanych).Returns(blokowani);
        //    IList<KlientKategoriaKlienta> laczniki = new List<KlientKategoriaKlienta>() { new KlientKategoriaKlienta() { KategoriaKlientaId = 1, KlientId = 1 } };
        //    A.CallTo(dostep).Where(x => x.Method.Name == "Pobierz").WithReturnType<IList<KlientKategoriaKlienta>>().WithAnyArguments().Returns(laczniki);

        //    wynik = dd.PobierzFakturyDoWyslaniaInfo(nieZaplacone, przeterminowane);

        //    Assert.True(wynik.Count == 2);
        //    Assert.False(wynik.ContainsKey(1));
        //    Assert.True(wynik.ContainsKey(2) && wynik[2].Count == 2);
        //    Assert.True(wynik.ContainsKey(3) && wynik[3].Count == 3);


        //}

        [Fact(DisplayName = "Test sprawdzająy czy poprawnie pobieraja sie zamowienia powiazane")]
        public void PobierzDokumentZZamowieniemPowiazanymTest()
        {
            DokumentyBll dok = new DokumentyBll();
            dok.Id = 1;
            dok.NazwaDokumentu = "Nazwa dokumentu";

            IKlient kl = new Core.Klient();

            ZamowieniaBLL zam = new ZamowieniaBLL();
            zam.Id = -1;

            ZamowienieDokumenty zd1 = new ZamowienieDokumenty(1, 1, "aaaa");
            ZamowienieDokumenty zd2 = new ZamowienieDokumenty(2, 2, "aaaa");
            ZamowienieDokumenty zd3 = new ZamowienieDokumenty(3, 3, "ccc");
            List<ZamowienieDokumenty> listZd = new List<ZamowienieDokumenty>() {zd1, zd2, zd3};

            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            IDaneDostep dd = A.Fake<IDaneDostep>();
            A.CallTo(() => calosc.DostepDane).Returns(dd);
            A.CallTo(dd).Where(x => x.Method.Name == "PobierzPojedynczy").WithReturnType<DokumentyBll>().WithAnyArguments().Returns(dok);
            A.CallTo(dd).Where(x => x.Method.Name == "Pobierz").WithReturnType<IList<ZamowienieDokumenty>>().WithAnyArguments().Returns(listZd);
            A.CallTo(dd).Where(x => x.Method.Name == "PobierzPojedynczy").WithReturnType<ZamowieniaBLL>().WithAnyArguments().Returns(zam);

            DokumentyDostep dokDostep = new DokumentyDostep(calosc);
            DokumentyBll wynik = dokDostep.PobierzDokumentZZamowieniemPowiazanym(1, 1, kl);
            Assert.True(wynik.PowiazaneZamowienieB2B != null);
            Assert.True(wynik.PowiazaneZamowienieB2B.Id == zam.Id);



            DokumentyBll dok2 = new DokumentyBll();
            dok.Id = 8;
            dok.NazwaDokumentu = "Nazwa dokumentu 8";
            A.CallTo(dd).Where(x => x.Method.Name == "PobierzPojedynczy").WithReturnType<DokumentyBll>().WithAnyArguments().Returns(dok2);

            DokumentyBll wynik2 = dokDostep.PobierzDokumentZZamowieniemPowiazanym(8, 1, kl);
            Assert.True(wynik2.PowiazaneZamowienieB2B == null);
        }



        [Fact(DisplayName = "Test sprawdzająy czy poprawnie sprawdzamy czy klient ma dokumenty do pokazania")]
        public void CzySaDokumentyDlaKlienta()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            ICacheBll cache = A.Fake<ICacheBll>();
            A.CallTo(() => solexBllCalosc.Cache).Returns(cache);
            A.CallTo(() => cache.PobierzObiekt<IList<Model.Klient>>(A<string>.Ignored)).Returns(null);


            IKlient klient = new Core.Klient(null);
            klient.Id = 1;

            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.DbFactory = polaczenie;

            A.CallTo(() => solexBllCalosc.DostepDane).Returns(dostep);

            StatusZamowienia s1 = new StatusZamowienia();
            s1.Id = 1;
            s1.Widoczny = true;
            s1.Nazwa = "s1";
            StatusZamowienia s2 = new StatusZamowienia();
            s1.Id = 2;
            s2.Widoczny = false;
            s2.Nazwa = "s2";


            HistoriaDokumentu h1 = new HistoriaDokumentu();
            h1.KlientId = klient.Id;
            h1.Id = 1;
            h1.Rodzaj = RodzajDokumentu.Faktura;
            h1.StatusId = 2;

            HistoriaDokumentu h2 = new HistoriaDokumentu();
            h1.KlientId = klient.Id;
            h2.Id = 2;
            h2.Rodzaj = RodzajDokumentu.Faktura;
            h2.StatusId = 1;

            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<StatusZamowienia>();
                db.Insert(s1);
                db.Insert(s2);
                db.CreateTable<HistoriaDokumentu>();
                //dodajemy dokument ze statusem niewidoczny
                db.Insert(h2);
            }
            DokumentyDostep dd = new DokumentyDostep(solexBllCalosc);
            //Jeden dokument ze statusem niewidocznym
            Assert.False(dd.CzySaDokumentyDlaKlienta(RodzajDokumentu.Faktura, klient));

            //Dodajemy dokument z widocznym statusem
            using (var db = polaczenie.OpenDbConnection())
            {
                db.Insert(h1);
            }
            Assert.True(dd.CzySaDokumentyDlaKlienta(RodzajDokumentu.Faktura, klient));

            //Usuwamy dokumenty oraz dodajemy jeden który nie ma statusu
            h1.StatusId = null;
            using (var db = polaczenie.OpenDbConnection())
            {
                db.DropAndCreateTable<HistoriaDokumentu>();
                db.Insert(h1);
            }
            Assert.True(dd.CzySaDokumentyDlaKlienta(RodzajDokumentu.Faktura, klient));

        }

        [Fact(DisplayName = "Test sptawdzający poprawne działanie metodu po usunieciu dokumentów")]
        public void UsunZamowienieDokumentTest()
        {

            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            IConfigBLL konfiguracja = A.Fake<IConfigBLL>();
            A.CallTo(() => solexBllCalosc.Konfiguracja).Returns(konfiguracja);

            Zamowienie z = new Zamowienie()
            {
                Id = -1,
                KlientId = 1,
                StatusId = StatusImportuZamowieniaDoErp.Zaimportowane,
                Uwagi = "test",
                PoziomCenyId = 1,
                WartoscNetto = 10m,
                WartoscBrutto = 10m,
                WalutaId = 1,
                NumerTymczasowyZamowienia = "1/2016"

            };


            Zamowienie z2 = new Zamowienie()
            {
                Id = -2,
                KlientId = 1,
                StatusId = StatusImportuZamowieniaDoErp.Złożone,
                Uwagi = "test",
                PoziomCenyId = 1,
                WartoscNetto = 10m,
                WartoscBrutto = 10m,
                WalutaId = 1,
                NumerTymczasowyZamowienia = "2/2016"

            };

            ZamowienieDokumenty zd = new ZamowienieDokumenty(-1, 1, "test");

            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<Zamowienie>();
                db.CreateTable<ZamowienieDokumenty>();

                db.Insert(z);
                db.Insert(z2);
                db.Insert(zd);
            }

            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.BindPoUsunieciu<HistoriaDokumentu, int>(obj => SolexBllCalosc.PobierzInstancje.DokumentyDostep.UsunZamowienieDokument(obj));
            dostep.DbFactory = polaczenie;
            A.CallTo(() => solexBllCalosc.DostepDane).Returns(dostep);

            DokumentyDostep dd = new DokumentyDostep(solexBllCalosc);
            dd.UsunZamowienieDokument(new List<int>() {1});

            var zamowienie = dostep.PobierzPojedynczy<Zamowienie>(-1);
            var zamowienie2 = dostep.PobierzPojedynczy<Zamowienie>(-2);
            var zamowienieDokument = dostep.Pobierz<ZamowienieDokumenty>(null);

            Assert.True(zamowienie.StatusId == StatusImportuZamowieniaDoErp.Usunięte);
            Assert.True(zamowienie2.StatusId == StatusImportuZamowieniaDoErp.Złożone);
            Assert.True(zamowienieDokument == null || !zamowienieDokument.Any());


            using (var db = polaczenie.OpenDbConnection())
            {
                db.DropAndCreateTable<Zamowienie>();
                db.DropAndCreateTable<ZamowienieDokumenty>();

                db.Insert(z);
                db.Insert(z2);
            }

            dd.UsunZamowienieDokument(new List<int>() {1});



            zamowienie = dostep.PobierzPojedynczy<Zamowienie>(-1);
            zamowienie2 = dostep.PobierzPojedynczy<Zamowienie>(-2);
            Assert.True(zamowienie.StatusId == StatusImportuZamowieniaDoErp.Zaimportowane);
            Assert.True(zamowienie2.StatusId == StatusImportuZamowieniaDoErp.Złożone);


            ZamowienieDokumenty zd2 = new ZamowienieDokumenty(-1, 2, "test");

            using (var db = polaczenie.OpenDbConnection())
            {
                db.DropAndCreateTable<Zamowienie>();
                db.DropAndCreateTable<ZamowienieDokumenty>();

                db.Insert(z);
                db.Insert(z2);
                db.Insert(zd);
                db.Insert(zd2);
            }
            dd.UsunZamowienieDokument(new List<int>() {1});

            zamowienie = dostep.PobierzPojedynczy<Zamowienie>(z.Id);
            zamowienie2 = dostep.PobierzPojedynczy<Zamowienie>(z2.Id);
            Assert.True(zamowienie.StatusId == StatusImportuZamowieniaDoErp.Zaimportowane);
            Assert.True(zamowienie2.StatusId == StatusImportuZamowieniaDoErp.Złożone);



        }


        [Fact(DisplayName = "Test sptawdzający poprawne usuwanie powiazan dokumentow z zamowieniem po zmianie statusu zamowienia")]
        public void UsunZamowienieDokumentTest2()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            IConfigBLL konfiguracja = A.Fake<IConfigBLL>();
            A.CallTo(() => solexBllCalosc.Konfiguracja).Returns(konfiguracja);

            Zamowienie z = new Zamowienie()
            {
                Id = -1,
                KlientId = 1,
                StatusId = StatusImportuZamowieniaDoErp.Zaimportowane,
                Uwagi = "test",
                PoziomCenyId = 1,
                WartoscNetto = 10m,
                WartoscBrutto = 10m,
                WalutaId = 1,
                NumerTymczasowyZamowienia = "1/2016"

            };
            Zamowienie z2 = new Zamowienie()
            {
                Id = -2,
                KlientId = 1,
                StatusId = StatusImportuZamowieniaDoErp.Zaimportowane,
                Uwagi = "test",
                PoziomCenyId = 1,
                WartoscNetto = 10m,
                WartoscBrutto = 10m,
                WalutaId = 1,
                NumerTymczasowyZamowienia = "1/2016"

            };
            ZamowienieDokumenty zd = new ZamowienieDokumenty(-1, 1, "test");
            ZamowienieDokumenty zd2 = new ZamowienieDokumenty(-2, 1, "test2");

            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<Zamowienie>();
                db.CreateTable<ZamowienieDokumenty>();

                db.Insert(z);
                db.Insert(z2);
                db.Insert(zd);
                db.Insert(zd2);
            }

            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.BindPoUsunieciu<HistoriaDokumentu, int>(obj => SolexBllCalosc.PobierzInstancje.DokumentyDostep.UsunZamowienieDokument(obj));
            dostep.BindPoUpdate<ZamowieniaBLL>(SolexBllCalosc.PobierzInstancje.DokumentyDostep.UsunDokumentDlaUsunietegoZamowienia);
            dostep.DbFactory = polaczenie;
            A.CallTo(() => solexBllCalosc.DostepDane).Returns(dostep);

            ZamowieniaBLL zam = new ZamowieniaBLL()
            {
                Id = -1,
                KlientId = 1,
                StatusId = StatusImportuZamowieniaDoErp.Usunięte,
                Uwagi = "test",
                PoziomCenyId = 1,
                WartoscNetto = 10m,
                WartoscBrutto = 10m,
                WalutaId = 1,
                NumerTymczasowyZamowienia = "1/2016"

            };

            DokumentyDostep dd = new DokumentyDostep(solexBllCalosc);
            dd.UsunDokumentDlaUsunietegoZamowienia(new List<ZamowieniaBLL>() {zam});

            var zamDOkumenty = dostep.Pobierz<ZamowienieDokumenty>(null);
            Assert.True(zamDOkumenty.Count == 1);


            using (var db = polaczenie.OpenDbConnection())
            {
                db.DropAndCreateTable<Zamowienie>();
                db.DropAndCreateTable<ZamowienieDokumenty>();

                db.Insert(z);
                db.Insert(z2);
                db.Insert(zd);
                db.Insert(zd2);
            }

            zam = new ZamowieniaBLL()
            {
                Id = -10,
                KlientId = 1,
                StatusId = StatusImportuZamowieniaDoErp.Złożone,
                Uwagi = "test",
                PoziomCenyId = 1,
                WartoscNetto = 10m,
                WartoscBrutto = 10m,
                WalutaId = 1,
                NumerTymczasowyZamowienia = "1/2016"

            };
            dd.UsunDokumentDlaUsunietegoZamowienia(new List<ZamowieniaBLL>() {zam});
            zamDOkumenty = dostep.Pobierz<ZamowienieDokumenty>(null);
            Assert.True(zamDOkumenty.Count == 2);

        }

        private HistoriaDokumentu StworzDokument(long idKlienta, bool tenSamOdbioraca)
        {
            Random rnd = new Random();
            HistoriaDokumentu h = new HistoriaDokumentu();
            h.KlientId = idKlienta;
            h.OdbiorcaId = tenSamOdbioraca ? idKlienta : rnd.Next(50, 100);
            h.Zaplacono = false;
            h.Rodzaj = RodzajDokumentu.Faktura;
            h.WartoscBrutto = rnd.Next(100, 500);
            h.TerminPlatnosci = DateTime.Now.AddDays(-10);
            h.WartoscNalezna = 100;
            h.StatusId = 1;

            return h;
        }

        private List<HistoriaDokumentu> WygenerujDokumenty()
        {
            HistoriaDokumentu h1 = StworzDokument(1, false);
            h1.Id = 1;

            //Dokument który jest zamówieniem
            HistoriaDokumentu h2 = StworzDokument(1, false);
            h2.Id = 2;
            h2.Rodzaj = RodzajDokumentu.Zamowienie;

            HistoriaDokumentu h3 = StworzDokument(h1.OdbiorcaId.Value, false);
            h3.Id = 3;

            HistoriaDokumentu h4 = StworzDokument(1, false);
            h4.Id = 4;
            h4.StatusId = 2;
            h4.WartoscNalezna = 500;

            return new List<HistoriaDokumentu>() {h1,h2,h3};
        }

        [Fact(DisplayName = "Test popawności wyciagania którym klientom trzeba wysłać maila z przypomniejniem o płatnościach")]
        public void PobierzKlientowKtorymWyslacMaileOPrzeterminowaniuTest()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            List<HistoriaDokumentu> dokumenty = WygenerujDokumenty();

            StworzBazeDokumentow(solexBllCalosc, dokumenty);
            DokumentyDostep dd = new DokumentyDostep(solexBllCalosc);
            string warunek = dd.ZbudujWarunekDlaKategorii(new int[0], new int[0]);
            Dictionary<long, List<HistoriaDokumentu>> slownikDokumentowPogrupowanychPoKliencie;
            HashSet<long> wynik = dd.PobierzKlientowKtorymWyslacMaileOPrzeterminowaniu(1, 1, 1, warunek, "2017-05-18", out slownikDokumentowPogrupowanychPoKliencie);


            Assert.True(wynik.Count == 2 && wynik.Contains(1) && wynik.Contains(dokumenty[0].OdbiorcaId.Value));
            Assert.True(slownikDokumentowPogrupowanychPoKliencie[1].Count == 1 && slownikDokumentowPogrupowanychPoKliencie[dokumenty[0].OdbiorcaId.Value].Count == 1);

            using (var db = polaczenie.OpenDbConnection())
            {
                db.Insert(new DzialaniaUzytkownikow() { Id = 1, Data = DateTime.Now, EmailKlienta = "test", ZdarzenieGlowne = ZdarzenieGlowne.PrzypomnienieNiezaplaconejFakturze });
                db.Insert(new DzialaniaUzytkwonikowParametry("Dokumenty id", 1, "1"));
            }
            
            slownikDokumentowPogrupowanychPoKliencie.Clear();
            wynik = dd.PobierzKlientowKtorymWyslacMaileOPrzeterminowaniu(1, 1, 1, warunek, "2017-05-18", out slownikDokumentowPogrupowanychPoKliencie);
            //Klient o id  został pominiey nie maial nowych dokumentów w porównaniu z ostatnim mailem
            Assert.True(wynik.Count == 1 && wynik.Contains(dokumenty[0].OdbiorcaId.Value));
            Assert.True(slownikDokumentowPogrupowanychPoKliencie.ContainsKey(1));
            Assert.True(slownikDokumentowPogrupowanychPoKliencie[dokumenty[0].OdbiorcaId.Value].Count == 1);
        }



        private void StworzBazeDokumentow(ISolexBllCalosc solexBllCalosc, List<HistoriaDokumentu> dokumenty)
        {
            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.DbFactory = polaczenie;

            A.CallTo(() => solexBllCalosc.DostepDane).Returns(dostep);
            Model.Klient klient = new Model.Klient(null) { Id = 1 };
            Model.Klient klient2 = new Model.Klient(null) { Id = dokumenty[0].OdbiorcaId.Value };

            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<HistoriaDokumentu>();
                db.Insert(dokumenty[0]);
                db.Insert(dokumenty[1]);
                db.Insert(dokumenty[2]);
                db.CreateTable<Model.Klient>();

                db.Insert(klient);
                db.Insert(klient2);

                db.CreateTable<DzialaniaUzytkownikow>();
                db.CreateTable<DzialaniaUzytkwonikowParametry>();
            }
        }

        [Fact(DisplayName = "Test sprawdzający poprawne ustawienia propertiśow na obiekcie mailowym - przeterminowane platnosci")]
        public void GlownaMetodaTestujacaObiektPrzeterminowanychPlatnosci()
        {
            IKlient k = new Core.Klient() {Email = "test@test.pl", Id = 1};

            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            A.CallTo(() => solexBllCalosc.Konfiguracja.StatusyZamowien).Returns(new Dictionary<int, StatusZamowienia>() { { 1, new StatusZamowienia() { Id = 1, TraktujJakoFaktoring = false } }, { 2, new StatusZamowienia() { Id = 2, TraktujJakoFaktoring = true } }});
            Random rnd = new Random();


            PrzeterminowanePlatnosci obiekt =A.Fake<PrzeterminowanePlatnosci>();
            List<DokumentyBll> doki = new List<DokumentyBll>();

            DokumentyBll dok1 = A.Fake<DokumentyBll>();
            dok1.Klient = k;
            dok1.KlientId = 1;
            dok1.OdbiorcaId = 1;
            dok1.Zaplacono = false;
            dok1.Rodzaj = RodzajDokumentu.Faktura;
            dok1.WartoscBrutto = rnd.Next(100, 500);
            A.CallTo(() => dok1.TerminPlatnosci).Returns(DateTime.Now.AddDays(-10));
            A.CallTo(() => dok1.DokumentWartoscNalezna).Returns(100);
            A.CallTo(() => dok1.CzyPrzeterminowany()).Returns(true);
            dok1.StatusId = 1;

            DokumentyBll dok2 = A.Fake<DokumentyBll>();
            dok2.Klient = k;
            dok2.KlientId = 1;
            dok2.OdbiorcaId = 1;
            dok2.Zaplacono = false;
            dok2.Rodzaj = RodzajDokumentu.Faktura;
            dok2.WartoscBrutto = rnd.Next(100, 500);
            A.CallTo(() => dok2.TerminPlatnosci).Returns(DateTime.Now.AddDays(-10));
            A.CallTo(()=> dok2.DokumentWartoscNalezna).Returns(500);
            dok2.StatusId = 2;
            A.CallTo(() => dok2.CzyPrzeterminowany()).Returns(true);


            doki.Add(dok1);
            doki.Add(dok2);

            obiekt.Dokumenty = doki;
            obiekt.Klient = k;
            obiekt.Calosc = solexBllCalosc;

            Assert.True(TestPrawidlowegoKlienta(obiekt));
            Assert.True(TestIloscDokumentowPrzeterminowanych(obiekt));
            Assert.True(testIloscDokumentowNadchodzacych(obiekt));
            Assert.True(testIloscDokumentowFaktoringowe(obiekt));
            Assert.True(testIloscDokumentowWlasne(obiekt));
            Assert.True(tesWlasneNadchodzaceDoZaplaty(obiekt));
            Assert.True(tesWlasnePrzeterminowaneDoZaplaty(obiekt));

        }

        private bool TestPrawidlowegoKlienta(PrzeterminowanePlatnosci obiekt)
        {
            return obiekt.Klient.Id == obiekt.Dokumenty.First().KlientId;
        }

        private bool TestIloscDokumentowPrzeterminowanych(PrzeterminowanePlatnosci obiekt)
        {
            return obiekt.Przeterminowane.Count==2;
        }
        private bool testIloscDokumentowNadchodzacych(PrzeterminowanePlatnosci obiekt)
        {
            return obiekt.Nadchodzace.Count == 0;
        }
        private bool testIloscDokumentowFaktoringowe(PrzeterminowanePlatnosci obiekt)
        {
            return obiekt.Faktoringowe.Count == 1;
        }
        private bool testIloscDokumentowWlasne(PrzeterminowanePlatnosci obiekt)
        {
            return obiekt.Wlasne.Count == 1;
        }
        private bool tesWlasneNadchodzaceDoZaplaty(PrzeterminowanePlatnosci obiekt)
        {
            return obiekt.WlasneNadchodzaceDoZaplaty.Wartosc == 0;
        }
        private bool tesWlasnePrzeterminowaneDoZaplaty(PrzeterminowanePlatnosci obiekt)
        {
            return obiekt.WlasnePrzeterminowaneDoZaplaty.Wartosc == 100;
        }
    }
}