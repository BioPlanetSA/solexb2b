using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.BLL.RegulyKoszyka;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using Xunit;
namespace SolEx.Hurt.Core.BLL.RegulyKoszyka.Tests
{
    public class KategoriaKlientaTests
    {
        [Fact(DisplayName = "Nalezy do ktorejkolwiek")]
        public void RegulaTest()
        {
            kategorie_klientow kk1 = new kategorie_klientow(){Id = 1, nazwa = "Jakas1"};
            kategorie_klientow kk3 = new kategorie_klientow() { Id = 3, nazwa = "Jakas3" };
            kategorie_klientow kk5 = new kategorie_klientow() { Id = 5, nazwa = "Jakas5" };
            List<kategorie_klientow> listaKategoriKlienta = new List<kategorie_klientow>(){kk1,kk3,kk5};

            var klient = A.Fake<IKlient>();
       //     A.CallTo(() => klient.Kategorie).Returns(listaKategoriKlienta);

            var klienciDostep = A.Fake<IKlienciDostep>();
            A.CallTo(() => klienciDostep.Pobierz(A<int>.Ignored)).Returns(klient);

            IKoszykiBLL koszyk = new KoszykiBLL(null);

            KategoriaKlienta kk = new KategoriaKlienta();
            kk.NaleznoscKlienta = KategoriaKlienta.Naleznosc.NalezyDoKoregokolwiek;
            kk.KategoriaID=new List<string>(){"1","2","4"};
            kk.klienciDostep = klienciDostep;
            bool wynik = kk.Regula(koszyk);
            Assert.True(wynik);
        }
        [Fact(DisplayName = "Do wszystkich")]
        public void RegulaTest2()
        {
            kategorie_klientow kk1 = new kategorie_klientow() { Id = 1, nazwa = "Jakas1" };
            kategorie_klientow kk3 = new kategorie_klientow() { Id = 3, nazwa = "Jakas3" };
            kategorie_klientow kk5 = new kategorie_klientow() { Id = 5, nazwa = "Jakas5" };
            List<kategorie_klientow> listaKategoriKlienta = new List<kategorie_klientow>() { kk1, kk3, kk5 };

            var klient = A.Fake<IKlient>();
       //     A.CallTo(() => klient.Kategorie).Returns(listaKategoriKlienta);

            var klienciDostep = A.Fake<IKlienciDostep>();
            A.CallTo(() => klienciDostep.Pobierz(A<int>.Ignored)).Returns(klient);

            IKoszykiBLL koszyk = new KoszykiBLL(null);

            KategoriaKlienta kk = new KategoriaKlienta();
            kk.NaleznoscKlienta = KategoriaKlienta.Naleznosc.NalezyDoWszystkich;
            kk.KategoriaID = new List<string>() { "1", "3", "5" };
            kk.klienciDostep = klienciDostep;
            bool wynik = kk.Regula(koszyk);
            Assert.True(wynik);
        }
        [Fact(DisplayName = "Nienalezy")]
        public void RegulaTest3()
        {
            kategorie_klientow kk1 = new kategorie_klientow() { Id = 1, nazwa = "Jakas1" };
            kategorie_klientow kk3 = new kategorie_klientow() { Id = 3, nazwa = "Jakas3" };
            kategorie_klientow kk5 = new kategorie_klientow() { Id = 5, nazwa = "Jakas5" };
            List<kategorie_klientow> listaKategoriKlienta = new List<kategorie_klientow>() { kk1, kk3, kk5 };

            var klient = A.Fake<IKlient>();
    //        A.CallTo(() => klient.Kategorie).Returns(listaKategoriKlienta);

            var klienciDostep = A.Fake<IKlienciDostep>();
            A.CallTo(() => klienciDostep.Pobierz(A<int>.Ignored)).Returns(klient);

            IKoszykiBLL koszyk = new KoszykiBLL(null);

            KategoriaKlienta kk = new KategoriaKlienta();
            kk.NaleznoscKlienta = KategoriaKlienta.Naleznosc.NieNalezy;
            kk.KategoriaID = new List<string>() { "2", "4"};
            kk.klienciDostep = klienciDostep;
            bool wynik = kk.Regula(koszyk);
            Assert.True(wynik);
        }
        [Fact(DisplayName = "Nalezy do ktorejkolwiek-false")]
        public void RegulaTest4()
        {
            kategorie_klientow kk1 = new kategorie_klientow() { Id = 1, nazwa = "Jakas1" };
            kategorie_klientow kk3 = new kategorie_klientow() { Id = 3, nazwa = "Jakas3" };
            kategorie_klientow kk5 = new kategorie_klientow() { Id = 5, nazwa = "Jakas5" };
            List<kategorie_klientow> listaKategoriKlienta = new List<kategorie_klientow>() { kk1, kk3, kk5 };

            var klient = A.Fake<IKlient>();
         //   A.CallTo(() => klient.Kategorie).Returns(listaKategoriKlienta);

            var klienciDostep = A.Fake<IKlienciDostep>();
            A.CallTo(() => klienciDostep.Pobierz(A<int>.Ignored)).Returns(klient);

            IKoszykiBLL koszyk = new KoszykiBLL(null);

            KategoriaKlienta kk = new KategoriaKlienta();
            kk.NaleznoscKlienta = KategoriaKlienta.Naleznosc.NalezyDoKoregokolwiek;
            kk.KategoriaID = new List<string>() { "2", "4" };
            kk.klienciDostep = klienciDostep;
            bool wynik = kk.Regula(koszyk);
            Assert.False(wynik);
        }
        [Fact(DisplayName = "Do wszystkich - false")]
        public void RegulaTest5()
        {
            kategorie_klientow kk1 = new kategorie_klientow() { Id = 1, nazwa = "Jakas1" };
            kategorie_klientow kk3 = new kategorie_klientow() { Id = 3, nazwa = "Jakas3" };
            kategorie_klientow kk5 = new kategorie_klientow() { Id = 5, nazwa = "Jakas5" };
            List<kategorie_klientow> listaKategoriKlienta = new List<kategorie_klientow>() { kk1, kk3, kk5 };

            var klient = A.Fake<IKlient>();
         //   A.CallTo(() => klient.Kategorie).Returns(listaKategoriKlienta);

            var klienciDostep = A.Fake<IKlienciDostep>();
            A.CallTo(() => klienciDostep.Pobierz(A<int>.Ignored)).Returns(klient);

            IKoszykiBLL koszyk = new KoszykiBLL(null);

            KategoriaKlienta kk = new KategoriaKlienta();
            kk.NaleznoscKlienta = KategoriaKlienta.Naleznosc.NalezyDoWszystkich;
            kk.KategoriaID = new List<string>() { "2", "3", "4" };
            kk.klienciDostep = klienciDostep;
            bool wynik = kk.Regula(koszyk);
            Assert.False(wynik);
        }
        [Fact(DisplayName = "Nienalezy - false")]
        public void RegulaTest6()
        {
            kategorie_klientow kk1 = new kategorie_klientow() { Id = 1, nazwa = "Jakas1" };
            kategorie_klientow kk3 = new kategorie_klientow() { Id = 3, nazwa = "Jakas3" };
            kategorie_klientow kk5 = new kategorie_klientow() { Id = 5, nazwa = "Jakas5" };
            List<kategorie_klientow> listaKategoriKlienta = new List<kategorie_klientow>() { kk1, kk3, kk5 };

            var klient = A.Fake<IKlient>();
         //   A.CallTo(() => klient.Kategorie).Returns(listaKategoriKlienta);

            var klienciDostep = A.Fake<IKlienciDostep>();
            A.CallTo(() => klienciDostep.Pobierz(A<int>.Ignored)).Returns(klient);

            IKoszykiBLL koszyk = new KoszykiBLL(null);

            KategoriaKlienta kk = new KategoriaKlienta();
            kk.NaleznoscKlienta = KategoriaKlienta.Naleznosc.NieNalezy;
            kk.KategoriaID = new List<string>() {"1", "2", "4" };
            kk.klienciDostep = klienciDostep;
            bool wynik = kk.Regula(koszyk);
            Assert.False(wynik);
        }
    }
}
