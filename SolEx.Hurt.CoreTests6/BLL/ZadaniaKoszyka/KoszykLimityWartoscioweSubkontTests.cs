using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using Xunit;
namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka.Tests
{
    public class KoszykLimityWartoscioweSubkontTests
    {
        [Fact(DisplayName = "Koszyk limity wartościowe subkont - limit nie jest przekroczony")]
        public void WykonajTest1()
        {
            KoszykLimityWartoscioweSubkont modul = new KoszykLimityWartoscioweSubkont();
            modul.Komunikat = "Komunikat";
            var koszyk = A.Fake<IKoszykiBLL>();
            var klient = A.Fake<IKlient>();

            A.CallTo(() => koszyk.PierwotnyKlient()).Returns(klient);
            A.CallTo(() => koszyk.PierwotnyKlient().LimitWartosciZamowien).Returns(30);
            A.CallTo(() => koszyk.PierwotnyKlient().PozostalyLimitWartosciZamowien).Returns(15);

            A.CallTo(() => koszyk.CalkowitaWartoscHurtowaNettoPoRabacie).Returns(10);

            Assert.True(modul.Wykonaj(koszyk));
        }

        [Fact(DisplayName = "Koszyk limity wartościowe subkont - limit jest przekroczony")]
        public void WykonajTest2()
        {
            KoszykLimityWartoscioweSubkont modul = new KoszykLimityWartoscioweSubkont();
            modul.KomunikatBlokada = "Komunikat";
            var koszyk = A.Fake<IKoszykiBLL>();
            var klient = A.Fake<IKlient>();

            A.CallTo(() => koszyk.PierwotnyKlient()).Returns(klient);
            A.CallTo(() => koszyk.PierwotnyKlient().LimitWartosciZamowien).Returns(1);
            A.CallTo(() => koszyk.PierwotnyKlient().PozostalyLimitWartosciZamowien).Returns(30);

            A.CallTo(() => koszyk.CalkowitaWartoscHurtowaNettoPoRabacie).Returns(50);

            Assert.False(modul.Wykonaj(koszyk));
        }
    }
}
