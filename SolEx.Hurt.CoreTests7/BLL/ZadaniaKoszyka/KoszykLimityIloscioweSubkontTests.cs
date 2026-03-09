using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Interfaces;
using Xunit;
namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka.Tests
{
    public class KoszykLimityIloscioweSubkontTests
    {
        [Fact(DisplayName = "Koszyk limity ilosciowe subkont - limit przekroczony")]
        public void WykonajTest1()
        {
            KoszykLimityIloscioweSubkont modul = new KoszykLimityIloscioweSubkont();
            modul.Komunikat = "Komunikat";
            var koszyk = A.Fake<IKoszykiBLL>();
            var klient = A.Fake<SolEx.Hurt.Model.Interfaces.IKlient>();

            A.CallTo(() => koszyk.Klient).Returns(klient);
            A.CallTo(() => koszyk.Klient.limiti).Returns(3);
            A.CallTo(() => koszyk.Klient.PozostalyLimitIlosciZamowien).Returns(1);

            Assert.True(modul.Wykonaj(koszyk));
        }

        [Fact(DisplayName = "Koszyk limity ilosciowe subkont - limit wykorzystany")]
        public void WykonajTest2()
        {
            KoszykLimityIloscioweSubkont modul = new KoszykLimityIloscioweSubkont();
            modul.KomunikatBlokada = "Komunikat";
            var koszyk = A.Fake<IKoszykiBLL>();
            var klient = A.Fake<IKlient>();

            A.CallTo(() => koszyk.PierwotnyKlient()).Returns(klient);
            A.CallTo(() => koszyk.PierwotnyKlient().LimitIlosciZamowien).Returns(1);
            A.CallTo(() => koszyk.PierwotnyKlient().PozostalyLimitIlosciZamowien).Returns(-1);

            Assert.False(modul.Wykonaj(koszyk));

        }
    }
}
