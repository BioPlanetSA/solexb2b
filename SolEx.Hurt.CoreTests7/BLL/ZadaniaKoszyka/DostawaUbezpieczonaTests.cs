using FakeItEasy;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using Xunit;
namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka.Tests
{
    public class DostawaUbezpieczonaTests
    {
        [Fact(DisplayName = "Liczenie ceny wysyłki z ubezpieczeniem")]
        public void WyliczCeneTest()
        {
         Test(100,0,10,10,20);
         Test(100, 1, 10, 10, 20);
         Test(100, 20, 10, 10,30);
        }

        void Test(decimal wartoscNetto, decimal procent, decimal kosztDostwy, decimal minimalneUbezpieczenie, decimal oczekiwane)
        {
            DostawaUbezpieczona modul=new DostawaUbezpieczona();
            modul.ProcentCeny = procent;
            modul.CenaNetto = kosztDostwy;
            modul.MinimalnaCena = minimalneUbezpieczenie;
            IKoszykiBLL koszyk = A.Fake<IKoszykiBLL>();
            A.CallTo(() => koszyk.CalkowitaWartoscHurtowaNettoPoRabacie()).Returns(wartoscNetto);
            decimal wynik = modul.WyliczCene(koszyk);

            Assert.True(wynik==oczekiwane,string.Format("wynik {0} oczekiwane {1}, wartosc netto {2}, procent {3}, koszt dostawy {4}, minimum {5}"
                , wynik, oczekiwane, wartoscNetto, procent, kosztDostwy, minimalneUbezpieczenie));
        }
    }
}
