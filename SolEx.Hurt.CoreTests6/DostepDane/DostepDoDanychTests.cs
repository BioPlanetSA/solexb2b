using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using ServiceStack.ServiceInterface.ServiceModel;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using Xunit;
using Assert = Xunit.Assert;

namespace SolEx.Hurt.Core.DostepDane.Tests
{
    public class DostepDoDanychTests
    {
        private string baza = ":memory:";
        private OrmLiteConnectionFactory polaczenie;

        public DostepDoDanychTests()
        {
            polaczenie = new OrmLiteConnectionFactory(baza, SqliteDialect.Provider) {DialectProvider = {UseUnicode = true},AutoDisposeConnection = false};
        }
     
        [Fact(DisplayName = "Test czy orm działa")]
        public void TestPobierania()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            ICacheBll cache = A.Fake<ICacheBll>();
            A.CallTo(() => solexBllCalosc.Cache).Returns(cache);
            A.CallTo(() => cache.PobierzObiekt<IList<Model.Klient>>(A<string>.Ignored)).Returns(null);
            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<Model.Klient>();
                Model.Klient nowy=new Model.Klient();
                for (int i = 0; i < 10; ++i)
                {
                    nowy.Id = i;
                    db.Insert(nowy);
                }
            }
      
            DostepDoDanych dostep =new DostepDoDanych(solexBllCalosc);
            dostep.BindSelect<Model.Klient>(SposobCachowania.Brak);
            dostep.DbFactory = polaczenie;
            var wynik = dostep.Pobierz<Model.Klient>(null);
            Assert.Equal(10,wynik.Count);
        }

        public class MockCache : ICacheBll
        {
            private Dictionary<string,object> cecheSztuczny=new Dictionary<string, object>(); 
            public void UsunObiekt(string klucz, params object[] parametryKlucza)
            {
                cecheSztuczny.Remove(klucz);
            }

            public void DodajObiekt(string klucz, object obiekt, params object[] parametryKlucza)
            {
                cecheSztuczny.Add(klucz,obiekt);
            }

            public T PobierzObiekt<T>(string klucz, params object[] parametryKlucza)
            {
                object wynik;
                cecheSztuczny.TryGetValue(klucz, out wynik);
                return (T) wynik;
            }

            public object PobierzObiekt(string klucz, params object[] parametryKlucza)
            {
                throw new NotImplementedException();
            }

            public void UsunGdzieKluczRozpoczynaSieOd(string poczatek)
            {
           
            }

            public bool ZawieraKluczRozpoczynaSieOd(string poczatek)
            {
                throw new NotImplementedException();
            }

            public void WyczyscCache()
            {
                cecheSztuczny.Clear();
            }

            public T PobierzChwilowy<T>(string klucz)
            {
                throw new NotImplementedException();
            }

            public void DodajChwilowy(string klucz, object dane)
            {
                throw new NotImplementedException();
            }

            public bool JestObiektChwilowy(string klucz)
            {
                throw new NotImplementedException();
            }

            public void DodajObiekt(int ileSekund, string klucz, object obiekt, params object[] parametryKlucza)
            {
                throw new NotImplementedException();
            }

            public T PobierzObiekt<T>(Func<T> metodaDoPobraniaWartosci, object lok, string klucz, params object[] parametryKlucza)
            {
                throw new NotImplementedException();
            }
        }

        [Fact(DisplayName = "Test czy cache działa")]
        public void TestCache()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            ICacheBll cache =new MockCache();
            A.CallTo(() => solexBllCalosc.Cache).Returns(cache);
       
            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<Model.Klient>();
                Model.Klient nowy = new Model.Klient();
                for (int i = 0; i < 10; ++i)
                {
                    nowy.Id = i;
                    db.Insert(nowy);
                }
            }

            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.BindSelect<Model.Klient>(SposobCachowania.CalaLista);
            dostep.DbFactory = polaczenie;
            var wynik = dostep.Pobierz<Model.Klient>(null);
            Assert.Equal(10, wynik.Count);
        }

        [Fact(DisplayName = "Test czy działa dodawanie warunków do sql")]
        public void TestPobieraniaWaruenSql()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            ICacheBll cache = A.Fake<ICacheBll>();
            A.CallTo(() => solexBllCalosc.Cache).Returns(cache);
            A.CallTo(() => cache.PobierzObiekt<IList<Model.Klient>>(A<string>.Ignored)).Returns(null);
            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<Model.Klient>();
                Model.Klient nowy = new Model.Klient();
                for (int i = 0; i < 10; ++i)
                {
                    nowy.Id = i;
                    db.Insert(nowy);
                }
            }

            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.DbFactory = polaczenie;
            dostep.BindSelect<Model.Klient>(SposobCachowania.Brak,null,null,null);
            var wynik = dostep.Pobierz<Model.Klient>(null,x=>x.Id>2);
            Assert.Equal(7, wynik.Count);
        }



        [Fact(DisplayName = "Test aktualizacji obiektu którego nie można pobierać")]
        public void TestAktualizacjiZabronionychObiektow()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            ICacheBll cache = A.Fake<ICacheBll>();
            A.CallTo(() => solexBllCalosc.Cache).Returns(cache);
            A.CallTo(() => cache.PobierzObiekt<IList<KategoriaProduktu>>(A<string>.Ignored)).Returns(null);
            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<KategoriaProduktu>();
                KategoriaProduktu nowy = new KategoriaProduktu();
                for (int i = 1; i < 10; ++i)
                {
                    nowy.Id = i;
                    nowy.Nazwa = "nazwa " + i;
                    db.Insert(nowy);
                }
            }
            string zmienionaNazwa = "Zmieniona nazwa dla id1";
            List<KategoriaProduktu> produktyDoAktualizacji = new List<KategoriaProduktu>() {new KategoriaProduktu() {Id=1, Nazwa = zmienionaNazwa } };

            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.DbFactory = polaczenie;
            dostep.BindSelect<KategoriaProduktu>(SposobCachowania.ZakazanePobieranie);
            dostep.AktualizujListe<KategoriaProduktu>(produktyDoAktualizacji);

            Assert.True(true);
        }






        [Fact(DisplayName = "Test czy działa filtrowanie w c#")]
        public void PobierzTest()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            ICacheBll cache = A.Fake<ICacheBll>();
            A.CallTo(() => solexBllCalosc.Cache).Returns(cache);
            A.CallTo(() => cache.PobierzObiekt<IList<Model.Klient>>(A<string>.Ignored)).Returns(null);
            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<Model.Klient>();
                Model.Klient nowy = new Model.Klient();
                for (int i = 0; i < 10; ++i)
                {
                    nowy.Id = i;
                    db.Insert(nowy);
                }
            }

            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.DbFactory = polaczenie;
            dostep.BindSelect<Model.Klient>(SposobCachowania.Brak, null, null, null);
            var wynik = dostep.Pobierz<Model.Klient>(null, x => x.Id > 2);
            Assert.Equal(7, wynik.Count);
        }

        [Fact(DisplayName = "Test czy działa filtrowanie w c# - pobranie jednego obiektu - wyraznie lambda")]
        public void PobierzTestJEden()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            ICacheBll cache = A.Fake<ICacheBll>();
            A.CallTo(() => solexBllCalosc.Cache).Returns(cache);
            A.CallTo(() => cache.PobierzObiekt<IList<Model.Klient>>(A<string>.Ignored)).Returns(null);
            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<Model.Klient>();
                Model.Klient nowy = new Model.Klient();
                for (int i = 0; i < 10; ++i)
                {
                    nowy.Id = i;
                    db.Insert(nowy);
                }
            }

            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.DbFactory = polaczenie;
            dostep.BindSelect<Model.Klient>(SposobCachowania.CalaLista, null, null, null);
            var wynik = dostep.PobierzPojednczy<Model.Klient>(x => x.Id > 2,null);
            Assert.Equal(3, wynik.Id);
        }


        [Fact(DisplayName = "Test czy działa filtrowanie w SQL - pobranie jednego obiektu")]
        public void PobierzPojedycznySQLTest()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<Model.Klient>();
                Model.Klient nowy = new Model.Klient();
                for (int i = 0; i < 10; ++i)
                {
                    nowy.Id = i;
                    db.Insert(nowy);
                }
            }

            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.DbFactory = polaczenie;
            dostep.BindSelect<Model.Klient>(SposobCachowania.Brak, null, null, null);
            var wynik = dostep.PobierzPojednczy<Model.Klient>(x => x.Id == 2, null);
            Assert.Equal(2, wynik.Id);
        }



        [Fact(DisplayName = "Test czy działa walidacja dostepu")]
        public void PobierzTestWalidacjaDostepu()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            ICacheBll cache = A.Fake<ICacheBll>();
            A.CallTo(() => solexBllCalosc.Cache).Returns(cache);
            A.CallTo(() => cache.PobierzObiekt<IList<Model.Klient>>(A<string>.Ignored)).Returns(null);

            IConfigBLL konfig = A.Fake<IConfigBLL>();
            A.CallTo(() => konfig.JezykIDDomyslny).Returns(3);

            A.CallTo(() => solexBllCalosc.Konfiguracja).Returns(konfig);

            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<Model.Klient>();
                Model.Klient nowy = new Model.Klient();
                for (int i = 0; i < 10; ++i)
                {
                    nowy.Id = i;
                    db.Insert(nowy);
                }
            }

            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.DbFactory = polaczenie;
            dostep.BindSelect<Model.Klient>(SposobCachowania.Brak, (x,k) => x.Id > 3, null, null);

            IKlient klientZadajacy = A.Fake<IKlient>();
            klientZadajacy.Id = 2345;

            var wynik = dostep.Pobierz<Model.Klient>(klientZadajacy);
            Assert.Equal(6, wynik.Count);
        }

        [Fact(DisplayName = "Test czy działa filtrowanie w c# - pobranie jednego obiektu - wg id")]
        public void PobierzTestJEdenWgId()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            ICacheBll cache = A.Fake<ICacheBll>();
            A.CallTo(() => solexBllCalosc.Cache).Returns(cache);
            A.CallTo(() => cache.PobierzObiekt<IList<Model.Klient>>(A<string>.Ignored)).Returns(null);
            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<Model.Klient>();
                Model.Klient nowy = new Model.Klient();
                for (int i = 0; i < 10; ++i)
                {
                    nowy.Id = i;
                    db.Insert(nowy);
                }
            }

            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.DbFactory = polaczenie;
            dostep.BindSelect<Model.Klient>(SposobCachowania.Brak, null, null, null);
            var wynik = dostep.PobierzPojednczy<Model.Klient>(3, null);
            Assert.Equal(3, wynik.Id);
        }
        [Fact(DisplayName = "Test czy działa sortowanie w c#")]
        public void PobierzTestSortowanie()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            ICacheBll cache = A.Fake<ICacheBll>();
            A.CallTo(() => solexBllCalosc.Cache).Returns(cache);
            A.CallTo(() => cache.PobierzObiekt<IList<Model.Klient>>(A<string>.Ignored)).Returns(null);
            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<Model.Klient>();
                Model.Klient nowy = new Model.Klient();
                for (int i = 0; i < 10; ++i)
                {
                    nowy.Id = i;
                    db.Insert(nowy);
                }
            }

            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.DbFactory = polaczenie;
            dostep.BindSelect<Model.Klient>(SposobCachowania.Brak, null, null, null);
            var wynik = dostep.Pobierz<Model.Klient>(null, null,new List<SortowanieKryteria<Model.Klient>>{new SortowanieKryteria<Model.Klient>(x=>x.Id,KolejnoscSortowania.desc, "Id")});
            Assert.Equal(9, wynik[0].Id);
        }
        [Fact(DisplayName = "Test czy działa sortowanie kilkukrotne w c#")]
        public void PobierzTestSortowanieKilkukrotne()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            ICacheBll cache = A.Fake<ICacheBll>();
            A.CallTo(() => solexBllCalosc.Cache).Returns(cache);
            A.CallTo(() => cache.PobierzObiekt<IList<Model.Klient>>(A<string>.Ignored)).Returns(null);
            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<Model.Klient>();
                Model.Klient nowy = new Model.Klient();
                for (int i = 0; i < 10; ++i)
                {
                    nowy.Id = i;
                    nowy.Nazwa = i.ToString();
                    nowy.Symbol = (i%2).ToString();
                    db.Insert(nowy);
                }
            }

            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.DbFactory = polaczenie;
            dostep.BindSelect<Model.Klient>(SposobCachowania.Brak, null, null, null);
            var wynik = dostep.Pobierz<Model.Klient>(null, null, new List<SortowanieKryteria<Model.Klient>> { new SortowanieKryteria<Model.Klient>(x => x.Symbol, KolejnoscSortowania.desc, "Symbol"),new SortowanieKryteria<Model.Klient>(x=>x.Nazwa,KolejnoscSortowania.desc, "Nazwa") });
            Assert.Equal("1", wynik[0].Symbol);
            Assert.Equal("9", wynik[0].Nazwa);

            wynik = dostep.Pobierz<Model.Klient>(null, null, new List<SortowanieKryteria<Model.Klient>> { new SortowanieKryteria<Model.Klient>(x => x.Symbol, KolejnoscSortowania.desc, "Symbol"), new SortowanieKryteria<Model.Klient>(x => x.Nazwa, KolejnoscSortowania.asc, "Nazwa") });
            Assert.Equal("1", wynik[0].Symbol);
            Assert.Equal("1", wynik[0].Nazwa);
        }
        [Fact(DisplayName = "Test czy działa stronicowanie w c#")]
        public void PobierzTestStronicowanie()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            ICacheBll cache = A.Fake<ICacheBll>();
            A.CallTo(() => solexBllCalosc.Cache).Returns(cache);
            A.CallTo(() => cache.PobierzObiekt<IList<Model.Klient>>(A<string>.Ignored)).Returns(null);
            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<Model.Klient>();
                Model.Klient nowy = new Model.Klient();
                for (int i = 0; i < 10; ++i)
                {
                    nowy.Id = i;
                    db.Insert(nowy);
                }
            }

            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.DbFactory = polaczenie;
            dostep.BindSelect<Model.Klient>(SposobCachowania.Brak, null, null, null);
            var wynik = dostep.Pobierz<Model.Klient>(null, null,null,2,3);
            Assert.Equal(3, wynik.Count);
            Assert.Equal(3, wynik[0].Id);
        }

        [Fact(DisplayName = "Test podpięcia własnej metodyu do tworzenia obiektów")]
        public void TestWlasnaMetodaTworzacaDane()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            ICacheBll cache = A.Fake<ICacheBll>();
            A.CallTo(() => solexBllCalosc.Cache).Returns(cache);
            A.CallTo(() => cache.PobierzObiekt<IList<Model.Klient>>(A<string>.Ignored)).Returns(null);
            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<Model.Klient>();
                Model.Klient nowy = new Model.Klient();
                for (int i = 0; i < 10; ++i)
                {
                    nowy.Id = i;
                    db.Insert(nowy);
                }
            }

            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.DbFactory = polaczenie;
            dostep.BindSelect<Model.Klient>(SposobCachowania.Brak, null, StworzKlientow, null);
            var wynik = dostep.Pobierz<Model.Klient>(null, x => x.Id <10);
            Assert.Equal(1, wynik.Count);
        }

        IList<Model.Klient> StworzKlientow(int jezyk,IKlient zadajacy)
        {
            List<Model.Klient> wynik=new List<Model.Klient>();
           
            for (int i = 0; i < 10; ++i)
            {
                Model.Klient nowy = new Model.Klient();
                nowy.Id = i*10;
                wynik.Add(nowy);
            }
            return wynik;

        }

        [Fact(DisplayName = "Test czy działa usuwanie")]
        public void PobierzTestUsuwania()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            ICacheBll cache = A.Fake<ICacheBll>();
            A.CallTo(() => solexBllCalosc.Cache).Returns(cache);
            A.CallTo(() => cache.PobierzObiekt<IList<Model.Klient>>(A<string>.Ignored)).Returns(null);
            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<Model.Klient>();
                Model.Klient nowy = new Model.Klient();
                for (int i = 0; i < 10; ++i)
                {
                    nowy.Id = i;
                    db.Insert(nowy);
                }
            }

            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.DbFactory = polaczenie;
            dostep.BindSelect<Model.Klient>(SposobCachowania.Brak, null, null, null);
            dostep.UsunPojedynczy<Model.Klient>(7);

            using (var db = polaczenie.OpenDbConnection())
            {
                var wynik = db.Select<Model.Klient>();
                Assert.Equal(9, wynik.Count);
                Assert.True(wynik.All(x=>x.Id!=7));
            }
        }

        [Fact(DisplayName = "Test czy działa usuwanie kilku elementów")]
        public void PobierzTestUsuwaniaKilku()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            ICacheBll cache = A.Fake<ICacheBll>();
            A.CallTo(() => solexBllCalosc.Cache).Returns(cache);
            A.CallTo(() => cache.PobierzObiekt<IList<Model.Klient>>(A<string>.Ignored)).Returns(null);
            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<Model.Klient>();
                Model.Klient nowy = new Model.Klient();
                for (int i = 0; i < 10; ++i)
                {
                    nowy.Id = i;
                    db.Insert(nowy);
                }
            }

            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.DbFactory = polaczenie;
            dostep.BindSelect<Model.Klient>(SposobCachowania.Brak, null, null, null);
            dostep.Usun<Model.Klient, object>(new List<object>{1,2,3});

            using (var db = polaczenie.OpenDbConnection())
            {
                var wynik = db.Select<Model.Klient>();
                Assert.Equal(7, wynik.Count);
            
            }


        }

        [Fact(DisplayName = "Test czy działa aktulizowanie")]
        public void PobierzTestAktualizowanie()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            ICacheBll cache = A.Fake<ICacheBll>();
            A.CallTo(() => solexBllCalosc.Cache).Returns(cache);
            A.CallTo(() => cache.PobierzObiekt<IList<Model.Klient>>(A<string>.Ignored)).Returns(null);
            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<Model.Klient>();

                Model.Klient nowy = new Model.Klient();
                for (int i = 0; i < 10; ++i)
                {
                    nowy.Id = i;
                    db.Insert(nowy);
                }
            }

            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.DbFactory = polaczenie;
            dostep.BindSelect<Model.Klient>(SposobCachowania.Brak, null, null, null);
            Model.Klient tmp=new Model.Klient(1){Nazwa = "abc"};
            object klucz=   dostep.AktualizujPojedynczy(tmp);
            Assert.Equal((long)1,klucz);
            using (var db = polaczenie.OpenDbConnection())
            {
                Model.Klient wynik = db.FirstOrDefault<Model.Klient>(x=>x.Id==1);
                Assert.Equal(tmp.Nazwa, wynik.Nazwa);
            }
        }
      
        [Fact(DisplayName = "Test czy działa aktulizowanie zbiorcze")]
        public void PobierzTestAktualizowanieKilku()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            ICacheBll cache = A.Fake<ICacheBll>();
            A.CallTo(() => solexBllCalosc.Cache).Returns(cache);
            A.CallTo(() => cache.PobierzObiekt<IList<Model.Klient>>(A<string>.Ignored)).Returns(null);
            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<Model.Klient>();
                
                Model.Klient nowy = new Model.Klient();
                for (int i = 0; i < 10; ++i)
                {
                    nowy.Id = i;
                    db.Insert(nowy);
                }
            }

            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.DbFactory = polaczenie;
            dostep.BindSelect<Model.Klient>(SposobCachowania.Brak, null, null, null);
            var tmp = new Model.Klient(1) {Nazwa = "abc"};
            var tmp2 = new Model.Klient(2) { Nazwa = "xyz" };
            List<Model.Klient> lista = new List<Model.Klient> { tmp, tmp2 };
            dostep.AktualizujListe<Model.Klient>(lista);

            using (var db = polaczenie.OpenDbConnection())
            {
                var wynik = db.Where<Model.Klient>(x => x.Id == 1 || x.Id==2);
                Assert.True(wynik.Any(x=>x.Nazwa==tmp.Nazwa));
                Assert.True(wynik.Any(x => x.Nazwa == tmp2.Nazwa));
            }
        }

        [Fact(DisplayName = "Wydajność aktualizowania obiektów bez wymaganego sprawdzawnia ID - 1 miliony rekordów FlatCen." +
                            " Obiekt Flat cen ma ID wyliczane więc dodawanie do bazy powinno być błyskawiczne i bez sprawdzania niepotrzebnych rzeczy")]
        public void TestAktualizowanieWydajnoscObiektowNieWymagajacychSprawdzaniaID()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            ICacheBll cache = A.Fake<ICacheBll>();
            A.CallTo(() => solexBllCalosc.Cache).Returns(cache);
            A.CallTo(() => cache.PobierzObiekt<IList<Model.Klient>>(A<string>.Ignored)).Returns(null);

            List<FlatCeny> tablicaCen = new List<FlatCeny>(1200000);

            for (int i = 0; i < 1200000; ++i)
            {
                FlatCeny flat = new FlatCeny();
                flat.ProduktId = i;
                flat.CenaNetto = 43;
                flat.KlientId = i;
                tablicaCen.Add(flat);
            }

            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<Model.FlatCeny>();
            }

            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.DbFactory = polaczenie;
            dostep.BindSelect<Model.FlatCeny>(SposobCachowania.Brak, null, null, null);
            var tmp2 = new Model.Klient(2) { Nazwa = "xyz" };

            Stopwatch sw = new Stopwatch();
            sw.Start();
            dostep.AktualizujListe<Model.FlatCeny>(tablicaCen);
            sw.Stop();

            Assert.True(sw.Elapsed.Seconds < 60);
        }


        [Fact(DisplayName = "Test czy działa bind przed aktualizację")]
        public void PobierzTestAktualizowanieZdarzeniePrzed()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            ICacheBll cache = A.Fake<ICacheBll>();
            A.CallTo(() => solexBllCalosc.Cache).Returns(cache);
            A.CallTo(() => cache.PobierzObiekt<IList<Model.Klient>>(A<string>.Ignored)).Returns(null);
            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<Model.Klient>();
           
            }

            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.DbFactory = polaczenie;
            dostep.BindSelect<Model.Klient>(SposobCachowania.Brak, null, null, null);
            dostep.BindPrzedUpdate<Model.Klient>(PrzedUpdate);
            Model.Klient tmp = new Model.Klient(1) { Nazwa = "abc" };
            Model.Klient tmp2 = new Model.Klient(2) { Nazwa = "xyz" };
            List<Model.Klient> lista = new List<Model.Klient> { tmp, tmp2 };
            dostep.AktualizujListe<Model.Klient>(lista);

            using (var db = polaczenie.OpenDbConnection())
            {
                var wynik = db.Select<Model.Klient>();
             Assert.Equal(1,wynik.Count);
            }

        }

        private void PrzedUpdate(IList<Model.Klient> obj)
        {
            for (int i = 0; i < obj.Count; i++)
            {
                if (obj[i].Id == 1)
                {
                    obj.RemoveAt(i);
                    i--;
                }
            }
        }
        [Fact(DisplayName = "Test czy działa bind przed aktualizacja - info rozne obiekty")]
        public void PobierzTestAktualizowanieZdarzeniePrzedRozne()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            ICacheBll cache = A.Fake<ICacheBll>();
            A.CallTo(() => solexBllCalosc.Cache).Returns(cache);
            A.CallTo(() => cache.PobierzObiekt<IList<Model.Klient>>(A<string>.Ignored)).Returns(null);
            var tmp = new Model.Klient(1) { Nazwa = "abc" };
            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<Model.Klient>();
                
                db.Insert(tmp);

            }

            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.DbFactory = polaczenie;
            dostep.BindSelect<Model.Klient>(SposobCachowania.Brak, null, null, null);
            dostep.BindPrzedUpdateRozrodnieNoweAktualizowane<Model.Klient>(PrzedUpdateRozne);

            var tmp2 = new Model.Klient(2) { Nazwa = "xyz" };
            var lista = new List<Model.Klient> { tmp, tmp2 };
            dostep.AktualizujListe<Model.Klient>(lista);
        }

        private void PrzedUpdateRozne(IList<Model.Klient> nowe, IList<Model.Klient> aktualizowne)
        {
            Assert.Equal(1, nowe.Count);
            Assert.Equal(2,nowe[0].Id);
            Assert.Equal(1, aktualizowne.Count);
            Assert.Equal(1, aktualizowne[0].Id);
        }
        [Fact(DisplayName = "Test czy działa bind po aktualizacji - info rozne obiekty")]
        public void PobierzTestAktualizowanieZdarzeniePoRozne()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            ICacheBll cache = A.Fake<ICacheBll>();
            A.CallTo(() => solexBllCalosc.Cache).Returns(cache);
            A.CallTo(() => cache.PobierzObiekt<IList<Model.Klient>>(A<string>.Ignored)).Returns(null);
            var tmp = new Model.Klient(1) { Nazwa = "abc" };
            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<Model.Klient>();
                
                db.Insert(tmp);

            }

            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.DbFactory = polaczenie;
            dostep.BindSelect<Model.Klient>(SposobCachowania.Brak, null, null, null);
            dostep.BindPoUpdateRozrodnieNoweAktualizowane<Model.Klient>(PoUpdateRozne);

            var tmp2 = new Model.Klient(2) { Nazwa = "xyz" };
            var lista = new List<Model.Klient> { tmp, tmp2 };
            dostep.AktualizujListe<Model.Klient>(lista);
        }

        private void PoUpdateRozne(IList<Model.Klient> nowe, IList<Model.Klient> aktualizowane)
        {
            Assert.Equal(1, nowe.Count);
            Assert.Equal(2, nowe[0].Id);
            Assert.Equal(1, aktualizowane.Count);
            Assert.Equal(1, aktualizowane[0].Id);
        }

        [Fact(DisplayName = "Test czy działa bind po aktualizacji")]
        public void PobierzTestAktualizowanieZdarzeniePo()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            ICacheBll cache = A.Fake<ICacheBll>();
            A.CallTo(() => solexBllCalosc.Cache).Returns(cache);
            A.CallTo(() => cache.PobierzObiekt<IList<Model.Klient>>(A<string>.Ignored)).Returns(null);
            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<Model.Klient>();
                
            }

            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.DbFactory = polaczenie;
            dostep.BindSelect<Model.Klient>(SposobCachowania.Brak, null, null, null);
            dostep.BindPoUpdate<Model.Klient>(PoUpdate);
            var tmp = new Model.Klient(1) { Nazwa = "abc" };
            var tmp2 = new Model.Klient(2) { Nazwa = "xyz" };
            var lista = new List<Model.Klient> { tmp, tmp2 };
            dostep.AktualizujListe<Model.Klient>(lista);
        }

        private void PoUpdate(IList<object> obj)
        {
            Assert.Equal(2, obj.Count);
            Assert.True(1 == (long)obj[0]);
            Assert.True(2 == (long)obj[1]);
        }

        [Fact(DisplayName = "Test czy działa aktualizacji gdy wyślemy pustą kolekcje")]
        public void PobierzTestAktualizowaniePustaKolecja()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            ICacheBll cache = A.Fake<ICacheBll>();
            A.CallTo(() => solexBllCalosc.Cache).Returns(cache);
            A.CallTo(() => cache.PobierzObiekt<IList<Model.Klient>>(A<string>.Ignored)).Returns(null);
            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<Model.Klient>();
                
            }

            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.DbFactory = polaczenie;
            dostep.BindSelect<Model.Klient>(SposobCachowania.Brak, null, null, null);
            dostep.BindPoUpdate<Model.Klient>(PoUpdate);

            List<Model.Klient> lista = new List<Model.Klient>();
            dostep.AktualizujListe<Model.Klient>(lista);

        }

        [Fact(DisplayName = "Test czy wywala gdy próbujemy zbindować drugi raz select dla tego samego typu")]
        public void PobierzTestPodwojneBindowanie()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.DbFactory = polaczenie;


            dostep.BindSelect<Model.Klient>(SposobCachowania.Brak, null, null, null);
            Assert.Throws<InvalidOperationException>(()=>  dostep.BindSelect<Model.Klient>(SposobCachowania.Brak, null, null, null));
        }

        [Fact(DisplayName = "Test czy działa usuwanie gdy wyślemy pustą kolekcje")]
        public void PobierzTestUsuwaniePustaKolecja()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            ICacheBll cache = A.Fake<ICacheBll>();
            A.CallTo(() => solexBllCalosc.Cache).Returns(cache);
            A.CallTo(() => cache.PobierzObiekt<IList<Model.Klient>>(A<string>.Ignored)).Returns(null);
            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<Model.Klient>();
                
                for (int i = 0; i < 10; ++i)
                {
                    Model.Klient nowy=new Model.Klient();
                    nowy.Id = i;
                    db.Insert(nowy);
                }
            }

            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.DbFactory = polaczenie;

          
            dostep.Usun<Model.Klient, object>(new List<object>());
            using (var db = polaczenie.OpenDbConnection())
            {
              var wynik=  db.Select<Model.Klient>();
              Assert.Equal(10,wynik.Count);
            }

        }
        [Fact(DisplayName = "Test czy działa usuwanie - generyczne wywolanie dla pojedynczego obiektu - zdarzenie Przed")]
        public void PobierzTestUsuwanieZdarzeniePrzed()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            ICacheBll cache = A.Fake<ICacheBll>();
            A.CallTo(() => solexBllCalosc.Cache).Returns(cache);
            A.CallTo(() => cache.PobierzObiekt<IList<Model.Klient>>(A<string>.Ignored)).Returns(null);
            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<Model.Klient>();
                
                for (int i = 0; i < 10; ++i)
                {
                    Model.Klient nowy = new Model.Klient();
                    nowy.Id = i;
                    db.Insert(nowy);
                }
            }

            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.DbFactory = polaczenie;
            dostep.BindPrzedUsunieciem<Model.Klient, long>(PrzedUsunieciem);

            dostep.UsunPojedynczy<Model.Klient>((long)1);
        }

        private void PrzedUsunieciem(IList<long> obj)
        {
           Assert.Equal(1,obj.Count);
           Assert.Equal(1,obj[0]);
        }

        [Fact(DisplayName = "Test czy działa usuwanie  - zdarzenie po")]
        public void PobierzTestUsuwanieZdarzeniePo()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            ICacheBll cache = A.Fake<ICacheBll>();
            A.CallTo(() => solexBllCalosc.Cache).Returns(cache);
            A.CallTo(() => cache.PobierzObiekt<IList<Model.Klient>>(A<string>.Ignored)).Returns(null);
            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<Model.Klient>();
                
                for (int i = 0; i < 10; ++i)
                {
                    Model.Klient nowy = new Model.Klient();
                    nowy.Id = i;
                    db.Insert(nowy);
                }
            }

            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.DbFactory = polaczenie;
            dostep.BindPoUsunieciu<Model.Klient, long>(PoUsunieciu);

            dostep.UsunPojedynczy<Model.Klient>((long)1);
        }

        private void PoUsunieciu(IList<long> obj)
        {
            Assert.Equal(1, obj.Count);
            Assert.Equal(1, obj[0]);
        }

        [Fact(DisplayName = "Test pobierania - metody nie generycznej")]
        public void PobierzTest1()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            ICacheBll cache = A.Fake<ICacheBll>();
            A.CallTo(() => solexBllCalosc.Cache).Returns(cache);
            A.CallTo(() => cache.PobierzObiekt<IList<Model.Klient>>(A<string>.Ignored)).Returns(null);
            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<Model.Klient>();
                Model.Klient nowy = new Model.Klient();
                for (int i = 0; i < 10; ++i)
                {
                    nowy.Id = i;
                    db.Insert(nowy);
                }
            }

            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.BindSelect<Model.Klient>(SposobCachowania.Brak);
            dostep.DbFactory = polaczenie;
            long lacznie;
            var wynik = dostep.PobierzWgTypu(typeof(Klient), null, null, null, KolejnoscSortowania.asc, 1, int.MaxValue,0,out lacznie);

          Assert.Equal(10, wynik.Count);
          Assert.Equal(10, lacznie);
          wynik = dostep.PobierzWgTypu(typeof(Klient), null, null, null, KolejnoscSortowania.asc, 1, 3,0, out lacznie);

          Assert.Equal(3, wynik.Count);
          Assert.Equal(10, lacznie);
        }

        [Fact(DisplayName = "Test czy działa aktualizowanie wersja nie generyczna")]
        public void AktualizujWgTypuTest()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            ICacheBll cache = A.Fake<ICacheBll>();
            A.CallTo(() => solexBllCalosc.Cache).Returns(cache);
            A.CallTo(() => cache.PobierzObiekt<IList<SolEx.Hurt.Model.Klient>>(A<string>.Ignored)).Returns(null);
            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<Model.Klient>();

                Model.Klient nowy = new Model.Klient();
                for (int i = 1; i < 10; ++i)
                {
                    nowy.Id = i;
                    nowy.PelnaOferta = true;
                    db.Insert(nowy);
                }
            }

            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.DbFactory = polaczenie;
            dostep.BindSelect<Model.Klient>(SposobCachowania.Brak, null, null, null);
            object tmp = new Model.Klient(1) { Nazwa = "abc", PelnaOferta =  false};
            dostep.AktualizujWgTypu(tmp);

            using (var db = polaczenie.OpenDbConnection())
            {
                Model.Klient wynik = db.FirstOrDefault<Model.Klient>(x => x.Id == 1);
                Assert.Equal("abc", wynik.Nazwa);
            }
        }

        [Fact(DisplayName = "Test usuwania wersja niegeneryczna")]
        public void UsunWyTypuTest()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            ICacheBll cache = A.Fake<ICacheBll>();
            A.CallTo(() => solexBllCalosc.Cache).Returns(cache);
            A.CallTo(() => cache.PobierzObiekt<IList<Model.Klient>>(A<string>.Ignored)).Returns(null);
            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<Model.Klient>();

                Model.Klient nowy = new Model.Klient();
                for (int i = 0; i < 10; ++i)
                {
                    nowy.Id = i;
                    db.Insert(nowy);
                }
            }

            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.DbFactory = polaczenie;
            dostep.BindSelect<Model.Klient>(SposobCachowania.Brak, null, null, null); 
            dostep.UsunWyTypu(typeof(Klient),"1");
            using (var db = polaczenie.OpenDbConnection())
            {
                 Assert.Equal(9,db.Select<Model.Klient>().Count);
            }
        }

        [Fact(DisplayName = "Test dodawania walidatora w pobieraniu SQL")]
        public void WalidatorPobieranieSQLTest()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            IKlient klient = A.Fake<Klient>();
            klient.Id = 1;

            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.DbFactory = polaczenie;
            dostep.BindSelect<Model.Produkt>(SposobCachowania.Brak, (x,k)=> x.Id < 5 && x.Widoczny && 
                        Sql.InSubquery(x.Id, new SqlServerExpressionVisitor<Model.ProduktCecha>().Select(z=> z.ProduktId).Where(z=>z.CechaId==2)),null, null);

            List<Model.Produkt> produkty = new List<Produkt>(10);

            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<Model.Produkt>();
                db.CreateTable<Model.ProduktCecha>();

                for (int i = 0; i < 10; ++i)
                {
                    Model.Produkt nowy = new Model.Produkt();
                    nowy.CenaWPunktach = 10;
                    nowy.Id = i;
                    nowy.Widoczny = (i % 2) == 0;
                    produkty.Add(nowy);
                    db.Insert(nowy);

                    for (int ii = 0; ii < 3; ++ii)
                    {
                        Model.ProduktCecha cp = new ProduktCecha();
                        cp.ProduktId = i;
                        cp.CechaId = ii;
                        db.Insert(cp);
                    }
                }
            }

            int spodziewnaLiczba = produkty.Count(x => x.Widoczny && x.Id < 5 && x.CenaWPunktach == 10);

            List<Model.Produkt> wynik = new List<Model.Produkt>();
            using (var db = polaczenie.OpenDbConnection())
            {
                wynik = dostep.Pobierz<Model.Produkt>(klient, x => x.CenaWPunktach == 10).ToList();
            }

            Assert.Equal(wynik.Count, spodziewnaLiczba);
        }


        [Fact(DisplayName = "Test działania Sql.In  gdy lista jest pusta i null")]
        public void FiltrSqlInPustaLista()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();
            ICacheBll cache = A.Fake<ICacheBll>();
            A.CallTo(() => solexBllCalosc.Cache).Returns(cache);
            A.CallTo(() => cache.PobierzObiekt<IList<Model.Klient>>(A<string>.Ignored)).Returns(null);
            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<Model.Klient>();

                Model.Klient nowy = new Model.Klient();
                for (int i = 0; i < 10; ++i)
                {
                    nowy.Id = i;
                    db.Insert(nowy);
                }
            }

            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.DbFactory = polaczenie;
            dostep.BindSelect<Model.Klient>(SposobCachowania.Brak, null, null, null);

            IList<Model.Klient> wynik = null;

            using (var db = polaczenie.OpenDbConnection())
            {
                List<int> kolekcjaIntow = null;
                wynik = dostep.Pobierz<Model.Klient>(null, x => Sql.In(x.Id, kolekcjaIntow));
            }
            Assert.Equal(10, wynik.Count);

            wynik = null;
            using (var db = polaczenie.OpenDbConnection())
            {
                List<int> kolekcjaIntow = new List<int>();
                wynik = dostep.Pobierz<Model.Klient>(null, x => Sql.In(x.Id, kolekcjaIntow));
            }
            Assert.Equal(10, wynik.Count);
        }


        [Fact(DisplayName = "Test dzielenia filtra SQL InSubquery")]
        public void PrzygotujFiltrySQLTest()
        {
            ISolexBllCalosc solexBllCalosc = A.Fake<ISolexBllCalosc>();

            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<Model.Produkt>();
                db.CreateTable<Model.ProduktCecha>();

                Model.Produkt nowy = new Model.Produkt();
                for (int i = 0; i < 10; ++i)
                {
                    nowy.Id = i;
                    db.Insert(nowy);
                }

                db.CreateTable<Model.ProduktCecha>();

                Model.ProduktCecha cp = new Model.ProduktCecha();
                for (int i = 0; i < 10; ++i)
                {
                    for (int ii = 0; ii < 3; ++ii)
                    {
                        //do kazdego produktu po 3 cechy
                        cp.CechaId = ii * 10;
                        cp.ProduktId = i;
                        db.Insert(cp);
                    }
                }
            }

            //test z jednym warunkiem
            Expression<Func<Model.Produkt, bool>> exp = x => Sql.InSubquery(x.Id, new SqlServerExpressionVisitor<Model.ProduktCecha>().Select(z=>z.ProduktId).Where(z=>z.CechaId == 20) ) ;
            List<Model.Produkt> wynik = new List<Model.Produkt>();
            using (var db = polaczenie.OpenDbConnection())
            {
              wynik =  db.Select<Model.Produkt>(exp);
            }

            Assert.Equal(wynik.Count, 10); //10 produktów z cechą 20

            //test z dwoami warunkami
            exp = x => x.Id < 5 &&  Sql.InSubquery(x.Id, new SqlServerExpressionVisitor<Model.ProduktCecha>().Select(z => z.ProduktId).Where(z => z.CechaId == 20));
            wynik = new List<Model.Produkt>();
            using (var db = polaczenie.OpenDbConnection())
            {
                wynik = db.Select<Model.Produkt>(exp);
            }

            Assert.Equal(wynik.Count, 5); //10 produktów z cechą 20


            DostepDoDanych dostep = new DostepDoDanych(solexBllCalosc);
            dostep.DbFactory = polaczenie;
            dostep.BindSelect<Model.Produkt>(SposobCachowania.Brak, null, null, null);

            //czy taki sam wynik da nasz pobierz i ORM
            List<Model.Produkt> lista2 = dostep.Pobierz<Model.Produkt>(null, exp).ToList();

            Assert.Equal(lista2.Count, wynik.Count);
        }


        
    }
}
      