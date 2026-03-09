using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.Importy.Eksporty;
using SolEx.Hurt.Model;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Stany;
using Xunit;

namespace SolEx.Hurt.Sync.App.Modules_Tests.NoweModuly.Stany
{
    public class StanyBazaTests
    {
        [Fact(DisplayName = "Test sprawdzający poprawne aktualizowanie listy ze stanami na podstawie slownika stanów który jest pobrany z pliku")]
        public void ZaktualizujListeStanowOStanyZPlikuTest()
        {
            Dictionary<long, decimal> stanyZPliku = new Dictionary<long, decimal> {{1, 11}, {2, 12}, {3, 13}};

            Dictionary<int, List<ProduktStan>> listaWejsciowa = new Dictionary<int, List<ProduktStan>> {{1, new List<ProduktStan>()}};

            listaWejsciowa[1].Add(new ProduktStan() {MagazynId = 1, ProduktId = 1, Stan = 2});

            List<Produkt> lista = new List<Produkt> {new Produkt(1), new Produkt(2), new Produkt(3), new Produkt(4)};

            StanyBaza modul = new StanyZPlikuCsv()
            {
                IdMagazynu = 1
            };

            modul.ZaktualizujListeStanowOStanyZPliku(stanyZPliku, ref listaWejsciowa, lista);

            Assert.True(listaWejsciowa[1].Count==3, $"Powinn być 3 stany a jest:{listaWejsciowa[1].Count}");
            Assert.True(listaWejsciowa[1].First(x=>x.ProduktId==1).Stan == 13, $"Stan powinien być 13 a jest:{listaWejsciowa[1].First(x => x.ProduktId == 1).Stan}");
            Assert.True(listaWejsciowa[1].First(x => x.ProduktId == 2).Stan == 12, $"Stan powinien być 12 a jest:{listaWejsciowa[1].First(x => x.ProduktId == 2).Stan}");
            Assert.True(listaWejsciowa[1].First(x => x.ProduktId == 3).Stan == 13, $"Stan powinien być 13 a jest:{listaWejsciowa[1].First(x => x.ProduktId == 3).Stan}");


            modul.DomyslnieZero = true;
            listaWejsciowa = new Dictionary<int, List<ProduktStan>> { { 1, new List<ProduktStan>() } };
            listaWejsciowa[1].Add(new ProduktStan() { MagazynId = 1, ProduktId = 1, Stan = 2 });
            modul.ZaktualizujListeStanowOStanyZPliku(stanyZPliku, ref listaWejsciowa, lista);

            Assert.True(listaWejsciowa[1].Count == 4, $"Powinn być 4 stany a jest:{listaWejsciowa[1].Count}");
            Assert.True(listaWejsciowa[1].First(x => x.ProduktId == 1).Stan == 13, $"Stan powinien być 13 a jest:{listaWejsciowa[1].First(x => x.ProduktId == 1).Stan}");
            Assert.True(listaWejsciowa[1].First(x => x.ProduktId == 2).Stan == 12, $"Stan powinien być 12 a jest:{listaWejsciowa[1].First(x => x.ProduktId == 2).Stan}");
            Assert.True(listaWejsciowa[1].First(x => x.ProduktId == 3).Stan == 13, $"Stan powinien być 13 a jest:{listaWejsciowa[1].First(x => x.ProduktId == 3).Stan}");
            Assert.True(listaWejsciowa[1].First(x => x.ProduktId == 4).Stan == 0, $"Stan powinien być 0 a jest:{listaWejsciowa[1].First(x => x.ProduktId == 4).Stan}");


            modul.SposobLaczenia = LaczenieStanow.Podmieniaj;

            listaWejsciowa = new Dictionary<int, List<ProduktStan>> { { 1, new List<ProduktStan>() } };
            listaWejsciowa[1].Add(new ProduktStan() { MagazynId = 1, ProduktId = 1, Stan = 2 });
            modul.ZaktualizujListeStanowOStanyZPliku(stanyZPliku, ref listaWejsciowa, lista);

            Assert.True(listaWejsciowa[1].Count == 4, $"Powinn być 4 stany a jest:{listaWejsciowa[1].Count}");
            Assert.True(listaWejsciowa[1].First(x => x.ProduktId == 1).Stan == 11, $"Stan powinien być 11 a jest:{listaWejsciowa[1].First(x => x.ProduktId == 1).Stan}");
            Assert.True(listaWejsciowa[1].First(x => x.ProduktId == 2).Stan == 12, $"Stan powinien być 12 a jest:{listaWejsciowa[1].First(x => x.ProduktId == 2).Stan}");
            Assert.True(listaWejsciowa[1].First(x => x.ProduktId == 3).Stan == 13, $"Stan powinien być 13 a jest:{listaWejsciowa[1].First(x => x.ProduktId == 3).Stan}");
            Assert.True(listaWejsciowa[1].First(x => x.ProduktId == 4).Stan == 0, $"Stan powinien być 0 a jest:{listaWejsciowa[1].First(x => x.ProduktId == 4).Stan}");


            modul.SposobLaczenia = LaczenieStanow.PodmieniajGdyWiekszy;

            listaWejsciowa = new Dictionary<int, List<ProduktStan>> { { 1, new List<ProduktStan>() } };
            listaWejsciowa[1].Add(new ProduktStan() { MagazynId = 1, ProduktId = 1, Stan = 20 });
            modul.ZaktualizujListeStanowOStanyZPliku(stanyZPliku, ref listaWejsciowa, lista);

            Assert.True(listaWejsciowa[1].Count == 4, $"Powinn być 4 stany a jest:{listaWejsciowa[1].Count}");
            Assert.True(listaWejsciowa[1].First(x => x.ProduktId == 1).Stan == 20, $"Stan powinien być 11 a jest:{listaWejsciowa[1].First(x => x.ProduktId == 1).Stan}");
            Assert.True(listaWejsciowa[1].First(x => x.ProduktId == 2).Stan == 12, $"Stan powinien być 12 a jest:{listaWejsciowa[1].First(x => x.ProduktId == 2).Stan}");
            Assert.True(listaWejsciowa[1].First(x => x.ProduktId == 3).Stan == 13, $"Stan powinien być 13 a jest:{listaWejsciowa[1].First(x => x.ProduktId == 3).Stan}");
            Assert.True(listaWejsciowa[1].First(x => x.ProduktId == 4).Stan == 0, $"Stan powinien być 0 a jest:{listaWejsciowa[1].First(x => x.ProduktId == 4).Stan}");

        }
        [Fact(DisplayName = "Sprawdzanie wylicznia nowego stanu")]
        public void NowyStanTest()
        {
            StanyBaza modul = new StanyZPlikuCsv();
            modul.SposobLaczenia = LaczenieStanow.Podmieniaj;
            decimal wynik = modul.NowyStan(null, 10);
            Assert.Equal(wynik,10);

            wynik = modul.NowyStan(1, 10);
            Assert.Equal(wynik, 10);

            wynik = modul.NowyStan(100, 10);
            Assert.Equal(wynik, 10);

            modul.SposobLaczenia = LaczenieStanow.PodmieniajGdyWiekszy;
            wynik = modul.NowyStan(null, 10);
            Assert.Equal(wynik, 10);

            wynik = modul.NowyStan(1, 10);
            Assert.Equal(wynik, 10);

            wynik = modul.NowyStan(100, 10);
            Assert.Equal(wynik, 100);

            modul.SposobLaczenia = LaczenieStanow.Sumuj;
            wynik = modul.NowyStan(null, 10);
            Assert.Equal(wynik, 10);

            wynik = modul.NowyStan(1, 10);
            Assert.Equal(wynik, 11);

            wynik = modul.NowyStan(100, 10);
            Assert.Equal(wynik, 110);

        }

