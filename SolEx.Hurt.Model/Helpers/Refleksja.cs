using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Compilation;
using FastMember;
using ServiceStack.Common;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Extensions;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Web;
using ServiceStack.DataAnnotations;
using StringExtensions = ServiceStack.Text.StringExtensions;

namespace SolEx.Hurt.Model.Helpers
{
    public static class Refleksja
    {
        private static Dictionary<Type, Dictionary<string, PropertyInfo>> _listaNazwPropertisowDlaTypow = new Dictionary<Type, Dictionary<string, PropertyInfo> >();
        private static Dictionary<Type, TypeAccessor> _akcesorFastMemberDostepu = new Dictionary<Type, TypeAccessor>();

        public static Dictionary<string, PropertyInfo> Properties(this object typ)
        {
            return typ.GetType().Properties();
        }

        public static object lok = new object();

        /// <summary>
        /// wersja metody Propertis pod inn¹ nazw¹ bo konfliky s¹ w innych klasach, które maj¹ te same nazwy extensionsow
        /// </summary>
        /// <param name="typ"></param>
        /// <returns></returns>
        public static Dictionary<string, PropertyInfo> PropertiesPobierzWszystkie(this Type typ)
        {
            return typ.Properties();
        }

        public static Dictionary<string, PropertyInfo> Properties(this Type typ)
        {
            Dictionary<string, PropertyInfo> lista = null;
            if (!_listaNazwPropertisowDlaTypow.TryGetValue(typ, out lista))
            {
                lock (lok)
                {
                    if (!_listaNazwPropertisowDlaTypow.TryGetValue(typ, out lista))
                    {
                        PropertyInfo[] pola = typ.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                        lista = new Dictionary<string, PropertyInfo>(pola.Length);
                        foreach (var p in pola)
                        {
                            try
                            {
                                var getMethod = p.GetGetMethod(false);
                                if (getMethod.GetBaseDefinition() == getMethod)
                                {
                                    lista.Add(p.Name, p);
                                }
                                if (getMethod.IsVirtual && !lista.ContainsKey(p.Name))
                                {
                                    //raczej nie bedzie 2 virtualnych?
                                    lista.Add(p.Name, p);
                                }
                            } catch (Exception e)
                            {
                                throw new Exception($"Nie uda³o siê pobraæ pól dla obiektu: {typ.Name}, pola: {p.Name}. B³¹d: {e.Message}");
                            }
                        }

                        _listaNazwPropertisowDlaTypow.Add(typ, lista);
                    }
                }
            }

            return lista;
        }

        /// <summary>
        /// Pobiera propertisy okreœlonego typu - ale tylko takie które maj¹ wymagany atrybut
        /// </summary>
        /// <param name="typ"></param>
        /// <param name="wymaganyAtrybut"></param>
        /// <returns></returns>
        public static Dictionary<string, PropertyInfo> Properties(this Type typ, Type wymaganyAtrybut)
        {
            return typ.Properties().Where(x => x.Value.GetCustomAttributes(wymaganyAtrybut, true).Length != 0).ToDictionary(x => x.Key, x => x.Value);
        }

        public static Dictionary<string, PropertyInfo> Properties(this Type typ, string[] polaDoPomijaniaNazwa )
        {
            if (polaDoPomijaniaNazwa == null || polaDoPomijaniaNazwa.IsEmpty())
            {
                return typ.Properties();
            }
            try
            {
                return typ.Properties().Where(x => !polaDoPomijaniaNazwa.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);
            }
            catch (Exception)
            {
                Log.FatalFormat($"Nie uda³o siê pobraæ pól dla obiektu: {typ.Name}, wszystkie pola: {StringExtensions.ToCsv(typ.Properties().Keys) } ");
                throw;
            }
        }

