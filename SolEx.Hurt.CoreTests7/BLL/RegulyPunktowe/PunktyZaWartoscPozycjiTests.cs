using FakeItEasy;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Core;
using Xunit;
namespace SolEx.Hurt.Core.BLL.RegulyPunktowe.Tests
{
    public class PunktyZaWartoscPozycjiTests
    {
        [Fact(DisplayName = "Test licznia puntków za wartość pozycji dokumentu")]
        public void WyliczPunktyTest()
        {
            Testuj(1, 1000, 1100, false, 1000);
            Testuj(1, 1000, 1100, true, 1100);
            Testuj(1.5M, 1000, 1100, true, 1650); 
            Testuj(1.3M, 1000, 1100, true, 1430);
            Testuj(1.33M, 10, 12, false, 13.3M);
            Testuj(1, 1000, 1100, false, 1000,true);
            Testuj(1, 1000, 1100, false, 900, true,10);
            Testuj(1, 1000, 1100, false, 1000, false, 15);
        }

        private void Testuj(decimal przelicznik, decimal netto, decimal brutto, bool czyBrutto, decimal oczekiwane,bool zmniejszrabat=false,decimal rabat=0)
        {
            PunktyZaWartoscPozycji modul = new PunktyZaWartoscPozycji();
            modul.CzyBrutto = czyBrutto;
            modul.IlePunktow = przelicznik;
            modul.ZmiejszORabat = zmniejszrabat;
            IDokument dok = A.Fake<IDokument>();
            IDokumentPozycja pozycja = A.Fake<IDokumentPozycja>();
            A.CallTo(() => pozycja.PozycjaDokumentuRabat).Returns(new WartoscLiczbowaZaokraglana(rabat));
            A.CallTo(() => pozycja.PozycjaDokumentuWartoscNetto).Returns(netto);
            A.CallTo(() => pozycja.PozycjaDokumentuWartoscBrutto).Returns(brutto);
            decimal wynik = modul.WyliczPunkty(pozycja,dok, 0);
            Assert.True(wynik == oczekiwane, string.Format("wynik {0} oczekiwane {1}, przelicznik {2} netto {3} brutto {4} czy brutto {5}", wynik, oczekiwane, przelicznik, netto, brutto, czyBrutto));

        }
    }
}
