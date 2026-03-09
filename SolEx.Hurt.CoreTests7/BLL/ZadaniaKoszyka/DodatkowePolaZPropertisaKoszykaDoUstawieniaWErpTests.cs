using Xunit;
using FakeItEasy;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka.Tests
{
    public class DodatkowePolaZPropertisaKoszykaDoUstawieniaWErpTests
    {
        [Fact()]
        public void WykonajTest()
        {
            Adres adres = new Adres(1, "zachlapana", "77-777", "Bagno");
            IKoszykiBLL koszyk = A.Fake<IKoszykiBLL>();
            A.CallTo(() => koszyk.Adres).Returns(adres);
            A.CallTo(() => koszyk.Platnosc).Returns("bylejaka");

            DodatkowePolaZPropertisaKoszykaDoUstawieniaWErp modul = new DodatkowePolaZPropertisaKoszykaDoUstawieniaWErp();
            modul.NazwaPola = "Pole1";
            modul.NazwaPropertisa = "Adres";

            var wynik = modul.Wykonaj(koszyk);
            Assert.True(wynik, "Modul sie wykonał");
            Assert.True(koszyk.DodatkowePolaErp.Contains("Pole1:"+adres), "Moduł zawiera info o adresie");

            modul.NazwaPola = "Pole2";
            modul.NazwaPropertisa = "Platnosc";

            wynik = modul.Wykonaj(koszyk);
            Assert.True(wynik, "Modul sie wykonał");
            Assert.True(koszyk.DodatkowePolaErp.Contains("Pole2:bylejaka"), "Moduł zawiera info o Platnosci");
        }
    }
}