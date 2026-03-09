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
    public class RozbijanieTowarowPoAtrybucieTests
    {

        private List<Cecha> WygenerujCechy()
        {
            Cecha c1 = new Cecha() { Id = 1, Nazwa = "Usługi", Symbol = "asortyment:usługi", AtrybutId = 1};
            Cecha c2 = new Cecha() { Id = 2, Nazwa = "Narciarstwo\\Buty", Symbol = "asortyment:narciarstwo\\buty", AtrybutId = 1};
            Cecha c3 = new Cecha() { Id = 3, Nazwa = "narciarstwo\\akcesoria", Symbol = "asortyment:narciarstwo\\akcesoria", AtrybutId = 1 };
            Cecha c4 = new Cecha() { Id = 4, Nazwa = "Narciarstwo\\Wiązania", Symbol = "asortyment:narciarstwo\\wiązania", AtrybutId = 1 };

            Cecha c5 = new Cecha() { Id = 5, Nazwa = "olimpia", Symbol = "producent:olimpia", AtrybutId = 2 };
            Cecha c6 = new Cecha() { Id = 6, Nazwa = "salomon", Symbol = "producent:salomon", AtrybutId = 2 };
            Cecha c7 = new Cecha() { Id = 7, Nazwa = "tyrolia", Symbol = "producent:tyrolia", AtrybutId = 2 };
            Cecha c8 = new Cecha() { Id = 8, Nazwa = "scouta", Symbol = "producent:scout", AtrybutId = 2 };


            return new List<Cecha>() { c1, c2, c3, c4, c5, c6, c7, c8 };

        }

        private List<ProduktCecha> WygenerujCechyProduktyNalatformie()
        {
            ProduktCecha cp1 = new ProduktCecha() { CechaId = 1, ProduktId = 1 };
            ProduktCecha cp3 = new ProduktCecha() { CechaId = 2, ProduktId = 2 };
            ProduktCecha cp5 = new ProduktCecha() {CechaId = 3, ProduktId = 4 };
            ProduktCecha cp7 = new ProduktCecha()  {CechaId = 4, ProduktId = 5 };
            ProduktCecha cp9 = new ProduktCecha() { CechaId = 1, ProduktId = 6 };
            ProduktCecha cp11 = new ProduktCecha() { CechaId = 2, ProduktId = 7 };
            ProduktCecha cp13 = new ProduktCecha() { CechaId = 5, ProduktId = 8 };

            return new List<ProduktCecha>() { cp1, cp3, cp5, cp7, cp9, cp11, cp13 };
        }
        [Fact()]
        public void PrzetworzTest()
        {
            ZamowienieSynchronizacja zam = new ZamowienieSynchronizacja
            {
                MagazynRealizujacy = "Moj",
                PochodziZRozbicia = false,
                Rozbijaj = true,
                StatusId = StatusImportuZamowieniaDoErp.Złożone
            };

            ZamowienieProdukt zp1 = new ZamowienieProdukt() { ProduktId = 1 };
            ZamowienieProdukt zp2 = new ZamowienieProdukt() { ProduktId = 2 };
            ZamowienieProdukt zp3 = new ZamowienieProdukt() { ProduktId = 3 };
            ZamowienieProdukt zp4 = new ZamowienieProdukt() { ProduktId = 4 };
            ZamowienieProdukt zp5 = new ZamowienieProdukt() { ProduktId = 5 };
            ZamowienieProdukt zp6 = new ZamowienieProdukt() { ProduktId = 6 };
            ZamowienieProdukt zp7 = new ZamowienieProdukt() { ProduktId = 7 };
            ZamowienieProdukt zp8 = new ZamowienieProdukt() { ProduktId = 8 };
            List<ZamowienieProdukt> zamowioneProdukty = new List<ZamowienieProdukt>() { zp1, zp2, zp3, zp4, zp5, zp6, zp7, zp8 };
            zam.pozycje = zamowioneProdukty;
            
            List<ZamowienieSynchronizacja> wszystkie = new List<ZamowienieSynchronizacja>();
            
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
            A.CallTo(() => api.PobierzCechyProdukty(A<HashSet<long>>.Ignored, A<int>.Ignored)).Returns(pobierzCechyProdukty);
            A.CallTo(() => api.PobierzCechy()).Returns(pobierzCechy);

            Magazyn mag1 = new Magazyn {Nazwa = "Usługi"};
            Magazyn mag2 = new Magazyn {Nazwa = "Narciarstwo\\Wiązania"};

            ISyncProvider provider = A.Fake<ISyncProvider>();
            A.CallTo(() => provider.PobierzMagazynyErp()).Returns(new List<Magazyn>(){mag1,mag2});

            RozbijanieTowarowPoAtrybucie rozbicie = new RozbijanieTowarowPoAtrybucie
            {
                Atrybuty = new List<string>() {"1", "2"},
                ApiWywolanie = api,
                Powod = SkadPowodyRozbicia.NazwaCechy,
                CzyUstawiacMagazyn = true
            };

            rozbicie.Przetworz(zam, ref wszystkie, provider, jednostki, laczniki, produktyB2B, pobierzCechy.Values.ToList(), pobierzCechyProdukty.Values.ToList());

            Assert.True(wszystkie.Count == 5, $"Wszystkich zamówień rozbitych powinno być 5 a jest {wszystkie.Count}");
            Assert.True(wszystkie[0].pozycje.Count == 2, $"zamównienie powinno mieć 2 pozycje a ma {wszystkie[0].pozycje.Count}");
            Assert.True(wszystkie[1].pozycje.Count == 2, $"zamównienie powinno mieć 2 pozycje a ma {wszystkie[1].pozycje.Count}");
            Assert.True(wszystkie[2].pozycje.Count == 1, $"zamównienie powinno mieć 1 pozycje a ma {wszystkie[2].pozycje.Count}");
            Assert.True(wszystkie[3].pozycje.Count == 1, $"zamównienie powinno mieć 1 pozycje a ma {wszystkie[3].pozycje.Count}");
            Assert.True(wszystkie[4].pozycje.Count == 1, $"zamównienie powinno mieć 1 pozycje a ma {wszystkie[4].pozycje.Count}");
        }
    }
}