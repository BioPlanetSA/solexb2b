using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL.ObiektyMaili;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using Xunit;
using SolEx.Hurt.Core.BLL;

namespace SolEx.Hurt.Core.BLL.Tests
{
    public class WysylanieWiadomosciEmailTests
    {
        [Fact()]
        public void NowyKlientTest()
        {
            int idj = 1;
            IKlient klient = A.Fake<IKlient>();
            klient.nazwa = "nazwa";
            klient.email = "test@test.pl";
            A.CallTo(() => klient.JezykId).Returns(idj);
            IConfigBLL config = A.Fake<IConfigBLL>();
          
            var produktybazowe = A.Fake<IProduktyBazowe>();
            var kliencidostep = A.Fake<IKlienciDostep>();
            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            A.CallTo(() => calosc.Konfiguracja).Returns(config);
            A.CallTo(() => calosc.Klienci).Returns(kliencidostep);
            A.CallTo(() => calosc.ProduktyBazowe).Returns(produktybazowe);
            WysylanieWiadomosciEmail modul = A.Fake<WysylanieWiadomosciEmail>(x => x.WithArgumentsForConstructor(new[] {calosc}));


            A.CallTo(() => config.PobierzTlumaczenie(A<int>._, A<string>._)).Returns("fraza");
            A.CallTo(() => config.JezykIDPolski).Returns(idj);
            string katalog = AppDomain.CurrentDomain.BaseDirectory + "\\..\\..\\..\\SolEx.Hurt.Web.Site2\\Views\\SzablonyMaili\\";
            A.CallTo(() => modul.KatalogSzablonow).Returns(katalog);
            A.CallTo(() => modul.Wszystykie(idj)).Returns(new List<Zdarzenia> { new Zdarzenia() { ModulFullTypeName = new NowyKlient(klient).Typ(), ParametryWysylania = new List<TypyPowiadomienia> { TypyPowiadomienia.Klient, TypyPowiadomienia.Opiekun, TypyPowiadomienia.Przedstawiciel }.Select(x => new ParametryWyslania() { DoKogo = x }).ToList() } });
            modul.NowyKlient(klient);

        }

        [Fact(DisplayName = "Dwa maile rożnych typow")]
        public void DwaMaileRoznychTypo()
        {
            int idj = 1;
            IKlient klient = A.Fake<IKlient>();
            klient.nazwa = "nazwa";
            klient.email = "test@test.pl";
            A.CallTo(() => klient.JezykId).Returns(idj);
            IConfigBLL config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.wlasciciel_AdresPlatformy).Returns("");
            A.CallTo(() => config.PobierzTlumaczenie(A<int>._, A<string>._)).Returns("fraza");
            A.CallTo(() => config.JezykIDPolski).Returns(idj);
            var produktybazowe = A.Fake<IProduktyBazowe>();
            var kliencidostep = A.Fake<IKlienciDostep>();
            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            A.CallTo(() => calosc.Konfiguracja).Returns(config);
            A.CallTo(() => calosc.Klienci).Returns(kliencidostep);
            A.CallTo(() => calosc.ProduktyBazowe).Returns(produktybazowe);
            WysylanieWiadomosciEmail modul = A.Fake<WysylanieWiadomosciEmail>(x => x.WithArgumentsForConstructor(new[] { calosc }));
            string katalog = AppDomain.CurrentDomain.BaseDirectory + "\\..\\..\\..\\SolEx.Hurt.Web.Site2\\Views\\SzablonyMaili\\";
            A.CallTo(() => modul.KatalogSzablonow).Returns(katalog);
            A.CallTo(() => modul.Wszystykie(idj)).Returns(new List<Zdarzenia>
            {
                new Zdarzenia() { ModulFullTypeName = new NowyKlient(klient).Typ(), 
                    ParametryWysylania = new List<TypyPowiadomienia>
                        {
                            TypyPowiadomienia.Klient, TypyPowiadomienia.Opiekun, TypyPowiadomienia.Przedstawiciel
                         }.Select(x=>new ParametryWyslania(){DoKogo = x}).ToList()
                },
                     new Zdarzenia() { ModulFullTypeName = new ResetHasla(klient).Typ(), 
                    ParametryWysylania = new List<TypyPowiadomienia>
                        {
                            TypyPowiadomienia.Klient, TypyPowiadomienia.Opiekun, TypyPowiadomienia.Przedstawiciel
                         }.Select(x=>new ParametryWyslania(){DoKogo = x}).ToList()
                }
            });
            modul.NowyKlient(klient);
            modul.ResetHasla(klient);
        }

        [Fact()]
        public void GenerujPodladTest()
        {
            IKlient klient = A.Fake<IKlient>();
            klient.nazwa = "nazwa";
            klient.email = "test@test.pl";
            int idj = 1;
            int id = 1;
            IConfigBLL config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.wlasciciel_AdresPlatformy).Returns("");
           // A.CallTo(() => config.ObiektUstawienSystemu(idj)).Returns(new SystemSettings());
            A.CallTo(() => config.PobierzTlumaczenie(A<int>._, A<string>._)).Returns("fraza");
            A.CallTo(() => config.JezykIDPolski).Returns(idj);

            var produktybazowe = A.Fake<IProduktyBazowe>();
            var kliencidostep = A.Fake<IKlienciDostep>();
            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            A.CallTo(() => calosc.Konfiguracja).Returns(config);
            A.CallTo(() => calosc.Klienci).Returns(kliencidostep);
            A.CallTo(() => calosc.ProduktyBazowe).Returns(produktybazowe);
            WysylanieWiadomosciEmail modul = A.Fake<WysylanieWiadomosciEmail>(x => x.WithArgumentsForConstructor(new[] { calosc }));
            string katalog = AppDomain.CurrentDomain.BaseDirectory + "\\..\\..\\..\\SolEx.Hurt.Web.Site2\\Views\\SzablonyMaili\\";
            A.CallTo(() => modul.KatalogSzablonow).Returns(katalog);
            A.CallTo(() => modul.Pobierz(id,idj)).Returns(
                new Zdarzenia()
                {
                    Id = id,
                    ModulFullTypeName = new NoweZamowienie().Typ(),
                    ParametryWysylania = new List<TypyPowiadomienia>
                        {
                            TypyPowiadomienia.Klient, TypyPowiadomienia.Opiekun, TypyPowiadomienia.Przedstawiciel
                         }.Select(x => new ParametryWyslania() { DoKogo = x }).ToList()
                
            });
            var wynik = modul.GenerujPodlad(id, idj, TypyPowiadomienia.Klient);
            Assert.NotNull(wynik);
        }
    }
}
