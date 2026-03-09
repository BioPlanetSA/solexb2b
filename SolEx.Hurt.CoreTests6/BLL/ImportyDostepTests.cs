using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.Importy;
using SolEx.Hurt.Model;
using Xunit;
namespace SolEx.Hurt.Core.BLL.Tests
{
    public class ImportyDostepTests
    {
        [Fact()]
        public void WyfiltrujDaneMajacePusteWymaganePolaTest()
        {
         List<produkty> produkty=new List<produkty>();
            produkty.Add(new produkty{produkt_id = 1,nazwa = ""});
            produkty.Add(new produkty { produkt_id =2, nazwa = "nazwa" });
            List<WierszMapowania> mapowania = new List<WierszMapowania>
            {
                new WierszMapowania {Wymagane = true, Widoczne = true, Nazwa = "produkt_id", LadnaNazwa = "id", Pole = "produkt_id"},
                new WierszMapowania {Wymagane = true, Widoczne = true, Nazwa = "nazwa", LadnaNazwa = "name", Pole = "nazwa"}
            };
            List<object> dane = new List<object>(produkty);
            ImportyDostep.PobierzInstancje.WyfiltrujDaneMajacePusteWymaganePola(dane, new List<WierszMapowania>{mapowania[0],mapowania[1]});
            Assert.Equal(1, dane.Count);

            dane = new List<object>(produkty);
            ImportyDostep.PobierzInstancje.WyfiltrujDaneMajacePusteWymaganePola(dane, new List<WierszMapowania> { mapowania[0] });
            Assert.Equal(2, dane.Count);
        }
    }
}
