using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ServiceStack.Text;
using SolEx.Hurt.Model.AtrybutyKlas;
using ILog = log4net.ILog;

namespace SolEx.Hurt.Model.Helpers
{
    public static class TypeExtensions
    {
        private static  ILog Log
        {
            get
            {
                return log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            }
        }

        /// <summary>
        /// Tworzy unikalne ID obiektu na podstawie jego pól, które nie są nullami. Przydatne przy tworzeniu nowych cech itd. Zwykłe GetHashCode na obiekcie dawało różne hashe ale kiedy brało się sklejkę do stringa z każdego pola obiektu to 
        /// GetHashCode ze sklejki zwracał za każdym razem unikalny identyfikator.
        /// </summary>
        /// <param name="obiekt">Aktualny obiekt, nie podaje się jako parametr bo to jest extension</param>
        /// <param name="ujemna">Czy ID ma być ujemne - żeby było wiadomo, że np cecha była dodana ręcznie</param>
        /// <returns></returns>
        [Obsolete("Nie korzystać, WygenerujIDObiektuSHA ")]
        public static int WygenerujIDObiektu(this object obiekt, bool ujemna = false)
        {
            string dane = "";

            if (obiekt is string)
                dane = (string) obiekt;

            else
            {
                Type myType = obiekt.GetType();
                var props = myType.Properties().Values;
                var akcesor = myType.PobierzRefleksja();
                foreach (PropertyInfo prop in props)
                {
                    if (prop.CanRead)
                    {
                        try
                        {
                            object propValue = akcesor[obiekt, prop.Name];
                            if (propValue != null)
                                dane += propValue.ToString();
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex);
                        }
                    }
                }
            }

           int id = Math.Abs(dane.Trim().GetHashCode());
          
           if (ujemna)
                id = -id;

            return id;
        }

        private static System.Security.Cryptography.SHA512 providerHash = System.Security.Cryptography.SHA512.Create();

        private static int WygenerujKluczSHA(string ciag)
        {
            byte[] b = null;
            lock (providerHash)
            {
                b = providerHash.ComputeHash(Encoding.UTF8.GetBytes(ciag));
            }
            int value = BitConverter.ToInt32(b, 0);
            return value;
        }

        public static long WygenerujKluczSHALong(string ciag)
        {
            byte[] utf = Encoding.UTF8.GetBytes(ciag);
            byte[] b = null;
            //musi tu byc lock bo ComputeHash nie jest threadSafe
            lock (providerHash)
            {
                b = providerHash.ComputeHash(utf);
            }
            long value = BitConverter.ToInt64(b, 0);

            return value;
        }
    

        public static long WygenerujIDObiektuSHAWersjaLong(this object obiekt, int UstawZnak = 0)
        {
            if (obiekt == null)
            {
                return 0;
            }
            string dane = ZbudujKluczStringDlaIDSHA(obiekt);
            long id = WygenerujKluczSHALong(dane);
            if (UstawZnak > 0)
            {
                id = Math.Abs(id);
            }
            else
            {
                if (UstawZnak < 0 && id > 0)
                {
                    //czyli minusowe
                    id = -id;
                }
            }

            return id;
        }

        
        /// <summary>
        /// Lepiej liczy id
        /// </summary>
        /// <param name="obiekt"></param>
        /// <param name="UstawZnak">jesli -1 to liczba na minus, jesli 1 to na plusie</param>
        /// <returns></returns>
        [Obsolete("Korzystać z wersji LONG")]
        public static int WygenerujIDObiektuSHA(this object obiekt, int UstawZnak = 0)
        {
            string dane = ZbudujKluczStringDlaIDSHA(obiekt);
            int id = WygenerujKluczSHA(dane);

            if (UstawZnak > 0)
            {
                id = Math.Abs(id);
            }
            else
            {
                if (UstawZnak <0 && id >0)
                {
                    //czyli minusowe
                    id = -id;
                }
            }
            
            return id;

        }

