using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL.ZadaniaDlaZamowienia;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using Xunit;

namespace SolEx.Hurt.CoreTests.BLL.ZadaniaDlaZamowienia
{
    public class KopiujPoleProduktDoPolaPozycjiZamowieniaTest
    {
        [Fact(DisplayName = "test sprawdzjący poprawność działania modulu")]
        public void WykonajTest()
        {
            string tekst = "test";
            decimal id = 23;
            KoszykPozycje poz = A.Fake<KoszykPozycje>();
            A.CallTo(() => poz.Produkt).Returns(new ProduktKlienta() {PoleTekst4 = tekst, IloscMinimalna = id });
            
            ZamowieniaProduktyBLL zam = new ZamowieniaProduktyBLL();

            KopiujPoleProduktDoPolaPozycjiZamowienia mod = new KopiujPoleProduktDoPolaPozycjiZamowienia();
            mod.PoleProduktu = "PoleTekst4";
            mod.PolePozycjiZamowienia = "Opis2";

            mod.Wykonaj(poz, zam);
            Assert.False(string.IsNullOrEmpty(zam.Opis2));
            Assert.True(zam.Opis2== tekst);

            //Test jak beda propertisy roznych typow
            mod.PoleProduktu = "IloscMinimalna";
            mod.Wykonaj(poz, zam);
            Assert.False(string.IsNullOrEmpty(zam.Opis2));
            Assert.True(zam.Opis2 == id.ToString(CultureInfo.InvariantCulture));
        }
        
    }
}
