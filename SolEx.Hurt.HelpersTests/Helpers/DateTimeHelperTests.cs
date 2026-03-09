using System;
using SolEx.Hurt.Model.Enums;
using Xunit;
namespace SolEx.Hurt.Model.Helpers.Tests
{
    public class DateTimeHelperTests
    {
        [Fact()]
        public void WyliczDateTest()
        {
           DateTime wynik= DateTimeHelper.PobierzInstancje.WyliczDate(DataOdKiedyLiczyc.PoczatekRoku, "");
           Assert.Equal(new DateTime(DateTime.Now.Year,1,1),wynik);
           wynik = DateTimeHelper.PobierzInstancje.WyliczDate(DataOdKiedyLiczyc.OstatniDzienRoku, "");
           Assert.Equal(new DateTime(DateTime.Now.Year,12, 31), wynik);
           wynik = DateTimeHelper.PobierzInstancje.WyliczDate(DataOdKiedyLiczyc.BiezacyDzien, "");
           Assert.Equal(DateTime.Now.Date, wynik);
           wynik = DateTimeHelper.PobierzInstancje.WyliczDate(DataOdKiedyLiczyc.WybranejDaty, "10.11.2011");
           Assert.Equal(new DateTime(2011,11,10), wynik);
        }
    }
}
