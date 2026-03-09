
using System.Collections.Generic;

namespace System
{
  public static class DictionaryExtensions
    {
        /// <summary>
        /// Łączymy dwa slowniki. Do słownika docelowego dodajemy nowe elementy ze słownika źródłowego. (nie łaczy wartości w przypadku wspólnych kluczy).
        /// </summary>
        /// <typeparam name="T">Typ klucza</typeparam>
        /// <typeparam name="S">typ wartości</typeparam>
        /// <param name="source">Słownik docelowy</param>
        /// <param name="collection">słownik źródłowy</param>
        public static void AddRange<T, S>(this Dictionary<T, S> source, Dictionary<T, S> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("Collection is null");
            }

            foreach (var item in collection)
            {
                if (!source.ContainsKey(item.Key))
                {
                    source.Add(item.Key, item.Value);
                }
            }
        }
    }
}
