using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using FastMember;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using ServiceStack.Common;

namespace SolEx.Hurt.Core
{
    public class SettingCollection : ISettingCollection
    {
        protected Dictionary<string, Ustawienie> settingCollection = null;

        public SettingCollection()
        {
            _wszystkiepolaDoPorownania = typeof(Ustawienie).Properties();
        }

        private IConfigDataProvider _provider;
        /// <summary>
        /// pusty konstruktor
        /// </summary>
        public SettingCollection(IConfigDataProvider provider)
        {
            _provider = provider;
            settingCollection = new Dictionary<string, Ustawienie>();
        }

        /// <summary>
        /// Konstruktor na bazie listy settingów
        /// </summary>
        /// <param name="listSettings"></param>
        public SettingCollection(List<Ustawienie> listSettings, IConfigDataProvider provider)
        {
            _provider = provider;
            settingCollection = new Dictionary<string, Ustawienie>();
            Wypelnij(listSettings);
        }
        private void Wypelnij(List<Ustawienie> listSettings)
        {
            foreach (Ustawienie s in listSettings)
            {
                if (settingCollection.ContainsKey(s.Symbol))
                    settingCollection[s.Symbol] = s;
                else
                    settingCollection.Add(s.Symbol, s);
            }
        }

        public void AddSetting(Ustawienie set)
        {
            settingCollection.Add(set.Symbol,set);
        }
        public void AddSettings(IEnumerable<Ustawienie> set)
        {
            foreach (var s in set)
            {
                settingCollection.Add(s.Symbol, s);
            }
        }

        private Dictionary<string, PropertyInfo> _polaPorownanie;
        private Dictionary<string, PropertyInfo> _wszystkiepolaDoPorownania;

        private Dictionary<string, PropertyInfo> UstawieniaPolaPorownanie
        {
                get
                {
                    if( _polaPorownanie == null)
                    {
                        Ustawienie wzor = new Ustawienie() {Symbol = "test"};
                        var polaKtorychNieSprawdzac = new {  wzor.Id, wzor.Wartosc,wzor.OddzialId,wzor.WartoscDlaNiezalogowanych, wzor.PoprzedniaWartoscDlaNiezalogowanych, wzor.PoprzedniaWartosc };
                        _polaPorownanie = typeof(Ustawienie).Properties(polaKtorychNieSprawdzac, true);
                    }
                    return _polaPorownanie;
                }
        }

        private TypeAccessor akcesorRefleksjaUstawienia = typeof(Ustawienie).PobierzRefleksja();

        //HashSet<string> idprzetworzonych = new HashSet<string>();

