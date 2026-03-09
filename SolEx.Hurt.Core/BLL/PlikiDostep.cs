using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Web;
using SolEx.Hurt.Core.ModelBLL.ObiektyMaili;

namespace SolEx.Hurt.Core.BLL
{
    public class PlikiDostep
    {
        private static readonly PlikiDostep Instancja = new PlikiDostep();

        public static PlikiDostep PobierzInstancje
        {
            get { return Instancja; }
        }

        private string PobierzRozszerzenie(string sciezka)
        {
            string ext = Path.GetExtension(sciezka);
            if (string.IsNullOrEmpty(ext))
            {
                throw new InvalidOperationException("Brak rozszerzenia. Scieżka " + sciezka);
            }
            ext = ext.ToLower().Replace(".", "");
            switch (ext)
            {
                case "config":
                    return "xml";

                default:
                    return ext;
            }
        }

        public List<string> PobierzWidokiZKatalogu(string sciezka)
        {
            List<string> wynik = new List<string>();
            string katalog = AppDomain.CurrentDomain.BaseDirectory + "Views" + sciezka;
            if (!Directory.Exists(katalog))
            {
                return wynik;
            }
            foreach (string k in Directory.GetFiles(katalog))
            {
                wynik.Add(Path.GetFileNameWithoutExtension(k));
            }
            return wynik;
        }

        /// <summary>
        /// Pobiera widoki z katalogu określonego na podstawie szablonu niestandardowego
        /// </summary>
        /// <param name="sciezka"></param>
        /// <returns></returns>
        public List<string> PobierzWidokiNiestandardowe(string sciezka)
        {
            List<string> wynik = new List<string>();
            string katalog = AppDomain.CurrentDomain.BaseDirectory + sciezka;
            if (!Directory.Exists(katalog))
            {
                return wynik;
            }
            foreach (string k in Directory.GetFiles(katalog))
            {
                wynik.Add(Path.GetFileNameWithoutExtension(k));
            }
            return wynik;
        }

        public List<string> PobierzPlikiGraficzne(string sciezka)
        {
            List<string> grafika = new List<string> { ".jpg", ".jpeg", ".png", ".gif" };
            return PobierzPliki(sciezka,grafika);
        }


        public List<string> PobierzPliki(string sciezka, List<string> grafika)
        {
            if (grafika == null) grafika = new List<string>();
            List<string> wynik = new List<string>();
            string katalog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,sciezka);
            if (!Directory.Exists(katalog))
            {
                return wynik;
            }
            foreach (string k in Directory.GetFiles(katalog))
            {
                if (grafika.Any() && !grafika.Contains(Path.GetExtension(k)))
                {
                    continue;
                }
                wynik.Add(k);
            }
            return wynik;
        }
        public List<string> PobierzTypyFiltow()
        {
            return PobierzWidokiZKatalogu("/Shared/Filtry/Typy");
        }

        private void ZagwarantujIstnieniePliku(string sciezka)
        {
            if (string.IsNullOrEmpty(sciezka))
            {
                throw new InvalidOperationException("Scieżka nie może być pusta");
            }
            string fizyczna = ZbudujSciezkeFizyczna(sciezka);

            if (!File.Exists(fizyczna))
            {
                string kats = Path.GetDirectoryName(fizyczna);
                if (!string.IsNullOrEmpty(kats))
                {
                    Directory.CreateDirectory(kats);
                }
                FileStream s = File.Create(fizyczna);
                s.Close();
            }
        }



        /// <summary>
        /// jesli podamy id newsletra to zwraca sciezke do pliku
        /// </summary>
        /// <param name="idNewslettera"></param>
        /// <returns></returns>
        public string KatalogSzablonowNewsletterow
        {
            get { return "/Views/Shared/NewsletterySzablony/"; }
        }

        public string ZbudujSciezkeFizyczna(string sciezka)
        {
            if (string.IsNullOrEmpty(sciezka))
            {
                throw new InvalidOperationException("Scieżka nie może być pusta");
            }
            return AppDomain.CurrentDomain.BaseDirectory + sciezka.Replace("/", "\\");
        }

        public IConfigBLL Config = SolexBllCalosc.PobierzInstancje.Konfiguracja;

        public Dictionary<string, PlikDoEdycji> PobierzPlikiDoEdycjiAdmin()
        {
            const string sciezkaobrazki = "/static/obrazki_ustawienie.config";
            string hashobrazki = Tools.PobierzInstancje.GetMd5Hash(sciezkaobrazki);
            string pliknazwa = Path.GetFileName(sciezkaobrazki);
            string pliktresc = WczytajTresc(sciezkaobrazki);
            Dictionary<string, PlikDoEdycji> pliki = new Dictionary<string, PlikDoEdycji>();
            pliki.Add(hashobrazki, new PlikDoEdycji(pliknazwa, sciezkaobrazki, hashobrazki, PobierzRozszerzenie(sciezkaobrazki), "Plik z konfiguracją rozmiarów zdjęć w systemie", pliktresc));
            if (!string.IsNullOrEmpty(Config.SzablonNiestandardowyNazwa))
            {
                string sciezkaszablon = Config.SzablonNiestandardowySciezkaWzgledna + "/custom.scss";
                string hashsciezka = Tools.PobierzInstancje.GetMd5Hash(sciezkaszablon);
                string nazwa = Path.GetFileName(sciezkaszablon);
                string configtresc = WczytajTresc(sciezkaszablon);
                pliki.Add(hashsciezka, new PlikDoEdycji(nazwa, sciezkaszablon, hashsciezka, PobierzRozszerzenie(sciezkaszablon), "Indywidualny arkusz stylów", configtresc));
            }

            return pliki;
        }

