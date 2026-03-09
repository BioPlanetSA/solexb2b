using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.DAL;
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
            List<IKoszykPozycja> pozycje = new List<IKoszykPozycja>();
            IKoszykiBLL koszyk = new KoszykiBLL(new Koszyki(), 1, pozycje);

            IKoszykPozycja pozycja1 = A.Fake<IKoszykPozycja>();
            pozycja1.TypPozycji = TypPozycjiKoszyka.Zwykly;
            pozycja1.id = 1;
            pozycja1.produkt_id = 1;
            pozycja1.IloscWJednostcePodstawowej = 1;

            IKoszykPozycja pozycja2 = A.Fake<IKoszykPozycja>();
            pozycja2.TypPozycji = TypPozycjiKoszyka.Automatyczny;
            pozycja2.id = 2;
            pozycja2.produkt_id = 2;
            pozycja2.IloscWJednostcePodstawowej = 6;

            IKoszykPozycja pozycja3 = A.Fake<IKoszykPozycja>();
            pozycja3.TypPozycji = TypPozycjiKoszyka.Zwykly;
            pozycja3.id = 3;
            pozycja3.produkt_id = 3;
            pozycja3.IloscWJednostcePodstawowej = 8;

            IKoszykPozycja pozycja4 = A.Fake<IKoszykPozycja>();
            pozycja4.TypPozycji = TypPozycjiKoszyka.Zwykly;
            pozycja4.id = 4;
            pozycja4.produkt_id = 4;
            pozycja4.IloscWJednostcePodstawowej = 12;

            IKoszykPozycja pozycja5 = A.Fake<IKoszykPozycja>();
            pozycja5.TypPozycji = TypPozycjiKoszyka.Automatyczny;
            pozycja5.id = 5;
            pozycja5.produkt_id = 5;
            pozycja5.IloscWJednostcePodstawowej = 16;

            koszyk.Pozycje.Add(pozycja1);
            koszyk.Pozycje.Add(pozycja2);
            koszyk.Pozycje.Add(pozycja3);
            koszyk.Pozycje.Add(pozycja4);
            koszyk.Pozycje.Add(pozycja5);

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
