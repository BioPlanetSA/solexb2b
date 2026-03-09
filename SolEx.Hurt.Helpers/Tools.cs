using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using HtmlAgilityPack;
using ServiceStack.Common;
using ServiceStack.Text;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.Hurt.Helpers
{
    public class Tools
    {
        /// <summary>
        /// wycina frazy z treści gdzie frazy podawać w formacie: "<tytul>", "</tytul>" - fraza otwierajaca i zamykajaca - to co miedzy nimi zostanie zwrocone
        /// </summary>
        /// <param name="tresc"></param>
        /// <param name="fraza"></param>
        /// <returns></returns>
        public string WytnijFrazeZTresci(ref string tresc, string[] fraza)
        {
            string wycieta = "";
            int idx = tresc.IndexOf(fraza[0], StringComparison.InvariantCultureIgnoreCase);
            if (idx > -1)
            {
                int koniect = tresc.IndexOf(fraza[1], idx + 1, StringComparison.InvariantCultureIgnoreCase);
                if (koniect > idx)
                {
                    var t = tresc.Substring(idx, koniect - idx);
                    wycieta = t.Substring(fraza[0].Length).Trim();
                    tresc = tresc.Remove(idx, koniect - idx + fraza[1].Length);
                }
            }
            return wycieta;
        }

        static Tools(){}

        private static readonly Tools _tools = new Tools();

        public static Tools PobierzInstancje => _tools;

        private static readonly char[] _polishChars = { 'ą', 'Ą', 'ć', 'Ć', 'ę', 'Ę', 'ł', 'Ł', 'ń', 'Ń', 'ó', 'Ó', 'ś', 'Ś', 'ż', 'Ż', 'ź', 'Ź' };
        private static readonly char[] _univChars = { 'a', 'A', 'c', 'C', 'e', 'E', 'l', 'L', 'n', 'N', 'o', 'O', 's', 'S', 'z', 'Z', 'z', 'Z' };

        /// <summary>
        /// dodaje przedrostek http do wejściowego stringa jeśli go brakuje
        /// </summary>
        /// <param name="wejsciowy">adres www do poprawienia</param>
        /// <returns>adres www z przedrostkiem http dla niepustego stringa</returns>
        public string PoprawAdresWWW(string wejsciowy)
        {
            if (wejsciowy.IsNullOrEmpty())
                return wejsciowy;


            if(wejsciowy.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) || wejsciowy.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
            {
                return wejsciowy;
            }
 
            wejsciowy = $"https://{wejsciowy}";

            return wejsciowy;
        }

        /// <summary>
        /// Usuwa polskie znaki z ciagu wejściowego. Zastępuje spacje innym znakiem
        /// </summary>
        /// <param name="str"></param>
        /// <param name="czymZastapicSpacje"></param>
        /// <returns></returns>
        public static string UsunPolskieZnaki(string str, char czymZastapicSpacje = '-')
        {
            char[] abc = str.ToCharArray();
            for (int i = 0; i < abc.Length; ++i)
            {
                char c = abc[i];
                if (char.IsDigit(c))
                    continue;
                if (c == '-')
                    continue;
                if (char.IsLetter(c))
                {
                    int j = 0;
                    while (j < _polishChars.Length)
                    {
                        if (c == _polishChars[j])
                        {
                            abc[i] = _univChars[j];
                            j = _polishChars.Length;
                        }
                        ++j;
                    }
                    continue;
                }
                abc[i] = czymZastapicSpacje;
            }
            return new string(abc);
        }

        /// <summary>
        /// zwraca info o tym czy podana ścieżka do pliku jest lokalna. false dla ścieżki zdalnej.
        /// </summary>
        /// <param name="sciezka"></param>
        /// <returns></returns>
        public bool CzyPlikJestLokalny(string sciezka)
        {
            try
            {
                return new Uri(sciezka).IsFile;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }
        }

        public string DoLadnejCyfry(decimal liczba)
        {
            return liczba.ToString("0.##");
        }        
        
        public string DoLadnejCyfry(double liczba)
        {
            return liczba.ToString("0.##");
        }   
        
        //public  string GetId(string s, out int id, out string group)
        //{
        //    if (string.IsNullOrEmpty(s) || s.Length < 8)
        //    {
        //        id = -1;
        //        group = "";

        //        return "Parametr wejsciowy ma nieprawidłową długość: " + s;
        //    }

        //    s = s.Trim().Replace("node-", "");
        //    string sid = s.Remove(0, 7);

        //    group = s.Substring(0, 7);

        //    if (!Int32.TryParse(sid, out id))
        //        return "Niepoprawny format parametru: " + sid + " " + s;

        //    return "";
        //}

        //public  string GetId(string s, out int id)
        //{
        //    if (string.IsNullOrEmpty(s) || s.Length < 1)
        //    {
        //        id = -1;

        //        return "Parametr wejsciowy ma nieprawidłową długość: " + s;
        //    }

        //    if (!Int32.TryParse(s, out id))
        //        return "Niepoprawny format parametru: " + s;

        //    return "";
        //}
        //public string GetId(string s, out long id)
        //{
        //    if (string.IsNullOrEmpty(s) || s.Length < 1)
        //    {
        //        id = -1;

        //        return "Parametr wejsciowy ma nieprawidłową długość: " + s;
        //    }

        //    if (!long.TryParse(s, out id))
        //        return "Niepoprawny format parametru: " + s;

        //    return "";
        //}

        private Dictionary<string, string> d = new Dictionary<string, string>
        {
            //Images'
            {".bmp", "image/bmp"},
            {".gif", "image/gif"},
            {".jpeg", "image/jpeg"},
            {".jpg", "image/jpeg"},
            {".png", "image/png"},
            {".tif", "image/tiff"},
            {".tiff", "image/tiff"},
            //Documents'
            {".doc", "application/msword"},
            {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
            {".pdf", "application/pdf"},

            {".rtf", "application/rtf"},
            //Slideshows'
            {".ppt", "application/vnd.ms-powerpoint"},
            {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
            //Data'
            {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
            {".xls", "application/vnd.ms-excel"},
            {".csv", "text/csv"},
            {".xml", "text/xml"},
            {".txt", "text/plain"},
            {".epp", "text/plain"},
            //Compressed Folders'
            {".zip", "application/zip"},
            //Audio'
            {".ogg", "application/ogg"},
            {".mp3", "audio/mpeg"},
            {".wma", "audio/x-ms-wma"},
            {".wav", "audio/x-wav"},
            //Video'
            {".wmv", "audio/x-ms-wmv"},
            {".swf", "application/x-shockwave-flash"},
            {".avi", "video/avi"},
            {".mp4", "video/mp4"},
            {".mpeg", "video/mpeg"},
            {".mpg", "video/mpeg"},
            {".qt", "video/quicktime"}
        };
              
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName">Ścieżka do pliku lub samo rozszerzenie (musi być podane z kropką np. .pdf)</param>
        /// <returns></returns>
        public string GetMimeType(string fileName)
        {
            string s = null;
            string ext = fileName;
            if (!fileName.StartsWith("."))
            {
                ext = Path.GetExtension(fileName);
            }
            ext = ext.ToLower();
            d.TryGetValue(ext, out s);
            return s;
        }

        public  byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = stream.Position;
            stream.Position = 0;

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            System.Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            System.Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    System.Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                stream.Position = originalPosition;
            }
        }

        private  bool CzyImportMozeByc(string test)
        {
            test = test.ToLower();
            return test.Contains('ó') || test.Contains('ł') || test.Contains('ą') || test.Contains('ę') || test.Contains('ź') || test.Contains('ż') || test.Contains('ć') || test.Contains('ś');
        }
        /// <summary>
        /// Pobieranie zawartosci pliku tekstowego z formatowaniem bez wzracania kodowania pliku
        /// </summary>
        /// <param name="sciezka"></param>
        /// <param name="e">kodowanie</param>
        /// <param name="kodowanie">czy dopadować kodowanie?</param>
        /// <param name="formatowacDoHtml"></param>
        /// <returns></returns>
        public virtual string PobierzZawartoscPlikuTekstowegoZFormatowaniem(string sciezka,  Opisy.KodowanieOpisow kodowanie = Opisy.KodowanieOpisow.Dopasuj, bool formatowacDoHtml = false)
        {
            Encoding e;
            return PobierzZawartoscPlikuTekstowegoZFormatowaniem(sciezka,out e, kodowanie, formatowacDoHtml);
        }

        /// <summary>
        /// Pobieranie zawartosci pliku tekstowego z formatowaniem z wzracaniem kodowania pliku
        /// </summary>
        /// <param name="sciezka"></param>
        /// <param name="e">kodowanie</param>
        /// <param name="kodowanie">czy dopadować kodowanie?</param>
        /// <param name="formatowacDoHtml"></param>
        /// <returns></returns>
        public virtual string PobierzZawartoscPlikuTekstowegoZFormatowaniem(string sciezka, out Encoding e, Opisy.KodowanieOpisow kodowanie = Opisy.KodowanieOpisow.Dopasuj, bool formatowacDoHtml = false)
        {
            if (string.IsNullOrEmpty(sciezka))
            {
                throw new NullReferenceException("Brak sciezki do pliku.");
            }
            var opis = "";
            e = Encoding.Default;
            switch (kodowanie)
            {
                case Opisy.KodowanieOpisow.Dopasuj:
                    {
                        opis = PobierzZawartoscPlikuTekstowego(sciezka, out e);
                    }
                    break;
                case Opisy.KodowanieOpisow.UTF8:
                    {
                        e = Encoding.UTF8;
                        opis = File.ReadAllText(sciezka, e);
                    }
                    break;
            }

            if (!regexHTML.IsMatch(opis) && formatowacDoHtml)
            {
                opis = opis.ZamienZnakKoncaLiniNaWebowy();
            }

            return opis;
        }

         Regex regexHTML = new Regex("<.*?>"); 

        /// <summary>
        /// Pobieranie zawartości pliku tekstowego wraz z rozpoznaniem i zwróceniem kodowania
        /// </summary>
        /// <param name="sciezka">Ścieżka do pliku</param>
        /// <param name="e">kodowanie pliku</param>
        /// <returns></returns>
        public  string PobierzZawartoscPlikuTekstowego(string sciezka, out Encoding e)
        {
            StreamReader sr = new StreamReader(sciezka, true);
            string dane = GetContent(sr.BaseStream, out e);
            sr.Close();
            return dane;

        }

        /// <summary>
        /// Pobieranie zarawtości strumienia tekstowego wraz z rozpoznaniem kodowania
        /// </summary>
        /// <param name="str">strumień</param>
        /// <returns></returns>
        public string GetContent(Stream str)
        {
            Encoding e;
            return GetContent(str, out e);
        }

        /// <summary>
        /// Pobieranie zarawtości strumienia tekstowego wraz z rozpoznaniem i zwróceniem kodowania
        /// </summary>
        /// <param name="str">strumień</param>
        /// <param name="e">Kodowanie strumienia</param>
        /// <returns></returns>
        public string GetContent( Stream str, out Encoding e)
        {
            // Create a Stream object.
            byte[] bytes = ReadToEnd(str);

            e = Encoding.UTF8;
            string result = e.GetString(bytes);
            if (CzyImportMozeByc(result))
            {
                return result;
            }

            e = Encoding.ASCII;
            result = e.GetString(bytes);
            if (CzyImportMozeByc(result))
            {
                return result;
            }

            e = Encoding.GetEncoding("windows-1250");
            result = e.GetString(bytes);
            if (CzyImportMozeByc(result))
            {
                return result;
            }

            e = Encoding.GetEncoding(1252);
            result = e.GetString(bytes);
            if (CzyImportMozeByc(result))
            {
                return result;
            }

            // jak nie ma nic - zwracamy Default
            e = Encoding.Default;
            return e.GetString(bytes);
        }

        private  log4net.ILog log
        {
            get
            {
                return log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            }
        }

        public long PoliczHashDokumentu(HistoriaDokumentu d, out string ciagDoHasha)
        {
            //eksperymentalnie dodaje UWAGI - jak bedzie problem to trzeba ja wywalic stad
            ciagDoHasha = string.Format(CultureInfo.InvariantCulture, "{0}-{1}-{2}-{3}-{4}-{5}-{6}", ((d.WartoscNetto + (3 * d.WartoscNalezna)) + (d.Zaplacono ? 100 : -100) + d.KlientId + (d.TerminPlatnosci.HasValue ? d.TerminPlatnosci.Value.ToString("yyMMdd").ToInt() : 0)).ToString("0.000", CultureInfo.InvariantCulture), d.StatusId, d.NazwaPlatnosci, d.NazwaDokumentu, d.Uwagi, d.DokumentPowiazanyId, d.OdbiorcaId ?? 0).Replace("--", "").Replace(" ", "");
            return ciagDoHasha.WygenerujIDObiektuSHAWersjaLong();
        }
        public long PoliczHashPozycjiDokumentu(List<HistoriaDokumentuProdukt>pozycje)
        {
            string hash1 = pozycje.Select(x => x.KodProduktu + "_" + x.JednostkaMiary + "_" + x.Opis).ToCsv();
            string hash2 = pozycje.Sum(x => x.Ilosc*x.CenaNetto*x.CenaNettoPoRabacie).ToString(CultureInfo.InvariantCulture);

            string fraza =  hash1 + "-" + hash2;
            return fraza.WygenerujIDObiektuSHAWersjaLong();
        }
        public  string GetMd5Hash(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new Exception("Błąd - nie można wyliczyć sumy MD5 z pustego ciągu");
            }
            var bity = Encoding.UTF8.GetBytes(input);
            return GetMd5Hash(bity);
        }

        public string GetMd5Hash(byte[] bity)
        {
            using (MD5 md5Hasher = MD5.Create())
            {
                byte[] data = md5Hasher.ComputeHash(bity);
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2").ToLower());
                }
                md5Hasher.Dispose();
                return sBuilder.ToString();
            }
        }

        public string PobierzWersjeDLL(string dllka)
        {
           return FileVersionInfo.GetVersionInfo(dllka).ProductVersion;
        }

        public  string PoprawHTML(string html, Encoding enkodowanie)
        {
            if (!string.IsNullOrEmpty(html))
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlDocument();
                MemoryStream ms = new MemoryStream();
                doc.LoadHtml(html);
                doc.Save(ms);
                return enkodowanie.GetString(ms.ToArray());
            }
            else return html;
        }

        /// <summary>
        /// przeszukuje wszystkie podkatalogi w poszukiwaniu plików i w razie braku dostępu do wybranego katalogu nie wywala się tak jak Directory.GetFiles
        /// </summary>
        /// <param name="root"></param>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        public  IEnumerable<string> GetFiles(string root, string searchPattern)
        {
            Stack<string> pending = new Stack<string>();
            pending.Push(root);
            while (pending.Count != 0)
            {
                var path = pending.Pop();
                string[] next = null;
                try
                {
                    next = Directory.GetFiles(path, searchPattern);
                }
                catch { }
                if (next != null && next.Length != 0)
                    foreach (var file in next) yield return file;
                try
                {
                    next = Directory.GetDirectories(path);
                    foreach (var subdir in next) pending.Push(subdir);
                }
                catch { }
            }
        }
        public bool CzyUkrycProdukt(bool widoczny, bool ukryty, string widocznyUstawienie, string ukrytyUstawienie)
        {
            if (!string.IsNullOrEmpty(ukrytyUstawienie))
                return ukryty;

            if (!string.IsNullOrEmpty(widocznyUstawienie))
                return !widoczny;

            return false;
        }
        private static Random random = new Random((int)DateTime.Now.Ticks);
      
        public string WygenerujString(int dlugosc)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < dlugosc; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        public static string OczyscCiagDoLinkuURL(string title)
        {
            // make it all lower case
            title = title.ToLower();
            // remove entities
            title = Regex.Replace(title, @"&\w+;", "");
            //zmieniam polskie znaki
            title = UsunPolskieZnaki(title);
            // remove anything that is not letters, numbers, dash, or space
            title = Regex.Replace(title, @"[^a-z0-9\-\s]", "");
            // replace spaces
            title = title.Replace(' ', '-');
            // collapse dashes
            title = Regex.Replace(title, @"-{2,}", "-");
            // trim excessive dashes at the beginning
            title = title.TrimStart(new[] { '-' });
            // if it's too long, clip it
            if (title.Length > 80)
                title = title.Substring(0, 79);
            // remove trailing dashes
            title = title.TrimEnd(new[] { '-' });
            return title;
        }

        /// <summary>
        /// Sprawdzamy czy obiekt na propertisach gdzie jest ustawiony atrybut MaksymalnaLiczbaZnakow zawartość testu nie przeprasza ustawionej wartości, jesli tak to obcina wartość. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obiekt">Obiekt jaki chcemy sprawdzić - musi być typ referencyjny bo zmieniamy na nim dane, ale nie chcemy jawnie REF dawać dlatego że to komplikuje nam pętle</param>
        /// <returns></returns>
        public static List<string> SprawdzIloscZnakow<T>(T obiekt)
        {
            List<string>wynik = new List<string>();
            Type typObjektu = obiekt.GetType();
            PropertyInfo[] polaObiektu = typObjektu.GetProperties();
            foreach (PropertyInfo info in polaObiektu)
            {
                //wyciagam maxymalną lilość zanków
                MaksymalnaLiczbaZnakowAttribute liczba = info.GetCustomAttribute<MaksymalnaLiczbaZnakowAttribute>();
                if(liczba== null) continue;
                var wart = info.GetValue(obiekt);
                if(wart == null) continue;
                string wartosc = wart.ToString();
                if (wartosc.Length > liczba.MaksymalnaLiczbaZnakow)
                {
                    wynik.Add(info.Name);
                    wartosc = wartosc.Substring(0, liczba.MaksymalnaLiczbaZnakow);
                    info.SetValue(obiekt, wartosc);
                }
            }

            return wynik;
        }
        /// <summary>
        /// Wyliczamuy sumę kontrolną dla kodu EAN-13
        /// </summary>
        /// <param name="kod">Kod ean bez sumy kontrolne. Musi posiadać 12 cyfr</param>
        /// <returns>suma kontrolna</returns>
        public static int WyliczSumeDoKoduEan(long kod)
        {
            if (kod<100000000000 && kod >999999999999)
                throw new ArgumentException($"Kod: {kod} musi mieć długość 12 cyfr. ");
            int suma = 0;
            long tmpKod = kod;
            for (int index = 0; index < 12; index++)
            {
                var wsp = index%2 == 1 ? 1 : 3;

                int cyfra = (int)(tmpKod%10);
                tmpKod = tmpKod/10;

                suma += cyfra*wsp;
            }
            var wynik = 10 - (suma % 10);
            wynik = wynik%10;
            return wynik;
        }
    }
}
