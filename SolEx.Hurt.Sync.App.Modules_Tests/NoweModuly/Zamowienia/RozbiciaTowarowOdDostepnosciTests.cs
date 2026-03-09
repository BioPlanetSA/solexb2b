using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using FakeItEasy;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Zamowienia;
using Xunit;

namespace SolEx.Hurt.Sync.App.Modules_Tests.NoweModuly.Zamowienia
{
    public class RozbiciaTowarowOdDostepnosciTests
    {

        [Fact(DisplayName = "Sprawdzanie poprawności rozbijania")]
        public void RozbijanieTest()
        {
            ZamowienieSynchronizacja zam = new ZamowienieSynchronizacja();
            ZamowienieProdukt zp1 = new ZamowienieProdukt() {ProduktId = 1, Ilosc = 1 };
            ZamowienieProdukt zp2 = new ZamowienieProdukt() {ProduktId = 2, Ilosc = 2 };
            ZamowienieProdukt zp3 = new ZamowienieProdukt() {ProduktId = 3, Ilosc = 3 };
            ZamowienieProdukt zp4 = new ZamowienieProdukt() {ProduktId = 4, Ilosc = 4 };
            ZamowienieProdukt zp5 = new ZamowienieProdukt() {ProduktId = 5, Ilosc = 5 };
            ZamowienieProdukt zp6 = new ZamowienieProdukt() {ProduktId = 6, Ilosc = 6 };
            ZamowienieProdukt zp7 = new ZamowienieProdukt() {ProduktId = 7, Ilosc = 7 };
            ZamowienieProdukt zp8 = new ZamowienieProdukt() {ProduktId = 8, Ilosc = 8 };

            List<ZamowienieProdukt> zamowioneProdukty = new List<ZamowienieProdukt>() {zp1, zp2, zp3, zp4, zp5, zp6, zp7, zp8};
            zam.pozycje = zamowioneProdukty;
            zam.StatusId = StatusImportuZamowieniaDoErp.Złożone;

            Dictionary<long, Produkt> produktyB2B = new Dictionary<long, Produkt>
            {
                {1, new Produkt(1){Typ = TypProduktu.Produkt} },
                {2, new Produkt(2){Typ = TypProduktu.Produkt} },
                {3, new Produkt(3){Typ = TypProduktu.Produkt} },
                {4, new Produkt(4){Typ = TypProduktu.Produkt} },
                {5, new Produkt(5){Typ = TypProduktu.Produkt} },
                {6, new Produkt(6){Typ = TypProduktu.Produkt} },
                {7, new Produkt(7){Typ = TypProduktu.Produkt} },
                {8, new Produkt(8){Typ = TypProduktu.Produkt} }
            };
            List<ProduktCecha> pobierzCechyProdukty = new List<ProduktCecha>();
            List<Cecha> pobierzCechy = new List<Cecha>();
            Dictionary<long, Jednostka> jednostki = new Dictionary<long, Jednostka>();
            Dictionary<long, ProduktJednostka> laczniki = new Dictionary<long, ProduktJednostka>();

            Magazyn mag1 = new Magazyn { Nazwa = "MAG1", Symbol = "mag1"};
            Dictionary<long, decimal> listaStanow = new Dictionary<long, decimal> {{1, 10}, {2,12}, {3,3}, {4,45}, {5,9}, {6,11}, {7,11} };

            List<ZamowienieSynchronizacja> wszystkie = new List<ZamowienieSynchronizacja>();
            wszystkie.Add(zam);

            ISyncProvider provider = A.Fake<ISyncProvider>();
            A.CallTo(() => provider.PobierzStanyDlaMagazynu(A<string>.Ignored)).Returns(listaStanow);

            var api = A.Fake<IAPIWywolania>();
            A.CallTo(() => api.PobierzMagazyny()).Returns(new List<Magazyn>() {mag1});

            RozbiciaTowarowOdDostepnosci rozbicia = new RozbiciaTowarowOdDostepnosci
            {
                Suffix = "Niedob",
                ApiWywolanie = api
            };
            
            rozbicia.Przetworz(zam,ref wszystkie,provider,jednostki,laczniki,produktyB2B,pobierzCechy,pobierzCechyProdukty);
           
            Assert.True(wszystkie.Count == 2,$"Zamówień powinno być 2 a jest {wszystkie.Count}");
            Assert.True(wszystkie[1].pozycje.Count == 1, $"Drugie zamówienie powinno mieć tylko jedną pozycję a ma {wszystkie[1].pozycje.Count}");

        }

