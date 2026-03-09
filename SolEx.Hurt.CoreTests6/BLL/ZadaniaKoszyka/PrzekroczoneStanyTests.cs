using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using Xunit;
namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka.Tests
{
    public class PrzekroczoneStanyTests
    {
        [Fact(DisplayName = "Test modułu przekroczone stany")]
        public void WykonajTest()
        {
            IKoszykiBLL koszyk = A.Fake<IKoszykiBLL>();

            IKoszykPozycja p1 = A.Fake<IKoszykPozycja>();
            p1.IloscWJednostcePodstawowej = 1;
            A.CallTo(() => p1.Produkt.IloscLaczna).Returns(5); //stan w magazynie

            IKoszykPozycja p2 = A.Fake<IKoszykPozycja>();
            p2.IloscWJednostcePodstawowej = 2;
            A.CallTo(() => p2.Produkt.IloscLaczna).Returns(3);

            IKoszykPozycja p3 = A.Fake<IKoszykPozycja>();
            p3.IloscWJednostcePodstawowej = 3;
            A.CallTo(() => p3.Produkt.IloscLaczna).Returns(3);

            IKoszykPozycja p4 = A.Fake<IKoszykPozycja>();
            p4.IloscWJednostcePodstawowej = 2;
            A.CallTo(() => p4.Produkt.IloscLaczna).Returns(1);

            IKoszykPozycja p5 = A.Fake<IKoszykPozycja>();
            p5.IloscWJednostcePodstawowej = 7;
            A.CallTo(() => p5.Produkt.IloscLaczna).Returns(2);

            IKoszykPozycja p6 = A.Fake<IKoszykPozycja>();
            p6.IloscWJednostcePodstawowej = 7;
            A.CallTo(() => p6.Produkt.IloscLaczna).Returns(0);

            List<IKoszykPozycja> listaProduktow = new List<IKoszykPozycja>(){p1,p2,p3,p4,p5,p6};

            A.CallTo(() => koszyk.Pozycje).Returns(listaProduktow);

            ///////////////////////////////////////////////////////////
            
            var ps = new PrzekroczoneStany();
            List<string> idMagazonow = new List<string>();
            ps.IdMagazynow = idMagazonow;
            ps.BlokadaPoPrzekroczeniu = false;

            var wynik = ps.Wykonaj(koszyk);

            Assert.False(koszyk.Pozycje[0].StanKoszyk == StanKoszyk.Przekroczony);
            Assert.False(koszyk.Pozycje[1].StanKoszyk == StanKoszyk.Przekroczony);
            Assert.False(koszyk.Pozycje[2].StanKoszyk == StanKoszyk.Przekroczony);
            Assert.True(koszyk.Pozycje[3].StanKoszyk == StanKoszyk.Przekroczony);
            Assert.True(koszyk.Pozycje[4].StanKoszyk == StanKoszyk.Przekroczony);
            Assert.True(koszyk.Pozycje[5].StanKoszyk == StanKoszyk.Niedostepy);
        }
    }
}
