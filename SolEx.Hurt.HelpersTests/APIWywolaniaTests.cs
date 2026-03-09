using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using FakeItEasy;
using RestSharp;
using ServiceStack.Common;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Helpers.JsonSerializer;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Model.Enums;
using Xunit;
namespace SolEx.Hurt.Helpers.Tests
{
    public class APIWywolaniaTests
    {
        //Do prawidłowego działania testów trzeba ustawiec 3 poniższcze zmienne

        private string url= "http://localhost/";
        private string kluczApi = "161caa49-3170-45a8-b8f3-0965b752a32e";
        private string kluczApiKlienta = "cc1097e0-4dd6-4e96-8b7e-eb48693aa273";
        //private Dictionary<int,Cecha> WygenerujeCechy1000()
        //{
        //    Dictionary<int, Cecha> wynik = new Dictionary<int,Cecha>();
        //    for (int i = 0; i < 1000; i++)
        //    {
        //        wynik.Add(i+1, new Cecha(){Id = i+1});
        //    }
        //    return wynik;
        //}
        //private Dictionary<int, Cecha> WygenerujeCechy500()
        //{
        //    Dictionary<int, Cecha> wynik = new Dictionary<int, Cecha>();
        //    for (int i = 1000; i < 1500; i++)
        //    {
        //        wynik.Add(i + 1, new Cecha() { Id = i + 1 });
        //    }
        //    return wynik;
        //}


        //[Fact(DisplayName = "Test wydajnościowy metody pobierajacej cechy")]
        //public void PobierzCechyTest()
        //{
        //    List<int> ListaId = new List<int>();
        //    for (int i = 0; i < 1500; i++)
        //    {
        //        ListaId.Add(i+1);
        //    }

        //    var api = A.Fake<APIWywolania>();

        //    CechySearchCriteria items = new CechySearchCriteria() { AddtionalSQL = " cecha_id>0 " };
        //    IEnumerable<int> listaidklientow = ListaId.Take(1000);
        //    items.cecha_id.AddRange(listaidklientow);

        //    A.CallTo(() => api.PobierzCechyId()).Returns(ListaId);
        //    A.CallTo(
        //        () =>
        //            api.UniwersalneWywolanieAPI<Dictionary<int, cechy>>(A<string>.Ignored, null,
        //               items, null, null, A<int>.Ignored)).Returns(WygenerujeCechy1000());

        //   Dictionary<int, cechy> wynik = api.PobierzCechy();
        //}




        [Fact(DisplayName = "Test sprawdzający pobieranie produktów nową metodą")]
        public void PobierzProduktyTest()
        {
            var api = A.Fake<APIWywolania>();
            api.Client.BaseUrl = new Uri(url);
            A.CallTo(() => api.SesjaId).Returns(kluczApi);
            A.CallTo(() => api.URL).Returns(url);
            Stopwatch stopWatch = new Stopwatch();
            api.ZalogujKlienta();
            stopWatch.Start();
            var produkty =  api.PobierzProdukty();
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            stopWatch.Restart();
            stopWatch.Start();
            var produkty2 =  api.PobierzProdukty();
            stopWatch.Stop();
            TimeSpan ts2 = stopWatch.Elapsed;
            api.WylogujKlienta();
            Assert.True(ts2<ts && ts2.Milliseconds<1);
        }

        [Fact(DisplayName = "Test sprawdzający pobieranie/Aktualizacja/Dodawanie/Usuwanie cech nową metodą")]
        public void PobierzCechTest()
        {
            var api = A.Fake<APIWywolania>();
            api.Client.BaseUrl = new Uri(url);
            A.CallTo(() => api.SesjaId).Returns(kluczApi);
            A.CallTo(() => api.URL).Returns(url);


            api.ZalogujKlienta();
            //Pobieranie
            var cechy = api.PobierzCechy();

            //Dodawania 
            Cecha c = new Cecha("testujemy","testujemy");
            long id = 12345678;
            c.Id = id;
            api.AktualizujCechy(new List<Cecha>() {c});

            //Usuwanie cechy
            api.UsunCechy(new HashSet<long>() {id});
            api.WylogujKlienta();

            Assert.True(true);
        }

        [Fact(DisplayName = "Test sprawdzający pobieranie cechyZAtrybutem")]
        public void PobierzCechZAtrybutemTest()
        {
            var api = A.Fake<APIWywolania>();
            api.Client.BaseUrl = new Uri(url);
            A.CallTo(() => api.SesjaId).Returns(kluczApi);
            A.CallTo(() => api.URL).Returns(url);


            api.ZalogujKlienta();
            //Pobieranie
            var cechy = api.PobierzCechy();

            var atrybut = cechy.FirstOrDefault(x => x.Value.AtrybutId.HasValue).Value.AtrybutId.Value;
            var cechyZAtrybutem = api.PobierzCechyDlaAtrybutow(new HashSet<int>() {atrybut});
            api.WylogujKlienta();

            Assert.True(cechy.Count(x=>x.Value.AtrybutId.Value==atrybut)==cechyZAtrybutem.Count);
        }