        public static Dictionary<string, PropertyInfo> Properties(this Type typ, object polaDoPomijaniaNazwa, bool usuwajPolaZAtrybutemIgnore = false)
        {
            if (polaDoPomijaniaNazwa == null && !usuwajPolaZAtrybutemIgnore)
            {
                return typ.Properties();
            }

            string[] polaDoPomijania = null;

            if (polaDoPomijaniaNazwa != null)
            {
                polaDoPomijania = polaDoPomijaniaNazwa.Properties().Keys.ToArray();
            }

            if (!usuwajPolaZAtrybutemIgnore)
            {
                return typ.Properties(polaDoPomijania);
            }
            try
            {
                return typ.Properties(polaDoPomijania).Where(x => x.Value.GetCustomAttributes(typeof(IgnoreAttribute), true).Length == 0).ToDictionary(x => x.Key, x => x.Value); //usuniêcie pó³ z atrybutem ignore)
            } catch (Exception)
            {
                Log.FatalFormat($"Nie uda³o siê pobraæ pól dla obiektu: {typ.Name}, wszystkie pola: {StringExtensions.ToCsv(typ.Properties().Keys) } ");
                throw;
            }
        }
        

        public static bool Implements<T>(this Type type)
        {
            return type.Implements(typeof(T));
        }

        public static bool Implements(this Type type, Type interfaceType)
        {
            if (type == null || interfaceType == null || type == interfaceType)
                return false;
            if (interfaceType.IsGenericTypeDefinition && type.GetInterfaces().Where(t => t.IsGenericType).Select(t => t.GetGenericTypeDefinition()).Any(gt => gt == interfaceType))
            {
                return true;
            }
            return interfaceType.IsAssignableFrom(type);
        }

        #region InheritsOrImplements
        /// <summary>
        /// Returns true if the supplied <paramref name="type"/> inherits from or implements the type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The base type to check for.</typeparam>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the given type inherits from or implements the specified base type.</returns>
        public static bool InheritsOrImplements<T>(this Type type)
        {
            return type.InheritsOrImplements(typeof(T));
        }

        /// <summary>
        /// Returns true of the supplied <paramref name="type"/> inherits from or implements the type <paramref name="baseType"/>.
        /// </summary>
        /// <param name="baseType">The base type to check for.</param>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the given type inherits from or implements the specified base type.</returns>
        public static bool InheritsOrImplements(this Type type, Type baseType)
        {
            if (type == null || baseType == null)
                return false;
            return baseType.IsInterface ? type.Implements(baseType) : type.Inherits(baseType);
        }
        #endregion

        #region Inherits
        /// <summary>
        /// Returns true if the supplied <paramref name="type"/> inherits from the given class <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type (class) to check for.</typeparam>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the given type inherits from the specified class.</returns>
        /// <remarks>This method is for classes only. Use <seealso cref="Implements"/> for interface types and <seealso cref="InheritsOrImplements"/> 
        /// to check both interfaces and classes.</remarks>
        public static bool Inherits<T>(this Type type)
        {
            return type.Inherits(typeof(T));
        }

        /// <summary>
        /// Returns true if the supplied <paramref name="type"/> inherits from the given class <paramref name="baseType"/>.
        /// </summary>
        /// <param name="baseType">The type (class) to check for.</param>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the given type inherits from the specified class.</returns>
        /// <remarks>This method is for classes only. Use <seealso cref="Implements"/> for interface types and <seealso cref="InheritsOrImplements"/> 
        /// to check both interfaces and classes.</remarks>
        public static bool Inherits(this Type type, Type baseType)
        {
            if (baseType == null || type == null || type == baseType)
                return false;
            var rootType = typeof(object);
            if (baseType == rootType)
                return true;
            while (type != null && type != rootType)
            {
                var current = type.IsGenericType && baseType.IsGenericTypeDefinition ? type.GetGenericTypeDefinition() : type;
                if (baseType == current)
                    return true;
                type = type.BaseType;
            }
            return false;
        }
        #endregion

        public static void DEBUG_ZliczWYkorzystanieFunkcjiWRequest(string symbolUstawienia, int limitIle = 20)
        {
            //tylko dla aplikacji webowych
            if (HttpContext.Current != null)
            {
                symbolUstawienia = "_ustwienia_" + symbolUstawienia;
                long iloscUruchomien = 0;
                if (HttpContext.Current.Items[symbolUstawienia] != null)
                {
                    iloscUruchomien = (long)HttpContext.Current.Items[symbolUstawienia];
                }
                
                ++iloscUruchomien;
                if (iloscUruchomien > limitIle)
                {
                    throw new Exception($"Pole: {symbolUstawienia} jest wywo³ane zbyt czêsto - ponad 50 razy w request! Popraw wywo³ania.");
                }
                HttpContext.Current.Items[symbolUstawienia] = iloscUruchomien;
            }
        }


