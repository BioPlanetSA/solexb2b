using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using Xunit;

namespace SolEx.Hurt.CoreTests.BLL
{
    public class ListyPrzewozoweBllTests
    {
        [Fact()]
        public void TworzenieListuPrzewozowegoTest()
        {
            ListyPrzewozoweBll lp = new ListyPrzewozoweBll();
            List<HistoriaDokumentuListPrzewozowy> wynik;
            string format = "link={0}";
            string nrDoTestow = " 554; 34";

            wynik = lp.TworzenieListuPrzewozowego(1,nrDoTestow,format);
            Assert.True(wynik.Count ==2);
            Assert.True(wynik.Any(x=>x.NumerListu.Equals("554",StringComparison.InvariantCultureIgnoreCase)));
            Assert.True(wynik.Any(x => x.NumerListu.Equals("34", StringComparison.InvariantCultureIgnoreCase)));

            nrDoTestow = "56 66 6344";
            wynik = lp.TworzenieListuPrzewozowego(1, nrDoTestow, format);
            Assert.True(wynik.Count == 3);
            Assert.True(wynik.Any(x => x.NumerListu.Equals("56", StringComparison.InvariantCultureIgnoreCase)));
            Assert.True(wynik.Any(x => x.NumerListu.Equals("66", StringComparison.InvariantCultureIgnoreCase)));
            Assert.True(wynik.Any(x => x.NumerListu.Equals("6344", StringComparison.InvariantCultureIgnoreCase)));

            nrDoTestow = "345634,534534";
            wynik = lp.TworzenieListuPrzewozowego(1, nrDoTestow, format);
            Assert.True(wynik.Count == 2);
            Assert.True(wynik.Any(x => x.NumerListu.Equals("345634", StringComparison.InvariantCultureIgnoreCase)));
            Assert.True(wynik.Any(x => x.NumerListu.Equals("534534", StringComparison.InvariantCultureIgnoreCase)));
        }
    }
}