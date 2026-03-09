using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Core;
using Xunit;
namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka.Tests
{
    public class KomunikatWagaKoszykaTests
    {
        [Fact()]
        public void WykonajTest()
        {
            TestWaga(3.34M, 1, "{0}","3,3");
            TestWaga(3.34M, 2, "{0}", "3,34");
            TestWaga(3.37M, 1, "{0}", "3,4");
            TestWaga(3.37M, 2, "{0}", "3,37");
            TestWaga(3.37M, 3, "{0}", "3,37");
        }

        private static string oczekiwanys="";
        private static decimal wagas;
        private static int miejscas;
        private void TestWaga(decimal waga, int ilemiejsc, string komunikat, string oczekiwany)
        {
            oczekiwanys = oczekiwany;
            wagas = waga;
            miejscas = ilemiejsc;
            var koszyk = A.Fake<IKoszykiBLL>();
            A.CallTo(() => koszyk.WagaCalokowita()).Returns(new WartoscLiczbowa(waga));
            KomunikatWagaKoszyka kwk=new KomunikatWagaKoszyka();
            kwk.IloscMiejscPoPrzecinku = ilemiejsc;
            kwk.Komunikat = komunikat;
            kwk.DodajWiadomosc += kwk_DodajWiadomosc;
            kwk.Wykonaj(koszyk);
        }

        void kwk_DodajWiadomosc(object sender, Model.Web.DodanieKomunikatuEventArgs e)
        {
            bool wynik = e.Komunikat.Wiadomosc == oczekiwanys;
            Assert.True(wynik,string.Format("Wynik {0} Oczekiwany {1} waga {2} ile miejsc {3}",e.Komunikat.Wiadomosc,oczekiwanys,wagas,miejscas));

        }
    }
}