        public static string ZbudujKluczStringDlaIDSHA(object obiekt)
        {
            string dane = "";

            if (obiekt is string)
                dane = ((string)obiekt);
            else

            if (obiekt is StringBuilder)
                dane = (obiekt as StringBuilder).ToString();

            else
            {
                Type myType = obiekt.GetType();
                var props = myType.Properties().Values;
                var akcesor = myType.PobierzRefleksja();
                foreach (PropertyInfo prop in props)
                {
                    if (prop.CanRead)
                    {
                        try
                        {
                            object propValue = akcesor[obiekt, prop.Name];
                            if (propValue != null)
                            {
                                dane += "|" + propValue + "|";
                            }
                            else
                            {
                                dane += "|null|";
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"Błąd pobierania pola: { prop.Name} dla obiektu: {obiekt}, dane: {dane}", ex);
                        }
                    }
                }
            }
            return dane.Trim();
        }

        public static X PobierzAtrybutEnuma<T, X>(this Enum enumeration, Func<T, X> expression) where T : Attribute
        {
            T attribute = enumeration.GetType().GetMember(enumeration.ToString())[0].GetCustomAttributes(typeof(T), false).Cast<T>().SingleOrDefault();

            if (attribute == null)
                return default(X);

            return expression(attribute);
        }
        public static X PobierzAtrybutPola<T, X>(this MemberInfo enumeration, Func<T, X> expression) where T : Attribute
        {
            T attribute = enumeration.GetCustomAttributes(typeof(T), false).Cast<T>().SingleOrDefault();

            if (attribute == null)
                return default(X);

            return expression(attribute);
        }

        public static void UstawWartoscPolaObiektu(this object obiekt, string pole, object wartosc)
        {
            string wartoscstring = wartosc.ToString();
            PropertyInfo[] propertisy = obiekt.GetType().GetProperties();
            foreach (var p in propertisy)
            {
                string nazwa = p.Name;
                var atribut =
                    p.GetCustomAttributes(true)
                     .FirstOrDefault(a => a.GetType() == typeof(FriendlyNameAttribute)) as
                    FriendlyNameAttribute;
                if (atribut != null)
                {
                    nazwa = atribut.FriendlyName;
                }

                if (pole == nazwa || pole == p.Name)
                {
                  
                        if (p.PropertyType == typeof(decimal))
                        {
                            decimal liczba = 0;
                            if (TextHelper.PobierzInstancje.SprobojSparsowac(wartoscstring.Trim(), out liczba))
                            {
                                p.SetValue(obiekt, liczba, null);
                            }
                        }
                        else p.SetValue(obiekt, wartoscstring.Trim(), null);
                }
            }
        }

        public static object PobierzWartoscPolaObiektu(this object obiekt, string pole, bool zwracajNull = true)
        {
            object wynik = null;
            PropertyInfo[] propertisy = obiekt.GetType().GetProperties();
            foreach (var p in propertisy)
            {
                string nazwa = p.Name;
                var atribut =
                    p.GetCustomAttributes(true)
                     .FirstOrDefault(a => a.GetType() == typeof(FriendlyNameAttribute)) as
                    FriendlyNameAttribute;
                if (atribut != null)
                {
                    nazwa = atribut.FriendlyName;
                }

                if (pole == nazwa || pole == p.Name)
                {
                    wynik = p.GetValue(obiekt, null);
                    break;
                }
            }

            //potrzebne w linq żeby nie robić podwójego sprawdzenia czy to jest nullem a potem ponowne wykonanie i sprawdzenie właściwej wartości jeśli jest != null
            if (!zwracajNull && wynik == null)
                return string.Empty;

            return wynik;
        }



        public static Type[] PobierzTypyWNamespace(Assembly assembly, string nameSpace)
        {
            var typy = assembly.GetTypes().Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)).ToArray();
            return typy;
        }


        
    }
}
