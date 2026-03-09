using System.Linq;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.Helpers.Tests
{
    public class KategorieKlientowHelperTests
    {
        [Fact(DisplayName = "Test poprawności metody sprawdzajacej czy cecha posiada wiecej niż jeden separator")]
        public void SprawdzCzyPoprawnySeparatorTest()
        {
            string cecha1 = "b2b_pracownik:Romek";
            string cecha2 = "b2b-pracownik:Romek";
            string cecha3 = "b2b:pracownik:Romek";
            string cecha4 = "b2b-pracownik-Romek";
            char[] separator = ":_".ToArray();

            Assert.False(KategorieKlientowHelper.SprawdzCzyPoprawnySeparator(cecha1, separator));
            Assert.True(KategorieKlientowHelper.SprawdzCzyPoprawnySeparator(cecha2, separator));
            Assert.True(KategorieKlientowHelper.SprawdzCzyPoprawnySeparator(cecha3, separator));
            Assert.True(KategorieKlientowHelper.SprawdzCzyPoprawnySeparator(cecha4, separator));
        }
    }
}
