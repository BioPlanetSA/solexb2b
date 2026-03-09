using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Rabaty.Tests
{
    public class UsuwanieRabatowTests
    {
        [Fact(DisplayName = "Moduł usuwający rabaty na podstawie pola klienta")]
        public void PrzetworzTest()
        {
            UsuwanieRabatow modul = new UsuwanieRabatow();
            modul.Pole = "pole_tekst1";
            modul.Wartosc = "PROMOCJE";
            modul.Porowanie = Wartosc.Rozne;

            Klient klient1 = new Klient(1) { Nazwa = "n1", Aktywny = true, Rabat = 5, PoleTekst1 = "5%" };
            Klient klient2 = new Klient(2) { Nazwa = "n2", Aktywny = true, Rabat = 5, PoleTekst1 = "PROMOCJE" };
            Klient klient3 = new Klient(3) { Nazwa = "n3", Aktywny = true, Rabat = 5, PoleTekst1 = "Promocje" };
            Dictionary<long, Klient> slownikKlienci = new Dictionary<long, Klient>();
            slownikKlienci.Add(klient1.Id, klient1);
            slownikKlienci.Add(klient2.Id, klient2);
            slownikKlienci.Add(klient3.Id, klient3);
            List<Cecha> listaCech = new List<Cecha>();

         
            List<Rabat> rabaty = new List<Rabat>();
            rabaty.Add(new Rabat(){KlientId = 1});
            rabaty.Add(new Rabat() { KlientId = 2 });
            rabaty.Add(new Rabat() { KlientId = 3 });
            rabaty.Add(new Rabat() { KlientId = 4 });
            rabaty.Add(new Rabat() { KlientId = 5 });
            rabaty.Add(new Rabat() { KlientId = 1 });
            rabaty.Add(new Rabat() { KlientId = 1 });
            IDictionary<int, KategoriaKlienta> katkl = new Dictionary<int, KategoriaKlienta>();
            IDictionary<long, KlientKategoriaKlienta> klkat = new Dictionary<long, KlientKategoriaKlienta>();
            List<ProduktUkryty> pu = new List<ProduktUkryty>();
            Dictionary<long, Konfekcje> konfekcje = new Dictionary<long, Konfekcje>();

            modul.Przetworz(ref rabaty, ref pu, ref konfekcje,slownikKlienci,
                new Dictionary<long, Produkt>(), new List<PoziomCenowy>(), listaCech, new Dictionary<long, ProduktCecha>(),
                new Dictionary<long, KategoriaProduktu>(), new List<ProduktKategoria>(), ref katkl, ref klkat);

            Assert.Equal(3, rabaty.Count);
            Assert.Equal(1, rabaty.Count(x=>x.KlientId==2));
        }
    }
}
