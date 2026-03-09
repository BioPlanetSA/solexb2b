using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Core.BLL.RegulyKoszyka;
using Xunit;
namespace SolEx.Hurt.Core.BLL.RegulyKoszyka.Tests
{
    public class PozycjaIloscTests
    {
        [Fact()]
        public void TestWarunkuTest()
        {
         PozycjaIlosc modul=new PozycjaIlosc();

            modul.Minimum = 10;
            modul.Maksimum = 10;
            Assert.True(modul.TestWarunku(10));
            modul.Minimum = null;
            modul.Maksimum = 10;
            Assert.True(modul.TestWarunku(10));

            modul.Minimum = null;
            modul.Maksimum = 11;
            Assert.True(modul.TestWarunku(10));

            modul.Minimum = null;
            modul.Maksimum = 9;
            Assert.False(modul.TestWarunku(10));


            modul.Minimum = null;
            modul.Maksimum = null;
            Assert.True(modul.TestWarunku(10));


            modul.Minimum = 5;
            modul.Maksimum = 20;
            Assert.True(modul.TestWarunku(10));

            modul.Minimum = 5;
            modul.Maksimum = 9;
            Assert.False(modul.TestWarunku(10));
        }
    }
}
