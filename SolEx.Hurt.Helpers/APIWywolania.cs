using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using Newtonsoft.Json;
using ServiceStack.Common;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Model.Web;
using System.Configuration;
using System.Runtime;
using RestSharp;
using ServiceStack.Text;
using SolEx.Hurt.Helpers.JsonSerializer;
using SolEx.Hurt.Model.Core;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using RestRequest = RestSharp.RestRequest;
using ServiceStack.Messaging.Rcon;
using log4net.Util;

namespace SolEx.Hurt.Helpers
{
    public class APIWywolania : IAPIWywolania
    {
        public RestClient Client = new RestClient();


        //prywatne keszowanie, żeby nie pobierać po kilka razy tych samych danych
        private List<Produkt> _produkty;
        private List<Cecha> _cechy;
        private List<ProduktCecha> _produktCecha;

        public APIWywolania()
        {
            Client.AddHandler("application/json", NewtonsoftJsonSerializer.Default);
            Client.AddHandler("text/json", NewtonsoftJsonSerializer.Default);
            Client.AddHandler("text/x-json", NewtonsoftJsonSerializer.Default);
            Client.AddHandler("text/javascript", NewtonsoftJsonSerializer.Default);
            Client.AddHandler("*+json", NewtonsoftJsonSerializer.Default);
        }

        public void ZalogujKlienta()
        {
            Dictionary<string, object> slownik = new Dictionary<string, object>();
            slownik.Add("id", SesjaId);
            slownik.Add("nazwaKomputera", Environment.MachineName);
            SesjaGuid = UniwersalneWywolanieApiZParametrami<Guid>("api2/klienci/Zaloguj/{id}/{nazwaKomputera}", Method.POST, slownik);
        }


        private T UniwersalneWywolanieApiZParametrami<T>(string adres,Method metoda, Dictionary<string,object> parametry,ParameterType typ = ParameterType.UrlSegment) where T : new()
        {
            var request = new RestRequest(adres);
            
            request.RequestFormat = DataFormat.Json;
            if (SesjaGuid!=default(Guid))
            {
                request.AddHeader("SesjaGuid", SesjaGuid.ToString());
            }
            request.JsonSerializer = NewtonsoftJsonSerializer.Default;
            switch (metoda)
            {
                case Method.GET:
                    request.Method = Method.GET;
                    break;
                case Method.POST:
                    request.Method = Method.POST;
                    break;
                case Method.DELETE:
                    request.Method = Method.DELETE;
                    break;
            }
            foreach (var param in parametry)
            {
                if (param.Value == null)
                {
                    continue;
                }
                string wartosc = string.Empty;
                //Sprawdzamy jakiego typu jest przekzywany parametr 
                Type t = param.Value.GetType();
                if (t.IsArray || t.IsGenericType)
                {
                    foreach (var elementListy in (IList)param.Value)
                    {
                        wartosc += $"{elementListy},";
                    }
                    wartosc=wartosc.TrimEnd(',');
                    if (!string.IsNullOrEmpty(wartosc))
                    {
                        request.AddParameter(param.Key, wartosc, ParameterType.QueryString);
                    }
                }
                else
                {
                    wartosc = param.Value.ToString();
                    request.AddParameter(param.Key, wartosc, typ);
                }
                
            }

            var response = Client.Execute<T>(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Log.Error($"Wystąpił błąd podczas wysyłania danych: {response.StatusDescription}, {response.Content}, kod błędu serwera: {response.StatusCode}. Adres requestu: {adres}, adres pełny: {Client.BuildUri(request)},  zrzut parametrów: {request.Parameters.ToJson()}");
                throw new APIException($"Wystąpił błąd podczas wysyłania danych: {response.StatusDescription}. Adres requestu: {adres}, adres pełny: {Client.BaseUrl}.");
            }
            if (typeof(T) == typeof(IRestResponse))
            {
                return default(T);
            }
            return response.Data;
        }
        
        private T UniwersalneWywolaneApi<T>(string adres, Method metoda, object daneDoWyslania, int? paczka=null, int? iloscElementow=null) where T : new()
        {
            var request = new RestRequest(adres);
            if (paczka.HasValue && iloscElementow.HasValue)
            {
                request.AddHeader("PageNumber", paczka.ToString());
                request.AddHeader("PageSize", iloscElementow.ToString());
            }
            if (SesjaGuid != default(Guid))
            {
                request.AddHeader("SesjaGuid", SesjaGuid.ToString());
            }
            request.RequestFormat = DataFormat.Json;
            request.JsonSerializer = NewtonsoftJsonSerializer.Default;
            switch (metoda)
            {
                case Method.GET:
                    request.Method = Method.GET;
                break;
                case Method.POST:
                    request.Method = Method.POST;
                    request.AddJsonBody(daneDoWyslania);
                    break;
                case Method.DELETE:
                    request.Method=Method.DELETE;
                    request.AddJsonBody(daneDoWyslania);
                    break;
            }

            //timeut na odpowiedz - musi byc krotki bo sie wiesza synchro
            request.Timeout = 1000*120; //x sekund na odpowiedz - musi tyle starczyc - najwyzej bedzie 2 proba

            IRestResponse<T> response = Client.Execute<T>(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Log.Error($"Wystąpił błąd podczas wysyłania danych: {response.StatusDescription}, {response.Content}, kod błędu serwera: {response.StatusCode}. Adres requestu: {adres}, adres pełny: {Client.BaseUrl},  zrzut parametrów: {request.Parameters.ToJson()}");
                throw new APIException($"Wystąpił błąd podczas wysyłania danych: {response.StatusDescription}. Adres requestu: {adres}, adres pełny: {Client.BaseUrl}.");
            }
            if (typeof(T) == typeof(IRestResponse))
            {
                return default(T);
            }
            return response.Data;
        }


        private void UniwersalneWywolaneApiPaczkowanieWysylanie<T>(string adres, Method metoda, IEnumerable<T> daneDoWyslania, int iloscWPaczce) where T : new()
        {
            if (daneDoWyslania == null || !daneDoWyslania.Any())
            {
                return;
            }
            if (iloscWPaczce >= daneDoWyslania.Count())
            {
                UniwersalneWywolaneApi<bool>(adres, metoda, daneDoWyslania, null, null);
                return;
            }
            int pozycjaPoczatkowa = 0;
            int calosc = daneDoWyslania.Count();
            while (calosc > 0)
            {
                int max = calosc > iloscWPaczce ? iloscWPaczce : calosc;
                IEnumerable<T> nowaLista = daneDoWyslania.Take(max);
                Log.DebugFormat($"Wysyłanie paczkowe danych - paczka od {pozycjaPoczatkowa} do {pozycjaPoczatkowa + max} pozostało {calosc}. Metoda: {adres}.");
                pozycjaPoczatkowa += max;
                UniwersalneWywolaneApi<bool>(adres, metoda, nowaLista,null,null);
                daneDoWyslania = daneDoWyslania.Skip(max);
                calosc = daneDoWyslania.Count();
            }
        }



        private List<T> UniwersalneWywolaneApiPaczkowaniePobieranie<T>(string adres, Method metoda, int iloscWPaczce, HashSet<object> elemntyDoWyslania=null) where T : new()
        {
            int iloscPobrana = iloscWPaczce;
            int paczka = 1;
            List<T> wynikKoncowy = new List<T>();

            if (elemntyDoWyslania != null)
            {
                if (elemntyDoWyslania.Count < iloscWPaczce)
                {
                    return UniwersalneWywolaneApi<List<T>>(adres, metoda, elemntyDoWyslania);
                }
            }
            while (iloscPobrana >= iloscWPaczce)
            {
                HashSet<object> doWyslania = null;
                if (elemntyDoWyslania != null)
                {
                    doWyslania = new HashSet<object>( elemntyDoWyslania.Skip((paczka-1)*iloscWPaczce).Take(iloscWPaczce) );
                }
                List<T> obiekty = UniwersalneWywolaneApi<List<T>>(adres, metoda, doWyslania, paczka, iloscWPaczce);
                wynikKoncowy.AddRange(obiekty);
                iloscPobrana = obiekty.Count;
                paczka++;
            }
            return wynikKoncowy;
        }

        public static APIWywolania PobierzInstancje { get; } = new APIWywolania();

        private log4net.ILog Log => log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private  string _url = "";
        private  string _sesjaID;

        public virtual string SesjaId
        {
            get
            {
                if (_sesjaID == null)
                {
                    _sesjaID = ConfigurationManager.AppSettings["api_token"];
                }

                return _sesjaID;
            }
        }

        public virtual Guid SesjaGuid { get; set; }


