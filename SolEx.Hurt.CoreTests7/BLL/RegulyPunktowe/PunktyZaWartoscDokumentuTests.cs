using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using Xunit;
using FakeItEasy;
namespace SolEx.Hurt.Core.BLL.RegulyPunktowe.Tests
{
    public class PunktyZaWartoscDokumentuTests
    {
        [Fact(DisplayName = "Test licznia puntków za wartość dokumentu")]
        public void WyliczPunktyTest()
        {
            Testuj(1, 1000, 1100, false, 1000);
            Testuj(1, 1000, 1100, true, 1100);
            Testuj(1.5M, 1000, 1100, true, 1650);
            Testuj(1.3M, 1000, 1100, true, 1430);
            Testuj(1.33M, 10, 12, false, 13);
        }

        private void Testuj(decimal przelicznik, decimal netto, decimal brutto, bool czyBrutto, int oczekiwane)
        {
            PunktyZaWartoscDokumentu modul=new PunktyZaWartoscDokumentu();
            modul.CzyBrutto = czyBrutto;
            modul.IlePunktow = przelicznik;
            IDokument dok = A.Fake<IDokument>();
            A.CallTo(() => dok.DokumentWartoscNetto).Returns(netto);
            A.CallTo(() => dok.DokumentWartoscBrutto).Returns(brutto);
            int wynik = modul.WyliczPunkty(dok,0);
            Assert.True(wynik==oczekiwane,string.Format("wynik {0} oczekiwane {1}, przelicznik {2} netto {3} brutto {4} czy brutto {5}",wynik,oczekiwane,przelicznik,netto,brutto,czyBrutto));

        }
    }
}