        public static TypeAccessor PobierzRefleksja(this Type typ)
        {
            TypeAccessor akcesor;
            if (!_akcesorFastMemberDostepu.TryGetValue(typ, out akcesor))
            {
                lock (lok)
                {
                    if (!_akcesorFastMemberDostepu.TryGetValue(typ, out akcesor))
                    {
                        akcesor = TypeAccessor.Create(typ);
                        _akcesorFastMemberDostepu.Add(typ, akcesor);
                    }
                }
            }

//#if DEBUG
//            DEBUG_ZliczWYkorzystanieFunkcjiWRequest($"akcesor dla typu: {typ.Name}");
//#endif

            return akcesor;
        }

        /// <summary>
        /// Pobiera pole z obiektu i formatuje wg. zadanego stylu
        /// </summary>
        /// <param name="obiekt"></param>
        /// <param name="pole"></param>
        /// <param name="format"></param>
        /// <param name="akcesor">wymagany jesli jest pole nie zagniezdzone (nie z koropka)</param>
        /// <returns></returns>
        public static object PobierzWartoscFormatowana(object obiekt, string pole, string format, TypeAccessor akcesor)
        {
            object wartosc;
            if (pole.Contains('.'))
            {
                wartosc = PobierzWartosc(obiekt, pole);
            }
            else
            {
                wartosc = akcesor[obiekt, pole];
            }

            string fraza = "";

            if (wartosc != null)
            {
                if (wartosc is decimal)
                {
                    fraza = ((decimal)wartosc).DoLadnejCyfry();
                }
                else if (wartosc is double)
                {
                    fraza = ((double)wartosc).DoLadnejCyfry();
                }
                else if (wartosc is int)
                {
                    fraza = ((decimal)wartosc).DoLadnejCyfry();
                }
                else
                {
                    fraza = wartosc.ToString();
                }
            }
            if (string.IsNullOrEmpty(format)) format = "{0}";
            return string.Format(format, fraza);
        }


        public static T StworzKontrolke<T>(this IObiektPrzechowujacyKontrolke pojemnik, Jezyk jezyk = null)
        {
            return (T)StworzKontrolke(pojemnik,jezyk);
        }

        public static object StworzKontrolke(this IObiektPrzechowujacyKontrolke pojemnik, Jezyk jezyk = null)
        {
            Type typ = Type.GetType(pojemnik.TypKontrolki(),true);
            object obiekt = Activator.CreateInstance(typ);
            if (pojemnik is IPoleJezyk )
            {
                if (obiekt is IPoleJezyk)
                {
                    ((IPoleJezyk) obiekt).JezykId = ((IPoleJezyk) pojemnik).JezykId;
                }

            }
            else if(obiekt is IPoleJezyk)
            {
                if (jezyk == null)
                {
                    throw new InvalidOperationException( $"Próba stworzenia kontrolki wymagaj¹cej t³umaczenia (typ: {obiekt.GetType()}), w obiekcie który nie jest t³umaczony (typ: {pojemnik.GetType()}). Brak podanego jêzyka do ustawienia.");
                }
                ((IPoleJezyk) obiekt).JezykId = jezyk.Id;
            }

            object klucz= pojemnik.PobierzKlucz();

            try
            {
                typ.PobierzRefleksja()[obiekt,"Id"] = klucz;
            }
            catch(Exception e)
            {
                throw new Exception("Tworzona kontrolka nie ma pola id definiuj¹cego klucz", e);
            }
        
            Dictionary<string, object> pars = PobierzParametry(pojemnik);

            UstawWartoscPol(obiekt, pars);
            Dictionary<string, object> tlumaczenia = pojemnik.ParametryLokalizowane();
            if (tlumaczenia != null && tlumaczenia.Any())
            {
                UstawWartoscPol(obiekt, tlumaczenia);
            }
            return obiekt;
        }

        public static Dictionary<string, object> PobierzParametry(this IObiektPrzechowujacyKontrolke pojemnik)
        {
            var parss = pojemnik.ParametrySerializowane();
            return !string.IsNullOrEmpty(parss) ? JSonHelper.Deserialize<Dictionary<string, object>>(parss) : new Dictionary<string, object>();
        }

