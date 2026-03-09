using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Zamowienia;
using Xunit;

namespace SolEx.Hurt.Sync.App.Modules_Tests.NoweModuly.Zamowienia
{
    public class RozbiciaTowarowPoCeszeTests
    {
       
        [Fact(DisplayName = "Sprawdzanie poprawności rozbijania")]
        public void RozbijanieTest()
        {
            Dictionary<long, ZamowienieSynchronizacja> slownikZamowien = new Dictionary<long, ZamowienieSynchronizacja>();
            ZamowienieSynchronizacja zam = new ZamowienieSynchronizacja();
            ZamowienieProdukt zp1 = new ZamowienieProdukt() { ProduktIdBazowy = 1 };
            ZamowienieProdukt zp2 = new ZamowienieProdukt() { ProduktIdBazowy = 2 };
            ZamowienieProdukt zp3 = new ZamowienieProdukt() { ProduktIdBazowy = 3 };
            ZamowienieProdukt zp4 = new ZamowienieProdukt() { ProduktIdBazowy = 4 };
            ZamowienieProdukt zp5 = new ZamowienieProdukt() { ProduktIdBazowy = 5 };
            ZamowienieProdukt zp6 = new ZamowienieProdukt() { ProduktIdBazowy = 6 };
            ZamowienieProdukt zp7 = new ZamowienieProdukt() { ProduktIdBazowy = 7 };
            ZamowienieProdukt zp8 = new ZamowienieProdukt() { ProduktIdBazowy = 8 };

            List<ZamowienieProdukt> zamowioneProdukty = new List<ZamowienieProdukt>() { zp1, zp2, zp3, zp4, zp5, zp6, zp7, zp8 };
            zam.pozycje = zamowioneProdukty;
            zam.StatusId= StatusImportuZamowieniaDoErp.Złożone;

            RozbiciaTowarowPoCesze rozbicia = new RozbiciaTowarowPoCesze();
           var listaCech = WygenerujCechy();
           rozbicia.Cechy = new List<string>() {"1", "2", "3"};


          Dictionary<long, ZamowienieSynchronizacja> wynik = rozbicia.RozbijZamowienie(ref zam, listaCech, WygenerujCechyProduktyNalatformie(), null);

            Assert.True(wynik.Count == 3);
            Assert.True(wynik.First().Value.PochodziZRozbicia == true);
            Assert.True(wynik.First().Value.pozycje.Count == 2);

            Assert.True(wynik.ElementAt(1).Value.pozycje.Count == 2);
            Assert.True(wynik.ElementAt(2).Value.pozycje.Count == 1);

            Assert.True(zam.PochodziZRozbicia == false);
            Assert.True(zam.pozycje.Count == 2); //tylko 2 nie maja cech rozbicia
           // Assert.True(slownikZamowien[1].pozycje.Count == 2);
        }
        
        private List<Cecha> WygenerujCechy()
        {
            Cecha c1 = new Cecha() { Id = 1, Nazwa = "Usługi", Symbol = "asortyment:usługi" };
            Cecha c2 = new Cecha() { Id = 2, Nazwa = "Narciarstwo\\Buty", Symbol = "asortyment:narciarstwo\\buty" };
            Cecha c3 = new Cecha() { Id = 3, Nazwa = "narciarstwo\\akcesoria", Symbol = "asortyment:narciarstwo\\akcesoria" };
            Cecha c4 = new Cecha() { Id = 4, Nazwa = "Narciarstwo\\Wiązania", Symbol = "asortyment:narciarstwo\\wiązania" };

            Cecha c5 = new Cecha() { Id = 5, Nazwa = "olimpia", Symbol = "producent:olimpia" };
            Cecha c6 = new Cecha() { Id = 6, Nazwa = "salomon", Symbol = "producent:salomon" };
            Cecha c7 = new Cecha() { Id = 7, Nazwa = "tyrolia", Symbol = "producent:tyrolia" };
            Cecha c8 = new Cecha() { Id = 8, Nazwa = "scouta", Symbol = "producent:scout" };

            return new List<Cecha>() { c1, c2, c3, c4, c5, c6, c7, c8 };
        }

