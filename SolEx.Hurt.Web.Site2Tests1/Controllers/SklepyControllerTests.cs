using SolEx.Hurt.Web.Site2.Controllers;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Web.Site2.Models;
using SolEx.Hurt.Web.Site2.Models.KontrolkiTresci;
using Xunit;

namespace SolEx.Hurt.Web.Site2.Controllers.Tests
{
    public class SklepyControllerTests: SklepyController
    {
        [Fact(DisplayName = "Poprawność pobierania sklepów na mapę.")]
        public void PobierzSklepyNaMapeWPostaciPuntowTest()
        {
            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            SklepyController sklep = new SklepyController();
            sklep.Calosc = calosc;
            JsonResult wynik;
            try
            {
                wynik = sklep.PobierzSklepyNaMapeWPostaciPuntow(0, new long[] { 1, 2 }, "test");
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("Brak kontrolki id, złe parametry"))
                {
                    throw new Exception("Nie wystapił wyjątek czyli test NIE przeszedł - powinien się wywalić błąd: Brak kontrolki id, złe parametry");
                }
            }
            //A.CallTo(() => calosc.Konfiguracja.JezykIDDomyslny).Returns(0);
            //TrescKolumnaBll trescKolumna = new TrescKolumnaBll();
            //trescKolumna.Calosc = calosc;
            //trescKolumna.RodzajKontrolki = "SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.Sklepy";
            //A.CallTo(() => calosc.DostepDane.PobierzPojedynczy<TrescKolumnaBll>(1)).Returns(trescKolumna);
            //try
            //{
            //    wynik = sklep.PobierzSklepyNaMapeWPostaciPuntow(1, new long[] { 1, 2 }, "test");
            //}
            //catch (Exception ex)
            //{
            //    if (!ex.Message.Contains("Brak kontrolki id: 1"))
            //    {
            //        throw new Exception("Nie wystapił wyjątek czyli test NIE przeszedł - powinien się wywalić błąd: Brak kontrolki id: 1");
            //    }
            //}
        }

        [Fact(DisplayName = "Sprawdzamy poprawność uzupełniania kategorii")]
        public void UzupelnijParametryOKategorieTest()
        {
            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            this.Calosc = calosc;
            A.CallTo(() => calosc.Sklepy.PobierzKategorieNiepusteIPoprawneKoordynaty(A<HashSet<long>>.Ignored, A<int>.Ignored)).Returns(null);

            ParametryDoSklepu parametry = new ParametryDoSklepu(null);
            int jezykId = 1;
            ActionResult content;
            bool wynik = this.UzupelnijParametryOKategorie(new HashSet<long>(),parametry, jezykId,out content);
            Assert.True(wynik, "Funkcja powinna zwrócić true.");
            Assert.True(content!=null, "Zwrócony content nie powinien być null-em");
            KategoriaSklepu k1 = new KategoriaSklepu();
            k1.Nazwa = "Test1";

            A.CallTo(() => calosc.Sklepy.PobierzKategorieNiepusteIPoprawneKoordynaty(A<HashSet<long>>.Ignored, A<int>.Ignored)).Returns(new List<KategoriaSklepu>() {k1});

            wynik = this.UzupelnijParametryOKategorie(new HashSet<long>(), parametry, jezykId, out content);
            Assert.False(wynik, "Funkcja powinna zwrócić false.");
            Assert.True(content == null, "Zwrócony content  powinien być null-em");
            Assert.True(parametry.KategorieSklepowDoPokazania.Count==1, "Ilosć zwróconych kategorii powinna być równa 1");
            Assert.True(parametry.KategorieSklepowDoPokazania[0].Nazwa.Equals("Test1"),$"Zwrócona kategoria powinna mieć wartość Test1 a ma {parametry.KategorieSklepowDoPokazania[0].Nazwa}" );
        }

        [Fact(DisplayName = "Sprawdzamy poprawność uzupełniania miast")]
        public void UzupelnijParametryOMiastaTest()
        {
            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            this.Calosc = calosc;
            A.CallTo(() => calosc.Sklepy.MiastaDlaWybranychKategorii(A<HashSet<long>>.Ignored)).Returns(null);

            ParametryDoSklepu parametry = new ParametryDoSklepu(null);
            parametry.Kontrolka = new Models.KontrolkiTresci.Sklepy();

            this.UzupelnijParametryOMiasta(new HashSet<long>(), parametry);
            Assert.False(parametry.Kontrolka.SklepyListaMiast, "NIe ma sklepów wiec parametr powinien być ustawiony na false");

            A.CallTo(() => calosc.Sklepy.MiastaDlaWybranychKategorii(A<HashSet<long>>.Ignored)).Returns(new List<string>() {"Miasto1","Miasto2"});

            this.UzupelnijParametryOMiasta(new HashSet<long>(), parametry);
            Assert.True(parametry.MiastaDlaKategorii.Count==2, $"Powinno być dwa miasta a jest {parametry.MiastaDlaKategorii}");

        }
    }
}