        public virtual string URL
        {
            get
            {
                if (string.IsNullOrEmpty(_url))
                {
                    _url = ConfigurationManager.AppSettings["url"];
                    Client.BaseUrl=new Uri(_url);
                }
                return _url;
            }
        }
        /// <summary>
        /// Czyści koszyk zalogowanego użytkownika
        /// </summary>
        /// <returns></returns>
        public  void ClearBasket()
        {
            UniwersalneWywolanieAPI<Status>("/api/WyczyscKoszykHandler.ashx", null, null);
        }

        /// <summary>
        /// Dodaje uwagi do koszyka
        /// </summary>
        /// <param name="desc">Opis</param>
        /// <param name="clear">Czy usunąć poprzednie uwagi</param>
        /// <returns></returns>
        public  void AddDescription(string desc, bool clear)
        {
            KeyValuePair<string, bool> pars = new KeyValuePair<string, bool>(desc, clear);
            UniwersalneWywolanieAPI<Status>("/api/DodajUwagiHandler.ashx", pars, null);
        }

        /// <summary>
        /// Dodaje pozycje do koszyka
        /// </summary>
        /// <param name="items">Pozycje do dodania</param>
        /// <returns></returns>
        public  void AddPRoductsToBasket(List<KeyValuePair<string, decimal>> items)
        {
            throw new NotImplementedException();
         //   UniwersalneWywolanieAPI<Status>("/api/DodajDoKoszykaHandler.ashx", items, null);
        }
        public  List<long> PobierzDostepneProduktyKlienta(long klienta)
        {
            return UniwersalneWywolanieAPI<List<long>>("/Api/klienci.oferta.pobierz.ashx", klienta, null);
        }
        public void DodajDokumenty(List<KlasaOpakowujacaDokumentyDoWyslania> items)
        {
            UniwersalneWywolanieAPIPaczkowane("/api/Dokumenty.dodaj.ashx", items, 50);
        }
        public  void DeleteDocuments(HashSet<int> items)
        {
            UniwersalneWywolaneApiPaczkowanieWysylanie("api2/dokumenty/usun", Method.DELETE, items, 10000);
            //UniwersalneWywolanieAPI<Status>("/api/Dokumenty.usun.ashx", items, null);
        }

        public  Dictionary<int,PoziomCenowy> PobierzPoziomyCen()
        {
            return UniwersalneWywolanieAPI<Dictionary<int, PoziomCenowy>>("/api/produkty.poziomycen.pobierz.ashx", null, null);
        }

        public Dictionary<long, Klient> PobierzKlientow()
        {
            List<Klient> klienci = UniwersalneWywolaneApiPaczkowaniePobieranie<Klient>("api2/klienci", Method.GET, 100);
            return klienci?.ToDictionary(x => x.Id, x => x);
        }

        public List<long> PobierzKlientowId()
        {
            return UniwersalneWywolanieAPI<List<long>>("/Api/klienci.hash.pobierz.ashx", null, null);
        }
        public  void AktualizujKlientow(IEnumerable<Klient> items)
        {
            UniwersalneWywolaneApiPaczkowanieWysylanie("api2/klienci/aktualizuj",Method.POST, items,50);
            //UniwersalneWywolanieAPIPaczkowane("/api/klienci.dodaj.ashx", items, 50);
        }


        public Dictionary<long, Produkt> PobierzProdukty()
        {

            if (_produkty == null)
            {
                List<Produkt> produkty = UniwersalneWywolaneApiPaczkowaniePobieranie<Produkt>("/api2/produkty", Method.GET, 1000);
                if (produkty == null)
                {
                    return null;
                }
                _produkty = produkty;
            }
            //zwracamy kopię slownika z cechami żeby nikt nie zmieniał danych jakie są realnie w bazie 
            return _produkty.Select(x => x.ClonePojedynczyObiekt()).ToDictionary(x => x.Id, x => x);
        }



        public  Dictionary<int, Jezyk> GetLanguages()
        {
            return UniwersalneWywolanieAPI<Dictionary<int, Jezyk>>("/api/jezyki.pobierz.ashx", null, null);

        }

        public  List<Ustawienie> GetSettings()
        {
            return UniwersalneWywolanieAPI<List<Ustawienie>>("/api/ustawienia.pobierz.ashx", null, null);
        }

        public  void UpdateSetting(List<Ustawienie> items)
        {
             UniwersalneWywolanieAPI<List<Ustawienie>>("/api/ustawienia.dodaj.ashx", items, null);
        }

        public  Ustawienie GetSetting(string symbol)
        {
            return GetSettings().FirstOrDefault(x=>x.Symbol==symbol && x.OddzialId==null);
        }

        public  List<TlumaczeniePole> GetSystemNames()
        {
            return UniwersalneWywolanieAPI<List<TlumaczeniePole>>("/api/system.pola.pobierz.ashx", null, null);
        }

        public  Dictionary<long, Tlumaczenie> GetSlowniki()
        {
           // if(items == null) items = new SlownikiSearchCriteria();

            Dictionary<long, Tlumaczenie> wynik = new Dictionary<long, Tlumaczenie>();
            var tlumaczenia = UniwersalneWywolaneApi<List<Tlumaczenie>>("api2/Tlumaczenia", Method.GET, null);
            if (tlumaczenia == null)
            {
                return null;
            }
            Dictionary<long, Tlumaczenie> wszystkie = tlumaczenia.ToDictionary(x => x.Id, x => x);

            //  = UniwersalneWywolanieAPI<Dictionary<long, Tlumaczenie>>("/api/system.slowniki.pobierz.ashx", null, null, null, null, 1000000); 

            List<int> jezyki  =  UniwersalneWywolaneApi<Dictionary<int, Jezyk>>("api2/Jezyki",Method.GET,null).Where(x=>!x.Value.Domyslny).Select(x=>x.Key).ToList();
            foreach (int i in jezyki)
            {
                Dictionary<long, Tlumaczenie> paczka = wszystkie.Where(x => x.Value.JezykId == i).ToDictionary(x => x.Key, x => x.Value);
                foreach (var slowniki in paczka)
                {
                    wynik.Add(slowniki.Key,slowniki.Value);
                }   
            }
            return wynik;
        }

        public  Dictionary<int,Atrybut> PobierzAtrybuty()
        {
            var atrybuty = UniwersalneWywolaneApi<List<Atrybut>>("api2/atrybuty", Method.GET, null);
            return atrybuty?.ToDictionary(x => x.Id, x => x);
            //return UniwersalneWywolanieAPI<Dictionary<int,Atrybut>>("/api/atrybuty.pobierz.ashx", null, null);
        }

        public Dictionary<long, Cecha> PobierzCechyDlaAtrybutow(HashSet<int>idAtrybutow)
        {
            var cechy = UniwersalneWywolaneApi<List<Cecha>>("api2/cechy/cechyZatrybutem", Method.POST, idAtrybutow);
            return cechy?.ToDictionary(x => x.Id, x => x);
        }

        public Dictionary<long, Cecha> PobierzCechy()
        {
            if (_cechy == null)
            {
                List<Cecha> cechy = UniwersalneWywolaneApi<List<Cecha>>("api2/cechy", Method.POST, null);
                if (cechy == null)
                {
                    return null;
                }
                _cechy= cechy;
            }
            //zwracamy kopię slownika z cechami żeby nikt nie zmieniał danych jakie są realnie w bazie 
            return _cechy.Select(x=>x.ClonePojedynczyObiekt()).ToDictionary(x => x.Id, x => x);
        }

        public List<long> PobierzCechyId()
        {
            return UniwersalneWywolanieAPI<List<long>>("/Api/cechy.hash.pobierz.ashx", null, null);
        }
        public  List<TlumaczeniePole> AddSystemNames(List<TlumaczeniePole> items)
        {
            return UniwersalneWywolanieAPI<List<TlumaczeniePole>>("/api/system.pola.aktualizuj.ashx", items, null);
        }
        
        public  List<Zadanie> PobierzZadaniaSynchronizatora()
        {
            return UniwersalneWywolanieAPI<List<Zadanie>>("/api/zadania.pobierz.ashx", null, null);
        }
        public  void WyslijMailePowiatalne()
        {
            UniwersalneWywolanieAPI<Status>("/Api/klienci.powitanie.ashx", null, null);
        }
        public void WyslijMaileNoweProdukty(List<long> idCechyKoniecznej, List<long> idCechyNieMozePosiadac, bool wysylajDoSubkont)
        {
            Dictionary<string, object> parametry = new Dictionary<string, object>();
            parametry.Add("idCechyKoniecznej", idCechyKoniecznej);
            parametry.Add("idCechyNieMozePosiadac", idCechyNieMozePosiadac);
            parametry.Add("wysylajDoSubkont", wysylajDoSubkont);

            UniwersalneWywolanieApiZParametrami<bool>("api2/maile/MailNoweProdukty", Method.GET, parametry,ParameterType.QueryString);
        }