        public static void UstawWartoscPol(this object o, Dictionary<string, object> wartosciPol)
        {
            if (wartosciPol == null || !wartosciPol.Any())
            {
                return;
            }

            var akcesor = o.GetType().PobierzRefleksja();
            var slwonikPropertisow = o.GetType().Properties();

            foreach (var p in wartosciPol)
            {
                try
                {
                    if (!slwonikPropertisow.TryGetValue(p.Key, out PropertyInfo property) || property.GetSetMethod()==null)
                    {
                        continue;
                    }
                    object pokonwersji = PobierzWartosc(p.Value, property.PropertyType);
                    akcesor[o, p.Key] = pokonwersji;
                } catch (Exception e)
                {
#if DEBUG
                    try
                    {
                        throw new Exception($"Nie uda³o siê ustawiæ refleksj¹ pola: {p.Key} dla obiektu o typie: {o.GetType().Name} na wartoœæ: {p.Value}. B³¹d: {e.Message}");
                    }
                    catch { }
#endif
                    //udajemy ze jest wszystko ok - nie mozemy wyalic bledu bo czasem tak jest ze probojemy wpisac pole kteorgo juz nie ma w kontrolce np.
                }
            }
        }

        public static string OpisObiektu(this object o)
        {
            StringBuilder sb=new StringBuilder();
            var pola = o.GetType().GetProperties();
            foreach (var p in pola)
            {
                sb.Append($"Pole {p.Name} wartosc {p.GetValue(o)},");
            }
            return sb.ToString().Trim(',');
        }

        public static List<ParametryPola> DodajPolaZGrupami(List<ParametryPola> pola)
        {
            List<ParametryPola> tempLista = new List<ParametryPola>();
            foreach (ParametryPola atr in pola)
            {
                //opcjonalne pole Grupy - jesli obecny ma grupe, poprzedni ma uzupelniona grupe i sa to inne grupy
                if (!string.IsNullOrEmpty(atr.Grupa))
                {
                    ParametryPola elementPoprzedni = tempLista.LastOrDefault();
                    if (elementPoprzedni == null)
                    {
                        tempLista.Add(new ParametryPola(atr.Grupa, "grupa" + atr.Grupa, "Grupa", false, null));
                    }
                    else
                    {
                        if (elementPoprzedni.Grupa != atr.Grupa)
                        {
                            tempLista.Add(new ParametryPola(atr.Grupa, "grupa" + atr.Grupa, "Grupa", false, null));
                        }
                    }
                }
                tempLista.Add(atr);
            }
            return tempLista;
        } 


        public static List<ParametryPola> WygenerujParametryPol<T>(string[] listaPolWymaganych = null)
        {
            Type typ = typeof(T);
            return WygenerujParametryPol(typ, listaPolWymaganych);
        }

        /// <summary>
        /// generuje parametry pol dla konkretnego obiektu z wartoœciami z tego obiektu
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static List<ParametryPola> WygenerujParametryPol(object o)
        {
            Type typ = o.GetType();
            List<ParametryPola> pola = WygenerujParametryPol(typ);
            var akcesor = typ.PobierzRefleksja();
            foreach (PropertyInfo p in typ.Properties().Values)
            {
                var pole = pola.FirstOrDefault(x => x.Nazwa == p.Name);
                if (pole != null)
                {
                    pole.Wartosc = akcesor[o, p.Name];
                }
            }
            return pola;
        }

        public static List<ParametryPola> WygenerujParametryPol(Type typ, string[] listaPolWymaganych = null)
        {
            List<ParametryPola> wynik = new List<ParametryPola>();
            foreach (PropertyInfo p in typ.Properties().Values)
            {
                string wyswietlanaNazwa = p.Name;
                Dictionary<string, object> slownikWartosci = null;
                PoleEdytowane atr = p.GetCustomAttribute<PoleEdytowane>();
                if (atr == null)
                {
                    continue;
                }

                string typpola = atr.TypKontrolki;
                if (string.IsNullOrEmpty(typpola))
                {
                    Type t = p.PropertyType.PobierzPodstawowyTyp();
                    typpola = t.Name;
                }
                //Musimy podpi¹c opcje do wyboru
                if (typpola == "DropDown")
                {
                    PobieranieSlownika atrSlownik = p.GetCustomAttribute<PobieranieSlownika>();
                    if (atrSlownik == null)
                    {
                        throw new Exception($"Pole: {p.Name} ma wybrana formê dropdowana jednak nie ma ustawionego s³ownika wartoœci.");
                    }
                    slownikWartosci = ((ISlownik)Activator.CreateInstance(atrSlownik.Typ)).PobierzWartosci();
                }
                FriendlyNameAttribute fajnanazwa = p.GetCustomAttribute<FriendlyNameAttribute>();
                if (fajnanazwa != null)
                {
                    wyswietlanaNazwa = fajnanazwa.FriendlyName;
                }
                bool wymagane = false;
                wymagane = listaPolWymaganych?.Contains(p.Name) ?? atr.Wymagane;

                ParametryPola pp = new ParametryPola(p.Name, wyswietlanaNazwa, typpola, wymagane, atr.Grupa);
                pp.SlownikWartosci = slownikWartosci;
                wynik.Add(pp);
            }
            return wynik;
        }