        public virtual string GetSetting(string symbol, string domyslnaWartosc, TypUstawienia type, string slownik, string opis, ustawieniaGrupa grupaUstawien, bool nadpisywanyPrzezPracownika,
            bool domyslnieWidoczne, bool zalogowany, bool multi = false, TypUstawieniaPodgrupa podgrupa = TypUstawieniaPodgrupa.Brak, bool dynamiczne = false, string podgrupaTeksotwea = null, 
            string domyslnaWartoscNiezalogowani = null)
            {
            string wartosc = null;
            string wartoscnNiezalogowani = null;
            symbol = symbol.ToLower();  //na wszelki wypadek - ustawienia sa roznie zapisywane i zeby ktos sie nie walnal (a byly takie przypadki)

            Ustawienie znalezione;
            if (settingCollection.TryGetValue(symbol,out znalezione)) //sprawdzanie typu ustawienia
            {
                Ustawienie wzor = new Ustawienie(symbol, opis, domyslnaWartosc, grupaUstawien.ToString(), symbol, type, domyslnieWidoczne, slownik, nadpisywanyPrzezPracownika, null, opis, multi, null, podgrupa, null, null, dynamiczne, podgrupaTeksotwea, domyslnaWartosc, domyslnaWartoscNiezalogowani);
                if (!znalezione.Porownaj(wzor, UstawieniaPolaPorownanie, akcesorRefleksjaUstawienia))
                {
                    //wzor.Id = znalezione.Id;
                    wzor.Wartosc = znalezione.Wartosc;
                    wzor.WartoscDlaNiezalogowanych = znalezione.WartoscDlaNiezalogowanych;
                    wzor.OddzialId = znalezione.OddzialId;
                    // wzor.WartoscDomyslna = znalezione.WartoscDomyslna;
                    //  wzor.WartoscDomyslnaDlaNiezalogowanych = znalezione.WartoscDomyslnaDlaNiezalogowanych;
                    // wzor.WartoscNiezalogowaniPoprzednia = settingCollection[symbol].Pracownik_id;
                    settingCollection[symbol] = wzor;
                    SolexBllCalosc.PobierzInstancje.Log.DebugFormat($"Poprawienie ustawienia symbol: {wzor.Symbol}, oddzia³: {wzor.OddzialId}- wartoœæ jest inna ni¿ zalcena w B2B, lub pojawi³y siê nowe wartoœci.");
                    AktualizujUstawienie(wzor);
                }
                //Niepotrzebne cachowanie jest to zdrobione wy¿ej
                //idprzetworzonych.Add(symbol);
                //if (!idprzetworzonych.Contains(symbol))
                //{

                //}
                wartosc = znalezione.Wartosc;
                wartoscnNiezalogowani = znalezione.WartoscDlaNiezalogowanych;
            }
            else
            {
                znalezione = _provider.PobierzUstawienie(symbol);
             
                if (znalezione != null)
                {
                    wartosc = znalezione.Wartosc;
                    wartoscnNiezalogowani = znalezione.WartoscDlaNiezalogowanych;
                }
            }

            if (znalezione == null)
            {
                Ustawienie wzor = new Ustawienie(symbol, opis, null, grupaUstawien.ToString(), symbol, type,
                    domyslnieWidoczne, slownik, nadpisywanyPrzezPracownika, null, opis, multi, null, podgrupa, null,
                    null, dynamiczne, podgrupaTeksotwea, domyslnaWartosc, domyslnaWartoscNiezalogowani);
                settingCollection.Add(symbol, wzor);
                SolexBllCalosc.PobierzInstancje.Log.DebugFormat($"Dodawanie nowego ustawienia - brak ustawienia o symbolu: {symbol} w kolekcji ustawieñ. Lista ustawieñ obecna (symbole): {settingCollection.Keys.ToList().Join(",")}");
                AktualizujUstawienie(wzor);
            }

            string wartoscDlaZalogowanych = domyslnaWartosc;
            
            if (wartosc != null)
            {
                wartoscDlaZalogowanych =  wartosc;
            }
           
            if (zalogowany)
            {
                return wartoscDlaZalogowanych;
            }

            if (wartoscnNiezalogowani != null)
            {
                return wartoscnNiezalogowani;
            }

            //dla NIEzalogowanych - jesli wartosc jest podana domyslna to domyslna, jak nie ma to dla zalogowanych
            if (domyslnaWartoscNiezalogowani != null)
            {
                return domyslnaWartoscNiezalogowani;
            }
            return wartoscDlaZalogowanych;
        }

