using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Rabaty;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Rabaty.Tests
{
    public class RabatySaturnTests
    {
        private List<string> PobierzLinijkiCSV()
        {
            string linijka1 =
                "ID;Symbol;Nazwa;Grupa;Rabat Podstawowy;Rabat_Paradyż;Rabat_Paradyż;Rabat_Paradyż;Rabat_Paradyż;Rabat_Paradyż;Rabat_Paradyż_Klinkier;Rabat_Paradyż_Klinkier;Rabat_Paradyż_Klinkier;Rabat_Paradyż_Klinkier;Rabat_Paradyż_Klinkier;Rabat_Myway;Rabat_Myway;Rabat_Myway;Rabat_Myway;Rabat_Myway;Rabat_Kwadro;Rabat_Kwadro;Rabat_Kwadro;Rabat_Kwadro;Rabat_Kwadro;Rabat_Cersanit;Rabat_Cersanit;Rabat_Cersanit;Rabat_Cersanit;Rabat_Cersanit;Rabat_Opoczno;Rabat_Opoczno;Rabat_Opoczno;Rabat_Opoczno;Rabat_Opoczno;Rabat_Opoczno_Klinkier;Rabat_Opoczno_Klinkier;Rabat_Opoczno_Klinkier;Rabat_Opoczno_Klinkier;Rabat_Opoczno_Klinkier;Rabat_Opoczno_Solar;Rabat_Opoczno_Solar;Rabat_Opoczno_Solar;Rabat_Opoczno_Solar;Rabat_Opoczno_Solar;Rabat_Polcolorit;Rabat_Polcolorit;Rabat_Polcolorit;Rabat_Polcolorit;Rabat_Polcolorit;Rabat_Tubądzin;Rabat_Tubądzin;Rabat_Tubądzin;Rabat_Tubądzin;Rabat_Tubądzin;Rabat_TubądzinPASTELE;Rabat_TubądzinPASTELE;Rabat_TubądzinPASTELE;Rabat_TubądzinPASTELE;Rabat_TubądzinPASTELE;Rabat_Domino;Rabat_Domino;Rabat_Domino;Rabat_Domino;Rabat_Domino;Rabat_Ceramstic;Rabat_Ceramstic;Rabat_Ceramstic;Rabat_Ceramstic;Rabat_Ceramstic;Rabat_Pilch;Rabat_Pilch;Rabat_Pilch;Rabat_Pilch;Rabat_Pilch;Rabat_PilchB;Rabat_PilchB;Rabat_PilchB;Rabat_PilchB;Rabat_PilchB;Rabat_Ceramika Iza;Rabat_Ceramika Iza;Rabat_Ceramika Iza;Rabat_Ceramika Iza;Rabat_Ceramika Iza;Rabat_Valdi;Rabat_Valdi;Rabat_Valdi;Rabat_Valdi;Rabat_Valdi;Rabat_Decore;Rabat_Decore;Rabat_Decore;Rabat_Decore;Rabat_Decore;Rabat_Incana;Rabat_Incana;Rabat_Incana;Rabat_Incana;Rabat_Incana;Rabat_Eleccio;Rabat_Eleccio;Rabat_Eleccio;Rabat_Eleccio;Rabat_Eleccio;Rabat_Eleccio_premium;Rabat_Eleccio_premium;Rabat_Eleccio_premium;Rabat_Eleccio_premium;Rabat_Eleccio_premium;Rabat_Mapei;Rabat_Mapei;Rabat_Mapei;Rabat_Mapei;Rabat_Mapei;Rabat_Mapei_NaStanie;Rabat_Mapei_NaStanie;Rabat_Mapei_NaStanie;Rabat_Mapei_NaStanie;Rabat_Mapei_NaStanie;Rabat_CersanitCeramika;Rabat_CersanitCeramika;Rabat_CersanitCeramika;Rabat_CersanitCeramika;Rabat_CersanitCeramika";

            string linijka2 = ";;;;;Odbiór Opoczno;Platny kurier;Odbiór Świecie;Dostawa;Kurier;Odbiór Opoczno;Platny kurier;Odbiór Świecie;Dostawa;Kurier;Odbiór Opoczno;Platny kurier;Odbiór Świecie;Dostawa;Kurier;Odbiór Opoczno;Platny kurier;Odbiór Świecie;Dostawa;Kurier;Odbiór Opoczno;Platny kurier;Odbiór Świecie;Dostawa;Kurier;Odbiór Opoczno;Platny kurier;Odbiór Świecie;Dostawa;Kurier;Odbiór Opoczno;Platny kurier;Odbiór Świecie;Dostawa;Kurier;Odbiór Opoczno;Platny kurier;Odbiór Świecie;Dostawa;Kurier;Odbiór Opoczno;Platny kurier;Odbiór Świecie;Dostawa;Kurier;Odbiór Opoczno;Platny kurier;Odbiór Świecie;Dostawa;Kurier;Odbiór Opoczno;Platny kurier;Odbiór Świecie;Dostawa;Kurier;Odbiór Opoczno;Platny kurier;Odbiór Świecie;Dostawa;Kurier;Odbiór Opoczno;Platny kurier;Odbiór Świecie;Dostawa;Kurier;Odbiór Opoczno;Platny kurier;Odbiór Świecie;Dostawa;Kurier;Odbiór Opoczno;Platny kurier;Odbiór Świecie;Dostawa;Kurier;Odbiór Opoczno;Platny kurier;Odbiór Świecie;Dostawa;Kurier;Odbiór Opoczno;Platny kurier;Odbiór Świecie;Dostawa;Kurier;Odbiór Opoczno;Platny kurier;Odbiór Świecie;Dostawa;Kurier;Odbiór Opoczno;Platny kurier;Odbiór Świecie;Dostawa;Kurier;Odbiór Opoczno;Platny kurier;Odbiór Świecie;Dostawa;Kurier;Odbiór Opoczno;Platny kurier;Odbiór Świecie;Dostawa;Kurier;Odbiór Opoczno;Platny kurier;Odbiór Świecie;Dostawa;Kurier;Odbiór Opoczno;Platny kurier;Odbiór Świecie;Dostawa;Kurier;Odbiór Opoczno;Platny kurier;Odbiór Świecie;Dostawa;Kurier";

            string linijka3 = "431;1000PŁYTEK;STRYCHALSKA - Końskie;Opoczno;Odbiór Opoczno;34;34;33;32;32;36;36;34;33;33;34;34;33;32;32;30;30;25;24;24;28;28;26;26;26;30;30;28;28;28;40;40;38;38;38;37;37;35;35;35;30;30;29;28;28;32;32;30;30;30;28;28;26;26;26;30;30;28;28;28;28;28;26;26;26;22;22;20;20;20;22;22;20;20;20;20;20;19;18;18;30;30;29;28;28;20;20;19;18;18;40;40;38;37;37;27;27;25;25;25;27;27;25;25;25;;;;;;;;;;;;;;;";

            string linijka4 = "458;A&D;A&D - Opoczno;Opoczno;Odbiór Opoczno;34;34;33;32;32;36;36;34;33;33;34;34;33;32;32;30;30;25;24;24;28;28;26;26;26;30;30;28;28;28;40;40;38;38;38;37;37;35;35;35;30;30;29;28;28;32;32;30;30;30;28;28;26;26;26;30;30;28;28;28;28;28;26;26;26;22;22;20;20;20;22;22;20;20;20;20;20;19;18;18;30;30;29;28;28;20;20;19;18;18;40;40;38;37;37;27;27;25;25;25;;;;;;;;;;;;;;;;;;;;";

            return new List<string>() { linijka1, linijka2, linijka3, linijka4 };
        }
        //TODO dodać test sprawdzający produkty ukryte i cechy
        [Fact(DisplayName = "Rabaty saturna - test rabatów i kategorii klienta")]
        public void PrzetworzTest()
        {
            string sciezka = "fakeplik.csv";

            RabatySaturn rab = A.Fake<RabatySaturn>();
            rab.SciezkaDoPliku = sciezka;

            List<string> linijkicsv = PobierzLinijkiCSV();
            Dictionary<long, Klient> klienci = StworzKlientow();

            A.CallTo(() => rab.CzyMoznaUruchomicModul()).Returns(true);

            A.CallTo(() => rab.PobierzLinijkiZPliku(sciezka)).Returns(linijkicsv);

            List<Rabat> rabaty = new List<Rabat>();
            List<ProduktUkryty> produktyukryte = new List<ProduktUkryty>();
            Dictionary<long, Konfekcje> konfekcje = new Dictionary<long, Konfekcje>();
            IDictionary<int, KategoriaKlienta> katkl = stworzKategorieKlientow();
            IDictionary<long, KlientKategoriaKlienta> klkat = new Dictionary<long, KlientKategoriaKlienta>();

            List<Cecha> cechy = StworzCechy();
            var cechyProdukty = new Dictionary<long, ProduktCecha>();
            rab.Przetworz(ref rabaty, ref produktyukryte, ref konfekcje, klienci,
                new Dictionary<long, Produkt>(), new List<PoziomCenowy>(), cechy, cechyProdukty,
                new Dictionary<long, KategoriaProduktu>(), new List<ProduktKategoria>(), ref katkl, ref klkat);

            Assert.True(rabaty.Any(a=> a.CechaId == 3 && a.Wartosc1 == 34 && a.KlientId == 431));
            Assert.True(rabaty.Any(a => a.CechaId == 2 && a.Wartosc1 == 36 && a.KlientId == 431));

            Assert.True(rabaty.Any(a => a.CechaId == 3 && a.Wartosc1 == 34 && a.KlientId == 458));
            Assert.True(rabaty.Any(a => a.CechaId == 2 && a.Wartosc1 == 36 && a.KlientId == 458));

            
            Assert.True(katkl.Any(a => a.Value.Nazwa == "Dostawa_Odbiór Opoczno:34"));
            Assert.True(katkl.Any(a => a.Value.Nazwa == "rabat_paradyż_Platny kurier:34"));

            Assert.True(katkl.Any(a => a.Value.Nazwa == "rabat_paradyż_Odbiór Świecie:33"));
            Assert.True(katkl.Any(a => a.Value.Nazwa == "rabat_paradyż_Dostawa:32"));

            Assert.True(katkl.Any(a => a.Value.Nazwa == "rabat_paradyż_Kurier:32"));
            Assert.True(katkl.Any(a => a.Value.Nazwa == "Dostawa_Odbiór Opoczno:36"));

            Assert.True(katkl.Any(a => a.Value.Nazwa == "rabat_paradyż_klinkier_Platny kurier:36"));
            Assert.True(katkl.Any(a => a.Value.Nazwa == "rabat_paradyż_klinkier_Odbiór Świecie:34"));

            Assert.True(katkl.Any(a => a.Value.Nazwa == "rabat_paradyż_klinkier_Dostawa:33"));
            Assert.True(katkl.Any(a => a.Value.Nazwa == "rabat_paradyż_klinkier_Kurier:33"));

            
        }

        private Dictionary<long, Klient> StworzKlientow()
        {
            Dictionary<long, Klient> slownik = new Dictionary<long, Klient>();

            slownik.Add(1, new Klient(1) { Symbol = "Klient1" });
            slownik.Add(2, new Klient(2) { Symbol = "Klient2" });
            slownik.Add(431, new Klient(431) { Symbol = "Klient431" });
            slownik.Add(458, new Klient(458) { Symbol = "Klient458" });
            slownik.Add(3, new Klient(3) { Symbol = "Klient3" });

            return slownik;
        }

        private List<Cecha> StworzCechy()
        {
            List<Cecha> listacech = new List<Cecha>();

            listacech.Add(new Cecha(){ Id = 1, Symbol = "AASD:ASDASAS"});
            listacech.Add(new Cecha() { Id = 2, Symbol = "rabat_paradyż_klinkier" });
            listacech.Add(new Cecha() { Id = 3, Symbol = "rabat_paradyż" });
            listacech.Add(new Cecha() { Id = 4, Symbol = "adfgadfgadfg" });
            listacech.Add(new Cecha() { Id = 5, Symbol = "peroitpowritpwoiurt" });

            return listacech;
        }

        private Dictionary<int, KategoriaKlienta> stworzKategorieKlientow()
        {
            Dictionary<int, KategoriaKlienta> kategorieklientowslownik = new Dictionary<int, KategoriaKlienta>();

            kategorieklientowslownik.Add(12, new KategoriaKlienta(){ Id = 12, Grupa = "aasdfasf", Nazwa = "retretiuert"});
            kategorieklientowslownik.Add(13, new KategoriaKlienta() { Id = 13, Grupa = "545645645", Nazwa = "retre567u545tiuert" });
            kategorieklientowslownik.Add(14, new KategoriaKlienta() { Id = 14, Grupa = "hyjtywstyh", Nazwa = "e56ybsrt7" });

            return kategorieklientowslownik;
        }
    }
}