        public void WyslijMaileBladSynchronizatora(WiadomoscEmail wiadomoscEmail)
        {
            UniwersalneWywolaneApi<bool>("api2/synchronizator/MailBladSynchronizacji", Method.POST, wiadomoscEmail, null, null);
        }
        /// <summary>
        /// Wywołuje metodę wysyłającą ponownie maile, których wcześniej nie udało się wysłać
        /// </summary>
        /// <returns></returns>
        public void WyslijPonownieBledneMaile()
        {
            UniwersalneWywolanieAPI<Status>("/Api/powiadomienia.lista.wyslij.ashx", null, null);
        }
        public  void AktualizujZadania(List<Zadanie> lista)
        {
            UniwersalneWywolanieAPI<Status>("/api/zadania.aktualizuj.ashx", lista, null);
        }
        public  void WykonajZadaniaAdministracyjne()
        {
            UniwersalneWywolanieAPI<Status>("/Api/zadania.administracyjne.wykonaj.ashx", null, null);
        }
        public  void DodajTlumaczenia(IEnumerable<Tlumaczenie> items)
        {
            UniwersalneWywolanieAPIPaczkowane<Tlumaczenie>("/api/system.slowniki.aktualizuj.ashx", items, 3000);
        }

        public void UsunTlumaczenia(IEnumerable<long> ids)
        {
            UniwersalneWywolanieAPIPaczkowane<long>("/api/system.slowniki.usun.ashx", ids, 3000);
        }

        public  void UsunMagazyny(HashSet<int> ids)
        {
            UniwersalneWywolanieAPI<Status>("/api/magazyny.usun.ashx", ids, null);
        }

        public List<Magazyn> _magazyny;
        public  List<Magazyn> PobierzMagazyny()
        {
            if (_magazyny == null)
            {
                _magazyny = UniwersalneWywolanieAPI<List<Magazyn>>("/api/magazyny.pobierz.ashx", null, null);
            }
            return _magazyny.Select(x => x.ClonePojedynczyObiekt()).ToList();
        }
        public  void WyslijPowiadomieniaODostepnosci()
        {
            UniwersalneWywolanieAPI<Status>("/Api/produkty.stany.powiadomienie.ashx", null, null);
        }
        public  void WyslijNewslettery()
        {
            UniwersalneWywolanieAPI<Status>("/Api/powiadomienia.mailing.wyslij.ashx", null, null);
        }
        public  void AktualizujMagazyny(IList<Magazyn> mags)
        {
            UniwersalneWywolanieAPI<Status>("/Api/magazyny.aktualizuj.ashx", mags, null);
            _magazyny = null;
        }


        public  List<ProduktStan> PobierzStanyProduktow(Magazyn mag)
        {
            return UniwersalneWywolanieAPI<List<ProduktStan>>("/Api/produkty.stany.pobierz.ashx", mag, null);
        }

        public void AktualizujProdukty(IList<Produkt> produktyDoAktualizacji)
        {

            UniwersalneWywolaneApiPaczkowanieWysylanie("api2/produkty/aktualizuj",Method.POST, produktyDoAktualizacji,500);
            _produkty = null;
        }

        public  void AktualizujStanyProduktow(List<ProduktStan> listaStanow)
        {
            UniwersalneWywolanieAPIPaczkowane<ProduktStan>("/Api/produkty.stany.aktualizuj.ashx", listaStanow, 500);
        }

        public void UsunStanyProduktow(HashSet<long> idStanow)
        {
            UniwersalneWywolaneApi<bool>("api2/ProduktyStany/usun", Method.DELETE, idStanow);
        }

        public  void AktualizujAtrybuty(IList<Atrybut> atrybutyDoAktualizacji)
        {
            UniwersalneWywolaneApi<bool>("api2/atrybuty/aktualizuj", Method.POST, atrybutyDoAktualizacji);
            //UniwersalneWywolanieAPI<Status>("/Api/atrybuty.aktualizuj.ashx", atrybutyDoAktualizacji, null);
        }

        public  void UsunAtrybuty(HashSet<int> listaStanow)
        {
            UniwersalneWywolaneApi<bool>("api2/atrybuty/usun", Method.DELETE, listaStanow);
            // UniwersalneWywolanieAPI<Status>("/Api/atrybuty.usun.ashx", atrybutyDoAktualizacji, null);
        }

        public void AktualizujKategorieProduktow(IList<KategoriaProduktu> atrybutyDoAktualizacji)
        {
            UniwersalneWywolanieAPI<Status>("/Api/kategorie.aktualizuj.ashx", atrybutyDoAktualizacji, null);
        }

        public  void AktualizujKategorieKlientow(IList<KategoriaKlienta> atrybutyDoAktualizacji)
        {
            UniwersalneWywolanieAPI<Status>("/Api/klienci.kategorie.aktualizuj.ashx", atrybutyDoAktualizacji, null);
        }

        public  void UsunKategorieProduktow(HashSet<long> atrybutyDoAktualizacji)
        {
            UniwersalneWywolanieAPIPaczkowane("/Api/kategorie.usun.ashx", atrybutyDoAktualizacji,100);
        }

        public  void AktualizujCechy(IList<Cecha> item)
        {
            UniwersalneWywolaneApiPaczkowanieWysylanie("api2/cechy/aktualizuj",Method.POST, item,1000);
            _cechy = null;
        }

        public  void UsunCechy(HashSet<long> ids)
        {
            UniwersalneWywolaneApiPaczkowanieWysylanie("api2/cechy/usun",Method.DELETE, ids,100);
            _cechy = null;
            //UniwersalneWywolanieAPIPaczkowane("/Api/cechy.usun.ashx", ids, 100);
            //UniwersalneWywolanieAPI<Status>("/Api/cechy.usun.ashx", ids, null);
        }
        public  void UsunKonfekcje(List<long> ids)
        {
            UniwersalneWywolanieAPI<Status>("/Api/produkty.konfekcje.usun.ashx", ids, null);
        }

        public  void UsunCechyProdukty(HashSet<long> elementyDoUsuniecia)
        {
            UniwersalneWywolanieAPIPaczkowane("/Api/produkty.cechy.usun.ashx", elementyDoUsuniecia, 1000);
            _produktCecha = null;
        }

        public  void AktualizujCechyProdukty(IList<ProduktCecha> cechyProduktyDoAktualizacji)
        {
            UniwersalneWywolaneApiPaczkowanieWysylanie("api2/produkty/AktualizujProduktyCechy",Method.POST, cechyProduktyDoAktualizacji,2000);
            _produktCecha = null;
            //UniwersalneWywolanieAPIPaczkowane("/Api/produkty.cechy.aktualizuj.ashx", cechyProduktyDoAktualizacji, 2000);
        }

        public  List<FlatCeny> PobierzCenyKlientow(HashSet<long> idKlienta)
        {
            return UniwersalneWywolanieAPI<List<FlatCeny>>("/Api/klienci.ceny.pobierz.ashx", idKlienta, null);
        }
        public  IEnumerable<Rejestracja> PobierzRejestracje()
        {
            return UniwersalneWywolanieAPI<List<Rejestracja>>("/Api/klienci.rejestracje.pobierz.ashx", null, null);
        }
        public   void AktualizujRejestracje(List<Rejestracja> dane)
        {
            UniwersalneWywolanieAPI<List<Rejestracja>>("/Api/klienci.rejestracje.aktualizuj.ashx", dane, null);
        }

         public  List<long> PobierzRabatyID()
        {
            return UniwersalneWywolanieAPI<List<long>>("/Api/klienci.rabaty.hash.pobierz.ashx", null, null);
        }
         public List<long> PobierzProduktyId()
         {
            return UniwersalneWywolanieAPI< List < long >> ("/Api/produkty.hash.pobierz.ashx", null, null);
        }
         public Dictionary<long, Rabat> PobierzRabaty()
        {
            return PobierzObiekty<long, Rabat>(PobierzRabatyID, "/Api/klienci.rabaty.pobierz.ashx");
            //int max = 5000;
            //List<string> listaidrabatow = PobierzRabatyID();
            //if (listaidrabatow.Count > max)
            //{
            //    Dictionary<string, rabaty> rabaty = new Dictionary<string, rabaty>();
            //    while (listaidrabatow.Count > 0)
            //    {
            //        int dopobrania = listaidrabatow.Count > max ? max : listaidrabatow.Count;
            //        IEnumerable<string> listaidklientow = listaidrabatow.Take(dopobrania);


            //        Dictionary<string, rabaty> pobraneRabaty = UniwersalneWywolanieAPI<Dictionary<string, rabaty>>("/Api/klienci.rabaty.pobierz.ashx", listaidklientow,null);

            //        foreach (KeyValuePair<string, rabaty> pobranyRabatKlienta in pobraneRabaty)
            //        {
            //            if (!rabaty.ContainsKey(pobranyRabatKlienta.Key))
            //            {
            //                rabaty.Add(pobranyRabatKlienta.Key, pobranyRabatKlienta.Value);
            //            }
            //        }
                  
            //        listaidrabatow.RemoveRange(0, dopobrania);
            //    }

            //    return rabaty;
            //}
            //return UniwersalneWywolanieAPI<Dictionary<string, rabaty>>("/Api/klienci.rabaty.pobierz.ashx", null, null);
        }

