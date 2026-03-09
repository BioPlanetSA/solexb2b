using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using FakeItEasy.Creation;
using ServiceStack.Common;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using Xunit;
namespace SolEx.Hurt.Core.Tests
{
    public class RabatyBllTests
    {
        public Dictionary<RabatTyp, HashSet<RabatBLL>> PrzygotujTabliceRabtow(int ile, int idKlienta)
        {
            var rabaty = new Dictionary<RabatTyp, HashSet<RabatBLL>>();
            for (int i = 0; i < ile; i++)
            {
                rabaty r = new rabaty();

                if (i%3 == 0)
                {
                    r.klient_id = new Random().Next(1, 2000);
                }


                if (i%10 == 0)
                    r.klient_id = idKlienta;

                if (i%7 == 0)
                    r.kategoria_klientow_id = new Random().Next(1, 22000);


                r.wartosc1 = new Random().Next(1, 2000);
                r.wartosc2 = r.wartosc1;
                r.wartosc3 = r.wartosc1;
                r.wartosc4 = r.wartosc1;
                r.wartosc5 = r.wartosc1;

                r.aktywny = true;
                if (i%13 == 0)
                {
                    r.aktywny = false;
                }

                if (i%3 == 0)
                    r.waluta = "USD";

                else if (i%2 == 0)
                    r.waluta = "EUR";

                else if (i%1 == 0)
                    r.waluta = "PLN";

                if (i%11 == 0)
                {
                    r.odKiedy = DateTime.Now.AddDays(new Random().Next(-100, -1));
                    r.doKiedy = DateTime.Now.AddDays(new Random().Next(1, 100));
                }

                if (i%5 == 0)
                {
                    r.odKiedy = DateTime.Now.AddDays(new Random().Next(-50, -1));
                    r.doKiedy = DateTime.Now.AddDays(new Random().Next(1, 50));
                }

                r.produkt_id = new Random().Next(1, 10000);


                if (!rabaty.ContainsKey(r.TypRabatu))
                    rabaty.Add(r.TypRabatu, new HashSet<RabatBLL>());

                rabaty[r.TypRabatu].Add(new RabatBLL(r));
            }

            return rabaty;
        }

        private IEnumerable<int> WygenerujListeIDKategorii(int ile = 100)
        {
            for (int i = 0; i < ile; i++)
                yield return new Random().Next(1, 10000);
        }

        private Dictionary<int, HashSet<int>> WygenerujSlownikProduktyKategorie(int ileProduktow = 100,
            int ileKategoriiNaProdukt = 200)
        {
            Dictionary<int, HashSet<int>> produktykategorie = new Dictionary<int, HashSet<int>>(ileProduktow);
            for (int i = 0; i < ileProduktow; i++)
            {
                produktykategorie.Add(i, new HashSet<int>());

                for (int j = 0; j < ileKategoriiNaProdukt; j++)
                {
                    produktykategorie[i].Add(j);
                }
            }
            return produktykategorie;
        }

        private IEnumerable<kategorie_klientow> WygenerujKategorieKlientow(int ile = 100)
        {
            for (int i = 0; i < ile; i++)
                yield return
                    new kategorie_klientow() {Id = i, grupa = "F", nazwa = "fajna nazwa", pokazuj_klientowi = i%2 != 0};
        }

