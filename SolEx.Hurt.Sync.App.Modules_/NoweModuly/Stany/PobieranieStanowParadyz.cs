using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Stany
{
    public class PobieranieStanowParadyz : BazowyPobieraniePlikuNaDysk, IModulStany
    {
        public PobieranieStanowParadyz()
        {
            AdresHttp = "https://platforma.paradyz.com.pl/eksport_csv_full?pacp_new_collection=&order_field=pacp_ean&order_type=asc";
            SciezkaPlikuNaDysku = @"c:\stany_paradyz.csv";
            AdresLogowaniaHttp = "https://platforma.paradyz.com.pl/ajaxindex.php?cp_login={0}&cp_pass={1}&page=logowanie&start=submitUserLogin";
        }

        public void Przetworz(ref Dictionary<int,List<ProduktStan>> listaWejsciowa, List<Magazyn> magazyny, List<Produkt>produkty )
        {
            PobierzStrone();

            /*
             * Inne strony:
             * 
             * 
             * 
             * 
             */

            //  var _driver = new HtmlUnitDriver(true);
            ////  var driver = new NHtmlUnit.WebClient();// new NHtmlUnit.Html.u HtmlUnitDriver(true);
            //  driver.get Url = @"http://www.facebook.com/login.php";

            //  var email = driver.FindElement(By.Name("email"));
            //  email.SendKeys("some@email.com");

            //  var pass = driver.FindElement(By.Name("pass"));
            //  pass.SendKeys("xxxxxxxx");

            //  var inputs = driver.FindElements(By.TagName("input"));
            //  var loginButton = (from input in inputs
            //                     where input.GetAttribute("value").ToLower() == "login"
            //                     && input.GetAttribute("type").ToLower() == "submit"
            //                     select input).First();
            //  loginButton.Click();

            //  driver.Url = @"https://m.facebook.com/profile.php?id=1111111111";
            //  Assert.That(driver.Title, Is.StringContaining("Title of page goes here"));
            //  //PobierzPlik();
        }
    }
}
