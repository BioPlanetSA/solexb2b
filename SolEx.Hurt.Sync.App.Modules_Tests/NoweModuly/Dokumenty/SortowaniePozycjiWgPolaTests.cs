using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using FakeItEasy;
using log4net;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Web;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Dokumenty;
using Xunit;

namespace SolEx.Hurt.Sync.App.Modules_Tests.NoweModuly.Dokumenty
{
    public class SortowaniePozycjiWgPolaTests
    {
        [Fact(DisplayName = "Test sprawdzjacy poprawnosc sortowania pozycji w dokumencie wzgledem pola")]
        public void PrzetworzTest()
        {
            var sortowanie = A.Fake<ILog>();
            A.CallTo(() => sortowanie.Error("Ustawienie Pole jest puste, moduł przerwie działanie!")).Throws(() => new NullReferenceException());
            ConcurrentDictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>>  listaWejsciowa = new ConcurrentDictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>>();
            List<StatusZamowienia> statusy = new List<StatusZamowienia>();
            List<Klient> listaKlientow = new List<Klient>();

            HistoriaDokumentuProdukt hdp1 = new HistoriaDokumentuProdukt(){ProduktId = 2, Opis = "hdpId1"};
            HistoriaDokumentuProdukt hdp2 = new HistoriaDokumentuProdukt(){ProduktId = 1, Opis = "hdpId2"};
            HistoriaDokumentuProdukt hdp3 = new HistoriaDokumentuProdukt(){ProduktId = 5, Opis = "hdpId3"};
            HistoriaDokumentuProdukt hdp4 = new HistoriaDokumentuProdukt(){ProduktId = 3, Opis = "hdpId4"};
            HistoriaDokumentuProdukt hdp5 = new HistoriaDokumentuProdukt(){ProduktId = 4, Opis = "hdpId5"};
            HistoriaDokumentuProdukt hdp6 = new HistoriaDokumentuProdukt(){ProduktId = 6, Opis = "hdpId6"};

            HistoriaDokumentu hd = new HistoriaDokumentu(){Id = 1};
            List<HistoriaDokumentuProdukt>hdp = new List<HistoriaDokumentuProdukt>() { hdp1, hdp2, hdp3, hdp4, hdp5, hdp6 };
            listaWejsciowa.TryAdd(hd,hdp);
            SortowaniePozycjiWgPola spWP2 = new SortowaniePozycjiWgPola {Pole = "ProduktId"};

            spWP2.Przetworz(ref listaWejsciowa, ref statusy, new Dictionary<int, long>(),ref listaKlientow);
            Assert.True(listaWejsciowa[hd][0].Opis == "hdpId2");
            Assert.True(listaWejsciowa[hd][1].Opis == "hdpId1");
            Assert.True(listaWejsciowa[hd][2].Opis == "hdpId4");
            Assert.True(listaWejsciowa[hd][3].Opis == "hdpId5");
            Assert.True(listaWejsciowa[hd][4].Opis == "hdpId3");
            Assert.True(listaWejsciowa[hd][5].Opis == "hdpId6");

            spWP2.Pole = "Opis";
            spWP2.Przetworz(ref listaWejsciowa, ref statusy, new Dictionary<int, long>(), ref listaKlientow);
            Assert.True(listaWejsciowa[hd][0].Opis == "hdpId1");
            Assert.True(listaWejsciowa[hd][1].Opis == "hdpId2");
            Assert.True(listaWejsciowa[hd][2].Opis == "hdpId3");
            Assert.True(listaWejsciowa[hd][3].Opis == "hdpId4");
            Assert.True(listaWejsciowa[hd][4].Opis == "hdpId5");
            Assert.True(listaWejsciowa[hd][5].Opis == "hdpId6");

        }
        [Fact(DisplayName = "Test sprawdzający czy zostanie wyłapany wyjatek")]
        public void PrzetworzTestWyjatek()
        {
            //var sortowanie = A.Fake<ILog>();
            //A.CallTo(() => sortowanie.Error("Ustawienie Pole jest puste, moduł przerwie działanie!")).Throws(() => new NullReferenceException());
            ConcurrentDictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>> listaWejsciowa = new ConcurrentDictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>>();
            List<StatusZamowienia> statusy = new List<StatusZamowienia>();
            List<Klient> listaKlientow = new List<Klient>();

            SortowaniePozycjiWgPola spWp = new SortowaniePozycjiWgPola();
            
            try
            {
                spWp.Przetworz(ref listaWejsciowa, ref statusy, new Dictionary<int, long>(),
                    ref listaKlientow);
            }
            catch (NullReferenceException) { }
        }
    }
}
