using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Model.Core;
using Xunit;
namespace SolEx.Hurt.Model.Core.Tests
{
    public class WartoscLiczbowaTests
    {
        [Fact(DisplayName = "separator dla tysiecy, czy poprawnie formatuje liczby - spacja odzielająca tyisące od setek")]
        public void ToStringTest()
        {
            decimal d = (decimal) 999999.9911;
            WartoscLiczbowa wartosc = new WartoscLiczbowa(d);
            string wyjscie = wartosc.ToString();
            Assert.Equal("999 999,99", wyjscie);
        }
    }
}
