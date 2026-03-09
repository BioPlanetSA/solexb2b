using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;
using SolEx.Hurt.Core.ModelBLL;
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
            List<KoszykPozycje> pozycje = new List<KoszykPozycje>();
            IKoszykiBLL koszyk = A.Fake<KoszykBll>();
            A.CallTo(() => koszyk.PobierzPozycje).Returns(pozycje);

            KoszykPozycje pozycja1 = A.Fake<KoszykPozycje>();
            pozycja1.TypPozycji = TypPozycjiKoszyka.Zwykly;
            pozycja1.Id = 1;
            pozycja1.ProduktId = 1;
            pozycja1.IloscWJednostcePodstawowej = 5;

            IProduktKlienta produkt1 = A.Fake<IProduktKlienta>();
            produkt1.Cechy.Add(1, new CechyBll(){Id = 1});
            A.CallTo(() => pozycja1.Produkt).Returns(produkt1);

            KoszykPozycje pozycja2 = A.Fake<KoszykPozycje>();
            pozycja2.TypPozycji = TypPozycjiKoszyka.Automatyczny;
            pozycja2.Id = 2;
            pozycja2.ProduktId = 2;
            pozycja2.IloscWJednostcePodstawowej = 6;

            IProduktKlienta produkt2 = A.Fake<IProduktKlienta>();
            produkt2.Cechy.Add(1, new CechyBll() { Id = 1 });
            A.CallTo(() => pozycja2.Produkt).Returns(produkt2);

            KoszykPozycje pozycja3 = A.Fake<KoszykPozycje>();
            pozycja3.TypPozycji = TypPozycjiKoszyka.Zwykly;
            pozycja3.Id = 3;
            pozycja3.ProduktId = 3;
            pozycja3.IloscWJednostcePodstawowej = 8;

            IProduktKlienta produkt3 = A.Fake<IProduktKlienta>();
            produkt3.Cechy.Add(1, new CechyBll() { Id = 1 });
            A.CallTo(() => pozycja3.Produkt).Returns(produkt3);

            KoszykPozycje pozycja4 = A.Fake<KoszykPozycje>();
            pozycja4.TypPozycji = TypPozycjiKoszyka.Zwykly;
            pozycja4.Id = 4;
            pozycja4.ProduktId = 4;
            pozycja4.IloscWJednostcePodstawowej = 12;

            IProduktKlienta produkt4 = A.Fake<IProduktKlienta>();
            produkt4.Cechy.Add(1, new CechyBll() { Id = 1 });
            A.CallTo(() => pozycja4.Produkt).Returns(produkt4);

            KoszykPozycje pozycja5 = A.Fake<KoszykPozycje>();
            pozycja5.TypPozycji = TypPozycjiKoszyka.Automatyczny;
            pozycja5.Id = 5;
            pozycja5.ProduktId = 5;
            pozycja5.IloscWJednostcePodstawowej = 16;

            IProduktKlienta produkt5 = A.Fake<IProduktKlienta>();
            produkt5.Cechy.Add(1, new CechyBll() { Id = 1 });
            A.CallTo(() => pozycja5.Produkt).Returns(produkt5);
            
            pozycje.Add(pozycja1);
            pozycje.Add(pozycja2);
            pozycje.Add(pozycja3);
            pozycje.Add(pozycja4);
            pozycje.Add(pozycja5);

            DodajPozycjeNaPodstawieProduktowZCecha modul = new DodajPozycjeNaPodstawieProduktowZCecha();

            modul.IdWymaganejCechy = 1; 
            modul.Ilosc = 2; 
            modul.IloscWielokrotnosc = 2;
            
            decimal wynik = modul.WyliczDodawanaIlosc(koszyk);
            
            Assert.Equal(wynik, 24);
        }
    }
}
