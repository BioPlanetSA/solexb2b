using Xunit;
using SolEx.Hurt.Web.Site2.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolEx.Hurt.Web.Site2.Controllers.Tests
{
    public class AdminControllerTests
    {
        [Fact(DisplayName = "Test metody testującej poprawności parametrów kontrolek")]
        public void TestujZgodnoscParametrowKontrolkiIKontroleraAkcjiTest()
        {
            AdminController admin = new AdminController();

            Dictionary<string, object> slownikParametrow = new Dictionary<string, object>
            {
                {"id", "idPowinnoBycIntowe"},
                {"nowy3", "wartoscjakasdodatkowao nadmiarowa"}
            };

            try
            {
                admin.TestujZgodnoscParametrowKontrolkiIKontroleraAkcji(slownikParametrow, "admin", "dodajWiersz");
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("id"))
                {
                    throw new Exception("Nie wystapił wyjątek czyli test NIE przeszedł - powinien się wywalić błąd bo parametry są niezgodne  - id jest stringowe a powinno być numeryczne");
                }
            }
        }


        [Fact(DisplayName = "Test metody testującej poprawności parametrów kontrolek2")]
        public void TestujZgodnoscParametrowKontrolkiIKontroleraAkcjiTest2()
        {
            AdminController admin = new AdminController();

            Dictionary<string, object> slownikParametrow = new Dictionary<string, object>
            {
                {"id", "56"},
                {"nowy3", "wartoscjakasdodatkowao nadmiarowa"}
            };

            try
            {
                admin.TestujZgodnoscParametrowKontrolkiIKontroleraAkcji(slownikParametrow, "admin", "dodajWiersz");
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("id"))
                {
                    throw new Exception("Nie wystapił wyjątek czyli test NIE przeszedł - powinien się wywalić błąd bo parametry są niezgodne  - id jest stringowe a powinno być numeryczne");
                }
            }
        }

        [Fact(DisplayName = "Test metody testującej poprawności parametrów kontrolek 3")]
        public void TestujZgodnoscParametrowKontrolkiIKontroleraAkcjiTest3()
        {
            AdminController admin = new AdminController();

            Dictionary<string, object> slownikParametrow = new Dictionary<string, object>
            {
                {"id", "56"},
                {"nowy3", "wartoscjakasdodatkowao nadmiarowa"},
                {"szablon", "szs43f" },
                {"GradacjaSposobPokazywania", "67" },
                {"lvat", "True" },
                {"strona", 78 }
            };

            try
            {
                admin.TestujZgodnoscParametrowKontrolkiIKontroleraAkcji(slownikParametrow, "produkty", "Lista");
            }
            catch (Exception ex)
            {
                throw ex;
            }


            //teraz ma nie przejsc bo inty pozamieione
            slownikParametrow["strona"] = "gfh4";


            try
            {
                admin.TestujZgodnoscParametrowKontrolkiIKontroleraAkcji(slownikParametrow, "produkty", "Lista");
            }
            catch
            {
                return;
            }
            throw new Exception("Nie wystapił wyjątek czyli test NIE przeszedł ");
        }




    }
}