using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Text;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using log4net;
using ServiceStack.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FastMember;
using ServiceStack.DesignPatterns.Model;
using ServiceStack.Text;

namespace System
{
    public static class ObjectExt
    {
        public static object PobierzKlucz(this object obiekt)
        {
            bool pusty = false;
            return PobierzKlucz(obiekt, ref pusty);
        }

        public static object PobierzKlucz(this object obiekt, ref bool brakKlucza)
        {
            brakKlucza = false;
            IHasStringId id = obiekt as IHasStringId;
            if (id != null)
            {
                if (id.Id == string.Empty)
                {
                    brakKlucza = true;
                }
                return id.Id;
            }
            IHasIntId intId = obiekt as IHasIntId;
            if (intId != null)
            {
                if (intId.Id == 0)
                {
                    brakKlucza = true;
                }
                return intId.Id;
            }
            IHasLongId longId = obiekt as IHasLongId;
            if (longId != null)
            {
                if (longId.Id == 0)
                {
                    brakKlucza = true;
                }
                return longId.Id;
            }
            IHasGuidId guidId = obiekt as IHasGuidId;
            if (guidId != null)
            {
                return guidId.Id;
            }
            throw new ArgumentException("Nie udało się odcztytać ID obiektu");
        }

        public static bool PorownajWartosc<T>(this T wartosc, T wartosc2, Wartosc warunek)
        {
            IComparable w1 = wartosc as IComparable;
            IComparable w2 = wartosc2 as IComparable;
            if (w1 == null)
            {
                w1 = wartosc.ToString();
                w2 = wartosc2.ToString();
            }
            int com = w1.CompareTo(w2);

            switch (warunek)
            {
                case Wartosc.Mniejsze:
                    return com < 0;
                case Wartosc.MniejszeRowne:
                    return com <= 0;
                case Wartosc.Rowne:
                    return com == 0;
                case Wartosc.Wieksze:
                    return com > 0;
                case Wartosc.WiekszeRowne:
                    return com >= 0;
                case Wartosc.Rozne:
                    return com != 0;
                case Wartosc.Dowolna:
                    return true;
                default:
                    throw new Exception("Nieobsługiwana opcja");
            }
        }

        private static ILog _log;

        public static ILog log
        {
            get
            {
                if (_log == null)
                {
                    _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                }
                return _log;
            }
            set { _log = value; }
        }

        public static bool RownyWartosciDomyslnej(this object o)
        {
            if (o == null) return true;
            Type type = o.GetType();
            if (type == typeof (string))
            {
                return string.IsNullOrEmpty(o.ToString());
            }
            if (!type.IsValueType) return false;
            if (Nullable.GetUnderlyingType(type) != null) return false; // ditto, Nullable<T>
            object defaultValue = Activator.CreateInstance(type); // must exist for structs
            return o.Equals(defaultValue);
        }

        public static string DoLadnejCyfry(this decimal o, string format = "0.##")
        {
            return o.ToString(format);
        }

        public static string DoLadnejCyfry(this double o, string format = "0.##")
        {
            return o.ToString(format);
        }

      
        public static Dictionary<string, T> ZbudojSlownikZKluczemPropertisowym<T>(this IEnumerable<T> kolekcja, Dictionary<string, PropertyInfo> ktorepola, bool pomijajduble = false)
        {
            Dictionary<string, T> wynik = new Dictionary<string, T>();
            foreach (T x in kolekcja)
            {
                string klucz = ZbudujKlucz(x, ktorepola);
                if (wynik.ContainsKey(klucz))
                {
                    if (!pomijajduble)
                    {
                        throw new Exception("Duble klucza " + klucz);
                    }
                }
                else
                {
                    wynik.Add(klucz, x);
                }

            }
            return wynik;
        }

        public static string ZbudujKlucz<T>(this T obiekt, object ktorepola)
        {
            return ZbudujKlucz(obiekt, ktorepola.Properties());
        }

