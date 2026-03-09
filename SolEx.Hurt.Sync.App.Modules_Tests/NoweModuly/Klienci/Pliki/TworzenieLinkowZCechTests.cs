using System.Collections.Generic;
using FakeItEasy;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Pliki;
using Xunit;

namespace SolEx.Hurt.Sync.App.Modules_Tests.NoweModuly.Pliki
{
    public class TworzenieLinkowZCechTests
    {
        [Fact(DisplayName = "Test sprawdzajacy poprawność działania modułu wyciągającego link do materiału dodatkowego dla produktu z cechy")]
        public void PrzetworzTest()
        {
            Cecha c1 = new Cecha(){Id = 1, Nazwa = "link1", AtrybutId = 1 };
            Cecha c2 = new Cecha() { Id = 2, Nazwa = "http:\\link2", AtrybutId = 1 };
            Cecha c3 = new Cecha() { Id = 3, Nazwa = "HTTPS:\\link3", AtrybutId = 3 };
            Cecha c4 = new Cecha() { Id = 4, Nazwa = "ftp:\\link4", AtrybutId = 1 };
            Cecha c5 = new Cecha() { Id = 5, Nazwa = "http:\\link5", AtrybutId = 2 };
            Cecha c6 = new Cecha() { Id = 6, Nazwa = "link6", AtrybutId = 2 };
            List<Cecha> listaCech = new List<Cecha>(){c1,c2,c3,c4,c5,c6};


            var api = A.Fake<IAPIWywolania>();

            ProduktCecha pp1 = new ProduktCecha(){CechaId = 1, ProduktId = 1};
            ProduktCecha pp2 = new ProduktCecha() {CechaId = 2, ProduktId = 2 };
            ProduktCecha pp3 = new ProduktCecha() { CechaId = 3, ProduktId = 3 };
            ProduktCecha pp4 = new ProduktCecha() { CechaId = 4, ProduktId = 4 };
            ProduktCecha pp5 = new ProduktCecha() {CechaId = 5, ProduktId = 5 };
            ProduktCecha pp6 = new ProduktCecha() { CechaId = 6, ProduktId = 6 };

            ProduktCecha pp7 = new ProduktCecha() { CechaId = 2, ProduktId = 5 };
            ProduktCecha pp8 = new ProduktCecha() { CechaId = 1, ProduktId = 6 };
            Dictionary<long, ProduktCecha> slownik = new Dictionary<long, ProduktCecha>();
            slownik.Add(pp1.Id,pp1);
            slownik.Add(pp2.Id, pp2);
            slownik.Add(pp3.Id, pp3);
            slownik.Add(pp4.Id, pp4);
            slownik.Add(pp5.Id, pp5);
            slownik.Add(pp6.Id, pp6);
            slownik.Add(pp7.Id, pp7);
            slownik.Add(pp8.Id, pp8);

            A.CallTo(() => api.PobierzCechyProdukty(A<HashSet<long>>.Ignored, A<int>.Ignored)).Returns(slownik);


            IDictionary<long, Produkt> produktyNaB2B = new Dictionary<long, Produkt>();
            List<ProduktPlik> plikiLokalnePowiazania = new List<ProduktPlik>();
            List<Plik> plikiLokalne = new List<Plik>();
            List<KategoriaProduktu> kategorieB2B = new List<KategoriaProduktu>();
            List<Klient> klienciB2B = new List<Klient>();


            TworzenieLinkowZCech modul = new TworzenieLinkowZCech();
            modul.ApiWywolanie = api;
            modul.ListaAtrybutow = new List<string>(){"1","3"};
            modul.SposobOtwieraniaLinku=SposobOtwierania.Link;
            modul.Obrazek = "obrazek";
            modul.Przetworz(produktyNaB2B, ref plikiLokalnePowiazania, ref plikiLokalne, null, ref listaCech,
                ref kategorieB2B, ref klienciB2B);

            Assert.True(plikiLokalne.Count==3);
            Assert.True(plikiLokalnePowiazania.Count==4);

        }
    }
}
