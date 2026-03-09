using Xunit;
using System.Web.Mvc;
using FakeItEasy;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Web.Site2.Models;

namespace SolEx.Hurt.Web.Site2.AtrybutyMvc.Tests
{
    public class OutputCacheSolexAttributeTests
    {
        private ISolexBllCalosc calosc;
        private ICacheBll cache;
        private string staleFiltry;

        private void InicjalizacjaZmiennych()
        {
            calosc = A.Fake<ISolexBllCalosc>();
            cache = A.Fake<ICacheBll>();
            A.CallTo(() => calosc.Cache).Returns(cache);
            
            staleFiltry = "12,36,45";
        }


        [Fact(DisplayName = "Test czy AkcjaMenu pobierze sie z cache poprawnie, lub nie jak nie ma klucza")]
        public void AkcjaMenuTest()
        {
            InicjalizacjaZmiennych();
            ISolexHelper helper = A.Fake<ISolexHelper>();
            IKlient klient = A.Fake<IKlient>();
            klient.Id = 33;
            Jezyk jezyk = new Jezyk { Id = 99 };

            A.CallTo(() => helper.AktualnyKlient).Returns(klient);
            A.CallTo(() => helper.AktualnyJezyk).Returns(jezyk);
            OutputCacheSolexAttribute outputCachecache = new OutputCacheSolexAttribute(TypDanychDoCache.Menu) { Calosc = calosc };

            string klucz = "53454";
            ActionResult wynik1 = new JsonResult();
            //pierwszy test jak id klienta == 0 to ma byc NIE sprwdzana czy menu ma ukrywane tresci
            A.CallTo(() => cache.WyliczKluczDlaMenu(4, 0, jezyk.Id, false)).Returns(klucz);
            A.CallTo(() => cache.PobierzObiekt<ActionResult>(klucz)).Returns(wynik1);
            ActionResult wynik = outputCachecache.AkcjaMenu(4, 0, jezyk.Id);
            Assert.True(wynik == wynik1);
            A.CallTo(() => calosc.TresciDostep.SprawdzCzyKontrolaMenuMaTresciUkrywaneWgKlientow(A<int>.Ignored, null)).MustHaveHappened(Repeated.Never);


            //jak jest null zwrocony w  Calosc.TresciDostep.SprawdzCzyKontrolaMenuMaTresciUkrywaneWgKlientow to trzba zwrcoci nulla bo nie bedzie cache
            int idKontrolki = -666;
            A.CallTo(() => calosc.TresciDostep.SprawdzCzyKontrolaMenuMaTresciUkrywaneWgKlientow(idKontrolki, A<TrescBll>.Ignored)).Returns(null);
            wynik = outputCachecache.AkcjaMenu(idKontrolki, klient.Id, jezyk.Id);
            Assert.True(idKontrolki == -666);
            Assert.Null(wynik);

            //drugi test - jak cache ma byc PER klient i id klienta != 0
            idKontrolki = 20;
            klucz += "22";
            A.CallTo(() => calosc.TresciDostep.SprawdzCzyKontrolaMenuMaTresciUkrywaneWgKlientow(idKontrolki, A<TrescBll>.Ignored)).Returns(true);
            A.CallTo(() => cache.WyliczKluczDlaMenu(idKontrolki, klient.Id, jezyk.Id, true)).Returns(klucz);
            A.CallTo(() => cache.PobierzObiekt<ActionResult>(klucz)).Returns(wynik1);

            wynik = outputCachecache.AkcjaMenu(idKontrolki, klient.Id, jezyk.Id);

            Assert.NotNull(wynik);
            Assert.Equal(wynik,wynik1);


            //TRZECI test - jak cache ma byc PER klient i id klienta != 0
            idKontrolki = 33;
            klucz += "33";
            A.CallTo(() => calosc.TresciDostep.SprawdzCzyKontrolaMenuMaTresciUkrywaneWgKlientow(idKontrolki, A<TrescBll>.Ignored)).Returns(false);
            A.CallTo(() => cache.WyliczKluczDlaMenu(idKontrolki, klient.Id, jezyk.Id, false)).Returns(klucz);
            A.CallTo(() => cache.PobierzObiekt<ActionResult>(klucz)).Returns(wynik1);

            wynik = outputCachecache.AkcjaMenu(idKontrolki, klient.Id, jezyk.Id);

            Assert.NotNull(wynik);
            Assert.Equal(wynik, wynik1);
        }

        [Fact(DisplayName = "Test czy AkcjaKategorie pobierze sie z cache poprawnie")]
        public void AkcjaKategorieTest()
        {
            InicjalizacjaZmiennych();
            IKlient klient = A.Fake<IKlient>();
            klient.Id = 33;
            
            string szukane = "szukane";
            string klucz = $"Kategorie_1_{staleFiltry}_{szukane}";
            int id1 = 11;
            ActionResult wynik1 = new JsonResult();
            A.CallTo(() => cache.WyliczKluczDlaKategorii(id1, klient, szukane)).Returns(klucz);

            A.CallTo(() => cache.PobierzObiekt<ActionResult>(klucz)).Returns(wynik1);

            OutputCacheSolexAttribute outputCachecache = new OutputCacheSolexAttribute(TypDanychDoCache.Kategorie) { Calosc = calosc };
            bool nieCachuj = false;
            var wynik = outputCachecache.AkcjaKategorie(id1, klient, szukane, out nieCachuj);
            Assert.NotNull(wynik);
            Assert.Equal(wynik,wynik1);
            
        }

        [Fact()]
        public void AkcjaFiltryTest()
        {
            InicjalizacjaZmiennych();
            IKlient klient = A.Fake<IKlient>();
            klient.Id = 33;
            
            A.CallTo(() => calosc.ProfilKlienta.PobierzStaleFiltryString(klient)).Returns(staleFiltry);
            ParametryPrzekazywaneDoListyProduktow param = new ParametryPrzekazywaneDoListyProduktow();
            param.Calosc = calosc;

            string klucz = param.KluczDoCachuFiltrow(klient);
            ActionResult wynik1 = new JsonResult();
            A.CallTo(() => cache.PobierzObiekt<ActionResult>(klucz)).Returns(wynik1);

            OutputCacheSolexAttribute outputCachecache = new OutputCacheSolexAttribute(TypDanychDoCache.Kategorie) { Calosc = calosc };
            bool nieCachuj;
            var wynik = outputCachecache.AkcjaFiltry(param, klient, out nieCachuj);
            Assert.True(wynik == wynik1);
        }
    }
}