        /// <summary>
        /// Pobiera aktualn¹ wartoœæ ustawienia, jeœli ustawienie nie istnieje to zwaraca wartoœæ domyœln¹ typu String
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="defaultValue"></param>
        /// <param name="zalogowany"></param>
        /// <param name="opis"></param>
        /// <param name="grupaUstawien"></param>
        /// <param name="nadpisywanyPrzezPracownika"></param>
        /// <param name="domyslnieWidoczne"></param>
        /// <param name="podgrupa"></param>
        /// <param name="domyslnaWartoscNiezalogowani"></param>
        /// <param name="dynamiczne"></param>
        /// <returns></returns>
        public virtual string GetSettingString(string symbol, string defaultValue, bool zalogowany, string opis = null, 
            ustawieniaGrupa grupaUstawien = ustawieniaGrupa.Brakuj¹ce, bool nadpisywanyPrzezPracownika = false, bool domyslnieWidoczne = true, 
            TypUstawieniaPodgrupa podgrupa = TypUstawieniaPodgrupa.Brak, string domyslnaWartoscNiezalogowani = null, bool dynamiczne=false)
        {
            string val = GetSetting(symbol, defaultValue, TypUstawienia.String, null, opis, grupaUstawien, nadpisywanyPrzezPracownika,
                domyslnieWidoczne, zalogowany, podgrupa: podgrupa, domyslnaWartoscNiezalogowani: domyslnaWartoscNiezalogowani, dynamiczne:dynamiczne);
            return string.IsNullOrEmpty(val) ? null : val;
        }

