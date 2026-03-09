using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.Pomocnicze;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces;
using Xunit;
namespace SolEx.Hurt.Core.BLL.Tests
{
    public class CechyAtrybutyTests
    {


        [Fact(DisplayName = "Test sprawdzajacy działanie metody zwracajacej atrybuty dla okreslonej cechy o okreslonym symbolu")]
        public void WyciagnijAtrybutZCechyTest()
        {
            string separator = ":";
            string symbol = "kolejność:758";
            string nazwa = "758";

            string symbol2 = "cos";
            string nazwa2 = "234";
            string symbolAuto = "--auto--";

            var config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.AtrybutZCechy).Returns(true);
            A.CallTo(() => config.SeparatorAtrybutowWCechach).Returns(separator);
            A.CallTo(() => config.CechaAuto).Returns(symbolAuto);


            AtrybutyWyszukiwanie ca = new AtrybutyWyszukiwanie();
            ca.Konfiguracja = config;
            atrybuty wynik = ca.WyciagnijAtrybutZCechy(ref symbol, ref nazwa);
            atrybuty wynik2 = ca.WyciagnijAtrybutZCechy(ref symbol2, ref nazwa2);

            int id1 = Math.Abs("kolejność".Trim().GetHashCode());
            int id2 = Math.Abs("--auto--".Trim().GetHashCode());

            Assert.Equal(wynik.nazwa, "kolejność");
            Assert.True(wynik.atrybut_id == id1);
            Assert.Equal(wynik2.nazwa, symbolAuto);
            Assert.True(wynik2.atrybut_id == id2);
        }

        [Fact(DisplayName = "Test sprawdzajacy czy metoda prawidłowo buduje slownik filtrów w oparciu o ciąg znakowy")]
        public void ZbudujSlownikFiltrowTest()
        {
            var ConfigBllFake = A.Fake<IConfigBLL>();
            string filtr = @"kategorie[agd i rtv\lodówki;sprzęt sportowy\narty;agd i rtv\kuchenki mikrofalowe]kolejność[452;361]długość[1000 mm;2000 mm;3000 mm; 40000mm]";
            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            A.CallTo(() => calosc.Konfiguracja).Returns(ConfigBllFake);
            CechyAtrybuty ca = A.Fake<CechyAtrybuty>(x => x.WithArgumentsForConstructor(new[] { calosc }));
            Dictionary<string, HashSet<string>> wynik = ca.ZbudujSlownikFiltrow(filtr);

            Assert.True(wynik.Count==3);
            Assert.True(wynik["kategorie"].Count == 3, "ilość cech dla atrybutu kategorie");
            Assert.True(wynik["kolejność"].Count == 2, "ilość cech dla atrybutu kolejność");
            Assert.True(wynik["długość"].Count == 4, "ilość cech dla atrybutu kolejność");
        }

        [Fact(DisplayName = "Test sprawdzajacy poprawnosc działania metody budujacej slownik na podstawie ciagu tekstowego ")]
        public void ZbudujSlownikFiltrowZnajdzAtrybutyTest()
        {
            string filtr = @"kategorie[agd i rtv\lodówki;sprzęt sportowy\narty;agd i rtv\kuchenki mikrofalowe]kolejność[452;361]długość[1000 mm;2000 mm;3000 mm; 40000mm]";

            AtrybutyBLL atrybut1 = new AtrybutyBLL();
            atrybut1.nazwa = "kategorie";
            

            AtrybutyBLL atrybut2 = new AtrybutyBLL();
            atrybut2.nazwa = "kolejność";

            AtrybutyBLL atrybut3 = new AtrybutyBLL();
            atrybut3.nazwa = "długość";
            List<AtrybutyBLL> listaAtrybutow = new List<AtrybutyBLL>(){atrybut1,atrybut2,atrybut3};
            var cechyAtr = A.Fake<CechyAtrybuty>();
            A.CallTo(() => cechyAtr.WszystkieAtrybutyWeWszystkichJezykach()).Returns(listaAtrybutow);

            Dictionary<AtrybutyBLL, HashSet<string>> wynik = cechyAtr.ZbudujSlownikFiltrowZnajdzAtrybuty(filtr);

            List<AtrybutyBLL> la = wynik.Keys.ToList();
            Assert.True(la.Contains(atrybut1));
            Assert.True(la.Contains(atrybut2));
            Assert.True(la.Contains(atrybut3));

        }

