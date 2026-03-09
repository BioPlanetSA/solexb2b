using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Web;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Dokumenty;
using Xunit;

namespace SolEx.Hurt.Sync.App.Modules_Tests.NoweModuly.Dokumenty
{
    public class UsuwanieZamowienBezRezerwacjiTests
    {
        [Fact(DisplayName = "Usuwanie zamówień bez rezerwacji")]
        public void PrzetworzTest()
        {
            UsuwanieZamowienBezRezerwacji modul=new UsuwanieZamowienBezRezerwacji();

            var Zamowienia = A.Fake<IAPIWywolania>();
            A.CallTo(() => Zamowienia.PobierzWszystkieZamowienia()).Returns(new List<ZamowienieSynchronizacja>());
            A.CallTo(() => Zamowienia.PobierzStatusyZamowien())
                .Returns(new List<StatusZamowienia>()
                {
                    new StatusZamowienia() {Id = 1, Symbol = "Usuniete"},
                    new StatusZamowienia() {Id = 2, Symbol = "Zrealizowane"}
                });
            modul.ApiWywolanie = Zamowienia;
            A.CallTo(() => Zamowienia.AktualizujZamowienie(A<List<ZamowienieSynchronizacja>>.Ignored)).DoesNothing();
            ConcurrentDictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>> dok = new ConcurrentDictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>>();
            dok.TryAdd(new HistoriaDokumentu(){Id=1,Rodzaj = RodzajDokumentu.Faktura},null);
            dok.TryAdd(new HistoriaDokumentu() { Id = 2, Rodzaj = RodzajDokumentu.Zamowienie }, null);
            dok.TryAdd(new HistoriaDokumentu() { Id = 3, Rodzaj = RodzajDokumentu.Zamowienie,Rezerwacja = true}, null);
            List<StatusZamowienia> statusy=new List<StatusZamowienia>();
            Dictionary<int, long> hashe = new Dictionary<int, long>();
            List<Klient> klienci = new List<Klient>();
            modul.Przetworz(ref dok,ref statusy,hashe,ref klienci);
            Assert.Equal(dok.Count,2);
            Assert.True(dok.All(x => x.Key.Id != 2));
            Assert.True(dok.Any(x=>x.Key.Id==3));
            Assert.True(dok.Any(x => x.Key.Id == 1));
        }
    }
}
