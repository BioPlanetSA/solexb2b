using Xunit;
using SolEx.Hurt.Web.Site2.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;

namespace SolEx.Hurt.Web.Site2.Controllers.Tests
{
    public class IntegracjaControllerTests: IntegracjaController
    {
        [Fact()]
        public void pobierzSzablonIntegracjiWgNazwyZrodlaDanychKataloguTest()
        {
            var fake = new TworzenieFakeObiektow();
            PlikIntegracjiSzablon szablonTestowy = (PlikIntegracjiSzablon)fake.PobierzObiektOTypie(typeof(PlikIntegracjiSzablon));

            this.Calosc = A.Fake<ISolexBllCalosc>();
            A.CallTo(() => this.Calosc.Konfiguracja.TablicaPlikowIntegracjiWgId).Returns(new Dictionary<int, PlikIntegracjiSzablon> {{szablonTestowy.IdSzablonu, szablonTestowy}} );

            string nazwaTestowa = PlikIntegracjiSzablon.GenerujNazwePliku_plikDoDrukowaniaKatalogu(szablonTestowy, 66);

            int wersja = 0;
            PlikIntegracjiSzablon wyciagniety = this.pobierzSzablonIntegracjiWgNazwyZrodlaDanychKatalogu(nazwaTestowa, out wersja);

            Assert.True(wersja == 66);
            Assert.True(wyciagniety.IdSzablonu == szablonTestowy.IdSzablonu);
        }

    }
}