        public  List<Grupa> PobierzGrupy()
        {
            return UniwersalneWywolaneApi<List<Grupa>>("api2/grupy", Method.GET, null);
            //return UniwersalneWywolanieAPI<List<Grupa>>("/Api/grupy.pobierz.ashx", null, null);
        }

       

        private  void UniwersalneWywolanieAPIPaczkowane<T>(string adresMetody, IEnumerable<T> doWyslania, int rozmiarPaczki)
        {
            if (doWyslania.Count() <= rozmiarPaczki)
            {
                UniwersalneWywolanieAPI<Status>(adresMetody, doWyslania, null);
                return;
            }
            int odIle = 0;
            int calosc = doWyslania.Count();
            while (calosc > 0)
            {
                int max = calosc > rozmiarPaczki ? rozmiarPaczki : calosc;
                IEnumerable<T> nowaLista = doWyslania.Take(max);
                Log.InfoFormat($"Wysyłanie paczkowe danych - paczka od {odIle} do {odIle + max} pozostało {calosc}. Metoda: {adresMetody}.");
                odIle += max;
                UniwersalneWywolanieAPI<Status>(adresMetody, nowaLista, null);
                doWyslania = doWyslania.Skip(max);
                calosc = doWyslania.Count();
            }
        }

        private  T UniwersalneWywolanieAPI<T>(string adresMetody, object doWyslania, BaseSearchCriteria crieria, List<object> parametry = null, string plik = null,int timeout=120000)
        {
            string content = this.ExecuteWithRetries( () => { return this.PrzygotujZadanie(adresMetody, doWyslania, crieria, parametry, plik, timeout); }, 3);
                try
                {
                    Status s = JsonConvert.DeserializeObject<Status>(content);
                    if (s.Result != StatusApi.Ok)
                    {
                        try
                        {
                            string temp = doWyslania.OpisObiektu();
                            Log.DebugFormat("Nie udało się wysłać na serwer przez API danych - próbowano wysłać dane: {0}", temp);
                        }
                        catch
                        {
                            Log.DebugFormat("Nie udało się wysłać na serwer przez API danych");
                        }
                       
                        throw new Exception("Nie udało się wywołać API: " + adresMetody + ". Komunikat " + s.Message);
                    }
                    if (typeof (T) == typeof (Status))
                    {
                        return (T) (s as object);
                    }
                    if (s.Data == null)
                    {
                        return default(T);
                    }
                    return JSonHelper.Deserialize<T>(s.Data.ToString());
                }
                catch(Exception ex)
                {
                    Log.Error($"Błąd odczytu odpowiedzi z API: {ex.Message} dla adersu: [{adresMetody}]. Treść zwrocona z api: " + content);
                    throw;
                }
        }

        public string PobierzURL(string adresMetody)
        {
            return URL + adresMetody;
        }


        private T ExecuteWithRetries<T>(Func<T> func, int maxAttempts)
        {
            int attempt = 0;
            while (true)
            {
                try
                {
                    attempt++;
                    return func(); // jeśli się udało, kończymy
                }
                catch
                {
                    if (attempt >= maxAttempts)
                    {
                        throw; // po ostatniej próbie wywalamy wyjątek
                    }
                    Thread.Sleep(attempt * 5000); // czekamy rosnącą ilość czasu przed ponowną próbą
                    Log.Info($"Wystąpiły błędy - ponawiam próbę ({attempt}/{maxAttempts})...");
                }
            }
        }


        private string PrzygotujZadanie(string adresMetody, object doWyslania, BaseSearchCriteria crieria, IEnumerable<object> parametry, string plik, int timeout)
        {
            string url = PobierzURL(adresMetody);
            HttpWebRequest request;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
            }
            catch (Exception e)
            {
                throw new Exception("Błąd wywołania adresu: " + url + ". Szczegóły: " + e.Message);
            }

            request.Method = "POST";
            request.Accept = "application/json";
            if (!url.EndsWith("/api/api.logowanie.ashx"))
            {
                request.Headers.Add("X-Solex-API", SesjaId);
            }
            request.Timeout = timeout;
            request.KeepAlive = false;
            request.ContentType = "application/x-www-form-urlencoded";
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            string json = JSonHelper.Serialize(doWyslania);
            string doWyslaniaJSon = string.Format("data=[{0}]", json);
            string criteria = JSonHelper.Serialize(crieria);
            string parametryStr = JSonHelper.Serialize(parametry);
            StringBuilder sb = new StringBuilder();
            sb.Append("data=" + HttpUtility.UrlEncode(json) + "&");
            sb.Append("criteria=" + HttpUtility.UrlEncode(criteria) + "&");
            sb.Append("plik=" + HttpUtility.UrlEncode(plik) + "&");
            sb.Append("parametry=" + HttpUtility.UrlEncode(parametryStr));