        [Fact(DisplayName = "Pobieranie SlownikProduktow")]
        public void SlownikProduktowTest()
        {

            List<Produkt> lista = new List<Produkt>
            {
                new Produkt(1) {Kod = "pro 1", KodKreskowy = "1234567891", Nazwa = "Produkt 1"},
                new Produkt(2) {Kod = "pro 2", KodKreskowy = "1234567892", Nazwa = "Produkt 2"},
                new Produkt(3) {Kod = "pro 3", KodKreskowy = "1234567893", Nazwa = "Produkt 3"},
                new Produkt(4) {Kod = "pro 4", KodKreskowy = "1234567894", Nazwa = "Produkt 4"}
            };
            StanyBaza modul = new StanyZPlikuCsv();
            modul.Pole = "Kod";
            var wynik = modul.SlownikProduktow(lista);
            Assert.True(wynik.ContainsKey("pro 1"));
            Assert.True(wynik.ContainsKey("pro 2"));
            Assert.True(wynik.ContainsKey("pro 3"));
            Assert.True(wynik.ContainsKey("pro 4"));

            modul.Pole = "KodKreskowy";
            wynik = modul.SlownikProduktow(lista);
            Assert.True(wynik.ContainsKey("1234567891"));
            Assert.True(wynik.ContainsKey("1234567892"));
            Assert.True(wynik.ContainsKey("1234567893"));
            Assert.True(wynik.ContainsKey("1234567894"));

            modul.Pole = "Nazwa";
            wynik = modul.SlownikProduktow(lista);
            Assert.True(wynik.ContainsKey("Produkt 1"));
            Assert.True(wynik.ContainsKey("Produkt 2"));
            Assert.True(wynik.ContainsKey("Produkt 3"));
            Assert.True(wynik.ContainsKey("Produkt 4"));
        }
    }
}
