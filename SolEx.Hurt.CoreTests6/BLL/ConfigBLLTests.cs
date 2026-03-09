using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using Xunit;
namespace SolEx.Hurt.Core.BLL.Tests
{
    public class ConfigBLLTests
    {
        [Fact(DisplayName = "Wyliczanie indywidualnej stawki vat")]
        public void SynchronizacjaPobierzPoleIndywidualnaStawaVatTest()
        {
            string domyslne = "";
            Dictionary<string,string> pars=new Dictionary<string, string>();
            ConfigBLL c = A.Fake<ConfigBLL>();
            A.CallTo(() => c.PobierzSynchronizacjaPoleUstawienie("IndywidualnaStawaVat", domyslne, pars, ustawieniaGrupa.Klienci, A<string>.Ignored,null)).Returns("");

            klienci k=new klienci();
            c.SynchronizacjaPobierzPoleIndywidualnaStawaVat(k,domyslne,pars,true,true);

            Assert.Equal(k.IndywidualnaStawaVat,0);
            c.SynchronizacjaPobierzPoleIndywidualnaStawaVat(k, domyslne, pars, false, true);

            Assert.Equal(k.IndywidualnaStawaVat, null);
            c.SynchronizacjaPobierzPoleIndywidualnaStawaVat(k, domyslne, pars, true, false);

            Assert.Equal(k.IndywidualnaStawaVat, 0);
            c.SynchronizacjaPobierzPoleIndywidualnaStawaVat(k, domyslne, pars, false, false);

            Assert.Equal(k.IndywidualnaStawaVat, 0);
        }
    }
}
