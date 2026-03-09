using System;
using System.Collections.Generic;
using System.Data.SqlTypes;

namespace SolEx.Hurt.Core.Helper
{
    public class Przedzial
    {
        public static List<string> Sprawdzenie<T>(T? min, T? max, string nazwaMin = "Wartość minimalna", string nazwaMax = "Wartość maksymalna", bool sprawdzacPrzedzial = true) where T :  struct, IComparable
        {
            var a = (T)Convert.ChangeType(0, typeof(T));
            List<string> listaBledow = new List<string>();
            if (!min.HasValue && !max.HasValue)
            {
                listaBledow.Add(string.Format("{0} i {1} są puste",nazwaMin,nazwaMax));
            }
            if (min.HasValue && max.HasValue)
            {
                if (min.Value.CompareTo(max.Value) > 0)
                {
                    listaBledow.Add(string.Format("{0} jest mniejsza od {1}", nazwaMax, nazwaMin));
                }
            }
            if (min.HasValue && min.Value.CompareTo(a) < 0)
            {
                listaBledow.Add(string.Format("{0} jest mniejsza od 0", nazwaMin));
            }
            if (max.HasValue && max.Value.CompareTo(a) < 0)
            {
                listaBledow.Add(string.Format("{0} jest mniejsza od 0", nazwaMax));
            }
            return listaBledow;
        }

        public static List<string> SpradzWartosc<T>(T pole, string nazwaPola) where T : struct, IComparable
        {
            var a = (T)Convert.ChangeType(0, typeof(T));
            List<string> listaBledow = new List<string>();
            if (pole.CompareTo(a) < 0)
            {
                listaBledow.Add(string.Format("{0} jest mniejsza od 0",nazwaPola));
            }
            return listaBledow;
        }
        //public static string SpradzWartosc<T>(T pole, string nazwaPola) where T : IComparable
        //{
        //    var a = (T)Convert.ChangeType(0, typeof(T));
        //    if (pole != null && pole.CompareTo(a) < 0)
        //    {
        //        return string.Format("{0} jest mniejsza od 0",nazwaPola);
        //    }
        //    return string.Empty;
        //}
    }
}
