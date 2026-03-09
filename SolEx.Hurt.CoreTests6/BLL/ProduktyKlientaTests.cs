using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using ServiceStack.Common;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces;
using Xunit;

namespace SolEx.Hurt.CoreTests.BLL
{

    public class ProduktyKlientaTest
    {

        private string baza = ":memory:";
        private OrmLiteConnectionFactory polaczenie;

        public ProduktyKlientaTest()
        {
            polaczenie = new OrmLiteConnectionFactory(baza, SqliteDialect.Provider) { DialectProvider = { UseUnicode = true }, AutoDisposeConnection = false };
        }


        [Fact(DisplayName="Testowanie czy ilość minimalna na produkcie klienta będize poprawnie ustawiana. ")]
        public void BindowanieTest()
        {
            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            ICacheBll cache = A.Fake<ICacheBll>();
            DostepDoDanych dostep = new DostepDoDanych(calosc);
            IProduktyKlienta produktyKlienta =new ProduktyKlienta(calosc);
            IProduktyBazowe produktyBazowe = new ProduktyBazowe(calosc);
            A.CallTo(() => calosc.ProduktyKlienta).Returns(produktyKlienta);
            A.CallTo(() => calosc.ProduktyBazowe).Returns(produktyBazowe);
            A.CallTo(() => calosc.Cache).Returns(cache);
            A.CallTo(() => cache.PobierzObiekt<IList<ProduktKlienta>>(A<string>.Ignored)).Returns(null);
            A.CallTo(() => cache.PobierzObiekt<IList<ProduktBazowy>>(A<string>.Ignored)).Returns(new List<ProduktBazowy>());

            dostep.DbFactory = polaczenie;
            dostep.BindSelect<ProduktKlienta>(SposobCachowania.Brak, null, produktyKlienta.Bindowanie);
            dostep.BindSelect<ProduktBazowy>(SposobCachowania.CalaLista, null, null, typeof(ProduktBazowy), produktyBazowe.MetodaPrzetwarzajacaPoSelect_UzupelnijProdutkBazowy);

            IConfigBLL config = A.Fake<IConfigBLL>();
            A.CallTo(() => calosc.Konfiguracja).Returns(config);
            A.CallTo(() => config.DeaktywujMinimumLogistyczneDlaWybranychKategoriiKlientow).Returns(new HashSet<int>() { 10, 11, 12 });
            A.CallTo(() => config.JezykIDDomyslny).Returns(1);

            var fake = new TworzenieFakeObiektow();
            IKlient klient = fake.FakeIKlient();

            List<Produkt> produkty = new List<Produkt>(10);
            using (var db = polaczenie.OpenDbConnection())
            {
                db.CreateTable<KategorieBLL>();
                db.Insert(new KategorieBLL() {Id = 1, Nazwa = "Test1"});

                db.CreateTable<Produkt>();
                db.CreateTable<ProduktKategoria>();
                for (int i = 0; i < 10; ++i)
                {
                    Produkt nowy = new Produkt();
                    nowy.CenaWPunktach = 10;
                    nowy.Id = i;
                    nowy.Widoczny = (i % 2) == 0;
                    nowy.IloscMinimalna = i*2;
                    produkty.Add(nowy);
                    db.Insert(nowy);
                    db.Insert(new ProduktKategoria(i,1));
                }
            }
            A.CallTo(() => cache.PobierzObiekt<Tuple<HashSet<long>, IList<ProduktKlienta>>>(A<string>.Ignored)).Returns(new Tuple<HashSet<long>, IList<ProduktKlienta>>(produkty.Select(x => x.Id).ToHashSet(), null));
            List<ProduktKlienta> wynik =  dostep.Pobierz<ProduktKlienta>(klient).ToList();
            
            Assert.True(wynik.Any(), "czy cokolwiek pobrało");
            foreach (ProduktKlienta produktKlienta in wynik)
            {
                Assert.True(produktKlienta.IloscMinimalna == 0, "Ilość minimalna dla produktów tego klienta powinna wynosić 0");
            }

        }

    }

}