        [Fact()]
        public void ZnajdzTestWydajnosci()
        {
            //int idklienta = 100;
            //int idProduktu = 1;
            //var configbll = A.Fake<ConfigBLL>();
            //var produktybazowe = A.Fake<IProduktyBazowe>();
            //var produktbazowy = A.Fake<ProduktBazowy>();
            //var kategoriebll = A.Fake<IKategorieDostep>();
            //var kliencidostep = A.Fake<IKlienciDostep>();
            //ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            //A.CallTo(() => calosc.Konfiguracja).Returns(configbll);
            //A.CallTo(() => calosc.Klienci).Returns(kliencidostep);
            //A.CallTo(() => calosc.ProduktyBazowe).Returns(produktybazowe);
            //A.CallTo(() => calosc.KategorieDostep).Returns(kategoriebll);
            //RabatyBll rabatybll = A.Fake<RabatyBll>(x => x.WithArgumentsForConstructor(new[] { calosc }));
       
         
            //var rabaty = PrzygotujTabliceRabtow(10000, idklienta).Values.SelectMany(x=>x).ToList();
            //klienci k = new klienci(idklienta);
            //k.rabat = 10;

            //var klient = A.Fake<Klient>(a => a.WithArgumentsForConstructor(new object[] {k}));
            //var produktykategorie = WygenerujSlownikProduktyKategorie();
            //var kategorieklientow = WygenerujKategorieKlientow().ToList();
            //var kategorieklientowint = kategorieklientow.Select(a => a.Id);

            //A.CallTo(() => configbll.JezykIDPolski).Returns(1);

            //A.CallTo(() => produktbazowy.IdCechPRoduktu).Returns(new HashSet<int>());
            //A.CallTo(() => produktybazowe.Pobierz(idProduktu, configbll.JezykIDPolski)).Returns(produktbazowy);
            //A.CallTo(() => kategoriebll.ProduktyKategorieGrupowanePoProdukcie)
            //    .Returns(produktykategorie);

            //A.CallTo(() => klient.Kategorie).Returns(kategorieklientow.ToList());
            //A.CallTo(() => kliencidostep.Pobierz(idklienta)).Returns(klient);
            //A.CallTo(() => rabatybll.ZnajdzWszystkieKlienta(kategorieklientowint, idklienta)).Returns(rabaty.GroupBy(x=>x.TypRabatu).ToDictionary(x=>x.Key,x=>x.Select(y=>y.rabat_id).ToArray()));
            //A.CallTo(() => rabatybll.WszystkieSlownik()).Returns(rabaty.ToDictionary(x => x.rabat_id, x => x));
            //int ileIteracji = 500;

            //var slownik = new List<RabatTyp>() {RabatTyp.Zaawansowany, RabatTyp.Prosty};


            //var timer = Stopwatch.StartNew();

            //for (int i = 0; i < ileIteracji; i++)
            //{
            //    rabatybll.Znajdz(i, klient, kategorieklientowint, slownik);
            //}
            //timer.Stop();
            //double czas = timer.Elapsed.TotalSeconds/ileIteracji; //45 s
            //Assert.True(czas < 1);
        }

        [Fact()]
        public void ZnajdzWszystkieKlientaTestWydajnosc()
        {
            //int idKlienta = 100;
            //var rabaty = A.Fake<List<RabatyBll>>();
            //var rabatybll = A.Fake<RabatyBll>();
            //var cache = A.Fake<ICacheBll>();
            //rabatybll.Cache = cache;
            //var kolekcjarabatow = PrzygotujTabliceRabtow(100000, idKlienta);
            //Dictionary<long, RabatBLL> slownik = KolekcjaRabatowNaSlownik(kolekcjarabatow);
            //A.CallTo(() => rabatybll.WszystkieSlownik()).Returns(slownik);
            //A.CallTo(
            //    () =>
            //        rabatybll.Cache.PobierzObiekt<List<RabatBLL>>(
            //            rabatybll.NazwaCacheDlaWszystkichRabatowKlienta(idKlienta)))
            //    .Returns(null);

            //List<int> idkategorii = WygenerujListeIDKategorii().ToList();

            //var timer = Stopwatch.StartNew();
            //int ileIteracji = 10;
            ////  var ileproduktow = rabatybll.ZnajdzWszystkieKlienta(idkategorii, idKlienta).Where(a=> a.produkt_id.HasValue).Count();
            //for (int i = 0; i < ileIteracji; i++)
            //{
            //    var rabatyklienta = rabatybll.ZnajdzWszystkieKlienta(idkategorii, i);
            //}
            //timer.Stop();
            //double czas = timer.Elapsed.TotalSeconds/ileIteracji;
            //Assert.True(czas < 1,
            //    string.Format("Dla kolekcji {0} rabatów  czas wynosi {1}s", kolekcjarabatow.Count, czas));
        }

     

        [Fact(DisplayName = "Wyszukiwanie gradacji produktu")]
        public void WyliczoneKonfekcjeTest()
        {
            WyliczoneKonfekcjeTest1();
            WyliczoneKonfekcjeTest2();
        }

