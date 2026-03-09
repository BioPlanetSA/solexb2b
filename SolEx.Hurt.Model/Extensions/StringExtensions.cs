
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

namespace System
{
    public class NaturalSortComparer<T> : IComparer<string>, IDisposable
    {
        private bool isAscending;
        private bool defualtCompare;
        public NaturalSortComparer(bool inAscendingOrder = true,bool sortowanieDomyslne=false)
        {
            this.isAscending = inAscendingOrder;
            defualtCompare = sortowanieDomyslne;
        }

        #region IComparer<string> Members

        public int Compare(string x, string y)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComparer<string> Members

        int IComparer<string>.Compare(string x, string y)
        {

            if (x == y)
                return 0;

            string[] x1, y1;

            if (!table.TryGetValue(x, out x1))
            {
                x1 = Regex.Split(x.Replace(" ", ""), "([0-9]+)");
                table.Add(x, x1);
            }

            if (!table.TryGetValue(y, out y1))
            {
                y1 = Regex.Split(y.Replace(" ", ""), "([0-9]+)");
                table.Add(y, y1);
            }

            int returnVal;
            if (defualtCompare)
            {
                returnVal = System.String.Compare(x, y, System.StringComparison.Ordinal);
            }
            else
            {
                for (int i = 0; i < x1.Length && i < y1.Length; i++)
                {
                    if (x1[i] != y1[i])
                    {
                        returnVal = PartCompare(x1[i], y1[i]);
                        return isAscending ? returnVal : -returnVal;
                    }
                }

                if (y1.Length > x1.Length)
                {
                    returnVal = 1;
                }
                else if (x1.Length > y1.Length)
                {
                    returnVal = -1;
                }
                else
                {
                    returnVal = 0;
                }
            }
            return isAscending ? returnVal : -returnVal;
        }

        private static int PartCompare(string left, string right)
        {
            int x, y;
            if (!int.TryParse(left, out x))
                return left.CompareTo(right);

            if (!int.TryParse(right, out y))
                return left.CompareTo(right);

            return x.CompareTo(y);
        }

        #endregion

        private Dictionary<string, string[]> table = new Dictionary<string, string[]>();

