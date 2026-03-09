using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using Xunit;
namespace SolEx.Hurt.Core.BLL.Tests
{
    public class RejestracjeBllTests
    {
        [Fact(DisplayName = "Test dostepu do rejestracji")]
        public void MaDostepDoRejestracjiTest()
        {
        
          TestDostepu(0,null,true,true);
          TestDostepu(0, 1, false, false);
          TestDostepu(0, 1, true, true);
          TestDostepu(1, 1, false, true);
        }

        private void TestDostepu(int idodzk, int? idodzir, bool admin,bool oczekiwant)
        {
            IKlient k = A.Fake<IKlient>();
            A.CallTo(() => k.OddzialDoJakiegoNalezyKlient).Returns(idodzk);
            Rejestracje r = new Rejestracje();
            r.IdOddzialu = idodzir;
            A.CallTo(() => k.CzyAdministrator).Returns(admin);
            bool wynik = RejestracjeBll.PobierzInstancje.MaDostepDoRejestracji(r, k);
            Assert.Equal(oczekiwant,wynik);
        }
    }
}