        private static  log4net.ILog Log
        {
            get
            {
                return log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            }
        }

        public static List<Type> PobierzListeKlasZAtrybutem<T>() where T:Attribute
        {
            List<Type> result = new List<Type>();
            foreach (var a in typypoassembly)
            {
                try
                {
                    foreach (Type t in a.Value)
                    {
                        if (t.IsClass)
                        {
                            bool add = t.GetCustomAttribute<T>() != null;
                            if (add) result.Add(t);
                        }
                    }
                }
                catch (Exception){}
            }
            return result;
         
        }

        private static ConcurrentDictionary<string,IList<Type>> typypoassembly=new ConcurrentDictionary<string, IList<Type>>();

        /// <summary>
        /// laduje assembyl jakei sa aktuanie w solucji - pozniej w tych asemblach sa robione wszystkie szukania obiektow / metod itp.
        /// </summary>
        public static void ZaladujAssembly()
        {
            foreach (Assembly a in BuildManager.GetReferencedAssemblies())
            {
                if (a.FullName.IndexOf("solex", StringComparison.InvariantCultureIgnoreCase)<0)
                {
                    continue;
                }
                if (!typypoassembly.ContainsKey(a.FullName))
                {
                    try
                    {
                        typypoassembly.TryAdd(a.FullName, a.GetTypes());
                    }
                    catch (Exception ex)
                    {
                        Log.Error("B³¹d ³adowania assembly", ex);
                    }
                }
            }
        }

        public static List<Type> PobierzListeKlasDziedziczacychPoKlasieBazowej(Type klasaBazowa, string namespac = null)
        {
            return PobierzListeKlasDziedziczacychPoKlasieBazowej(new List<Type> {klasaBazowa}, namespac);
        }

        /// <summary>
        /// Wyszukuje klasy dziedzicz¹ce po obreœlonych typach
        /// </summary>
        /// <param name="types">Lista typów po których ma dziedziczyæ dana klasa</param>
        /// <param name="namespac"></param>
        /// <returns></returns>
        public static List<Type> PobierzListeKlasDziedziczacychPoKlasieBazowej(List<Type> types, string namespac = null)
        {
      
            List<Type> result = new List<Type>();
            foreach (var a in typypoassembly)
            {
                try
                {
                    foreach (Type t in a.Value)
                    {
                        if (t.IsClass && (string.IsNullOrEmpty(namespac) || t.Namespace == namespac))
                        {
                            bool add = true;
                            foreach (Type req in types)
                            {
                                if (!t.InheritsOrImplements(req))    //bartek - wczesniej bylo InheritsOrImplements z fastreflecta
                                {
                                    add = false;
                                    break;
                                }
                            }
                            if (add) result.Add(t);
                        }
                    }
                }
                catch (Exception){}
            }
            return result;
        }

      public static Type PobierzPodstawowyTyp(this Type t)
        {
            Type tkol = t;
            if (t.IsArray)
            {
                tkol = t.GetElementType();
            }
            if (tkol.IsGenericType)
            {
                tkol = tkol.GetGenericArguments()[0];
            }


            if(tkol.IsNullableType())
            {
                tkol = Nullable.GetUnderlyingType(tkol);
            }

            return tkol;
        }

      public static string OpisWartosci(object p)
      {
          if (p == null)
          {
              return "";
          }
         StringBuilder sb=new StringBuilder();
          if (p is IEnumerable && !(p is string))
          {
              foreach (var w in (IEnumerable) p)
              {
                  sb.AppendFormat("{0}, ", OpisWartosci(w));
              }
          }
          else
          {
              sb.AppendFormat("{0}, ",p);
          }
          return sb.ToString().Trim().Trim(',');
      }

