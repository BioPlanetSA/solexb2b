using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;
namespace SolEx.Hurt.Core.Helper.Tests
{
    public class EppHelperTests
    {
        [Fact(DisplayName = "Test sprawdzajacy poprawność generowania sekcji Towary")]
        public void WygenrujSekcjeTowaryTest()
        {
            EppHelper.TowarDoGeneracjiEpp towar = new EppHelper.TowarDoGeneracjiEpp();
            towar.Kod = "JakisKod";
            towar.SymbolTowaruDostawcy = "JakisSymbolTowaruDostawcy";
            towar.KodKreskowy = "JakisKodKreskowy";
            towar.Nazwa = "JakasNazwa";
            towar.Opis = "JakisOpis";
            towar.PKWIU = "JakiesPKWIU";
            towar.Jednostka = "Jakas jednostka";
            towar.StawkaVat =((int) 33).ToString();
            towar.Uwagi = "JakiesUwagi";
            towar.SymbolDostawcy = "JakisSymbolDostawcy";
            
            List<EppHelper.TowarDoGeneracjiEpp> listaTowarow = new List<EppHelper.TowarDoGeneracjiEpp>(){towar};
            EppHelper eh = new EppHelper();
            string wynik= eh.WygenrujSekcjeTowary(listaTowarow);

            string[] podzielone = wynik.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);


            Assert.True(podzielone[1] == Parametr(towar.Kod), string.Format("Otrzymano {0}, Oczekiwano:{1}", podzielone[1], Parametr(towar.Kod)));
            Assert.True(podzielone[2] == Parametr(towar.SymbolTowaruDostawcy), string.Format("Otrzymano {0}, Oczekiwano:{1}", podzielone[2], Parametr(towar.SymbolTowaruDostawcy)));
            Assert.True(podzielone[3] == Parametr(towar.KodKreskowy), string.Format("Otrzymano {0}, Oczekiwano:{1}", podzielone[3], Parametr(towar.KodKreskowy)));
            Assert.True(podzielone[4] == Parametr(towar.Nazwa), string.Format("Otrzymano {0}, Oczekiwano:{1}", podzielone[4], Parametr(towar.Nazwa)));
            Assert.True(podzielone[5] == Parametr(towar.Opis), string.Format("Otrzymano {0}, Oczekiwano:{1}", podzielone[5], Parametr(towar.Opis)));
            Assert.True(podzielone[6] == Parametr(""));
            Assert.True(podzielone[7] == Parametr(""));
            Assert.True(podzielone[8] == Parametr(towar.PKWIU));
            Assert.True(podzielone[9] == Parametr(towar.Jednostka));
            Assert.True(podzielone[10] == Parametr(towar.StawkaVat.ToString(CultureInfo.InvariantCulture)), string.Format("Oczekiwano: {0}, otrzymano{1}", podzielone[10], Parametr(towar.StawkaVat.ToString(CultureInfo.InvariantCulture))));
            Assert.True(podzielone[11] == towar.StawkaVat.ToString(CultureInfo.InvariantCulture), string.Format("Oczekiwano: {0}, otrzymano{1}", podzielone[11], towar.StawkaVat.ToString(CultureInfo.InvariantCulture)));
            Assert.True(podzielone[27] == Parametr(towar.Uwagi));
            
        }

        public string Parametr(string param)
        {
            return string.Format("\"{0}\"", param);
        }

        [Fact(DisplayName = "Test sprawdzajacy poprawnosc dzialania metody Wygeneruj sekcje ceny")]
        public void WygerujSekcjeCenyTest()
        {
            EppHelper.TowarDoGeneracjiEpp towar = new EppHelper.TowarDoGeneracjiEpp();

            towar.PoziomCenowyDetalicznaNetto = ((decimal)543545.43).ToString();
            towar.PoziomCenowyDetalicznaBrutto = ((decimal) 543534.56).ToString();
            towar.PoziomCenowyHurtowaBrutto = ((decimal)89.56).ToString();
            towar.PoziomCenowyHurtowaNetto = ((decimal)34.56).ToString();
            towar.PoziomCenowyZakupBrutto = ((decimal) 43543.54).ToString();
            towar.PoziomCenowyZakupNetto = ((decimal)3543.54).ToString();
            
            List<EppHelper.TowarDoGeneracjiEpp> listaTowarow = new List<EppHelper.TowarDoGeneracjiEpp>(){towar};
            EppHelper eh = new EppHelper();
            string wynik = eh.WygerujSekcjeCeny(listaTowarow);
            string[] podzielone = wynik.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            Assert.True(podzielone[1] == Parametr("Hurtowa-B2B"), string.Format("Otrzymano {0}, Oczekiwano:\"Hurtowa-B2B\"", podzielone[1]));
            Assert.True(podzielone[2] == towar.PoziomCenowyHurtowaNetto.ToString().Replace(",","."), string.Format("Otrzymano {0}, Oczekiwano:{1}", podzielone[2], towar.PoziomCenowyHurtowaNetto));
            Assert.True(podzielone[3] == towar.PoziomCenowyHurtowaBrutto.ToString().Replace(",", "."), string.Format("Otrzymano {0}, Oczekiwano:{1}", podzielone[3], towar.PoziomCenowyHurtowaBrutto));


            Assert.True(podzielone[7] == Parametr("Detaliczna-B2B"), string.Format("Otrzymano {0}, Oczekiwano:\"Detaliczna-B2B\"", podzielone[7]));
            Assert.True(podzielone[8] == towar.PoziomCenowyDetalicznaNetto.ToString().Replace(",", "."), string.Format("Otrzymano {0}, Oczekiwano:{1}", podzielone[8], towar.PoziomCenowyDetalicznaNetto));
            Assert.True(podzielone[9] == towar.PoziomCenowyDetalicznaBrutto.ToString().Replace(",", "."), string.Format("Otrzymano {0}, Oczekiwano:{1}", podzielone[9], towar.PoziomCenowyDetalicznaBrutto));

            Assert.True(podzielone[13] == Parametr("Zakup-B2B"), string.Format("Otrzymano {0}, Oczekiwano:\"Zakup-B2B\"", podzielone[13]));
            Assert.True(podzielone[14] == towar.PoziomCenowyZakupNetto.ToString().Replace(",", "."), string.Format("Otrzymano {0}, Oczekiwano:{1}", podzielone[14], towar.PoziomCenowyZakupNetto));
            Assert.True(podzielone[15] == towar.PoziomCenowyZakupBrutto.ToString().Replace(",", "."), string.Format("Otrzymano {0}, Oczekiwano:{1}", podzielone[15], towar.PoziomCenowyZakupBrutto));
           
        }
    }
    
}
