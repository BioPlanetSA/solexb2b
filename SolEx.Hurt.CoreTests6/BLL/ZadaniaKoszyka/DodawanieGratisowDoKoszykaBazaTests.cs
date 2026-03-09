using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Core.BLL.RegulyKoszyka;
using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using Xunit;
namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka.Tests
{
    public class DodawanieGratisowDoKoszykaBazaTests
    {
        [Fact(DisplayName = "Dodawanie gratisów do koszyka - modul bazowy - gdy nie przekroczono")]
        public void DodawanieGratisowDoKoszyka()
        {
            //Gdy wartość gratisów NIE przekracza wartości koszyka
            List<IKoszykPozycja> pozycje = new List<IKoszykPozycja>();
            IKoszykiBLL koszyk = A.Fake<IKoszykiBLL>();

            //Gratis
            IKoszykPozycja pozycja1 = A.Fake<IKoszykPozycja>();
            pozycja1.TypPozycji = TypPozycjiKoszyka.Gratis;
            pozycja1.id = 1;
            pozycja1.produkt_id = 1;
            pozycja1.IloscWJednostcePodstawowej = 5;
            pozycja1.Produkt.FlatCeny.cena_hurtowa_netto = 2;          

            IProduktKlienta produkt2 = A.Fake<IProduktKlienta>();
            produkt2.Cechy.Add(1, new CechyBll());
            
            IKoszykPozycja pozycja2 = A.Fake<IKoszykPozycja>();
            pozycja2.TypPozycji = TypPozycjiKoszyka.Zwykly;
            A.CallTo(() => pozycja2.WartoscNetto).Returns(30);
            A.CallTo(() => pozycja2.Produkt).Returns(produkt2);

            IProduktKlienta produkt3 = A.Fake<IProduktKlienta>();
            produkt3.Cechy.Add(1, new CechyBll());

            IKoszykPozycja pozycja3 = A.Fake<IKoszykPozycja>();
            pozycja3.TypPozycji = TypPozycjiKoszyka.Zwykly;
            A.CallTo(() => pozycja3.WartoscNetto).Returns(360);
            A.CallTo(() => pozycja3.Produkt).Returns(produkt3);

            IProduktKlienta produkt4 = A.Fake<IProduktKlienta>();
            produkt4.Cechy.Add(2, new CechyBll());

            IKoszykPozycja pozycja4 = A.Fake<IKoszykPozycja>();
            pozycja4.TypPozycji = TypPozycjiKoszyka.Zwykly;
            A.CallTo(() => pozycja4.WartoscNetto).Returns(460);
            A.CallTo(() => pozycja4.Produkt).Returns(produkt4);

            pozycje.Add(pozycja1);
            pozycje.Add(pozycja2);
            pozycje.Add(pozycja3);
            pozycje.Add(pozycja4);

            A.CallTo(() => koszyk.Pozycje).Returns(pozycje);

            DodawanieGratisowDoKoszyka modul = new DodawanieGratisowDoKoszyka(); 
            modul.Cecha = new List<string>(){"1"};
            modul.ProcentKwoty = 5;
            var wynik = modul.Wykonaj(koszyk);

            Assert.True(wynik);
        }


        [Fact(DisplayName = "Dodawanie gratisów do koszyka - modul bazowy - gdy przekroczono")]
        public void DodawanieGratisowDoKoszyka2()
        {
            //Gdy wartość gratisów PRZEKRACZA wartość koszyka
            List<IKoszykPozycja> pozycje = new List<IKoszykPozycja>();
            IKoszykiBLL koszyk = A.Fake<IKoszykiBLL>();

            //Gratis
            IKoszykPozycja pozycja1 = A.Fake<IKoszykPozycja>();
            pozycja1.TypPozycji = TypPozycjiKoszyka.Gratis;
            pozycja1.id = 1;
            pozycja1.produkt_id = 1;
            pozycja1.IloscWJednostcePodstawowej = 50; //
            pozycja1.Produkt.FlatCeny.cena_hurtowa_netto = 5;           

            IProduktKlienta produkt2 = A.Fake<IProduktKlienta>();
            produkt2.Cechy.Add(1, new CechyBll());

            IKoszykPozycja pozycja2 = A.Fake<IKoszykPozycja>();
            pozycja2.TypPozycji = TypPozycjiKoszyka.Zwykly;
            A.CallTo(() => pozycja2.WartoscNetto).Returns(100);
            A.CallTo(() => pozycja2.Produkt).Returns(produkt2);

            IProduktKlienta produkt3 = A.Fake<IProduktKlienta>();
            produkt3.Cechy.Add(1, new CechyBll());

            IKoszykPozycja pozycja3 = A.Fake<IKoszykPozycja>();
            pozycja3.TypPozycji = TypPozycjiKoszyka.Zwykly;
            A.CallTo(() => pozycja3.WartoscNetto).Returns(60);
            A.CallTo(() => pozycja3.Produkt).Returns(produkt3);

            pozycje.Add(pozycja1);
            pozycje.Add(pozycja2);
            pozycje.Add(pozycja3);

            A.CallTo(() => koszyk.Pozycje).Returns(pozycje);

            DodawanieGratisowDoKoszyka modul = new DodawanieGratisowDoKoszyka();
            modul.Cecha = new List<string>() { "1" };
            modul.ProcentKwoty = 5;
            var wynik = modul.Wykonaj(koszyk);

            Assert.False(wynik);
        }

        [Fact(DisplayName = "Dodawanie gratisów do koszyka - modul ilosc z cecha - gdy nie przekroczono")]
        public void DodawanieGratisowDoKoszykaIlosc1()
        {
            //Gdy ilość gratisów NIE przekracza ilości pozycji nie gratisowych w koszyku
            List<IKoszykPozycja> pozycje = new List<IKoszykPozycja>();
            IKoszykiBLL koszyk = A.Fake<IKoszykiBLL>();

            //Gratis
            IKoszykPozycja pozycja1 = A.Fake<IKoszykPozycja>();
            pozycja1.TypPozycji = TypPozycjiKoszyka.Gratis;
            pozycja1.id = 1;
            pozycja1.produkt_id = 1;
            pozycja1.IloscWJednostcePodstawowej = 5;

            IProduktKlienta produkt2 = A.Fake<IProduktKlienta>();
            produkt2.Cechy.Add(1, new CechyBll());

            IKoszykPozycja pozycja2 = A.Fake<IKoszykPozycja>();
            pozycja2.TypPozycji = TypPozycjiKoszyka.Zwykly;
            pozycja2.id = 2;
            pozycja2.produkt_id = 2;
            pozycja2.IloscWJednostcePodstawowej = 10;
            A.CallTo(() => pozycja2.Produkt).Returns(produkt2);

            IProduktKlienta produkt3 = A.Fake<IProduktKlienta>();
            produkt3.Cechy.Add(1, new CechyBll());

            IKoszykPozycja pozycja3 = A.Fake<IKoszykPozycja>();
            pozycja3.TypPozycji = TypPozycjiKoszyka.Zwykly;
            pozycja3.id = 3;
            pozycja3.produkt_id = 3;
            pozycja3.IloscWJednostcePodstawowej = 20;
            A.CallTo(() => pozycja3.Produkt).Returns(produkt3);

            IProduktKlienta produkt4 = A.Fake<IProduktKlienta>();
            produkt4.Cechy.Add(1, new CechyBll());

            IKoszykPozycja pozycja4 = A.Fake<IKoszykPozycja>();
            pozycja4.TypPozycji = TypPozycjiKoszyka.Zwykly;
            pozycja4.id = 10;
            pozycja4.produkt_id = 10;
            pozycja4.IloscWJednostcePodstawowej = 200;
            A.CallTo(() => pozycja3.Produkt).Returns(produkt4);

            pozycje.Add(pozycja1);
            pozycje.Add(pozycja2);
            pozycje.Add(pozycja3);
            pozycje.Add(pozycja4);

            A.CallTo(() => koszyk.Pozycje).Returns(pozycje);

            DodawanieGratisowDoKoszykaIlosc modul = new DodawanieGratisowDoKoszykaIlosc(); 
            modul.X = 2;    //za kazde X produktów
            modul.Y = new List<string>() { "1", "2", "3" }; //z cechą Y
            modul.Z = 1;    //można wybrać Z gratisów
            var wynik = modul.Wykonaj(koszyk);

            Assert.True(wynik);
        }


        [Fact(DisplayName = "Dodawanie gratisów do koszyka - modul ilosc z cecha - gdy przekroczono")]
        public void DodawanieGratisowDoKoszykaIlosc2()
        {
            //Gdy ilość gratisów PRZEKRACZA ilości pozycji nie gratisowych w koszyku
            List<IKoszykPozycja> pozycje = new List<IKoszykPozycja>();
            IKoszykiBLL koszyk = A.Fake<IKoszykiBLL>();

            //Gratis
            IKoszykPozycja pozycja1 = A.Fake<IKoszykPozycja>();
            pozycja1.TypPozycji = TypPozycjiKoszyka.Gratis;
            pozycja1.id = 1;
            pozycja1.produkt_id = 1;
            pozycja1.IloscWJednostcePodstawowej = 300;

            IProduktKlienta produkt2 = A.Fake<IProduktKlienta>();
            produkt2.Cechy.Add(1, new CechyBll());

            IKoszykPozycja pozycja2 = A.Fake<IKoszykPozycja>();
            pozycja2.TypPozycji = TypPozycjiKoszyka.Zwykly;
            pozycja2.id = 2;
            pozycja2.produkt_id = 2;
            pozycja2.IloscWJednostcePodstawowej = 10;
            A.CallTo(() => pozycja2.Produkt).Returns(produkt2);

            IProduktKlienta produkt3 = A.Fake<IProduktKlienta>();
            produkt3.Cechy.Add(1, new CechyBll());

            IKoszykPozycja pozycja3 = A.Fake<IKoszykPozycja>();
            pozycja3.TypPozycji = TypPozycjiKoszyka.Zwykly;
            pozycja3.id = 3;
            pozycja3.produkt_id = 3;
            pozycja3.IloscWJednostcePodstawowej = 20;
            A.CallTo(() => pozycja3.Produkt).Returns(produkt3);

            IProduktKlienta produkt4 = A.Fake<IProduktKlienta>();
            produkt4.Cechy.Add(1, new CechyBll());

            IKoszykPozycja pozycja4 = A.Fake<IKoszykPozycja>();
            pozycja4.TypPozycji = TypPozycjiKoszyka.Zwykly;
            pozycja4.id = 10;
            pozycja4.produkt_id = 10;
            pozycja4.IloscWJednostcePodstawowej = 200;
            A.CallTo(() => pozycja3.Produkt).Returns(produkt4);

            pozycje.Add(pozycja1);
            pozycje.Add(pozycja2);
            pozycje.Add(pozycja3);
            pozycje.Add(pozycja4);

            A.CallTo(() => koszyk.Pozycje).Returns(pozycje);

            DodawanieGratisowDoKoszykaIlosc modul = new DodawanieGratisowDoKoszykaIlosc();
            modul.X = 2;    //za kazde X produktów
            modul.Y = new List<string>() { "1", "2", "3" }; //z cechą Y
            modul.Z = 1;    //można wybrać Z gratisów
            var wynik = modul.Wykonaj(koszyk);

            Assert.False(wynik);
        }
    }

}