        private static string ZbudujKlucz<T>(this T obiekt, Dictionary<string, PropertyInfo> polaDoSprawdzenia, TypeAccessor akcesor)
        {
            if (polaDoSprawdzenia == null || polaDoSprawdzenia.IsEmpty())
            {
                throw new Exception("Brak pól do wyliczenia klucza!");
            }

            StringBuilder klucz = new StringBuilder(10000);
            foreach (var pi in polaDoSprawdzenia)
            {
                object val = akcesor[obiekt, pi.Key];
                klucz.Append(pi.Value.Name);
                klucz.Append(":");
             
                if (val == null)
                {
                    klucz.Append("NULL");
                }

                else if (val is decimal)
                {
                    klucz.Append(((decimal) val).ToString("0.0000"));
               
                }
                else if (val is double)
                {
                    klucz.Append(((double) val).ToString(CultureInfo.InvariantCulture));
                }else if (val is Array)
                {
                    klucz.Append((val.ToCsv()));
                }
                else if(val.GetType().IsGenericType && val.GetType().GetGenericTypeDefinition() == typeof(HashSet<>))
                {
                    //jesli jest HASHSETEM TO nie wazna kolejnosc!!!
                    object posortowanaKolekcjaCsv = null;
                    if (val is HashSet<int>)
                    {
                        posortowanaKolekcjaCsv = (val as HashSet<int>).OrderBy(x => x).ToCsv();
                    }else if (val is HashSet<long>)
                    {
                        posortowanaKolekcjaCsv = (val as HashSet<long>).OrderBy(x => x).ToCsv();
                    }
                    else if (val is HashSet<string>)
                    {
                        posortowanaKolekcjaCsv = (val as HashSet<string>).OrderBy(x => x).ToCsv();
                    }else if (val is HashSet<decimal>)
                    {
                        posortowanaKolekcjaCsv = (val as HashSet<decimal>).OrderBy(x => x).ToCsv();
                    }
                    else if (val.GetType().GetGenericArguments()[0].IsEnum)
                    {
                        posortowanaKolekcjaCsv = val.PosortujHashSetEnumow();
                    }
                    else
                    {
                        throw new Exception("Nie można posortować hashseta - porpaw kod robienia klucza");
                    }


                    klucz.Append(posortowanaKolekcjaCsv);
                }
                else if (val is List<Dictionary<object, object>>)
                {
                    string wartosc = string.Empty;
                    foreach (var slownik in val as List<Dictionary<object, object>>)
                    {
                        if (slownik != null)
                        {
                            wartosc += slownik.Values.Select(x => x.ToString()).ToCsv();
                        }
                    }
                    klucz.Append(wartosc.WygenerujIDObiektuSHAWersjaLong());
                }
                else
                {
                    klucz.Append(val);
                }
                klucz.Append("_");
            }
            string wynik = klucz.ToString();
           
            if (string.IsNullOrEmpty(wynik))
            {
                throw new Exception("Wygenrowano pusty klucz");
            }
            return wynik;
        }

        //Upubliczniene ze wzglęu na test jednostkowy
        public static string PosortujHashSetEnumow(this object ob)
        {
            HashSet<int> wynik = new HashSet<int>();
            IEnumerable os = ob as IEnumerable;
            if (os != null)
            {
                foreach (object o in os)
                {
                    try
                    {
                        wynik.Add(Convert.ToInt32(o));
                    }
                    catch
                    {
                        throw new Exception($"Sprawdz poprawność wywołania metody sortującej HashSet enumów. Nie można rzutować wartości: {o} na wartość liczbową");
                    }
                }
            }
            else
            {
                throw new Exception($"Nie można korzystać z metody sortującej dla HashSetow jeżeli obiekt nie jest HashSetem");
            }
            return string.Join(",", wynik.OrderBy(x => x));
        }


        private static readonly object objektLocka = new object();

