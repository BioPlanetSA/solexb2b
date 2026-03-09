using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using Xunit;

namespace SolEx.Hurt.CoreTests.BLL.ZadaniaKoszyka
{
    public class PrzekroczoneStanyTests
    {
        [Fact(DisplayName = "Test modułu przekroczone stany - łatwy przypadek")]
        public void WykonajTest()
        {
            IKoszykiBLL koszyk = A.Fake<IKoszykiBLL>();

            KoszykPozycje p1 = A.Fake<KoszykPozycje>();
            p1.IloscWJednostcePodstawowej = 7;

            IProduktKlienta fp1 = A.Fake<IProduktKlienta>();
            A.CallTo(() => fp1.IloscLaczna).Returns(50);
            A.CallTo(() => fp1.Id).Returns(50);
            p1.ProduktId = fp1.Id;
            A.CallTo(() => p1.Produkt).Returns(fp1);


            KoszykPozycje p2 = A.Fake<KoszykPozycje>();
            p2.IloscWJednostcePodstawowej = 3;
            IProduktKlienta fp2 = A.Fake<IProduktKlienta>();
            A.CallTo(() => fp2.IloscLaczna).Returns(30);
            A.CallTo(() => p2.Produkt).Returns(fp2);
            A.CallTo(() => fp2.Id).Returns(51);
            p2.ProduktId = fp2.Id;

            KoszykPozycje p3 = A.Fake<KoszykPozycje>();
            p3.IloscWJednostcePodstawowej = 2;
            IProduktKlienta fp3 = A.Fake<IProduktKlienta>();
            A.CallTo(() => fp3.IloscLaczna).Returns(30);
            A.CallTo(() => p3.Produkt).Returns(fp3);
            A.CallTo(() => fp3.Id).Returns(52);
            p3.ProduktId = fp3.Id;

            KoszykPozycje p4 = A.Fake<KoszykPozycje>();
            p4.IloscWJednostcePodstawowej = 7;
            IProduktKlienta fp4 = A.Fake<IProduktKlienta>();
            A.CallTo(() => fp4.IloscLaczna).Returns(10);
            A.CallTo(() => p4.Produkt).Returns(fp4);
            A.CallTo(() => fp4.Id).Returns(53);
            p4.ProduktId = fp4.Id;

            KoszykPozycje p5 = A.Fake<KoszykPozycje>();
            p5.IloscWJednostcePodstawowej = 2;
            IProduktKlienta fp5 = A.Fake<IProduktKlienta>();
            A.CallTo(() => fp5.IloscLaczna).Returns(20);
            A.CallTo(() => p5.Produkt).Returns(fp5);
            A.CallTo(() => fp5.Id).Returns(54);
            p5.ProduktId = fp5.Id;

            KoszykPozycje p6 = A.Fake<KoszykPozycje>();
            p6.IloscWJednostcePodstawowej = 1;
            IProduktKlienta fp6 = A.Fake<IProduktKlienta>();
            A.CallTo(() => fp6.IloscLaczna).Returns(0);
            A.CallTo(() => p6.Produkt).Returns(fp6);
            A.CallTo(() => fp6.Id).Returns(55);
            p6.ProduktId = fp6.Id;

            List<KoszykPozycje> listaProduktow = new List<KoszykPozycje>(){p1,p2,p3,p4,p5,p6};

            A.CallTo(() => koszyk.PobierzPozycje).Returns(listaProduktow);
           
            ///////////////////////////////////////////////////////////
            //Test bez wskazywania magazynu konkretnego - czyli powinno wziac stan z magazynu podstawowego koszyka - czyli Wawa

            PrzekroczoneStany ps = new PrzekroczoneStany();

            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            ps.Calosc = calosc;

            //A.CallTo(() => calosc.ProduktyStanBll.PobierzStanyDlaProduktu(A< HashSet < int >>.That.Matches(x=> x.Count == 1 && x.First() ==1 ), fp1.Id)).Returns(6);    //kupujemy 7 - czyli przekroczony
            //A.CallTo(() => calosc.ProduktyStanBll.PobierzStanyDlaProduktu(A<HashSet<int>>.That.Matches(x => x.Count == 1 && x.First() == 1), fp2.Id)).Returns(6); //kupujemy 3 - czyli nie przekroczony
            //A.CallTo(() => calosc.ProduktyStanBll.PobierzStanyDlaProduktu(A<HashSet<int>>.That.Matches(x => x.Count == 1 && x.First() == 1), fp3.Id)).Returns(6); //kupujemy 2 - czyli nie przekroczony
            //A.CallTo(() => calosc.ProduktyStanBll.PobierzStanyDlaProduktu(A<HashSet<int>>.That.Matches(x => x.Count == 1 && x.First() == 1), fp4.Id)).Returns(0);//kupujemy 7 - czyli niedostepnu
            //A.CallTo(() => calosc.ProduktyStanBll.PobierzStanyDlaProduktu(A<HashSet<int>>.That.Matches(x => x.Count == 1 && x.First() == 1), fp5.Id)).Returns(76);//kupujemy 2 - czyli nie przekroczony
            //A.CallTo(() => calosc.ProduktyStanBll.PobierzStanyDlaProduktu(A<HashSet<int>>.That.Matches(x => x.Count == 1 && x.First() == 1), fp6.Id)).Returns(0);//kupujemy 1 - czyli niedostpeny


            A.CallTo(() => fp1.PobierzStan(A<HashSet<int>>.That.Matches(x => x.Count == 1 && x.First() == 1))).Returns(6);    //kupujemy 7 - czyli przekroczony
            A.CallTo(() => fp2.PobierzStan(A<HashSet<int>>.That.Matches(x => x.Count == 1 && x.First() == 1))).Returns(6); //kupujemy 3 - czyli nie przekroczony
            A.CallTo(() => fp3.PobierzStan(A<HashSet<int>>.That.Matches(x => x.Count == 1 && x.First() == 1))).Returns(6); //kupujemy 2 - czyli nie przekroczony
            A.CallTo(() => fp4.PobierzStan(A<HashSet<int>>.That.Matches(x => x.Count == 1 && x.First() == 1))).Returns(0);//kupujemy 7 - czyli niedostepnu
            A.CallTo(() => fp5.PobierzStan(A<HashSet<int>>.That.Matches(x => x.Count == 1 && x.First() == 1))).Returns(76);//kupujemy 2 - czyli nie przekroczony
            A.CallTo(() => fp6.PobierzStan(A<HashSet<int>>.That.Matches(x => x.Count == 1 && x.First() == 1))).Returns(0);//kupujemy 1 - czyli niedostpeny

            Dictionary<string, Magazyn> dict = new Dictionary<string, Magazyn>();
            dict.Add("WAWA", new Magazyn {Id =  1, Symbol = "WAWA"} );

            A.CallTo(() => calosc.Konfiguracja.SlownikMagazynowPoSymbolu).Returns(dict);
            A.CallTo(() => koszyk.MagazynRealizujacy).Returns("WAWA");

            List<int> idMagazonow = new List<int>();
            ps.IdMagazynow = idMagazonow;
            ps.BlokadaPoPrzekroczeniu = BlokadaKoszyka.BlokujGdyWszystkieNiedostepne;
            ps.PobieracStanyTylkoZMagazynuPodstawowegoKoszyka = true;

            Assert.True(ps.Wykonaj(koszyk));

            Assert.True(koszyk.PobierzPozycje[0].StanKoszyk == StanKoszyk.Przekroczony);
            Assert.True(koszyk.PobierzPozycje[1].StanKoszyk == StanKoszyk.Ok);
            Assert.True(koszyk.PobierzPozycje[2].StanKoszyk == StanKoszyk.Ok);
            Assert.True(koszyk.PobierzPozycje[3].StanKoszyk == StanKoszyk.Niedostepy);
            Assert.True(koszyk.PobierzPozycje[4].StanKoszyk == StanKoszyk.Ok);
            Assert.True(koszyk.PobierzPozycje[5].StanKoszyk == StanKoszyk.Niedostepy);


            //A.CallTo(() => calosc.ProduktyStanBll.PobierzStanyDlaProduktu(A<HashSet<int>>.That.Matches(x => x.Count == 1 && x.First() == 1), fp1.Id)).Returns(0);
            //A.CallTo(() => calosc.ProduktyStanBll.PobierzStanyDlaProduktu(A<HashSet<int>>.That.Matches(x => x.Count == 1 && x.First() == 1), fp2.Id)).Returns(0);
            //A.CallTo(() => calosc.ProduktyStanBll.PobierzStanyDlaProduktu(A<HashSet<int>>.That.Matches(x => x.Count == 1 && x.First() == 1), fp3.Id)).Returns(0);
            //A.CallTo(() => calosc.ProduktyStanBll.PobierzStanyDlaProduktu(A<HashSet<int>>.That.Matches(x => x.Count == 1 && x.First() == 1), fp4.Id)).Returns(0);
            //A.CallTo(() => calosc.ProduktyStanBll.PobierzStanyDlaProduktu(A<HashSet<int>>.That.Matches(x => x.Count == 1 && x.First() == 1), fp5.Id)).Returns(0);
            //A.CallTo(() => calosc.ProduktyStanBll.PobierzStanyDlaProduktu(A<HashSet<int>>.That.Matches(x => x.Count == 1 && x.First() == 1), fp6.Id)).Returns(0);

            A.CallTo(() => fp1.PobierzStan(A<HashSet<int>>.That.Matches(x => x.Count == 1 && x.First() == 1))).Returns(0);
            A.CallTo(() => fp2.PobierzStan(A<HashSet<int>>.That.Matches(x => x.Count == 1 && x.First() == 1))).Returns(0);
            A.CallTo(() => fp3.PobierzStan(A<HashSet<int>>.That.Matches(x => x.Count == 1 && x.First() == 1))).Returns(0);
            A.CallTo(() => fp4.PobierzStan(A<HashSet<int>>.That.Matches(x => x.Count == 1 && x.First() == 1))).Returns(0);
            A.CallTo(() => fp5.PobierzStan(A<HashSet<int>>.That.Matches(x => x.Count == 1 && x.First() == 1))).Returns(0);
            A.CallTo(() => fp6.PobierzStan(A<HashSet<int>>.That.Matches(x => x.Count == 1 && x.First() == 1))).Returns(0);

            ps.BlokadaPoPrzekroczeniu = BlokadaKoszyka.BlokujGdyWszystkieNiedostepne;
            Assert.False(ps.Wykonaj(koszyk));
        }
    }
}
