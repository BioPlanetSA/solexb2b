using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using Xunit;

namespace SolEx.Hurt.Sync.CoreTests.GlobalneModuly
{
    public class KategorieModTests
    {
        [Fact(DisplayName = "Test dla metody pobierajacej kategorie")]
        public void PobierzKategorieTest()
        {
            Grupa g = new Grupa(){Id = 1, Parametry = "kategorie"};
            Cecha cecha1 = new Cecha() { Id = 1,Symbol = "kategorie:agd i rtv\\lodówki", Nazwa="agd i rtv\\lodówki" };
            Cecha cecha2 = new Cecha() { Id = 2, Symbol = "kategorie:sprzęt sportowy\\rowery", Nazwa = "sprzęt sportowy\\rowery" };
            Cecha cecha3 = new Cecha() { Id = 3, Symbol = "kategorie:agd i rtv\\kuchenki mikrofalowe", Nazwa = "agd i rtv\\kuchenki mikrofalowe" };
            Cecha cecha4 = new Cecha() { Id = 4, Symbol = "kategorie:sprzęt sportowy\\narty", Nazwa = "sprzęt sportowy\narty" };
            Cecha cecha5 = new Cecha() { Id = 5, Symbol = "kategorie:sprzęt sportowy", Nazwa = "sprzęt sportowy" };
            Cecha cecha6 = new Cecha() { Id = 6, Symbol = "kategorie:agd i rtv", Nazwa = "agd i rtv" };
            Dictionary<long, Cecha> listaCech = new Dictionary<long, Cecha>();
            listaCech.Add(cecha1.Id,cecha1);
            listaCech.Add(cecha2.Id,cecha2);
            listaCech.Add(cecha3.Id,cecha3);
            listaCech.Add(cecha4.Id,cecha4);
            listaCech.Add(cecha5.Id,cecha5);
            listaCech.Add(cecha6.Id,cecha6);

            string[] separatory = { "/", "\\" };

            var konf = A.Fake<IConfigBLL>();
            A.CallTo(() => konf.DadawanieAtrybutuDoKategorii).Returns(true);
            A.CallTo(() => konf.SeparatoryDrzewkaKategorii).Returns(separatory);


            var konf2 = A.Fake<IConfigBLL>();
            A.CallTo(() => konf2.DadawanieAtrybutuDoKategorii).Returns(false);
            A.CallTo(() => konf2.SeparatoryDrzewkaKategorii).Returns(separatory);

            var api = A.Fake<IAPIWywolania>();
            A.CallTo(() => api.PobierzCechy()).Returns(listaCech);

            var kat = new KategorieProduktowSyncHelper();
            kat.Config = konf;
            kat.ApiWywolanie = api;

            List<KategoriaProduktu> listaKategorii = kat.PobierzKategorie(g);
            Assert.True(listaKategorii[0].Nazwa==cecha1.Symbol);
            Assert.True(listaKategorii[1].Nazwa == cecha2.Symbol);
            Assert.True(listaKategorii[2].Nazwa == cecha3.Symbol);
            Assert.True(listaKategorii[3].Nazwa == cecha4.Symbol);
            Assert.True(listaKategorii[4].Nazwa == cecha5.Symbol);
            Assert.True(listaKategorii[5].Nazwa == cecha6.Symbol);

            var kat2 = new KategorieProduktowSyncHelper();
            kat2.Config = konf2;
            kat2.ApiWywolanie = api;
            listaKategorii = kat2.PobierzKategorie(g);
            Assert.True(listaKategorii[0].Nazwa == cecha1.Nazwa);
            Assert.True(listaKategorii[1].Nazwa == cecha2.Nazwa);
            Assert.True(listaKategorii[2].Nazwa == cecha3.Nazwa);
            Assert.True(listaKategorii[3].Nazwa == cecha4.Nazwa);
            Assert.True(listaKategorii[4].Nazwa == cecha5.Nazwa);
            Assert.True(listaKategorii[5].Nazwa == cecha6.Nazwa);



        }