        public static void Porownaj<T, TT>(this IDictionary<T, TT> zrodlowy, IDictionary<T, TT> doPorownania, ref HashSet<T> doAktualizacji, ref HashSet<T> doDodania,
            ref HashSet<T> doUsuniecia, object PomijanePola = null, IWyswietlanieKomunikatu msg = null)
        {
            DateTime start = DateTime.Now;
            Type t = typeof (TT);
            log.DebugFormat("Porównywanie słowników typu <{0},{1}> ilosc zrodlowy {2} doPorownania {3}", typeof (T).Name, typeof (TT).Name, zrodlowy.Count, doPorownania.Count);
            if (PomijanePola != null)
            {
                log.DebugFormat("Pola pomijane jawnie: {0}. Dodatkowo będą pomijane pola ignorowane SQL.", PomijanePola.ToCsv());
            }
            doUsuniecia = new HashSet<T>( zrodlowy.Keys.Except(doPorownania.Keys) );
            doDodania = new HashSet<T>(doPorownania.Keys.Except(zrodlowy.Keys));
 
            IEnumerable<T> kluczeDoSprawdzenia = zrodlowy.Keys.Except(doUsuniecia).Except(doDodania);

            HashSet<T> doAktualiacjiBezRef = doAktualizacji;

            var polaDoSprawdzenia = typeof(TT).Properties(PomijanePola, true);
            var akcesor = typeof(TT).PobierzRefleksja();
            Parallel.ForEach(kluczeDoSprawdzenia, klucz =>
            {
                if (zrodlowy[klucz].Porownaj(doPorownania[klucz], polaDoSprawdzenia, akcesor)) return;
                lock (objektLocka)
                {
                    doAktualiacjiBezRef.Add(klucz);
                }
            });

            doAktualizacji = doAktualiacjiBezRef;

            //sprawdzenie typu na pierwszym obiekcie
            if (doUsuniecia.Any())
            {
                var pierwszy = zrodlowy.FirstOrDefault().Value;
                if (!(pierwszy is IPolaIDentyfikujaceRecznieDodanyObiekt))
                {
                    log.Debug("Brak implementacji IPolaIDentyfikujaceRecznieDodanyObiekt, może nastąpić usunięcie ręcznie dodanych obiektów. Typ:" + pierwszy.GetType().FullName);
                }
                else
                {
                    foreach (var x in zrodlowy)
                    {
                        if ((x.Value as IPolaIDentyfikujaceRecznieDodanyObiekt).RecznieDodany())
                        {
                            doUsuniecia.Remove(x.Key);
                        }
                    }
                }
            }

            if (msg != null && (doDodania.Count != 0 || doAktualizacji.Count != 0 || doUsuniecia.Count != 0))
            {
                string doDodaniaIds = (doDodania != null && doDodania.Any() && doDodania.Count < 7) ? " (" + doDodania.Join(", ") + ")" : "";
                string doAktualizacjiIds = (doAktualizacji != null && doAktualizacji.Any() && doAktualizacji.Count < 7) ? " (" + doAktualizacji.Join(", ") + ")" : "";
                string doUsunieciaIds = (doUsuniecia != null && doUsuniecia.Any() && doUsuniecia.Count < 7) ? " (" + doUsuniecia.Join(", ") + ")" : "";

                msg.WyswietlString($"{t.Name} - do dodania {doDodania.Count}{doDodaniaIds}, do aktualizacji {doAktualizacji.Count}{doAktualizacjiIds}, do usunięcia {doUsuniecia.Count}{doUsunieciaIds}");

                if (log.IsDebugEnabled)
                {
                    if (doUsuniecia.Count != 0)
                    {
                        log.DebugFormat("Usuwanie: {0}", doUsuniecia.SerializeToString());
                    }
                    if (doDodania.Count != 0)
                    {
                        log.DebugFormat("Dodawanie: {0}", doDodania.SerializeToString());
                    }
                    if (doAktualizacji.Count != 0)
                    {
                        log.DebugFormat("Aktualizacja: {0}", doAktualizacji.SerializeToString());
                    }
                }

            }
            TimeSpan czas = DateTime.Now - start;
            log.DebugFormat("Czas porównywania kolekcji typu {0} zlodlowa rozmiar {1} doPorownania rozmiar {2} czas {3} milisekund,do dodania {4}, do aktualizacji {5}, do usunięcia {6}", t.Name, zrodlowy.Count, doPorownania.Count, czas.TotalMilliseconds, doDodania.Count, doAktualizacji.Count, doUsuniecia.Count);
        }

       
        public static List<T> WhereKeyIsIn<TT, T>(this IDictionary<TT, T> slownik, IEnumerable<TT> ids)
        {
            if (ids == null || !ids.Any())
            {
                return new List<T>();
            }

            try
            {
                List<T> temp = new List<T>(ids.Count());
                foreach (TT id in ids)
                {
                    if (slownik.TryGetValue(id, out T wynik))
                    {
                        temp.Add(wynik);
                    }
                }
                return temp;
            }catch(IndexOutOfRangeException e)
            {
                log.Error($"Błąd odczytu atrybutów - ids: [ {string.Join(",", ids)} ], slownik zawiera: {slownik.Count} elementów");
                throw;
            }
            catch 
            {
                throw;
            }
        }