        [Fact(DisplayName = "Test sprawdzajacy generowanie słownika przechowującego jako klucz numeru id atrybutu oraz jako wartosci zbior numerow id cech")]
        public void SlownikFiltrowTest()
        {
            CechyBll cecha1 = new CechyBll(){cecha_id = 1, nazwa = "agd i rtv\\lodówki"};
            CechyBll cecha2 = new CechyBll(){cecha_id = 2, nazwa = "sprzęt sportowy\\narty"};
            CechyBll cecha3 = new CechyBll(){cecha_id = 3, nazwa = "agd i rtv\\kuchenki mikrofalowe"};
            CechyBll cecha4 = new CechyBll(){cecha_id = 4, nazwa = "452"};
            CechyBll cecha5 = new CechyBll(){cecha_id = 5, nazwa = "361"};
            CechyBll cecha6 = new CechyBll(){cecha_id = 6, nazwa = "1000 mm"};
            CechyBll cecha7 = new CechyBll(){cecha_id = 7, nazwa = "2000 mm"};
            CechyBll cecha8 = new CechyBll(){cecha_id = 8, nazwa = "3000 mm"};
            CechyBll cecha9 = new CechyBll(){cecha_id = 9, nazwa = "4000 mm"};
            List<CechyBll> listacech1 = new List<CechyBll>(){cecha1,cecha2,cecha3};
            List<CechyBll> listacech2 = new List<CechyBll>(){cecha4,cecha5};
            List<CechyBll> listacech3 = new List<CechyBll>(){cecha6,cecha7,cecha8,cecha9};


            string filtr = @"kategorie[agd i rtv\lodówki;sprzęt sportowy\narty;agd i rtv\kuchenki mikrofalowe]kolejność[452;361]długość[1000 mm;2000 mm;3000 mm; 4000 mm]";
            AtrybutyBLL atrybut1 = new AtrybutyBLL();
            atrybut1.atrybut_id = 1;
            atrybut1.nazwa = "kategorie";
            atrybut1.ListaCech = listacech1;
            

            AtrybutyBLL atrybut2 = new AtrybutyBLL();
            atrybut2.atrybut_id = 2;
            atrybut2.nazwa = "kolejność";
            atrybut2.ListaCech=listacech2;

            AtrybutyBLL atrybut3 = new AtrybutyBLL();
            atrybut3.atrybut_id = 3;
            atrybut3.nazwa = "długość";
            atrybut3.ListaCech = listacech3;
            List<AtrybutyBLL> listaAtrybutow = new List<AtrybutyBLL>() { atrybut1, atrybut2, atrybut3 };

            var cechyAtr = A.Fake<CechyAtrybuty>();
            A.CallTo(() => cechyAtr.WszystkieAtrybutyWeWszystkichJezykach()).Returns(listaAtrybutow);

            Dictionary<int, HashSet<int>> wynik = cechyAtr.SlownikFiltrow(filtr);

            Assert.True(wynik[1].Count == 3);
            Assert.True(wynik[2].Count == 2);
            Assert.True(wynik[3].Count == 4);

            Assert.True(wynik.ContainsKey(1));
            Assert.True(wynik.ContainsKey(2));
            Assert.True(wynik.ContainsKey(3));

            Assert.True((wynik[1].Contains(cecha1.cecha_id)));
            Assert.True((wynik[1].Contains(cecha2.cecha_id)));
            Assert.True((wynik[1].Contains(cecha3.cecha_id)));
            Assert.True((wynik[2].Contains(cecha4.cecha_id)));
            Assert.True((wynik[2].Contains(cecha5.cecha_id)));
            Assert.True((wynik[3].Contains(cecha6.cecha_id)));
            Assert.True((wynik[3].Contains(cecha7.cecha_id)));
            Assert.True((wynik[3].Contains(cecha8.cecha_id)));
            Assert.True((wynik[3].Contains(cecha9.cecha_id)));

        }
    }
}
