using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using Xunit;
namespace SolEx.Hurt.Core.Tests
{
    public class TemplateParserTests
    {

           [Fact(DisplayName = "Parsowanie szablonów")]
        public void ParseTest1()
        {
          //  var ConfigBllFake = A.Fake<IConfigBLL>();
          //  var CacheBllFake = A.Fake<ICacheBll>();
          //  TemplateParser.ConfigBll = ConfigBllFake;
          //  TemplateParser.CacheBll = CacheBllFake;

          //  var tempParser = A.Fake<TemplateParser>();

          //  klienci baza = new klienci { nazwa = "test", symbol = "symbol test" };
          //  Klient klient = new Klient(baza);

          //  List<object> data = new List<object>();
          //  data.Add(klient);
          //     IDictionary<string, object> expando = new ExpandoObject();
          //     foreach (object ob in data.Where(x => x != null))
          //     {
          //         Type obType = ob.GetType();
          //         expando.Add(obType.Name, ob);
          //     }
            
          //  string szablon = "Witaj @Model.Klient.nazwa @Model.Klient.symbol";
            
          ////  A.CallTo(() => tempParser.SzablonKluczCache(szablon)).Returns("klucz");
          //  A.CallTo(() => CacheBllFake.PobierzObiekt<Type>("klucz")).Returns(null);
          //  string wynik = tempParser.Parse(szablon, expando);

          //  string oczekiwane = szablon.Replace("@Model.Klient.nazwa", klient.nazwa).Replace("@Model.Klient.symbol", klient.symbol);
          //  Assert.Equal(oczekiwane, wynik);
        }
    }


}
