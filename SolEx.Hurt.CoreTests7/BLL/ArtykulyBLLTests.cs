using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.CoreTests;
using SolEx.Hurt.DAL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces.Modele;
using Xunit;
namespace SolEx.Hurt.Core.BLL.Tests
{
    public class ArtykulyBLLTests
    {

        public ArtykulyBLLTests()
        {
            SolEx.Hurt.DAL.TworzenieBazyDanych.ReCreateTabele();
            TestHelperDB.WypelnijBazePrzykladowymiDanymi();
        }

        [Fact()]
        public void ZapiszKategorieArtykolowTest()
        {
            ArtykulyKategoriaBLL testowa = new ArtykulyKategoriaBLL(TestHelperDB.testowaKategoria, 1);
            ArtykulyBLL art = A.Fake<ArtykulyBLL>();
            A.CallTo(() => art.wyczyscCalyCacheKatIArt());

            LokalizacjeBLL lokalizacje = A.Fake<LokalizacjeBLL>();

            //A.CallTo(() => lokalizacje.PobierzTlumaczenie(ref testowa, 1, null));

            art.lokalizacje = lokalizacje;

            testowa.Id = art.ZapiszKategorieArtykolow(testowa);
            Assert.NotEqual(testowa.Id, 0);
            //string lastSQL = MainDAO.db.GetLastSql();
            //ArtykulyKategoriaBLL temp = art.PobierzKategorieArtykolow(testowa.Id, 1, null, false, false);

            //AssertEx.PropertyValuesAreEquals(temp, testowa);

            ////update  opisów
            //testowa.opis = "nowy opis dlugi jakis";
            //testowa.opis_krotki = "nowy krotki ois inny";

            //ArtykulyBLL.PobierzInstancje.ZapiszKategorieArtykolow(testowa);
            //lastSQL = MainDAO.db.GetLastSql();
            //temp = ArtykulyBLL.PobierzInstancje.PobierzKategorieArtykolow(testowa.Id, 1, null, false, false);
            //AssertEx.PropertyValuesAreEquals(temp, testowa);
        }

        [Fact(DisplayName = "Test sprawdzajacy poprawność metody okreslajacej widoczność artykułu dla klienta")]
        public void CzyArtykulMaBycWidocznyDlaKlientaTest()
        {
            var klient = A.Fake<IKlient>();
            A.CallTo(() => klient.Dostep).Returns(AccesLevel.Niezalogowani);
            A.CallTo(() => klient.OddzialDoJakiegoNalezyKlient).Returns(1);

            var art = A.Fake<ArtykulModelBll>();
            //ArtykulModelBll art = new ArtykulModelBll();
            art.id = 1;
            art.Dostep = AccesLevel.Niezalogowani;
            art.widoczny = true;
            A.CallTo(() => art.OddzialDoJakiegoNalezyArt).Returns(1);
            

            var widocznosc = A.Fake<IWidocznosciTypowBLL>();
            A.CallTo(() => widocznosc.KlientMaDostepDoObiektu(klient, art.id, A<Type>.Ignored)).Returns(true);

            ArtykulyBLL artykul = new ArtykulyBLL();
            artykul.Widocznosc = widocznosc;
            bool wynik = artykul.CzyArtykulMaBycWidocznyDlaKlienta(art, klient);
            Assert.True(wynik);

            //2
            A.CallTo(() => klient.Dostep).Returns(AccesLevel.Zalogowani);
            A.CallTo(() => klient.OddzialDoJakiegoNalezyKlient).Returns(1);

            //ArtykulModelBll art = new ArtykulModelBll();
            art.id = 1;
            art.Dostep = AccesLevel.Niezalogowani;
            art.widoczny = true;
            A.CallTo(() => art.OddzialDoJakiegoNalezyArt).Returns(1);

            wynik = artykul.CzyArtykulMaBycWidocznyDlaKlienta(art, klient);
            Assert.False(wynik);

            //3
            A.CallTo(() => klient.Dostep).Returns(AccesLevel.Zalogowani);
            A.CallTo(() => klient.OddzialDoJakiegoNalezyKlient).Returns(1);

            //ArtykulModelBll art = new ArtykulModelBll();
            art.id = 1;
            art.Dostep = AccesLevel.Zalogowani;
            art.widoczny = true;
            A.CallTo(() => art.OddzialDoJakiegoNalezyArt).Returns(2);

            wynik = artykul.CzyArtykulMaBycWidocznyDlaKlienta(art, klient);
            Assert.False(wynik);

        }
    }
}