        /// <summary>
        /// Pobiera ³¹dn¹ nazwê (FriendlyName) dla wartoœci enuma jesli jest
        /// </summary>
        /// <param name="wartosc">wartosc dla której chcemy pobraæ ³adn¹ nazwê</param>
        /// <param name="typ">Typ enuma</param>
        /// <returns></returns>
        public static string PobierzLadnaWartoscEnuma(object wartosc, Type typ)
        {
            if (typ == null)
            {
                return wartosc.ToString();
            }
            List<string> wartosclista = new List<string>();
            if (wartosc is IEnumerable)
            {
                foreach (object o in (IEnumerable) wartosc)
                {
                    string w = o.ToString();
                    var friendlyName = typ.PobierzPodstawowyTyp().GetMember(o.ToString())[0].GetCustomAttribute<FriendlyNameAttribute>();
                    if (friendlyName != null)
                    {
                        w = friendlyName.ToString();
                    }
                    wartosclista.Add(w);
                }
                return string.Join(",", wartosclista);
            }
            var mem = typ.PobierzPodstawowyTyp().GetMember(wartosc.ToString())[0].GetCustomAttribute<FriendlyNameAttribute>();
            string n = wartosc.ToString();
            if (mem != null)
            {
                n = mem.FriendlyName;
            }
            return n;
        }

        public static object PobierzWartosc(object wartosc,Type typ)
        {
            if (typ == null)
            {
                throw new NullReferenceException("TypPrzechowywanejWartosci jest nullem");
            }
            if (wartosc == null)
            {
                return null;
            }
            Type bazowy = typ.PobierzPodstawowyTyp();
            Type typwartosc = wartosc.GetType();
       
            if (typ == typwartosc)
            {
                return wartosc;
            }
            List<string> wartosclista = new List<string>();
            if (wartosc is IEnumerable && wartosc.GetType()!=typeof(string))
            {
                foreach (object o in (IEnumerable) wartosc)
                {
                    if (o != null)
                    {
                        wartosclista.Add(HttpContext.Current == null
                            ? o.ToString()
                            : HttpUtility.HtmlDecode(o.ToString()));
                    }
                }
            }
            else
            {
                wartosclista.Add(HttpContext.Current == null
                    ? wartosc.ToString()
                    : HttpContext.Current.Server.HtmlDecode(wartosc.ToString()));
            }

            if (typ.IsArray)
            {
                int ilosc = wartosclista.Count(x => !string.IsNullOrEmpty(x));
                Array tablica = Array.CreateInstance(bazowy, ilosc);
                int i = 0;
                foreach (string w in wartosclista)
                {
                    if (!string.IsNullOrEmpty(w))
                    {
                        tablica.SetValue(w.KonwertujWartosc(bazowy), i);
                        i++;
                    }
                }
                return tablica;
            }

            Type typlistageneryczna = typeof(List<>);
            if (typ.IsGenericType&& typ.GetGenericTypeDefinition() == typlistageneryczna)
            {
                Type typlisty = typlistageneryczna.MakeGenericType(new[] { bazowy });
                object lista = Activator.CreateInstance(typlisty);
                MethodInfo metodaadd = lista.GetType().GetMethod("Add");//.Invoke(Result, new[] { objTemp });
                foreach (string w in wartosclista)
                {
                    if (!string.IsNullOrEmpty(w))
                    {
                        metodaadd.Invoke(lista, new[] { w.KonwertujWartosc(bazowy) });
                    }
                }
                return lista;
            }

            typlistageneryczna = typeof(HashSet<>);
            if (typ.IsGenericType && typ.GetGenericTypeDefinition() == typlistageneryczna)
            {
                Type typlisty = typlistageneryczna.MakeGenericType(new[] { bazowy });
                object lista = Activator.CreateInstance(typlisty);
                MethodInfo metodaadd = lista.GetType().GetMethod("Add");//.Invoke(Result, new[] { objTemp });
                foreach (string w in wartosclista)
                {
                    if (!string.IsNullOrEmpty(w))
                    {
                        metodaadd.Invoke(lista, new[] { w.KonwertujWartosc(bazowy) });
                    }
                }
                return lista;
            }
            

