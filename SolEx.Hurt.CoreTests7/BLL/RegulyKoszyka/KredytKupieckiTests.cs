using FakeItEasy;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Core;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using Xunit;
namespace SolEx.Hurt.Core.BLL.RegulyKoszyka.Tests
{
    public class KredytKupieckiTests
    {
        [Fact(DisplayName = "Testy wartości kredytu kupieckiego")]
        public void KoszykSpelniaReguleTest()
        {
            Testuj(100, 90, 1, Wartosc.Wieksze, true);
            Testuj(100, 90, 1, Wartosc.Mniejsze, false);
            Testuj(100, 0,  1, Wartosc.Mniejsze, false);


            Testuj(100, 90, 1.5M, Wartosc.Wieksze, false);
            Testuj(100, 90, 1.5M, Wartosc.Mniejsze, true);
            Testuj(100, 50, 2, Wartosc.Rowne, true);
        }

        void Testuj(decimal wartoscKoszykaNetto, decimal kredytKupiecki, decimal wspolczynnik, Wartosc regula, bool oczekiwany)
        {
            KredytKupiecki modul=new KredytKupiecki();
            modul.Mnoznik = wspolczynnik;
            modul.WartoscWarunek = regula;
            IKoszykiBLL koszyk = A.Fake<IKoszykiBLL>();
            IKlient klient = A.Fake<IKlient>();
            A.CallTo(() => klient.LimitKredytu).Returns(kredytKupiecki);
            A.CallTo(() => klient.IloscPozostalegoKredytu).Returns(kredytKupiecki);
            A.CallTo(() => koszyk.CalkowitaWartoscHurtowaBruttoPoRabacie()).Returns(new WartoscLiczbowa(wartoscKoszykaNetto, "pln"));
            A.CallTo(() => koszyk.Klient).Returns(klient);

            bool wynik = modul.KoszykSpelniaRegule(koszyk);
            Assert.True(wynik==oczekiwany, $"wynik {wynik}, oczekiwany {oczekiwany} wartosc k {wartoscKoszykaNetto} wartosc limit {kredytKupiecki} wsp {wspolczynnik} regula {regula}");
        }
    }
}