        [Fact(DisplayName = "Test sprawdzający pobieranie/Aktualizacja/Dodawanie/Usuwanie atrybutow nową metodą")]
        public void PobierzAtrybutyTest()
        {
            var api = A.Fake<APIWywolania>();
            api.Client.BaseUrl = new Uri(url);
            A.CallTo(() => api.SesjaId).Returns(kluczApi);
            A.CallTo(() => api.URL).Returns(url);


            api.ZalogujKlienta();
            //Pobieranie
            var atrybuty = api.PobierzAtrybuty();

            //Dodawania 
            int id = 1234567;
            Atrybut at = new Atrybut("testujemy", 1234567);
            at.JezykId = 1;
            api.AktualizujAtrybuty(new List<Atrybut>() { at });

            //Usuwanie atrybutu
            api.UsunAtrybuty(new HashSet<int>() { id });

            api.WylogujKlienta();
            Assert.True(true);
        }

        [Fact(DisplayName = "Test sprawdzający pobieranie grup nową metodą")]
        public void PobierGrupyTest()
        {
            var api = A.Fake<APIWywolania>();
            api.Client.BaseUrl = new Uri(url);
            A.CallTo(() => api.SesjaId).Returns(kluczApi);
            A.CallTo(() => api.URL).Returns(url);

            api.ZalogujKlienta();
            //Pobieranie
            var grupy = api.PobierzGrupy();
            api.WylogujKlienta();
            Assert.True(true);
        }
        [Fact(DisplayName = "Test sprawdzający pobieranie kategorii nową metodą")]
        public void PobierKategorieTest()
        {
            var api = A.Fake<APIWywolania>();
            api.Client.BaseUrl = new Uri(url);
            A.CallTo(() => api.SesjaId).Returns(kluczApi);
            A.CallTo(() => api.URL).Returns(url);

            api.ZalogujKlienta();
            //Pobieranie
            var kategorieProduktu = api.PobierzKategorie();
            api.WylogujKlienta();
            Assert.True(true);
        }

        [Fact(DisplayName = "Test sprawdzający pobieranie lacznikow cech nową metodą")]
        public void PobierLacznikiCechTest()
        {
            var api = A.Fake<APIWywolania>();
            api.Client.BaseUrl = new Uri(url);
            A.CallTo(() => api.SesjaId).Returns(kluczApi);
            A.CallTo(() => api.URL).Returns(url);

            api.ZalogujKlienta();
            //Pobieranie
            var lacznikiCech = api.PobierzCechyProdukty();

            ProduktCecha pc = new ProduktCecha()
            {
                CechaId = 68445,
                ProduktId = 10
            };
            api.AktualizujCechyProdukty(new List<ProduktCecha>() {pc});

            //usuwamy

            api.UsunCechyProdukty(new HashSet<long>() { pc.Id });


            api.WylogujKlienta();
            Assert.True(true);
        }

        [Fact(DisplayName = "Test sprawdzający pobieranie lacznikow cech nową metodą")]
        public void PobierLacznikiCechDlaPodanychIdCechTest()
        {
            var api = A.Fake<APIWywolania>();
            api.Client.BaseUrl = new Uri(url);
            A.CallTo(() => api.SesjaId).Returns(kluczApi);
            A.CallTo(() => api.URL).Returns(url);

            api.ZalogujKlienta();
            //Pobieranie
            var lacznikiCech = api.PobierzCechyProdukty();

            var id = new HashSet<long>( lacznikiCech.Take(100).Select(x => x.Value.CechaId) );
            var lacznikiCech2 = api.PobierzCechyProdukty(id, 50);
           
            api.WylogujKlienta();
            Assert.True(lacznikiCech.Count(x=>id.Contains(x.Value.CechaId))==lacznikiCech2.Count);
        }

        [Fact(DisplayName = "Test sprawdzający pobieranie dokumentow nową metodą")]
        public void PobierzDokumentyTest()
        {
            var api = A.Fake<APIWywolania>();
            api.Client.BaseUrl = new Uri(url);
            A.CallTo(() => api.SesjaId).Returns(kluczApi);
            A.CallTo(() => api.URL).Returns(url);

            api.ZalogujKlienta();
            //Pobieranie
            var hashDokumentow = api.PobierzHashDokumentow();

          

            //usuwamy
            
            api.DeleteDocuments(new HashSet<int>() { 67342 });
            var hashDokumentow2 = api.PobierzHashDokumentow();


            api.WylogujKlienta();
            Assert.True(true);
        }


