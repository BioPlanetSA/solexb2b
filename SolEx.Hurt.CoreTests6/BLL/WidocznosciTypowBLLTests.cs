using FakeItEasy;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using Xunit;
namespace SolEx.Hurt.Core.BLL.Tests
{
    public class WidocznosciTypowBLLTests
    {
        [Fact(DisplayName = "Test sprawdzania dostępności")]
        public void KlientMaDostepDoObiektuTest()
        {
            TestDostep(false, new[] {1, 2}, new[] {1, 2, 3}, new int[0], WidocznoscTypoKierunek.DostepDlaKlientowZKategorii);
            TestDostep(true, new[] { 1, 2 }, new[] { 1, 2, 3 }, new int[0], WidocznoscTypoKierunek.BrakDostepuDlaKlientowZKategorii);
            TestDostep(true, new[] { 1, 2, 3 }, new[] { 1, 2, 3 }, new int[0], WidocznoscTypoKierunek.DostepDlaKlientowZKategorii);
            TestDostep(false, new[] { 1, 2, 3 }, new[] { 1, 2, 3 }, new int[0], WidocznoscTypoKierunek.BrakDostepuDlaKlientowZKategorii);

            TestDostep(true, new[] { 1, 2 }, new int[0], new[] { 1, 3 }, WidocznoscTypoKierunek.DostepDlaKlientowZKategorii);
            TestDostep(false, new[] { 1 }, new int[0], new[] { 1, 3 }, WidocznoscTypoKierunek.BrakDostepuDlaKlientowZKategorii);
            TestDostep(true, new[] { 1, 2, 3 }, new int[0], new int[0], WidocznoscTypoKierunek.DostepDlaKlientowZKategorii);
            TestDostep(false, new[] { 1, 2, 3 }, new int[0], new int[0], WidocznoscTypoKierunek.BrakDostepuDlaKlientowZKategorii);
        }

        public void TestDostep(bool wynik, int[] kategorieKlienta, int[] wymaganeWszystkie, int[] wymaganeKtoreKolwiek, WidocznoscTypoKierunek ukrywanie)
        {
                        ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            WidocznosciTypow test=new WidocznosciTypow();
            test.Kierunek = ukrywanie;
            test.KategoriaKlientaIdWszystkie = wymaganeWszystkie;
            test.KategoriaKlientaIdKtorakolwiek = wymaganeKtoreKolwiek;
            IKlient klient = A.Fake<IKlient>();
            A.CallTo(() => klient.Kategorie).Returns(kategorieKlienta);
            WidocznosciTypowBLL widocznosc = new WidocznosciTypowBLL(calosc);
          Assert.Equal(wynik,  widocznosc.KlientMaDostepDoObiektuWylicz(klient, test));
        }
    }
}