        private List<ProduktCecha> WygenerujCechyProduktyNalatformie()
        {
            ProduktCecha cp1 = new ProduktCecha() {  CechaId = 1, ProduktId = 1 };
            ProduktCecha cp3 = new ProduktCecha() { CechaId = 2, ProduktId = 2 };
            ProduktCecha cp5 = new ProduktCecha() {  CechaId = 3, ProduktId = 4 };
            ProduktCecha cp7 = new ProduktCecha() {  CechaId = 4, ProduktId = 5 };
            ProduktCecha cp9 = new ProduktCecha() { CechaId = 1, ProduktId = 6 };
            ProduktCecha cp11 = new ProduktCecha() { CechaId = 2, ProduktId = 7 };
            ProduktCecha cp13 = new ProduktCecha() { CechaId = 5, ProduktId = 8 };

            return new List<ProduktCecha>() { cp1, cp3, cp5, cp7, cp9, cp11,cp13 };
        }



        [Fact(DisplayName = "Sprawdzanie tworzenia nazw")]
        public void PrzetworzTest1()
        {
            ZamowienieSynchronizacja zam = new ZamowienieSynchronizacja();
            ZamowienieProdukt zp1 = new ZamowienieProdukt() { ProduktId = 1 };
            ZamowienieProdukt zp2 = new ZamowienieProdukt() { ProduktId = 2 };
            ZamowienieProdukt zp4 = new ZamowienieProdukt() { ProduktId = 4 };
            ZamowienieProdukt zp6 = new ZamowienieProdukt() { ProduktId = 6 };
            ZamowienieProdukt zp7 = new ZamowienieProdukt() { ProduktId = 7 };
            List<ZamowienieProdukt> zamowioneProdukty = new List<ZamowienieProdukt>() { zp1, zp2, zp4, zp6, zp7 };
            zam.pozycje = zamowioneProdukty;
            zam.PochodziZRozbicia = false;
            zam.Rozbijaj = true;
            zam.StatusId = StatusImportuZamowieniaDoErp.Złożone;

            List<ZamowienieSynchronizacja> wszystkie = new List<ZamowienieSynchronizacja> {zam};
            ISyncProvider provider = null;
            Dictionary<long, Jednostka> jednostki = new Dictionary<long, Jednostka>();
            Dictionary<long, ProduktJednostka> laczniki = new Dictionary<long, ProduktJednostka>();
            Dictionary<long, Produkt> produktyB2B = new Dictionary<long, Produkt>();

            Dictionary<long, ProduktCecha> pobierzCechyProdukty = new Dictionary<long, ProduktCecha>();
            foreach (var cecha in WygenerujCechyProduktyNalatformie())
            {
                pobierzCechyProdukty.Add(cecha.Id, cecha);
            }

            Dictionary<long, Cecha> pobierzCechy = new Dictionary<long, Cecha>(); 
            foreach (var cechy in WygenerujCechy())
            {
                pobierzCechy.Add(cechy.Id, cechy);
            }

            var api = A.Fake<IAPIWywolania>();
            A.CallTo(() => api.PobierzCechyProdukty(A<HashSet<long>>.Ignored, A<int>.Ignored))
                .Returns(pobierzCechyProdukty);
            A.CallTo(() => api.PobierzCechy()).Returns(pobierzCechy);

            RozbiciaTowarowPoCesze rozbicie = new RozbiciaTowarowPoCesze
            {
                Cechy = new List<string>() {"1", "2", "3"},
                ApiWywolanie = api
            };
            rozbicie.Przetworz(zam, ref wszystkie, provider, jednostki, laczniki, produktyB2B, pobierzCechy.Values.ToList(), pobierzCechyProdukty.Values.ToList());

            Assert.True(wszystkie.Count == 3);
            Assert.True(wszystkie[0].pozycje.Count == 2);
            string numer = $"{wszystkie[0].NumerTymczasowyZamowienia}/1/Usługi";
            Assert.Equal(wszystkie[0].NumerZRozbicia,numer);
        }


