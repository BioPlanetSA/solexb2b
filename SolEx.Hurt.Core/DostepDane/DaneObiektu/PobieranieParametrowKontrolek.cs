using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Extensions;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.DostepDane.DaneObiektu
{
    public static class PobieranieParametrowKontrolek
    {
        /// <summary>
        /// Buduje slownik na podstawie enuma
        /// </summary>
        /// <param name="typ">Typ enuma</param>
        /// <returns></returns>
        public static Dictionary<string, object> PobierzSlownikEnum(Type typ)
        {
            Dictionary<string, object> slownik = SolexBllCalosc.PobierzInstancje.Cache.PobierzObiekt<Dictionary<string, object>>(cacheKlucz, typ);

            if (slownik == null)
            {
                slownik = new Dictionary<string, object>();
                foreach (var e in Enum.GetValues(typ))
                {
                    string n = Refleksja.PobierzLadnaWartoscEnuma(e, typ);
                    if (!string.IsNullOrEmpty(n))
                    {
                        try
                        {
                            slownik.Add(n, e.ToString());
                        }
                        catch (Exception ex)
                        {
                            throw  new Exception("Błąd dodawania do słownika klucza: " + n + " i wartości: " + e, ex );
                        }
                    }
                }
                slownik = slownik.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

                //maly cache - kilka sekund tylko bo wartości mogą się szybko zmieniać
                SolexBllCalosc.PobierzInstancje.Cache.DodajObiekt(cacheKlucz, slownik, typ);
            }
            return slownik;
        }

        /// <summary>
        /// Buduje slownik na podstawie enuma z tłumaczeniem
        /// </summary>
        /// <param name="typ">Typ enuma</param>
        /// <param name="jezykId">Język tłumaczenia</param>
        /// <returns></returns>
        public static Dictionary<string, object> PobierzSlownikEnumTlumaczony(Type typ, int jezykId)
        {
            Dictionary<string, object> slownik = SolexBllCalosc.PobierzInstancje.Cache.PobierzObiekt<Dictionary<string, object>>(cacheKluczTl, typ);

            if (slownik == null)
            {
                slownik = new Dictionary<string, object>();
                foreach (var e in Enum.GetValues(typ))
                {
                    string n = SolexBllCalosc.PobierzInstancje.Konfiguracja.PobierzTlumaczenie(jezykId, Refleksja.PobierzLadnaWartoscEnuma(e, typ));
                    if (!string.IsNullOrEmpty(n))
                    {
                        try
                        {
                            slownik.Add(n, ((int)e).ToString());
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Błąd dodawania do słownika klucza: " + n + " i wartości: " + e, ex);
                        }
                    }
                }
                slownik = slownik.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

                //maly cache - kilka sekund tylko bo wartości mogą się szybko zmieniać
                SolexBllCalosc.PobierzInstancje.Cache.DodajObiekt(cacheKluczTl, slownik, typ);
            }
            return slownik;
        }

        /// <summary>
        /// Buduje slownik na podstawie pól obiektu
        /// </summary>
        /// <param name="typ">Typ obiektu którego propertisy mają być wartościami słownika</param>
        /// <param name="czyWszystkie">Czy pobieramy wszystkie propertisy</param>
        /// <returns></returns>
        public static Dictionary<string, object> PobierzSlownikPolaObiektu(Type typ, bool czyWszystkie = false)
        {
            Dictionary<string, object> slownik = SolexBllCalosc.PobierzInstancje.Cache.PobierzObiekt<Dictionary<string, object>>(cacheKlucz2, typ, czyWszystkie);

            if (slownik == null)
            {
                slownik = new Dictionary<string, object>();
                PropertyInfo[] propertisy = typ.GetProperties();
                foreach (PropertyInfo prop in propertisy)
                {
                    if (czyWszystkie && prop.GetGetMethod(true) != null)
                    {
                        slownik.Add(prop.Name, prop.Name);
                    }
                    if (!czyWszystkie && prop.GetSetMethod(true) != null)
                    {

                        slownik.Add(prop.Name, prop.Name);
                    }
                }
                slownik = slownik.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

                //maly cache - kilka sekund tylko bo wartości mogą się szybko zmieniać
                SolexBllCalosc.PobierzInstancje.Cache.DodajObiekt(cacheKlucz2, slownik, typ, czyWszystkie);
            }
            return slownik;
        }

        private static string cacheKlucz = "slownik_{0}";
        private static string cacheKluczTl = "slownikTl_{0}";
        private static string cacheKlucz2 = "slownik_{0}_{1}";

        /// <summary>
        /// Buduje slownik na podstawie słownika typu ISlownik
        /// </summary>
        /// <param name="typ">Typ słownika dziedziczący po ISlownik</param>
        /// <returns></returns>
        public static Dictionary<string, object> PobierzSlownik(Type typ)
        {
            Dictionary<string, object> slownik =  SolexBllCalosc.PobierzInstancje.Cache.PobierzObiekt<Dictionary<string, object>>(cacheKlucz, typ);

            if (slownik == null)
            {
                slownik = new Dictionary<string, object>();
                ISlownik pobieralnie = (ISlownik) Activator.CreateInstance(typ);
                foreach (var w in pobieralnie.PobierzWartosci())
                {
                    slownik.Add(w.Key, w.Value);
                }
                slownik = slownik.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

                //maly cache - kilka sekund tylko bo wartości mogą się szybko zmieniać
                SolexBllCalosc.PobierzInstancje.Cache.DodajObiekt( cacheKlucz, slownik, typ);
            }

            return slownik;
        }
        /// <summary>
        /// Buduje slownik na podstawie wartości w parametrze
        /// </summary>
        /// <param name="pola">Wartości słownika oddzielone średnikiem</param>
        /// <returns></returns>
        public static Dictionary<string, object> PobierzSlownik(string pola)
        {
            Dictionary<string, object> slownik = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(pola))
            {
                slownik = pola.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToDictionary(x => x, x => (object)x);
            }
            return slownik.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        }
        /// <summary>
        /// Tworzy słownik dopuszczalny wartości
        /// </summary>
        /// <param name="propertis">Pole na podstawie którego tworzymy słownik</param>
        /// <param name="typStlownika">Typ słownika</param>
        /// <returns></returns>
        public static Dictionary<string, object> ZbudujSlownikDlaPola(this PropertyInfo propertis, out Type typStlownika)
        {
            Type typ = propertis.PropertyType;
            if (typ.IsNullableType())
            {
                typ = typ.PobierzPodstawowyTyp();
            }
            if (propertis.PropertyType.IsArray || propertis.IsList())
            {
                if (propertis.PropertyType.GenericTypeArguments.Any() && propertis.PropertyType.GenericTypeArguments.Length==1)
                {
                    typ = propertis.PropertyType.GenericTypeArguments.Single();
                }
                else
                {
                    typ = propertis.PropertyType.GetElementType();
                }
                
            }
            typStlownika = null;
            if (typ!=null && typ.IsEnum )
            {
                return PobierzSlownikEnum(typ);
            }

            //czy typ ma atrybut mówiacy o tym ze slownik wartosci generowac 
            var atrybut = propertis.GetCustomAttributes(true).FirstOrDefault(a => a.GetType() == typeof(SyncSlownikNaPodstawieInnegoTypu)) as SyncSlownikNaPodstawieInnegoTypu;
            if (atrybut != null )
            {
                return PobierzSlownikPolaObiektu(atrybut.Typ, atrybut.PobierajWszystkie);
            }
            var atrybutSlownik = propertis.GetCustomAttributes(true).FirstOrDefault(a => a.GetType() == typeof(PobieranieSlownika)) as PobieranieSlownika;
            if (atrybutSlownik != null)
            {
                typStlownika = atrybutSlownik.Typ;
                return PobierzSlownik(atrybutSlownik.Typ);
            }
            return null;
        }
    }
}
