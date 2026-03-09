using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces;
using Xunit;
namespace SolEx.Hurt.Core.Tests
{
    public class KonfekcjaBLLTests
    {
        [Fact()]
        public void ZnajdzKonfekcjeProduktuTest()
        {
            Test1();
            Test2();
        }

        private void Test1()
        {
            int[] cechy = {1, 2};
            int produkt = 20;
            int jezyk = 1;
            ProduktBazowy pb = A.Fake<ProduktBazowy>(a => a.WithArgumentsForConstructor(new object[] { jezyk }));
            pb.produkt_id = produkt;
            A.CallTo(() => pb.IdCechPRoduktu).Returns(cechy);
            int idKlienta = 100;

            string waluta = "PLN";
            var produkty = A.Fake<IProduktyBazowe>();
            A.CallTo(() => produkty.Pobierz(produkt, jezyk)).Returns(pb);
            var config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.JezykIDPolski).Returns(jezyk);
            var klient = A.Fake<IKlient>();
            klient.klient_id = idKlienta;
            klient.waluta_dokumenty = waluta;
            var klienci = A.Fake<KlienciDostep>();
            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            A.CallTo(() => calosc.Konfiguracja).Returns(config);
            A.CallTo(() => calosc.Klienci).Returns(klienci);
            A.CallTo(() => calosc.ProduktyBazowe).Returns(produkty);
            RabatyBll konfekcje = A.Fake<RabatyBll>(x => x.WithArgumentsForConstructor(new[] { calosc }));
            var cache = A.Fake<ICacheBll>();
            konfekcje.Cache = cache;
            Konfekcje konf1 = new Konfekcje() { Id = 1, Ilosc = 10, KlientId = idKlienta, ProduktId = produkt, Rabat = 7, Waluta = waluta };
            Konfekcje konf2 = new Konfekcje() { Id = 2, Ilosc = 8, KlientId = idKlienta, ProduktId = produkt, Rabat = 5, Waluta = "" };
            Konfekcje konf3 = new Konfekcje() { Id = 3, Ilosc = 10, KlientId = idKlienta, ProduktId = produkt, Rabat = 5, Waluta = "EUR" };
            Konfekcje konf4 = new Konfekcje() { Id = 4, Ilosc = 20, KlientId = idKlienta, ProduktId = produkt, Rabat = 10, Waluta = waluta };

            List<Konfekcje> wszystkie = new List<Konfekcje>();
            wszystkie.Add(konf1);
            wszystkie.Add(konf2);
            wszystkie.Add(konf3);
            wszystkie.Add(konf4);
            A.CallTo(() => klient.KlientPodstawowy()).Returns(klient);
            A.CallTo(() => konfekcje.WszystkieRegulyKonfekcji()).Returns(wszystkie.ToDictionary(x => x.Id, x => x));


            var wynik = konfekcje.ZnajdzKonfekcjeProduktuTesty(pb, klient);
            int oczekiwano = 3;
            bool res = wynik.Count == oczekiwano;
            Assert.True(res, string.Format("Znaleziono {0} powinno być {1}", wynik.Count, oczekiwano));
        }

        private void Test2()
        {
            int[] cechy = { 1, 2 };
            int produkt = 20;
            int jezyk = 1;
            ProduktBazowy pb = A.Fake<ProduktBazowy>(a => a.WithArgumentsForConstructor(new object[] { jezyk }));
            pb.produkt_id = produkt;
            A.CallTo(() => pb.IdCechPRoduktu).Returns(cechy);
            int idKlienta = 100;

            string waluta = "PLN";
            var produkty = A.Fake<IProduktyBazowe>();
            A.CallTo(() => produkty.Pobierz(produkt, jezyk)).Returns(pb);
            var config = A.Fake<IConfigBLL>();
            A.CallTo(() => config.JezykIDPolski).Returns(jezyk);
            var klient = A.Fake<IKlient>();
            klient.klient_id = idKlienta;

            klient.waluta_dokumenty = waluta;
            var klienci = A.Fake<KlienciDostep>();
            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            A.CallTo(() => calosc.Konfiguracja).Returns(config);
            A.CallTo(() => calosc.Klienci).Returns(klienci);
            A.CallTo(() => calosc.ProduktyBazowe).Returns(produkty);
            RabatyBll konfekcje = A.Fake<RabatyBll>(x => x.WithArgumentsForConstructor(new[] { calosc }));
            var cache = A.Fake<ICacheBll>();
          
            konfekcje.Cache = cache;
            Konfekcje konf1 = new Konfekcje() { Id = 1, Ilosc = 10, KlientId = idKlienta,CechaId = 1, Rabat = 7, Waluta = waluta };
            Konfekcje konf2 = new Konfekcje() { Id = 2, Ilosc = 8, KlientId = idKlienta, CechaId = 2, Rabat = 5, Waluta = "" };
            Konfekcje konf3 = new Konfekcje() { Id = 3, Ilosc = 10, KlientId = idKlienta, CechaId =3, Rabat = 5, Waluta = "EUR" };
            Konfekcje konf4 = new Konfekcje() { Id = 4, Ilosc = 20, KlientId = idKlienta, CechaId = 1, Rabat = 10, Waluta = waluta };

            List<Konfekcje> wszystkie = new List<Konfekcje>();
            wszystkie.Add(konf1);
            wszystkie.Add(konf2);
            wszystkie.Add(konf3);
            wszystkie.Add(konf4);
            A.CallTo(() => klient.KlientPodstawowy()).Returns(klient);
            A.CallTo(() => konfekcje.WszystkieRegulyKonfekcji()).Returns(wszystkie.ToDictionary(x => x.Id, x => x));


            var wynik = konfekcje.ZnajdzKonfekcjeProduktuTesty(pb, klient);
            int oczekiwano = 3;
            bool res = wynik.Count == oczekiwano;
            Assert.True(res, string.Format("Znaleziono {0} powinno być {1}", wynik.Count, oczekiwano));
        }
    }
}
