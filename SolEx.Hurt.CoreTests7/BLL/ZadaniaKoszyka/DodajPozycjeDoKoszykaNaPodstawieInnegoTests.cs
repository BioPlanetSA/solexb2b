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
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Interfaces.Modele;
using Xunit;
namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka.Tests
{
    public class DodajPozycjeDoKoszykaNaPodstawieInnegoTests
    {
        [Fact(DisplayName = "Test dla modułu DodajPozycjeDoKoszykaNaPodstawieInnego")]
        public void WyliczDodawanaIloscTest()
        {
            List<KoszykPozycje> pozycje = new List<KoszykPozycje>();
            IKoszykiBLL koszyk = A.Fake<KoszykBll>();
            A.CallTo(() => koszyk.PobierzPozycje).Returns(pozycje);

            KoszykPozycje pozycja1 = A.Fake<KoszykPozycje>();
            pozycja1.TypPozycji = TypPozycjiKoszyka.Zwykly;
            pozycja1.Id = 1;
            pozycja1.ProduktId = 1;
            pozycja1.IloscWJednostcePodstawowej = 1;

            KoszykPozycje pozycja2 = A.Fake<KoszykPozycje>();
            pozycja2.TypPozycji = TypPozycjiKoszyka.Automatyczny;
            pozycja2.Id = 2;
            pozycja2.ProduktId = 2;
            pozycja2.IloscWJednostcePodstawowej = 6;

            KoszykPozycje pozycja3 = A.Fake<KoszykPozycje>();
            pozycja3.TypPozycji = TypPozycjiKoszyka.Zwykly;
            pozycja3.Id = 3;
            pozycja3.ProduktId = 3;
            pozycja3.IloscWJednostcePodstawowej = 8;

            KoszykPozycje pozycja4 = A.Fake<KoszykPozycje>();
            pozycja4.TypPozycji = TypPozycjiKoszyka.Zwykly;
            pozycja4.Id = 4;
            pozycja4.ProduktId = 4;
            pozycja4.IloscWJednostcePodstawowej = 12;

            KoszykPozycje pozycja5 = A.Fake<KoszykPozycje>();
            pozycja5.TypPozycji = TypPozycjiKoszyka.Automatyczny;
            pozycja5.Id = 5;
            pozycja5.ProduktId = 5;
            pozycja5.IloscWJednostcePodstawowej = 16;

            pozycje.Add(pozycja1);
            pozycje.Add(pozycja2);
            pozycje.Add(pozycja3);
            pozycje.Add(pozycja4);
            pozycje.Add(pozycja5);

            DodajPozycjeDoKoszykaNaPodstawieInnego modul = new DodajPozycjeDoKoszykaNaPodstawieInnego();

            // Dla testu z exela
            modul.IdProduktuWymaganego = new List<string>(){"1"}; //Produkty wymagane
            modul.Ilosc = 1; //Ilość do dodania za każdą wielokrotność w jednostce podstawowej - V		
            modul.IloscWielokrotnosc = 1; // Wielokrotność na podstawie której dodajemy - X		

            decimal wynik = modul.WyliczDodawanaIlosc(koszyk);
            Assert.Equal(1, wynik);

            //Pozostale testy
            modul.Ilosc = 2;
            modul.IloscWielokrotnosc = 2;
            modul.IdProduktuWymaganego = new List<string>() { "1", "2", "3", "4", "5" }; //1,6,8,12,16
            wynik = modul.WyliczDodawanaIlosc(koszyk);
            Assert.Equal(20, wynik);

            modul.IdProduktuWymaganego = new List<string>() { "1","3","5" }; // pozycje 2,5 nie sa brane pod uwage bo maja typ pozycji na automatyczny
            wynik = modul.WyliczDodawanaIlosc(koszyk);
            Assert.Equal(8, wynik);
        }
    }
}
