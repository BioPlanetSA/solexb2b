using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces;
using Xunit;
namespace SolEx.Hurt.Core.ModelBLL.Tests
{
    public class SklepyBllTests
    {
        [Fact(DisplayName = "Sklepy test tworzenia adresu")]
        public void SklepyBllTestAdres()
        {
            SklepyBll sklepybll = new SklepyBll();

            sklepybll.kod_pocztowy = "42-200";
            sklepybll.KrajId = 1;
            sklepybll.ulica_nr = "Kapitana Bomby 69";
            sklepybll.miasto = "Dupkowo";

            IAdres adres = sklepybll.Adres;

            Assert.Equal(sklepybll.kod_pocztowy, adres.KodPocztowy);
            Assert.Equal(sklepybll.KrajId, adres.KrajId);
            Assert.Equal(sklepybll.ulica_nr, adres.UlicaNr);
            Assert.Equal(sklepybll.miasto, adres.Miasto);
        }

        private Dictionary<int, sklepy_kategorie_polaczenia> WygenerujLacznikiKategorii()
        {
            Dictionary<int, sklepy_kategorie_polaczenia> slownik = new Dictionary<int, sklepy_kategorie_polaczenia>();

            slownik.Add(1, new sklepy_kategorie_polaczenia() { sklep_id = 1, sklep_kategoria_id = 1, Id = 1});
            slownik.Add(2, new sklepy_kategorie_polaczenia() { sklep_id = 1, sklep_kategoria_id = 2, Id = 2 });
            slownik.Add(3, new sklepy_kategorie_polaczenia() { sklep_id = 2, sklep_kategoria_id = 69, Id = 3 });
            slownik.Add(4, new sklepy_kategorie_polaczenia() { sklep_id = 2, sklep_kategoria_id = 666, Id = 4 });
            slownik.Add(5, new sklepy_kategorie_polaczenia() { sklep_id = 3, sklep_kategoria_id = 1, Id = 5 });
            slownik.Add(6, new sklepy_kategorie_polaczenia() { sklep_id = 4, sklep_kategoria_id = 2, Id = 6 });

            return slownik;
        }

        private List<sklepy_kategorie> WygenerujListeKategoriiSklepow()
        {
            List<sklepy_kategorie> sklepykategorie = new List<sklepy_kategorie>();

            sklepykategorie.Add(new sklepy_kategorie(){ id = 69});
            sklepykategorie.Add(new sklepy_kategorie() { id = 666 });

            //for (int i = 0; i < 10; i++)
            //{
            //    sklepy_kategorie skl = new sklepy_kategorie();
            //    skl.id = i;
            //    skl.nazwa = "sklep " + i;
            //}

            return sklepykategorie;
        }

        [Fact(DisplayName = "Pobieranie kategorii sklepów")]
        public void SklepyBllTestKategorie()
        {
            var laczniki = WygenerujLacznikiKategorii();
            var kategorie = WygenerujListeKategoriiSklepow();

            SklepyBll sklepybll = A.Fake<SklepyBll>();

            ISklepy sklepy = A.Fake<ISklepy>();
            A.CallTo(() => sklepy.PobierzWszystkiePolaczenia()).Returns(laczniki);
            sklepybll.sklepy = sklepy;

            IConfigBLL configbll = A.Fake<IConfigBLL>();
            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            A.CallTo(() => calosc.Konfiguracja).Returns(configbll);

            SklepyKategorieBll sklepyKategorieBll = A.Fake<SklepyKategorieBll>(x => x.WithArgumentsForConstructor(new[] { calosc }));


            SesjaHelper sesjaHelper = A.Fake<SesjaHelper>();
            A.CallTo(() => sesjaHelper.JezykID).Returns(1);

            sklepybll.SklepyKategorieBll = sklepyKategorieBll;
            sklepybll.sesjaHelper = sesjaHelper;
            sklepybll.id = 2;

            var pobraneLaczniki = sklepybll.PobierzPolaczeniaKategorii().ToList();

            Assert.Equal(2, pobraneLaczniki.Count);

            Assert.Equal(69, pobraneLaczniki[0]);
            Assert.Equal(666, pobraneLaczniki[1]);

            A.CallTo(() => sklepyKategorieBll.Pobierz(A<HashSet<long>>.Ignored, sesjaHelper.JezykID)).Returns(kategorie);
           
            var pobraneKategorie = sklepybll.PobierzKategorie();

            Assert.Equal(2, pobraneKategorie.Count);

            Assert.Equal(69, pobraneKategorie[0].id);
            Assert.Equal(666, pobraneKategorie[1].id);
        }
    }
}
