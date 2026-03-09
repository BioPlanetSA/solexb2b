using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.RegulyPokazywaniaStanow;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using Xunit;
namespace SolEx.Hurt.Core.RegulyPokazywaniaStanow.Tests
{
    public class ProstaRegulyPokazywaniaStanowTests
    {
        [Fact(DisplayName = "Test warunku cyklicznej dostawy")]
        public void SpelniaWarunekCyklicznejDostawyTest()
        {
         Test(true,null,SposobyPokazyaniaStanowCyklicznaDostawa.Obojetnie);
         Test(true, DateTime.Now.AddDays(1), SposobyPokazyaniaStanowCyklicznaDostawa.Obojetnie);
         Test(true, null, SposobyPokazyaniaStanowCyklicznaDostawa.NiePosiada);
         Test(false, DateTime.Now.AddDays(1), SposobyPokazyaniaStanowCyklicznaDostawa.NiePosiada);
         Test(false, null, SposobyPokazyaniaStanowCyklicznaDostawa.Posiada);
         Test(true, DateTime.Now.AddDays(1), SposobyPokazyaniaStanowCyklicznaDostawa.Posiada);
        }

        public void Test(bool oczekiwany, DateTime? najblizsza, SposobyPokazyaniaStanowCyklicznaDostawa regula)
        {
               sposoby_pokazywania_stanow_reguly r=new sposoby_pokazywania_stanow_reguly();
               r.parametry = string.Format("1;1;1;1;1;1;{0};", (int)regula);
            ProstaRegulyPokazywaniaStanow modul=new ProstaRegulyPokazywaniaStanow(r);
            var p = A.Fake<ProduktBazowy>();
            A.CallTo(() => p.NajblizszaDostawa).Returns(najblizsza);
            bool wynik = modul.SpelniaWarunekCyklicznejDostawy(p);
            Assert.True(wynik==oczekiwany,string.Format("Wynik {0} oczekiwany {1}, datsa {2}, regula {3}",wynik,oczekiwany,najblizsza,regula));
        }
    }
}