        public HashSet<T> GetSettingSlownik<T>(string symbol, HashSet<T> defaultValue, IEnumerable<T> dostepneOpcje, bool zalogowany, string opis = null, ustawieniaGrupa grupaUstawien = ustawieniaGrupa.Brakuj¹ce, bool nadpisywanyPrzezPracownika = false, bool domyslnieWidoczne = true, bool multi = false, TypUstawieniaPodgrupa podgrupa = TypUstawieniaPodgrupa.Brak, bool dynamiczne = false,string podGrupaTeksotwa=null)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in dostepneOpcje)
            {
                sb.AppendFormat("{0};", item);
            }
            StringBuilder sb2 = new StringBuilder();
            foreach (var item in defaultValue)
            {
                sb2.AppendFormat("{0};", item);
            }
            string s = GetSetting(symbol, sb2.ToString(), TypUstawienia.Combo, sb.ToString(), opis, grupaUstawien, nadpisywanyPrzezPracownika, domyslnieWidoczne, zalogowany, multi, podgrupa, dynamiczne, podGrupaTeksotwa);
            if (string.IsNullOrEmpty(s))
            {
                return defaultValue;
            }
            var typeConverter = TypeDescriptor.GetConverter(typeof(T));
            HashSet<T> wynik = new HashSet<T>();
            var wartosci = s.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string st in wartosci)
            {
                wynik.Add((T)typeConverter.ConvertFrom(st));
            }
            return wynik;

            //return string.IsNullOrEmpty(val) ? defaultValue : val;
        }
        public string GetSettingPassword(string symbol, string defaultValue, bool zalogowany, string opis = null, ustawieniaGrupa grupaUstawien = ustawieniaGrupa.Brakuj¹ce,
            bool nadpisywanyPrzezPracownika = false, bool domyslnieWidoczne = true, TypUstawieniaPodgrupa podgrupa = TypUstawieniaPodgrupa.Brak,
            string domyslnaWartoscNiezalogowani = null)
        {
            string val = GetSetting(symbol, defaultValue, TypUstawienia.Password,  null, opis, grupaUstawien, nadpisywanyPrzezPracownika,
                domyslnieWidoczne, zalogowany, podgrupa: podgrupa, domyslnaWartoscNiezalogowani: domyslnaWartoscNiezalogowani);
            return string.IsNullOrEmpty(val) ? null : val;
        }

        /// <summary>
        /// Pobiera aktualn¹ wartoœæ ustawienia, jeœli ustawienie nie istnieje to zwaraca wartoœæ domyœln¹ typu Int
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="defaultValue"></param>
        /// <param name="zalogowany"></param>
        /// <param name="opis"></param>
        /// <param name="grupaUstawien"></param>
        /// <param name="nadpisywanyPrzezPracownika"></param>
        /// <param name="domyslnieWidoczne"></param>
        /// <param name="podgrupa"></param>
        /// <param name="dynamiczne"></param>
        /// <param name="domyslnaWartoscNiezalogowani"></param>
        /// <returns></returns>
        public int GetSettingInt(string symbol, int defaultValue, bool zalogowany = true, string opis = null, ustawieniaGrupa grupaUstawien = ustawieniaGrupa.Brakuj¹ce,
            bool nadpisywanyPrzezPracownika = false, bool domyslnieWidoczne = true, TypUstawieniaPodgrupa podgrupa = TypUstawieniaPodgrupa.Brak, 
            bool dynamiczne = false, int? domyslnaWartoscNiezalogowani = null)
        {
            int value;
            if (int.TryParse(GetSetting(symbol, defaultValue.ToString(CultureInfo.InvariantCulture), TypUstawienia.Decimal, null, opis, grupaUstawien,
                nadpisywanyPrzezPracownika, domyslnieWidoczne, zalogowany, false, podgrupa, dynamiczne, 
                domyslnaWartoscNiezalogowani != null ? domyslnaWartoscNiezalogowani.Value.ToString(CultureInfo.InvariantCulture) : null), out value))
            {
                return value;
            }
            return defaultValue;
        }

        public HashSet<TZwracany> GetSettingSlownikRefleksja<T, TZwracany>(string symbol, HashSet<TZwracany> defaultValue, bool zalogowany = true, string opis = null,
            ustawieniaGrupa grupaUstawien = ustawieniaGrupa.Brakuj¹ce, bool nadpisywanyPrzezPracownika = false,
            bool domyslnieWidoczne = true,bool multi=false, TypUstawieniaPodgrupa podgrupa = TypUstawieniaPodgrupa.Brak,
            HashSet<TZwracany> domyslnaWartoscNiezalogowani = null)
        {
            Type typ = typeof (T);
            string wartoscDefaultString = null;
           
                StringBuilder sb = new StringBuilder();
                foreach (var item in defaultValue)
                {
                    sb.AppendFormat("{0};", item);
                }
                wartoscDefaultString = sb.ToString();
           

            string wartoac = GetSetting(symbol, wartoscDefaultString, TypUstawienia.ComboRefleksja, 
                typ.PobierzOpisTypu(), opis, grupaUstawien,
                nadpisywanyPrzezPracownika, domyslnieWidoczne, zalogowany, multi, podgrupa, 
                domyslnaWartoscNiezalogowani: domyslnaWartoscNiezalogowani != null ? domyslnaWartoscNiezalogowani.ToString() : null);

            if (string.IsNullOrEmpty(wartoac))
            {
                return defaultValue;
            }
            var typeConverter = TypeDescriptor.GetConverter(typeof(TZwracany));
            var wartosci = wartoac.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries);
            HashSet<TZwracany> hashe = new HashSet<TZwracany>();
            foreach (var w in wartosci)
            {
                hashe.Add((TZwracany)typeConverter.ConvertFrom(w));
            }
            return hashe;
        }

        /// <summary>
        /// Pobiera aktualn¹ wartoœæ ustawienia, jeœli ustawienie nie istnieje to zwaraca wartoœæ domyœln¹ typu Decimal
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="defaultValue"></param>
        /// <param name="zalogowany"></param>
        /// <param name="opis"></param>
        /// <param name="grupaUstawien"></param>
        /// <param name="nadpisywanyPrzezPracownika"></param>
        /// <param name="domyslnieWidoczne"></param>
        /// <param name="domyslnaWartoscNiezalogowani"></param>
        /// <returns></returns>
        public decimal GetSettingDecimal(string symbol, decimal defaultValue, bool zalogowany = true, string opis = null, 
            ustawieniaGrupa grupaUstawien = ustawieniaGrupa.Brakuj¹ce, bool nadpisywanyPrzezPracownika = false,
            bool domyslnieWidoczne = true, decimal? domyslnaWartoscNiezalogowani = null)
        {
            decimal value;
            if (TextHelper.PobierzInstancje.SprobojSparsowac(GetSetting(symbol, defaultValue.ToString(), TypUstawienia.Decimal,  null, opis, grupaUstawien,
                nadpisywanyPrzezPracownika, domyslnieWidoczne, zalogowany, 
                domyslnaWartoscNiezalogowani: domyslnaWartoscNiezalogowani != null ? domyslnaWartoscNiezalogowani.ToString() : null), out value))
            {
                return value;
            }
            return defaultValue;
        }

        public HashSet<T> GetSettingEnum<T>(string symbol, HashSet<T> defaultValue, bool zalogowany, string opis = null, ustawieniaGrupa grupaUstawien = ustawieniaGrupa.Brakuj¹ce, bool nadpisywanyPrzezPracownika = false,
                                            bool domyslnieWidoczne = true, bool multi = false, TypUstawieniaPodgrupa podgrupa = TypUstawieniaPodgrupa.Brak, HashSet<T> domyslnaWartoscNiezalogowani = null)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in defaultValue)
            {
                sb.AppendFormat("{0};", item);
            }
            string s = GetSetting(symbol, sb.ToString(), TypUstawienia.Enum, typeof(T).PobierzOpisTypu(), opis, grupaUstawien, nadpisywanyPrzezPracownika, domyslnieWidoczne, zalogowany, multi, podgrupa,
                domyslnaWartoscNiezalogowani: domyslnaWartoscNiezalogowani != null ? domyslnaWartoscNiezalogowani.ToString() : null);

            if (string.IsNullOrEmpty(s))
            {
                return defaultValue;
            }

            HashSet<T> wynik = new HashSet<T>();
            //wynik = SolexBllCalosc.PobierzInstancje.Cache.SlownikPrywatny_PobierzObiekt<HashSet<T>>(() => { return new HashSet<T>(); }, lok, 1, $"{symbol}-{typeof(T)}", " df");

            var wartosci = s.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries);
            foreach (string st in wartosci)
            {
                string u = st.Trim().ToLower();
                foreach (T item in Enum.GetValues(typeof(T)))
                {
                    if (item.ToString().ToLower().Equals(u))
                    {
                        wynik.Add(item);
                    }
                }
                if (!wynik.Any())
                {
                    return defaultValue;
                }
            }
            return wynik;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TZwaracany"></typeparam>
        /// <param name="symbol"></param>
        /// <param name="defaultValue"></param>
        /// <param name="zalogowany"></param>
        /// <param name="opis"></param>
        /// <param name="grupaUstawien"></param>
        /// <param name="nadpisywanyPrzezPracownika"></param>
        /// <param name="domyslnieWidoczne"></param>
        /// <param name="podgrupa"></param>
        /// <param name="domyslnaWartoscNiezalogowani"></param>
        /// <param name="multi"></param>
        /// <returns></returns>
        public HashSet<TZwaracany> GetSettingReflekcja<T, TZwaracany>(string symbol, HashSet<TZwaracany> defaultValue, bool zalogowany, 
            string opis = null, ustawieniaGrupa grupaUstawien = ustawieniaGrupa.Brakuj¹ce, bool nadpisywanyPrzezPracownika = false, 
            bool domyslnieWidoczne = true, bool multi = false, TypUstawieniaPodgrupa podgrupa = TypUstawieniaPodgrupa.Brak,
            HashSet<TZwaracany> domyslnaWartoscNiezalogowani = null)
          
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in defaultValue)
            {
                sb.AppendFormat("{0};", item);
            }
            string s = GetSetting(symbol, sb.ToString(), TypUstawienia.Refleksja, typeof(T).FullName + "," + typeof(T).Assembly.FullName, opis,
                grupaUstawien, nadpisywanyPrzezPracownika, domyslnieWidoczne, zalogowany, multi,
                podgrupa, domyslnaWartoscNiezalogowani: domyslnaWartoscNiezalogowani != null ? domyslnaWartoscNiezalogowani.ToString() : null);
            //string s = GetSetting(symbol, JSonHelper.Serialize(defaultValue), TypUstawienia.Refleksja, typeof(T).FullName + "," + typeof(T).Assembly.FullName, opis,
            //    grupaUstawien, nadpisywanyPrzezPracownika, domyslnieWidoczne, zalogowany, multi,
            //    podgrupa, domyslnaWartoscNiezalogowani: domyslnaWartoscNiezalogowani != null ? domyslnaWartoscNiezalogowani.ToString() : null);


            if (string.IsNullOrEmpty(s))
            {
                return defaultValue;
            }
            HashSet<TZwaracany> wynik = new HashSet<TZwaracany>();
            var wartosc = s.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            var typeConverter = TypeDescriptor.GetConverter(typeof(T));
            foreach (object w in wartosc)
            {
                if (typeof (TZwaracany) != typeof (string))
                {
                    wynik.Add((TZwaracany) typeConverter.ConvertFrom(w));
                }
                else
                {
                    wynik.Add((TZwaracany) w);
                }
            }
            return wynik;
        }

        /// <summary>
        /// Pobiera aktualn¹ wartoœæ ustawienia, jeœli ustawienie nie istnieje to zwaraca wartoœæ domyœln¹ typu Bool
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="defaultValue"></param>
        /// <param name="zalogowany"></param>
        /// <param name="opis"></param>
        /// <param name="grupaUstawien"></param>
        /// <param name="nadpisywanyPrzezPracownika"></param>
        /// <param name="domyslnieWidoczne"></param>
        /// <param name="podgrupa"></param>
        /// <param name="domyslnaWartoscNiezalogowani"></param>
        /// <returns></returns>
        public bool GetSettingBool(string symbol, bool defaultValue, bool zalogowany = true, string opis = null,
            ustawieniaGrupa grupaUstawien = ustawieniaGrupa.Brakuj¹ce, bool nadpisywanyPrzezPracownika = false, bool domyslnieWidoczne = true,
            TypUstawieniaPodgrupa podgrupa = TypUstawieniaPodgrupa.Brak,
            bool? domyslnaWartoscNiezalogowani = null)
        {
            string s = GetSetting(symbol, defaultValue.ToString(), TypUstawienia.Bool,  null, opis,
                grupaUstawien, nadpisywanyPrzezPracownika, domyslnieWidoczne,
                zalogowany, podgrupa: podgrupa, domyslnaWartoscNiezalogowani: domyslnaWartoscNiezalogowani != null ? (domyslnaWartoscNiezalogowani.Value ? "1" : "0") : null);
            if (string.IsNullOrEmpty(s))
            {
                return defaultValue;
            }
            bool wartosc = false;
            if (bool.TryParse(s, out wartosc))
            {
                return wartosc;
            }
            if (s == "1" || s == "0")
            {
                return s == "1";
            }
            return defaultValue;
            //if (s == "1" || s == "0" || s.Equals("True", StringComparison.InvariantCultureIgnoreCase) || s.Equals("False", StringComparison.InvariantCultureIgnoreCase))
            //{
            //    return s == "1" || s.Equals("True", StringComparison.InvariantCultureIgnoreCase);
            //}
            //return defaultValue;
        }

        /// <summary>
        /// Pobiera aktualn¹ wartoœæ ustawienia, jeœli ustawienie nie istnieje to zwaraca wartoœæ domyœln¹ typu DateTime
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="defaultValue"></param>
        /// <param name="zalogowany"></param>
        /// <param name="opis"></param>
        /// <param name="grupaUstawien"></param>
        /// <param name="nadpisywanyPrzezPracownika"></param>
        /// <param name="domyslnieWidoczne"></param>
        /// <param name="domyslnaWartoscNiezalogowani"></param>
        /// <returns></returns>
        public DateTime GetSettingDateTime(string symbol, DateTime defaultValue, bool zalogowany, string opis = null,
            ustawieniaGrupa grupaUstawien = ustawieniaGrupa.Brakuj¹ce, bool nadpisywanyPrzezPracownika = false,
            bool domyslnieWidoczne = true, DateTime? domyslnaWartoscNiezalogowani = null)
        {
            DateTime value;
            if (DateTime.TryParse(GetSetting(symbol, defaultValue.ToString(), TypUstawienia.Datetime,  null, opis, grupaUstawien,
                nadpisywanyPrzezPracownika, domyslnieWidoczne, zalogowany,
                domyslnaWartoscNiezalogowani: domyslnaWartoscNiezalogowani !=null ? domyslnaWartoscNiezalogowani.ToString() : null), out value))
            {
                return value;
            }
            return defaultValue;
        }

        /// <summary>
        /// Pobiera aktualn¹ wartoœæ ustawienia, jeœli ustawienie nie istnieje to zwaraca wartoœæ domyœln¹ typu HTML
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="defaultValue"></param>
        /// <param name="zalogowany"></param>
        /// <param name="opis"></param>
        /// <param name="grupaUstawien"></param>
        /// <param name="nadpisywanyPrzezPracownika"></param>
        /// <param name="domyslnieWidoczne"></param>
        /// <param name="podgrupa"></param>
        /// <param name="domyslnaWartoscNiezalogowani"></param>
        /// <returns></returns>
        public string GetSettingHTML(string symbol, string defaultValue, bool zalogowany, string opis = null, ustawieniaGrupa grupaUstawien = ustawieniaGrupa.Brakuj¹ce,
            bool nadpisywanyPrzezPracownika = false, bool domyslnieWidoczne = true, 
            TypUstawieniaPodgrupa podgrupa = TypUstawieniaPodgrupa.Brak, string domyslnaWartoscNiezalogowani = null )
        {
            return GetSetting(symbol, defaultValue, TypUstawienia.HTML,  null, opis, grupaUstawien, nadpisywanyPrzezPracownika, domyslnieWidoczne,
                zalogowany, podgrupa: podgrupa, domyslnaWartoscNiezalogowani: domyslnaWartoscNiezalogowani);
        }
        public List<Ustawienie> GetSettingsList(bool onlyVisibleToUser, int? pracownikId)
        {
            return settingCollection.Where(s =>
                                                (!onlyVisibleToUser || s.Value.Widoczne) &&
                                                (pracownikId == null || s.Value.OddzialId == pracownikId)).Select(s => s.Value ).ToList();
        }

        public List<Ustawienie> GetSettingsList()
        {
            return settingCollection.Select(s => s.Value).ToList();
        }

        public NameValueCollection ToNameValueCollection()
        {
            NameValueCollection result = new NameValueCollection();
            foreach (var r in settingCollection)
            {
                result.Add(r.Key, r.Value.Wartosc);
            }
            return result;
        }

        /// <summary>
        /// Ustawia wartoœæ parametru
        /// </summary>
        /// <param name="key">Klucz</param>
        /// <param name="value">Wartoœæ</param>
        /// <param name="type">Typ ustawienia</param>
        public void SetSetting(string key, string value, TypUstawienia type)
        {
            Ustawienie u;
            if (settingCollection.ContainsKey(key))
            {
                settingCollection[key].Wartosc = value;
                u = settingCollection[key];
                u.Wartosc = value;
            }
            else
            {
                u = _provider.PobierzUstawienie(key);
                if (u == null)
                {
                    throw  new Exception($"Nie mo¿na pobraæ ustawienia: {key}.");
                }
                u.Wartosc = value;
                settingCollection.Add(key, u);
            }
           // SolexBllCalosc.PobierzInstancje.Log.DebugFormat($"Dodawanie nowego ustawienia. Brak klucza w slowniku {key}.");
            AktualizujUstawienie(u);
        }

        public void AktualizujUstawienie(Ustawienie u)
        {
           SprawdzWartosc(u);
            _provider.AktualizujUstawienie(u);
        }

        private void SprawdzWartosc(Ustawienie p)
        {
            switch (p.Typ)
            {
                case TypUstawienia.String:
                    if (p.Wartosc != null)
                    {
                        p.Wartosc = p.Wartosc.UsunFormatowanieHTML();
                    }
                    break;
            }
        }

    }
}