using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Core.BLL.RegulyKoszyka;
using SolEx.Hurt.Core.BLL.Web;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Enums;
using Xunit;
namespace SolEx.Hurt.Core.BLL.RegulyKoszyka.Tests
{
    public class RegulaKwotyTests
    {
        public void WykonajTest()
        {
            Test1();
            Test2();
            Test3();
        }


        [Fact(DisplayName = "Reguly ktowty - Nie wybieramy żadnej cechy - spradzamy tylko kwoty")]
        public void Test1()
        {
            var reguly = A.Fake<RegulaKwoty>();
            reguly.Cechy=null;    // cechy maja być puste
            reguly.CzyBrutto = false;                   // liczymy wg cen netto
            reguly.WartoscWarunek = Wartosc.Wieksze;    // Wartosc koszyka ma byc wieksza
            reguly.Kwota = 500;
            IKoszykiBLL koszyk = A.Fake<IKoszykiBLL>();

            A.CallTo(() => koszyk.CalkowitaWartoscHurtowaNettoPoRabacie()).Returns(1942); //Wartosc koszyka 500

            KoszykPozycje p1 = A.Fake<KoszykPozycje>();
            A.CallTo(() => p1.CenaNetto).Returns(30);
            A.CallTo(() => p1.WartoscNetto).Returns(30);

            KoszykPozycje p2 = A.Fake<KoszykPozycje>();
            A.CallTo(() => p2.CenaNetto).Returns(101);
            A.CallTo(() => p2.WartoscNetto).Returns(202);

            KoszykPozycje p3 = A.Fake<KoszykPozycje>();
            A.CallTo(() => p3.CenaNetto).Returns(120);
            A.CallTo(() => p3.WartoscNetto).Returns(360);

            KoszykPozycje p4 = A.Fake<KoszykPozycje>();
            A.CallTo(() => p4.CenaNetto).Returns(30);
            A.CallTo(() => p4.WartoscNetto).Returns(300);

            KoszykPozycje p5 = A.Fake<KoszykPozycje>();
            A.CallTo(() => p5.CenaNetto).Returns(150);
            A.CallTo(() => p5.WartoscNetto).Returns(1050);

            List<KoszykPozycje> lista = new List<KoszykPozycje>(){p1,p2,p3,p4,p5};

            A.CallTo(() => koszyk.PobierzPozycje).Returns(lista);

            Assert.True(reguly.KoszykSpelniaRegule(koszyk));
        }

        [Fact(DisplayName = "Reguly ktowty - Wybrać ceche ktora maja 2 pozycje w koszyku i ich cena w sumie przechodzi przez warunek")]
        public void Test2()
        {
            var reguly = A.Fake<RegulaKwoty>();
            reguly.Cechy = new List<long>() { 0,1};    
            reguly.CzyBrutto = false;                   // liczymy wg cen netto
            reguly.WartoscWarunek = Wartosc.Wieksze;    // Wartosc koszyka ma byc wieksza
            reguly.Kwota = 500;

            IKoszykiBLL koszyk = A.Fake<IKoszykiBLL>();

            A.CallTo(() => koszyk.CalkowitaWartoscHurtowaNettoPoRabacie()).Returns(1942); //Wartosc koszyka

            KoszykPozycje p1 = A.Fake<KoszykPozycje>();
            A.CallTo(() => p1.Produkt.IdCechPRoduktu).Returns(new HashSet<long>() { 0 });
            A.CallTo(() => p1.CenaNetto).Returns(30);
            A.CallTo(() => p1.WartoscNetto).Returns(30);


            KoszykPozycje p2 = A.Fake<KoszykPozycje>();
            A.CallTo(() => p2.CenaNetto).Returns(101);
            A.CallTo(() => p2.WartoscNetto).Returns(202);
            A.CallTo(() => p2.Produkt.IdCechPRoduktu).Returns(new HashSet<long>() { 1 });

            KoszykPozycje p3 = A.Fake<KoszykPozycje>();
            A.CallTo(() => p3.CenaNetto).Returns(120);
            A.CallTo(() => p3.WartoscNetto).Returns(360);
            A.CallTo(() => p3.Produkt.IdCechPRoduktu).Returns(new HashSet<long>() { 0 });

            KoszykPozycje p4 = A.Fake<KoszykPozycje>();
            A.CallTo(() => p4.CenaNetto).Returns(30);
            A.CallTo(() => p4.WartoscNetto).Returns(300);
            A.CallTo(() => p4.Produkt.IdCechPRoduktu).Returns(new HashSet<long>() { 1 });

            KoszykPozycje p5 = A.Fake<KoszykPozycje>();
            A.CallTo(() => p5.CenaNetto).Returns(150);
            A.CallTo(() => p5.WartoscNetto).Returns(1050);
            A.CallTo(() => p5.Produkt.IdCechPRoduktu).Returns(new HashSet<long>() { 0 });

            List<KoszykPozycje> lista = new List<KoszykPozycje>() { p1, p2, p3, p4, p5 };

            A.CallTo(() => koszyk.PobierzPozycje).Returns(lista);

            Assert.True(reguly.KoszykSpelniaRegule(koszyk));
        }