        [Fact(DisplayName = "Sprawdzanie tworzenia nazw")]
        public void PrzetworzTest2()
        {
            int dlugoscRozbicia = 40;
            ZamowienieSynchronizacja zam = new ZamowienieSynchronizacja();
            ZamowienieProdukt zp1 = new ZamowienieProdukt() { ProduktId = 1 };
            ZamowienieProdukt zp2 = new ZamowienieProdukt() { ProduktId = 2 };
            ZamowienieProdukt zp3 = new ZamowienieProdukt() { ProduktId = 3 };
            ZamowienieProdukt zp4 = new ZamowienieProdukt() { ProduktId = 4 };
            ZamowienieProdukt zp5 = new ZamowienieProdukt() { ProduktId = 5 };
            ZamowienieProdukt zp6 = new ZamowienieProdukt() { ProduktId = 6 };
            ZamowienieProdukt zp7 = new ZamowienieProdukt() { ProduktId = 7 };
            ZamowienieProdukt zp8 = new ZamowienieProdukt() { ProduktId = 8 };
            List<ZamowienieProdukt> zamowioneProdukty = new List<ZamowienieProdukt>() { zp1, zp2, zp3, zp4, zp5, zp6, zp7, zp8 };
            zam.Id = 69;
            zam.pozycje = zamowioneProdukty;
            zam.PochodziZRozbicia = false;
            zam.Rozbijaj = true;
            zam.StatusId = StatusImportuZamowieniaDoErp.Złożone;

            List<ZamowienieSynchronizacja> wszystkie = new List<ZamowienieSynchronizacja> {zam};
            ISyncProvider provider = null;
            Dictionary<long, Jednostka> jednostki = new Dictionary<long, Jednostka>();
            Dictionary<long, ProduktJednostka> laczniki = new Dictionary<long, ProduktJednostka>();
            Dictionary<long, Produkt> produktyB2B = new Dictionary<long, Produkt>();

            Dictionary<long, ProduktCecha> PobierzCechyProdukty = new Dictionary<long, ProduktCecha>();
            foreach (ProduktCecha cecha in WygenerujCechyProduktyNalatformie())
            {
                PobierzCechyProdukty.Add(cecha.Id, cecha);
            }

            Dictionary<long, Cecha> PobierzCechy = new Dictionary<long, Cecha>();
            foreach (var cechy in WygenerujCechy())
            {
                PobierzCechy.Add(cechy.Id, cechy);
            }

            var api = A.Fake<IAPIWywolania>();
            A.CallTo(() => api.PobierzCechyProdukty(A<HashSet<long>>.Ignored,A<int>.Ignored)).Returns(PobierzCechyProdukty);
            A.CallTo(() => api.PobierzCechy()).Returns(PobierzCechy);

            RozbiciaTowarowPoCesze rozbicie = new RozbiciaTowarowPoCesze();
            rozbicie.Cechy = new List<string>() { "1", "2", "3" };
            rozbicie.ApiWywolanie = api;
            rozbicie.Powod = SkadPowodyRozbicia.SymbolCechy;
            rozbicie.DlugoscNumeru = dlugoscRozbicia;
            rozbicie.Przetworz(zam, ref wszystkie, provider, jednostki, laczniki, produktyB2B, PobierzCechy.Values.ToList(), PobierzCechyProdukty.Values.ToList());
            Assert.True(wszystkie[1].pozycje.Count == 2);

            string numer = $"{wszystkie[1].NumerTymczasowyZamowienia}/2/asortyment:narciarstwo\\buty";
            if (numer.Length > dlugoscRozbicia)
            {
                numer.Substring(0, dlugoscRozbicia);
            }
            Assert.Equal(wszystkie[2].NumerZRozbicia,numer);
        }


