using System;
using System.Collections.Generic;
using System.Data;
using FakeItEasy;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using Xunit;

namespace SolEx.Hurt.Core.BLL.Tests
{
    public class AdresyBLLTests
    {
        [Fact(DisplayName = "Sprawdzenie czy Adres o określonym id zostal uzyty")]
        public void CzyMoznaUzycTest()
        {
            //int id = 1;

            //IKlient k1 = new Klient(null) {Id=1, adres_wysylki_id = 1};
            //IKlient k2 = new Klient(null) { Id = 2, adres_wysylki_id = 5 };
            //IKlient k3 = new Klient(null) { Id = 3 };
            //IKlient k4 = new Klient(null) { Id = 4, adres_wysylki_id = 1 };
            //List<IKlient> listaKlientowTrue = new List<IKlient>(){k1,k2,k3,k4};
            //List<IKlient> listaKlientowFalse = new List<IKlient>(){k2,k3};

            //var klienciDostep = A.Fake<IKlienciDostep>();
            //A.CallTo(() => klienciDostep.PobierzWszystkich(null)).Returns(listaKlientowTrue);

            //AdresyBLL a1 = new AdresyBLL(){KlienciDostep = klienciDostep};

            //bool wynik = a1.CzyUzyty(id);
            //Assert.True(wynik);

            //A.CallTo(() => klienciDostep.PobierzWszystkich(null)).Returns(listaKlientowFalse);
            //wynik = a1.CzyUzyty(id);
            //Assert.False(wynik);
            throw new NotImplementedException();
        }
        private OrmLiteConnectionFactory polaczenie;

        private string baza = ":memory:";
        public AdresyBLLTests()
        {
            polaczenie = new OrmLiteConnectionFactory(baza, SqliteDialect.Provider) { DialectProvider = { UseUnicode = true }, AutoDisposeConnection = false };
        }

        [Fact(DisplayName = "Sprawdzenie uzupelnianie po select")]
        public void UzupelnijPozycjePoSelectTest()
        {

            IKlient klient = new Klient(null) { Id = 1 };
            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            DostepDoDanych dostep = new DostepDoDanych(calosc);
            A.CallTo(() => calosc.DostepDane).Returns(dostep);

            IAdresyDostep adresy = new AdresyBLL(calosc);

            Adres a1 = A.Fake<Adres>();
            a1.Id = 1;
            a1.RegionId = 1;
            a1.KrajId = 1;

            Adres a2 = A.Fake<Adres>();
            a2.Id = 0;
            a2.RegionId = 1;
            a2.KrajId = 1;

            Adres a3 = A.Fake<Adres>();
            a3.Id = 3;
            a3.RegionId = 1;
            a3.KrajId = 1;

            KlientAdres ka = new KlientAdres() {AdresId = 3, KlientId = 1, TypAdresu = TypAdresu.Glowny};


            Kraje kr = new Kraje(1,"Polska","pl");
            IList<Kraje> listaKrajow = new List<Kraje>() {kr};

            Region reg = new Region(1, "Mazowieckie",1);
            IList<Region> listaRegionow = new List<Region>() { reg };
            

            IList<Adres> adresyPoSelect = new List<Adres>() {a1, a2, a3};
            using (var db = polaczenie.OpenDbConnection())
            {
                dostep.DbFactory = polaczenie;

                db.CreateTable<Zamowienie>();
                db.CreateTable<Kraje>();
                db.CreateTable<Region>();
                db.CreateTable<KlientAdres>();
                db.CreateTable<Adres>();
                dostep.AktualizujListe(listaKrajow);
                dostep.AktualizujListe(listaRegionow);
                //dostep.AktualizujListe(adresyPoSelect);
                dostep.AktualizujPojedynczy(ka);
                dostep.AktualizujPojedynczy(new Zamowienie() {Id = 1, AdresId = 0});

                IList<Adres> odp = adresy.UzupelnijPozycjePoSelect(1, klient, adresyPoSelect, null);

                Assert.True(odp[0].Kraj.Equals(kr.Nazwa));
                Assert.True(odp[0].KrajSymbol.Equals(kr.Symbol));
                Assert.True(odp[0].Region.Equals(reg.Nazwa));
                Assert.True(odp[0].MoznaEdytowac);
                Assert.False(odp[0].CzyUzyty);

                Assert.True(odp[1].Kraj.Equals(kr.Nazwa));
                Assert.True(odp[1].KrajSymbol.Equals(kr.Symbol));
                Assert.True(odp[1].Region.Equals(reg.Nazwa));
                Assert.False(odp[1].MoznaEdytowac);
                Assert.False(odp[1].CzyUzyty);
                
                Assert.True(odp[2].Kraj.Equals(kr.Nazwa));
                Assert.True(odp[2].KrajSymbol.Equals(kr.Symbol));
                Assert.True(odp[2].Region.Equals(reg.Nazwa));
                Assert.True(odp[2].MoznaEdytowac);
                Assert.True(odp[2].CzyUzyty);
            }

        }


    }
}
