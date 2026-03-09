using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Model.Helpers;
using Xunit;
namespace SolEx.Hurt.Model.Helpers.Tests
{
    public class OperacjeZbiorachTests
    {
        List<HashSet<int>> kolekcja = new List<HashSet<int>>() {
            new HashSet<int>() { 1, 2, 3, 999, 4, 5,10 },
            new HashSet<int>() { 1, 10, 2, 3, 4, 888, 5 },
            new HashSet<int>() { 1, 2, 3, 10, 4, 666, 5 }, 
            new HashSet<int>() { 1, 2, 3, 4, 777, 5,10 } 
};

        [Fact()]
        public void PobierzUnikalneElementyTest()
        {
                var wynik = OperacjeZbiorach.PobierzUnikalneElementy(kolekcja);
                HashSet<int> spodziewanyWynik = new HashSet<int>{999,888,666,777};

                Assert.True(spodziewanyWynik.Count == wynik.Count, "Różne wyniki cech unikalnych");

                Assert.True(!spodziewanyWynik.Except(wynik).Any(), "Różne wyniki cech unikalnych");
        }

        [Fact()]
        public void PobierzPowtarzajaceElementyTest()
        {
            var wynik = OperacjeZbiorach.PobierzPowtarzajaceElementy(kolekcja);
            HashSet<int> spodziewanyWynik = new HashSet<int> { 1,2,3,4,5,10 };

            Assert.True(spodziewanyWynik.Count == wynik.Count, "Różne wyniki cech powtarzajacych sie");

            Assert.True(!spodziewanyWynik.Except(wynik).Any(), "Różne wyniki cech powtarzajacych sie");
        }



    }
}
