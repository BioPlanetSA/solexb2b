using System;
using System.Collections.Generic;
using Xunit;

namespace SolEx.Hurt.Sync.ProviderSubiekt.Tests
{
    public class MainDAOTests
    {
        [Fact(DisplayName = "Wyliczanie terminu rezerwacji")]
        public void WyliczTerminDostawyTest()
        {

          var wynik=  MainDAO.WyliczTerminDostawy(1, WygenerujDane(), 0, 0);
            Assert.True(wynik==DateTime.Now.Date);
           wynik = MainDAO.WyliczTerminDostawy(2, WygenerujDane(), 0, 0);
            Assert.Null(wynik);
            wynik = MainDAO.WyliczTerminDostawy(1, WygenerujDane(), 1, 0);
            Assert.True(wynik == DateTime.Now.Date.AddDays(3));
            wynik = MainDAO.WyliczTerminDostawy(1, WygenerujDane(), 2, 0);
            Assert.True(wynik == DateTime.Now.Date.AddDays(3));
            wynik = MainDAO.WyliczTerminDostawy(1, WygenerujDane(), 3, 5);
            Assert.Null(wynik);
            wynik = MainDAO.WyliczTerminDostawy(1, WygenerujDane(), 50, 5);
            Assert.Null(wynik);

        }

        private Dictionary<int, List<Tuple<decimal, DateTime>>> WygenerujDane()
        {
            Dictionary<int, List<Tuple<decimal, DateTime>>> wynik=new Dictionary<int, List<Tuple<decimal, DateTime>>>();
            wynik.Add(1,new List<Tuple<decimal, DateTime>>
            {
                new Tuple<decimal, DateTime>(1,DateTime.Now.Date),
                new Tuple<decimal, DateTime>(3,DateTime.Now.Date.AddDays(3))
            ,new Tuple<decimal, DateTime>(5,DateTime.Now.Date.AddDays(5))
            });
            return wynik;
        }
    }
}
