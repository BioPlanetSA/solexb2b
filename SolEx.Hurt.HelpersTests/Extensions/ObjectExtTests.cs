using System.Collections.Generic;
using SolEx.Hurt.Model;
using Xunit;
namespace System.Tests
{
    public class ObjectExtTests
    {
        [Fact]
        public void RownyWartosciDomyslnejTest()
        {
            Assert.Equal(false, 3.RownyWartosciDomyslnej());
            Assert.Equal(true, 0.RownyWartosciDomyslnej());
            int? test = null;
            Assert.Equal(true, test.RownyWartosciDomyslnej());
            test = 3;
            Assert.Equal(false, test.RownyWartosciDomyslnej());
            Produkt p = new Produkt();
            Assert.Equal(false, p.RownyWartosciDomyslnej());

            Assert.Equal(false, (3.0).RownyWartosciDomyslnej());
            Assert.Equal(true, (0.0).RownyWartosciDomyslnej());

            Assert.Equal(false, "aaa".RownyWartosciDomyslnej());
            Assert.Equal(true, "".RownyWartosciDomyslnej());
        }


        [Fact(DisplayName = "Test sprawdzający poprawne działanie metody sprawdzjącej duble")]
        public void SprawdzDubleTest()
        {
            List<Cecha>cechy = new List<Cecha>();
            Cecha c1 = new Cecha("Towary wszystkie\\karnisze", "kategoria:towary wszystkie\\karnisze");
            c1.Id = 1;

            Cecha c2 = new Cecha("Towary wszystkie\\karnisze", "kategoria:towary wszystkie\\karnisze");
            c2.Id = 2;

            cechy.Add(c1);
            cechy.Add(c2);

            Cecha cc = new Cecha();
            try
            {
                cechy.SprawdzDuble(new {cc.Symbol});
                Assert.True(false);
            }
            catch (Exception )
            {
                Assert.True(true);
            }
           

        }
    }
}
