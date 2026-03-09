using FakeItEasy;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using Xunit;
namespace SolEx.Hurt.Core.BLL.RegulyPunktowe.Tests
{
    public class PomnozeniePunktowPozycjiDokumentuTests
    {
        [Fact(DisplayName = "Mnożenie punktów za pozycje dokumentu")]
        public void WyliczPunktyTest()
        {
            Test(50, 100, 0.5M);
            Test(200, 100, 2);
        }

        private void Test(decimal oczekiwane, decimal wejsciowe, decimal przelicznik)
        {
            PomnozeniePunktowPozycjiDokumentu mod = new PomnozeniePunktowPozycjiDokumentu();
            IDokument dok = A.Fake<IDokument>();
            IDokumentPozycja pozycja = A.Fake<IDokumentPozycja>();
            mod.Przelicznik = przelicznik;
            decimal wynik = mod.WyliczPunkty(pozycja,dok, wejsciowe);
            Assert.True(oczekiwane == wynik, string.Format("Oczekiwane {0}, wynik {1} wejsciowe {2} przelicznik {3}", oczekiwane, wynik, wejsciowe, przelicznik));
        }
    }
}
