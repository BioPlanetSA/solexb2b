using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using Fasterflect;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using Xunit;
namespace SolEx.Hurt.Core.BLL.Tests
{
    public class SklepyTests
    {
        //[Fact(DisplayName = "Pobieranie nazw miast sklepów dla wybranej kategorii w której jest sklep")]
        //public void PobierzMiastaTest()
        //{
        //    string nazwamiasta = "Miasto";
        //    int wlasciwyNumerKategorii = 69;
        //    Sklepy sklepy = A.Fake<Sklepy>();
        //    var slownik = WygenerujTestoweSklepy(nazwamiasta, wlasciwyNumerKategorii);
        //    A.CallTo(() => sklepy.WszystkieSlownik(1)).Returns(slownik);

        //    List<string> pobraneNazwySklepow = sklepy.PobierzMiasta(1, 69);


        //    Assert.Equal(4, pobraneNazwySklepow.Count);
        //    Assert.Equal(string.Format("{0} {1}", nazwamiasta, 0), pobraneNazwySklepow[0]);
        //    Assert.Equal(string.Format("{0} {1}", nazwamiasta, 3), pobraneNazwySklepow[1]);
        //    Assert.Equal(string.Format("{0} {1}", nazwamiasta, 6), pobraneNazwySklepow[2]);
        //    Assert.Equal(string.Format("{0} {1}", nazwamiasta, 9), pobraneNazwySklepow[3]);
        //}

        //private Dictionary<int, ISklepyBll> WygenerujTestoweSklepy(string nazwaMiasta, int idWlasciwejKategorii)
        //{
        //    Dictionary<int, ISklepyBll> slownik = new Dictionary<int, ISklepyBll>();

        //    for (int i = 0; i < 10; i++)
        //    {
        //        ISklepyBll sklep = A.Fake<ISklepyBll>();
        //        sklep.id = i;
        //        A.CallTo(() => sklep.Adres.Miasto).Returns(string.Format("{0} {1}", nazwaMiasta, i));
        //        A.CallTo(() => sklep.Kategorie).Returns(new List<sklepy_kategorie>() { new sklepy_kategorie() { id = i % 3 == 0 ? idWlasciwejKategorii : i, nazwa = "AAAAAA" } });
        //        A.CallTo(() => sklep.CzyPoprawneKoodrynaty).Returns(true);

        //        slownik.Add(sklep.id, sklep);
        //    }

        //    return slownik;
        //}

        [Fact(DisplayName = "Sprawdzenie poprawności danych dla metody uzupełniającej współrzędne sklepu")]
        public void UzupelnijWspolrzedneTest()
        {
            decimal lat1 = 23.44m;
            decimal lon1 = 23.59m;
            bool zwrocona1;
            bool zwracany1 = false;
            sklepy sklep1 = UzupelnijWspolrzedne(lat1, lon1,zwracany1, out zwrocona1);

            Assert.Equal(zwracany1, zwrocona1);
            Assert.Equal(lat1, sklep1.lat);
            Assert.Equal(lon1, sklep1.lon);

            decimal lat2 = -23.44m;
            decimal lon2 = 23.59m;
            bool zwrocona2;
            bool zwracany2 = true;
            sklepy sklep2 = UzupelnijWspolrzedne(lat2, lon2, zwracany2, out zwrocona2);

            Assert.Equal(zwracany2, zwrocona2);
            Assert.Equal(lat2, sklep2.lat);
            Assert.Equal(lon2, sklep2.lon);

            decimal lat3 = 0m;
            decimal lon3 = 0m;
            bool zwrocona3;
            bool zwracany3 = false;
            sklepy sklep3 = UzupelnijWspolrzedne(lat3, lon3, zwracany3, out zwrocona3);

            Assert.Equal(zwracany3, zwrocona3);
            Assert.Equal(lat3, sklep3.lat);
            Assert.Equal(lon3, sklep3.lon);

            decimal lat4 = -3.99m;
            decimal lon4 = -159.01m;
            bool zwrocona4;
            bool zwracany4 = false;
            sklepy sklep4 = UzupelnijWspolrzedne(lat4, lon4, zwracany4, out zwrocona4);

            Assert.Equal(zwracany4, zwrocona4);
            Assert.Equal(lat4, sklep4.lat);
            Assert.Equal(lon4, sklep4.lon);

        }

        private sklepy UzupelnijWspolrzedne(decimal lat, decimal lon, bool zwracany, out bool zwrocony)
        {
            Sklepy sklepy = A.Fake<Sklepy>();
            sklepy sklep = new sklepy();
            decimal la, lo;
            A.CallTo(() => sklepy.LocationGeoCode(sklep, out la, out lo))
                .Returns(zwracany)
                .AssignsOutAndRefParameters(new object[] { lat, lon});
            
            zwrocony = sklepy.UzupelnijWspolrzedne(sklep);

            return sklep;
        }

        [Fact(DisplayName = "Test sprawdzający metodę PobierzNiepusteKategorie")]
        public void PobierzNiepusteKategorieTest()
        {
            HashSet<long> kategorieIds = new HashSet<long>(){2,3,4,5};
            List<sklepy_kategorie> listaKategrii = new List<sklepy_kategorie>();
            IList<sklepy_kategorie> listaSklepowKategorie = new List<sklepy_kategorie>();
            ISklepyKategorieBll kategorie = A.Fake<ISklepyKategorieBll>();
            List<ISklepyBll> skl = new List<ISklepyBll>();
            ISklepy sklepy = A.Fake<ISklepy>();
            
            ISklepyBll sklep = A.Fake<ISklepyBll>();
            sklep.nazwa = "sklep1";
            skl.Add(sklep);

            sklepy_kategorie s1 = new sklepy_kategorie() { id = 1, PokazywanaNaMapie = true};
            sklepy_kategorie s2 = new sklepy_kategorie() { id = 2, PokazywanaNaMapie = true};
            sklepy_kategorie s3 = new sklepy_kategorie() { id = 3, PokazywanaNaMapie = true };
            sklepy_kategorie s4 = new sklepy_kategorie() { id = 4, PokazywanaNaMapie = true };

            listaSklepowKategorie.Add(s1);
            listaSklepowKategorie.Add(s2);
            listaSklepowKategorie.Add(s3);
            listaSklepowKategorie.Add(s4);
            
            var calosc = A.Fake<ISolexBllCalosc>();
            A.CallTo(() => calosc.KategorieSklepow).Returns(kategorie);
            A.CallTo(() => calosc.KategorieSklepow.Wszystykie(A<int>.Ignored)).Returns(listaSklepowKategorie);

            
            A.CallTo(() => calosc.Sklepy).Returns(sklepy);
            A.CallTo(() => sklepy.PobierzSklepyWgKategoriiAdresu(new HashSet<long>() {2}, A<string>.Ignored)).Returns(skl);

            var fsklepy = A.Fake<Sklepy>(x => x.WithArgumentsForConstructor(new[] { calosc }));
            listaKategrii = fsklepy.PobierzNiepusteKategorie(kategorieIds, 1);
            Assert.True(listaKategrii.Count == 0);
        }
    }
}
