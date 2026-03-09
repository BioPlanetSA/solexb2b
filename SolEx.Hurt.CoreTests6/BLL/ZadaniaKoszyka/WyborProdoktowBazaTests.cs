using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using ServiceStack.Common.Extensions;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.BLL.RegulyKoszyka;
using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.DAL;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Interfaces.Modele;
using Xunit;
namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka.Tests
{
    public class WyborProdoktowBazaTests
    {
        [Fact()]
        public void WyborProduktowGratisowychCenaKatalogowa()
        {
            //Wybór gratisów po id
            var rabaty = A.Fake<IRabatyBll>();
            var klienci = A.Fake<IKlienci>();
            var klient = new Klient(klienci);
            
            var ceny = A.Fake<FlatCenyBLL>();
            ceny.cena_detaliczna_netto = 30;
            ceny.cena_netto = 28.5m;
            ceny.waluta = "PLN";
            IProduktBazowy pb = A.Fake<IProduktBazowy>();
            pb.produkt_id = 1;
            A.CallTo(() => rabaty.Pobierz(klient, pb)).Returns(ceny);

            var modul = new WyborProduktowGratisowych {Produkty = new List<string>() {"1"}};
            modul.Rabaty = rabaty;
            modul.CenaKatalogowa = 1;
            List<OpisProduktuGratisowego> wynik = modul.PobierzProdukty(klient).ToList();
            Assert.Equal(wynik[0].Cena.cena_hurtowa_netto, 1); // CenaKatalogowa - używana dla wyliczenia wartości wykoszystanego limitu (jeżeli pusta to cena wg rabatów klienta)
        }

        [Fact()]
        public void WyborProduktowGratisowychCenaNettoRzeczywista()
        {
            //Wybór gratisów po id
            var rabaty = A.Fake<IRabatyBll>();
            var klienci = A.Fake<IKlienci>();
            var klient = new Klient(klienci);

            var ceny = A.Fake<FlatCenyBLL>();
            ceny.cena_detaliczna_netto = 30;
            ceny.cena_netto = 28.5m;
            ceny.waluta = "PLN";
            IProduktBazowy pb = A.Fake<IProduktBazowy>();
            pb.produkt_id = 1;
            A.CallTo(() => rabaty.Pobierz(klient, pb)).Returns(ceny);

            var modul = new WyborProduktowGratisowych { Produkty = new List<string>() { "1" } };
            modul.Rabaty = rabaty;
            modul.Cena = 1;
            List<OpisProduktuGratisowego> wynik = modul.PobierzProdukty(klient).ToList();
            Assert.Equal(wynik[0].Cena.cena_netto, 1); // Cena - używana dla wyliczenia wartości gratisów na zamówieniu/fakturze	
        }

        [Fact()]
        public void WyborProduktowGratisowychPoCesze()
        {
            //Wybór gratisów po cesze
            var modul = new WyborProduktowGratisowychPoCesze();
            var klient = A.Fake<IKlient>();
            var cechy = A.Fake<ICechyProduktyDostep>();
            var rabaty = A.Fake<IRabatyBll>();

            var slownik = new Dictionary<int,int[]>();
            slownik.Add(1, new[]{1});

            var ceny = A.Fake<FlatCenyBLL>();
            ceny.cena_detaliczna_netto = 10;
            ceny.cena_netto = 8;
            ceny.waluta = "PLN";
            IProduktBazowy pb = A.Fake<IProduktBazowy>();
            pb.produkt_id = 1;
            A.CallTo(() => rabaty.Pobierz(klient, pb)).Returns(ceny);
            A.CallTo(() => cechy.WszystkieLacznikiWgCech()).Returns(slownik);

            modul.CechyProduktyDostep = cechy;
            modul.Cecha = new List<string>(){"1"};
            modul.Rabaty = rabaty;
            List<OpisProduktuGratisowego> wynik = modul.PobierzProdukty(klient).ToList();
            Assert.True(wynik.Count == 1);
        }
    }
}
