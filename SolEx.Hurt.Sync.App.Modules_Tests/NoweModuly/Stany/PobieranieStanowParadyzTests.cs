using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Stany;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Stany.Tests
{
    public class PobieranieStanowParadyzTests
    {
        [Fact()]
        public void PobierzStanyParadyzTest()
        {
            PobieranieStanowParadyz StanyParadyz = A.Fake<PobieranieStanowParadyz>();

            StanyParadyz.Login = "SATURN";
            StanyParadyz.Haslo = "Saturn2013!";

            Dictionary<int,List<ProduktStan>> t = new Dictionary<int, List<ProduktStan>>();

            StanyParadyz.Przetworz(ref t,null, null);

            Assert.True(File.Exists(StanyParadyz.SciezkaPlikuNaDysku) && File.ReadAllText(StanyParadyz.SciezkaPlikuNaDysku).Contains("EAN"));
        }
    }
}
