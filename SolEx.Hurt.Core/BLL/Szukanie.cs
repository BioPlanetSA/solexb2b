using ServiceStack.Common;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.Core;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Extensions;
using SolEx.Hurt.Model.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL
{
    public partial class Szukanie : LogikaBiznesBaza, ISzukanie
    {
        public Szukanie(ISolexBllCalosc calosc) : base(calosc)
        {
        }

        public virtual string OczyscFrazePrzygotujDoRegexa(string fraza)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var s in Calosc.Konfiguracja.SzumyWyszukiwania)
            {
                sb = sb.Replace(s.ToString(CultureInfo.InvariantCulture), "");
            }
            foreach (char c in fraza.ToLower())
            {
                if (Calosc.Konfiguracja.PodczasWyszukiwaniaZmienPolskeZnaki)
                {
                    switch (c)
                    {
                        case 'ą':
                            sb.Append("[aą]");
                            break;
                        case 'ę':
                            sb.Append("[eę]");
                            break;
                        case 'ó':
                            sb.Append("[óo]");
                            break;
                        case 'ł':
                            sb.Append("[łl]");
                            break;
                        case 'ń':
                            sb.Append("[nń]");
                            break;
                        case 'ć':
                            sb.Append("[cć]");
                            break;
                        case 'ś':
                            sb.Append("[śs]");
                            break;
                        case 'ź':
                            sb.Append("[zź]");
                            break;
                        case 'ż':
                            sb.Append("[żz]");
                            break;
                        case 'z':
                            sb.Append("[zźż]");
                            break;
                        case 'a':
                            sb.Append("[aą]");
                            break;
                        case 'c':
                            sb.Append("[cć]");
                            break;
                        case 'e':
                            sb.Append("[eę]");
                            break;
                        case 'o':
                            sb.Append("[óo]");
                            break;
                        case 'l':
                            sb.Append("[łl]");
                            break;
                        case 'n':
                            sb.Append("[nń]");
                            break;
                        case 's':
                            sb.Append("[śs]");
                            break;
                        default:
                            sb.Append(Regex.Escape(c.ToString()));
                            break;
                    }
                }
                else
                {
                    switch (c)
                    {
                        default:
                            sb.Append(Regex.Escape(c.ToString()));
                            break;
                    }
                }

                foreach (char s in Calosc.Konfiguracja.SzumyWyszukiwania)
                {
                    sb.Append("[" + s + "]*");
                }
            }
            return sb.ToString();
        }

        [ExcludeFromCodeCoverage]
        public List<Regex> PobierzWyszukiwanieRegex(string wyszukiwane)
        {
            if (String.IsNullOrEmpty(wyszukiwane)) return new List<Regex>();
            HashSet<string> frazy = new HashSet<string>( wyszukiwane.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries) );
            List<Regex> regexy = new List<Regex>();
            foreach (string s in frazy)
            {
                string f = OczyscFrazePrzygotujDoRegexa(s);
                Regex r = StworzRegexDlaFrazy(f);
                regexy.Add(r);
            }
            return regexy;
        }

        [ExcludeFromCodeCoverage]
        public Regex StworzRegexDlaFrazy(string fraza)
        {
            return new Regex(fraza, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        }

        private const string OznaczenieTypu = "m";

        private Expression GenerujFrazeSzukaniaStringa<T>(Expression fieldAccess, string fraza, PropertyInfo info,
            ParametrBindowaniaPobieraniaDanych<T> bindowanie)
        {
            if (!bindowanie.FiltrySql)
            {
                fraza = OczyscFrazePrzygotujDoRegexa(fraza);
                var pattern = Expression.Constant(fraza);
                var opcje = Expression.Constant(RegexOptions.IgnoreCase);
                var nienull = Expression.NotEqual(fieldAccess, Expression.Constant(null));
                Type typRegex = typeof(Regex);
                MethodInfo infoMetody = info.PropertyType.GetMethod("ToString", Type.EmptyTypes);
                MethodCallExpression toString = Expression.Call(fieldAccess, infoMetody);

                MethodCallExpression test = Expression.Call(typRegex, "IsMatch", null, toString, pattern, opcje);

                return Expression.AndAlso(nienull, test);
            }
            //dla filtrów SQL inny warunek
            MethodInfo method;
            ConstantExpression someValue;
         
            method = info.PropertyType.GetMethod("Contains", new[] {typeof(string)});
            someValue = Expression.Constant(fraza, typeof(string));
           
            return Expression.Call(fieldAccess, method, someValue);
           
        }

        private Expression GenerujFrazeSzukaniaEnum(Expression fieldAccess, string fraza, PropertyInfo info)
        {
            Type bazowy = info.PropertyType.IsNullableType() ? Nullable.GetUnderlyingType(info.PropertyType) : info.PropertyType;
            bool nulowany = info.PropertyType.IsNullableType();
            if (nulowany)
            {
                fieldAccess = Expression.Convert(fieldAccess, bazowy);
            }
            object wartosc;
            try
            {
                wartosc = Enum.Parse(bazowy, fraza);
            }
            catch (Exception)//wyjątek gdy nie da się convertować czyli na pweno nie będzie spełniony
            {
                Expression l = Expression.Constant(true);
                Expression p = Expression.Constant(false);
                return Expression.Equal(l, p);
            }
            Expression liczba = Expression.Constant(wartosc);
            Expression body = Expression.Equal(fieldAccess, liczba);
            return body;
        }

        private Expression GenerujFrazeSzukaniaKolekcjiLong(Expression fieldAccess, string fraza, PropertyInfo info)
        {
            long wartosc = long.Parse(fraza);
            ConstantExpression ce = Expression.Constant(wartosc);

            MethodInfo method = info.PropertyType.GetMethod("Contains", new[] { typeof(long) });
            MethodCallExpression call = Expression.Call(fieldAccess, method, ce);

            return call;
        }

        private Expression GenerujFrazeSzukaniaKolekcjiInt(Expression fieldAccess, string fraza, PropertyInfo info)
        {
            int wartosc = int.Parse(fraza);
            ConstantExpression ce = Expression.Constant(wartosc);

            //var call = Expression.Call(typeof(Enumerable), "Contains", new[] { fieldAccess.Type.PobierzPodstawowyTyp() }, fieldAccess, ce);
            MethodInfo method = info.PropertyType.GetMethod("Contains", new[] { typeof(int) });
            MethodCallExpression call = Expression.Call(fieldAccess, method,  ce);

            return call;
        }

        static bool IsIEnumerable(Type type)
        {
            return type.IsGenericType
                && type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        static Type GetIEnumerableImpl(Type type)
        {
            // Get IEnumerable implementation. Either type is IEnumerable<T> for some T, 
            // or it implements IEnumerable<T> for some T. We need to find the interface.
            if (IsIEnumerable(type))
                return type;
            Type[] t = type.FindInterfaces((m, o) => IsIEnumerable(m), null);
            return t[0];
        }
        static MethodBase GetGenericMethod(Type type, string name, Type[] typeArgs,
    Type[] argTypes, BindingFlags flags)
        {
            int typeArity = typeArgs.Length;
            var methods = type.GetMethods()
                .Where(m => m.Name == name)
                .Where(m => m.GetGenericArguments().Length == typeArity)
                .Select(m => m.MakeGenericMethod(typeArgs));

            return Type.DefaultBinder.SelectMethod(flags, methods.ToArray(), argTypes, null);
        }

        private Expression GenerujFrazeSzukanieListy<TDane>(Expression fieldAccess, string fraza, PropertyInfo info, ParametrBindowaniaPobieraniaDanych<TDane> bindowanie)
        {
            if (!bindowanie.FiltrySql)
            {
                Type cType = GetIEnumerableImpl(fieldAccess.Type);
                fieldAccess = Expression.Convert(fieldAccess, fieldAccess.Type);
                Type elemType = cType.GetGenericArguments()[0];
                Type predType = typeof(Func<,>).MakeGenericType(elemType, typeof(bool));

                ParameterExpression p = Expression.Parameter(elemType, "item");

                //tylko jesli typ jes nullowy to mozna to zrobic
                BinaryExpression nieNUll = null;
                if (info.GetType().IsNullableType())
                {
                    nieNUll = Expression.NotEqual(p, Expression.Constant(null));
                }

                var toString =  Expression.Call(p, typeof(object).GetMethod("ToString"));


                fraza = OczyscFrazePrzygotujDoRegexa(fraza);
                var pattern = Expression.Constant(fraza);
                var opcje = Expression.Constant(RegexOptions.IgnoreCase);
                Type typRegex = typeof(Regex);

                Delegate predicate = null;

                if (nieNUll != null)
                {
                    predicate = Expression.Lambda(Expression.AndAlso(nieNUll, Expression.Call(typRegex, "IsMatch", null, toString, pattern, opcje)), p).Compile();
                }
                else
                {
                    predicate = Expression.Lambda(Expression.Call(typRegex, "IsMatch", null, toString, pattern, opcje), p).Compile();
                }

                MethodInfo anyMethod = (MethodInfo)GetGenericMethod(typeof(Enumerable), "Any", new[] { elemType }, new[] { cType, predType }, BindingFlags.Static);

                var exp =  Expression.Call(anyMethod, fieldAccess, Expression.Constant(predicate));

                return exp;
            }

            //dla filtrów SQL inny warunek
            MethodInfo method = info.PropertyType.GetMethod("Contains", new[] {typeof(string)});
            var someValue = Expression.Constant(fraza, typeof(string));
            return Expression.Call(fieldAccess, method, someValue);

        }

        private Expression GenerujFrazeSzukaniaSlownika(Expression fieldAccess, string fraza, PropertyInfo info)
        {
            ConstantExpression pattern = Expression.Constant(fraza);
            ConstantExpression opcje = Expression.Constant(RegexOptions.IgnoreCase);
            BinaryExpression nienull = Expression.NotEqual(fieldAccess, Expression.Constant(null));
            Type typRegex = typeof(Regex);
            MethodInfo infoMetody = info.PropertyType.GetMethod("ContainsValue");
            MethodCallExpression toString = Expression.Call(fieldAccess, infoMetody);

            var test = Expression.Call(typRegex, "IsMatch", null, toString, pattern, opcje);

            return Expression.AndAlso(nienull, test);
        }

        private Expression GenerujFrazeSzukaniaBool(Expression fieldAccess, string fraza, PropertyInfo info)
        {
            Type bazowy = info.PropertyType.IsNullableType() ? Nullable.GetUnderlyingType(info.PropertyType) : info.PropertyType;
            bool nulowany = info.PropertyType.IsNullableType();
            if (nulowany)
            {
                fieldAccess = Expression.Convert(fieldAccess, bazowy);
            }
            object wartosc;
            try
            {
                wartosc = Convert.ChangeType(fraza, bazowy);
            }
            catch (Exception) //wyjątek gdy nie da się convertować czyli na pweno nie będzie spełniony
            {
                Expression l = Expression.Constant(true);
                Expression p = Expression.Constant(false);
                return Expression.Equal(l, p);
            }
            Expression liczba = Expression.Constant(wartosc);
            Expression body = Expression.Equal(fieldAccess, liczba);
            return body;
        }


        private Expression GenerujFrazeSzukaniaGuid(Expression fieldAccess, string fraza, PropertyInfo info)
        {
            Expression stalaSzukana = Expression.Constant(Guid.Parse(fraza) );
            Expression body = Expression.Equal(fieldAccess, stalaSzukana);
            return body;
        }

        private Expression GenerujFrazeSzukaniaLiczby(Expression fieldAccess, string fraza, PropertyInfo info)
        {
            Type bazowy = info.PropertyType.IsNullableType() ? Nullable.GetUnderlyingType(info.PropertyType) : info.PropertyType;
            bool nulowany = info.PropertyType.IsNullableType();
            object wartosc;
            try
            {
                wartosc = Convert.ChangeType(fraza, bazowy);
            }
            catch (Exception)//wyjątek gdy nie da się convertować czyli na pweno nie będzie spełniony
            {
                //paramrtry nie pasuja do siebie
                throw new Exception("Błąd parametrów - powinna być wartość typu: " + bazowy.Name + ". Podana wartość: " + fraza);
            }
            Expression body = null;
            Expression nullCheck = null;
            if (nulowany)
            {
                nullCheck = Expression.NotEqual(fieldAccess, Expression.Constant(null));
                fieldAccess = Expression.Convert(fieldAccess, bazowy);
            }
            Expression liczba = Expression.Constant(wartosc);

            body = Expression.Equal(fieldAccess, liczba);
            if (nullCheck != null)
            {
                return Expression.AndAlso(nullCheck,body);
            }
            return body;
        }

        private Expression GenerujFrazeSzukaniaDaty(Expression fieldAccess, string fraza, PropertyInfo info)
        {
            bool nulowany = info.PropertyType.IsNullableType();
            if (nulowany)
            {
                fieldAccess = Expression.Convert(fieldAccess, typeof(DateTime));
            }
            Expression body = null;
            string[] daty = fraza.Split(new[] { ';' });
            if (daty.Length == 1)
            {
                DateTime odKiedy;
                bool okod = TextHelper.PobierzInstancje.SprobojSparsowac(daty[0], out odKiedy);
                if (okod)
                {
                    var dataod = Expression.Constant(odKiedy);
                    body = Expression.Equal(fieldAccess, dataod);
                }
            }
            else if (daty.Length == 2)
            {
                DateTime odKiedy, doKiedy;
                bool okod = TextHelper.PobierzInstancje.SprobojSparsowac(daty[0], out odKiedy);
                bool okdo = TextHelper.PobierzInstancje.SprobojSparsowac(daty[1], out doKiedy);
                if (okdo && okod)
                {
                    body = Expression.And(WyrazenieDataOd(fieldAccess, odKiedy), WyrazenieDataDo(fieldAccess, doKiedy));
                }
                else if (okod)
                {
                    body = WyrazenieDataOd(fieldAccess, odKiedy);
                }
                else if (okdo)
                {
                    body = WyrazenieDataDo(fieldAccess, doKiedy);
                }
            }
            return body;
        }

        private Expression WyrazenieDataOd(Expression pole, DateTime data)
        {
            var dataod = Expression.Constant(data);

            Expression waruekod = Expression.GreaterThanOrEqual(pole, dataod);
            return waruekod;
        }

        private Expression WyrazenieDataDo(Expression pole, DateTime data)
        {
            var dataod = Expression.Constant(data);

            Expression waruekod = Expression.LessThanOrEqual(pole, dataod);
            return waruekod;
        }

        public LambdaExpression StworzWhereEpression(Type typ, string[] szukanepola, string[] wyszukiwane, bool jednaFrazaJednoPole)
        {
            var metoda = this.GetType().GetMethods().First(x => x.Name == "StworzWhereEpression" && x.IsGenericMethod).MakeGenericMethod(typ);
            return metoda.Invoke(this, new object[] {szukanepola, wyszukiwane, jednaFrazaJednoPole, null}) as LambdaExpression;
        }


        /// <summary>
        /// Generuje wyrażenie do wyszukiwania
        /// </summary>
        /// <param name="typ">Typ obiektu w którym szukamy</param>
        /// <param name="szukanepola">Kolekcja pól w których szukamy</param>
        /// <param name="wyszukiwane">Kolejcja szukanych fraz</param>
        /// <param name="jednaFrazaJednoPole">Dana fraza musi występować w odpowidającym mu polu - i-ta fraza w itym polu</param>
        /// <returns></returns>
        public LambdaExpression StworzWhereEpression<TDane>(string[] szukanepola, string[] wyszukiwane, bool jednaFrazaJednoPole, ParametrBindowaniaPobieraniaDanych<TDane> bindowanie = null)
        {
            if (bindowanie == null)
            {
                bindowanie = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzBindowaniaPobieraniaDanych<TDane>();
            }

            Type typ = typeof(TDane);
            if (jednaFrazaJednoPole && szukanepola.Length != wyszukiwane.Length)
            {
                throw new InvalidOperationException("Liczba szukanych pól musi być równa liczbie szukanych fraz");
            }

            Type lambdaType = typeof(Func<,>).MakeGenericType(typ, typeof(bool));
            PropertyInfo[] pola = typ.GetProperties();
            
            ParameterExpression pe = Expression.Parameter(typ, OznaczenieTypu);
            Expression warunek = null;
            if (wyszukiwane.Any() && !wyszukiwane.All(string.IsNullOrEmpty))
            {
                for (int i = 0; i < wyszukiwane.Length; i++)
                {
                    if (string.IsNullOrEmpty(wyszukiwane[i]))
                    {
                        continue; //pusta fraza pomijamy
                    }
                    Expression warunekJednejPrazy = null;
                    for (int j = 0; j < szukanepola.Length; j++)
                    {
                        if (jednaFrazaJednoPole && i != j)
                        {
                            continue; //tylko generujemy gdy i==j
                        }
                        Expression waruenPola;
                        PropertyInfo info = pola.First(x => x.Name == szukanepola[j]);
                        bool czyIgnorowane = info.GetCustomAttributes<IgnoreAttribute>().Any();
                        if (czyIgnorowane && bindowanie.FiltrySql)
                        {
                            throw new Exception(string.Format("Nie można wyszukiwać po polu {0}. Włączone jest filtrowanie sql a dane pole ma atrybut ignorowane. ", szukanepola[j]));
                        }
                        Type typpola = info.PropertyType.PobierzPodstawowyTyp();
                        MemberExpression fieldAccess = Expression.Property(pe, info);

                        if (info.PropertyType.Implements(typeof(IEnumerable<long>)) || info.PropertyType.Implements(typeof(IEnumerable<long?>)))
                        {
                            waruenPola = GenerujFrazeSzukaniaKolekcjiLong(fieldAccess, wyszukiwane[i], info);
                        }else if (info.PropertyType.Implements(typeof(IEnumerable<int>)) || info.PropertyType.Implements(typeof(IEnumerable<int?>)))
                        {
                            waruenPola = GenerujFrazeSzukaniaKolekcjiInt(fieldAccess, wyszukiwane[i], info);
                        }
                        else if (info.PropertyType.Implements(typeof(IList)))
                        {
                            waruenPola = GenerujFrazeSzukanieListy(fieldAccess, wyszukiwane[i], info, bindowanie);
                        }
                        else if (info.PropertyType.Implements(typeof(IDictionary<string,string>)))
                        {
                            waruenPola = GenerujFrazeSzukaniaSlownika(fieldAccess, wyszukiwane[i], info);
                        }else if (typpola == typeof(Guid))
                        {
                            waruenPola = GenerujFrazeSzukaniaGuid(fieldAccess, wyszukiwane[i], info);
                        }
                        else if (typpola == typeof(DateTime))
                        {
                            waruenPola = GenerujFrazeSzukaniaDaty(fieldAccess, wyszukiwane[i], info);
                        }
                        else if (typpola.IsEnum)
                        {
                            waruenPola = GenerujFrazeSzukaniaEnum(fieldAccess, wyszukiwane[i], info);
                        }
                        else if (typpola == typeof(int) || typpola == typeof(decimal) || typpola == typeof(long) || typpola == typeof(float) || typpola == typeof(double))
                        {
                            waruenPola = GenerujFrazeSzukaniaLiczby(fieldAccess, wyszukiwane[i], info);
                        }
                        else if (typpola == typeof(bool))
                        {
                            waruenPola = GenerujFrazeSzukaniaBool(fieldAccess, wyszukiwane[i], info);
                        }
                        else
                        {
                            waruenPola = GenerujFrazeSzukaniaStringa(fieldAccess, wyszukiwane[i], info, bindowanie);
                        }
                        if (waruenPola != null)
                        {
                            if (warunekJednejPrazy == null)
                            {
                                warunekJednejPrazy = waruenPola;
                            }
                            else
                            {
                                warunekJednejPrazy = Expression.Or(warunekJednejPrazy, waruenPola);
                            }
                        }
                    }
                    if (warunekJednejPrazy != null)
                    {
                        if (warunek == null)
                        {
                            warunek = warunekJednejPrazy;
                        }
                        else
                        {
                            warunek = Expression.AndAlso(warunek, warunekJednejPrazy);
                        }
                    }
                }
            }
            if (warunek == null)
            {
                if (!bindowanie.FiltrySql)
                {

                    ConstantExpression w1 = Expression.Constant(true);
                    ConstantExpression w2 = Expression.Constant(true);
                    warunek = Expression.Equal(w1, w2);
                }
                else
                {
                    return null;
                }
            }
            LambdaExpression le = Expression.Lambda(lambdaType, warunek, pe);
            return le;
        }

        /// <summary>
        /// metoda do szukania - wykorzystywana generycznie wiec nie ma referencji ale jest potrzebna w dostepDane
        /// </summary>
        /// <typeparam name="TDane"></typeparam>
        /// <param name="pole"></param>
        /// <param name="kolejnosc"></param>
        /// <returns></returns>
        public List<SortowanieKryteria<TDane>> WygenerujSortowanieLista<TDane>(string pole, KolejnoscSortowania kolejnosc = KolejnoscSortowania.asc)
        {
            List<SortowanieKryteria<TDane>> lista = new List<SortowanieKryteria<TDane>>();

            SortowanieKryteria<TDane> sort = WygenerujSortowanie<TDane>(pole, kolejnosc);
            lista.Add(sort);

            return lista;
        }

        public SortowanieKryteria<TDane> WygenerujSortowanie<TDane>(string pole, KolejnoscSortowania kolejnosc = KolejnoscSortowania.asc)
        {
            if (string.IsNullOrEmpty(pole))
            {
                return null;
            }

            Type typ = typeof(TDane);
            PropertyInfo poleprop;
            try
            {
                poleprop = typ.PropertiesPobierzWszystkie()[pole];
            }
            catch (Exception ex)
            {
                throw new Exception($"Brak pola: {pole} w obiekcie: {typ.Name}");
            }
            
            if (!poleprop.PropertyType.Implements(typeof(IComparable)))
            {
                if (!poleprop.PropertyType.IsGenericType ||
                    poleprop.PropertyType.GetGenericTypeDefinition() != typeof(Nullable<>) ||
                    !poleprop.PropertyType.GetGenericArguments()[0].Implements(typeof(IComparable))
                    )
                {
                    return null;
                }
            }

            Type lambdaType = typeof(Func<,>).MakeGenericType(typ, typeof(object));

            ParameterExpression param = Expression.Parameter(typ, "x");

            Expression wyraznie = Expression.Convert(Expression.Property(param, poleprop), typeof(object));
            Expression<Func<TDane, object>> le = Expression.Lambda(lambdaType, wyraznie, param) as Expression<Func<TDane, object>>;

            SortowanieKryteria<TDane> sort = new SortowanieKryteria<TDane>(le, kolejnosc, pole);
            return sort;
        }


        public Delegate WygenerujSortowanie(Type typ, string pole)
        {
            if (string.IsNullOrEmpty(pole))
            {
                return null;
            }
            PropertyInfo poleprop = typ.GetProperty(pole);
            if (poleprop == null)
            {
                throw new Exception(string.Format("Brak pola: {0} w obiekcie: {1}",pole,typ.Name));
            }
            if (!poleprop.PropertyType.Implements(typeof(IComparable)))
            {
                if (!poleprop.PropertyType.IsGenericType ||
                    poleprop.PropertyType.GetGenericTypeDefinition() != typeof(Nullable<>) ||
                    !poleprop.PropertyType.GetGenericArguments()[0].Implements(typeof(IComparable))
                    )
                {
                    return null;
                }
            }
            Type lambdaType = typeof(Func<,>).MakeGenericType(typ, typeof(object));

            ParameterExpression param = Expression.Parameter(typ, "x");

            Expression wyraznie = Expression.Convert(Expression.Property(param, poleprop), typeof(object));   //important to use the Expression.Convert
            LambdaExpression le = Expression.Lambda(lambdaType, wyraznie, param);
            return le.Compile();
        }


























        public Func<TDane, object> WygenerujSortowanie<TDane>(string pole)
        {
            return (Func<TDane, object>)WygenerujSortowanie(typeof(TDane), pole);
        }

        [ExcludeFromCodeCoverage]
        [Obsolete("Uzywać StworzWhere generującego wyrażenie lambda")]
        public IList<T> WyszukajObiekty<T>(IList<T> wyszukajObiekty, string szukane, IEnumerable<string> szukanepola)
        {
            if (string.IsNullOrEmpty(szukane) || wyszukajObiekty == null ||  wyszukajObiekty.IsEmpty() || szukane=="null")
            {
                return wyszukajObiekty;
            }

            List<T> wynik = new List<T>();

            IEnumerable<string> enumerable = szukanepola as string[] ?? szukanepola.ToArray();
            List<Tuple<PropertyInfo, bool,string>> pifos = new List<Tuple<PropertyInfo, bool,string>>();

            Type typWlasciwyObiektuSzukanego = wyszukajObiekty.First().GetType();

            Dictionary<string, PropertyInfo> propertisy = typWlasciwyObiektuSzukanego.Properties();

            foreach (string p in enumerable)
            {
                bool polezagniezdzone = p.Contains(".");

                PropertyInfo propertis = null;
                if (!polezagniezdzone)
                {
                    propertis = propertisy[p];
                }

                pifos.Add(new Tuple<PropertyInfo, bool, string>(propertis, polezagniezdzone, p));
            }

            List<Regex> regexy = PobierzWyszukiwanieRegex(szukane);
            var akcesor = typWlasciwyObiektuSzukanego.PobierzRefleksja();

            var poladatoweSaJakiekolwiek = pifos.Where(x =>x.Item1 != null && x.Item1.PropertyType.PobierzPodstawowyTyp() == typeof(DateTime)).ToList();
            if (poladatoweSaJakiekolwiek.Any())
            {
                var daty = szukane.Split(new[] { ';' });

                if (daty.Length == 2)
                {
                    DateTime odKiedy, doKiedy;
                    bool okod = TextHelper.PobierzInstancje.SprobojSparsowac(daty[0], out odKiedy);
                    bool okdo = TextHelper.PobierzInstancje.SprobojSparsowac(daty[1], out doKiedy);
                    if (okdo || okod)
                    {
                        for (int i = 0; i < wyszukajObiekty.Count; i++)
                        {
                            foreach (var pi in poladatoweSaJakiekolwiek)
                            {
                                object wal = null;
                                if (pi.Item2)
                                {
                                    //zagniezdzenie
                                    wal = Refleksja.PobierzWartosc(wyszukajObiekty[i], pi.Item3);
                                }
                                else
                                {
                                    wal = akcesor[wyszukajObiekty[i], pi.Item1.Name];
                                }
                                if (wal == null)
                                {
                                    wyszukajObiekty.RemoveAt(i);
                                    i--;
                                }
                                else
                                {
                                    DateTime wartosc = (DateTime)wal;
                                    if (okod && okdo)//mamy przedzial
                                    {
                                        if (wartosc < odKiedy || wartosc > doKiedy)
                                        {
                                            wyszukajObiekty.RemoveAt(i);
                                            i--;
                                        }
                                    }
                                    else if (okod)//tylko od kiedy
                                    {
                                        if (wartosc < odKiedy)
                                        {
                                            wyszukajObiekty.RemoveAt(i);
                                            i--;
                                        }
                                    }
                                    else
                                    {
                                        if (wartosc > doKiedy)
                                        {
                                            wyszukajObiekty.RemoveAt(i);
                                            i--;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (var pi in poladatoweSaJakiekolwiek)
                {
                    pifos.Remove(pi);
                }
            }

            if (!pifos.Any())
            {
                wynik.AddRange(wyszukajObiekty);
            }
            else
            {
                foreach (var dok in wyszukajObiekty)
                {
                    string fraza = "";
                    foreach (var pi in pifos)
                    {
                        object wal = null;
                        if (pi.Item2)
                        {
                            //zagniezdzenie
                            wal = Refleksja.PobierzWartosc(dok, pi.Item3);
                        }
                        else
                        {
                            wal = akcesor[dok, pi.Item1.Name];
                        }


                        fraza += " " + wal;
                    }
                    bool pasujefraza = true;
                    foreach (Regex wf in regexy)
                    {
                        if (!wf.IsMatch(fraza))
                        {
                            pasujefraza = false;
                            break;
                        }
                    }
                    if (pasujefraza)
                    {
                        wynik.Add(dok);
                    }
                }
            }
            return wynik;
        }

        [ExcludeFromCodeCoverage]
        public IList<T> SortujObiekty<T>(IList<T> obiekty, Sortowanie sortowanie)
        {
            if (obiekty == null || obiekty.IsEmpty())
            {
                return obiekty;
            }

            Type typRealny = obiekty.First().GetType();
            var propertisy = typRealny.Properties();
            var akcesor = typRealny.PobierzRefleksja();

            List<Tuple<PropertyInfo, bool, bool, bool, string>> parametrysortowania = new List<Tuple<PropertyInfo, bool, bool, bool, string>>();
            for (int i = 0 ; i < sortowanie.Pola.Count ; ++i)
            {
                bool polezagniezdzone = sortowanie.Pola[i].Pole.Contains(".");
                PropertyInfo pi = null;
                if (!polezagniezdzone)
                {
                    //sortujemy tylko jesli jest takie pole po ktorym sortujemy
                    if (!propertisy.ContainsKey(sortowanie.Pola[i].Pole))
                    {
                        continue; //nie sortujemy bo nie ma proepritsu i tak
                    }
        
                    pi = propertisy[sortowanie.Pola[i].Pole];
                    Type t = pi.PropertyType;
                    bool liczbowy = t.IsEnum || t.IsAssignableFrom(typeof(decimal)) || t.IsAssignableFrom(typeof(long)) || t.IsAssignableFrom(typeof(int)) || t.IsAssignableFrom(typeof(WartoscLiczbowa)) ||
                                    t.IsAssignableFrom(typeof(WartoscLiczbowaZaokraglana));
                    bool isEnum = t.IsEnum;

                    parametrysortowania.Add(new Tuple<PropertyInfo, bool, bool, bool, string>(pi, liczbowy, isEnum, polezagniezdzone, sortowanie.Pola[i].Pole));
                }
                else
                {
                    parametrysortowania.Add(new Tuple<PropertyInfo, bool, bool, bool, string>(null, false, false,true, sortowanie.Pola[i].Pole));
                }
            }

            if (parametrysortowania.IsEmpty())
            {
                //nic nie sortuem bo nie ma po czym już
                return obiekty;
            }

            T[] produkty = obiekty.ToArray();
            List<Tuple<T, List<object>>> ids = new List<Tuple<T, List<object>>>();
            for (int i = 0; i < produkty.Length; i++)
            {
                List<object> sortowania = new List<object>();
                foreach (var s in parametrysortowania)
                {
                    object wartoscDoSortowania = null;
                    if (s.Item4)
                    {
                        //zagniezdzone
                        wartoscDoSortowania = Refleksja.PobierzWartosc(produkty[i], s.Item5);
                    }
                    else
                    {
                        wartoscDoSortowania = akcesor[produkty[i], s.Item5];
                    }
                 
                    if (wartoscDoSortowania != null)
                    {
                        var typ = wartoscDoSortowania.GetType();
                        if (typ == typeof(DateTime) || typ == typeof(DateTime?))
                        {
                            wartoscDoSortowania = (DateTime)wartoscDoSortowania;
                        }
                        else if (typ == typeof(WartoscLiczbowa))
                        {
                            wartoscDoSortowania = ((WartoscLiczbowa)wartoscDoSortowania).Wartosc;
                        }
                        else if ( typ == typeof(WartoscLiczbowaZaokraglana))
                        {
                            wartoscDoSortowania = ((WartoscLiczbowaZaokraglana)wartoscDoSortowania).Wartosc;
                        }
                        else if (typ == typeof(int))
                        {
                            wartoscDoSortowania = (int) wartoscDoSortowania;
                        }
                        else if (s.Item3)
                        {
                            wartoscDoSortowania = (decimal) wartoscDoSortowania;
                        }
                        else
                        {
                            wartoscDoSortowania = wartoscDoSortowania.ToString();
                        }
                    }
                    sortowania.Add(wartoscDoSortowania);
                }
                ids.Add(new Tuple<T, List<object>>(produkty[i], sortowania));
            }
            IOrderedEnumerable<Tuple<T, List<object>>> sortowane = ids.OrderBy(x => 1);
            int elsort = 0;
            for (int i = 0; i < sortowanie.Pola.Count; i++)
            {
                int elsort1 = elsort;
                if (sortowanie.Pola[i].KolejnoscSortowania == KolejnoscSortowania.desc)
                {
                    sortowane = sortowane.ThenByDescending(p => p.Item2[elsort1]);
                }
                else
                {
                    sortowane = sortowane.ThenBy(p => p.Item2[elsort1]);
                }
                elsort++;
            }

            return sortowane.Select(x => x.Item1).ToList();
        }

        [ExcludeFromCodeCoverage]
        [Obsolete]
        public IList<T> SortujObiekty<T>(IList<T> obiekty, string sortKolumna, KolejnoscSortowania kolejnoscSortowania)
        {
            if (string.IsNullOrEmpty(sortKolumna))
            {
                return obiekty;
            }
            var sort = new Sortowanie();
            sort.Pola.Add(new SortowaniePole(sortKolumna, kolejnoscSortowania));
            return SortujObiekty(obiekty, sort);
        }

    }
}