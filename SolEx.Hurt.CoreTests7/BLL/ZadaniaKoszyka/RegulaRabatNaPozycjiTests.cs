using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Core.BLL.RegulyKoszyka;
using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces.Modele;
using Xunit;
namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka.Tests
{
    public class RegulaRabatNaPozycjiTests
    {
        [Fact(DisplayName = "Test dla modułu regula rabat na pozycji - rabat 2%")]
        public void WykonajTest1()
        {
            //Moduł wylicza procent 
            var testowanyModul = new RegulaRabatNaPozycji
            {
                Rabat = 2,
                JakLiczyc = OdCzegoLiczyc.OdCenyPrzedRabatem,
                TypLiczeniaRabatu = TrybLiczeniaRabatuWKoszyku.NADPISZ,
                WarunekNaliczeniaRabatu = Wartosc.Dowolna
            };


            var koszyk = A.Fake<IKoszykiBLL>();

            var poz1 = A.Fake<IKoszykPozycja>();
            poz1.Produkt.FlatCeny.CenaHurtowaNetto = 30;
            poz1.Produkt.FlatCeny.CenaNetto = 30;
            poz1.Produkt.FlatCeny.Rabat = 0;
            poz1.RabatDodatkowy = 0;

            var poz2 = A.Fake<IKoszykPozycja>();
            poz2.Produkt.FlatCeny.CenaHurtowaNetto = 101;
            poz2.Produkt.FlatCeny.CenaNetto = 101;
            poz2.Produkt.FlatCeny.Rabat = 0;
            poz2.RabatDodatkowy = 0;

            var poz3 = A.Fake<IKoszykPozycja>();
            poz3.Produkt.FlatCeny.CenaHurtowaNetto = 120;
            poz3.Produkt.FlatCeny.CenaNetto = 120;
            poz3.Produkt.FlatCeny.Rabat = 0;
            poz3.RabatDodatkowy = 0;

            var wynik = testowanyModul.Wykonaj(poz1, koszyk);
            Assert.Equal(poz1.RabatDodatkowy, 2);
            decimal cenaRabatu = (poz1.Produkt.FlatCeny.CenaNetto - (poz1.RabatDodatkowy * poz1.Produkt.FlatCeny.CenaNetto) / 100);
            Assert.Equal(cenaRabatu, 29.4m);

            wynik = testowanyModul.Wykonaj(poz2, koszyk);
            Assert.Equal(poz2.RabatDodatkowy, 2);
            cenaRabatu = (poz2.Produkt.FlatCeny.CenaNetto - (poz2.RabatDodatkowy * poz2.Produkt.FlatCeny.CenaNetto) / 100);
            Assert.Equal(cenaRabatu, 98.98m);

            wynik = testowanyModul.Wykonaj(poz3, koszyk);
            Assert.Equal(poz3.RabatDodatkowy, 2);
            cenaRabatu = (poz3.Produkt.FlatCeny.CenaNetto - (poz3.RabatDodatkowy * poz3.Produkt.FlatCeny.CenaNetto) / 100);
            Assert.Equal(cenaRabatu , 117.6m);
        }

        [Fact(DisplayName = "Test dla modułu regula rabat na pozycji - rabat 2% + dodatkowy rabat na produkt")]
        public void WykonajTest2()
        {
            var testowanyModul = new RegulaRabatNaPozycji
            {
                Rabat = 2,
                JakLiczyc = OdCzegoLiczyc.OdCenyPrzedRabatem,
                TypLiczeniaRabatu = TrybLiczeniaRabatuWKoszyku.NADPISZ,
                WarunekNaliczeniaRabatu = Wartosc.Dowolna
            };

            var koszyk = A.Fake<IKoszykiBLL>();
            var poz = A.Fake<IKoszykPozycja>();
            poz.Produkt.FlatCeny.CenaHurtowaNetto = 285.18m;
            poz.Produkt.FlatCeny.CenaNetto = 100m;
            poz.Produkt.FlatCeny.Rabat = 65; //Pozycja posiada rabat na produkt w wysokosci 65%
            poz.RabatDodatkowy = 0;

            var wynik = testowanyModul.Wykonaj(poz, koszyk);
            Assert.True(Math.Round(poz.RabatDodatkowy, 2) == 5.89m);
            decimal cenaRabatu = (poz.Produkt.FlatCeny.CenaNetto - (poz.RabatDodatkowy * poz.Produkt.FlatCeny.CenaNetto) / 100);
            Assert.Equal(Math.Round(cenaRabatu, 2), 94.11m);
        }

        [Fact(DisplayName = "Test dla modułu regula rabat na pozycji - rabat 2% + tryb liczenia sumuj")]
        public void WykonajTest3()
        {
            var testowanyModul = new RegulaRabatNaPozycji
            {
                Rabat = 2,
                JakLiczyc = OdCzegoLiczyc.OdCenyPrzedRabatem,
                TypLiczeniaRabatu = TrybLiczeniaRabatuWKoszyku.SUMUJ,
                WarunekNaliczeniaRabatu = Wartosc.Dowolna
            };

            var koszyk = A.Fake<IKoszykiBLL>();
            var poz = A.Fake<IKoszykPozycja>();
            poz.Produkt.FlatCeny.CenaHurtowaNetto = 100m;
            poz.Produkt.FlatCeny.CenaNetto = 100m;
            poz.Produkt.FlatCeny.Rabat = 2; 
            poz.RabatDodatkowy = 0;

            var wynik = testowanyModul.Wykonaj(poz, koszyk);
            Assert.Equal(poz.RabatDodatkowy, 4);
        }
    }
}
