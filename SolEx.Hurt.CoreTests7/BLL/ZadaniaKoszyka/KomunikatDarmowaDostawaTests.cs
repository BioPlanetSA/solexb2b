using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.BLL.RegulyKoszyka;
using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using Xunit;
namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka.Tests
{
    public class KomunikatDarmowaDostawaTests
    {
        [Fact(DisplayName = "Test sprawdzajacy czy metoda poprawnie zwraca moduly")]
        public void ModulySpelniajaceWarunekTest()
        {
            IKoszykiBLL koszyk = A.Fake<IKoszykiBLL>();
            IKlient klientPierwotny = A.Fake<IKlient>();
            klientPierwotny.Id = 1;

            List<IKoszykPozycja> pozycje = new List<IKoszykPozycja>();

            IKoszykPozycja pozycja1 = A.Fake<IKoszykPozycja>();
            A.CallTo(() => pozycja1.Produkt().IloscLaczna).Returns(10);
            A.CallTo(() => pozycja1.WartoscNetto).Returns(200);

            IKoszykPozycja pozycja2 = A.Fake<IKoszykPozycja>();
            A.CallTo(() => pozycja2.Produkt().IloscLaczna).Returns(10);
            A.CallTo(() => pozycja2.WartoscNetto).Returns(700);

            pozycje.Add(pozycja1);
            pozycje.Add(pozycja2);

            A.CallTo(() => koszyk.Klient).Returns(klientPierwotny);
            Waluta waluta = A.Fake<Waluta>();
            waluta.WalutaB2b = "PLN";

            A.CallTo(() => koszyk.WalutaKoszyka()).Returns(waluta);
            A.CallTo(() => koszyk.PobierzPozycje).Returns(pozycje);

            RegulaKoszyka r1 = new LacznaWartoscKoszyka() { WartoscMinimalna = 200, WartoscMaksymalna = 400, CzyBrutto = false};
            RegulaKoszyka r2 = new LacznaWartoscKoszyka() { WartoscMinimalna = 500, WartoscMaksymalna = 700, CzyBrutto = false};
            RegulaKoszyka r3 = new LacznaWartoscKoszyka() { WartoscMinimalna = 800, WartoscMaksymalna = 1500, CzyBrutto = false};
           
            var k1 = A.Fake<KosztDostawy>();
            A.CallTo(() => k1.Warunki() ).Returns(new List<RegulaKoszyka>() { r1});
            var k2 = A.Fake<KosztDostawy>();
            A.CallTo(() => k2.RegulyZadania).Returns(new List<RegulaKoszyka>() { r2 });
            var k3 = A.Fake<KosztDostawy>();
            A.CallTo(() => k3.RegulyZadania).Returns(new List<RegulaKoszyka>() { r3 });

            ZadanieCalegoKoszyka z1 = k1;
           
            z1.powiazaneZadanie = new Zadania() { id = 1, Centralne = true};
            ZadanieCalegoKoszyka z2 = k2;
            z2.powiazaneZadanie = new Zadania() { id = 2, Centralne = true};
            ZadanieCalegoKoszyka z3 = k3;
            z3.powiazaneZadanie = new Zadania() { id = 3, Centralne = false, OddzialId =1};
            ZadanieCalegoKoszyka z4 = new KoszykLimityIloscioweSubkont();
            z4.powiazaneZadanie = new Zadania() { id = 4 };
            List<ZadanieCalegoKoszyka> listaZadanCalegoKoszyka = new List<ZadanieCalegoKoszyka>() {z1, z2, z3, z4};
 
            var zadania = A.Fake<IZadaniaBLL>();
            A.CallTo(() => zadania.PobierzZadaniaCalegoKoszyka<ISposobDostawy>(koszyk.JezykID, klientPierwotny))
                .Returns(listaZadanCalegoKoszyka);

            var kdd = new KomunikatDarmowaDostawa();
            kdd.IdDoPominiecia = null;
            kdd.ZadaniaBll = zadania;
            kdd.Komunikat = "Komunikat";

            List<LacznaWartoscKoszyka> wynik = kdd.ModulySpelniajaceWarunek(koszyk);

            Assert.True(wynik.Count()==3); //Zwraca zadania posiadajace laczna wartosc koszyka
        }

        [Fact(DisplayName = "Test sprawdzajacy darmową dostawę")]
        public void KomunikatDarmowaDostawaTest()
        {
            IKoszykiBLL koszyk = A.Fake<IKoszykiBLL>();
            IKlient klientPierwotny = A.Fake<IKlient>();
            klientPierwotny.klient_id = 1;

            List<IKoszykPozycja> pozycje = new List<IKoszykPozycja>();

            IKoszykPozycja pozycja1 = A.Fake<IKoszykPozycja>();
            A.CallTo(() => pozycja1.WartoscNetto).Returns(200);

            IKoszykPozycja pozycja2 = A.Fake<IKoszykPozycja>();
            A.CallTo(() => pozycja2.WartoscNetto).Returns(199);

            pozycje.Add(pozycja1);
            pozycje.Add(pozycja2);

            A.CallTo(() => koszyk.PierwotnyKlient()).Returns(klientPierwotny);
            A.CallTo(() => koszyk.WalutaKoszyka).Returns("PLN");
            A.CallTo(() => koszyk.Pozycje).Returns(pozycje);

            RegulaKoszyka r1 = new LacznaWartoscKoszyka() { WartoscMinimalna = 400, WartoscMaksymalna = 800, CzyBrutto = false };

            var k1 = A.Fake<KosztDostawy>();
            A.CallTo(() => k1.RegulyZadania).Returns(new List<RegulaKoszyka>() { r1 });

            ZadanieCalegoKoszyka z1 = k1;
            z1.powiazaneZadanie = new Zadania() { id = 1, Centralne = true };

            List<ZadanieCalegoKoszyka> listaZadanCalegoKoszyka = new List<ZadanieCalegoKoszyka>() { z1};

            var zadania = A.Fake<IZadaniaBLL>();
            A.CallTo(() => zadania.PobierzZadaniaCalegoKoszyka<ISposobDostawy>(koszyk.JezykID, klientPierwotny))
                .Returns(listaZadanCalegoKoszyka);

            var kdd = new KomunikatDarmowaDostawa();
            kdd.IdDoPominiecia = null;
            kdd.ZadaniaBll = zadania;
            kdd.Komunikat = "Komunikat";

            Assert.True(kdd.Wykonaj(koszyk));
        }
    }
}