        private string WczytajTresc(string sciezka)
        {
            ZagwarantujIstnieniePliku(sciezka);
            string fizyczna = ZbudujSciezkeFizyczna(sciezka);
            return File.ReadAllText(fizyczna);
        }

        private void ZapiszTresc(string sciezka, string tresc)
        {
            string fizyczna = ZbudujSciezkeFizyczna(sciezka);
            File.WriteAllText(fizyczna, tresc);
        }

        public void ZapiszPlik(PlikDoEdycji zmieniany)
        {
            if (!PobierzPlikiDoEdycjiAdmin().ContainsKey(zmieniany.Hash))
            {
                throw new SecurityException("Próba edycji pliku którego nie wolno edytować");
            }
            string sciezka = PobierzPlikiDoEdycjiAdmin()[zmieniany.Hash].Sciezka;
            ZagwarantujIstnieniePliku(sciezka);
            ZapiszTresc(sciezka, zmieniany.Zawartosc);
        }
        /// <summary>
        /// Pobiera scieżke dla pliku użytkownika
        /// </summary>
        /// <param name="typ"></param>
        /// <param name="objektId"></param>
        /// <param name="nazwaPliku"></param>
        /// <param name="linkBezwzledny"></param>
        /// <returns></returns>
        public string PobierzSciezkePlikUsera(Type typ, object objektId, string nazwaPliku, bool linkBezwzledny)
        {
            string sciezka = typ + "\\" + objektId + "\\" + nazwaPliku;
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, sciezka);
        }
        //public string KatalogPlikow(string sciezka)
        //{
        //    string katalog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
        //                                  Config.KatalogPliki.TrimStart('/')).Replace("/", "\\");
        //    if (!Directory.Exists(katalog))
        //    {
        //        Directory.CreateDirectory(katalog);
        //    }
        //    string cala = Path.Combine(katalog, sciezka);
        //    if (!Directory.Exists(cala))
        //    {
        //        Directory.CreateDirectory(cala);
        //    }
        //    return cala;
        //}

        public string Podkatalog()
        {
            Random r = new Random(DateTime.Now.Millisecond);
            return DateTime.Now.ToString("ddMMyyyymmhhss") + "_" + r.Next();
        }

        public HashSet<DanePlik> PobierzInfoOPlikachSynchronizatora()
        {
            HashSet<DanePlik> wynik = new HashSet<DanePlik>();
            string sciezka = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Synchronizator");
            if (!Directory.Exists(sciezka))
            {
                throw new InvalidOperationException("Brak katalogu synchronizator na serwerze");
            }
            if (HttpContext.Current == null)
            {
                throw new InvalidOperationException("To nie wywołanie webowe");
            }
            var pliki = Directory.GetFiles(sciezka);
            foreach (var p in pliki)
            {
                string ext = Path.GetExtension(p);
                if (ext == ".exe" || ext == ".dll" || ext == ".txt")
                {
                    DateTime datamodyfikacji = File.GetLastWriteTime(p);
                    string nazwa = Path.GetFileName(p);
                    string hash = Tools.PobierzInstancje.GetMd5Hash(File.ReadAllBytes(p));

                    string strona = "";
                    if (HttpContext.Current != null)
                    {
                        strona = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host +
                                 (HttpContext.Current.Request.Url.IsDefaultPort ? "" : ":" + HttpContext.Current.Request.Url.Port.ToString(CultureInfo.InvariantCulture));
                    }
                    string url = strona + string.Format("/Synchronizator/{0}", nazwa);
                    DanePlik inf = new DanePlik(nazwa, datamodyfikacji, url, hash);
                    wynik.Add(inf);
                }
            }
            return wynik;
        }

        public string SprawdzSciezkeWlasnychSzablonow()
        {
            string sciezkacss = SolexBllCalosc.PobierzInstancje.Cache.PobierzObiekt<string>("WlasnyCss");
            if (sciezkacss == null)
            {
                sciezkacss = "";
                if (!string.IsNullOrEmpty(SolexBllCalosc.PobierzInstancje.Konfiguracja.SzablonNiestandardowyNazwa))
                {
                    string path = AppDomain.CurrentDomain.BaseDirectory + SolexBllCalosc.PobierzInstancje.Konfiguracja.SzablonNiestandardowySciezkaWzgledna.Replace("/", "\\");
                    if (!path.EndsWith("\\"))
                    {
                        path += "\\";
                    }
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    string widokik = Path.Combine(path, "views");
                    if (!Directory.Exists(widokik))
                    {
                        Directory.CreateDirectory(widokik);
                    }
                    string config = Path.Combine(widokik, "web.config");
                    if (!File.Exists(config))
                    {
                        string sciezkaorg = AppDomain.CurrentDomain.BaseDirectory + "views\\web.config";
                        File.Copy(sciezkaorg, config, true);
                    }
                    string sciezkaless = path + "custom.scss";
                    if (!File.Exists(sciezkaless))
                    {
                        File.WriteAllText(sciezkaless, "");
                    }
                    sciezkacss = SolexBllCalosc.PobierzInstancje.Konfiguracja.SzablonNiestandardowySciezkaWzgledna + "/custom.scss";
                }
                SolexBllCalosc.PobierzInstancje.Cache.DodajObiekt("WlasnyCss", sciezkacss);
            }
            return sciezkacss;
        }
    }
}