using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Core.BLL.WarunkiRegulPunktow;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Enums;
using Xunit;
namespace SolEx.Hurt.Core.BLL.WarunkiRegulPunktow.Tests
{
    public class IloscProduktowTests
    {
        [Fact(DisplayName = "Warunek ilości")]
        public void SpelniaWarunekTest()
        {
           Test(5,4,Wartosc.Wieksze, true);
           Test(5, 4, Wartosc.Mniejsze, false);
        }

        void Test(decimal ilosc, decimal iloscmodulkl, Wartosc warunek, bool oczekiwany)
        {
            IloscProduktow mod=new IloscProduktow();
            mod.Ilosc = iloscmodulkl;
            mod.WartoscWarunek = warunek;
            IDokument dok = A.Fake<IDokument>();
            IDokumentPozycja poz = A.Fake<IDokumentPozycja>();
            A.CallTo(() => poz.PozycjaDokumentuIlosc).Returns(ilosc);
            bool wynik = mod.SpelniaWarunek(poz, dok);
            Assert.True(wynik==oczekiwany,string.Format("wynik {0} oczekiwany {1}, ilosc {2} ilosc modul {3} warunekl {4}",wynik,oczekiwany,ilosc,iloscmodulkl,warunek));
        }
    }
}
