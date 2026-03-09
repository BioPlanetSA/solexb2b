using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using ServiceStack.Common.Extensions;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using Xunit;
namespace SolEx.Hurt.Core.Tests
{
    public class ProduktyUkryteBllTests
    {
        [Fact(DisplayName = "Pobieranie produktów ukrytych dla niezalogowanych dla wszystkich typów")]
        public void ProduktyUkryteWykluczeniaTest()
        {
            ProduktyUkryteTest(KatalogKlientaTypy.Wykluczenia);
            ProduktyUkryteTest(KatalogKlientaTypy.MojKatalog);
            ProduktyUkryteTest(KatalogKlientaTypy.Dostepne);
        }

        private void ProduktyUkryteTest(KatalogKlientaTypy typ)
        {
            var reguly = WygenerujReguly();
            var kategorie = WygenerujKategorie();

            IKlienciDostep kliencidostep = A.Fake<IKlienciDostep>();
            IKlient klient = A.Fake<IKlient>();

            var cechy = WygenerujCechy();

            ICechyProduktyDostep cechyAtrybuty = A.Fake<ICechyProduktyDostep>();
            A.CallTo(() => cechyAtrybuty.WszystkieLacznikiWgCech()).Returns(cechy);

            IProduktyKategorieDostep kategorieDostep = A.Fake<IProduktyKategorieDostep>();
            A.CallTo(() => kategorieDostep.ProduktyKategorieGrupowanePoKategorii).Returns(kategorie);

            A.CallTo(() => klient.klient_id).Returns(69);

            A.CallTo(() => kliencidostep.Pobierz(69)).Returns(klient);

            ProduktyUkryteBll produkryBll = A.Fake<ProduktyUkryteBll>();
            produkryBll.Kliencidostep = kliencidostep;
            produkryBll.CechyProdukty = cechyAtrybuty;
            produkryBll.ProduktyKategorieDostep = kategorieDostep;

            A.CallTo(() => produkryBll.PobierzReguly(typ)).Returns(reguly);

            var wynik = produkryBll.Get(69, typ);

            Assert.Equal(27, wynik.Count);
            int doOdjecia = 0;
            for (int i = 0; i < 27; i++)
            {
                if (i == 13 || i == 19 || i == 25)
                    doOdjecia++;

                Assert.Equal(i + 1, wynik.ElementAt(i) - doOdjecia);
            }
        }

        [Fact(DisplayName = "Pobieranie produktów ukrytych dla kategorii klienta dla wszystkich typów")]
        public void ProduktyUkryteKategorieKlientaTest()
        {
            ProduktyUkryteKategorieKlienta(KatalogKlientaTypy.Wykluczenia);
            ProduktyUkryteKategorieKlienta(KatalogKlientaTypy.MojKatalog);
            ProduktyUkryteKategorieKlienta(KatalogKlientaTypy.Dostepne);
        }


        private void ProduktyUkryteKategorieKlienta(KatalogKlientaTypy typ)
        {
            var reguly = WygenerujReguly();
            var kategorie = WygenerujKategorie();

            IKlienciDostep kliencidostep = A.Fake<IKlienciDostep>();
            IKlient klient = A.Fake<IKlient>();

            var cechy = WygenerujCechy();

            ICechyProduktyDostep cechyAtrybuty = A.Fake<ICechyProduktyDostep>();
            A.CallTo(() => cechyAtrybuty.WszystkieLacznikiWgCech()).Returns(cechy);

            IProduktyKategorieDostep kategorieDostep = A.Fake<IProduktyKategorieDostep>();
            A.CallTo(() => kategorieDostep.ProduktyKategorieGrupowanePoKategorii).Returns(kategorie);

            A.CallTo(() => klient.klient_id).Returns(69);
            var kategorieklienta = WygenerujKategorieKlientow();
          //  A.CallTo(() => klient.Kategorie).Returns(kategorieklienta);

            A.CallTo(() => kliencidostep.Pobierz(69)).Returns(klient);

            ProduktyUkryteBll produkryBll = A.Fake<ProduktyUkryteBll>();
            produkryBll.Kliencidostep = kliencidostep;
            produkryBll.CechyProdukty = cechyAtrybuty;
            produkryBll.ProduktyKategorieDostep = kategorieDostep;

            A.CallTo(() => produkryBll.PobierzReguly(typ)).Returns(reguly);
         
            var wynik = produkryBll.Get(69, typ);

            Assert.Equal(27, wynik.Count);
            int doOdjecia = 0;
            for (int i = 0; i < 27; i++)
            {
                if (i == 13 || i == 19 || i == 25)
                    doOdjecia++;

                Assert.Equal(i + 1, wynik.ElementAt(i) - doOdjecia);
            }
        }

        private List<kategorie_klientow> WygenerujKategorieKlientow()
        {
            List<kategorie_klientow> kategorie = new List<kategorie_klientow>();

            for (int i = 8; i < 16; i++)
            {
                kategorie_klientow k = new kategorie_klientow();
                k.Id = i;
                k.nazwa = "kategoria " + i;
            }

            return kategorie;
        }

    private Dictionary<int, int[]> WygenerujCechy()
        {
            Dictionary<int,int[]> slownik = new Dictionary<int, int[]>();
            for (int i = 1; i <= 200; i++)
            {
                slownik.Add(i, new int[10]);

                for (int j = 1; j <= 10; j++)
                    slownik[i][j-1] = j;
            }

            return slownik;
        }

        private Dictionary<int, int[]> WygenerujKategorie()
        {
            Dictionary<int, int[]> slownik = new Dictionary<int, int[]>();
            for (int i = 1; i <= 200; i++)
            {
                slownik.Add(i, new int[10]);

                for (int j = 1; j <= 10; j++)
                    slownik[i][j-1]=j;
            }

            return slownik;
        }

        private List<produkty_ukryte> WygenerujReguly()
        {
            List<produkty_ukryte> listapu = new List<produkty_ukryte>();
            int indexpoczatkowy = 1;
            
            for (int i = indexpoczatkowy; i <= 10; i++)
            {
                produkty_ukryte pu = new produkty_ukryte();

                pu.CechaProduktuId = i;

                if (i % 7 == 0)
                {
                    pu.KategoriaKlientowId = i;
                    pu.klient_zrodlo_id = null;
                }

                listapu.Add(pu);
            }

            for (int i = indexpoczatkowy; i <= 20; i++)
            {
                produkty_ukryte pu = new produkty_ukryte();

                pu.kategoria_id = i;

                if (i % 7 == 0)
                {
                    pu.KategoriaKlientowId = i;
                    pu.klient_zrodlo_id = null;
                }

                listapu.Add(pu);
            }

            for (int i = indexpoczatkowy; i <= 30; i++)
            {
                produkty_ukryte pu = new produkty_ukryte();

                pu.produkt_zrodlo_id = i;

                if (i % 7 == 0)
                {
                    pu.KategoriaKlientowId = i;
                    pu.klient_zrodlo_id = null;
                }

                listapu.Add(pu);
            }

            return listapu;
        }
    }
}