        [Fact(DisplayName = "Sprawdzanie poprawności rozbijania z minimalnym stanem")]
        public void RozbijanieTest2()
        {
            ZamowienieSynchronizacja zam = new ZamowienieSynchronizacja();
            ZamowienieProdukt zp1 = new ZamowienieProdukt() { ProduktId = 1, Ilosc = 1 };
            ZamowienieProdukt zp2 = new ZamowienieProdukt() { ProduktId = 2, Ilosc = 2 };
            ZamowienieProdukt zp3 = new ZamowienieProdukt() { ProduktId = 3, Ilosc = 3 };
            ZamowienieProdukt zp4 = new ZamowienieProdukt() { ProduktId = 4, Ilosc = 4 };
            ZamowienieProdukt zp5 = new ZamowienieProdukt() { ProduktId = 5, Ilosc = 5 };
            ZamowienieProdukt zp6 = new ZamowienieProdukt() { ProduktId = 6, Ilosc = 6 };
            ZamowienieProdukt zp7 = new ZamowienieProdukt() { ProduktId = 7, Ilosc = 7 };
            ZamowienieProdukt zp8 = new ZamowienieProdukt() { ProduktId = 8, Ilosc = 8 };

            List<ZamowienieProdukt> zamowioneProdukty = new List<ZamowienieProdukt>() { zp1, zp2, zp3, zp4, zp5, zp6, zp7, zp8 };
            zam.pozycje = zamowioneProdukty;
            zam.StatusId = StatusImportuZamowieniaDoErp.Złożone;

            Dictionary<long, Produkt> produktyB2B = new Dictionary<long, Produkt>
            {
                {1, new Produkt(1){Typ = TypProduktu.Produkt} },
                {2, new Produkt(2){Typ = TypProduktu.Produkt} },
                {3, new Produkt(3){Typ = TypProduktu.Produkt} },
                {4, new Produkt(4){Typ = TypProduktu.Produkt} },
                {5, new Produkt(5){Typ = TypProduktu.Produkt} },
                {6, new Produkt(6){Typ = TypProduktu.Produkt} },
                {7, new Produkt(7){Typ = TypProduktu.Produkt} },
                {8, new Produkt(8){Typ = TypProduktu.Produkt} }
            };
            List<ProduktCecha> pobierzCechyProdukty = new List<ProduktCecha>();
            List<Cecha> pobierzCechy = new List<Cecha>();
            Dictionary<long, Jednostka> jednostki = new Dictionary<long, Jednostka>();
            Dictionary<long, ProduktJednostka> laczniki = new Dictionary<long, ProduktJednostka>();

            Magazyn mag1 = new Magazyn { Nazwa = "MAG1", Symbol = "mag1" };
            Dictionary<long, decimal> listaStanow = new Dictionary<long, decimal> { { 1, 10 }, { 2, 12 }, { 3, 3 }, { 4, 45 }, { 5, 9 }, { 6, 11 }, { 7, 9 } };

            List<ZamowienieSynchronizacja> wszystkie = new List<ZamowienieSynchronizacja>();
            wszystkie.Add(zam);

            ISyncProvider provider = A.Fake<ISyncProvider>();
            A.CallTo(() => provider.PobierzStanyDlaMagazynu(A<string>.Ignored)).Returns(listaStanow);

            var api = A.Fake<IAPIWywolania>();
            A.CallTo(() => api.PobierzMagazyny()).Returns(new List<Magazyn>() { mag1 });

            RozbiciaTowarowOdDostepnosci rozbicia = new RozbiciaTowarowOdDostepnosci
            {
                Suffix = "Niedob",
                ApiWywolanie = api,
                MinimalnyStan = 11
            };

            rozbicia.Przetworz(zam, ref wszystkie, provider, jednostki, laczniki, produktyB2B, pobierzCechy, pobierzCechyProdukty);
            Assert.True(wszystkie[1].pozycje.Count == 5, $"Powinno być 5 pozycji w nowym zamówieniu a jest {wszystkie[1].pozycje.Count}");
        }

        [Fact(DisplayName = "Sprawdzanie poprawności rozbijania pozycji z minimalnym stanem")]
        public void RozbijaniePozycjiTest()
        {
            ZamowienieSynchronizacja zam = new ZamowienieSynchronizacja();
            ZamowienieProdukt zp1 = new ZamowienieProdukt() { ProduktId = 1, Ilosc = 10 };
            ZamowienieProdukt zp2 = new ZamowienieProdukt() { ProduktId = 2, Ilosc = 2 };
            ZamowienieProdukt zp3 = new ZamowienieProdukt() { ProduktId = 3, Ilosc = 3 };

            List<ZamowienieProdukt> zamowioneProdukty = new List<ZamowienieProdukt>() { zp1, zp2, zp3 };
            zam.pozycje = zamowioneProdukty;
            zam.StatusId = StatusImportuZamowieniaDoErp.Złożone;

            Dictionary<long, Produkt> produktyB2B = new Dictionary<long, Produkt>
            {
                {1, new Produkt(1){Typ = TypProduktu.Produkt} },
                {2, new Produkt(2){Typ = TypProduktu.Produkt} }
            };
            Dictionary<long, decimal> listaStanow = new Dictionary<long, decimal> { { 1, 5 }, { 2, 12 }, { 3, 3 }, { 4, 45 }, { 5, 9 }, { 6, 11 }, { 7, 9 } };

            Dictionary<long, ZamowienieSynchronizacja> slownikZamowien = new Dictionary<long, ZamowienieSynchronizacja>();

            RozbiciaTowarowOdDostepnosci rozbicia = new RozbiciaTowarowOdDostepnosci
            {
                Suffix = "Niedob"
            };
            int i = 0;
            rozbicia.RozbijaniePozycji(zam.pozycje[i],produktyB2B,listaStanow,ref zam,ref slownikZamowien,ref i);

            Assert.True(zam.pozycje[i].Ilosc == 5, $"Ilość dla pozycji powinna wynieść 5 a wynosi {zam.pozycje[i].Ilosc}");
            Assert.True(slownikZamowien[1].pozycje[0].Ilosc == 5, $"Ilość dla pozycji w zamówieniu rozbitym powinna wynieść 5 a wynosi {slownikZamowien[1].pozycje[0].Ilosc}");
        }

    }
}
