using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Model;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty;
using SolEx.Hurt.Sync.App.Modules_.Rozne;
using SolEx.Hurt.Helpers;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.Rozne.Tests
{
    public class ProduktySMMASHTests : ProduktySMMASH
    {
        [Fact()]
        public void PobierzBazowyKodBezRozmiaruTest()
        {
            ProduktySMMASHTests p = new ProduktySMMASHTests();
            string rozmiar = p.PobierzRozmiarMod("RASHGUARD SMMASH SKULL GREY - M");
            string rozmiar2 = p.PobierzRozmiarMod("RASHGUARD SMMASH SKULL GREY-M");
            string nazwa3 = "RASHGUARD SMMASH SKULL GREYM";
            string rozmiar3 = p.PobierzRozmiarMod(nazwa3);

            Assert.True(!string.IsNullOrEmpty(rozmiar) && rozmiar == rozmiar2, "metoda pobierająca rozmiar nie zwróciła rozmiaru");
            Assert.True(rozmiar3 == nazwa3, "Metoda pobierająca rozmiar źle pobrała rozmiar!");
            string nazwaproduktu = "RASHGUARD SMMASH SKULL GREY - M";
            string nazwa = p.PobierzBazowyKodBezRozmiaru(nazwaproduktu, rozmiar).TrimEnd(" -");

            Assert.True(nazwaproduktu != nazwa , "Metoda pobierająca nazwę bazową nie wyciągnęła jej poprawnie");
        }

        [Fact()]
        public void PobierzRozmiarTest()
        {
            string test = "RASHGUARD SPECIAL ORDER2 - L";
            SMMASH_ProduktyZPlikow obiekt=new SMMASH_ProduktyZPlikow();
          string rozmiar=  obiekt.PobierzRozmiar(test);
            Assert.Equal("L",rozmiar);
        }
    }
}
