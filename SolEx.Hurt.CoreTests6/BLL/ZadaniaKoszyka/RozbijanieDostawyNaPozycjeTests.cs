using System.Collections.Generic;
using FakeItEasy;
using ServiceStack.Common.Extensions;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using Xunit;
using System.Linq;

namespace SolEx.Hurt.CoreTests.BLL.ZadaniaKoszyka
{
    public class RozbijanieDostawyNaPozycjeTests
    {
        [Fact(DisplayName = "Test - Rozbijanie dostawy na pozycje")]
        public void WykonajTest()
        {
            IKoszykiBLL koszyk = A.Fake<IKoszykiBLL>();
            koszyk.KosztDostawyID = 1;
            //Produkty
            IKoszykPozycja p1 = A.Fake<IKoszykPozycja>();
            p1.Produkt.nazwa = "A.PHA-102 ROZDZIELACZ Z NYPLAMI 2 OBW.";
            A.CallTo(() => p1.CenaNetto).Returns(100);
            A.CallTo(() => p1.IloscWJednostcePodstawowej).Returns(1);

            IKoszykPozycja p2 = A.Fake<IKoszykPozycja>();
            p2.Produkt.nazwa = "A.PHA-102 ROZDZIELACZ Z NYPLAMI 5 OBW.";
            A.CallTo(() => p2.CenaNetto).Returns(10);
            A.CallTo(() => p2.IloscWJednostcePodstawowej).Returns(10);

            IKoszykPozycja p3 = A.Fake<IKoszykPozycja>();
            p3.Produkt.nazwa = "KOLANO PP 20/90";
            A.CallTo(() => p3.CenaNetto).Returns(12);
            A.CallTo(() => p3.IloscWJednostcePodstawowej).Returns(2);

            //Pozycje w koszyku
            List<IKoszykPozycja> pozycje = new List<IKoszykPozycja>() { p1, p2, p3 };
            A.CallTo(() => koszyk.Pozycje).Returns(pozycje);
            
            ISposobDostawy sposobDostawy = A.Fake<ISposobDostawy>();
            A.CallTo(() => sposobDostawy.WyliczCene(A<IKoszykiBLL>.Ignored)).Returns(200);
            A.CallTo(() => koszyk.KosztDostawy()).Returns(sposobDostawy);
            
            RozbijanieDostawyNaPozycje modul = new RozbijanieDostawyNaPozycje();
            var wynik = modul.Wykonaj(koszyk);

            //Test dla 1 sztuki 
            Assert.True(koszyk.Pozycje[0].przedstawiciel_cena_netto * koszyk.Pozycje[0].IloscWJednostcePodstawowej == 189.28m);        
            Assert.True(koszyk.Pozycje[1].przedstawiciel_cena_netto * koszyk.Pozycje[1].IloscWJednostcePodstawowej == 189.28m);        
            Assert.True(koszyk.Pozycje[2].przedstawiciel_cena_netto * koszyk.Pozycje[2].IloscWJednostcePodstawowej == 45.44m);   // do ostatniego produktu doliczne są brakujące 2 grosze
        }
    }
}
