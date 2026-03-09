using System.Collections.Concurrent;
using System.Collections.Generic;
using FakeItEasy;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Model.Web;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Dokumenty;
using Xunit;

namespace SolEx.Hurt.Sync.App.Modules_Tests.NoweModuly.Dokumenty
{
    public class PodmianaPozycjiTests
    {
        [Fact(DisplayName = "Test sprawdzajacy poprawność działania metody podmieniajacej pozycje na dokumenci na podstawie opisu pozycji")]
        public void PrzetworzTest()
        {
            Dictionary<long,Produkt> wszystkieprodukty = new Dictionary<long, Produkt>();
            Produkt p1 = new Produkt() { Id = 1, Kod = "jakisKod1", Nazwa = "jakasNazwa1", PoleLiczba1 = 2, PoleTekst2 = "visual" };
            Produkt p2 = new Produkt() { Id = 2, Kod = "jakisKod2", Nazwa = "jakasNazwa2" };
            Produkt p3 = new Produkt() { Id = 3, Kod = "jakisKod3", Nazwa = "jakasNazwa3", PoleLiczba1 = 3, PoleTekst2 = "visual" };
            Produkt p4 = new Produkt() { Id = 4, Kod = "jakisKod4", Nazwa = "jakasNazwa4" };
            Produkt p5 = new Produkt() { Id = 5, Kod = "jakisKod5", Nazwa = "jakasNazwa5" };
            Produkt p6 = new Produkt() { Id = 6, Kod = "jakisKod6", Nazwa = "jakasNazwa6", PoleLiczba1 = 2, PoleTekst2 = "visual" };

            List<long> listaProduktowKlient1 = new List<long>(){1,2,3,4};
            List<long> listaProduktowKlient2 = new List<long>() { 3, 4, 5, 6 };

            wszystkieprodukty.Add(p1.Id, p1);
            wszystkieprodukty.Add(p2.Id, p2);
            wszystkieprodukty.Add(p3.Id, p3);
            wszystkieprodukty.Add(p4.Id, p4);
            wszystkieprodukty.Add(p5.Id, p5);
            wszystkieprodukty.Add(p6.Id, p6);

            var api = A.Fake<IAPIWywolania>();
            A.CallTo(() => api.PobierzProdukty()).Returns(wszystkieprodukty);
            A.CallTo(() => api.PobierzDostepneProduktyKlienta(1)).Returns(listaProduktowKlient1);
            A.CallTo(() => api.PobierzDostepneProduktyKlienta(2)).Returns(listaProduktowKlient2);

            HistoriaDokumentuProdukt hdp1 = new HistoriaDokumentuProdukt(){ProduktId = 1, KodProduktu = "JakisKodProdukty1", NazwaProduktu = "JakasNazwaProduktu1", Opis = "JakisOpis1"};
            HistoriaDokumentuProdukt hdp2 = new HistoriaDokumentuProdukt() { ProduktId = 2, KodProduktu = "JakisKodProdukty2", NazwaProduktu = "JakasNazwaProduktu2", Opis = "JakisOpis2Visual" };
            HistoriaDokumentuProdukt hdp3 = new HistoriaDokumentuProdukt() { ProduktId = 3, KodProduktu = "JakisKodProdukty3", NazwaProduktu = "JakasNazwaProduktu3", Opis = "JakisOpis3Visual" };

            HistoriaDokumentu hd1 = new HistoriaDokumentu(){KlientId = 1};
            List<HistoriaDokumentuProdukt> hd1p = new List<HistoriaDokumentuProdukt>() {hdp1, hdp2, hdp3};
            HistoriaDokumentu hd2 = new HistoriaDokumentu() { KlientId = 2 };
            List<HistoriaDokumentuProdukt> hd2p = new List<HistoriaDokumentuProdukt>() { hdp2, hdp3 };

            ConcurrentDictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>> listaWejsciowa = new ConcurrentDictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>>();
            listaWejsciowa.TryAdd(hd1,hd1p);
            listaWejsciowa.TryAdd(hd2, hd2p);

            PodmianaPozycji pp = new PodmianaPozycji();
            pp.ApiWywolanie = api;
            List<StatusZamowienia> st = new List<StatusZamowienia>();
            List<Klient> k = new List<Klient>();

            pp.Przetworz(ref listaWejsciowa, ref st,new Dictionary<int, long>(),ref k);
            Assert.True(hdp2.ProduktId == 1);
            Assert.True(hdp2.KodProduktu==p1.Kod);
            Assert.True(hdp2.NazwaProduktu == p1.Nazwa);

            Assert.True(hdp3.ProduktId == p3.Id);
            Assert.True(hdp3.KodProduktu == p3.Kod);
            Assert.True(hdp3.NazwaProduktu == p3.Nazwa);
        }
    }
}
