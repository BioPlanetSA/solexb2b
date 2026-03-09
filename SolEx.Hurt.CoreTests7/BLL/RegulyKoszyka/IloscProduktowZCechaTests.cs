using System.Collections.Generic;
using FakeItEasy;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Interfaces;
using Xunit;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka.Tests
{
    public class IloscProduktowZCechaTests
    {
        [Fact()]
        public void KoszykSpelniaReguleTest()
        {
            CechyBll cecha1 = new CechyBll() { cecha_id = 1, symbol = "Symbol1" };
            CechyBll cecha2 = new CechyBll() { cecha_id = 2, symbol = "Symbol2" };
            CechyBll cecha3 = new CechyBll() { cecha_id = 3, symbol = "Symbol3" };
            CechyBll cecha4 = new CechyBll() { cecha_id = 4, symbol = "Symbol4" };

            Dictionary<int, CechyBll> d1 = new Dictionary<int, CechyBll>();
            d1.Add(cecha1.cecha_id, cecha1);
            d1.Add(cecha2.cecha_id, cecha2);

            Dictionary<int, CechyBll> d2 = new Dictionary<int, CechyBll>();
            d1.Add(cecha3.cecha_id, cecha3);
            d1.Add(cecha4.cecha_id, cecha4);

            var prod = A.Fake<IProduktKlienta>();
            prod.produkt_id = 1;
            prod.nazwa = "pierwszy";
            A.CallTo(() => prod.Cechy).Returns(d1);


            var prod2 = A.Fake<IProduktKlienta>();
            prod2.produkt_id = 1;
            prod2.nazwa = "pierwszy";
            A.CallTo(() => prod2.Cechy).Returns(d2);

            var ikoszykpoz = A.Fake<IKoszykPozycja>();
            ikoszykpoz.IloscWJednostcePodstawowej = 8;
            A.CallTo(() => ikoszykpoz.Produkt).Returns(prod);
            var ikoszykpoz2 = A.Fake<IKoszykPozycja>();
            ikoszykpoz.IloscWJednostcePodstawowej = 8;
            A.CallTo(() => ikoszykpoz2.Produkt).Returns(prod2);

            IKoszykiBLL koszyk = A.Fake<IKoszykiBLL>();
            A.CallTo(() => koszyk.Pozycje).Returns(new List<IKoszykPozycja>() { ikoszykpoz, ikoszykpoz2 });

            var config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.JezykIDPolski).Returns(1);

            var cechyAtr = A.Fake<ICechyAtrybuty>();
            A.CallTo(() => cechyAtr.PobierzCecheOid(1, 1)).Returns(cecha1);
            A.CallTo(() => cechyAtr.PobierzCecheOSymbolu("Symbol1", 1)).Returns(cecha1);

            IloscProduktowZCecha i = new IloscProduktowZCecha();
            i.Symbol = "Symbol1";
            i.Config = config;
            i.cechyAtrybuty = cechyAtr;
            i.Minimum = 5;
            i.Maksimum = 10;
            bool wynik = i.KoszykSpelniaRegule(koszyk);
            Assert.True(wynik);


            //i.Cecha.Add(1);
            //i.Minimum = 5;
            //i.Maksimum = 10;
            //wynik = i.KoszykSpelniaRegule(koszyk);
            //Assert.True(wynik);

            //i.Cecha.Add(5);
            //i.Minimum = 5;
            //i.Maksimum = 10;
            //wynik = i.KoszykSpelniaRegule(koszyk);
            //Assert.False(wynik);

            //i.Cecha.Add(2);
            //i.Minimum = 5;
            //i.Maksimum = 7;
            //wynik = i.KoszykSpelniaRegule(koszyk);
            //Assert.False(wynik);

        }


        [Fact()]
        public void KoszykSpelniaReguleTest2()
        {
            CechyBll cecha1 = new CechyBll() { cecha_id = 1, symbol = "Symbol1" };
            CechyBll cecha2 = new CechyBll() { cecha_id = 2, symbol = "Symbol2" };

            Dictionary<int, CechyBll> d1 = new Dictionary<int, CechyBll>();
            d1.Add(cecha1.cecha_id, cecha1);

            Dictionary<int, CechyBll> d2 = new Dictionary<int, CechyBll>();
            d2.Add(cecha2.cecha_id, cecha2);

            var prod = A.Fake<IProduktKlienta>();
            prod.produkt_id = 1;
            prod.nazwa = "pierwszy";
            A.CallTo(() => prod.Cechy).Returns(d1);

            var prod2 = A.Fake<IProduktKlienta>();
            prod2.produkt_id = 2;
            prod2.nazwa = "drugi";
            A.CallTo(() => prod2.Cechy).Returns(d2);

            var ikoszykpoz = A.Fake<IKoszykPozycja>();
            ikoszykpoz.IloscWJednostcePodstawowej = 8;
            A.CallTo(() => ikoszykpoz.Produkt).Returns(prod);

            var ikoszykpoz2 = A.Fake<IKoszykPozycja>();
            ikoszykpoz2.IloscWJednostcePodstawowej = 8;
            A.CallTo(() => ikoszykpoz2.Produkt).Returns(prod2);

            IKoszykiBLL koszyk = A.Fake<IKoszykiBLL>();
            A.CallTo(() => koszyk.Pozycje).Returns(new List<IKoszykPozycja>() { ikoszykpoz, ikoszykpoz2 });

            var config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.JezykIDPolski).Returns(1);

            var cechyAtr = A.Fake<ICechyAtrybuty>();
            A.CallTo(() => cechyAtr.PobierzCecheOid(1, 1)).Returns(cecha1);
            A.CallTo(() => cechyAtr.PobierzCecheOSymbolu("Symbol1", 1)).Returns(cecha1);

            IloscProduktowZCecha i = new IloscProduktowZCecha();
            i.Ilosc = IloscProduktowZCecha.IleProduktow.Wszystkie;
            //i.Cecha.Add(1);
            i.Config = config;
            i.cechyAtrybuty = cechyAtr;
            i.Minimum = 5;
            i.Maksimum = 10;
            bool wynik = i.KoszykSpelniaRegule(koszyk);
            Assert.False(wynik);


            i.Ilosc = IloscProduktowZCecha.IleProduktow.NieWszystkie;
            wynik = i.KoszykSpelniaRegule(koszyk);
            Assert.True(wynik);
        }

    }
}