            // Create POST data and convert it to a byte array.
            string postData = sb.ToString();
            byte[] data = Encoding.UTF8.GetBytes(postData);
            try
            {
                using (Stream str = request.GetRequestStream())
                {
                    str.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat($"Błąd pobierania danych z serwera (1 GetRequestStream). Metoda: {adresMetody}, url: {url}");
                Log.Error(ex);
                Log.DebugFormat($"Do wysłania: {doWyslaniaJSon}");

                throw;
            }

            data = null;
            postData = null;
            sb.Clear();
            sb = null;
            json = null;
            try
            {
                string content = null;
                using (HttpWebResponse wr = (HttpWebResponse)request.GetResponse())
                {
                    try
                    {
                        using (Stream responseStream = wr.GetResponseStream())
                        {
                            if (responseStream == null)
                            {
                                Log.ErrorFormat($"Nie udało się pobrać odpowiedzi z serwera. Link {adresMetody}");
                            }
                            try
                            {
                                using (StreamReader sr = new StreamReader(responseStream))
                                {
                                    content = sr.ReadToEnd();
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.ErrorFormat($"Błąd pobierania danych z serwera (4 StreamReader). Metoda {adresMetody},url: {url}");
                                Log.Error(ex);

                                throw;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.ErrorFormat($"Błąd pobierania danych z serwera (2 GetResponseStream). Metoda: {adresMetody}, url: {url}");
                        Log.Error(ex);
                        Log.DebugFormat($"Do wysłania: {doWyslaniaJSon}");

                        throw;
                    }
                }
                return content;
            }
            catch (Exception ex)
            {
                Log.ErrorFormat($"Błąd pobierania danych z serwera (3 GetResponse). Metoda: {adresMetody}, url: {url}");
                Log.Error(ex);

                Log.DebugFormat($"Do wysłania: {doWyslaniaJSon}");

                throw new Exception($"Błąd pobierania danych z API, url: {url}, wiadomość: {ex.Message}", ex);
            }
        }

        public Dictionary<long, KategoriaProduktu> PobierzKategorie()
        {
            var kategorie = UniwersalneWywolaneApi<List<KategoriaProduktu>>("api2/KategorieProduktu", Method.GET, null);
            return kategorie?.ToDictionary(x => x.Id, x => x);
            //return UniwersalneWywolanieAPI<Dictionary<long, KategoriaProduktu>>("/Api/kategorie.pobierz.ashx", null, null);
        }

        public  Dictionary<int, KategoriaKlienta> PobierzKategorieKlientow()
        {
            return UniwersalneWywolanieAPI<Dictionary<int, KategoriaKlienta>>("/Api/klienci.kategorie.pobierz.ashx", null,null);
        }

        public  Dictionary<long,KlientKategoriaKlienta> PobierzKlienciKategorie(Dictionary<string, object> parametry)
        {
            return UniwersalneWywolanieAPI<Dictionary<long, KlientKategoriaKlienta>>("/Api/klienci.kategorie.polaczenia.pobierz.ashx", parametry, null);
        }

        public  void AktualizujKlienciKategorie(IList<KlientKategoriaKlienta> items)
        {
            UniwersalneWywolanieAPI<List<KlientKategoriaKlienta>>("/Api/klienci.kategorie.polaczenia.aktualizuj.ashx", items,
                                                             null);
        }

        public void UsunKlienciKategorie(HashSet<long> items)
        {
            UniwersalneWywolanieAPI<HashSet<long>>("/Api/klienci.kategorie.polaczenia.usun.ashx", items, null);
        }

        public  void UsunKategorieKlientow(HashSet<int> elementyDoUsuniecia)
        {
            UniwersalneWywolanieAPI<HashSet<int>>("/Api/klienci.kategorie.usun.ashx", elementyDoUsuniecia, null);
        }

        public Dictionary<long, ProduktCecha> PobierzCechyProdukty(HashSet<long> idCech = null, int max = 100000)
        {
            if (_produktCecha == null)
            {
                _produktCecha = new List<ProduktCecha>();
                List<ProduktCecha> laczniki = UniwersalneWywolaneApiPaczkowaniePobieranie<ProduktCecha>("api2/produkty/ProduktyCechy", Method.POST, max);
                if (laczniki != null)
                {
                    _produktCecha = laczniki;
                }
            }
            if (idCech == null || !idCech.Any()) return _produktCecha.Select(x => x.ClonePojedynczyObiekt()).ToDictionary(x => x.Id, x => x);
            
            List<ProduktCecha> lacznikiWybranych = _produktCecha.Where(x => idCech.Contains(x.CechaId)).ToList();
            return lacznikiWybranych.Select(x => x.ClonePojedynczyObiekt()).ToDictionary(x => x.Id, x => x);
        }

        ///// <summary>
        ///// Pobieranie Id łaczników między cechami i produktami 
        ///// </summary>
        ///// <returns></returns>
        //public  HashSet<long> PobierzCechyProduktyID()
        //{
        //    return UniwersalneWywolanieAPI<HashSet<long>>("/Api/produkty.cechy.id.pobierz.ashx", null, null);
        //}

        public  Dictionary<int,HistoriaDokumentuListPrzewozowy> PobierzListyPrzewozowe()
        {
            return UniwersalneWywolanieAPI<Dictionary<int,HistoriaDokumentuListPrzewozowy>>("/api/Dokumenty.listyprzewozowe.pobierz.ashx", null, null);
        }

        public Dictionary<long, ProduktKategoria> PobierzProduktyKategoriePolaczenia()
        {
            return UniwersalneWywolanieAPI<Dictionary<long, ProduktKategoria>>("/Api/produkty.kategorie.pobierz.ashx", null, null);
        }

        public void UsunProduktyKategoriePolaczenia(HashSet<long> elementyDoUsuniecia)
        {
            UniwersalneWywolanieAPI<HashSet<int>>("/Api/produkty.kategorie.usun.ashx", elementyDoUsuniecia, null);
        }

        public void UsunRabaty(HashSet<long> elementyDoUsuniecia)
        {
            UniwersalneWywolanieAPIPaczkowane("/Api/klienci.rabaty.usun.ashx", elementyDoUsuniecia, 5000);
        }

        public  void AktualizujProduktyKategoriePolaczenia(IList<ProduktKategoria> cechyProduktyDoAktualizacji)
        {
            UniwersalneWywolanieAPIPaczkowane("/Api/produkty.kategorie.aktualizuj.ashx",cechyProduktyDoAktualizacji, 500);
        }

        public  List<long> PobierzProduktyUkryteID()
        {
            return UniwersalneWywolanieAPI<List<long>>("/api/klienci.produkt_ukryte.pobierzid.ashx", null, null);
        }

         public  Dictionary<long, ProduktUkryty> PobierzProduktyUkryte(HashSet<int> klientId=null)
        {
            return UniwersalneWywolanieAPI<Dictionary<long, ProduktUkryty>>("/Api/klienci.produkt_ukryte.pobierz.ashx", klientId, null);
        }

         public void AktualizujProduktyUkryte(IList<ProduktUkryty> produktyUkryte)
        {
            UniwersalneWywolanieAPIPaczkowane("/Api/klienci.produkt_ukryte.dodaj.ashx", produktyUkryte, 2000);
        }

        public int PobierzOstatnieIDCechyProdukty()
        {
            return UniwersalneWywolanieAPI<int>("/Api/produkty.cechy.maxid.pobierz.ashx", null, new CechyProduktySearchCriteria());
        }

        public  void UsunProduktyUkryte(HashSet<long> elementyDoUsuniecia)
        {
            UniwersalneWywolanieAPIPaczkowane("/Api/klienci.produkt_ukryte.usun.ashx", elementyDoUsuniecia, 2000);
        }

        public void AktualizujProduktyKategoriePolaczenia(List<ProduktUkryty> pu)
        {
            UniwersalneWywolanieAPI<List<ProduktUkryty>>("/Api/klienci.produkt_ukryte.dodaj.ashx", pu, null);
        }


        public  void AktualizujCenyWyliczoneErp(List<FlatCeny> cenyDoZmiany)
        {
            UniwersalneWywolanieAPIPaczkowane<FlatCeny>("/Api/klienci.wyliczoneceny.dodaj.ashx", cenyDoZmiany, 2000);
        }

        public List<FlatCeny> PobierzCenyWyliczoneErp()
        {
            return PobierzObiekty<long,FlatCeny>(PobierzCenyWyliczoneErpID, "/Api/klienci.wyliczoneceny.pobierz.ashx").Values.ToList();
        }

        private Dictionary<T, TT> PobierzObiekty<T, TT>(Func<IEnumerable<T>> metodaPobierajacaId, string adresApi, int max = 5000)
        {
            var lista = metodaPobierajacaId();
            if (lista == null)
            {
                return null;
            }
            List<T> ids = lista.ToList();
            if (ids.Count > max)
            {
                Log.DebugFormat("Do pobrania {0} elementówtypu: {1}", ids.Count,typeof(TT));
                Dictionary<T, TT> wynik = new Dictionary<T, TT>();
                int paczka = 0;
                while (ids.Count > 0)
                {
                    int dopobrania = ids.Count > max ? max : ids.Count;

                    var listaid = ids.Take(dopobrania).ToList();
                    Dictionary<T, TT> pobraneElementy = UniwersalneWywolanieAPI<Dictionary<T, TT>>(adresApi, listaid, null);
                    foreach (KeyValuePair<T, TT> element in pobraneElementy)
                    {
                        if (!wynik.ContainsKey(element.Key))
                        {
                            wynik.Add(element.Key, element.Value);
                        }
                    }
                    ids.RemoveRange(0, dopobrania);
                    Log.DebugFormat("Pobieranie paczki {0} rozmiar {1}, rzeczywiście pobrano {3} pozostalo {2}", paczka, dopobrania, ids.Count, pobraneElementy.Count);
                    paczka++;
                }
                return wynik;
            }
            return UniwersalneWywolanieAPI<Dictionary<T, TT>>(adresApi, ids, null);
        } 

        private  List<long> PobierzCenyWyliczoneErpID()
        {
            return UniwersalneWywolanieAPI<List<long>>("/Api/klienci.wyliczoneceny.id.pobierz.ashx", null, null);
        }

        public void UsunCenyWyliczoneErp(List<long> cenyDoZmiany)
        {
            UniwersalneWywolanieAPI<Status>("/Api/klienci.wyliczoneceny.usun.ashx", cenyDoZmiany, null);
        }
        public  void AktualizujRabaty(List<Rabat> rabatyDoAktualizacji)
        {
            UniwersalneWywolanieAPIPaczkowane("/Api/klienci.rabaty.aktualizuj.ashx", rabatyDoAktualizacji, 2000);
        }

        public Dictionary<long, CenaPoziomu> PobierzPoziomyCenProduktow()
        {
            List<CenaPoziomu> cenyPoziomow = UniwersalneWywolaneApiPaczkowaniePobieranie<CenaPoziomu>("api2/ceny/CenyPoziomy", Method.POST, 1000);
            return cenyPoziomow?.ToDictionary(x => x.Id, x => x);
        }
        
        public  List<KlientLimitIlosciowy> PobierzLimityIlosciowe()
        {
            return UniwersalneWywolanieAPI<List<KlientLimitIlosciowy>>("/Api/klienci.limityilosciowe.pobierz.ashx", null, null);
        }
        public  void UsunLimityIlosciowe(List<int> ids )
        {
             UniwersalneWywolanieAPI<Status>("/Api/klienci.limityilosciowe.usun.ashx", ids, null);
        }
        public  void AktualizujLimityIlosciowe(List<KlientLimitIlosciowy> limity)
        {
            UniwersalneWywolanieAPI<Status>("/Api/klienci.limityilosciowe.aktualizuj.ashx", limity, null);
        }
        public  void UsunCenyPoziomy(HashSet<long> doUsuniecia)
        {
          //  UniwersalneWywolanieAPIPaczkowane("/Api/produkty.ceny.usun.ashx", doUsuniecia, 500);
            UniwersalneWywolaneApiPaczkowanieWysylanie("api2/ceny/UsunCenyPoziomy", Method.DELETE, doUsuniecia, 500);
        }
        public  void AktualizujPoziomyCenProduktow(IList<CenaPoziomu> ceny)
        {
            UniwersalneWywolaneApiPaczkowanieWysylanie("api2/ceny/AktualizujCenyPoziomy", Method.POST, ceny, 500);
        }

        public  Dictionary<long, Adres> PobierzAdresy()
        {
            return UniwersalneWywolanieAPI < Dictionary<long, Adres>>("/Api/klienci.adresy.pobierz.ashx", null, null);
        }
        public void AktualizujAdresy(IList<Adres> adresy)
        {
            UniwersalneWywolanieAPI<List<Adres>>("/Api/klienci.adresy.dodaj.ashx", adresy, null);
        }
        public  void UsunAdresy(HashSet<long> elementyDoUsunieciaAdresy)
        {
            UniwersalneWywolanieAPI<HashSet<long>>("/Api/klienci.adresy.usun.ashx", elementyDoUsunieciaAdresy, null);
        }
        public Dictionary<long, KlientAdres> PobierzLacznikiAdresow()
        {
            return UniwersalneWywolanieAPI<Dictionary<long, KlientAdres>>("/Api/laczniki.adresy.pobierz.ashx", null, null);
        }

        public void AktualizujLacznikiAdresow(IList<KlientAdres> laczniki)
        {
            UniwersalneWywolanieAPI<List<Adres>>("/Api/laczniki.adresy.dodaj.ashx", laczniki, null);
        }
        public void UsunLacznikiAdresow(HashSet<long> elementyDoUsunieciaAdresy)
        {
            UniwersalneWywolanieAPI<Status>("/Api/laczniki.adresy.usun.ashx", elementyDoUsunieciaAdresy, null);
        }


        public  Dictionary<int, Kraje> PobierzKraje()
        {
            return UniwersalneWywolanieAPI<Dictionary<int, Kraje>>("/Api/klienci.kraje.pobierz.ashx", null,null);
        }

        public  void AktualizujKraje(IList<Kraje> kraje)
        {
            UniwersalneWywolanieAPI<List<Kraje>>("/Api/klienci.kraje.dodaj.ashx", kraje, null);
        }
        public  void UsunKraje(HashSet<int> dousunieca)
        {
            UniwersalneWywolanieAPI<HashSet<int>>("/Api/klienci.kraje.usun.ashx", dousunieca, null);
        }

        public  Dictionary<int, Region> PobierzRegiony()
        {
            return UniwersalneWywolanieAPI<Dictionary<int, Region>>("/Api/klienci.regiony.pobierz.ashx", null, null);
        }

        public  void UsunRegiony(HashSet<int> doUsuniecia)
        {
            UniwersalneWywolanieAPI<HashSet<int>>("/Api/klienci.regiony.usun.ashx", doUsuniecia, null);
        }

        public  void AktualizujRegiony(IList<Region> regiony)
        {
            UniwersalneWywolanieAPI<List<Region>>("/Api/klienci.regiony.dodaj.ashx", regiony, null);
        }



        public  void AktalizujListyPrzewozowe(IList<HistoriaDokumentuListPrzewozowy> listyDoAktualizacji)
        {
            UniwersalneWywolanieAPI<List<HistoriaDokumentuListPrzewozowy>>(
                "/api/Dokumenty.listyprzewozowe.aktualizuj.ashx", listyDoAktualizacji, null);
        }

   

        public  void UsunListyPrzewozowe(HashSet<int> elementyDoUsunieciaAdresy)
        {
            UniwersalneWywolanieAPI<HashSet<int>>(
                "/api/Dokumenty.listyprzewozowe.usun.ashx", elementyDoUsunieciaAdresy, null);
        }

        public  void AktalizujZamienniki(IList<ProduktyZamienniki> listyDoAktualizacji)
        {
            UniwersalneWywolanieAPI<IList<ProduktyZamienniki>>( "/Api/produkty.zamienniki.aktualizuj.ashx", listyDoAktualizacji, null);
        }
        public Dictionary<long, ProduktyZamienniki> PobierzZamienniki()
        {
            return UniwersalneWywolanieAPI<Dictionary<long, ProduktyZamienniki>>("/Api/produkty.zamienniki.pobierz.ashx", null, null);
        }

        public void UsunZamienniki(HashSet<long> dousuniecia)
        {
            UniwersalneWywolanieAPI<List<long>>("/Api/produkty.zamienniki.usun.ashx", dousuniecia, null);
        }
        public  List<StatusZamowienia> PobierzStatusyZamowien()
        {

            return UniwersalneWywolanieAPI<List<Model.StatusZamowienia>>("/Api/zamowienia.statusy.pobierz.ashx", null,
                                                                           null);
        }
        public  void AktualizujStatusyZamowien(IList<StatusZamowienia> data)
        {

            UniwersalneWywolanieAPI<List<Model.StatusZamowienia>>("/Api/zamowienia.statusy.aktualizuj.ashx", data, null);
        }

        public void UsunStatusyZamowien(HashSet<int> data)
        {
            //Nie wolno usuwać statusów Bartek 15-09-2016
            //UniwersalneWywolanieAPI<HashSet<int>>("/Api/zamowienia.statusy.usun.ashx", data, null);
        }

        public  List<ZamowienieSynchronizacja> PobierzZamowienia()
        {
            return UniwersalneWywolanieAPI<List<Model.ZamowienieSynchronizacja>>("/Api/zamowienia.pobierz.ashx", null,null);
        }
        public List<ZamowienieSynchronizacja> PobierzWszystkieZamowienia()
        {
            return UniwersalneWywolanieAPI<List<Model.ZamowienieSynchronizacja>>("/Api/zamowieniaWszystkie.pobierz.ashx", null,
                                                                                 null);
        }

        public  void AktualizujZamowienie(List<ZamowienieSynchronizacja> zapisaneZamowienia)
        {
            UniwersalneWywolanieAPI<List<Model.ZamowienieSynchronizacja>>("/Api/zamowienia.aktualizuj.ashx",zapisaneZamowienia, null);
        }

        public  void AktualizujPoziomyCen(IList<PoziomCenowy> poziomy_cenDoAktualizacji)
        {
            UniwersalneWywolanieAPI<Status>("/Api/produkty.poziomycen.dodaj.ashx", poziomy_cenDoAktualizacji, null);
        }
        public  void UsunPoziomyCen(HashSet<int> doUsuniecia)
        {                                  
            UniwersalneWywolanieAPI<Status>("/Api/produkty.poziomycen.usun.ashx", doUsuniecia, null);
        }

        public  Dictionary<string, ProduktPlik> PlikiProduktowPobierz()
        {
            return UniwersalneWywolanieAPI<Dictionary<string, ProduktPlik>>("/Api/pliki.produktow.pobierz.ashx", null, null);
        }

        public void PlikProduktowUsun(HashSet<long> DoUsuwania)
        {
            UniwersalneWywolanieAPIPaczkowane<long>("/Api/pliki.produktow.usun.ashx", DoUsuwania, 1000);
        }

        public  void PlikProduktowDodaj(List<ProduktPlik> DoDodania)
        {
            UniwersalneWywolanieAPIPaczkowane("/Api/pliki.produktow.dodaj.ashx", DoDodania, 5000);
        }
     
        public List<int> PlikNaB2BIdPobierz()
        {
            Log.DebugFormat("Pobieranie id zdjec");
            return UniwersalneWywolanieAPI<List<int>>("/Api/pliki.id.pobierz.ashx", null, null);
        }

        public List<Plik> PlikNaB2BPobierz()
        {
            var ids = PlikNaB2BIdPobierz();
            Log.DebugFormat(" id zdjec {0}",ids.Count);
            int max = 1000;
            int i = 0;
            List<Plik> produkty = new List<Plik>();
            while (ids.Count > 0)
            {
                Log.DebugFormat("Pobieranie paczki {0} pozostalo {1}",i,ids.Count);
                int dopobrania = ids.Count > max ? max : ids.Count;
                IEnumerable<int> listaidklientow = ids.Take(dopobrania);
                List<Plik> pobraneRabaty = UniwersalneWywolanieAPI<List<Plik>>("/Api/pliki.pobierz.ashx", listaidklientow, null);
                produkty.AddRange(pobraneRabaty);
                ids.RemoveRange(0, dopobrania);
                i++;
            }
            return produkty;
        }

        public  void PlikNaB2BUsun(List<int> ids)
        {
            UniwersalneWywolanieAPI<Status>("/Api/pliki.usun.ashx", ids, null);
        }

        public List<Plik> PlikiNaB2BDodajPaczkowanie(List<Plik> pliki, Action<string> infoBledy )
        {
            List<Plik> zwroconeDane = new List<Plik>(pliki.Count);
            
            while (pliki.Count > 0)
            {
                List<Plik> paczka = new List<Plik>();
                double rozmiarPaczki = 0;
                double rozmiarDoWyslania = 0;
                for (int i = 0; i < pliki.Count; ++i)
                {
                    try
                    {
                        Plik p = new Plik(pliki[i]);
                        double rozmiarRzeczywisty = 0;
                        try
                        {
                            if (p.RodzajPliku != RodzajPliku.Link)
                            {
                                //Path.Combine(pliki[i].Sciezka,pliki[i].nazwaLokalna)
                                //koszystamy tutaj z nazwy lokalnej ponieważ w nazwie mamy juz zakodowane znaki (spacje, polskie znaki) a my potrzebujemy dokładnej nazwy jakas jest lokalnie na komputerze
                                p.DanePlikBase64 = PlikiBase64.ResizePhoto(pliki[i],out rozmiarRzeczywisty);
                            }
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }

                        rozmiarPaczki += rozmiarRzeczywisty;

                        //max rozmiar paczki 7MB
                        if (rozmiarPaczki > 7 * 1024 * 1024)
                        {
                            Log.Info($"Rozmiar paczki: {rozmiarDoWyslania}, rozmiar paczki: {rozmiarPaczki}");
                            break;
                        }
                        rozmiarDoWyslania += rozmiarRzeczywisty;
                        paczka.Add(p);
                        pliki.RemoveAt(i);
                        --i;
                    }
                    catch (OutOfMemoryException)
                    {
                       //Log.Debug($"Brak pamięci do wysłania zdjęć. Wymuszone czyszczenie pamięci. Ustawienie GC server ma wartość = {GCSettings.IsServerGC}.");
                        GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                        GCSettings.LatencyMode = GCLatencyMode.Interactive;
                        GC.Collect();
                    }
                    catch (Exception ex)
                    {
                        infoBledy(string.Format("błąd przy przetwarzaniu pliku:'{0}', ścieżka: '{2}'. Błąd: {1}", pliki[i].SciezkaBezwzgledna, ex.Message, pliki[i].Sciezka));
                        pliki.RemoveAt(i);
                    }
                }

                Log.Info($"Paczka zdjęć - ilość plików: {paczka.Count}, pozostało: {pliki.Count}");
                Log.Debug($"Paczka plików: {string.Join(", ", paczka.Select(x => x.nazwaLokalna))}");

                try
                {
                    zwroconeDane.AddRange( this.PlikNaB2BDodaj(paczka) );
                    paczka.Clear();
                }
                catch (OutOfMemoryException)
                {
                    infoBledy("Brak pamięci do wysłania zdjęć.");

                    //czyszczenie pamieci
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GCSettings.LatencyMode = GCLatencyMode.Interactive;
                    GC.Collect();
                }
                catch (Exception e)
                {
                    infoBledy("Błąd: " + e.Message);
                }
            }
            return zwroconeDane;
        }
        
        public List<Plik> PlikNaB2BDodaj(List<Plik> pliki)
        {
            return UniwersalneWywolanieAPI<List<Plik>>("/Api/pliki.dodaj.ashx", pliki, null,null,null,3000000);
        }

        public  void PlatnosciUsun(List<int> lista)
        {
            if (lista.Count > 0)
            {
                UniwersalneWywolanieAPI<Status>("/Api/platnosci.usun.ashx", lista, null);
            }
        }
     
        public  string PobierzEksportowaneDane(int IdSzablonu, List<int> list)
        {
                return PrzygotujZadanie("/Api/synchronizacja.pobierz.ashx", IdSzablonu, null, list.Select(p => (object)p), null, 1000000);
        }
        
        public  List<Komunikat> WywolajImportEksport(int IdSzablonu, string dane)
        {
                return UniwersalneWywolanieAPI<List<Komunikat>>("/Api/synchronizacja.wykonaj.ashx", IdSzablonu, null, null, dane,2000000);

        }
        public  List<StatusDokumentuPDF> PobierzDokumentyDlaKtorychTrzebaDrukowacFaktureElektroniczna()
        {
            return UniwersalneWywolanieAPI<List<StatusDokumentuPDF>>("/api/Dokumenty.elektroniczne.dlakogo.ashx", null, null);
        }

        public  void AktualizujDokumentElektroniczne(List<StatusDokumentuPDF> dane)
        {
            UniwersalneWywolanieAPI<bool>("/api/Dokumenty.elektroniczne.aktualizuj.ashx", dane, null);
        }

        public void WysylaniePowiadomienONowychFakturach(HashSet<int> idKategoriKlientow)
        {
            UniwersalneWywolanieAPI<bool>("/api/Dokumenty.elektroniczne.wysylanie.ashx", idKategoriKlientow, null);
        }

        public  Dictionary<long, Jednostka> PobierzJednostki()
        {
            return UniwersalneWywolanieAPI<Dictionary<long, Jednostka>>("/api/produkty.jednostki.pobierz.ashx", null, null);
        }
         public  List<long> PobierzPobierzKonfekcjeId()
         {
             return UniwersalneWywolanieAPI<List<long>>("/Api/produkty.konfekcje.pobierzid.ashx", null, null);
         }

        public  List<Konfekcje> PobierzKonfekcje()
        {
            int max = 10000;
            List<long> listaidrabatow = PobierzPobierzKonfekcjeId();
            List<Konfekcje> konfekcje = new List<Konfekcje>();
            int i = 1;
            while (listaidrabatow.Count > 0)
            {
                Log.DebugFormat("Pobieranie konfekcji - paczka o rozmiarze {0} nr {1} pozostało {2} ",max,i,listaidrabatow.Count);
                int dopobrania = listaidrabatow.Count > max ? max : listaidrabatow.Count;
                HashSet<long> listaidklientow = new HashSet<long>( listaidrabatow.Take(dopobrania) );
                List<Konfekcje> pobraneRabaty = UniwersalneWywolanieAPI<List<Konfekcje>>("/api/produkty.konfekcje.pobierz.ashx", listaidklientow, null);
                konfekcje.AddRange(pobraneRabaty);
                listaidrabatow.RemoveRange(0, dopobrania);
                i++;
            }
            return konfekcje;
        }

        public  Dictionary<long, ProduktJednostka> PobierzProduktyJednostki()
        {
            return UniwersalneWywolanieAPI<Dictionary<long, ProduktJednostka>>("/api/produkty.jednostki.laczniki.pobierz.ashx", null, null);
        }

        public  void AktualizujJednostki(IList<Jednostka> list)
        {
            UniwersalneWywolanieAPI<IList<Jednostka>>("/api/produkty.jednostki.aktualizuj.ashx", list, null);
        }
        public  void AktualizujKonfekcje(List<Konfekcje> list)
        {
            UniwersalneWywolanieAPIPaczkowane<Konfekcje>("/api/produkty.konfekcje.aktualizuj.ashx",list, 5000);
        }
        public  void UsunJednostki(HashSet<long> jednostkiDoUsuniecia)
        {
            //mus byc paczkowane po jeden bo strasznie obicaza system usuwanie jednostek - dlatego ze jednostki sa w dokumentach
            UniwersalneWywolanieAPIPaczkowane<long>("/api/produkty.jednostki.usun.ashx", jednostkiDoUsuniecia, 1);
        }

        public  void AktualizujProduktyJednostkiJednostki(IList<ProduktJednostka> jednostkiLacznikiDoAktualizacji)
        {
            UniwersalneWywolanieAPI<IList<ProduktJednostka>>("/api/produkty.jednostki.laczniki.aktualizuj.ashx", jednostkiLacznikiDoAktualizacji, null);
        }

        public void UsunJednostkiLaczniki(HashSet<long> jednostkiLacznikiDoUsuniecia)
        {
            UniwersalneWywolanieAPIPaczkowane<long>("/api/produkty.jednostki.laczniki.usun.ashx",
            jednostkiLacznikiDoUsuniecia, 1000);
        }

        public void WyslijPowiadomieniaOTerminiePlatnosci(int ileDniPrzed, int ileDniPo, int? coIleDniPonowneWyslanie, List<int> kategoriaKlientaNieWysylaj, List<int> kategoriaKlientaWysylaj)
        {
            Dictionary<string, object> parametry = new Dictionary<string, object>();
            parametry.Add("ileDniPrzed", ileDniPrzed);
            parametry.Add("ileDniPo", ileDniPo);
            parametry.Add("coIleDniPonowneWyslanie", coIleDniPonowneWyslanie);
            parametry.Add("kategoriaKlientaNieWysylaj", kategoriaKlientaNieWysylaj);
            parametry.Add("kategoriaKlientaWysylaj", kategoriaKlientaWysylaj);
            UniwersalneWywolanieApiZParametrami<bool>("api2/maile/Niezaplacone/{ileDniPrzed}/{ileDniPo}/{coIleDniPonowneWyslanie}", Method.GET, parametry);
            //UniwersalneWywolanieAPI<Status>("/api/Dokumenty.powiadomienia.niezaplacone.ashx", parametry, null);
        }

        public  List<ProduktyKodyDodatkowe> PobierzKodyAlternatywne()
        {
            return UniwersalneWywolanieAPI<List<ProduktyKodyDodatkowe>>("/api/produkty.kody.pobierz.ashx", null, null);
        }

        public  void AktualizujKodyAlternatywne(List<ProduktyKodyDodatkowe> doAktualizacji)
        {
            UniwersalneWywolanieAPI<List<ProduktyKodyDodatkowe>>("/api/produkty.kody.aktualizuj.ashx", doAktualizacji, null);
        }

        public  void UsunAlternatywneKody(List<int> elementyDoUsuniecia)
        {
            UniwersalneWywolanieAPI<List<ProduktyKodyDodatkowe>>("/api/produkty.kody.usun.ashx", elementyDoUsuniecia, null);
        }

        public  Dictionary<int, long> PobierzHashDokumentow()
        {
            return UniwersalneWywolaneApi<Dictionary<int, long>>("api2/dokumenty/HashDokumentow", Method.GET, null);
        }

        public Dictionary<long, List<HistoriaDokumentuProdukt>> PobierzIdProduktowZDokumentowOStatusie(long[] klienci, int[] statusyDokumentow = null, bool tylkoAktualneOferty = false)
        {
            Dictionary<string, object> parametry = new Dictionary<string, object>();
            parametry.Add("klienci", klienci);
            parametry.Add("statusyDokumentow", statusyDokumentow);
            parametry.Add("tylkoAktualneOferty", tylkoAktualneOferty);
            return UniwersalneWywolanieApiZParametrami<Dictionary<long, List<HistoriaDokumentuProdukt>>>("api2/dokumenty/PobierzOfertyPozycje/{klienci}/{statusyDokumentow}/{tylkoAktualneOferty}", Method.GET, parametry);
        }


        public Dictionary<int, long> PobierzHashPozycjiDokumentow(HashSet<int> idDokumentow)
        {
            return UniwersalneWywolanieAPI<Dictionary<int, long>>("/api/HashDokumentyPozycje.pobierz.ashx", idDokumentow, null);
        }
     
        public  List<int> PobierzKlientowZRabatami()
        {
            return UniwersalneWywolanieAPI<List<int>>("/api/klienci.zrabatami.pobierz.ashx", null, null);
        }

        public  Dictionary<int, Sklep> PobierzSklepy()
        {
            return UniwersalneWywolanieAPI<Dictionary<int, Sklep>>("/Api/sklepy.pobierz.ashx", null, null);
        }

        public  void AktualizujSklepy(IList<Model.Sklep> sklepy)
        {
            //UniwersalneWywolanieAPI<List<Sklep>>("/Api/sklepy.aktualizuj.ashx", adresy, null);
            UniwersalneWywolanieAPIPaczkowane("/Api/sklepy.aktualizuj.ashx", sklepy, 50);
        }
        public  void UsunSklepy(HashSet<long> elementyDoUsunieciaAdresy)
        {
            UniwersalneWywolanieAPI<Status>("/Api/sklepy.usun.ashx", elementyDoUsunieciaAdresy, null);
        }

        public  Dictionary<long, KategoriaSklepu> PobierzSklepyKategorie()
        {
            return UniwersalneWywolanieAPI<Dictionary<long, KategoriaSklepu>>("/Api/sklepy.kategorie.pobierz.ashx", null, null);
        }

        public  void AktualizujSklepyKategorie(IList<KategoriaSklepu> adresy)
        {
            UniwersalneWywolanieAPI<List<KategoriaSklepu>>("/Api/sklepy.kategorie.aktualizuj.ashx", adresy, null);
        }
        public  void UsunSklepyKategorie(HashSet<long> elementyDoUsunieciaAdresy)
        {
            UniwersalneWywolanieAPI<Status>("/Api/sklepy.kategorie.usun.ashx", elementyDoUsunieciaAdresy, null);
        }

        public Dictionary<long, Waluta> PobierzWaluty()
        {
            return UniwersalneWywolanieAPI<Dictionary<long, Waluta>>("/Api/waluty.pobierz.ashx", null, null);
        }

        public void AktualizujWaluty(IList<Waluta> waluty)
        {
            UniwersalneWywolanieAPI<List<Waluta>>("/Api/waluty.aktualizuj.ashx", waluty, null);
        }
        public void UsunWaluty(HashSet<long> elementyDoUsunieciaWaluty)
        {
            UniwersalneWywolanieAPI<Status>("/Api/waluty.usun.ashx", elementyDoUsunieciaWaluty, null);
        }

        public  List< SklepKategoriaSklepu> PobierzSklepyKategorieLaczniki()
        {
            return UniwersalneWywolanieAPI<List<SklepKategoriaSklepu>>("/Api/sklepy.laczniki.pobierz.ashx", null, null);
        }

        public  void AktualizujSklepyKategorieLaczniki(IList<SklepKategoriaSklepu> adresy)
        {
            UniwersalneWywolanieAPI<Status>("/Api/sklepy.laczniki.aktualizuj.ashx", adresy, null);
        }
        public  void UsunSklepyKategorieLaczniki(HashSet<long> elementyDoUsunieciaAdresy)
        {
            UniwersalneWywolanieAPI<Status>("/Api/sklepy.laczniki.usun.ashx", elementyDoUsunieciaAdresy, null);
        }

        public HashSet<string> PobierzMagazynySymbole()
        {
            var mags = PobierzMagazyny().Where(x=>x.ImportowacZErp);
            HashSet<string> wynik=new HashSet<string>();
            foreach (var m in mags)
            {
                string[] symbole = m.Symbol.Split('+');
                foreach (var s in symbole)
                {
                    wynik.Add(s);
                }
            }
            return wynik;
        }

        public HashSet<DanePlik> PobierzPlikiSynchronizatora()
        {
            return UniwersalneWywolanieAPI<HashSet<DanePlik>>("/Api/synchronizator.info.pobierz.ashx", null, null,timeout:1200);
        }


        public void WylogujKlienta()
        {
            UniwersalneWywolanieApiZParametrami<bool>("api2/klienci/wyloguj/{id}", Method.POST, new Dictionary<string,object>() { {"id", SesjaGuid } });
        }

        public void WyslijMaileProduktyPrzyjeteNaMagazyn(List<long> idCechKoniecznych, List<long> idCechZabronionych,
            decimal minimalneZwiekszenieStanuPrzelicznik, decimal minimalnaIloscBrakuPrzelicznik, List<int> idMagazynow)
        {
            Dictionary<string, object> parametry =
                new Dictionary<string, object>
                {
                    {"idCechKoniecznych", idCechKoniecznych},
                    {"idCechZabronionych", idCechZabronionych},
                    {"minimalneZwiekszenieStanuPrzelicznik", minimalneZwiekszenieStanuPrzelicznik},
                    {"minimalnaIloscBrakuPrzelicznik", minimalnaIloscBrakuPrzelicznik},
                    {"idMagazynow", idMagazynow}
                };

            UniwersalneWywolanieApiZParametrami<bool>("api2/maile/MailProduktyPrzyjeteNaMagazyn", Method.GET, parametry, ParameterType.QueryString);
        }

    }

    public class APIException : Exception
    {
        public APIException(string msg) : base(msg) { }
        public APIException(string wiadomosc, string ladnyKomunikat)
            : base(wiadomosc)
        {
            PrzyjemnaTrescBledu = ladnyKomunikat;
        }
        public string PrzyjemnaTrescBledu { get; set; }
    }
    public class Status
    {
        public StatusApi Result { get; set; }
        public string Message {get;set;}
        public object Data { get; set; }
        public Status() { Result = StatusApi.Ok; Message = ""; }
        public Status(int id, string wiadomosc) { Result = (StatusApi)id; Message = wiadomosc; }
        public Status(StatusApi s, string wiadomosc) { Result = s; Message = wiadomosc; }
        public static Status Utworz(StatusApi a)
        {
            return Utworz(a, "");
        }
        public static Status Utworz(StatusApi a, string wiadomosc)
        {
            string msg = "";
            switch (a)
            {
                case StatusApi.BladOgolny:
                    msg = "Błąd ogólny. Skontaktuj się z dostawcą oprogramowania SOLEX jeśli widzisz ten błąd.";
                    break;
                case StatusApi.BladLogowania:
                    msg = "Błędne dane logowania. Sprawdz klucz API którym się posługujesz.";
                    break;
                case StatusApi.BrakUprawnien:
                    msg = "Brak uprawnień. Klucz API jest poprawny, ale nie masz dostępu do wybranych rzeczy.";
                    break;
                default:
                    msg = "";
                    break;
            }
            return new Status(a, msg + wiadomosc);
        }
    }
    public enum StatusApi
    {
        Ok = 0,
        BladOgolny = 1,
        BladLogowania = 3,
        BrakUprawnien=4
    }
}

