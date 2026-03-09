using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using Xunit;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Rabaty;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Rabaty.Tests
{
    public class WidocznoscProduktowPoCeszeTests
    {
        [Fact(DisplayName = "Test sprawdzający poprawność działania modułu ukrywającego/ pokazujacego po okreslonej cesze")]
        public void WidocznoscProduktowPoCeszeTest()
        {
            Cecha cecha1 = new Cecha("tak", "cecha1") { Id = 1 };
            Cecha cecha2 = new Cecha("nie", "cecha2") { Id = 2 };
            Cecha cecha3 = new Cecha("adfasdfasdf", "cecha3") { Id = 3 };
            Cecha cecha4 = new Cecha("sghfghdfgh", "cecha4") { Id = 4 };

            List<Cecha> listaCech = new List<Cecha>() { cecha1, cecha2, cecha3, cecha4 };

            WidocznoscProduktowPoCesze wppc = new WidocznoscProduktowPoCesze();
            wppc.Cecha=new List<string>(){"1","8"};
            wppc.TypWidocznosci = KatalogKlientaTypy.Dostepne;

            List<Rabat> rabaty = new List<Rabat>();
            IDictionary<int, KategoriaKlienta> katkl = new Dictionary<int, KategoriaKlienta>();
            IDictionary<long, KlientKategoriaKlienta> klkat = new Dictionary<long, KlientKategoriaKlienta>();
            List<ProduktUkryty> pu = new List<ProduktUkryty>();
            Dictionary<long, Konfekcje> konfekcje = new Dictionary<long, Konfekcje>();

            wppc.Przetworz(ref rabaty, ref pu, ref konfekcje, new Dictionary<long, Klient>(0), new Dictionary<long, Produkt>(), new List<PoziomCenowy>(), listaCech, new Dictionary<long, ProduktCecha>(), new Dictionary<long, KategoriaProduktu>(), new List<ProduktKategoria>(), ref katkl, ref klkat);
            Assert.True(pu.Count==1);
            Assert.True(pu[0].Tryb==KatalogKlientaTypy.Dostepne && pu[0].CechaProduktuId==cecha1.Id);
        }

    }
}

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci.Tests
{
    public class WidocznoscProduktowPoCeszeTests
    {
        [Fact()]
        public void PrzetworzTest()
        {
            Cecha cecha1 = new Cecha("tak", "ukryj:tak"){ Id = 1};
            Cecha cecha2 = new Cecha("nie", "ukryj:nie"){ Id = 2};
            Cecha cecha3 = new Cecha("adfasdfasdf", "asdadf:nie"){ Id = 3};
            Cecha cecha4 = new Cecha("sghfghdfgh", "w5t54tertrt:sghsdfghtg"){ Id = 4};

            List<Cecha> listaCech = new List<Cecha>(){  cecha1, cecha2, cecha3, cecha4 };
            
            WidocznoscProduktowPoPoczatkuCechy modul = new WidocznoscProduktowPoPoczatkuCechy();

            modul.PoczatekCechy = "ukryj:n";
            modul.TypWidocznosci = KatalogKlientaTypy.Wykluczenia;
            List<Rabat> rabaty = new List<Rabat>();
            IDictionary<int, KategoriaKlienta> katkl = new Dictionary<int, KategoriaKlienta>();
            IDictionary<long, KlientKategoriaKlienta> klkat = new Dictionary<long, KlientKategoriaKlienta>();
            List<ProduktUkryty> pu = new List<ProduktUkryty>();
            Dictionary<long, Konfekcje> konfekcje = new Dictionary<long, Konfekcje>();

            modul.Przetworz(ref rabaty, ref pu, ref konfekcje, new Dictionary<long, Klient>(0),
                new Dictionary<long, Produkt>(), new List<PoziomCenowy>(), listaCech, new Dictionary<long, ProduktCecha>(),
                new Dictionary<long, KategoriaProduktu>(), new List<ProduktKategoria>(), ref katkl, ref klkat);

            Assert.Equal(1, pu.Count);
            Assert.Equal(cecha2.Id, pu.First().CechaProduktuId);
            Assert.Equal(modul.TypWidocznosci, pu.First().Tryb);

        }
    }
}
