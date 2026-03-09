using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Core;
using Xunit;
namespace SolEx.Hurt.Core.Tests
{
    public class LinkBuilderTests
    {
        [Fact()]
        public void SprawdzLinkTest()
        {
              LinkTest("http://onet.pl","http://onet.pl");
              LinkTest("www.onet.pl", "http://www.onet.pl");
              LinkTest("test", "/test");
              LinkTest("/test", "/test");

        }
        private static void LinkTest(string zrodlo, string wynik)
        {
            string w = LinkBuilder.PobierzInstancje.SprawdzLink(zrodlo);
            Assert.True(w==wynik,string.Format("Zrodlo {0}, oczekiwano {1} otrzymano {2}",zrodlo,wynik,w));
        }
    }
}