            if (typwartosc.IsArray && bazowy == typeof(string))
            {
                if (wartosclista.Any())
                {
                    return string.Join(";", wartosclista);
                }
                return null;
            }

            return wartosclista.FirstOrDefault(x => !string.IsNullOrEmpty(x)).KonwertujWartosc(bazowy);
        }

        public static object KonwertujWartosc(this object wartosc, Type real)
        {
            const string komunikatBledu = "Nie uda³o siê ustawiæ wartoœci '{0}' dla typu {1}";
            if (wartosc == null)
            {
                return null;
            }
            if (real == wartosc.GetType()) //ten sam typ wiêc nie ma problemów
            {
                return wartosc;
            }
            if (real == typeof (object))
            {
                return wartosc;
            }
            if (real.IsEnum)
            {
                if (Enum.IsDefined(real, wartosc.ToString()))
                {
                    return Enum.Parse(real, wartosc.ToString());
                }
                try
                {
                    return Enum.ToObject(real, int.Parse(wartosc.ToString()));
                } catch (Exception e)
                {
                    Log.Error($"W typie enum o nazwie: {real} nie uda³o siê dopasowaæ wartoœci:  {wartosc}. Komunikat: {e.Message}");
                }
                return null;
            }

            if (real == typeof(DateTime))
            {
                DateTime data;
                if (TextHelper.PobierzInstancje.SprobojSparsowac(wartosc.ToString().Replace("-", "."), out data))
                {
                    return data;
                }
                throw new Exception(string.Format(komunikatBledu, wartosc, "dateTime"));
            }
            if (real == typeof(int))
            {
                int i;
                if (int.TryParse(wartosc.ToString(), out i))
                {
                    return i;
                }
                throw new Exception(string.Format(komunikatBledu, wartosc, "int"));
            }
            if (real == typeof(long))
            {
                long i;
                if (long.TryParse(wartosc.ToString(), out i))
                {
                    return i;
                }
                throw new Exception(string.Format(komunikatBledu, wartosc, "long"));
            }
            if (real == typeof(bool))
            {
                bool b;
                if (PrzetworzBool(wartosc.ToString(), out b))
                {
                    return b;
                }
                throw new Exception(string.Format(komunikatBledu, wartosc, "bool"));
            }
            if (real == typeof(decimal))
            {
                decimal d;
                if (TextHelper.PobierzInstancje.SprobojSparsowac(wartosc.ToString(), out d))
                {
                    return d;
                }
                throw new Exception(string.Format(komunikatBledu, wartosc, "decimal"));
            }
            if (real == typeof(string))
            {
                return wartosc.ToString();
            }
            if (real == typeof(char))
            {
                return wartosc.ToString()[0];
            }
            if (real == typeof (WidocznosciTypow))
            {
                if (!String.IsNullOrEmpty(wartosc.ToString()))
                {
                    return JSonHelper.Deserialize<WidocznosciTypow>(wartosc.ToString());
                }
                return null;
            }
            if (real == typeof(AdresUrl))
            {
                string wart = wartosc.ToString();
                if (!string.IsNullOrEmpty(wart))
                {
                    if (wart.StartsWith("{"))
                    {
                        return JSonHelper.Deserialize<AdresUrl>(wart);
                    }
                    return new AdresUrl(wart);
                }
                return null;
            }
            throw new InvalidOperationException("Nieznana konwersja");
        }

        public static string TekstTakNie(this bool b)
        {
            return (b == true) ? "TAK" : "NIE";
        }

        public static bool PrzetworzBool(string wartosc, out bool b)
        {
            wartosc = wartosc.ToLower();

            if (wartosc == "tak" || wartosc == "yes" || wartosc == "t" || wartosc == "y" || wartosc == "1" || wartosc == "true")
            {
                b = true;
                return true;
            }

            if (wartosc == "nie" || wartosc == "no" || wartosc == "n" || wartosc == "f" || wartosc == "0" || wartosc == "false")
            {
                b = false;
                return true;
            }

            return bool.TryParse(wartosc, out b);
        }

        public static Type PobierzBazowy(this Type t)
        {
            if (t == null) throw new NullReferenceException();
            if (t.BaseType == typeof (object))
            {
                return t;
            }
            return t.BaseType.PobierzBazowy();
        }

