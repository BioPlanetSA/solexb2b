using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Model.Enums;
using Xunit;

namespace SolEx.Hurt.CoreTests.BLL.TestKoszyka
{
    public class PodsumowanieKoszykTest
    {
        [Fact(DisplayName = "Sprawdzenie poprawnosci wyliczanych cen w koszyku")]
        public void PodsumowanieTest()
        {
            SztucznyObiekKoszyka k = new SztucznyObiekKoszyka();
            var a = k.StworzKoszyk();

            

            //Wartosc katalogowa 
            Assert.True(Decimal.Round(a.CalkowitaWartoscCenaKatalogowaNetto, 2) == 164023.19m);     
            Assert.True(Decimal.Round(a.CalkowitaWartoscCenaKatalogowaBrutto, 2) == 181496.25m );   
            Assert.True(Decimal.Round(a.WartoscVatCenaKatalogowa, 2) == 17473.06m);                         

            //Wartosc przed rabatem
            Assert.True(Decimal.Round(a.CalkowitaWartoscHurtowaNetto, 2) == 82500.92m);
            Assert.True(Decimal.Round(a.CalkowitaWartoscHurtowaBrutto, 2) == 91351.04m); 
            Assert.True(Decimal.Round(a.WartoscVatCenaHurtowa.Wartosc, 2) == 8850.12m );           

            //Wartosc po rabacie
            Assert.True(Decimal.Round(a.CalkowitaWartoscHurtowaNettoPoRabacie, 2) == 81663.39m);
            Assert.True(Decimal.Round(a.CalkowitaWartoscHurtowaBruttoPoRabacie, 2) == 90371.51m);
            Assert.True(Decimal.Round(a.VatRabat, 2) == 8708.12m);      

            //Zysk klienta
            Assert.True(Decimal.Round(a.CalkowitaWartoscCenaKatalogowaNettoZysk, 2) == 82359.80m);         
            Assert.True(Decimal.Round(a.CalkowitaWartoscCenaKatalogowaBruttoZysk, 2) == 91124.74m);          
            Assert.True(Decimal.Round(a.VatZyskKlienta, 2) == 8764.94m);
            //Assert.True(Decimal.Round(%, 2) == 50.2100m);  
            
            //Rabat
            //Assert.True(Decimal.Round(NETTO, 2) == 837.53m);
            //Assert.True(Decimal.Round(BRUTTO, 2) == 979.53m);
            //Assert.True(Decimal.Round(VAT, 2) == 142);  
            //Assert.True(Decimal.Round(%, 2) == 1.0200m); 

        }
    }
}
