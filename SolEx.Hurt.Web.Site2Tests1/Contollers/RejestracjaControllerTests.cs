using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Model.Web;
using SolEx.Hurt.Web.Site2.Controllers;
using Xunit;

namespace SolEx.Hurt.Web.Site2Tests.Contollers
{
    public class RejestracjaControllerTests
    {
        [Fact(DisplayName = "Test sprawdzający poprawnośc pobierania pól rejestracji (Parametry pola) - nie wszystkie pola są wybrane")]
        public void PobierzPolaTest()
        {
            string[] polaWFormularzuREjestracji = {"Nazwa", "WiadomoscEmail", "AkceptacjaRegulaminu", "Polecajcacy", "KodPocztowy", "PrzetwarzanieDanychOsobowych", "FakturyElektroniczne" };
            RejestracjaController controler = new RejestracjaController();
            List<ParametryPola> wynik = controler.PobierzPola(polaWFormularzuREjestracji);

            //Polecajcacy nie jest polem edytowalnym wiec nie bedzie go w otrzymanej liscie oraz dodatkowe dwa parametry pola dla grup (naglowki)
            Assert.True(wynik.Count==8);
            //Powstały 3 grupy pusta, zamawiajacy oraz adres firmy
            Assert.True(wynik.GroupBy(x=>x.Grupa??"").Count()==3);

            //Polecajcacy - nie edytowalne (olewamy), FakturyElektroniczne-niewymagane (olewamy)
            Assert.True(wynik.Count(x => x.Wymagane) == 5);
        }
    }
}