        public static IDictionary<TT, T> WhereKeyIsInDoSlownika<TT, T>(this IDictionary<TT, T> slownik, HashSet<TT> ids)
        {
            if (ids == null)
            {
                return new Dictionary<TT, T>();
            }
            Dictionary<TT,T> temp = new Dictionary<TT,T>(ids.Count);
            foreach (TT id in ids)
            {
                T wynik;
                if (slownik.TryGetValue(id, out wynik))
                {
                    temp.Add(id, wynik);
                }
            }
            return temp;
        }

        /// <summary>
        /// Usuwa zdublowane pozycje z kolekcji, duble szuka po PolaDoSprawdzenia
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="kolekcjaWejsciowa"></param>
        /// <param name="PolaDoSprawdzenia">po jakich polach szuka dubli</param>
        public static void UsunDuble<T>(this IList<T> kolekcjaWejsciowa, object PolaDoSprawdzenia)
        {
            List<T> doUsuniecia = new List<T>();
            List<string> wartosciDoUsuniecia = new List<string>();
            List<T> lista = kolekcjaWejsciowa.ToList();

            var polaZrodlowego = typeof(T).PropertiesPobierzWszystkie();
            var polaDoSprawdzania = PolaDoSprawdzenia.GetType().PropertiesPobierzWszystkie().Keys;
            List<PropertyInfo> doSprawdzania = polaZrodlowego.Values.Where(x => polaDoSprawdzania.Contains(x.Name)).ToList();
           
            if (doSprawdzania.Count > 0)
            {
                var akcesor = typeof(T).PobierzRefleksja();
                foreach (PropertyInfo pi in doSprawdzania)
                {
                    Dictionary<string, int> slownik = new Dictionary<string, int>(lista.Count);
                    Parallel.For(0, lista.Count, i =>
                    {
                        object val = akcesor[lista[i], pi.Name];
                        if (val == null)
                        {
                            return;
                        }
                        string valo = val.ToString();
                        if (!string.IsNullOrWhiteSpace(valo))
                        {
                            if (slownik.ContainsKey(valo))
                            {
                                doUsuniecia.Add(lista[i]);
                                doUsuniecia.Add(lista[slownik[valo]]);
                                wartosciDoUsuniecia.Add(valo);
                            }
                            else slownik.Add(valo, i);
                        }
                    });
                }
            }
            if (doUsuniecia.Any())
            {
                log.Info($"Zdublowane pozycje: {wartosciDoUsuniecia.ToArray().Join(", ")}");
            }
            foreach (var val in doUsuniecia)
            {
                kolekcjaWejsciowa.Remove(val);
            }
        }

