using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System
{
    public static class ListExtension
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
    (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static IOrderedEnumerable<TSource> OrderByWithDirection<TSource, TKey>
    (this IEnumerable<TSource> source,
     Func<TSource, TKey> keySelector,
     bool descending)
        {
            return descending ? source.OrderByDescending(keySelector)
                              : source.OrderBy(keySelector);
        }

        public static IOrderedQueryable<TSource> OrderByWithDirection<TSource, TKey>
            (this IQueryable<TSource> source,
             Expression<Func<TSource, TKey>> keySelector,
             bool descending)
        {
            return descending ? source.OrderByDescending(keySelector)
                              : source.OrderBy(keySelector);
        }

        public static IList<T> CloneList<T>(this IList<T> listToClone)
        {
            return listToClone.Select(item => item.ClonePojedynczyObiekt()).ToList();
        }

        /// <summary>
        /// W kolekcji wszystkie pola stringkowe które są puste ustawia na null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obiekty"></param>
        public static void ZerowaniePolZPustymiStringami<T>(this IEnumerable<T> obiekty, List<PropertyInfo> polaStringowe)
        {
            //var polaStringowe = typeof (T).GetProperties().Where(x => x.PropertyType == typeof (string) && x.CanRead && x.CanWrite).ToArray();
            foreach (T o in obiekty)
            {
                foreach (var p in polaStringowe)
                {
                    string wartosc = (string) p.GetValue(o);
                    if (wartosc == string.Empty)
                    {
                        p.SetValue(o, null);
                    }
                }
            }
        }

        /// <summary>
        ///  Zamiana znaków końca lini dla pól tekstowych na <br />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obiekty"></param>
        /// <param name="polaStringowe"></param>
        public static void ZamienZnakiKoncaLiniNaBr<T>(this IEnumerable<T> obiekty, List<PropertyInfo> polaStringowe)
        {
            foreach (T o in obiekty)
            {
                foreach (var p in polaStringowe)
                {
                    string wartosc = ((string)p.GetValue(o)).ZamienZnakKoncaLiniNaWebowy();
                    p.SetValue(o, wartosc);
                }
            }
        }
        

        /// <summary>
        /// W kolekcji we wszystkich polach stringowych usuwamy puste znaki
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obiekty"></param>
        public static void CzyszczenieObiektuPusteZnaki<T>(this IEnumerable<T> obiekty, List<PropertyInfo> polaStringowe)
        {
            //var polaStringowe = typeof (T).GetProperties().Where(x => x.PropertyType == typeof (string) && x.CanRead && x.CanWrite).ToArray();
            foreach (T o in obiekty)
            {
                foreach (var p in polaStringowe)
                {
                    string wartosc = (string) p.GetValue(o);
                    if (wartosc != null)
                    {
                        wartosc = wartosc.Trim();
                    }
                    p.SetValue(o, wartosc);
                }
            }
        }

        /// <summary>
        /// W kolekcji wszystkie pola stringowe które mają niedozwolne znaki są czyszczone
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obiekty"></param>
        /// /// <param name="polaString"></param>
        public static void ZerowaniePolStringZeZnakowSpecjalnych<T>(this IEnumerable<T> obiekty, List<PropertyInfo> polaString)
        {
            //var polaString = typeof (T).GetProperties().Where(x => x.PropertyType == typeof (string) && x.CanRead && x.CanWrite).ToArray();
            foreach (T o in obiekty)
            {
                foreach (var p in polaString)
                {
                    string wartosc = (string) p.GetValue(o);
                    if (!string.IsNullOrEmpty(wartosc))
                    {
                        p.SetValue(o, wartosc.ReplaceHexadecimalSymbols());
                    }
                }
            }
        }

        /// <summary>
        /// Metoda wywołująca metody: ZerowaniePolStringZeZnakowSpecjalnych, CzyszczenieObiektuPusteZnaki, ZerowaniePolZPustymiStringami
        /// </summary>
        public static void OperacjeNaPolachTekstowych<T>(this IEnumerable<T> obiekty)
        {
            List<PropertyInfo>listaStringowychPol = obiekty.First().GetType().GetProperties().Where(x => x.PropertyType == typeof(string) && x.CanRead && x.CanWrite).ToList();

            obiekty.ZerowaniePolStringZeZnakowSpecjalnych(listaStringowychPol);
            obiekty.CzyszczenieObiektuPusteZnaki(listaStringowychPol);
            obiekty.ZerowaniePolZPustymiStringami(listaStringowychPol);
            //obiekty.ZamienZnakiKoncaLiniNaBr(listaStringowychPol);
        }

        /// <summary>
        /// Generates tree of items from item list
        /// </summary>
        /// 
        /// <typeparam name="T">Type of item in collection</typeparam>
        /// <typeparam name="K">Type of parent_id</typeparam>
        /// 
        /// <param name="collection">Collection of items</param>
        /// <param name="id_selector">Function extracting item's id</param>
        /// <param name="parent_id_selector">Function extracting item's parent_id</param>
        /// <param name="root_id">Root element id</param>
        /// 
        /// <returns>Tree of items</returns>
        public static IList<TreeItem<T>> GenerateTree<T, TK>(
            this ICollection<T> collection,
            Func<T, TK> id_selector,
            Func<T, TK> parent_id_selector,
          //  IList<TreeItem<T>> listaWyjsciowa,
            TK root_id = default(TK)
            )
        {
            var lista = new List<TreeItem<T>>();

            foreach (var c in collection.Where(c => parent_id_selector(c).Equals(root_id)))
            {
                TreeItem<T> obiekt = new TreeItem<T>();
                obiekt.Item = c;
                obiekt.Children = collection.GenerateTree(id_selector, parent_id_selector, id_selector(c));

                lista.Add(obiekt);
            }
            return lista;
        }

        public static IEnumerable<T> PobierzWszystkieDzieci<T,TElem>(this ICollection<T> lista,Func<T,TElem> id,Func<T,TElem> ojciec,TElem parent  )
        {
            List<T> dzieci = lista.Where(c => ojciec(c).Equals(parent)).ToList();

            var ids = dzieci.Select(x => id(x)).ToList();
            foreach (var d in ids)
            {
                dzieci.AddRange(PobierzWszystkieDzieci(lista,id,ojciec,d));
            }
            return dzieci;
        }
    }
   
    public class TreeItem<T>
    {
        public T Item { get; set; }
        public IList<TreeItem<T>> Children { get; set; }

        public override string ToString()
        {
            return Item.ToString();
        }
    }

}
