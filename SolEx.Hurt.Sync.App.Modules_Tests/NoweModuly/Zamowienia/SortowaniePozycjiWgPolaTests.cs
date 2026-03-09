using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Zamowienia;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Zamowienia.Tests
{
    public class SortowaniePozycjiWgPolaTests
    {
        [Fact()]
        public void PrzetworzTest()
        {
            SortowaniePozycjiWgPola modul = new SortowaniePozycjiWgPola();

            modul.Pole = "Kod";

            ZamowienieSynchronizacja zamowienie = new ZamowienieSynchronizacja();

            Produkt p1 = new Produkt
            {
                Id = 1,
                Kod = "jeden"
            };

            Produkt p2 = new Produkt
            {
                Id = 2,
                Kod = "dwa"
            };

            Produkt p3 = new Produkt
            {
                Id = 3,
                Kod = "trzy"
            };

            Dictionary<long, Produkt> produktyNaB2B = new Dictionary<long, Produkt>(3) {{p1.Id, p1}, {p2.Id, p2}, {p3.Id, p3}};

            zamowienie.pozycje.Add(new ZamowienieProdukt() { Id = 1, ProduktId = p1.Id, ProduktIdBazowy = p1.Id, Ilosc = p1.Id });
            zamowienie.pozycje.Add(new ZamowienieProdukt() { Id = 2, ProduktId = p2.Id, ProduktIdBazowy = p2.Id, Ilosc = p2.Id });
            zamowienie.pozycje.Add(new ZamowienieProdukt() { Id = 3, ProduktId = p3.Id, ProduktIdBazowy = p3.Id, Ilosc = 2*p3.Id });
            zamowienie.pozycje.Add(new ZamowienieProdukt() { Id = 4, ProduktId = p3.Id, ProduktIdBazowy = p3.Id, Ilosc = p3.Id });


            List<ZamowienieSynchronizacja> listazamowien = new List<ZamowienieSynchronizacja>(0);
            modul.Przetworz(zamowienie, ref listazamowien, null, new Dictionary<long, Jednostka>(), new Dictionary<long, ProduktJednostka>(), produktyNaB2B, new List<Cecha>(), new List<ProduktCecha>());

            Assert.Equal(p2.Id, zamowienie.pozycje[0].ProduktId);
            Assert.Equal(p1.Id, zamowienie.pozycje[1].ProduktId);
            Assert.Equal(p3.Id, zamowienie.pozycje[2].ProduktId);
            Assert.Equal(p3.Id, zamowienie.pozycje[3].ProduktId);

        }
    }
}