        public static void SprawdzDuble<T>(this IEnumerable<T> kolekcjaWejsciowa, object PolaDoSprawdzenia)
        {
            string nazwaTypu = typeof(T).Name;
            string komunikat = "Wykryto zdublowane pozycje, pierwszy dubel- pole: {0} i wartość: {1} na pozycjach: {2} i {3}. \r\n"+nazwaTypu+ " 1: {4}\r\n" + nazwaTypu + " 2: {5}";
            log.Debug($"Sprawdzanie dubli dla {nazwaTypu}" );
            List<T> lista = kolekcjaWejsciowa.ToList();
            var polaZrodlowego = typeof (T).PropertiesPobierzWszystkie();
            var polaDoSprawdzania = PolaDoSprawdzenia.GetType().PropertiesPobierzWszystkie().Keys;
            List<PropertyInfo> doSprawdzania = polaZrodlowego.Values.Where(x => polaDoSprawdzania.Contains(x.Name)).ToList();

            if (doSprawdzania.Count == 0)
            {
                ConcurrentDictionary<string, int> slownik = new ConcurrentDictionary<string, int>();
                Parallel.For(0, lista.Count, i =>
                {
                    string valo = lista[i].ToString();
                    if (!string.IsNullOrWhiteSpace(valo))
                    {
                        try
                        {
                            slownik.TryAdd(valo, i);
                            //slownik.AddOrUpdate(valo, i);
                        } catch
                        {
                            int element;
                            slownik.TryRemove(valo, out element);
                            log.Error(string.Format(komunikat, valo, valo, i, slownik[valo], lista[slownik[valo]].ToCsv(), lista[i].ToCsv()));
                            //throw new Exception(string.Format(komunikat, valo, valo, i, slownik[valo], lista[slownik[valo]].ToCsv(), lista[i].ToCsv()));
                        }
                    }
                });
            }
            else
            {
                var akcesor = typeof(T).PobierzRefleksja();
                Parallel.ForEach(doSprawdzania, pi =>
                {
                    ConcurrentDictionary<string, int> slownik = new ConcurrentDictionary<string, int>();
                    for (int i = 0; i < lista.Count; ++i)
                    {
                        object val = akcesor[lista[i], pi.Name];
                        if (val == null)
                        {
                            return;
                        }
                        string valo = val.ToString();
                        if (!string.IsNullOrWhiteSpace(valo))
                        {
                            try
                            {
                                slownik.TryAdd(valo, i);
                            } catch
                            {
                                int element;
                                slownik.TryRemove(valo, out element);
                                log.Error(string.Format(komunikat, valo, valo, i, slownik[valo], lista[slownik[valo]].ToCsv(), lista[i].ToCsv()));
                                // new Exception(string.Format(komunikat, pi.Name, valo, i, slownik[valo], lista[slownik[valo]].ToCsv(), lista[i].ToCsv()));
                            }
                        }
                    }
                });
            }
            log.Debug($"Koniec sprawdzani dubli dla {nazwaTypu}");
        }

        public static string TrimEnd(this string s, string doWyciecia)
        {
            if (s.EndsWith(doWyciecia))
            {
                return s.Substring(0, s.LastIndexOf(doWyciecia, StringComparison.OrdinalIgnoreCase));
            }
            return s;
        }

        public static T ClonePojedynczyObiekt<T>(this T zrodlo)
        {
            T nowy = Activator.CreateInstance<T>();
            nowy.KopiujPola(zrodlo);
            return nowy;
        }

        public static void PokazRozniceWStringach(string target, string source)
        {
            int indexPierwszejPozycji = source.Zip(target, (c1, c2) => c1 == c2).TakeWhile(b => b).Count() + 1;

            if (log.IsDebugEnabled)
            {
                //tylko dla dlugich ciagow to liczymy
                if (source.Length > 50)
                {
                    string zrodlo10znakowRoznicy = null, target10znakowRoznicy = null;
                    int wycinekStart = indexPierwszejPozycji - 10;

                    zrodlo10znakowRoznicy = source.Substring(wycinekStart < 0 ? 0 : wycinekStart, (wycinekStart + 20) < source.Length ? 20 : source.Length - 1 - wycinekStart);
                    target10znakowRoznicy = target.Substring(wycinekStart < 0 ? 0 : wycinekStart, (wycinekStart + 20) < target.Length ? 20 : target.Length - 1 - wycinekStart);

                    log.Debug($"porównanie obiektu: pierwsza różnica na pozycji {indexPierwszejPozycji}.Wartości [zrodlo, cel]: wycinek przed i po: \r\n[{zrodlo10znakowRoznicy}]\r\n[{target10znakowRoznicy}]\r\nDokładny zrzut obiektów:\r\n{source}\r\n{target}");
                }
                else
                {
                    log.Debug($"porównanie obiektu: pierwsza różnica na pozycji {indexPierwszejPozycji}.Wartości [zrodlo, cel]:\r\n{source}\r\n{target}");
                }
            }
        }
        
