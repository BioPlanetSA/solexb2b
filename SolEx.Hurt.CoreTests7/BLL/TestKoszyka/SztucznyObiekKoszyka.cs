using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using FakeItEasy.ExtensionSyntax.Full;
using Fasterflect;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Core;

namespace SolEx.Hurt.CoreTests.BLL.TestKoszyka
{
    public class SztucznyObiekKoszyka
    {
        public IKoszykiBLL StworzKoszyk()
        {
            var koszyk = A.Fake<IKoszykiBLL>();

            //Produkt 1
            IKoszykPozycja pozycja1 = A.Fake<IKoszykPozycja>();
            pozycja1.Produkt.nazwa = "Produkt 1";
            pozycja1.Produkt.kod_kreskowy = "58123456789";                                     
            pozycja1.IloscWJednostcePodstawowej = 10;
            pozycja1.ilosc = 1;
            pozycja1.Produkt.vat = 23;
            decimal cenaNettoPrzedRabatem1 = 1000; // J
            decimal cenaBruttoPrzedRabatem1 = 1230; 
            decimal stawkaVat1 = 23;
            decimal rabatWyliczony1 = 10;

            A.CallTo(() => pozycja1.Produkt.CenaHurtowaNetto).Returns(new WartoscLiczbowa(cenaNettoPrzedRabatem1, "PLN"));
            A.CallTo(() => pozycja1.Produkt.CenaHurtowaBrutto).Returns(new WartoscLiczbowa(cenaBruttoPrzedRabatem1, "PLN"));
            A.CallTo(() => pozycja1.CalkowityRabat).Returns(new WartoscLiczbowa(rabatWyliczony1, ""));
            //A.CallTo(() => pozycja1.Produkt.CenaNetto).Returns(new WartoscLiczbowa(900.0000m, "PLN"));
            //A.CallTo(() => pozycja1.Produkt.CenaBrutto).Returns(new WartoscLiczbowa(1107.00m, "PLN"));  
            
            IFlatCenyBLL cena1 = A.Fake<IFlatCenyBLL>();
            cena1.cena_detaliczna_netto = 2000;                 // Cena netto katalogowa
            cena1.cena_detaliczna_netto.Waluta = "PLN";
            A.CallTo(() => cena1.cena_detaliczna_brutto).Returns(new WartoscLiczbowa(2460, "PLN"));
            A.CallTo(() => pozycja1.Produkt.FlatCeny).Returns(cena1);

            //////////////////////////////////////////////////////////////////////////////

            decimal wartoscNettoKatalogowa1 = Decimal.Round(cena1.cena_detaliczna_netto * pozycja1.IloscWJednostcePodstawowej,2); // H
            decimal wartoscBruttoKatalogowa1 = Decimal.Round(wartoscNettoKatalogowa1*(1 + (stawkaVat1/100)),2); // I
            decimal wartoscNettoPrzedRabatem1 = Decimal.Round(pozycja1.IloscWJednostcePodstawowej*cenaNettoPrzedRabatem1,2);
            decimal wartoscBruttoPrzedRabatem1 = Decimal.Round(wartoscNettoPrzedRabatem1 * (1 + (stawkaVat1 / 100)),2);
            decimal rabatWartosc1 = Decimal.Round(pozycja1.IloscWJednostcePodstawowej * cenaNettoPrzedRabatem1 * ((100 - rabatWyliczony1) / 100),4);
            decimal cenaNettoPoRabacie1 = Decimal.Round(cenaNettoPrzedRabatem1 * ((100 - rabatWyliczony1) / 100),2);
            decimal wartoscNettoPoRabacie1 = Decimal.Round(pozycja1.IloscWJednostcePodstawowej * cenaNettoPrzedRabatem1 * ((100 - rabatWyliczony1) / 100),2);
            decimal wartoscBruttoPoRabacie1 = Decimal.Round(wartoscNettoPoRabacie1*(1 + (stawkaVat1/100)),2);
            decimal vatWartoscKatalogowa1 = Decimal.Round(wartoscNettoKatalogowa1 * (stawkaVat1 / 100),2);
            decimal vatWartoscPrzedRabatem1 = Decimal.Round(wartoscNettoPrzedRabatem1 * (stawkaVat1 / 100),2);
            decimal vatWartoscPoRabacie1 = Decimal.Round(wartoscNettoPoRabacie1 * (stawkaVat1 / 100),2);
            
            //////////////////////////////////////////////////////////////////////////////

            //detal brutto


           
            //Produkt 2 
            IKoszykPozycja pozycja2 = A.Fake<IKoszykPozycja>();
            pozycja2.Produkt.nazwa = "Produkt 2";
            pozycja2.Produkt.kod_kreskowy = "58123456789";                                     
            pozycja2.IloscWJednostcePodstawowej = 135;
            pozycja2.ilosc = 1;
            pozycja2.Produkt.vat = 8;
            pozycja2.Produkt.FlatCeny.rabat = 0.5m;
            pozycja2.Produkt.FlatCeny.cena_detaliczna_netto = 1000;
            pozycja2.Produkt.FlatCeny.cena_hurtowa_netto = 500;
            decimal cenaNettoPrzedRabatem2 = 500;
            decimal cenaBruttoPrzedRabatem2 = 540;
            decimal stawkaVat2 = 8;
            decimal rabatWyliczony2 = 0.5m;

            A.CallTo(() => pozycja2.Produkt.CenaHurtowaNetto).Returns(new WartoscLiczbowa(cenaNettoPrzedRabatem2, "PLN"));
            A.CallTo(() => pozycja2.Produkt.CenaHurtowaBrutto).Returns(new WartoscLiczbowa(cenaBruttoPrzedRabatem2, "PLN"));
            A.CallTo(() => pozycja2.CenaBrutto).Returns(new WartoscLiczbowa(72900.0000m, "PLN"));
            A.CallTo(() => pozycja2.CalkowityRabat).Returns(new WartoscLiczbowa(rabatWyliczony2, ""));

            IFlatCenyBLL cena2 = A.Fake<IFlatCenyBLL>();
            cena2.cena_detaliczna_netto = 1000;
            cena2.cena_detaliczna_netto.Waluta = "PLN";
            A.CallTo(() => cena2.cena_detaliczna_brutto).Returns(new WartoscLiczbowa(1080, "PLN"));
            A.CallTo(() => pozycja2.Produkt.FlatCeny).Returns(cena2);
            //////////////////////////////////////////////////////////////////////////////

            decimal wartoscNettoKatalogowa2 = Decimal.Round(cena2.cena_detaliczna_netto * pozycja2.IloscWJednostcePodstawowej,2); // H
            decimal wartoscBruttoKatalogowa2 = Decimal.Round(wartoscNettoKatalogowa2 * (1 + (stawkaVat2 / 100)),2); // I
            decimal wartoscNettoPrzedRabatem2 = Decimal.Round(pozycja2.IloscWJednostcePodstawowej * cenaNettoPrzedRabatem2,2);
            decimal wartoscBruttoPrzedRabatem2 = Decimal.Round(wartoscNettoPrzedRabatem2 * (1 + (stawkaVat2 / 100)),2);
            decimal rabatWartosc2 = Decimal.Round(pozycja2.IloscWJednostcePodstawowej * cenaNettoPrzedRabatem2 * ((100 - rabatWyliczony2) / 100),4);
            decimal cenaNettoPoRabacie2 = Decimal.Round(cenaNettoPrzedRabatem2 * ((100 - rabatWyliczony2) / 100),2);
            decimal wartoscNettoPoRabacie2 = Decimal.Round(pozycja2.IloscWJednostcePodstawowej * cenaNettoPrzedRabatem2 * ((100 - rabatWyliczony2) / 100),2);
            decimal wartoscBruttoPoRabacie2 = Decimal.Round(wartoscNettoPoRabacie2 * (1 + (stawkaVat2 / 100)),2);
            decimal vatWartoscKatalogowa2 = Decimal.Round(wartoscNettoKatalogowa2 * (stawkaVat2 / 100),2);
            decimal vatWartoscPrzedRabatem2 = Decimal.Round(wartoscNettoPrzedRabatem2 * (stawkaVat2 / 100),2);
            decimal vatWartoscPoRabacie2 = Decimal.Round(wartoscNettoPoRabacie2 * (stawkaVat2 / 100),2);

            //////////////////////////////////////////////////////////////////////////////



            //Produkt 3
            IKoszykPozycja pozycja3 = A.Fake<IKoszykPozycja>();
            pozycja3.Produkt.nazwa = "Produkt 3";
            pozycja3.Produkt.kod_kreskowy = "58123456789";                                     
            pozycja3.IloscWJednostcePodstawowej = 8;
            pozycja3.ilosc = 1;
            pozycja3.Produkt.vat = 23;
            pozycja3.Produkt.FlatCeny.rabat = 10;
            pozycja3.Produkt.FlatCeny.cena_detaliczna_netto = 1;
            pozycja3.Produkt.FlatCeny.cena_hurtowa_netto = 0.04m;
            decimal cenaNettoPrzedRabatem3 = 0.04m;
            decimal cenaBruttoPrzedRabatem3 = 0.04875m;
            decimal stawkaVat3 = 23;
            decimal rabatWyliczony3 = 10;

            A.CallTo(() => pozycja3.Produkt.CenaHurtowaNetto).Returns(new WartoscLiczbowa(cenaNettoPrzedRabatem3, "PLN"));
            A.CallTo(() => pozycja3.Produkt.CenaHurtowaBrutto).Returns(new WartoscLiczbowa(cenaBruttoPrzedRabatem3, "PLN"));
            A.CallTo(() => pozycja3.CenaBrutto).Returns(new WartoscLiczbowa(0.3900m, "PLN"));
            A.CallTo(() => pozycja3.CalkowityRabat).Returns(new WartoscLiczbowa(rabatWyliczony3, ""));
            
            IFlatCenyBLL cena3 = A.Fake<IFlatCenyBLL>();
            cena3.cena_detaliczna_netto = 1;
            cena3.cena_detaliczna_netto.Waluta = "PLN";
            A.CallTo(() => cena3.cena_detaliczna_brutto).Returns(new WartoscLiczbowa(1.23m, "PLN"));
            A.CallTo(() => pozycja3.Produkt.FlatCeny).Returns(cena3); 

            //////////////////////////////////////////////////////////////////////////////

            decimal wartoscNettoKatalogowa3 =   Decimal.Round(cena3.cena_detaliczna_netto * pozycja3.IloscWJednostcePodstawowej,2); // H
            decimal wartoscBruttoKatalogowa3 =  Decimal.Round(wartoscNettoKatalogowa3 * (1 + (stawkaVat3 / 100)),2); // I
            decimal wartoscNettoPrzedRabatem3 = Decimal.Round(pozycja3.IloscWJednostcePodstawowej * cenaNettoPrzedRabatem3,2);
            decimal wartoscBruttoPrzedRabatem3 = Decimal.Round(wartoscNettoPrzedRabatem3 * (1 + (stawkaVat3 / 100)),2);
            decimal rabatWartosc3 = Decimal.Round(pozycja3.IloscWJednostcePodstawowej * cenaNettoPrzedRabatem3 * ((100 - rabatWyliczony3) / 100),4);
            decimal cenaNettoPoRabacie3 = cenaNettoPrzedRabatem3 * ((100 - rabatWyliczony3) / 100);
            decimal wartoscNettoPoRabacie3 = Decimal.Round(pozycja3.IloscWJednostcePodstawowej * cenaNettoPrzedRabatem3 * ((100 - rabatWyliczony3) / 100),2);
            decimal wartoscBruttoPoRabacie3 = Decimal.Round(wartoscNettoPoRabacie3 * (1 + (stawkaVat3 / 100)),2);
            decimal vatWartoscKatalogowa3 = Decimal.Round(wartoscNettoKatalogowa3 * (stawkaVat3 / 100),2);
            decimal vatWartoscPrzedRabatem3 = Decimal.Round(wartoscNettoPrzedRabatem3 * (stawkaVat3 / 100),2);
            decimal vatWartoscPoRabacie3 = Decimal.Round(wartoscNettoPoRabacie3 * (stawkaVat3 / 100),2);

            //////////////////////////////////////////////////////////////////////////////

            

            //Produkt 4
            IKoszykPozycja pozycja4 = A.Fake<IKoszykPozycja>();
            pozycja4.Produkt.nazwa = "Produkt 4";
            pozycja4.Produkt.kod_kreskowy = "58123456789";                                     
            pozycja4.IloscWJednostcePodstawowej = 10;
            pozycja4.ilosc = 1;
            pozycja4.Produkt.vat = 8;
            decimal cenaNettoPrzedRabatem4 = 0.05m;
            decimal cenaBruttoPrzedRabatem4 = 0.054m;
            decimal stawkaVat4 = 8;
            decimal rabatWyliczony4 = 0.5m;

            A.CallTo(() => pozycja4.Produkt.CenaHurtowaNetto).Returns(new WartoscLiczbowa(cenaNettoPrzedRabatem4, "PLN"));
            A.CallTo(() => pozycja4.Produkt.CenaHurtowaBrutto).Returns(new WartoscLiczbowa(cenaBruttoPrzedRabatem4, "PLN"));
            A.CallTo(() => pozycja4.CenaBrutto).Returns(new WartoscLiczbowa(0.5400m, "PLN"));
            A.CallTo(() => pozycja4.CalkowityRabat).Returns(new WartoscLiczbowa(rabatWyliczony4, ""));

            IFlatCenyBLL cena4 = A.Fake<IFlatCenyBLL>();
            cena4.cena_detaliczna_netto = 1.5m;
            cena4.cena_detaliczna_netto.Waluta = "PLN";
            
            A.CallTo(() => cena4.cena_detaliczna_brutto).Returns(new WartoscLiczbowa(1.62m, "PLN"));
            A.CallTo(() => pozycja4.Produkt.FlatCeny).Returns(cena4); 
            //////////////////////////////////////////////////////////////////////////////

            decimal wartoscNettoKatalogowa4 = Decimal.Round(cena4.cena_detaliczna_netto * pozycja4.IloscWJednostcePodstawowej,2); // H
            decimal wartoscBruttoKatalogowa4 = Decimal.Round(wartoscNettoKatalogowa4 * (1 + (stawkaVat4 / 100)),2); // I
            decimal wartoscNettoPrzedRabatem4 = Decimal.Round(pozycja4.IloscWJednostcePodstawowej * cenaNettoPrzedRabatem4,2);
            decimal wartoscBruttoPrzedRabatem4 = Decimal.Round(wartoscNettoPrzedRabatem4 * (1 + (stawkaVat4 / 100)),2);
            decimal rabatWartosc4 = Decimal.Round(pozycja4.IloscWJednostcePodstawowej * cenaNettoPrzedRabatem4 * ((100 - rabatWyliczony4) / 100),4);
            decimal cenaNettoPoRabacie4 = Decimal.Round(cenaNettoPrzedRabatem4 * ((100 - rabatWyliczony4) / 100),2);
            decimal wartoscNettoPoRabacie4 = Decimal.Round(pozycja4.IloscWJednostcePodstawowej * cenaNettoPrzedRabatem4 * ((100 - rabatWyliczony4) / 100),2);
            decimal wartoscBruttoPoRabacie4 = Decimal.Round(wartoscNettoPoRabacie4 * (1 + (stawkaVat4 / 100)),2);
            decimal vatWartoscKatalogowa4 = Decimal.Round(wartoscNettoKatalogowa4 * (stawkaVat4 / 100),2);
            decimal vatWartoscPrzedRabatem4 = Decimal.Round(wartoscNettoPrzedRabatem4 * (stawkaVat4 / 100),2);
            decimal vatWartoscPoRabacie4= Decimal.Round(wartoscNettoPoRabacie4 * (stawkaVat4 / 100),2);

            //////////////////////////////////////////////////////////////////////////////

            
            
            
            //Produkt 5
            IKoszykPozycja pozycja5 = A.Fake<IKoszykPozycja>();
            pozycja5.Produkt.nazwa = "Produkt 5";
            pozycja5.Produkt.kod_kreskowy = "58123456789";                                     
            pozycja5.IloscWJednostcePodstawowej = 5;
            pozycja5.ilosc = 1;
            pozycja5.Produkt.vat = 23;
            decimal cenaNettoPrzedRabatem5 = 1000;
            decimal cenaBruttoPrzedRabatem5 = 1230;
            decimal stawkaVat5 = 23;
            decimal rabatWyliczony5 = -10;

            A.CallTo(() => pozycja5.Produkt.CenaHurtowaNetto).Returns(new WartoscLiczbowa(cenaNettoPrzedRabatem5, "PLN"));
            A.CallTo(() => pozycja5.Produkt.CenaHurtowaBrutto).Returns(new WartoscLiczbowa(cenaBruttoPrzedRabatem5, "PLN"));
            A.CallTo(() => pozycja5.CenaBrutto).Returns(new WartoscLiczbowa(6150.00m, "PLN"));
            A.CallTo(() => pozycja5.CalkowityRabat).Returns(new WartoscLiczbowa(rabatWyliczony5, ""));

            IFlatCenyBLL cena5 = A.Fake<IFlatCenyBLL>();
            cena5.cena_detaliczna_netto = 1800;
            cena5.cena_detaliczna_netto.Waluta = "PLN";

            A.CallTo(() => cena5.cena_detaliczna_brutto).Returns(new WartoscLiczbowa(2214, "PLN"));
            A.CallTo(() => pozycja5.Produkt.FlatCeny).Returns(cena5); 

            //////////////////////////////////////////////////////////////////////////////

            decimal wartoscNettoKatalogowa5 = Decimal.Round(cena5.cena_detaliczna_netto * pozycja5.IloscWJednostcePodstawowej,2); // H
            decimal wartoscBruttoKatalogowa5 = Decimal.Round(wartoscNettoKatalogowa5 * (1 + (stawkaVat5 / 100)),2); // I
            decimal wartoscNettoPrzedRabatem5 = Decimal.Round(pozycja5.IloscWJednostcePodstawowej * cenaNettoPrzedRabatem5,2);
            decimal wartoscBruttoPrzedRabatem5 = Decimal.Round(wartoscNettoPrzedRabatem5 * (1 + (stawkaVat5 / 100)), 2);
            decimal rabatWartosc5 = Decimal.Round(pozycja5.IloscWJednostcePodstawowej * cenaNettoPrzedRabatem5 * ((100 - rabatWyliczony5) / 100), 4);
            decimal cenaNettoPoRabacie5 = Decimal.Round(cenaNettoPrzedRabatem5 * ((100 - rabatWyliczony5) / 100), 2);
            decimal wartoscNettoPoRabacie5 = Decimal.Round(pozycja5.IloscWJednostcePodstawowej * cenaNettoPrzedRabatem5 * ((100 - rabatWyliczony5) / 100), 2);
            decimal wartoscBruttoPoRabacie5 = Decimal.Round(wartoscNettoPoRabacie5 * (1 + (stawkaVat5 / 100)), 2);
            decimal vatWartoscKatalogowa5 = Decimal.Round(wartoscNettoKatalogowa5 * (stawkaVat5 / 100), 2);
            decimal vatWartoscPrzedRabatem5 = Decimal.Round(wartoscNettoPrzedRabatem5 * (stawkaVat5 / 100), 2);
            decimal vatWartoscPoRabacie5 = Decimal.Round(wartoscNettoPoRabacie5 * (stawkaVat5 / 100), 2);

            //////////////////////////////////////////////////////////////////////////////


            
           
            
            //Produkt 6
            IKoszykPozycja pozycja6 = A.Fake<IKoszykPozycja>();
            pozycja6.Produkt.nazwa = "Produkt 6";
            pozycja6.Produkt.kod_kreskowy = "58123456789";                                    
            pozycja6.IloscWJednostcePodstawowej = 2.4m;
            pozycja6.ilosc = 1;
            pozycja6.Produkt.vat = 8; 
            decimal cenaNettoPrzedRabatem6 = 0.04m;
            decimal cenaBruttoPrzedRabatem6 = 0.0458m;
            decimal stawkaVat6 = 8;
            decimal rabatWyliczony6 = -0.5m;

            A.CallTo(() => pozycja6.Produkt.CenaHurtowaNetto).Returns(new WartoscLiczbowa(cenaNettoPrzedRabatem6, "PLN"));
            A.CallTo(() => pozycja6.Produkt.CenaHurtowaBrutto).Returns(new WartoscLiczbowa(cenaBruttoPrzedRabatem6, "PLN"));
            A.CallTo(() => pozycja6.CenaBrutto).Returns(new WartoscLiczbowa(0.11m, "PLN"));
            A.CallTo(() => pozycja6.CalkowityRabat).Returns(new WartoscLiczbowa(rabatWyliczony6, ""));

            IFlatCenyBLL cena6 = A.Fake<IFlatCenyBLL>();
            cena6.cena_detaliczna_netto = 0.08m;
            cena6.cena_detaliczna_netto.Waluta = "PLN";

            A.CallTo(() => cena6.cena_detaliczna_brutto).Returns(new WartoscLiczbowa(0.09m, "PLN"));
            A.CallTo(() => pozycja6.Produkt.FlatCeny).Returns(cena6); 
            //////////////////////////////////////////////////////////////////////////////

            decimal wartoscNettoKatalogowa6 = Decimal.Round(cena6.cena_detaliczna_netto * pozycja6.IloscWJednostcePodstawowej,2); // H
            decimal wartoscBruttoKatalogowa6 = Decimal.Round(wartoscNettoKatalogowa6 * (1 + (stawkaVat6 / 100)),2); // I
            decimal wartoscNettoPrzedRabatem6 = Decimal.Round(pozycja6.IloscWJednostcePodstawowej * cenaNettoPrzedRabatem6,2);
            decimal wartoscBruttoPrzedRabatem6 = Decimal.Round(wartoscNettoPrzedRabatem6 * (1 + (stawkaVat6 / 100)), 2);
            decimal rabatWartosc6 = Decimal.Round(pozycja6.IloscWJednostcePodstawowej * cenaNettoPrzedRabatem6 * ((100 - rabatWyliczony6) / 100), 4);
            decimal cenaNettoPoRabacie6 = Decimal.Round(cenaNettoPrzedRabatem6 * ((100 - rabatWyliczony6) / 100), 2);
            decimal wartoscNettoPoRabacie6 = Decimal.Round(pozycja6.IloscWJednostcePodstawowej * cenaNettoPrzedRabatem6 * ((100 - rabatWyliczony6) / 100), 2);
            decimal wartoscBruttoPoRabacie6 = Decimal.Round(wartoscNettoPoRabacie6 * (1 + (stawkaVat6 / 100)), 2);
            decimal vatWartoscKatalogowa6 = Decimal.Round(wartoscNettoKatalogowa6 * (stawkaVat6 / 100), 2);
            decimal vatWartoscPrzedRabatem6 = Decimal.Round(wartoscNettoPrzedRabatem6 * (stawkaVat6 / 100), 2);
            decimal vatWartoscPoRabacie6 = Decimal.Round(wartoscNettoPoRabacie6 * (stawkaVat6 / 100), 2);

            //////////////////////////////////////////////////////////////////////////////

            
            
            List<IKoszykPozycja> listaPozycji = new List<IKoszykPozycja>(){pozycja1, pozycja2, pozycja3, pozycja4, pozycja5, pozycja6};
            
            A.CallTo(() => koszyk.Pozycje).Returns(listaPozycji);
            return new KoszykiBLL(koszyk);
        }
    }
}
