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
    public class DodajPozycjeNaPodstawieProduktowZCechaTests
    {
        [Fact(DisplayName = "Test sprawdzający moduł DodajPozycjeNaPodstawieProduktowZCecha")]
        public void WyliczDodawanaIloscTest()
        {
            List<IKoszykPozycja> pozycje = new List<IKoszykPozycja>();
            IKoszykiBLL koszyk = new KoszykiBLL(new Koszyki(), 1, pozycje);

            IKoszykPozycja pozycja1 = A.Fake<IKoszykPozycja>();
            pozycja1.TypPozycji = TypPozycjiKoszyka.Zwykly;
            pozycja1.id = 1;
            pozycja1.produkt_id = 1;
            pozycja1.IloscWJednostcePodstawowej = 5;

            IProduktKlienta produkt1 = A.Fake<IProduktKlienta>();
            produkt1.Cechy.Add(1, new CechyBll(){cecha_id = 1});
            A.CallTo(() => pozycja1.Produkt).Returns(produkt1);

            IKoszykPozycja pozycja2 = A.Fake<IKoszykPozycja>();
            pozycja2.TypPozycji = TypPozycjiKoszyka.Automatyczny;
            pozycja2.id = 2;
            pozycja2.produkt_id = 2;
            pozycja2.IloscWJednostcePodstawowej = 6;

            IProduktKlienta produkt2 = A.Fake<IProduktKlienta>();
            produkt2.Cechy.Add(1, new CechyBll() { cecha_id = 1 });
            A.CallTo(() => pozycja2.Produkt).Returns(produkt2);

            IKoszykPozycja pozycja3 = A.Fake<IKoszykPozycja>();
            pozycja3.TypPozycji = TypPozycjiKoszyka.Zwykly;
            pozycja3.id = 3;
            pozycja3.produkt_id = 3;
            pozycja3.IloscWJednostcePodstawowej = 8;

            IProduktKlienta produkt3 = A.Fake<IProduktKlienta>();
            produkt3.Cechy.Add(1, new CechyBll() { cecha_id = 1 });
            A.CallTo(() => pozycja3.Produkt).Returns(produkt3);

            IKoszykPozycja pozycja4 = A.Fake<IKoszykPozycja>();
            pozycja4.TypPozycji = TypPozycjiKoszyka.Zwykly;
            pozycja4.id = 4;
            pozycja4.produkt_id = 4;
            pozycja4.IloscWJednostcePodstawowej = 12;

            IProduktKlienta produkt4 = A.Fake<IProduktKlienta>();
            produkt4.Cechy.Add(1, new CechyBll() { cecha_id = 1 });
            A.CallTo(() => pozycja4.Produkt).Returns(produkt4);

            IKoszykPozycja pozycja5 = A.Fake<IKoszykPozycja>();
            pozycja5.TypPozycji = TypPozycjiKoszyka.Automatyczny;
            pozycja5.id = 5;
            pozycja5.produkt_id = 5;
            pozycja5.IloscWJednostcePodstawowej = 16;

            IProduktKlienta produkt5 = A.Fake<IProduktKlienta>();
            produkt5.Cechy.Add(1, new CechyBll() { cecha_id = 1 });
            A.CallTo(() => pozycja5.Produkt).Returns(produkt5);
            
            koszyk.Pozycje.Add(pozycja1);
            koszyk.Pozycje.Add(pozycja2);
            koszyk.Pozycje.Add(pozycja3);
            koszyk.Pozycje.Add(pozycja4);
            koszyk.Pozycje.Add(pozycja5);
            
            DodajPozycjeNaPodstawieProduktowZCecha modul = new DodajPozycjeNaPodstawieProduktowZCecha();

            modul.IdWymaganejCechy = 1; 
            modul.Ilosc = 2; 
            modul.IloscWielokrotnosc = 2;
            
            decimal wynik = modul.WyliczDodawanaIlosc(koszyk);
            
            Assert.Equal(wynik, 24);
        }
    }
}
