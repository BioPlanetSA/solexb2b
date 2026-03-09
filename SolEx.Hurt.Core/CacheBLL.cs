using System.Diagnostics;
using System.IO;
using log4net;
using log4net.Core;
using log4net.Layout.Pattern;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.Web;
using System.Web.Caching;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Extensions;
using SolEx.Hurt.Model.Interfaces;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using FastMember;
using ServiceStack.Common;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Core.DostepDane;

namespace SolEx.Hurt.Core
{
    public class CacheBll :  ICacheBll
    {
        private IConfigBLL _konfiguracja;

        public ISolexBllCalosc Calosc; 
        public CacheBll(ISolexBllCalosc calosc)
        {
            Calosc = calosc;
            _konfiguracja = calosc.Konfiguracja;

        }
        public T PobierzChwilowy<T>(string klucz)
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Items[klucz] != null)
                {
                    return (T) HttpContext.Current.Items[klucz];
                }
                return default(T);

            }
            throw new InvalidOperationException("To nie aplikacja http");
        }

        private int LosujCzasCache()
        {
            Random random = new Random();
            return random.Next(980, 2940);
        }

        public bool JestObiektChwilowy(string klucz)
        {
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Items.Contains(klucz);
            }
            return false;
        }

        public void DodajChwilowy(string klucz, object dane)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Items[klucz] = dane;
            }
        }


        public void UsunObiekt(string klucz, params object[] parametryKlucza)
        {
            if (HttpRuntime.Cache != null)
            {
                if (parametryKlucza.Length > 0)
                {
                    klucz = string.Format(klucz, parametryKlucza);
                }

                HttpRuntime.Cache.Remove(klucz);
            }
        }
        private ILog Log
        {
            get
            {
                return LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            }
        }

        /// <summary>
        /// metoda dodaje do cache, ale wydłuża czas trzymania w cache - stosować tylko dla krytycznych obiektów które powinny być długo trzymane w cache
        /// </summary>
        /// <param name="klucz"></param>
        /// <param name="obiekt"></param>
        /// <param name="parametryKlucza"></param>
        public void DodajObiekt_Kluczowy(string klucz, object obiekt, params object[] parametryKlucza)
        {
            DodajObiekt(LosujCzasCache() * 8, klucz, obiekt, parametryKlucza);
        }

        public void DodajObiekt(string klucz, object obiekt, params object[] parametryKlucza)
        {
            DodajObiekt(LosujCzasCache() * 5, klucz, obiekt, parametryKlucza);
        }


        /// <summary>
        /// Fajnie ze ta metoda ma opis czym się zajmuję. Chyba autor mimo tego że innym roni awantury sam zapomniał dodać opisu i testu do czegoś co sam robił
        /// 
        /// </summary>
        /// <param name="obiekt"></param>
        /// <param name="akcesor"></param>
        /// <param name="polaDoInternacji"></param>
        public void InternujStringiWObiekcie(object obiekt, TypeAccessor akcesor, Dictionary<string, PropertyInfo> polaDoInternacji)
        {
            //sprawdzanie czy w ogóle jest obiekt z którego chcemy pobrać wartość 
            if (obiekt == null)
            {
                throw new Exception($"Obiekt z którego chcemy pobierać wartość jest nulllem");
            }
           
            foreach (var pole in polaDoInternacji)
            {
                string aktualnaWartosc = akcesor[obiekt, pole.Key] as string;
                if (aktualnaWartosc != null)
                {
                    akcesor[obiekt, pole.Key] = string.Intern(aktualnaWartosc);
                }
            }
        }


        public void DodajObiekt(int ileSekund, string klucz, object obiekt, params object[] parametryKlucza)
        {
            if (obiekt == null)
            {
                throw new Exception("Próba dodania pustego elementu do cache");
            }

            if (parametryKlucza.Length > 0)
            {
                klucz = string.Format(klucz, parametryKlucza);
            }

            //jesli obiekt dziedziczy po Interfejsie InterString - to wykonujemy internacje stringow po proeprtisach ktore tego wymagaja
            if (obiekt.GetType().InheritsOrImplements(typeof(IStringIntern)))
            {
                TypeAccessor akcesor = obiekt.GetType().PobierzRefleksja();
                Dictionary<string, PropertyInfo> polaDoInternacji = obiekt.GetType().Properties(typeof(StringInternuj));

                if (polaDoInternacji.IsEmpty())
                {
                    throw new Exception($"Brak pól do internacji Stringów - typ: {obiekt.GetType().Name} posiada interfejs IStringIntern, więc musi mieć pola internowane.");
                }

                this.InternujStringiWObiekcie(obiekt, akcesor, polaDoInternacji);
            }


            HttpRuntime.Cache.Add(klucz, obiekt, null, DateTime.Now.AddSeconds(ileSekund), Cache.NoSlidingExpiration, CacheItemPriority.Normal, OnRemoveCallback);
        }

        private void OnRemoveCallback(string key, object value, CacheItemRemovedReason reason)
        {
            SolexBllCalosc.PobierzInstancje.Log.Debug($"Usuwanie a cache klucza: {key} z powodu: {reason}");
        }

        private string _katalogPlikowCache = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plikiCache");

        /// <summary>
        /// uuwa pliki cache stworzone dłużej niż 12 godziny temu
        /// </summary>
        public void PlikowyCache_wyczyscWszystkoStare(string[] typyDanychInicjalizacja)
        {
            try
            {
                foreach (string typ in typyDanychInicjalizacja)
                {
                    string sciezka = Path.Combine(_katalogPlikowCache, typ);
                    Directory.CreateDirectory(sciezka);

                    var files = Directory.GetFiles(sciezka);
                    foreach (string file in files)
                    {
                        var info = new FileInfo(file);
                        if (info.CreationTime < DateTime.Now.AddHours(-12))
                        {
                            info.Delete();
                        }
                    }
                }
            } catch (Exception e)
            {
                Log.Fatal( $"Bląd przy usuwaniu starych plików cache z ścieżki: {_katalogPlikowCache}. Błąd: {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// cache plikowy - typ danych musi byc zainicjalizowany wczesniej w globalasax w metodzie PlikowyCache_wyczyscWszystkoStare
        /// </summary>
        /// <param name="metodaDoPobieraniaWartosci"></param>
        /// <param name="lok"></param>
        /// <param name="jezykId"></param>
        /// <param name="klucz"></param>
        /// <param name="typDanych"></param>
        /// <param name="czasCache"></param>
        /// <returns></returns>
        public virtual string PlikowyCache_PobierzObiekt(Func<string> metodaDoPobieraniaWartosci, object lok, int jezykId, string klucz, string typDanych, int czasCache = 30*60)
        {
            //brak cache
            if (czasCache == 0)
            {
                return metodaDoPobieraniaWartosci();
            }
            try
            {
                string sciezkaPliku = null;
                try
                {
                    sciezkaPliku = System.IO.Path.Combine(_katalogPlikowCache, typDanych, klucz + ".cache.gz");
                } catch
                {
                    Log.ErrorFormat($"Ścieżka do pliku cache jest nieprawidłowo. Katalog: {_katalogPlikowCache}, typ danych: {typDanych}, klucz: {klucz}. Upewnij się że katalog został zainicjalizowany w global.asax");
                }

                //kijowanie - ale w kolejce jak czekamy dlugo to polaczenie lepiej zeby bylo zamkniete
                this.Calosc.DostepDane.ZamknijPolaczenieDoBazy();

                //lokujemy odrazu dlatego ze duzo operacji na dysku - trzeba sprawdzac daty plikow itp. - cache plikowy jest wolno z natury
                lock (lok)
                {
                    bool naNowoWyliczycDane = false;
                    //jak plik istnieje to spradzmay jego date stworzenia zeby sie upewnic ze cache jest jeszcze aktywny
                    if (System.IO.File.Exists(sciezkaPliku))
                    {
                        var fileInfo = new FileInfo(sciezkaPliku);
                        if (fileInfo.CreationTime < DateTime.Now.AddSeconds(-czasCache))
                        {
                            try
                            {
                                File.Delete(sciezkaPliku);
                            } catch
                            {
                                Log.ErrorFormat($"Nie udało się skasować pliku cache: {sciezkaPliku}");
                                throw;
                            }
                            naNowoWyliczycDane = true;
                        }
                    }
                    else
                    {
                        naNowoWyliczycDane = true;
                    }

                    string dane;
                    if (naNowoWyliczycDane)
                    {
                        dane = metodaDoPobieraniaWartosci();
                        try
                        {
                            using (FileStream outFile = File.Create(sciezkaPliku))
                            using (GZipStream compress = new GZipStream(outFile, CompressionLevel.Fastest))
                            using (StreamWriter writer = new StreamWriter(compress))
                            {
                                writer.Write(dane); 
                            }
                        } catch
                        {
                            Log.Error($"Nie udało się zapisać pliku cache na dysku w ścieżce: {sciezkaPliku}. Być może nazwa pliku za długa lub brak miejsca na dysku.");
                            throw;
                        }
                        return dane;
                    }

                    //odczytujemy z cache pliku
                    using (FileStream outFile = File.OpenRead(sciezkaPliku))
                    using (GZipStream compress = new GZipStream(outFile, CompressionMode.Decompress))
                    using (StreamReader reader = new StreamReader(compress))
                    {
                        dane = reader.ReadToEnd();
                        return dane;
                    }
                    
                }
            } catch (Exception e)
            {
                Log.ErrorFormat($"Błąd w dostępie do cache plikowego. {e.Message} {e.StackTrace}", e);
                throw;
            }
        }

        private ConcurrentDictionary< string, ConcurrentDictionary<int, ConcurrentDictionary<long, object> > > slownikPrywatnyCache = null;

        public void InicjalizujPrywatnySlownik( List<string> typyDlaKtorychInicjalizacja  )
        {
            slownikPrywatnyCache = new ConcurrentDictionary<string, ConcurrentDictionary<int, ConcurrentDictionary<long, object>>>();
            
                foreach (string typ in typyDlaKtorychInicjalizacja)
                {
                ConcurrentDictionary<int, ConcurrentDictionary< long, object>> slownik = new ConcurrentDictionary<int, ConcurrentDictionary<long, object>>();
                    foreach (int jezyk in this._konfiguracja.JezykiWSystemie.Keys)
                    {
                        slownik.TryAdd(jezyk, new ConcurrentDictionary<long, object>());
                    }

                    slownikPrywatnyCache.TryAdd(typ, slownik);
                }
        }

        /// <summary>
        /// Pobieranie danych z prywatnego slownika - slownik jest czyszczony tylko przy restarcie aplikacji (thread safe)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="metodaDoPobraniaWartosci">funkcja do pobierania danych</param>
        /// <param name="jezykId">id jezyka  - musi być podany, nie może być null</param>
        /// <param name="klucz">klucz obiektu</param>
        /// <param name="typDanych">typ danych przetrzymywanych - typ dancyh musi byc zainicjalizowany w global.asax dla slownika prywatnego przy starcie aplikacji</param>
        /// <returns></returns>
        public virtual T SlownikPrywatny_PobierzObiekt<T>(Func<long,object> metodaDoPobraniaWartosci, int jezykId, long klucz, string typDanych)
        {
            try
            {
                return (T) slownikPrywatnyCache[typDanych][jezykId].GetOrAdd(klucz, metodaDoPobraniaWartosci);
            } catch (Exception e)
            {
                Log.Debug(e);
                throw new Exception($"Problem z cache prywatnym - czy na pewno słownik jest zainicjalizowany dla typu danych: {typDanych} (InicjalizujPrywatnySlownik)?. Błąd: {e.Message}", e);
            }
        }

        /// <summary>
        /// Wersja thread safe - należy podać obiekt loka któy jest statyczny w ramach danych wątków
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="metodaDoPobraniaWartosci"></param>
        /// <param name="lok"></param>
        /// <param name="klucz"></param>
        /// <param name="parametryKlucza"></param>
        /// <returns></returns>
        public virtual T PobierzObiekt<T>(Func<T> metodaDoPobraniaWartosci, object lok, string klucz, params object[] parametryKlucza)
        {
            object pobranaWartosc = PobierzObiekt(klucz, parametryKlucza);
            if (pobranaWartosc != null)
            {
                return (T) pobranaWartosc;
            }

            lock (lok)
            {
                pobranaWartosc = PobierzObiekt(klucz, parametryKlucza);
                if (pobranaWartosc == null)
                {
                    pobranaWartosc = metodaDoPobraniaWartosci();
                    if (pobranaWartosc == null && typeof(T).IsNullableType())
                    {
                        throw new Exception("Cachowany obiekt nie może być nullem. Przerób kod tak żeby nie zwracał NULLa - albo wyłącz cache na obiekcie");
                    }
                    this.DodajObiekt(klucz, pobranaWartosc, parametryKlucza);
                }
            }
            return (T)pobranaWartosc;
        }
        
        [Obsolete("Należy korzystać z wersji thread safe")]
        public virtual T PobierzObiekt<T>(string klucz, params object[] parametryKlucza)
        {
            return (T)PobierzObiekt(klucz, parametryKlucza);
        }

        public object PobierzObiekt(string klucz, params object[] parametryKlucza)
        {
            if (string.IsNullOrEmpty(klucz))
            {
                throw new Exception("Nie można pobierać z cache kluczy NULL!");
            }
            
            if (parametryKlucza.Length > 0)
            {
                klucz = string.Format(klucz, parametryKlucza);
            }
            
            object dane = HttpRuntime.Cache.Get(klucz);
            return dane;
        }

        public void UsunGdzieKluczRozpoczynaSieOd(string poczatek)
        {
            var keysDoWywalenia = new List<string>();
            IDictionaryEnumerator enumerator = HttpRuntime.Cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string s = enumerator.Key as string;
                if (s != null && s.StartsWith(poczatek))
                {
                    keysDoWywalenia.Add(s);
                }
            }
            for (int i = 0; i < keysDoWywalenia.Count; i++)
            {
                UsunObiekt(keysDoWywalenia[i]);
            }

        }

        public bool ZawieraKluczRozpoczynaSieOd(string poczatek)
        {
            IDictionaryEnumerator enumerator = HttpRuntime.Cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string s = enumerator.Key as string;
                if (s != null && s.StartsWith(poczatek))
                {
                    return true;
                }
            }
            return false;
        }

        public void WyczyscCache()
        {
           UsunGdzieKluczRozpoczynaSieOd("");
        }

        /// <summary>
        /// Wyliczamy nazwe do cache dla kategorii
        /// </summary>
        /// <param name="idStaleFiltry">Stałe filtry zapisane jako string</param>
        /// <param name="szukane">Wyszukania</param>
        /// <param name="idKontrolki">Id kontrolki kategorii</param>
        /// <param name="klient">klient dla którego wyliczmy klucz</param>
        /// <returns></returns>
        public string WyliczKluczDlaKategorii(int idKontrolki, IKlient klient, string szukane)
        {
            if (Calosc.ProfilKlienta.CzyWStalychFiltrachSaUlubioneWybrane(klient))
            {
                return null;
            }

            string idStaleFiltry = Calosc.ProfilKlienta.PobierzStaleFiltryString(klient);
            string cachedKey = string.Format("Kategorie_{0}_{1}_{2}_{3}", idKontrolki, idStaleFiltry, szukane,klient.Id!=0);

            //jesli klient ma indywidualizowaną ofertę to tworzymy klucz z id klienta
            //jesli klien nie ma indywidualizowanej oferty to ma cache ogólny
            if (klient.OfertaIndywidualizowana)
            {
                cachedKey += "_" + klient.Id;
            }
            return cachedKey;
        }

        /// <summary>
        /// Wylicza nazwe do cache dla menu
        /// </summary>
        /// <param name="idKontrolki">Id kontrolki menu</param>
        /// <param name="klient">klient dla którego wyliczmy klucz</param>
        /// <param name="idJezyka">Id języka dla którego wyliczamy klucz</param>
        /// <param name="czyPerKlient">Czy klucz ma być z id klienta</param>
        /// <returns></returns>
        public string WyliczKluczDlaMenu(int idKontrolki, long idKlienta, int idJezyka, bool czyPerKlient)
        {
            if (idKlienta == 0)
            {
                return string.Format("Menu_N_{0}_{1}", idJezyka, idKontrolki);
            }

            if (czyPerKlient)
            {
                return string.Format("Menu_Z_{0}_{1}_{2}", idJezyka, idKontrolki, idKlienta);
            }

            return string.Format("Menu_Z_{0}_{1}", idJezyka, idKontrolki);
        }
    }
}
