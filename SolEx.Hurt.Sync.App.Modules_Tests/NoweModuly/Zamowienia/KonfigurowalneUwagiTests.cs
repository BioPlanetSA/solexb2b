using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Zamowienia;
using Xunit;

namespace SolEx.Hurt.Sync.App.Modules_Tests.NoweModuly.Zamowienia
{
    public class KonfigurowalneUwagiTests
    {
        [Fact(DisplayName = "Sprawdzanie poprawności odczytu cechy rozbicia z nr zamówienia")]
        public void PobierzCecheRozbiciaTest()
        {
            KonfigurowalneUwagi uwagi = new KonfigurowalneUwagi {Filtr = " [A-Z]+ "};
            string wynik = uwagi.Oczysc("O - WROS (AC)");
            Assert.Equal(wynik, "WROS");
          
        }
    }
}
