using ServiceStack.Common;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL
{
    public class PlikiBLL : BllBazaCalosc, IPliki
    {
        public PlikiBLL(ISolexBllCalosc calosc) : base(calosc)
        {
        }

        public IObrazek PobierzObrazek(string sciezkaPliku)
        {
            try
            {
                Plik p = Calosc.DostepDane.PobierzPojedynczy<Plik>(a => a.Sciezka + a.Nazwa == sciezkaPliku, null);
                if (p == null)
                {
                    return null;
                }
                return new Obrazek(p);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Pobiera obrazek po id
        /// </summary>
        /// <param name="idObrazka">id obrazka</param>
        /// <returns></returns>
        public IObrazek PobierzObrazek(int idObrazka)
        {
            try
            {
                Plik p = Calosc.DostepDane.PobierzPojedynczy<Plik>(x => x.Id == idObrazka, null);
                if (p == null)
                {
                    return null;
                }
                return new Obrazek(p);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Pobieramy liste Obrazków na podstawie HashSet ze sciażką
        /// </summary>
        /// <param name="sciezkiObrazkow"></param>
        /// <returns></returns>
        public List<IObrazek> PobierzListeObrazkow(HashSet<string> sciezkiObrazkow)
        {
            List<IObrazek> obrazki = new List<IObrazek>();
            foreach (var i in sciezkiObrazkow)
            {
                IObrazek obrazek = PobierzObrazek(i);
                if(obrazek == null) continue;
                obrazki.Add(obrazek);
            }
            return obrazki;
        }

        private string[] zdjeciaRozszerzenia = { ".PNG", ".JPG", ".JPEG", ".BMP", ".GIF", ".TIFF" };

        public void PlikiProduktuUsun(HashSet<string> ids)
        {
            Calosc.DostepDane.Usun<ProduktPlik,string>(ids.ToList());
        }

        public Plik DodajPlikUzytkownika(Plik p)
        {
            //p.PoprawNazwaPlikuDlaURL();   //nie wolno zmienic nazwy pliku!!
            p.RodzajPliku = CzyPlikToZdjecie(p) ? RodzajPliku.Zdjecie : RodzajPliku.Zalacznik;
            Calosc.DostepDane.AktualizujPojedynczy(p);
            return p;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p">plik do zapisu</param>
        /// <param name="dodacDoBazyDanychPlikowB2B">jeśłi false to nie dodajmy do bazy danych B2B</param>
        /// <returns></returns>
        public Plik ImportPlikBase64(Plik p, bool dodacDoBazyDanychPlikowB2B = true)
        {
            if (p.RodzajPliku == RodzajPliku.Zalacznik || p.RodzajPliku == RodzajPliku.Zdjecie || p.RodzajPliku == RodzajPliku.PlikDoPrzeniesieniaJedenNaJedenOdKlienta)
            {
                if (p.RodzajPliku == RodzajPliku.PlikDoPrzeniesieniaJedenNaJedenOdKlienta)
                {
                    //sceizka zawsze zaczyna sie od /zasoby/import/{0}
                    p.Sciezka = "/zasoby/import/"+p.lokalnaSciezkaDoZapisuPliku.Replace('\\','/').TrimStart('/');

                }
                else
                {
                    p.PoprawNazwaPlikuDlaURL();
                    p.Sciezka = "/zasoby/import/" + p.Nazwa[0] + "/";
                }

                Helpers.PlikiBase64.Base64ToFile(p.DanePlikBase64, p.SciezkaBezwzgledna);
                p.DanePlikBase64 = null; //czyszczenie pliku z zawartosci
            }

            //p.CzyObrazek = CzyPlikToZdjecie(p);
            if (dodacDoBazyDanychPlikowB2B)
            {
                p.Id = Db.Scalar<int>("select ISNULL(max(id), 0) + 1 from plik where id >0");
                Calosc.DostepDane.AktualizujPojedynczy(p);
            }
            
            return p;
        }

        public virtual bool CzyPlikToZdjecie(string sciezka)
        {
            var rozszerzenie = Path.GetExtension(sciezka);
            if (rozszerzenie == null)
            {
                return false;
            }
            return zdjeciaRozszerzenia.Contains(rozszerzenie, StringComparer.OrdinalIgnoreCase);
        }

        public virtual bool CzyPlikToZdjecie(Plik p)
        {
            return CzyPlikToZdjecie(p.Nazwa);
        }

        public void UsunPlikiZBazyBezPlikowFizycznych()
        {
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "zasoby\\import"))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "zasoby\\import");
            }

            HashSet<string> plikiDysku = new HashSet<string>( Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "zasoby\\import\\", "*", SearchOption.AllDirectories).Select(x => x.ToLower())  );

            IList<Plik> pliki = Calosc.DostepDane.Pobierz<Plik>(null);

            List<int> doUsuniecia = new List<int>();

            foreach (Plik p in pliki)
            {
                if (p.Id <= 0)
                {
                    continue;
                }
                if (p.RodzajPliku == RodzajPliku.Link)
                {
                    continue;
                }
                if (!plikiDysku.Contains(p.SciezkaBezwzgledna))
                {
                    doUsuniecia.Add(p.Id);
                }
            }
            if (doUsuniecia.Count > 0)
            {
                Calosc.Log.Info("Usuwanie zbędnych plików w liczbie: " + doUsuniecia.Count);
                Calosc.DostepDane.Usun<Plik, int>(doUsuniecia);
            }
        }

        public List<IObrazek> PobierzObrazkiProduktu(long produktID)
        {
            List<IObrazek> wynik = new List<IObrazek>();

            List<ProduktPlik> lacznikiWybrane = null;

            if (Laczniki.TryGetValue(produktID, out lacznikiWybrane))
            {
                bool jestGlowne = lacznikiWybrane.Any(a => a.Glowny);
                HashSet<int> ids = new HashSet<int>( lacznikiWybrane.Select(x => x.PlikId) );
                IList<Plik> pliki = Calosc.DostepDane.Pobierz(null, x => ids.Contains(x.Id), new[] { new SortowanieKryteria<Plik>(x => x.NazwaBezRozszerzenia, KolejnoscSortowania.asc, "NazwaBezRozszerzenia") });
                foreach (var x in pliki)
                {
                    if (x.RodzajPliku == null || x.RodzajPliku == RodzajPliku.Zdjecie)
                    {
                        wynik.Add(new Obrazek(x));
                    }
                }
                long pierwszeIDzdjGlowne = ids.FirstOrDefault();

                IObrazek zdjecieGlowne = wynik.FirstOrDefault(a => a.Id == pierwszeIDzdjGlowne);
                if (zdjecieGlowne == null || !jestGlowne)
                {
                    zdjecieGlowne = wynik.FirstOrDefault();
                }
                if (zdjecieGlowne != null)
                {
                    wynik.Remove(zdjecieGlowne);
                    wynik.Insert(0, zdjecieGlowne);
                }
            }
            return wynik;
        }

        private Dictionary<long, List<ProduktPlik>> _lacznikiPlikow = null;

        /// <summary>
        /// lista łączników pogrupowana wg. produktow id - laczniki w kolejnosci takiej jak powinny byc pokazywane na stronie
        /// </summary>
        /// <returns></returns>
        public Dictionary<long, List<ProduktPlik>> Laczniki
        {
            get
            {
                if (_lacznikiPlikow == null)
                {
                 _lacznikiPlikow = Calosc.DostepDane.Pobierz<ProduktPlik>(null).GroupBy(x => x.ProduktId).ToDictionary(x => x.Key, x => x.OrderByDescending(a => a.Glowny).ToList());
                }
                return _lacznikiPlikow;
            }
            set { _lacznikiPlikow = value; }
        }
        /// <summary>
        /// Metoda uruchamiana w Global.asax która usuwa pliki fizyczne z dysku gdy nie ma powiazanych z nim plików w bazie
        /// </summary>
        public void UsunPlikiFizyczneBezPlikuWBazie()
        {
            HashSet<string> sciezkiZBazy = new HashSet<string>( Calosc.DostepDane.Pobierz<Plik>(null).Select(x=>x.SciezkaBezwzgledna) );
            HashSet<string> plikiNaDysku = PobierzPlikiZKataloguImportu();

            HashSet<string> plikiDoUsuniecia = new HashSet<string>( plikiNaDysku.Except(sciezkiZBazy, StringComparer.InvariantCultureIgnoreCase) );
            foreach (var plik in plikiDoUsuniecia)
            {
                if (File.Exists(plik))
                {
                    File.Delete(plik);
                    string katalog = Path.GetDirectoryName(plik);
                    if (katalog != null && !Directory.GetFiles(katalog, "*.*", SearchOption.AllDirectories).Any())
                    {
                        Directory.Delete(katalog);
                    }
                }
            }
        }

        public virtual HashSet<string> PobierzPlikiZKataloguImportu()
        {
            string katalog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "zasoby\\import");
            
            //jesli nie am katalogu to go tworzymy, przydatne przy stawiamiu nowych wdrożeń
            if (!Directory.Exists(katalog))
            {
                Directory.CreateDirectory(katalog);
            }

            return new HashSet<string>( Directory.GetFiles(katalog, "*.*", SearchOption.AllDirectories) );
        }

        public void UsunCache(IList<object> obj)
        {
            Calosc.Cache.UsunGdzieKluczRozpoczynaSieOd(Calosc.DostepDane.KluczCacheTypDanych<Obrazek>());
        }


         public void BindPoAktualizacji(IList<ProduktPlik> obj)
        {
            _lacznikiPlikow = null;
        }
    }
}