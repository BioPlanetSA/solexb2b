using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces.Sync;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci.Tests
{
    public class DaneLogowaniaPoleWlasneTests
    {
        [Fact(DisplayName = "Test sprawdzajacy dzialanie modulu rozbijajacego dene do logowania")]
        public void PrzetworzTest()
        {
            Klient k1 = new Klient(null) { Id = 1, Login = "JakisLogin1", HasloZrodlowe = "JakiesHaslo1" };
            Klient k2 = new Klient(null) { Id = 2, Login = "", HasloZrodlowe = "JakisLogin2/JakiesHaslo2" };
            Dictionary<long, Klient> slownikKlientow = new Dictionary<long, Klient>();
            slownikKlientow.Add(k1.Id, k1);
            slownikKlientow.Add(k2.Id, k2);

            List<KupowaneIlosci> iloscii = new List<KupowaneIlosci>();
            Dictionary<Adres, KlientAdres> adresyWErp = new Dictionary<Adres, KlientAdres>();
            List<Sklep> sklepy = new List<Sklep>();
            List<SklepKategoriaSklepu> sklpeylaczniki = new List<SklepKategoriaSklepu>();
            List<KategoriaSklepu> sklepyKategorie = new List<KategoriaSklepu>();
            List<Kraje> kraje = new List<Kraje>();
            List<Region> regiony = new List<Region>();

            DaneLogowaniaPoleWlasne dlpw = new DaneLogowaniaPoleWlasne();

            List<Magazyn> magazyny = new List<Magazyn>();
            ISyncProvider provider = A.Fake<ISyncProvider>();
            dlpw.Przetworz(ref slownikKlientow,new Dictionary<long, Produkt>(),ref adresyWErp,new List<KategoriaKlienta>(), new List<KlientKategoriaKlienta>(), ref sklepy, ref sklpeylaczniki,ref sklepyKategorie,ref kraje, ref regiony, ref magazyny,provider  );

            var wynik = slownikKlientow.Values.ToList();
            Assert.True(wynik[0].Login == "JakisLogin1" && wynik[0].HasloZrodlowe == "JakiesHaslo1");
            Assert.True(wynik[1].Login == "JakisLogin2" && wynik[1].HasloZrodlowe == "JakiesHaslo2");
        }
    }
}
