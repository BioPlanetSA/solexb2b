using System.Collections.Generic;
using FakeItEasy;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.BLL.RegulyKoszyka;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Interfaces;
using Xunit;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka.Tests
{
    public class IloscProduktowZAtrybutemTest
    {
        [Fact()]
        public void KoszykSpelniaReguleTest()
        {

            CechyBll cecha1 = new CechyBll() { cecha_id = 1, symbol = "Symbol1", atrybut_id = 1 };
            CechyBll cecha2 = new CechyBll() { cecha_id = 2, symbol = "Symbol2", atrybut_id = 1 };
            CechyBll cecha3 = new CechyBll() { cecha_id = 3, symbol = "Symbol3", atrybut_id = 2 };
            CechyBll cecha4 = new CechyBll() { cecha_id = 4, symbol = "Symbol4", atrybut_id = 2 };

            AtrybutyBLL atrybut1 = new AtrybutyBLL() { atrybut_id = 1, symbol = "Atrybut1" };
            atrybut1.ListaCech = new List<CechyBll>(){cecha1,cecha2};
            AtrybutyBLL atrybut2 = new AtrybutyBLL() { atrybut_id = 2, symbol = "Atrybut2" };
            atrybut2.ListaCech = new List<CechyBll>(){cecha3,cecha4};

            Dictionary<int, CechyBll> d1 = new Dictionary<int, CechyBll>();
            d1.Add(cecha1.cecha_id, cecha1);
            d1.Add(cecha2.cecha_id, cecha2);

            Dictionary<int, CechyBll> d2 = new Dictionary<int, CechyBll>();
            d2.Add(cecha3.cecha_id, cecha3);
            //d2.Add(cecha4.cecha_id, cecha4);
            
            Dictionary<int, CechyBll> d = new Dictionary<int, CechyBll>();
            d.Add(cecha1.cecha_id, cecha1);
            d.Add(cecha2.cecha_id, cecha2);
            d.Add(cecha3.cecha_id, cecha3);
            d.Add(cecha4.cecha_id, cecha4);

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
            ikoszykpoz2.IloscWJednostcePodstawowej = 8;
            A.CallTo(() => ikoszykpoz2.Produkt).Returns(prod2);

            IKoszykiBLL koszyk = A.Fake<IKoszykiBLL>();
            A.CallTo(() => koszyk.Pozycje).Returns(new List<IKoszykPozycja>() { ikoszykpoz, ikoszykpoz2 });

            var config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.JezykIDPolski).Returns(1);

            var cechyAtr = A.Fake<ICechyAtrybuty>();
            A.CallTo(() => cechyAtr.PobierzPoID(1, 1)).Returns(atrybut1);
            A.CallTo(() => cechyAtr.PobierzPoID(2, 1)).Returns(atrybut2);
            A.CallTo(() => cechyAtr.PobierzCecheDlaAtrybutu(1, 1)).Returns(atrybut1.ListaCech);
            A.CallTo(() => cechyAtr.PobierzCecheDlaAtrybutu(2, 1)).Returns(atrybut2.ListaCech);


            IloscProduktowZAtrybutem i = new IloscProduktowZAtrybutem();
            i.Config = config;
            i.cechyAtrybuty = cechyAtr;
            i.Ilosc = IloscLacznaBaza.IleProduktow.NieWszystkie;

            i.Atrybut = 1;
            i.IloscCech = IloscProduktowZAtrybutem.IleCechAtrybutu.NieWszystkie;
            i.Minimum = 5;
            i.Maksimum = 20;
            bool wynik = i.KoszykSpelniaRegule(koszyk);
            Assert.True(wynik);
            
            i.Atrybut = 1;
            i.IloscCech = IloscProduktowZAtrybutem.IleCechAtrybutu.Wszystkie;
            i.Minimum = 5;
            i.Maksimum = 20;
            wynik = i.KoszykSpelniaRegule(koszyk);
            Assert.True(wynik);


            i.Atrybut = 2;
            i.IloscCech = IloscProduktowZAtrybutem.IleCechAtrybutu.NieWszystkie;
            i.Minimum = 5;
            i.Maksimum = 20;
            wynik = i.KoszykSpelniaRegule(koszyk);
            Assert.True(wynik);

            i.Atrybut = 2;
            i.IloscCech = IloscProduktowZAtrybutem.IleCechAtrybutu.Wszystkie;
            i.Minimum = 5;
            i.Maksimum = 20;
            wynik = i.KoszykSpelniaRegule(koszyk);
            Assert.False(wynik);
           
        }
    }
}