        [Fact(DisplayName = "Sprawdzanie tworzenia nazw")]
        public void PrzetworzTest3()
        {
            int dlugoscRozbicia = 40;
            ZamowienieSynchronizacja zam = new ZamowienieSynchronizacja();
            ZamowienieProdukt zp1 = new ZamowienieProdukt() {ProduktId = 1};
            ZamowienieProdukt zp2 = new ZamowienieProdukt() {ProduktId = 2};
            ZamowienieProdukt zp3 = new ZamowienieProdukt() {ProduktId = 3};
            ZamowienieProdukt zp4 = new ZamowienieProdukt() {ProduktId = 4};
            ZamowienieProdukt zp5 = new ZamowienieProdukt() {ProduktId = 5};
            ZamowienieProdukt zp6 = new ZamowienieProdukt() {ProduktId = 6};
            ZamowienieProdukt zp7 = new ZamowienieProdukt() {ProduktId = 7};
            ZamowienieProdukt zp8 = new ZamowienieProdukt() {ProduktId = 8};
            List<ZamowienieProdukt> zamowioneProdukty = new List<ZamowienieProdukt>(){zp1,zp2,zp3,zp4,zp5,zp6,zp7,zp8};

            zam.pozycje = zamowioneProdukty;
            zam.PochodziZRozbicia = false;
            zam.Rozbijaj = true;
            zam.StatusId = StatusImportuZamowieniaDoErp.Złożone;

            List<ZamowienieSynchronizacja> wszystkie = new List<ZamowienieSynchronizacja> {zam};
            ISyncProvider provider = null;
            Dictionary<long, Jednostka> jednostki = new Dictionary<long, Jednostka>();
            Dictionary<long, ProduktJednostka> laczniki = new Dictionary<long, ProduktJednostka>();
            Dictionary<long, Produkt> produktyB2B = new Dictionary<long, Produkt>();

            Dictionary<long, ProduktCecha> pobierzCechyProdukty = new Dictionary<long, ProduktCecha>();
            foreach (var cecha in WygenerujCechyProduktyNalatformie())
            {
                pobierzCechyProdukty.Add(cecha.Id, cecha);
            }

            Dictionary<long, Cecha> PobierzCechy = new Dictionary<long, Cecha>();
            foreach (var cechy in WygenerujCechy())
            {
                PobierzCechy.Add(cechy.Id, cechy);
            }

            var api = A.Fake<IAPIWywolania>();
            A.CallTo(() => api.PobierzCechyProdukty(A<HashSet<long>>.Ignored, A<int>.Ignored))
                .Returns(pobierzCechyProdukty);
            A.CallTo(() => api.PobierzCechy()).Returns(PobierzCechy);

            RozbiciaTowarowPoCesze rozbicie = new RozbiciaTowarowPoCesze
            {
                Cechy = new List<string>() {"1", "2", "3"},
                ApiWywolanie = api,
                DlugoscNumeru = dlugoscRozbicia,
                Powod = SkadPowodyRozbicia.NieUmieszczaj
            };
            rozbicie.Przetworz(zam, ref wszystkie, provider, jednostki, laczniki, produktyB2B, PobierzCechy.Values.ToList(), pobierzCechyProdukty.Values.ToList());
            Assert.True(wszystkie[1].pozycje.Count == 2);

            string numer = $"{wszystkie[1].NumerTymczasowyZamowienia}/2/";
            if (numer.Length > dlugoscRozbicia)
            {
                numer = numer.Substring(0, dlugoscRozbicia);
            }
            Assert.Equal(wszystkie[2].NumerZRozbicia,numer);
        }

    }
}
