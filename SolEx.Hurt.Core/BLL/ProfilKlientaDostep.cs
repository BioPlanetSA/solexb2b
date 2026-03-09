using ServiceStack.Text;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ServiceStack.Common;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core.ModelBLL.ObiektyMaili;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Core.Helper;
using System.Collections.Concurrent;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace SolEx.Hurt.Core.BLL
{
    public class ProfilKlientaDostep : LogikaBiznesBaza, IProfilKlientaBll
    {
        public ProfilKlientaDostep(ISolexBllCalosc calosc)
            : base(calosc)
        {

        }

        //prywatne wartosci dla profilu klientów wartosci domyślnych
        Dictionary<AccesLevel, Dictionary<TypUstawieniaKlienta, ProfilKlienta[]> > profileDomyslne_DomyslneWartosci = null;

        /// <summary>
        /// odczytuje domyślną wartości z bazy dla danego typu ustawnienia
        /// domyśla wartość to taka która w id klienta ma null
        /// NIE WOLNO Z TEJ METODY KORZYSTAC poza klasa profileKlientowDostep - ona jest tylko do wewnetrznych zastosowan profilu klientów
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="typ">Typ ustawienia jakie chcemy pobrać</param>
        /// <param name="dodatkoweDane">dodatkowa informacja np czy klient zalogowany cvzy nie podczas pobierania domyślenj wartości</param>
        /// <param name="dopisek"></param>
        /// <returns></returns>
        public T PobierzWartoscDomyslna<T>(TypUstawieniaKlienta typ, string dodatkoweDane, AccesLevel dopisek = AccesLevel.Niezalogowani)
        {
#if DEBUG
            if (dodatkoweDane == "")
            {
                throw new Exception("dodatkoweDane maja byc NULL a nie EMpty '' ");
            }
#endif

            if (dodatkoweDane== "")
            {
                //bch - zjeban jest w systemie w wielu miejscach jest starym zjebane ze jest pusty string - zamiast null - pojebane!
                dodatkoweDane = null;
            }

            if(profileDomyslne_DomyslneWartosci == null)
            {
                LockHelper.PobierzInstancje.UruchomKodWLocku_BezUzywaniaCache("profile", () =>
                {
                    var profile = Calosc.DostepDane.Pobierz<ProfilKlienta>(null, x => x.KlientId == null);
                    profileDomyslne_DomyslneWartosci = profile.GroupBy(x=> x.Dopisek).ToDictionary(x=> (AccesLevel) Enum.Parse( typeof(AccesLevel), x.Key, true), x=> x.GroupBy(z=> z.TypUstawienia).ToDictionary(y=> y.Key, y=> y.ToArray()) );
                });
            }

          if(profileDomyslne_DomyslneWartosci.TryGetValue(dopisek, out var dict))
            {
                if(dict.TryGetValue(typ, out ProfilKlienta[] profile))
                {
                    ProfilKlienta result = profile.FirstOrDefault(x => x.Dodatkowe == dodatkoweDane);

                    if (result == null && !string.IsNullOrEmpty(dodatkoweDane))
                    {
                        result = profile.FirstOrDefault(x => x.Dodatkowe == null);
                    }

                    return result == null ? default(T) : JsonSerializer.DeserializeFromString<T>(result.Wartosc);
                }
            }

            return default(T);


            //var wynik = string.IsNullOrEmpty(dodatkoweDane)
            //    ? Calosc.DostepDane.PobierzPojedynczy<ProfilKlienta>(x => x.KlientId == null && x.TypUstawienia == typ && x.Dopisek == dopisek.ToString() && x.Dodatkowe == null, null)
            //    : Calosc.DostepDane.PobierzPojedynczy<ProfilKlienta>(x => x.KlientId == null && x.TypUstawienia == typ && x.Dopisek == dopisek.ToString() && (x.Dodatkowe != null && x.Dodatkowe == dodatkoweDane), null);
            //if (wynik == null && !string.IsNullOrEmpty(dodatkoweDane))
            //{
            //    wynik = Calosc.DostepDane.PobierzPojedynczy<ProfilKlienta>(x => x.KlientId == null && x.TypUstawienia == typ && x.Dopisek == dopisek.ToString() && x.Dodatkowe == null, null);
            //}
        }

        private string kluczCacheProfiluKlientaPerKlient(long klientID)
        {
            return $"profil_klient_{klientID}";
        }


        private void usunCacheProfiluKlienta(IKlient klient)
        {
            if (klient != null)
            {
                string klucz = this.kluczCacheProfiluKlientaPerKlient(klient.Id);
                Calosc.Cache.UsunObiekt(klucz);
            }
        }

        private ProfilKlienta _pobierzWartoscProfilu(long klientID, TypUstawieniaKlienta typ, string dodatkoweDane = null)
        {
#if DEBUG
            if (dodatkoweDane == "")
            {
                throw new Exception("dodatkoweDane maja byc NULL a nie EMpty '' ");
            }
#endif
            ProfilKlienta result = null;

            var profilKlienta = LockHelper.PobierzInstancje.PobierzDaneWLocku_zUcyciemCache(this.kluczCacheProfiluKlientaPerKlient(klientID), () =>
            {
                var profileDb = Calosc.DostepDane.Pobierz<ProfilKlienta>(null, x => x.KlientId == klientID);
                return profileDb.GroupBy(z => z.TypUstawienia).ToDictionary(y => y.Key, y => y.ToArray());
            }  );


            if (profilKlienta == null || !profilKlienta.Any())
            {
                return null;
            }

                if (profilKlienta.TryGetValue(typ, out ProfilKlienta[] profile))
                {
                    result = profile.FirstOrDefault(x => string.IsNullOrEmpty(dodatkoweDane) || x.Dodatkowe == dodatkoweDane);

                    if (result == null && !string.IsNullOrEmpty(dodatkoweDane))
                    {
                        result = profile.FirstOrDefault(x => x.Dodatkowe == null || x.Dodatkowe == "");
                    }
                }

            return result;
        }


        /// <summary>
        /// odczytuje wartośc dla określonego typu
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="klient"></param>
        /// <param name="typ"></param>
        /// <param name="dodatkoweDane"></param>
        /// <returns></returns>
        public T PobierzWartosc<T>(IKlient klient, TypUstawieniaKlienta typ, string dodatkoweDane = null)
        {
            if (klient != null && klient.Dostep == AccesLevel.Zalogowani)
            {
               // var profile = Calosc.DostepDane.PobierzPojedynczy<ProfilKlienta>(x => x.TypUstawienia == typ && x.KlientId == klient.Id && (dodatkoweDane==null || (x.Dodatkowe != null && x.Dodatkowe==dodatkoweDane)), klient);
                var profile = this._pobierzWartoscProfilu(klient.Id, typ, dodatkoweDane);
                return profile == null ? PobierzWartoscDomyslna<T>(typ, dodatkoweDane, AccesLevel.Zalogowani) : JsonSerializer.DeserializeFromString<T>(profile.Wartosc);
            }

            var wartosc = CookieHelper.PobierzInstancje.GetCookieValue(CookieName + typ);
            if (wartosc == null)
            {
                return PobierzWartoscDomyslna<T>(typ, dodatkoweDane);
            }
            else
            {
                try
                {
                    return JsonSerializer.DeserializeFromString<T>(wartosc);
                }catch(Exception)
                {
                    //jakas zla wartosc jest wpisana?
                    return PobierzWartoscDomyslna<T>(typ, dodatkoweDane);
                }
            }
        }

        /// <summary>
        /// Odczytuje wartość dlaokreślonego typu dla wszystkich klientów którzy maja wartość podaną w parametrze, id=0 oznacza wartoscDomyslna
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="wartosc"></param>
        /// <param name="typ"></param>
        /// <param name="wartoscDomyslna">zwracamy wartosc domyslna dla ustawienia</param>
        /// <param name="dodatkoweDane"></param>
        /// <param name="dopisek"></param>
        /// <returns></returns>
        public Dictionary<long, T> PobierzKlientowZWartosciaUstawienia<T>(TypUstawieniaKlienta typ, string dodatkoweDane = null, AccesLevel dopisek = AccesLevel.Niezalogowani)
        {
            Type typUStawienia = typeof(T);
            return Calosc.DostepDane.Pobierz<ProfilKlienta>(null, x => x.KlientId!=0 &&   x.TypUstawienia == typ && (dodatkoweDane == null || (x.Dodatkowe != null && x.Dodatkowe == dodatkoweDane))
                                                                       && x.Dopisek == dopisek.ToString()).ToDictionary(x => x.KlientId??0, x => (T)Convert.ChangeType(typUStawienia == typeof(bool) ? x.Wartosc.Replace("\"","") : x.Wartosc, typUStawienia));
        }

        
        /// <summary>
        /// Usuwa wartośc dla określonego typu
        /// </summary>
        /// <param name="klient"></param>
        /// <param name="typ"></param>
        /// <param name="dodatkoweDane"></param>
        /// <returns></returns>
        public void UsunWartosc(IKlient klient, TypUstawieniaKlienta typ, string dodatkoweDane = null)
        {
            if (klient == null || klient.Dostep != AccesLevel.Zalogowani)
            {
               CookieHelper.PobierzInstancje.DeleteCookie(CookieName + typ + dodatkoweDane);
            };

            if (string.IsNullOrEmpty(dodatkoweDane)) {
                Calosc.DostepDane.UsunWybrane<ProfilKlienta, long>(x => x.TypUstawienia == typ && x.KlientId == klient.Id);
            }
            else
            {
                Calosc.DostepDane.UsunWybrane<ProfilKlienta, long>(x => x.TypUstawienia == typ && x.KlientId == klient.Id && x.Dodatkowe == dodatkoweDane);
            }

            usunCacheProfiluKlienta(klient);

           //var profile = Calosc.DostepDane.PobierzPojedynczy<ProfilKlienta>(x => x.TypUstawienia == typ && x.KlientId == klient.Id && (dodatkoweDane==null ||
           //                                                                                                                             (x.Dodatkowe != null && x.Dodatkowe==dodatkoweDane)), klient);
           // if (profile != null) //jeśli nie ma wartości w profilu to pobieram wartość domyślną
           // {
           //     Calosc.DostepDane.UsunPojedynczy<ProfilKlienta>(profile.Id);
           // }
        }

        private const string CookieName = "profil_klienta_";

        /// <summary>
        /// dodaje/aktualizuje wartość w profilu
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="klient"></param>
        /// <param name="typ"></param>
        /// <param name="wartosc"></param>
        /// <param name="dodatkoweDane"></param>
        public void DodajWartosc<T>(IKlient klient, TypUstawieniaKlienta typ, T wartosc, string dodatkoweDane = null)
        {
            AccesLevel dostep = AccesLevel.Niezalogowani;
            if (klient != null)
            {
                dostep = klient.Dostep;
            }
            T domyslna = PobierzWartoscDomyslna<T>(typ, dodatkoweDane, dostep);
            string wartoscJson = JsonSerializer.SerializeToString(wartosc);
            string domyslnaJson = JsonSerializer.SerializeToString(domyslna);

            if (klient != null && klient.Dostep == AccesLevel.Zalogowani)
            {
                ProfilKlienta profil = new ProfilKlienta();
                profil.Wartosc = wartoscJson;
                profil.TypUstawienia = typ;
                profil.KlientId = klient.Id;
                profil.Dodatkowe = dodatkoweDane;

                if (wartoscJson == domyslnaJson)
                {
                    Calosc.DostepDane.UsunPojedynczy<ProfilKlienta>(profil.Id);
                    usunCacheProfiluKlienta(klient);
                    return;
                }

                LockHelper.PobierzInstancje.UruchomKodWLocku_BezUzywaniaCache($"{profil.KlientId}_{profil.TypUstawienia}_{profil.Dodatkowe}", () =>
                {
                    Calosc.DostepDane.AktualizujPojedynczy(profil);
                });

                usunCacheProfiluKlienta(klient);
            }
            else
            {
                CookieHelper.PobierzInstancje.DeleteCookie(CookieName + typ + dodatkoweDane);
                CookieHelper.PobierzInstancje.SetCookie(CookieName + typ + dodatkoweDane, wartoscJson);
            }
        }
      
        /// <summary>
        /// pobiera profil klienta z bazy
        /// czyli pobiera wszystkie ustawienia klienta
        /// </summary>
        /// <param name="klient"></param>
        /// <returns></returns>
        public IList<ProfilKlienta> PobierzProfilKlienta(IKlient klient)
        {
            IList<ProfilKlienta> wynikProfil = Calosc.DostepDane.Pobierz<ProfilKlienta>(klient);
            return wynikProfil;
        }

        /// <summary>
        /// Pobiera stałe filtry dla klienta
        /// </summary>
        /// <param name="aktualnyKlient">Klient dla którego chcemu pobrać stałe filtry</param>
        /// <returns></returns>
        public Dictionary<int, HashSet<long>> PobierzStaleFiltry(IKlient aktualnyKlient)
        {
            Dictionary<int, HashSet<long>> wynik;
            wynik = PobierzWartosc<Dictionary<int, HashSet<long>>>(aktualnyKlient, TypUstawieniaKlienta.StalyFiltr) ?? new Dictionary<int, HashSet<long>>();
            return wynik;
        }

        /// <summary>
        /// Pobiera stałe filtry dla klienta jako string 
        /// </summary>
        /// <param name="aktualnyKlient">Klient dla którego chcemu pobrać stałe filtry</param>
        /// <returns></returns>
        public string PobierzStaleFiltryString(IKlient aktualnyKlient)
        {
            string kluczCache = "staleFitryString";
            //sprawdzamy czy jest już w cachy chwilowym  gotowy wynik 
            string wynik = Calosc.Cache.PobierzChwilowy<string>(kluczCache);
            if (wynik != null)
            {
                return wynik;
            }

            //pobieramy aktualne stałe filtry
            Dictionary<int, HashSet<long>> staleFiltry = PobierzStaleFiltry(aktualnyKlient);
           
            wynik = staleFiltry != null ? string.Join(",", staleFiltry.Values.SelectMany(x => x).Distinct().OrderBy(x => x)) : "";

            //dodajemy wynik do cache chwilowego choć nie wiem po co? ale tak kazali mi zrobić w tasku #9647
            Calosc.Cache.DodajChwilowy(kluczCache,wynik);
            return wynik;
        }

        public bool CzyWStalychFiltrachSaUlubioneWybrane(IKlient aktualnyKlient)
        {
            string kluczCache = "CzyWStalychFiltrachSaUlubioneWybrane";
            bool? wynik = Calosc.Cache.PobierzChwilowy<bool?>(kluczCache);
            if (wynik != null)
            {
                return wynik.Value;
            }

            //zakladamy ze cecha jest - jak nie ma to powinien byc blad
            var stale = PobierzStaleFiltry(aktualnyKlient);
            wynik = stale.ContainsKey(Calosc.Konfiguracja.CechaUlubione.AtrybutId.Value);
            Calosc.Cache.DodajChwilowy(kluczCache, wynik);
            return wynik.Value;
        }

        /// <summary>
        /// Dodaje stały filtr do profilu
        /// </summary>
        /// <param name="aktualnyKlient">Klient do którego dodajemy filtr</param>
        /// <param name="filtry">wartości fitru którą dodajemy</param>
        /// <param name="zamien">Czy dodajemy cechę czy zamieniamy</param>
        public void DodajStatyFiltr(IKlient aktualnyKlient, HashSet<long> filtry,bool zamien = false)
        {
            Dictionary<int, HashSet<long>> obecne = PobierzStaleFiltry(aktualnyKlient);
            foreach (var cecha in filtry)
            {
                int? atrybutId = Calosc.CechyAtrybuty.PobierzWszystkieCechy(aktualnyKlient.JezykId)[cecha].AtrybutId;
                if (atrybutId == null) continue;
                int atrybut = atrybutId.Value;
                if (obecne.ContainsKey(atrybut))
                {
                    if (zamien)
                    {
                        obecne[atrybut] = new HashSet<long> {cecha};
                    }
                    else
                    {
                        obecne[atrybut].Add(cecha);
                    }
                }
                else
                {
                    obecne.Add(atrybut,new HashSet<long> {cecha});
                }
            }
            if (obecne.Any())
            {
                DodajWartosc(aktualnyKlient, TypUstawieniaKlienta.StalyFiltr, obecne);
            }
        }
        
        /// <summary>
        /// Usuwa stały filtr w profilu
        /// </summary>
        /// <param name="aktualnyKlient">Klient do którego dodajemy filtr</param>
        /// <param name="filtry">wartości fitru którą usuwamy</param>
        public void UsunStalyFiltr(IKlient aktualnyKlient, HashSet<long> filtry)
        {
            Dictionary<int, HashSet<long>> obecne = PobierzStaleFiltry(aktualnyKlient);
            foreach (var cecha in filtry)
            {
                var atrybutId = Calosc.CechyAtrybuty.PobierzWszystkieCechy(aktualnyKlient.JezykId)[cecha].AtrybutId;
                if (atrybutId == null) continue;
                int atrybut = atrybutId.Value;
                if (!obecne.ContainsKey(atrybut))
                {
                    continue;
                }
                obecne[atrybut].Remove(cecha);

                if (!obecne[atrybut].Any())
                {
                    obecne.Remove(atrybut);
                }
            }
            if (obecne.Any())
            {
                DodajWartosc(aktualnyKlient, TypUstawieniaKlienta.StalyFiltr, obecne);
            }
            else
            {
                //jesli nie ma żadnych wartości to usuwamy ustawienie 
                UsunWartosc(aktualnyKlient, TypUstawieniaKlienta.StalyFiltr);
            }
        }
        
        /// <summary>
        /// Pobiera sortowanie z profilu
        /// </summary>
        /// <param name="klient">Klient dla którego pobieramy sortowanie</param>
        /// <param name="typ">Typ ustawienia</param>
        /// <param name="dodatkoweDane">dodatkowe dane</param>
        /// <returns>Obiekt Sortowanie</returns>
        public Sortowanie PobierzSortowanie(IKlient klient, TypUstawieniaKlienta typ, string dodatkoweDane = null)
        {
            string klucz = $"prof_{klient.Id}_{typ}_{dodatkoweDane}";
            Sortowanie wynik = null;
            wynik = Calosc.Cache.PobierzChwilowy<Sortowanie>(klucz);

            if (wynik == null)
            {
                string wartosc = PobierzWartosc<string>(klient, typ, dodatkoweDane);
                if (string.IsNullOrEmpty(wartosc))
                {
                    return null;
                }
                wynik = Calosc.Konfiguracja.PrzygotujSortowanie(wartosc).First();
                Calosc.Cache.DodajChwilowy(klucz, wynik);
            }
            return wynik;
        }

        public void CzyszczenieUstawienWProfilu()
        {
            //pobieramy z bazy - tam gdzie enuma juz nie ma bedzie 0 wpisane
            List<long> ids = Calosc.DostepDane.Pobierz<ProfilKlienta>(null, x => x.TypUstawienia == 0).Select(x => x.Id).ToList();
            Calosc.DostepDane.Usun<ProfilKlienta, long>(ids);
        }

        /// <summary>
        /// Binding po selekcie sprawdzający i poprawiający pobrane profile.
        /// </summary>
        /// <param name="jezykId"></param>
        /// <param name="zadajacyKlient"></param>
        /// <param name="ListaProfili"></param>
        /// <param name="parametryDoMetodyPoSelect"></param>
        /// <returns></returns>
        public IList<ProfilKlienta> SprawdzIPopraw(int jezykId, IKlient zadajacyKlient, IList<ProfilKlienta> listaProfili, object parametryDoMetodyPoSelect)
        {
            var doAktualizacji = SprawdzProfile(listaProfili);

            if (doAktualizacji.Any())
            {
                AktualizujProfile(doAktualizacji);
            }
            return listaProfili;
        }

        /// <summary>
        /// Sprawdzam Liste profili
        /// </summary>
        /// <param name="listaProfili"></param>
        /// <returns>Lista profili która uległa zmianie</returns>
        public List<ProfilKlienta> SprawdzProfile(IList<ProfilKlienta> listaProfili)
        {
            List<ProfilKlienta> doAktualizacji = new List<ProfilKlienta>();
            foreach (ProfilKlienta profilKlienta in listaProfili)
            {
                switch (profilKlienta.TypUstawienia)
                {
                    case TypUstawieniaKlienta.StalyFiltr:
                        if (SprawdzStalyFiltr(profilKlienta))
                        {
                            doAktualizacji.Add(profilKlienta);
                        }
                        break;
                }
            }
            return doAktualizacji;
        }

        private HashSet<int> _dostepneIdAtrybuty;
        private HashSet<long> _dostepneIdCechy;

        /// <summary>
        /// Sprawdza czy wartość profilu zawiera aktualne id cech i atrybutów
        /// </summary>
        /// <param name="profil"></param>
        /// <returns>Czy profil byl poprawiona.</returns>
        public bool SprawdzStalyFiltr(ProfilKlienta profil)
        {
            bool zmiany = false;
            if (_dostepneIdAtrybuty == null)
            {
                _dostepneIdAtrybuty = new HashSet<int>( Calosc.CechyAtrybuty.SlownikIdAtrybutowIIdCech.Keys );
            }
            if (_dostepneIdCechy == null)
            {
                _dostepneIdCechy = new HashSet<long>( Calosc.CechyAtrybuty.SlownikIdAtrybutowIIdCech.SelectMany(x => x.Value) );
            }

            Dictionary<int, HashSet<long>> wartosc = null;

            try{
                    wartosc = JsonSerializer.DeserializeFromString<Dictionary<int, HashSet<long>>>(profil.Wartosc);
            }catch(Exception e)
            {
                Calosc.Log.Error($"Wartość stalych filtrów ([{profil.Wartosc}]) nie jest poprawna - błąd: {e.Message}. Kasowanie filtrów dla klienta id: {profil.KlientId}.");
                profil.Wartosc = null;
                return true;
            }

            if (wartosc == null)
            {
                return false;
            }

            for (int i=0;i<wartosc.Count;i++)
            {
                var klucz = wartosc.Keys.ToList()[i];
                if (_dostepneIdAtrybuty.Contains(klucz))
                {
                    var value = wartosc[klucz];
                    HashSet<long> cechy = new HashSet<long>( value.Intersect(_dostepneIdCechy) );
                    wartosc[klucz] = cechy;
                    if (cechy.Count != value.Count)
                    {
                        zmiany = true;
                    }
                }
                else
                {
                    wartosc.Remove(klucz);
                    zmiany = true;
                    i--;
                }
            }
            profil.Wartosc = JsonSerializer.SerializeToString(wartosc);

            return zmiany;
        }

        /// <summary>
        /// Aktualizuje listę profili
        /// </summary>
        /// <param name="profile"></param>
        private void AktualizujProfile(IList<ProfilKlienta> profile)
        {
            foreach (ProfilKlienta profilKlienta in profile)
            {
                AktualizujProfil(profilKlienta);
            }
        }

        /// <summary>
        /// Aktualizuje profil klienta bezpośrednio w bazie, omija bindingi
        /// </summary>
        /// <param name="profil"></param>
        private void AktualizujProfil(ProfilKlienta profil)
        {
            string dodatkowe = string.IsNullOrEmpty(profil.Dodatkowe) ? "" : $",Dodatkowe = '{profil.Dodatkowe}'";
            string dopisek = string.IsNullOrEmpty(profil.Dodatkowe) ? "" : $",Dopisek = '{profil.Dopisek}'";
            string sql =$"UPDATE ProfilKlienta SET  Wartosc='{profil.Wartosc}' {dodatkowe} {dopisek} WHERE Id = {profil.Id} AND KlientId = {profil.KlientId} AND TypUstawienia LIKE '{profil.TypUstawienia}'";
            try
            {
                Calosc.DostepDane.DbORM.ExecuteSql(sql);
            }
            catch (Exception ex)
            {
                throw new Exception($"Bład przy sprawdzaniu i poprawie profilu klienta: {profil.KlientId}, ustawienie: {profil.TypUstawienia}\r\n{ex.Message}");
            }
        }

        public void InicjalizujPowiadomieniaMailowe()
        {
            List<SzablonMailaBaza> powiadomienia = Calosc.MaileBLL.PobierzListeWszystkichPowiadomienMailowych();
            IList<UstawieniePowiadomienia> listaZdarzenWidocznych = Calosc.DostepDane.Pobierz<UstawieniePowiadomienia>(null, x => x.ZgodaNaZmianyPrzezKlienta).Where(x => x.ParametryWysylania != null && x.ParametryWysylania.Any(y => y.Aktywny)).ToList();
            foreach (var item in listaZdarzenWidocznych)
            {
                var powiadomienie = powiadomienia.First(x => x.Id == item.Id);
                bool? powiadomieniaMailowe = PobierzWartoscDomyslna<bool?>(TypUstawieniaKlienta.PowiadomieniaMailowe, powiadomienie.GetType().Name, AccesLevel.Zalogowani);
                if (!powiadomieniaMailowe.HasValue)
                {
                    ProfilKlienta pk = new ProfilKlienta(TypUstawieniaKlienta.PowiadomieniaMailowe, true, powiadomienie.GetType().Name, AccesLevel.Zalogowani);
                    Calosc.DostepDane.AktualizujPojedynczy(pk);
                }
            }
        }

        /// <summary>
        /// Ustawiamy domyślne wartości dla uswawień w profilu klient
        /// </summary>
        public void InicjalizujDomyslneWartosci()
        {
            #region Szablon listy
            string szablonListyNiezalogowani = PobierzWartoscDomyslna<string>(TypUstawieniaKlienta.SzablonListy, null);
            string widoknz = Calosc.Konfiguracja.AktywneWidokiListyProduktow(false).First();
            if (string.IsNullOrEmpty(szablonListyNiezalogowani) )
            {
                ProfilKlienta pk = new ProfilKlienta(TypUstawieniaKlienta.SzablonListy, widoknz, null, AccesLevel.Niezalogowani);
                Calosc.DostepDane.AktualizujPojedynczy(pk);
            }
            string szablonListyZalogowani = PobierzWartoscDomyslna<string>(TypUstawieniaKlienta.SzablonListy, null, AccesLevel.Zalogowani);
            string widokza = Calosc.Konfiguracja.AktywneWidokiListyProduktow(true).First();
            if (string.IsNullOrEmpty(szablonListyZalogowani) )
            {
                ProfilKlienta pk = new ProfilKlienta(TypUstawieniaKlienta.SzablonListy, widokza, null, AccesLevel.Zalogowani);
                Calosc.DostepDane.AktualizujPojedynczy(pk);
            }
            #endregion

            #region Stałe filtry

            ProfilKlienta stalyFiltr = Calosc.DostepDane.PobierzPojedynczy<ProfilKlienta>(x => x.KlientId == null && x.TypUstawienia == TypUstawieniaKlienta.StalyFiltr,null);
            if (stalyFiltr== null)
            {
                ProfilKlienta pk = new ProfilKlienta(TypUstawieniaKlienta.StalyFiltr, new Dictionary<int, HashSet<int>> (), null, AccesLevel.Zalogowani);
                Calosc.DostepDane.AktualizujPojedynczy(pk);
            }

            #endregion Stałe filtry

            #region Rozmiar Strony Lista Produktów

            string rozmiarDomyslny = Calosc.Konfiguracja.IleProduktowPokazacNaStronie.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).First();
            int rozmiarStronyNiezalogowani = PobierzWartoscDomyslna<int>(TypUstawieniaKlienta.RozmiarStronyListaProduktow, null);
            if (rozmiarStronyNiezalogowani == 0)
            {
                ProfilKlienta pk = new ProfilKlienta(TypUstawieniaKlienta.RozmiarStronyListaProduktow, rozmiarDomyslny, null, AccesLevel.Niezalogowani);
                Calosc.DostepDane.AktualizujPojedynczy(pk);
            }
            int rozmiarStronyZalogowani = PobierzWartoscDomyslna<int>(TypUstawieniaKlienta.RozmiarStronyListaProduktow, null, AccesLevel.Zalogowani);
            if (rozmiarStronyZalogowani == 0)
            {
                ProfilKlienta pk = new ProfilKlienta(TypUstawieniaKlienta.RozmiarStronyListaProduktow, rozmiarDomyslny, null, AccesLevel.Zalogowani);
                Calosc.DostepDane.AktualizujPojedynczy(pk);
            }

            #endregion Rozmiar Strony Lista Produktów

            #region Rozmiar Strony Lista w adminie

            int rozmiarStronyAdminZalogowani = PobierzWartoscDomyslna<int>(TypUstawieniaKlienta.ListaKolumnWAdminieRozmiarStrony, null, AccesLevel.Zalogowani);
            if (rozmiarStronyAdminZalogowani == 0)
            {
                ProfilKlienta pk = new ProfilKlienta(TypUstawieniaKlienta.ListaKolumnWAdminieRozmiarStrony, rozmiarDomyslny, null, AccesLevel.Zalogowani);
                Calosc.DostepDane.AktualizujPojedynczy(pk);
            }

            #endregion Rozmiar Strony Lista w adminie

            bool pokazFiltryNiezalogowani = PobierzWartoscDomyslna<bool>(TypUstawieniaKlienta.WidocznoscFiltrow, null);
            if (!pokazFiltryNiezalogowani)
            {
                ProfilKlienta pk = new ProfilKlienta(TypUstawieniaKlienta.WidocznoscFiltrow, "true", null, AccesLevel.Niezalogowani);
                Calosc.DostepDane.AktualizujPojedynczy(pk);
            }
            bool pokazFiltryZalogowani = PobierzWartoscDomyslna<bool>(TypUstawieniaKlienta.WidocznoscFiltrow, null ,AccesLevel.Zalogowani);
            if (!pokazFiltryZalogowani)
            {
                ProfilKlienta pk = new ProfilKlienta(TypUstawieniaKlienta.WidocznoscFiltrow, "true", null, AccesLevel.Zalogowani);
                Calosc.DostepDane.AktualizujPojedynczy(pk);
            }

            #region Sortowanie dla listy dokumentów

            Sortowanie sortDok = new Sortowanie();
            sortDok.Opis = "DataWystawienia";
            sortDok.Pola.Add(new SortowaniePole("DataUtworzenia", KolejnoscSortowania.desc));
            string wartoscDok = sortDok.ToString();

            string kolumnaSortowaniaZalogowani = PobierzWartoscDomyslna<string>(TypUstawieniaKlienta.KolumnaSortowaniaDokumentow, null, AccesLevel.Zalogowani);
            if (string.IsNullOrEmpty(kolumnaSortowaniaZalogowani))
            {
                ProfilKlienta pk = new ProfilKlienta(TypUstawieniaKlienta.KolumnaSortowaniaDokumentow, wartoscDok, null, AccesLevel.Zalogowani);
                Calosc.DostepDane.AktualizujPojedynczy(pk);
            }
            string kolumnaSortowaniaNieZalogowani = PobierzWartoscDomyslna<string>(TypUstawieniaKlienta.KolumnaSortowaniaDokumentow, null);
            if (string.IsNullOrEmpty(kolumnaSortowaniaNieZalogowani))
            {
                ProfilKlienta pk = new ProfilKlienta(TypUstawieniaKlienta.KolumnaSortowaniaDokumentow, wartoscDok, null, AccesLevel.Niezalogowani);
                Calosc.DostepDane.AktualizujPojedynczy(pk);
            }


            #endregion Sortowanie dla listy dokumentów

            #region Sortowanie dla listy Produktów

            Sortowanie sort = Calosc.Konfiguracja.DostepneSortowanieListyProduktow.First();
            string wartosc = sort.ToString();
            string kolumnaSortowaniaProdZalogowani = PobierzWartoscDomyslna<string>(TypUstawieniaKlienta.KolumnaSortowaniaListyProduktow, null, AccesLevel.Zalogowani);
            if (string.IsNullOrEmpty(kolumnaSortowaniaProdZalogowani))
            {
                ProfilKlienta pk = new ProfilKlienta(TypUstawieniaKlienta.KolumnaSortowaniaListyProduktow, wartosc, null, AccesLevel.Zalogowani);
                Calosc.DostepDane.AktualizujPojedynczy(pk);
            }
            string kolumnaSortowaniaProdNieZalogowani = PobierzWartoscDomyslna<string>(TypUstawieniaKlienta.KolumnaSortowaniaListyProduktow, null);
            if (string.IsNullOrEmpty(kolumnaSortowaniaProdNieZalogowani))
            {
                ProfilKlienta pk = new ProfilKlienta(TypUstawieniaKlienta.KolumnaSortowaniaListyProduktow, wartosc, null, AccesLevel.Niezalogowani);
                Calosc.DostepDane.AktualizujPojedynczy(pk);
            }

            #endregion Sortowanie dla listy Produktów

            #region Sortowanie dla koszyka

            sort = Calosc.Konfiguracja.DostepneSortowanieKoszyka.First();
            wartosc = sort.ToString();
            kolumnaSortowaniaProdZalogowani = PobierzWartoscDomyslna<string>(TypUstawieniaKlienta.KolumnaSortowaniaKoszykLista, null, AccesLevel.Zalogowani);
            if (string.IsNullOrEmpty(kolumnaSortowaniaProdZalogowani))
            {
                ProfilKlienta pk = new ProfilKlienta(TypUstawieniaKlienta.KolumnaSortowaniaKoszykLista, wartosc, null, AccesLevel.Zalogowani);
                Calosc.DostepDane.AktualizujPojedynczy(pk);
            }

            #endregion Sortowanie dla koszyka

            #region Dokumenty tylko niezapłacone
            //domyslnie i tak będzie na false więc po co ustawiać 
            //bool niezaplaconeZalog = PobierzWartoscDomyslna<bool>(TypUstawieniaKlienta.DokumentyTylkoNiezaplacone, null, AccesLevel.Zalogowani);
            //if (!niezaplaconeZalog)
            //{
            //    ProfilKlienta pk = new ProfilKlienta(TypUstawieniaKlienta.DokumentyTylkoNiezaplacone, false, null, AccesLevel.Zalogowani);
            //    Calosc.DostepDane.AktualizujPojedynczy(pk);
            //}

            #endregion Dokumenty tylko niezapłacone

            #region Dokumenty tylko niezrealizowane

            bool? niezrealizowaneZalog = PobierzWartoscDomyslna<bool?>(TypUstawieniaKlienta.DokumentyTylkoNiezrealizowane, null, AccesLevel.Zalogowani);
            if (niezrealizowaneZalog==null)
            {
                ProfilKlienta pk = new ProfilKlienta(TypUstawieniaKlienta.DokumentyTylkoNiezrealizowane, false, null, AccesLevel.Zalogowani);
                Calosc.DostepDane.AktualizujPojedynczy(pk);
            }

            #endregion Dokumenty tylko niezrealizowane

            #region Dokumenty tylko przeterminowane

            bool przeterminowaneZalog = PobierzWartoscDomyslna<bool>(TypUstawieniaKlienta.DokumentyTylkoPrzeterminowane, null, AccesLevel.Zalogowani);
            if (!przeterminowaneZalog)
            {
                ProfilKlienta pk = new ProfilKlienta(TypUstawieniaKlienta.DokumentyTylkoPrzeterminowane, false, null, AccesLevel.Zalogowani);
                Calosc.DostepDane.AktualizujPojedynczy(pk);
            }

            #endregion Dokumenty tylko przeterminowane

            #region Dotumenty Z Ilu Dni Domyslnie Pokazywac Dokumenty

            int domysnaLiczbaPokazywania = Calosc.Konfiguracja.ZIleDniDomyslniePokazywacDokumenty;
            int zIlu = PobierzWartoscDomyslna<int>(TypUstawieniaKlienta.DokumentyZIluDniDomyslniePokazywacFaktura, null, AccesLevel.Zalogowani);
            if (zIlu == 0)
            {
                ProfilKlienta pk = new ProfilKlienta(TypUstawieniaKlienta.DokumentyZIluDniDomyslniePokazywacFaktura, 90, null, AccesLevel.Zalogowani);
                Calosc.DostepDane.AktualizujPojedynczy(pk);
            }


            zIlu = PobierzWartoscDomyslna<int>(TypUstawieniaKlienta.DokumentyZIluDniDomyslniePokazywacZamowienie, null, AccesLevel.Zalogowani);
            if (zIlu == 0)
            {
                ProfilKlienta pk = new ProfilKlienta(TypUstawieniaKlienta.DokumentyZIluDniDomyslniePokazywacZamowienie, 45, null, AccesLevel.Zalogowani);
                Calosc.DostepDane.AktualizujPojedynczy(pk);
            }



            #endregion Dotumenty Z Ilu Dni Domyslnie Pokazywac Dokumenty

            #region Powiadomienia Mailowe

            InicjalizujPowiadomieniaMailowe();

            #endregion Powiadomienia Mailowe

            #region Ukrywaj cenę hurtową 
            bool ukrywajCeneHurtowaNiezalogowani = PobierzWartoscDomyslna<bool>(TypUstawieniaKlienta.UkryjCenyHurtowe, null);
            if (!ukrywajCeneHurtowaNiezalogowani)
            {
                ProfilKlienta pk = new ProfilKlienta(TypUstawieniaKlienta.UkryjCenyHurtowe, "true", null, AccesLevel.Niezalogowani);
                Calosc.DostepDane.AktualizujPojedynczy(pk);
            }

            #endregion

        }

    }
}