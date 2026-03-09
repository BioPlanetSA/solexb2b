using System.Collections.Generic;
using FakeItEasy;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using Xunit;

namespace SolEx.Hurt.CoreTests.BLL.ZadaniaKoszyka
{
    public class PozostawWKoszykuPozycjePonadStanTests
    {
        [Fact(DisplayName = "Test modułu przekroczone stany")]
        public void WykonajTest()
        {
            IKoszykiBLL koszyk = A.Fake<IKoszykiBLL>();
            IKlient klient = A.Fake<IKlient>();
            klient.Id = 1;
            A.CallTo(()=> koszyk.Klient).Returns(klient);

            JednostkaProduktu jp = new JednostkaProduktu();
            jp.Id = 1;
            jp.Przelicznik = 1;

            KoszykPozycje p1 = A.Fake<KoszykPozycje>();
            p1.IloscWJednostcePodstawowej = 1;
            p1.JednostkaId = 1;

            IProduktKlienta fp1 = A.Fake<IProduktKlienta>();
            A.CallTo(() => fp1.Jednostki).Returns(new List<JednostkaProduktu>() {jp});
            A.CallTo(() => fp1.IloscLaczna).Returns(5);
            A.CallTo(() => p1.Produkt).Returns(fp1);
            p1.Klient=klient;
            //A.CallTo(() => p1.Produkt().IloscLaczna).Returns(5); //stan w magazynie

            KoszykPozycje p2 = A.Fake<KoszykPozycje>();
            p2.IloscWJednostcePodstawowej = 2;
            IProduktKlienta fp2 = A.Fake<IProduktKlienta>();
            A.CallTo(() => fp2.Jednostki).Returns(new List<JednostkaProduktu>() { jp });
            A.CallTo(() => fp2.IloscLaczna).Returns(3);
            A.CallTo(() => p2.Produkt).Returns(fp2);
            p2.Klient = klient;
            //A.CallTo(() => p2.Produkt().IloscLaczna).Returns(3);

            KoszykPozycje p3 = A.Fake<KoszykPozycje>();
            p3.IloscWJednostcePodstawowej = 3;
            IProduktKlienta fp3 = A.Fake<IProduktKlienta>();
            A.CallTo(() => fp3.Jednostki).Returns(new List<JednostkaProduktu>() { jp });
            A.CallTo(() => fp3.IloscLaczna).Returns(3);
            A.CallTo(() => p3.Produkt).Returns(fp3);
            p3.Klient = klient;
            //A.CallTo(() => p3.Produkt().IloscLaczna).Returns(3);

            KoszykPozycje p4 = A.Fake<KoszykPozycje>();
            p4.IloscWJednostcePodstawowej = 2;
            IProduktKlienta fp4 = A.Fake<IProduktKlienta>();
            A.CallTo(() => fp4.Jednostki).Returns(new List<JednostkaProduktu>() { jp });
            A.CallTo(() => fp4.IloscLaczna).Returns(1);
            A.CallTo(() => p4.Produkt).Returns(fp4);
            p4.Klient = klient;
            // A.CallTo(() => p4.Produkt().IloscLaczna).Returns(1);

            KoszykPozycje p5 = A.Fake<KoszykPozycje>();
            p5.IloscWJednostcePodstawowej = 7;
            IProduktKlienta fp5 = A.Fake<IProduktKlienta>();
            A.CallTo(() => fp5.Jednostki).Returns(new List<JednostkaProduktu>() { jp });
            A.CallTo(() => fp5.IloscLaczna).Returns(2);
            A.CallTo(() => p5.Produkt).Returns(fp5);
            p5.Klient = klient;
            // A.CallTo(() => p5.Produkt().IloscLaczna).Returns(2);

            KoszykPozycje p6 = A.Fake<KoszykPozycje>();
            p6.IloscWJednostcePodstawowej = 7;
            IProduktKlienta fp6 = A.Fake<IProduktKlienta>();
            A.CallTo(() => fp6.Jednostki).Returns(new List<JednostkaProduktu>() { jp });
            A.CallTo(() => fp6.IloscLaczna).Returns(0);
            A.CallTo(() => p6.Produkt).Returns(fp6);
            p6.Klient = klient;
            // A.CallTo(() => p6.Produkt().IloscLaczna).Returns(0);

            List<KoszykPozycje> listaProduktow = new List<KoszykPozycje>(){p1,p2,p3,p4,p5,p6};

            A.CallTo(() => koszyk.PobierzPozycje).Returns(listaProduktow);

            ///////////////////////////////////////////////////////////
            
            var pps = new PozostawWKoszykuPozycjePonadStan();
            List<int> idMagazonow = new List<int>();
            pps.IdMagazynow = idMagazonow;


            Dictionary<IKoszykPozycja, decimal> braki = pps.PobierzBraki(koszyk);
            Assert.True(braki.Count==3);

            var nowyKoszyk = pps.TworzNowyKoszyk(braki);
            Assert.True(nowyKoszyk.Nazwa.Contains("Braki z zamówienia z dnia"));
            Assert.True(nowyKoszyk.PobierzPozycje.Count==3);

            pps.PoprawPozycjeKoszyka(koszyk, braki);
            
            var ps = new PrzekroczoneStany();
            ps.Wykonaj(koszyk);
            Assert.True(koszyk.PobierzPozycje[0].StanKoszyk == StanKoszyk.Ok);
            Assert.True(koszyk.PobierzPozycje[1].StanKoszyk == StanKoszyk.Ok);
            Assert.True(koszyk.PobierzPozycje[2].StanKoszyk == StanKoszyk.Ok);
            Assert.True(koszyk.PobierzPozycje[3].StanKoszyk == StanKoszyk.Przekroczony);
            Assert.True(koszyk.PobierzPozycje[4].StanKoszyk == StanKoszyk.Przekroczony);
            Assert.True(koszyk.PobierzPozycje[5].StanKoszyk == StanKoszyk.Niedostepy);

        }
    }
}
