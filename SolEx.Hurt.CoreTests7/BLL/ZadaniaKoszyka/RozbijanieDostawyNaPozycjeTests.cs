using System.Collections.Generic;
using FakeItEasy;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using Xunit;

namespace SolEx.Hurt.CoreTests.BLL.ZadaniaKoszyka
{
    public class RozbijanieDostawyNaPozycjeTests
    {
        [Fact(DisplayName = "Test - Rozbijanie dostawy na pozycje")]
        public void WykonajTest()
        {
            IKoszykiBLL koszyk = A.Fake<IKoszykiBLL>();
            koszyk.KosztDostawyId = 1;
            //Produkty
            KoszykPozycje p1 = A.Fake<KoszykPozycje>();
            p1.Produkt.Nazwa = "A.PHA-102 ROZDZIELACZ Z NYPLAMI 2 OBW.";
            A.CallTo(() => p1.CenaNetto).Returns(100);
            A.CallTo(() => p1.IloscWJednostcePodstawowej).Returns(1);

            KoszykPozycje p2 = A.Fake<KoszykPozycje>();
            p2.Produkt.Nazwa = "A.PHA-102 ROZDZIELACZ Z NYPLAMI 5 OBW.";
            A.CallTo(() => p2.CenaNetto).Returns(10);
            A.CallTo(() => p2.IloscWJednostcePodstawowej).Returns(10);

            KoszykPozycje p3 = A.Fake<KoszykPozycje>();
            p3.Produkt.Nazwa = "KOLANO PP 20/90";
            A.CallTo(() => p3.CenaNetto).Returns(12);
            A.CallTo(() => p3.IloscWJednostcePodstawowej).Returns(2);

            //Pozycje w koszyku
            List<KoszykPozycje> pozycje = new List<KoszykPozycje>() { p1, p2, p3 };
            A.CallTo(() => koszyk.PobierzPozycje).Returns(pozycje);
            
            ISposobDostawy sposobDostawy = A.Fake<ISposobDostawy>();
            A.CallTo(() => sposobDostawy.WyliczCene(A<IKoszykiBLL>.Ignored)).Returns(200);
            A.CallTo(() => koszyk.KosztDostawy()).Returns(sposobDostawy);
            
            RozbijanieDostawyNaPozycje modul = new RozbijanieDostawyNaPozycje();
            var wynik = modul.Wykonaj(koszyk);

            //Test dla 1 sztuki 
            Assert.True(koszyk.PobierzPozycje[0].WymuszonaCenaNettoModul * koszyk.PobierzPozycje[0].IloscWJednostcePodstawowej == 189.28m);        
            Assert.True(koszyk.PobierzPozycje[1].WymuszonaCenaNettoModul * koszyk.PobierzPozycje[1].IloscWJednostcePodstawowej == 189.28m);        
            Assert.True(koszyk.PobierzPozycje[2].WymuszonaCenaNettoModul * koszyk.PobierzPozycje[2].IloscWJednostcePodstawowej == 45.44m);   // do ostatniego produktu doliczne są brakujące 2 grosze
        }
    }
}
