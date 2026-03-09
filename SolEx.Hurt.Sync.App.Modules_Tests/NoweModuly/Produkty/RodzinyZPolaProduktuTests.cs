using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty.Tests
{
    public class RodzinyZPolaProduktuTests
    {
        [Fact()]
        public void NowaNazwaTest()
        {
            RodzinyZPolaProduktu r = new RodzinyZPolaProduktu();
            string n1 = r.NowaNazwa("RASHGUARD SMMASH BLACK JACK - LS");
            string n2 = r.NowaNazwa("RASHGUARD SMMASH BLACK JACK- LS");
            string n3 = r.NowaNazwa("RASHGUARD SMMASH BLACK JACK -LS");

            Assert.Equal(n1, n2);
            Assert.Equal(n2, n3);
        }
    }
}