        [Fact(DisplayName = "Test sprawdzający pobieranie/aktualizacje klientow nową metodą")]
        public void PobierKlietnowTest()
        {
            var api = A.Fake<APIWywolania>();
            api.Client.BaseUrl = new Uri(url);
            A.CallTo(() => api.SesjaId).Returns(kluczApi);
            A.CallTo(() => api.URL).Returns(url);

            api.ZalogujKlienta();
            //Pobieranie
            var klienci = api.PobierzKlientow();
            string test = klienci.First(x => x.Key > 0).Value.PoleTekst1;
            Klient k = new Klient(klienci.First(x=>x.Key>0).Value);
            k.PoleTekst1 = "TEstujemy";
            api.AktualizujKlientow(new List<Klient>() {k});

            k.PoleTekst1 = test;
            api.AktualizujKlientow(new List<Klient>() { k });

            api.WylogujKlienta();
            Assert.True(true);
        }



        [Fact(DisplayName = "Test sprawdzający logowanie/wylogowanie klientow nową metodą")]
        public void ZalogowanieWylogowanieTest()
        {
            var api = A.Fake<APIWywolania>();
            api.Client.BaseUrl = new Uri(url);
            A.CallTo(() => api.SesjaId).Returns(kluczApi);
            A.CallTo(() => api.URL).Returns(url);

            //Pobieranie
            api.ZalogujKlienta();
            api.WylogujKlienta();

            Assert.True(true);
        }



        [Fact(DisplayName = "Test sprawdzający pobieranie metody bez obslugi pagowania - powinien byc wyjątek")]
        public void PobierzMetodaBezPagowaniaTest()
        {
            var api = A.Fake<APIWywolania>();
            api.Client.BaseUrl = new Uri(url);
            A.CallTo(() => api.SesjaId).Returns(kluczApi);
            A.CallTo(() => api.URL).Returns(url);

            RestClient Client = new RestClient(url);
            Client.AddHandler("application/json", NewtonsoftJsonSerializer.Default);
            Client.AddHandler("text/json", NewtonsoftJsonSerializer.Default);
            Client.AddHandler("text/x-json", NewtonsoftJsonSerializer.Default);
            Client.AddHandler("text/javascript", NewtonsoftJsonSerializer.Default);
            Client.AddHandler("*+json", NewtonsoftJsonSerializer.Default);

            var request = new RestRequest("/api2/zamowienia/zamowienia", Method.GET);
            request.AddHeader("PageSize", 10000.ToString());

            request.RequestFormat = DataFormat.Json;
            request.JsonSerializer = NewtonsoftJsonSerializer.Default;

            api.ZalogujKlienta();
            request.AddHeader("SesjaGuid", api.SesjaGuid.ToString());
            var response = Client.Execute<List<ZamowienieSynchronizacja>>(request);
            api.WylogujKlienta();
            Assert.True(response.StatusCode==HttpStatusCode.BadRequest);
        }

        [Fact(DisplayName = "Test sprawdzający pobieranie produktów bez podanego klucza sesji w requescie")]
        public void BrakKluczaAutoryzacjaTest()
        {
            RestClient Client = new RestClient(url);
            Client.AddHandler("application/json", NewtonsoftJsonSerializer.Default);
            Client.AddHandler("text/json", NewtonsoftJsonSerializer.Default);
            Client.AddHandler("text/x-json", NewtonsoftJsonSerializer.Default);
            Client.AddHandler("text/javascript", NewtonsoftJsonSerializer.Default);
            Client.AddHandler("*+json", NewtonsoftJsonSerializer.Default);

            var request = new RestRequest("/api2/zamowienia/zamowienia", Method.GET);

            request.RequestFormat = DataFormat.Json;
            request.JsonSerializer = NewtonsoftJsonSerializer.Default;

            var response = Client.Execute<List<ZamowienieSynchronizacja>>(request);
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized);
        }


