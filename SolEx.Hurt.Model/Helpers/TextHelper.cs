using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces.Helpers;

namespace SolEx.Hurt.Model.Helpers
{
    /// <summary>
    /// Zbiór metod pomagających w operacjach na stringach
    /// </summary>
    public class TextHelper : ITextHelper
    {
        private static TextHelper _textHelper = new TextHelper();

        public static TextHelper PobierzInstancje
        {
            get { return _textHelper; }
            set { _textHelper = value; }
        }

        public bool SprobojSparsowac(string text, out decimal wynik)
        {
            wynik = 0;
            if (string.IsNullOrEmpty(text)) return false;
            text = text.Replace(" ", "").Trim();
            return decimal.TryParse(text.Replace(',', '.'), out wynik) || decimal.TryParse(text.Replace('.', ','), out wynik);
        }

        /// <summary>
        /// Pobiera zawartosc taga body
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string GetHTMLBodyContent(string text)
        {

            string result = text;
            if (!string.IsNullOrEmpty(text))
            {
                string start = "<body>";
                string end = "</body>";
                int idxStart = result.IndexOf(start,StringComparison.InvariantCultureIgnoreCase);
                if (idxStart > -1)
                {
                    idxStart += start.Length;
                    result = result.Substring(idxStart);
                }

                int idxEnd = result.LastIndexOf(end, StringComparison.InvariantCultureIgnoreCase);
                if (idxEnd > -1)
                {

                    result = result.Substring(0, idxEnd);
                }
            }
            return result;
        }
        //przeniesione do StrinfExtensionm
        ///// <summary>
        ///// Usuwa polskie znaki i zamienia odpowiednikiem "bez ogonka"
        ///// </summary>
        ///// <param name="text">tekst</param>
        ///// <returns></returns>
        //public string ReplacePolishChars(string text)
        //{
        //    return text.Replace("ń", "n").Replace("Ń", "N").Replace("ó", "o").Replace("Ó", "O").Replace("ś", "s").Replace("Ś", "S")
        //        .Replace("ć", "c").Replace("Ć", "C").Replace("ę", "e").Replace("Ę", "E").Replace("ą", "a").Replace("Ą", "A").Replace("ł", "l").Replace("Ł", "L").Replace("ż", "z").Replace("Ż", "Z").Replace("ź", "z").Replace("Ź", "Z");
        //}
        
        ///// <summary>
        ///// Usuwa polskie znaki
        ///// </summary>
        ///// <param name="text">tekst</param>
        ///// <param name="defaultValue">Znak na co zamieniane są polskie znaki</param>
        ///// <returns></returns>
        //public object ReplacePolishChars(string text, string defaultValue)
        //{
        //    return text
        //    .Replace("ń", defaultValue).Replace("Ń", defaultValue).Replace("ó", defaultValue).Replace("Ó", defaultValue).Replace("ś", defaultValue).Replace("Ś", defaultValue).Replace("ć", defaultValue)
        //    .Replace("Ć", defaultValue).Replace("ę", defaultValue).Replace("Ę", defaultValue).Replace("ą", defaultValue).Replace("Ą", defaultValue).Replace("ł", defaultValue).Replace("Ł", defaultValue)
        //    .Replace("ż", defaultValue).Replace("Ż", defaultValue).Replace("ź", defaultValue).Replace("Ź", defaultValue);
        //}


        /// <summary>
        /// Koduje znaki Base64
        /// </summary>
        /// <param name="str">Ciąg do zakodowania</param>
        /// <returns></returns>
        public string Encode(string str)
        {
            byte[] encbuff = Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(encbuff);
        }
        /// <summary>
        /// Odkodowuje string z Base64 
        /// </summary>
        /// <param name="str">Zakodowany tekst</param>
        /// <returns></returns>
        public string Decode(string str)
        {
            byte[] decbuff = Convert.FromBase64String(str);
            return Encoding.UTF8.GetString(decbuff);
        }

