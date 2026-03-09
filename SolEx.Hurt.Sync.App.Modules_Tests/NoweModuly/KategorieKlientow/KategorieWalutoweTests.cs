using System.Collections.Generic;
using FakeItEasy;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.KategorieKlientow.Tests
{
    public class KategorieWalutoweTests
    {
        [Fact(DisplayName="Test sprawdzajacy dzialanie modulu tworzacego kategorie klientow na podstawie ich waluty")]
        public void PrzetworzTest()
        {
            Dictionary<long, Waluta> slownikWalut = new Dictionary<long, Waluta>();
            Waluta w1 = new Waluta(1, "PLN", "PLN");
            Waluta w4 = new Waluta(4, "USD", "USD");
            slownikWalut.Add(w1.Id, w1);
            slownikWalut.Add(w4.Id, w4);
            Klient k1 = new Klient(null) { Id = 1, Nazwa = "JakasNazwa1", WalutaId = 1 };
            Klient k2 = new Klient(null) { Id = 2, Nazwa = "JakasNazwa2", WalutaId = 1 };
            Klient k3 = new Klient(null) { Id = 3, Nazwa = "JakasNazwa3", WalutaId = 4 };
            Klient k4 = new Klient(null) { Id = 4, Nazwa = "JakasNazwa4", WalutaId = 4 };
            Klient k5 = new Klient(null) { Id = 5, Nazwa = "JakasNazwa5", WalutaId = 1 };
            Klient k6 = new Klient(null) { Id = 6, Nazwa = "JakasNazwa6", WalutaId = 4 };
            Klient k7 = new Klient(null) { Id = 7, Nazwa = "JakasNazwa7", WalutaId = 4 };
            Klient k8 = new Klient(null) { Id = 8, Nazwa = "JakasNazwa8", WalutaId = 1 };
            Dictionary<long, Klient> slownikKlientow = new Dictionary<long, Klient>();
            slownikKlientow.Add(k1.Id, k1);
            slownikKlientow.Add(k2.Id, k2);
            slownikKlientow.Add(k3.Id, k3);
            slownikKlientow.Add(k4.Id, k4);
            slownikKlientow.Add(k5.Id, k5);
            slownikKlientow.Add(k6.Id, k6);
            slownikKlientow.Add(k7.Id, k7);
            slownikKlientow.Add(k8.Id, k8);
            
            var api = A.Fake<IAPIWywolania>();
            A.CallTo(() => api.PobierzKlientow()).Returns(slownikKlientow);
            A.CallTo(() => api.PobierzWaluty()).Returns(slownikWalut);

            KategorieWalutowe kw = new KategorieWalutowe();
            kw.DomyslnaWaluta = "PLN";
            kw.ApiWywolanie = api;


            List<KategoriaKlienta>listaKategori = new List<KategoriaKlienta>();
            List<KlientKategoriaKlienta>listaKlientowKategorie = new List<KlientKategoriaKlienta>();
            
            kw.Przetworz(ref listaKategori, ref listaKlientowKategorie);
            Assert.True(listaKategori.Count==2);
            Assert.True(listaKategori[0].Grupa == "Waluta" && listaKategori[0].Nazwa=="PLN");
            Assert.True(listaKategori[1].Grupa == "Waluta" && listaKategori[1].Nazwa == "USD");

        }
    }
}
