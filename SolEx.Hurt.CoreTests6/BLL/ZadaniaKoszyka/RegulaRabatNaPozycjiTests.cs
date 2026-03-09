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
            decimal wartosc = 0;
            //Moduł wylicza procent 
            var testowanyModul = new RegulaRabatNaPozycji
            {
                Rabat = 2,
                JakLiczyc = OdCzegoLiczyc.OdCenyPrzedRabatem,
                TypLiczeniaRabatu = TrybLiczeniaRabatuWKoszyku.NADPISZ,
                WarunekNaliczeniaRabatu = Wartosc.Dowolna
            };


            var koszyk = A.Fake<IKoszykiBLL>();

            var poz1 = A.Fake<KoszykPozycjaBLL>();
            poz1.Produkt.FlatCeny.cena_hurtowa_netto = 30;
            poz1.Produkt.FlatCeny.cena_netto = 30;
            poz1.Produkt.FlatCeny.rabat = 0;
            poz1.rabat_dodatkowy = 0;

            var poz2 = A.Fake<KoszykPozycjaBLL>();
            poz2.Produkt.FlatCeny.cena_hurtowa_netto = 101;
            poz2.Produkt.FlatCeny.cena_netto = 101;
            poz2.Produkt.FlatCeny.rabat = 0;
            poz2.rabat_dodatkowy = 0;

            var poz3 = A.Fake<KoszykPozycjaBLL>();
            poz3.Produkt.FlatCeny.cena_hurtowa_netto = 120;
            poz3.Produkt.FlatCeny.cena_netto = 120;
            poz3.Produkt.FlatCeny.rabat = 0;
            poz3.rabat_dodatkowy = 0;

            var wynik = testowanyModul.Wykonaj(poz1, koszyk);
            Assert.Equal(poz1.rabat_dodatkowy, 2);
            decimal cenaRabatu = (poz1.Produkt.FlatCeny.cena_netto - (poz1.rabat_dodatkowy*poz1.Produkt.FlatCeny.cena_netto) / 100);
            Assert.Equal(cenaRabatu, 29.4m);

            wynik = testowanyModul.Wykonaj(poz2, koszyk);
            Assert.Equal(poz2.rabat_dodatkowy, 2);
            cenaRabatu = (poz2.Produkt.FlatCeny.cena_netto - (poz2.rabat_dodatkowy * poz2.Produkt.FlatCeny.cena_netto) / 100);
            Assert.Equal(cenaRabatu, 98.98m);

            wynik = testowanyModul.Wykonaj(poz3, koszyk);
            Assert.Equal(poz3.rabat_dodatkowy, 2);
            cenaRabatu = (poz3.Produkt.FlatCeny.cena_netto - (poz3.rabat_dodatkowy * poz3.Produkt.FlatCeny.cena_netto) / 100);
            Assert.Equal(cenaRabatu , 117.6m);
        }

        [Fact(DisplayName = "Test dla modułu regula rabat na pozycji - rabat 2% + dodatkowy rabat na produkt")]
        public void WykonajTest2()
        {
            decimal wartosc = 0;
            var testowanyModul = new RegulaRabatNaPozycji
            {
                Rabat = 2,
                JakLiczyc = OdCzegoLiczyc.OdCenyPrzedRabatem,
                TypLiczeniaRabatu = TrybLiczeniaRabatuWKoszyku.NADPISZ,
                WarunekNaliczeniaRabatu = Wartosc.Dowolna
            };

            var koszyk = A.Fake<IKoszykiBLL>();
            var poz = A.Fake<KoszykPozycjaBLL>();
            poz.Produkt.FlatCeny.cena_hurtowa_netto = 285.18m;
            poz.Produkt.FlatCeny.cena_netto = 100m;
            poz.Produkt.FlatCeny.rabat = 65; //Pozycja posiada rabat na produkt w wysokosci 65%
            poz.rabat_dodatkowy = 0;

            var wynik = testowanyModul.Wykonaj(poz, koszyk);
            Assert.True(Math.Round(poz.rabat_dodatkowy, 2) == 5.89m);
            decimal cenaRabatu = (poz.Produkt.FlatCeny.cena_netto - (poz.rabat_dodatkowy * poz.Produkt.FlatCeny.cena_netto) / 100);
            Assert.Equal(Math.Round(cenaRabatu, 2), 94.11m);
        }

        [Fact(DisplayName = "Test dla modułu regula rabat na pozycji - rabat 2% + tryb liczenia sumuj")]
        public void WykonajTest3()
        {
            decimal wartosc = 0;
            var testowanyModul = new RegulaRabatNaPozycji
            {
                Rabat = 2,
                JakLiczyc = OdCzegoLiczyc.OdCenyPrzedRabatem,
                TypLiczeniaRabatu = TrybLiczeniaRabatuWKoszyku.SUMUJ,
                WarunekNaliczeniaRabatu = Wartosc.Dowolna
            };

            var koszyk = A.Fake<IKoszykiBLL>();
            var poz = A.Fake<KoszykPozycjaBLL>();
            poz.Produkt.FlatCeny.cena_hurtowa_netto = 100m;
            poz.Produkt.FlatCeny.cena_netto = 100m;
            poz.Produkt.FlatCeny.rabat = 2; 
            poz.rabat_dodatkowy = 0;

            var wynik = testowanyModul.Wykonaj(poz, koszyk);
            Assert.Equal(poz.rabat_dodatkowy, 4);
        }
    }
}