        public static bool IsDerivativeOf(this Type t, Type typeToCompare)
        {
            if (t == null) throw new NullReferenceException();
           
            if (t.BaseType == null) return false;
            if (t.IsGenericType)
            {
                if (t.GetGenericTypeDefinition() == typeToCompare) return true;
            }
            if (t.BaseType == typeToCompare) return true;
            return t.BaseType.IsDerivativeOf(typeToCompare);
        }
     
        public static object PobierzWartosc(object obiekt, string pole)
        {
            if (String.IsNullOrEmpty(pole))
            {
                throw new InvalidOperationException();
            }

            if (!pole.Contains(".") )
            {
                throw  new Exception("Metoda jest tylko dla pobieranie zagnie¿dzonych propertisow - zwykle pobieraj akcesorem refleksji.");
            }

            string[] sciezka = pole.Split('.');
            object dane = obiekt;
            foreach (var s in sciezka)
            {
                TypeAccessor akcesor = dane.GetType().PobierzRefleksja();   //todo: optymazliacja wywolania
                dane = akcesor[dane, s];
                if (dane == null)
                {
                    return null;
                }
            }
            return dane;
        }

        public static Dictionary<string, PropertyInfo> _cachePol = new Dictionary<string, PropertyInfo>();

        public static object lokPropertis = new object();

        /// <summary>
        /// szuka proeprtisy zagniezdzne z kropkami itp
        /// </summary>
        /// <param name="type"></param>
        /// <param name="pole"></param>
        /// <returns></returns>
        public static PropertyInfo ZnajdzPropertis(Type type, string pole)
        {
            if (String.IsNullOrEmpty(pole)) throw new InvalidOperationException();

            if (!pole.Contains("."))
            {
                throw new Exception("Metoda jest tylko dla pobieranie zagnie¿dzonych propertisow - zwykle pobieraj akcesorem refleksji.");
            }

            string klucz = type.FullName + "_" + pole;
            PropertyInfo wynik = null;
            lock (lokPropertis)
            {
                if (!_cachePol.TryGetValue(klucz, out wynik))
                {
                    string[] sciezka = pole.Split('.');
                    Type dane = type;
                    foreach (var s in sciezka)
                    {
                        if (!dane.Properties().TryGetValue(s, out wynik))
                        {
                            throw new Exception($"Brak propertisa o nazwie: {s} w typie: {dane.Name}");
                        }

                        dane = wynik.PropertyType;
                    }
                    _cachePol.Add(klucz, wynik);
                }
            }
            return wynik;
        }

        public static IList StworzListeGeneryczna(Type typObiektow, object[] obiektyDoListy)
        {
            var genericListType = typeof(List<>);
            var specificListType = genericListType.MakeGenericType(typObiektow);
            var list = (IList)Activator.CreateInstance(specificListType);
            foreach (var o in obiektyDoListy)
            {
                list.Add(Convert.ChangeType(o, typObiektow));
            }
            return list;
        }

        /// <summary>
        /// Pobiera propertisy, zwraca poprawne wyniki gdy interfejs dziedziczy po interfejsie
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PropertyInfo[] GetPublicProperties(this Type type)
        {
            if (type.IsInterface)
            {
                var propertyInfos = new List<PropertyInfo>();

                var considered = new List<Type>();
                var queue = new Queue<Type>();
                considered.Add(type);
                queue.Enqueue(type);
                while (queue.Count > 0)
                {
                    var subType = queue.Dequeue();
                    foreach (var subInterface in subType.GetInterfaces())
                    {
                        if (considered.Contains(subInterface)) continue;

                        considered.Add(subInterface);
                        queue.Enqueue(subInterface);
                    }

                    PropertyInfo[] typeProperties = null;
                    typeProperties = subType.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);
                  
                    var newPropertyInfos = typeProperties.Where(x => !propertyInfos.Contains(x));

                    propertyInfos.InsertRange(0, newPropertyInfos);
                }

                return propertyInfos.ToArray();
            }

            return type.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);
        }

        public static T PobierzAtrybutDlaEnuma<T>(this Enum enumVal) where T : System.Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return (attributes.Length > 0) ? (T)attributes[0] : null;
        }


        public static string PobierzOpisTypu(this Type e)
        {
            return string.Format("{0},{1}", e.FullName, e.Assembly.GetName().Name);
        }
    }
}
