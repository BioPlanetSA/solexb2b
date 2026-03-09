using System.IO;
using FakeItEasy;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly;
using Xunit;

namespace SolEx.Hurt.Sync.App.Modules_Tests.NoweModuly
{
    public class BazowyPobieraniePlikuNaDyskTests
    {
        [Fact()]
        public void PobierzPlikTest()
        {
            BazowyPobieraniePlikuNaDysk baza = A.Fake<BazowyPobieraniePlikuNaDysk>();
            baza.AdresHttp = "http://wp.pl";
            baza.SciezkaPlikuNaDysku = @"c:/plik-test.txt";

            bool status = baza.PobierzPlik();
            
            Assert.True(status);

            Assert.True(File.Exists(baza.SciezkaPlikuNaDysku) && File.ReadAllText(baza.SciezkaPlikuNaDysku).Contains("Wirtualna Polska"));
        }

    }
}
