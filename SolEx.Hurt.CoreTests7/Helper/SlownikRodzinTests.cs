using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Model;
using Xunit;
namespace SolEx.Hurt.Core.Helper.Tests
{
    public class SlownikRodzinTests
    {
        [Fact(DisplayName = "Test sprawdzający poprawność budowania słownika rodzin")]
        public void PobierzRodzinySlownikTest()
        {
            produkty p1 = new produkty(){produkt_id = 1, rodzina = "aaaa"};
            produkty p2 = new produkty(){produkt_id = 2, rodzina = "bbbb"};
            produkty p3 = new produkty(){produkt_id = 3, rodzina = "aaaa"};
            produkty p4 = new produkty(){produkt_id = 4, rodzina = "bbbb"};
            produkty p5 = new produkty(){produkt_id = 5, rodzina = ""};
            produkty p6 = new produkty(){produkt_id = 6, rodzina = null};
            List<produkty> listaProduktow = new List<produkty>(){p1,p2,p3,p4,p5,p6};

            var wynik= SlownikRodzin.PobierzRodzinySlownik(listaProduktow);
            Assert.True(wynik.Count==2);
            Assert.True(wynik["aaaa"].Count == 2);
            Assert.True(wynik["bbbb"].Count == 2);
        } 
    }
}