        /// <summary>
        /// metoda porównująca obiekty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="zrodlo"></param>
        /// <param name="target"></param>
        /// <param name="kppola">pola do pominiecia</param>
        /// <param name="wszystkiepropertisy">pola do sprawdzenia</param>
        /// <returns></returns>
        public static bool Porownaj<T>(this T zrodlo, T target, Dictionary<string, PropertyInfo> polaDoSprawdzania, TypeAccessor akcesor)
        {
            string kluczz = zrodlo.ZbudujKlucz(polaDoSprawdzania, akcesor);
            string kluczt = target.ZbudujKlucz(polaDoSprawdzania, akcesor);

            if (kluczt != kluczz)
            {
                PokazRozniceWStringach(kluczt, kluczz);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Kopiuje wybrane pola z obiektu wzorcowego
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TT"></typeparam>
        /// <param name="uzupelniana">Słownik obiektów którę bedą miały uzupełnione pola.</param>
        /// <param name="wzorcowa">Słownik obiektów z którego bedą pobierane wartości.</param>
        /// <param name="polaDoKopiowanie">Tablica z propertisami do skopiowania</param>
        /// <param name="polaDoPomijania">Tablica z propertisami do pominięcią</param>
        public static void KopiujPolaIstniejaceObiekty<T, TT>(this Dictionary<T, TT> uzupelniana, Dictionary<T, TT> wzorcowa,object polaDoKopiowania = null, object polaDoPomijania = null)
        {
            Dictionary<string, PropertyInfo> propertisyObiektu = typeof(TT).Properties(polaDoPomijania);
            if (polaDoKopiowania != null)
            {
                Dictionary<string, PropertyInfo> listaPolDokopiowania = polaDoKopiowania.Properties();
                propertisyObiektu = propertisyObiektu.Where(x => listaPolDokopiowania.ContainsKey(x.Key)).ToDictionary(x=>x.Key, x=>x.Value);
            }

            propertisyObiektu = propertisyObiektu.Where(x=> x.Value.CanWrite && x.Value.CanRead).ToDictionary(x => x.Key, x => x.Value);

            //wyajtek jsadks pole puste
            var akcesor = typeof(TT).PobierzRefleksja();

            Parallel.ForEach(uzupelniana, x =>
            {
                TT wartoscWzorcowa;
                if (wzorcowa.TryGetValue(x.Key, out wartoscWzorcowa))
                {
                    foreach (var p in propertisyObiektu)
                    {
                        var value = akcesor[wartoscWzorcowa, p.Key];
                        akcesor[x.Value, p.Key] = value;
                    }
                }
            });
        }
        
        /// <summary>
        /// Kopiuje pola wybranago obiektu do danego obiektu. Muszą zgadzać się nazy propertisów i ich typy
        /// </summary>
        /// <param name="dokogo">Obiekt do którego wstawiamy  wartość</param>
        /// <param name="zrodlowy">Obieekt z którego będzięmy pobierać wartość</param>
        /// <param name="propertisyKtorePomijac">tablica propertisów do pominięcia</param>
        public static void KopiujPola(this object dokogo, object zrodlowy, object propertisyKtorePomijac = null)
        {
            if (zrodlowy == null) return;

            Type typDocelowego = dokogo.GetType();
            while (typDocelowego != typeof(object) && typDocelowego.BaseType != typeof(object))
            {
                typDocelowego = typDocelowego.BaseType;
            }

            Type typZrododlowego = zrodlowy.GetType();

            while (typZrododlowego != typeof(object) && typZrododlowego.BaseType != typeof(object))
            {
                typZrododlowego = typZrododlowego.BaseType;
            }

            Dictionary<string, PropertyInfo> polaZrodlowego = typZrododlowego.Properties(propertisyKtorePomijac);
            Dictionary<string, PropertyInfo> polaDocelowego = typDocelowego.Properties(propertisyKtorePomijac);

            var akcesorDocelowy = typDocelowego.PobierzRefleksja();
            var akcesorZrodlowy = typZrododlowego.PobierzRefleksja();

            foreach (var pole in polaDocelowego)
            {
                if (pole.Value.CanWrite)
                {
                    var poleZrodlo = polaZrodlowego[pole.Key];
                    if (poleZrodlo.CanRead && poleZrodlo.PropertyType == pole.Value.PropertyType)
                    {
                        akcesorDocelowy[dokogo, pole.Key] = akcesorZrodlowy[zrodlowy, pole.Key];
                    }
                }
            }
        }

    }

}