        [Fact(DisplayName = "Reguly ktowty - Wybrać ceche którą mają 2 pozycje w koszyku, ale suma wartośći nie przechodzi warunku")]
        public void Test3()
        {
            var reguly = A.Fake<RegulaKwoty>();
            reguly.Cechy = new List<long>() { 1 };
            reguly.CzyBrutto = false;                   
            reguly.WartoscWarunek = Wartosc.Rowne;    
            reguly.Kwota = 502;

            IKoszykiBLL koszyk = A.Fake<IKoszykiBLL>();

            A.CallTo(() => koszyk.CalkowitaWartoscHurtowaNettoPoRabacie()).Returns(1942); //suma koszyka

            KoszykPozycje p1 = A.Fake<KoszykPozycje>();
            A.CallTo(() => p1.CenaNetto).Returns(30);
            A.CallTo(() => p1.WartoscNetto).Returns(30);
            A.CallTo(() => p1.Produkt.IdCechPRoduktu).Returns(new HashSet<long>{ 0 });

            KoszykPozycje p2 = A.Fake<KoszykPozycje>();
            A.CallTo(() => p2.CenaNetto).Returns(101);
            A.CallTo(() => p2.WartoscNetto).Returns(202);
            A.CallTo(() => p2.Produkt.IdCechPRoduktu).Returns(new HashSet<long> { 1 });

            KoszykPozycje p3 = A.Fake<KoszykPozycje>();
            A.CallTo(() => p3.CenaNetto).Returns(120);
            A.CallTo(() => p3.WartoscNetto).Returns(360);
            A.CallTo(() => p3.Produkt.IdCechPRoduktu).Returns(new HashSet<long> { 0 });

            KoszykPozycje p4 = A.Fake<KoszykPozycje>();
            A.CallTo(() => p4.CenaNetto).Returns(30);
            A.CallTo(() => p4.WartoscNetto).Returns(300);
            A.CallTo(() => p4.Produkt.IdCechPRoduktu).Returns(new HashSet<long> { 1 });

            KoszykPozycje p5 = A.Fake<KoszykPozycje>();
            A.CallTo(() => p5.CenaNetto).Returns(150);
            A.CallTo(() => p5.WartoscNetto).Returns(1050);
            A.CallTo(() => p5.Produkt.IdCechPRoduktu).Returns(new HashSet<long>{ 0 });

            List<KoszykPozycje> lista = new List<KoszykPozycje>() { p1, p2, p3, p4, p5 };

            A.CallTo(() => koszyk.PobierzPozycje).Returns(lista);

            Assert.True(reguly.KoszykSpelniaRegule(koszyk));
        }
    }
}
