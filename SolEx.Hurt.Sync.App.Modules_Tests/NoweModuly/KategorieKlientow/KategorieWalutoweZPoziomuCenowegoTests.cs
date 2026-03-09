using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.KategorieKlientow;
using Xunit;
namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.KategorieKlientow.Tests
{
    public class KategorieWalutoweZPoziomuCenowegoTests
    {
        [Fact(DisplayName = "Test sprawdzajacy moduł tworzący kategorie klientow na podstawie poziomow cenowych")]
        public void PrzetworzTest()
        {
            Dictionary<long, Waluta> slownikWalut = new Dictionary<long, Waluta>();
            Waluta w1 = new Waluta(1,"JakasWalutaPoziomCen1","PLN");
            Waluta w2 = new Waluta(2,"JakasWalutaPoziomCen2","PLN");
            Waluta w3 = new Waluta(3,"JakasWalutaPoziomCen3","PLN");
            Waluta w4 = new Waluta(4,"USD","USD");
            slownikWalut.Add(w1.Id, w1);
            slownikWalut.Add(w2.Id, w2);
            slownikWalut.Add(w3.Id, w3);
            slownikWalut.Add(w4.Id, w4);
            Klient k1 = new Klient(null) { Id = 1, Nazwa = "JakasNazwa1", WalutaId = 1, PoziomCenowyId = 1 };
            Klient k2 = new Klient(null) { Id = 2, Nazwa = "JakasNazwa2", WalutaId = 1, PoziomCenowyId = 1 };
            Klient k3 = new Klient(null) { Id = 3, Nazwa = "JakasNazwa3", WalutaId = 4, PoziomCenowyId = 1 };
            Klient k4 = new Klient(null) { Id = 4, Nazwa = "JakasNazwa4", WalutaId = 4, PoziomCenowyId = 2 };
            Klient k5 = new Klient(null) { Id = 5, Nazwa = "JakasNazwa5", WalutaId = 1, PoziomCenowyId = 1 };
            Klient k6 = new Klient(null) { Id = 6, Nazwa = "JakasNazwa6", WalutaId = 4, PoziomCenowyId = 2 };
            Klient k7 = new Klient(null) { Id = 7, Nazwa = "JakasNazwa7", WalutaId = 4, PoziomCenowyId = 1 };
            Klient k8 = new Klient(null) { Id = 8, Nazwa = "JakasNazwa8", WalutaId = 1, PoziomCenowyId = 1 };
            Dictionary<long, Klient> slownikKlientow = new Dictionary<long, Klient>();
            slownikKlientow.Add(k1.Id, k1);
            slownikKlientow.Add(k2.Id, k2);
            slownikKlientow.Add(k3.Id, k3);
            slownikKlientow.Add(k4.Id, k4);
            slownikKlientow.Add(k5.Id, k5);
            slownikKlientow.Add(k6.Id, k6);
            slownikKlientow.Add(k7.Id, k7);
            slownikKlientow.Add(k8.Id, k8);

            PoziomCenowy pc = new PoziomCenowy() { Id = 1, Nazwa = "JakasNazwaPoziomu1", WalutaId = 1};
            PoziomCenowy pc2 = new PoziomCenowy() { Id = 2, Nazwa = "JakasNazwaPoziomu2", WalutaId = 2};
            PoziomCenowy pc3 = new PoziomCenowy() { Id = 3, Nazwa = "JakasNazwaPoziomu3", WalutaId = 3 };
            Dictionary<int, PoziomCenowy> slownikPoziomow = new Dictionary<int, PoziomCenowy>();
            slownikPoziomow.Add(pc.Id, pc);
            slownikPoziomow.Add(pc2.Id, pc2);
            slownikPoziomow.Add(pc3.Id, pc3);
            
            var api = A.Fake<IAPIWywolania>();
            A.CallTo(() => api.PobierzKlientow()).Returns(slownikKlientow);
            A.CallTo(() => api.PobierzPoziomyCen()).Returns(slownikPoziomow);
            A.CallTo(() => api.PobierzWaluty()).Returns(slownikWalut);
            KategorieWalutoweZPoziomuCenowego kw = new KategorieWalutoweZPoziomuCenowego();
            kw.NazwaPoziomuZamiastWaluty = true;
            kw.DomyslnaWaluta = "PLN";
            kw.ApiWywolanie = api;

            List<KategoriaKlienta> listaKategori = new List<KategoriaKlienta>();
            List<KlientKategoriaKlienta> listaKlientowKategorie = new List<KlientKategoriaKlienta>();

            kw.Przetworz(ref listaKategori, ref listaKlientowKategorie);
            Assert.True(listaKategori.Count == 2);
            Assert.True(listaKategori[0].Grupa == "Waluta" && listaKategori[0].Nazwa == "JakasNazwaPoziomu1");
            Assert.True(listaKategori[1].Grupa == "Waluta" && listaKategori[1].Nazwa == "JakasNazwaPoziomu2");
            kw.NazwaPoziomuZamiastWaluty = false;
            listaKategori.Clear();
            listaKlientowKategorie.Clear();
            kw.Przetworz(ref listaKategori, ref listaKlientowKategorie);
            Assert.True(listaKategori.Count == 2);
            Assert.True(listaKategori[0].Grupa == "Waluta" && listaKategori[0].Nazwa == "JakasWalutaPoziomCen1");
            Assert.True(listaKategori[1].Grupa == "Waluta" && listaKategori[1].Nazwa == "JakasWalutaPoziomCen2");
        }
    }
}
