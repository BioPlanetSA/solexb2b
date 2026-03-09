using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Helpers;
using Xunit;
namespace SolEx.Hurt.Helpers.Tests
{
    public class JSonHelperTests
    {
        public class Test
        {
            public bool Pole { get; set; }
        }
        [Fact()]
        public void SerializeTest()
        {
            Test t=new Test{Pole = true};
            var wynik = JSonHelper.Serialize(t);
        }

        [Fact()]
        public void SerializeTest1()
        {
            DateTime data = DateTime.Now;


            JsonSerializerSettings sett = new JsonSerializerSettings();
 

            string ser = JsonConvert.SerializeObject(data, sett);
         //     ser = "\"2015-08-19T09:20:59.4295+01:00\"";
                    var d1= JsonConvert.DeserializeObject<DateTime>(ser, sett);
        }

        [Fact()]
        public void SerializeTest2()
        {
          Produkt p=new Produkt(){IloscMinimalna = 0.3M};
            List<Produkt> produkty=new List<Produkt>{p};
            string set = JSonHelper.Serialize(produkty);
        }
    }
}
