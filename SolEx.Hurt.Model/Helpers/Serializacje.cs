using ServiceStack.Common;

namespace SolEx.Hurt.Model.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public  class Serializacje
    {

        private static readonly Serializacje _tools = new Serializacje();

        public static Serializacje PobierzInstancje
        {
            get { return _tools; }
        }

        public HashSet<T> DeSerializeList<T>(string s, string separator)
        {
            return DeSerializeListZachowajKolejnosc<T>(s, separator).ToHashSet();
        }

        public string SerializeList<T>(IEnumerable<T> list, char separator=',')
        {
            if (list == null)
            {
                return "";
            }
            StringBuilder builder = new StringBuilder();
            foreach (T str in list)
            {
                builder.AppendFormat("{0}{1}",str, separator);
            }
            return builder.ToString().TrimEnd(new [] { separator });
        }
        public Dictionary<string, string> StworzKolekcjeZeStringa(string dane)
        {
            Dictionary<string, string> parametryKoszyka = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(dane))
            {
                string[] pars = dane.Split(new[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in pars)
                {
                    string[] data = s.Split(new[] { "^^" }, StringSplitOptions.RemoveEmptyEntries);
                    parametryKoszyka.Add(data[0], data.Length > 1 ? data[1] : "");
                }
            }
            return parametryKoszyka;
        }
        public string PrzetowrzKolekcjeNaString(Dictionary<string, string> dane)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var key in dane.Keys)
            {
                sb.AppendFormat("{0}^^{1}||", key, dane[key]);
            }
            return sb.ToString();
        }
        /// <summary>
        /// Deserializuje listę z zachowaniem kolejności elementów
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="produktyid"></param>
        /// <returns></returns>
        public List<T> DeSerializeListZachowajKolejnosc<T>(string s, string separator)
        {
            if (string.IsNullOrEmpty(s))
            {
                return new List<T>();
            }
            Type typ = typeof(T);
            string[] strArray = s.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            List<T> list = new List<T>();
            foreach (string str in strArray)
            {
                list.Add((T)Convert.ChangeType(str, typ));
            }
            return list;
        }
    }
}

