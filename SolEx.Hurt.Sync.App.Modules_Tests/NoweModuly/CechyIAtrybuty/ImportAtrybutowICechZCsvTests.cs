using System.Collections.Generic;
using System.IO;
using System.Linq;
using FakeItEasy;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using Xunit;
using Assert = Xunit.Assert;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.CechyIAtrybuty.Tests
{
    public class ImportAtrybutowICechZCsvTests
    {
        [Fact()]
        public void WyciagnijCecheTest()
        {
            List<Atrybut> listaAtr = new List<Atrybut>();
            List<Cecha> listaCech = new List<Cecha>();

            CSVHelperExt csv = A.Fake<CSVHelperExt>();

            Dictionary<long, ProduktCecha> lacznikiCech = new Dictionary<long, ProduktCecha>();

            ImportAtrybutowICechZCsv opis = A.Fake<ImportAtrybutowICechZCsv>();
            opis.PoCzymSzukacProdukty = TypyPolDoDopasowaniaZdjecia.Kod;
            opis.KolumnyDoImportu = "T";
            opis.SciezkaDoPlikuCsv = "C:\\pliki\\cecha.csv";
            opis.HelperCsv = csv;
            opis.CzyTworzeniaCechNadrzednych = false;
            string plik = @"symbol;T
symbol1;TestowaCecha1
symbol2;TestowaCecha2";
            StringReader sr = new StringReader(plik);
            A.CallTo(() => csv.OtworzPlik(A<string>.Ignored)).Returns(sr);

            IConfigSynchro config = A.Fake<IConfigSynchro>();
            A.CallTo(() => config.SeparatorAtrybutowWCechach).Returns(":".ToArray());
            A.CallTo(() => config.CechaAuto).Returns("--auto--");

            opis.Config = config;

            Produkt produkt1 = new Produkt();
            produkt1.Id = 1;
            produkt1.Kod = "symbol1";

            Produkt produkt2 = new Produkt();
            produkt2.Id = 2;
            produkt2.Kod = "symbol2";

            List<Produkt> listaproduktow = new List<Produkt>(2) { produkt1, produkt2 };

            opis.WyciagnijCeche(ref listaAtr, ref listaCech, listaproduktow.ToDictionary(x => x.Id, x => x), ref lacznikiCech);
            Assert.True(listaCech.Any(x => x.Nazwa == "TestowaCecha1"));
            Assert.True(listaCech.Any(x => x.Nazwa == "TestowaCecha2"));

            opis.SciezkaDoPlikuCsv = "C:\\pliki\\cechy.csv";
            opis.KolumnyDoImportu = "marka;model;silnik";
            string plik2 =
                @"symbol;marka;model;silnik;typ;rocznik;nacisk va;nacisk ha;obniżenieKITY;obniżenie va;obniżenie ha;dodatkowe informacje;dopisek pod cena;certyfikat;foto;uwaga1;uwaga2;uwaga3;uwaga4;uwaga5;uwaga6;uwaga7;uwaga8
MTSAL001F;ALFA ROMEO;147;1.6 / 2.0 / 1.9JTD;937;11/00-06/10;-;-;-;-;-;jednostka sprzedaży: sztuka . Prosimy pamiętać, że amortyzatory powinno wymieniać się parami;;;MTSAL001F.jpg;;;;;;;;
MTSAL001F;ALFA ROMEO;147;1.6 / 2.0 / 1.9JTD;937;11/00-06/10;-;-;-;-;-;jednostka sprzedaży: sztuka . Prosimy pamiętać, że amortyzatory powinno wymieniać się parami;;;MTSAL001F.jpg;;;;;;;;";
            StringReader sr1 = new StringReader(plik2);
            A.CallTo(() => csv.OtworzPlik(A<string>.Ignored)).Returns(sr1);
            Produkt produkt4 = new Produkt();
            produkt4.Id = 4;
            produkt4.Kod = "MTSAU001F";

            Produkt produkt5 = new Produkt();
            produkt5.Id = 5;
            produkt5.Kod = "MTSAL001F";
            Produkt produkt3 = new Produkt();
            produkt3.Id = 3;
            produkt3.Kod = "MTSAU003F";

            listaproduktow = new List<Produkt>(2) { produkt1, produkt2,produkt3,produkt4,produkt5};
            opis.PomijajProduktyNieaktywne = true;
            sr1 = new StringReader(plik2);
            A.CallTo(() => csv.OtworzPlik(A<string>.Ignored)).Returns(sr1);
            listaAtr = new List<Atrybut>();
            listaCech = new List<Cecha>();

            opis.WyciagnijCeche(ref listaAtr, ref listaCech, listaproduktow.ToDictionary(x => x.Id, x => x), ref lacznikiCech);
            Assert.True(listaCech.Any(x => x.Nazwa == "ALFA ROMEO"));
            Assert.True(!listaCech.Any(x => x.Nazwa == "148"));

            opis.PomijajProduktyNieaktywne = false;

            listaAtr = new List<Atrybut>();
            listaCech = new List<Cecha>();
            sr1 = new StringReader(plik2);
            A.CallTo(() => csv.OtworzPlik(A<string>.Ignored)).Returns(sr1);

            opis.WyciagnijCeche(ref listaAtr, ref listaCech, listaproduktow.ToDictionary(x => x.Id, x => x), ref lacznikiCech);
            Assert.False(listaCech.Any(x => x.Nazwa == "148"));
            Assert.False(listaCech.Any(x => x.CechyNadrzedne.Count>0));

            listaAtr = new List<Atrybut>();
            listaCech = new List<Cecha>();
            opis.CzyTworzeniaCechNadrzednych = true;
            A.CallTo(() => config.TrybPokazywaniaFiltrow).Returns(TrybPokazywaniaFiltrow.WymuszajSciezke);
            sr1 = new StringReader(plik2);
            A.CallTo(() => csv.OtworzPlik(A<string>.Ignored)).Returns(sr1);

            opis.WyciagnijCeche(ref listaAtr, ref listaCech, listaproduktow.ToDictionary(x => x.Id, x => x), ref lacznikiCech);
            Assert.True(listaCech.Any(x => x.CechyNadrzedne.Count>0));

            //  Assert.True(listaCech.Any(x => x.Nazwa == "AUDI"));
        }
    }
}