using System.Collections.Generic;
using FakeItEasy;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using Xunit;
namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka.Tests
{
    public class PlatnoscTests
    {
        [Fact(DisplayName = "Test sprawdzajacy poprawność modulu wyswietlajacego sposoby platnosci - poprawne dane")]
        public void WykonajTest1()
        {
       

            var klient = A.Fake<IKlient>();
            A.CallTo(() => klient.Kategorie).Returns(new []{1,2});

            var klienciDostpe = A.Fake<IKlienciDostep>();
            A.CallTo(() => klienciDostpe.Pobierz(A<int>.Ignored)).Returns(klient);

            IKoszykiBLL koszyk = new KoszykiBLL(null);

            Platnosc p = new Platnosc();
            p.GrupaKategorii = "JakasGrupa1";

            p.klienciDostep = klienciDostpe;
            Assert.True(p.Wykonaj(koszyk));
        }
        [Fact(DisplayName = "Test sprawdzajacy poprawność modulu wyswietlajacego sposoby platnosci - Brak wybranej kategorii")]
        public void WykonajTest2()
        {
            kategorie_klientow kk1 = new kategorie_klientow() { Id = 1, grupa = "JakasGrupa1", nazwa = "3" };
            kategorie_klientow kk2 = new kategorie_klientow() { Id = 2, grupa = "JakasGrupa2", nazwa = "4" };
            List<kategorie_klientow> Kategorie = new List<kategorie_klientow>() { kk1, kk2 };

            var klient = A.Fake<IKlient>();
        //    A.CallTo(() => klient.Kategorie).Returns(Kategorie);

            var klienciDostpe = A.Fake<IKlienciDostep>();
            A.CallTo(() => klienciDostpe.Pobierz(A<int>.Ignored)).Returns(klient);

            IKoszykiBLL koszyk = new KoszykiBLL(null);

            Platnosc p = new Platnosc();
            p.klienciDostep = klienciDostpe;
            Assert.True(p.Wykonaj(koszyk));
        }

        [Fact(DisplayName = "Test sprawdzajacy poprawność modulu wyswietlajacego sposoby platnosci - Wybrana kategoria brak jej u klienta")]
        public void WykonajTest3()
        {
            kategorie_klientow kk1 = new kategorie_klientow() { Id = 1, grupa = "JakasGrupa1", nazwa = "3" };
            kategorie_klientow kk2 = new kategorie_klientow() { Id = 2, grupa = "JakasGrupa2", nazwa = "4" };
            List<kategorie_klientow> Kategorie = new List<kategorie_klientow>() { kk1, kk2 };

            var klient = A.Fake<IKlient>();
         //   A.CallTo(() => klient.Kategorie).Returns(Kategorie);

            var klienciDostpe = A.Fake<IKlienciDostep>();
            A.CallTo(() => klienciDostpe.Pobierz(A<int>.Ignored)).Returns(klient);

            IKoszykiBLL koszyk = new KoszykiBLL(null);

            Platnosc p = new Platnosc();
            p.klienciDostep = klienciDostpe;
            p.GrupaKategorii = "BrakujacaGrupa";
            Assert.False(p.Wykonaj(koszyk));
        }
        
        [Fact(DisplayName = "Test sprawdzajacy poprawność modulu wyswietlajacego sposoby platnosci - kompatyilnosc wstecz")]
        public void WykonajTest5()
        {
            kategorie_klientow kk1 = new kategorie_klientow() { Id = 1, grupa = "JakasGrupa1", nazwa = "2" };
            kategorie_klientow kk2 = new kategorie_klientow() { Id = 2, grupa = "JakasGrupa2", nazwa = "4" };
            List<kategorie_klientow> Kategorie = new List<kategorie_klientow>() { kk1, kk2 };

            var klient = A.Fake<IKlient>();
       //     A.CallTo(() => klient.Kategorie).Returns(Kategorie);

            var klienciDostpe = A.Fake<IKlienciDostep>();
            A.CallTo(() => klienciDostpe.Pobierz(A<int>.Ignored)).Returns(klient);

            IKoszykiBLL koszyk = new KoszykiBLL(null);

            Platnosc p = new Platnosc();
            p.klienciDostep = klienciDostpe;
      //      p.GrupaKlientow = "JakasGrupa1";

            Assert.True(p.Wykonaj(koszyk));
        }
        [Fact(DisplayName = "Test sprawdzajacy poprawność modulu wyswietlajacego sposoby platnosci - kompatyilnosc wstecz 2")]
        public void WykonajTest4()
        {
            kategorie_klientow kk1 = new kategorie_klientow() { Id = 1, grupa = "JakasGrupa1", nazwa = "2" };
            kategorie_klientow kk2 = new kategorie_klientow() { Id = 2, grupa = "JakasGrupa2", nazwa = "4" };
            List<kategorie_klientow> Kategorie = new List<kategorie_klientow>() { kk1, kk2 };

            var klient = A.Fake<IKlient>();
         //   A.CallTo(() => klient.Kategorie).Returns(Kategorie);

            var klienciDostpe = A.Fake<IKlienciDostep>();
            A.CallTo(() => klienciDostpe.Pobierz(A<int>.Ignored)).Returns(klient);

            IKoszykiBLL koszyk = new KoszykiBLL(null);

            Platnosc p = new Platnosc();
            p.klienciDostep = klienciDostpe;
         //   p.GrupaKlientow = "JakasGrupa1:fdsfsd";

            Assert.True(p.Wykonaj(koszyk));
        }
    }
}