        [Fact(DisplayName = "Test sprawdzający pobieranie produktów z nieistniejacym kluczem sesji w requescie")]
        public void LosowyKluczAutoryzacjaTest()
        {
            RestClient Client = new RestClient(url);
            Client.AddHandler("application/json", NewtonsoftJsonSerializer.Default);
            Client.AddHandler("text/json", NewtonsoftJsonSerializer.Default);
            Client.AddHandler("text/x-json", NewtonsoftJsonSerializer.Default);
            Client.AddHandler("text/javascript", NewtonsoftJsonSerializer.Default);
            Client.AddHandler("*+json", NewtonsoftJsonSerializer.Default);

            var request = new RestRequest("/api2/zamowienia/zamowienia", Method.GET);
            request.AddHeader("SesjaGuid", new Guid().ToString());

            request.RequestFormat = DataFormat.Json;
            request.JsonSerializer = NewtonsoftJsonSerializer.Default;

            var response = Client.Execute<List<ZamowienieSynchronizacja>>(request);
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized);
        }

        [Fact(DisplayName = "Test sprawdzający pobieranie produktów z sesja ktora została wylogowana w requescie")]
        public void PoWylogowaniuAutoryzacjaTest()
        {
            var api = A.Fake<APIWywolania>();
            api.Client.BaseUrl = new Uri(url);
            A.CallTo(() => api.SesjaId).Returns(kluczApi);
            A.CallTo(() => api.URL).Returns(url);

            //Pobieranie
            api.ZalogujKlienta();
            var sesja = api.SesjaGuid;
            api.WylogujKlienta();

            RestClient Client = new RestClient(url);
            Client.AddHandler("application/json", NewtonsoftJsonSerializer.Default);
            Client.AddHandler("text/json", NewtonsoftJsonSerializer.Default);
            Client.AddHandler("text/x-json", NewtonsoftJsonSerializer.Default);
            Client.AddHandler("text/javascript", NewtonsoftJsonSerializer.Default);
            Client.AddHandler("*+json", NewtonsoftJsonSerializer.Default);

            var request = new RestRequest("/api2/zamowienia/zamowienia", Method.GET);
            request.AddHeader("SesjaGuid", sesja.ToString());

            request.RequestFormat = DataFormat.Json;
            request.JsonSerializer = NewtonsoftJsonSerializer.Default;
            
            var response = Client.Execute<List<ZamowienieSynchronizacja>>(request);
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized);
        }
        [Fact(DisplayName = "Test sprawdzający pobieranie produktów z poprawna sesja w requescie")]
        public void PoprawnaSesjaAutoryzacjaTest()
        {
            var api = A.Fake<APIWywolania>();
            api.Client.BaseUrl = new Uri(url);
            A.CallTo(() => api.SesjaId).Returns(kluczApi);
            A.CallTo(() => api.URL).Returns(url);

            //Pobieranie
            api.ZalogujKlienta();
            var sesja = api.SesjaGuid;
          

            RestClient Client = new RestClient(url);
            Client.AddHandler("application/json", NewtonsoftJsonSerializer.Default);
            Client.AddHandler("text/json", NewtonsoftJsonSerializer.Default);
            Client.AddHandler("text/x-json", NewtonsoftJsonSerializer.Default);
            Client.AddHandler("text/javascript", NewtonsoftJsonSerializer.Default);
            Client.AddHandler("*+json", NewtonsoftJsonSerializer.Default);

            var request = new RestRequest("/api2/zamowienia/zamowienia", Method.GET);
            request.AddHeader("SesjaGuid", sesja.ToString());

            request.RequestFormat = DataFormat.Json;
            request.JsonSerializer = NewtonsoftJsonSerializer.Default;

            var response = Client.Execute<List<ZamowienieSynchronizacja>>(request);
            api.WylogujKlienta();
            Assert.True(response.StatusCode == HttpStatusCode.OK);
        }



        [Fact(DisplayName = "Test sprawdzający pobieranie produktów z kluczem sesji klienta w requescie")]
        public void KluczSesjiKlientaAutoryzacjaTest()
        {
            var api = A.Fake<APIWywolania>();
            api.Client.BaseUrl = new Uri(url);
            A.CallTo(() => api.SesjaId).Returns(kluczApiKlienta);
            A.CallTo(() => api.URL).Returns(url);

            //Pobieranie
            api.ZalogujKlienta();
            var sesja = api.SesjaGuid;
            

            RestClient Client = new RestClient(url);
            Client.AddHandler("application/json", NewtonsoftJsonSerializer.Default);
            Client.AddHandler("text/json", NewtonsoftJsonSerializer.Default);
            Client.AddHandler("text/x-json", NewtonsoftJsonSerializer.Default);
            Client.AddHandler("text/javascript", NewtonsoftJsonSerializer.Default);
            Client.AddHandler("*+json", NewtonsoftJsonSerializer.Default);

            var request = new RestRequest("/api2/zamowienia/zamowienia", Method.GET);
            request.AddHeader("SesjaGuid", sesja.ToString());

            request.RequestFormat = DataFormat.Json;
            request.JsonSerializer = NewtonsoftJsonSerializer.Default;

            var response = Client.Execute<List<ZamowienieSynchronizacja>>(request);
            api.WylogujKlienta();
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized);
        }

    }
}