        private void WyliczoneKonfekcjeTest1()
        {
            int produktid = 5;
            int klientid = 1;

            int jezyk = 1;
            decimal ilosc = 5;
            string waluta = "PLN";
            IKlient klient = A.Fake<IKlient>();
            klient.klient_id = klientid;
            var configbll = A.Fake<ConfigBLL>();
            var produktybazowe = A.Fake<IProduktyBazowe>();
            var kliencidostep = A.Fake<IKlienciDostep>();
            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            A.CallTo(() => calosc.Konfiguracja).Returns(configbll);
            A.CallTo(() => calosc.Klienci).Returns(kliencidostep);
            A.CallTo(() => calosc.ProduktyBazowe).Returns(produktybazowe);
            RabatyBll rabatybll = A.Fake<RabatyBll>(x => x.WithArgumentsForConstructor(new[] { calosc }));
            A.CallTo(() => rabatybll.ZdefiniowaneKonfekcje()).Returns(true);
            List<Konfekcje> konfekcje = new List<Konfekcje>();
            konfekcje.Add(new Konfekcje() { ProduktId = produktid, Ilosc = 5, RabatKwota = 10 });
            produkty p = new produkty();
            p.produkt_id = produktid;
            var jednostki = new List<JednostkaProduktu>();
            jednostki.Add(new JednostkaProduktu() { Przelicznik = 1, Podstawowa = true });
            ProduktBazowy pb = new ProduktBazowy(jezyk, p, jednostki, new List<KategorieBLL>());


            A.CallTo(() => kliencidostep.Pobierz(klientid)).Returns(klient);
            A.CallTo(() => produktybazowe.Pobierz(produktid, jezyk)).Returns(pb);
            A.CallTo(() => configbll.JezykIDPolski).Returns(jezyk);
            A.CallTo(() => rabatybll.ZnajdzKonfekcjeProduktu(pb, klient)).Returns(konfekcje);
            A.CallTo(() => rabatybll.PobierzIlosc(klient, produktid)).Returns(ilosc);
            flat_ceny fc = new flat_ceny();
            fc.cena_netto = 11;
            fc.waluta = waluta;
            A.CallTo(() => rabatybll.Wylicz(klient, pb)).Returns(fc);



            var wynik = rabatybll.WyliczoneKonfekcje(pb, klientid);
            Assert.True(wynik.Count == 2, "Powinny być 2");
        }
        private void WyliczoneKonfekcjeTest2()
        {
            int produktid = 5;
            int klientid = 1;
            int jezyk = 1;
            decimal ilosc = 5;
            string waluta = "PLN";
            IKlient klient = A.Fake<IKlient>();
            klient.klient_id = klientid;
            var configbll = A.Fake<ConfigBLL>();
            var produktybazowe = A.Fake<IProduktyBazowe>();
            var kliencidostep = A.Fake<IKlienciDostep>();
            ISolexBllCalosc calosc = A.Fake<ISolexBllCalosc>();
            A.CallTo(() => calosc.Konfiguracja).Returns(configbll);
            A.CallTo(() => calosc.Klienci).Returns(kliencidostep);
            A.CallTo(() => calosc.ProduktyBazowe).Returns(produktybazowe);
            RabatyBll rabatybll = A.Fake<RabatyBll>(x => x.WithArgumentsForConstructor(new[] { calosc }));
            A.CallTo(() => rabatybll.ZdefiniowaneKonfekcje()).Returns(true);
            List<Konfekcje> konfekcje = new List<Konfekcje>();
            konfekcje.Add(new Konfekcje() { ProduktId = produktid, Ilosc = 5, RabatKwota = 10 });
            produkty p = new produkty();
            p.produkt_id = produktid;
            var jednostki = new List<JednostkaProduktu>();
            jednostki.Add(new JednostkaProduktu() { Przelicznik = 1, Podstawowa = true });
            ProduktBazowy pb = new ProduktBazowy(jezyk, p, jednostki, new List<KategorieBLL>());


            A.CallTo(() => kliencidostep.Pobierz(klientid)).Returns(klient);
            A.CallTo(() => produktybazowe.Pobierz(produktid, jezyk)).Returns(pb);
            A.CallTo(() => configbll.JezykIDPolski).Returns(jezyk);
            A.CallTo(() => rabatybll.ZnajdzKonfekcjeProduktu(pb, klient)).Returns(konfekcje);
            A.CallTo(() => rabatybll.PobierzIlosc(klient, produktid)).Returns(ilosc);
            flat_ceny fc = new flat_ceny();
            fc.cena_netto = 6;
            fc.waluta = waluta;
            A.CallTo(() => rabatybll.Wylicz(klient, pb)).Returns(fc);

            var wynik = rabatybll.WyliczoneKonfekcje(pb, klientid);
            Assert.True(wynik.Count == 1, "Powinny być 1");
        }
    }
}
