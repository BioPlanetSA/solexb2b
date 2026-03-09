using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.Interfaces;
using Xunit;
namespace SolEx.Hurt.Core.ModelBLL.Tests
{
    public class RabatBLLTests
    {
        private RabatBLL Testowy()
        {
            RabatBLL rabat = new RabatBLL();
            rabat.wartosc1 = 10;
            rabat.wartosc2 = 11;
            rabat.wartosc3 = 12;
            rabat.wartosc4 = 13;
            rabat.wartosc5 = 14;
            return rabat;
        }
        [Fact]
        public void PobierzWartoscRabatuTest()
        {
            IConfigBLL configbll = A.Fake<IConfigBLL>();
            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            A.CallTo(() => calosc.Konfiguracja).Returns(configbll);
    
            RabatyBll konfekcje = A.Fake<RabatyBll>(x => x.WithArgumentsForConstructor(new[] { calosc }));
            
            RabatBLL rabat = Testowy();

            A.CallTo(() => configbll.PrzedzialyCenowe).Returns("");
            decimal? wynik = rabat.PobierzWartoscRabatu(100.01M);
            Assert.Equal(10, wynik);

            A.CallTo(() => configbll.PrzedzialyCenowe).Returns("10;15;20");

            decimal? wartosc = rabat.PobierzWartoscRabatu(konfekcje.PobierzPrzedzialyCenowe()[0] - 1);
            Assert.Equal(10, wartosc);
            decimal? wartosc2 = rabat.PobierzWartoscRabatu(konfekcje.PobierzPrzedzialyCenowe()[0]);
            Assert.Equal(11, wartosc2);
            decimal? wartosc3 = rabat.PobierzWartoscRabatu(konfekcje.PobierzPrzedzialyCenowe()[0] + 0.01M);
            Assert.Equal(11, wartosc3);

        }

        [Fact()]
        public void PobierzWartoscPrzedzialuTest()
        {
            RabatBLL rabat = Testowy();
            decimal? wartosc = rabat.PobierzWartoscPrzedzialu(1);
            Assert.Equal(10, wartosc);
            wartosc = rabat.PobierzWartoscPrzedzialu(2);
            Assert.Equal(11, wartosc);
            wartosc = rabat.PobierzWartoscPrzedzialu(3);
            Assert.Equal(12, wartosc);
            wartosc = rabat.PobierzWartoscPrzedzialu(4);
            Assert.Equal(13, wartosc);
            wartosc = rabat.PobierzWartoscPrzedzialu(5);
            Assert.Equal(14, wartosc);
        }
    }
}