        Regex myRegex = new Regex(@"[^0-9a-zA-Z-,$_@]+",RegexOptions.Compiled);
        public virtual string OczyscNazwePliku(string nazwa)
        {
      
            if (!string.IsNullOrEmpty(nazwa))
            {
                string n = nazwa.ZamienPolskieZnaki();//ReplacePolishChars(nazwa);
                return  myRegex.Replace(n, "-").ToLower();
            }
            return "";
        }


        public string WygenerujNazweZdjecia(TypyPolDoDopasowaniaZdjecia pole, string separator, int idProduktu, string kodKreskowy, string kodProduktu,  bool zdjecieGlowne, int idzdjecia, string rozszerzenie= ".jpg")
        {
            string nowaNazwa = "";
            switch (pole)
            {
                case TypyPolDoDopasowaniaZdjecia.Idproduktu:
                {
                    nowaNazwa = string.Format("{0}{2}{3}zdjid{2}{1}", idProduktu, Convert.ToInt16(zdjecieGlowne),separator, idzdjecia).ZamienPolskieZnaki();
                        //ReplacePolishChars(string.Format("{0}{2}{3}zdjid{2}{1}", idProduktu, Convert.ToInt16(zdjecieGlowne), separator, idzdjecia));
                } break;
                case TypyPolDoDopasowaniaZdjecia.KodKreskowy:
                    {
                        nowaNazwa = OczyscNazwePliku(string.Format("{0}{2}{3}{2}{1}", kodKreskowy, Convert.ToInt16(zdjecieGlowne), separator, idzdjecia));
                    } break;

                case TypyPolDoDopasowaniaZdjecia.Kod:
                    {
                        nowaNazwa = OczyscNazwePliku(string.Format("{0}{2}{3}{2}{1}", kodProduktu, Convert.ToInt16(zdjecieGlowne), separator, idzdjecia));
                    } break;
            }
            //roboczo zakladamy JPG
            nowaNazwa = nowaNazwa + rozszerzenie;
            return nowaNazwa;
        }
        public DateTime? ParsujDate(string data)
        {
            DateTime wynik;
            if (DateTime.TryParseExact(data, new[] { 
              "yyyy.MM.dd", "yyyy.M.d", "yyyy.M.dd", "yyyy.MM.d"
            , "yyyy.MM.dd hh:mm", "yyyy.M.d hh:mm", "yyyy.M.dd hh:mm", "yyyy.MM.d hh:mm"
            , "yyyy.MM.dd hh:mm:ss", "yyyy.M.d hh:mm:ss", "yyyy.M.dd hh:mm:ss", "yyyy.MM.d hh:mm:ss"
            , "dd.MM.yyyy", "d.M.yyyy", "dd.M.yyyy", "d.MM.yyyy"
            , "dd.MM.yyyy hh:mm", "d.M.yyyy hh:mm", "dd.M.yyyy hh:mm", "d.MM.yyyy hh:mm"
            , "dd.MM.yyyy hh:mm:ss", "d.M.yyyy hh:mm:ss", "dd.M.yyyy hh:mm:ss", "d.MM.yyyy hh:mm:ss","yyyy-MM-dd HH:mm","yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd"
            ,"yyyyMMdd"
            }, CultureInfo.CurrentCulture, DateTimeStyles.None, out wynik))
            {
                return wynik;
            }
            if (DateTime.TryParse(data, out wynik))
            {
                return wynik;
            }
            return null;
        }
        public bool SprobojSparsowac(string data, out DateTime wynik)
        {
            wynik = DateTime.MinValue;
            DateTime? tmp = ParsujDate(data);
            if (tmp != null)
            {
                wynik = tmp.Value;
            }
            return tmp.HasValue;
        }
        public string ParsujDateDoStringa(DateTime? value)
        {
            if (value == null) return "";

            return value.Value.ToString("dd.MM.yyyy");
        }
        public string GetRandomString(int size)
        {
            Random random = new Random();
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return builder.ToString();
        }

    }
}
