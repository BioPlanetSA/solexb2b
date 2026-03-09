using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Helpers;
using Xunit;
namespace SolEx.Hurt.Model.Helpers.Tests
{
    public class TypesHelperTests
    {
        string frazaWzorcowa = "Pobierz Dokument do Analizy";

        private string PobierzFrazeTestowa
        {
            get
            {
                StringBuilder sb = new StringBuilder("   Pobierz Dokument");
                sb.Append(" do Analizy     "); //specjlanie jest spacja na koncu i na poczatku
                return  sb.ToString();
            }
        }

        [Fact(DisplayName = "Test WygenerujIDObiektuSHAWersjaLong - spradzamy czy w rownloeglych petlach generuej sie to samo ID")]
        public void WygenerujIDObiektuSHAWersjaLongTest()
        {
            long idWzorcowe = frazaWzorcowa.WygenerujIDObiektuSHAWersjaLong();

            Parallel.For(0, 1000000, i =>
                {
                    long id2 = PobierzFrazeTestowa.WygenerujIDObiektuSHAWersjaLong();
                    Assert.Equal(idWzorcowe, id2);
                }
            );
        }


        [Fact(DisplayName = "Test dla WygenerujKluczSHALong - spradzamy czy w rownloeglych petlach generuej sie to samo ID")]
        public void WygenerujKluczSHALongTest()
        {
            var wynikWzorcowy = TypeExtensions.WygenerujKluczSHALong(frazaWzorcowa);

            Parallel.For(0, 1000000, i =>
            {
                var wynik = TypeExtensions.WygenerujKluczSHALong(PobierzFrazeTestowa.Trim());
                Assert.Equal(wynik, wynikWzorcowy);
            });
        }

        [Fact(DisplayName = "Test ZbudujKluczStringDlaIDSHA")]
        public void ZbudujKluczStringDlaIDSHATest()
        {
            var wynikWzorcowy = TypeExtensions.ZbudujKluczStringDlaIDSHA(frazaWzorcowa);

            Parallel.For(0, 1000000, i =>
            {
                var wynik = TypeExtensions.ZbudujKluczStringDlaIDSHA(PobierzFrazeTestowa);
                Assert.Equal(wynik, wynikWzorcowy);
            }
                );
        }
    }
}