        [Fact(DisplayName = "Test poprawności budowania drzewka")]
        public void KategorieZCechyTest()
        {
            Dictionary<long, KategoriaProduktu> kategoriee = new Dictionary<long, KategoriaProduktu>();
            Grupa grupa = new Grupa() { Id = 1, Nazwa = "kategorie" };
            string[] separatory = { "/", "\\" };

            var konf = A.Fake<IConfigBLL>();
            A.CallTo(() => konf.SeparatoryDrzewkaKategorii).Returns(separatory);

            KategoriaProduktu k1 = new KategoriaProduktu() { Id = 1, GrupaId = 1, Nazwa = "kategorie:agd i rtv\\lodówki" };
            KategoriaProduktu k2 = new KategoriaProduktu() { Id = 2, GrupaId = 1, Nazwa = "kategorie:sprzęt sportowy\\narty" };
            KategoriaProduktu k3 = new KategoriaProduktu() { Id = 3, GrupaId = 1, Nazwa = "kategorie:towary używane\\stoly, stoliki" };
            KategoriaProduktu k4 = new KategoriaProduktu() { Id = 4, GrupaId = 1, Nazwa = "kategorie:agd i rtv" };
            KategoriaProduktu k5 = new KategoriaProduktu() { Id = 5, GrupaId = 1, Nazwa = "kategorie:sprzęt sportowy" };
            KategoriaProduktu k6 = new KategoriaProduktu() { Id = 6, GrupaId = 1, Nazwa = "kategorie:towary używane" };
            List<KategoriaProduktu> listaKategori = new List<KategoriaProduktu>() { k1, k2, k3, k4, k5, k6 };

            var kategorie = A.Fake<KategorieProduktowSyncHelper>();
            A.CallTo(() => kategorie.PobierzKategorie(grupa)).Returns(listaKategori);
            kategorie.Config = konf;
            kategorie.KategorieZCechy(grupa, kategoriee);

            var kat = kategoriee.Values.ToList();
            Assert.True(kat[0].ParentId == 4, string.Format("Oczekiwano 4, otrzymano {0}", kat[0].ParentId));
            Assert.True(kat[1].ParentId == 5, string.Format("Oczekiwano 5, otrzymano {0}", kat[1].ParentId));
            Assert.True(kat[2].ParentId == 6, string.Format("Oczekiwano 6, otrzymano {0}", kat[2].ParentId));
            Assert.True(kat[3].ParentId == null, string.Format("Oczekiwano null, otrzymano {0}", kat[3].ParentId));
            Assert.True(kat[4].ParentId == null, string.Format("Oczekiwano null, otrzymano {0}", kat[4].ParentId));
            Assert.True(kat[5].ParentId == null, string.Format("Oczekiwano null, otrzymano {0}", kat[5].ParentId));
        }

        [Fact]
        public void TestCaloscParenci()
        {
            Grupa g = new Grupa() { Id = 1, Parametry = "kategorie" };
            Cecha cecha1 = new Cecha() { Id = 1, Symbol = "kategorie:agd i rtv - lodówki", Nazwa = "agd i rtv - lodówki" };
            Cecha cecha2 = new Cecha() { Id = 2, Symbol = "kategorie:sprzęt sportowy - rowery", Nazwa = "sprzęt sportowy - rowery" };
            Cecha cecha3 = new Cecha() { Id = 3, Symbol = "kategorie:agd i rtv - kuchenki mikrofalowe", Nazwa = "agd i rtv - kuchenki mikrofalowe" };
            Cecha cecha4 = new Cecha() { Id = 4, Symbol = "kategorie:sprzęt sportowy - narty", Nazwa = "sprzęt sportowy\narty" };
            Cecha cecha5 = new Cecha() { Id = 5, Symbol = "kategorie:sprzęt sportowy", Nazwa = "sprzęt sportowy" };
            Cecha cecha6 = new Cecha() { Id = 6, Symbol = "kategorie:agd i rtv", Nazwa = "agd i rtv" };
            Dictionary<long, Cecha> listaCech = new Dictionary<long, Cecha>();
            listaCech.Add(cecha1.Id, cecha1);
            listaCech.Add(cecha2.Id, cecha2);
            listaCech.Add(cecha3.Id, cecha3);
            listaCech.Add(cecha4.Id, cecha4);
            listaCech.Add(cecha5.Id, cecha5);
            listaCech.Add(cecha6.Id, cecha6);

            string[] separatory = { "-" };

            var konf = A.Fake<IConfigBLL>();
            A.CallTo(() => konf.DadawanieAtrybutuDoKategorii).Returns(false);
            A.CallTo(() => konf.SeparatoryDrzewkaKategorii).Returns(separatory);



            var api = A.Fake<IAPIWywolania>();
            A.CallTo(() => api.PobierzCechy()).Returns(listaCech);

            var kat = new KategorieProduktowSyncHelper();
            kat.Config = konf;
            kat.ApiWywolanie = api;
            Dictionary<long, KategoriaProduktu> kategorie = new Dictionary<long, KategoriaProduktu>();
            kat.KategorieZCechy(g, kategorie);
            Assert.Equal(kategorie[1].ParentId,6);
            Assert.Equal(kategorie[3].ParentId, 6);
        }
    }
}