        public void Dispose()
        {
            table.Clear();
            table = null;
        }
    }
    public static class StringExtensions
    {
        public static Stream ToStream(this string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        //x@x.pl  i x@x.p
        private const string RegexPoprawnyEmail = @"^[a-zA-ZżźćńółęąśŻŹĆĄŚĘŁÓŃ0-9_.+-]+@[a-zA-ZżźćńółęąśŻŹĆĄŚĘŁÓŃ0-9-]+\.[a-zA-Z0-9-.]{1,}$";

        public static bool PoprawnyAdresEmail(this string emailaddress)
        {
            if (emailaddress != null) return Regex.IsMatch(emailaddress, RegexPoprawnyEmail);
            return false;
        }
        /// <summary>
        /// Pobieram maxymalną ilosć znaków z tekstu, jesli tekst jest większy to ucina
        /// </summary>
        /// <param name="s"></param>
        /// <param name="maksimum"></param>
        /// <returns></returns>
        public static string PobierzMaksZnakow(this string s, int maksimum)
        {
            if (s.Length > maksimum)
            {
                return s.Substring(0, maksimum-3)+"...";
            }
            return s;
        }

        /// <summary>
        /// Zamienia polskie znaki na odpowiednikiem "bez ogonka"
        /// </summary>
        /// <returns></returns>
        public static string ZamienPolskieZnaki(this string text)
        {
            return text.Replace("ń", "n").Replace("Ń", "N").Replace("ó", "o").Replace("Ó", "O").Replace("ś", "s").Replace("Ś", "S")
                .Replace("ć", "c").Replace("Ć", "C").Replace("ę", "e").Replace("Ę", "E").Replace("ą", "a").Replace("Ą", "A").Replace("ł", "l")
                .Replace("Ł", "L").Replace("ż", "z").Replace("Ż", "Z").Replace("ź", "z").Replace("Ź", "Z");
        }

        
        /// <summary>
        /// Zamienia \r\n na br by ładnie wyświetlić tekst
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ZamienZnakKoncaLiniNaWebowy(this string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return s.Replace("\r\n", "<br/>").Replace("\r", "<br/>").Replace("\n", "<br/>");
        }

        /// <summary>
        /// Zamienia br na r\n\
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string HtmlToText(this string s)
        {
            return s.Replace("<br/>","\r\n");
        }

        public static string DodajTagPreDoFormatowanegoTekstu(this string s)
        {
            return $"<pre class=\"opisZpliku\">{s}</pre>";
        }

        public static string UsunFormatowanieHTML(this string s)
        {
            //bartek - tu ma byc wyrzucanie wyjatkow jesli programisci nie umieja kodowac i na nullach uruchamiaja funckje
            if (s == null)
            {
                throw new NullReferenceException();
            }

            //TODO:inna wersja: <(.|\n)*?> ???
            var objRegEx = new Regex("<[^>]*>");

            return objRegEx.Replace(s, "");
        }

        /// <summary>
        /// Usuwa sormatowanie htmlowe by wyświetlić jako tesk
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string UsunFormatowanie(this string s)
        {
            //bartek - tu ma byc wyrzucanie wyjatkow jesli programisci nie umieja kodowac i na nullach uruchamiaja funckje
            if (s == null)
            {
                throw new NullReferenceException();
            }

            //TODO:inna wersja: <(.|\n)*?> ???
            var objRegEx = new Regex("<[^>]*>");

            return objRegEx.Replace(s, "").Replace("\r\n", "").Replace("\r", "").Replace("\n", "").Replace("&nbsp;", " ").Replace("  ", " ");
        }
        public static string Replace(this string originalString, string oldValue, string newValue, StringComparison comparisonType)
        {
            int startIndex = 0;
            while (true)
            {
                startIndex = originalString.IndexOf(oldValue, startIndex, comparisonType);
                if (startIndex == -1)
                    break;

                originalString = originalString.Substring(0, startIndex) + newValue + originalString.Substring(startIndex + oldValue.Length);

                startIndex += newValue.Length;
            }

            return originalString;
        }

        public static string ToString(this object anObject, string aFormat)
        {
            return ToString(anObject, aFormat, null);
        }

        public static string ToString(this object anObject, string aFormat, IFormatProvider formatProvider)
        {
            StringBuilder sb = new StringBuilder();
            Type type = anObject.GetType();
            Regex reg = new Regex(@"({)([^}]+)(})", RegexOptions.IgnoreCase);
            MatchCollection mc = reg.Matches(aFormat);
            int startIndex = 0;
            foreach (Match m in mc)
            {
                Group g = m.Groups[2]; //it's second in the match between { and }
                int length = g.Index - startIndex - 1;
                sb.Append(aFormat.Substring(startIndex, length));

                string toGet = String.Empty;
                string toFormat = String.Empty;
                int formatIndex = g.Value.IndexOf(":"); //formatting would be to the right of a :
                if (formatIndex == -1) //no formatting, no worries
                {
                    toGet = g.Value;
                }
                else //pickup the formatting
                {
                    toGet = g.Value.Substring(0, formatIndex);
                    toFormat = g.Value.Substring(formatIndex + 1);
                }

                //first try properties
                PropertyInfo retrievedProperty = type.GetProperty(toGet);
                Type retrievedType = null;
                object retrievedObject = null;
                if (retrievedProperty != null)
                {
                    retrievedType = retrievedProperty.PropertyType;
                    retrievedObject = retrievedProperty.GetValue(anObject);
                }
                else //try fields
                {
                    FieldInfo retrievedField = type.GetField(toGet);
                    if (retrievedField != null)
                    {
                        retrievedType = retrievedField.FieldType;
                        retrievedObject = retrievedField.GetValue(anObject);
                    }
                }

                if (retrievedType != null) //Cool, we found something
                {
                    string result = String.Empty;
                    if (toFormat == String.Empty) //no format info
                    {
                        result = retrievedType.InvokeMember("ToString",BindingFlags.Public | BindingFlags.NonPublic |BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.IgnoreCase, null, retrievedObject ?? "", null) as string;
                    }
                    else //format info
                    {
                        result = retrievedType.InvokeMember("ToString",BindingFlags.Public | BindingFlags.NonPublic |BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.IgnoreCase, null, retrievedObject ?? "", new object[] { toFormat, formatProvider }) as string;
                    }
                    sb.Append(result);
                }
                else //didn't find a property with that name, so be gracious and put it back
                {
                    sb.Append("{");
                    sb.Append(g.Value);
                    sb.Append("}");
                }
                startIndex = g.Index + g.Length + 1;
            }
            if (startIndex < aFormat.Length) //include the rest (end) of the string
            {
                sb.Append(aFormat.Substring(startIndex));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Usuwanie znaków specjalnych które przeszkadzają w tekscie
        /// </summary>
        /// <param name="txt">Tekst do oczyszczenia</param>
        /// <returns></returns>
        public static string ReplaceHexadecimalSymbols(this string txt)
        {
            List<string> fraza = new List<string>();
            fraza.Add("[\x00-\x08\x0B\x0C\x0E-\x1F]");
            fraza.Add("\x2060");
            fraza.Add("\xC5");
            fraza.Add("\xC3");
            fraza.Add("\xB3");
            fraza.Add("\x82");
            fraza.Add("\x84");
            fraza.Add("\xC282");
            fraza.Add("\xC284");
            fraza.Add("\xC2");
            fraza.Add("\xd83d\xde09");
            foreach (var f in fraza)
            {
                txt = Regex.Replace(txt, f, "", RegexOptions.Compiled);
            }
            return txt.Trim();

        }

        public static string PobierzSkrot(this string tekst,int ileZnakow,string oznaczenieZeUciety="")
        {
            if (ileZnakow == 0)
            {
                return null;
            }

            if (String.IsNullOrEmpty(tekst))
            {
                return "";
            }
            string oczyszczony = tekst.UsunFormatowanieHTML();
            if (oczyszczony.Length > ileZnakow)
            {
                return oczyszczony.Substring(0, ileZnakow) + oznaczenieZeUciety;
            }
            return oczyszczony;
        }

        /// <summary>
        /// usuwa wielokrotne spacje z stringa i pozostawia tylko jedną spacje
        /// </summary>
        /// <param name="obiekt"></param>
        /// <returns></returns>
        public static string OczyscCiagZZbednychSpacji(this string obiekt)
        {
            return Regex.Replace(obiekt, @" {2,}", " ");
        }
    }
}
