using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using SolEx.Hurt.Sync.App.Modules_.Helpers;


namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Pliki
{
    [FriendlyName("Zdjęcia do produktów z plików", FriendlyOpis = "Przeszukuje określony katalog i dopasowuje pliki jako Zdjęcie do produktów.")]

    public class ZdjeciaDoProduktowZPlikow : SyncModul, IModulPliki
    {
        public IPliki PlikBll { get; set; } = SolexBllCalosc.PobierzInstancje.Pliki;

        public TextHelper Texthelper { get; set; } = TextHelper.PobierzInstancje;

        public LogiFormatki Logi { get; set; } = LogiFormatki.PobierzInstancje;

        public override string uwagi => "Dozolone znaki w nazwie pliku: 0-9a-zA-Z-,$_. Wszystkie pozostałe będą zastąpione znakiem: -";

        [FriendlyName("Ścieżka do katalogu z plikami - np. c:\\pliki\\")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Sciezka { get; set; }
        
        [Niewymagane]
        [FriendlyName("Pojedyńczy znak który rozdziela nazwe pliku od symbolu produktu - np. jeśli mamy taką nazwę pliku: <b>zdjecie34#SKU123.jpg</b> gdzie kod produktu to SKU123 - naszym separatorem jest znak #")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Separator { get; set; }

        [Niewymagane]
        [FriendlyName("Pojedyńczy znak który rozdziela identyfikatory produktów jeśli jedno zdjęcie jest przeznaczone dla wielu produktów - np. jeśli mamy taką nazwę pliku: <b>zdjecie34#SKU123$SKU234$SKU345.jpg</b> gdzie znak $ rozdziela poszczególne identyfikatory produktów")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string SeparatorIdentyfikatorów { get; set; }

        [FriendlyName("Ustawienie określające względem jakiego parametru będzie ustawiane zdjęcie główne dla produktu")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public ZjecieGlowne zdjecieGlowne { get; set; }

        //[FriendlyName("Czy usuwać z nazwy pliku separator i kod produktu?")]
        //public bool CzyUsuwacSeparatorIKodZpliku { get; set; }

        [FriendlyName("Po jakim polu z produktu dopasowywać zdjęcia?")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public TypyPolDoDopasowaniaZdjecia PoCzymSzukacPlikow { get; set; }


        [FriendlyName("Sposob szukania")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public SposobSzukania SposobSzukania { get; set; }

        [FriendlyName("Jeśli w nazwie pliku jest kilka razy używany separator, to czy dopasować zdjęcie tylko do pierwszego symbolu czy do wszystkich")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public SzukanieIdentyfikatora SzukanieIdentyfikatoraWNazwiePliku { get; set; }

        [FriendlyName("Sposób sortowania plików przed wysłaniem na B2B")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public SortowaniePlikow SposobSortowania { get; set; }

        [FriendlyName("Nazwa pliku na serwerze z pola po którym mapujemy", FriendlyOpis = "W nazwie pliku będzie dopisana scieżka lokalna(oczyszczona) ze żrodła pliku ")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool NazwaMapowanie { get; set; }
        
        [FriendlyName("Maksymalny rozmiar pliku, w MB")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int RozmiarZdjeciaMaks { get; set; }
        public ZdjeciaDoProduktowZPlikow()
        {
            Sciezka = "";
            Separator = "_";
            //CzyUsuwacSeparatorIKodZpliku = false;
            PoCzymSzukacPlikow = TypyPolDoDopasowaniaZdjecia.Kod;
            SposobSortowania = SortowaniePlikow.NazwaAsc;
            RozmiarZdjeciaMaks = 8;
            SzukanieIdentyfikatoraWNazwiePliku = SzukanieIdentyfikatora.WszystkieElementy;
            zdjecieGlowne = ZjecieGlowne.PierwszyWKolekcji;
        }

        /// <summary>
        /// Metoda do zmockowania która pobiera z podanej ścieżki wszystkie pliki, wliczając podkatalogi
        /// </summary>
        /// <param name="sciezka">Ścieżka do katalogu z plikami</param>
        /// <returns>Lista ścieżek do plików z podanego katalogu</returns>
        public virtual string[] PobierzPlikiZKatalogu(string sciezka)
        {
            var tmp = Tools.PobierzInstancje.GetFiles(sciezka, "*").ToArray();
            return Tools.PobierzInstancje.GetFiles(sciezka, "*").ToArray();
        }

        /// <summary>
        /// Metoda do zmockowania która sprawdza czy plik w podanej ścieżce istnieje
        /// </summary>
        /// <param name="Sciezka">Ścieżka do pliku</param>
        /// <returns>True jeśli plik istnieje</returns>
        public virtual bool CzySciezkaIstnieje(string Sciezka)
        {
            return Directory.Exists(Sciezka);
        }

        /// <summary>
        /// Metoda do zmockowania która zwraca obiekt klasy FileInfo
        /// </summary>
        /// <param name="sciezka">Ścieżka do pliku</param>
        /// <returns>Obiekt klasy FileInfo</returns>
        public virtual FileInfo InformacjeOPliku(string sciezka)
        {
            return new FileInfo(sciezka);
        }

        public int licznikMinimalny = -10000;

        public void Przetworz(IDictionary<long, Produkt> produktyNaB2B, ref List<ProduktPlik> plikiLokalnePowiazania, ref List<Plik> plikiLokalne, ISyncProvider provider, ref List<Cecha> cechyB2B, ref List<KategoriaProduktu> kategorieB2B, ref List<Klient> klienciB2B)
        {

            if (string.IsNullOrEmpty(Sciezka) || !CzySciezkaIstnieje(Sciezka))
            {
                throw new Exception($"Brak katalogu: '{Sciezka}' skonfigurowanego w module ({this.Nazwa}) lub katalog nie istnieje. Sprawdź konfiguracje modułu.");
            }
            Dictionary<string, List<Produkt>> produktyPogrupowaneWgSzukania = WygenerujListeKluczyDlaProduktow(produktyNaB2B, PoCzymSzukacPlikow);

            List<string> plikiWKatalogu = PobierzPlikiZKatalogu(Sciezka).ToList();

            if (plikiLokalne.Count > 0)
            {
                licznikMinimalny = plikiLokalne.Min(b => b.Id) - 1;
                if (licznikMinimalny >= 0)
                {
                    licznikMinimalny = -10000;
                }
            }

            Logi.LogujInfo("plików w katalogu: {0} produktów na b2b {1} szukanie po {2}" ,plikiWKatalogu.Count,produktyNaB2B.Count,PoCzymSzukacPlikow);

            Dictionary<long, ProduktPlik> powiazaniazlownik = plikiLokalnePowiazania.ToDictionary(x=>x.Id, x=>x);
            plikiLokalnePowiazania.Clear();
            Dictionary<long, HashSet<string>> zdjeciaDlaProduktow = PobierzZdjeciaDlaProduktow(plikiWKatalogu, produktyPogrupowaneWgSzukania, SposobSortowania, Separator, SposobSzukania);
            Logi.LogujInfo("grupowanie produktów koniec");
            foreach (KeyValuePair<long, HashSet<string>> produkt in zdjeciaDlaProduktow)
            {
                foreach (string plik in produkt.Value)
                {
                    licznikMinimalny = DodajPlikiDoProduktow(powiazaniazlownik, plikiLokalne, plik, licznikMinimalny, Separator, produkt.Key, produkt.Value);
                }
            }
            plikiLokalnePowiazania.AddRange(powiazaniazlownik.Values);
            Logi.LogujInfo($"lokalnych powiązań po przetworzeniu przez moduł ZdjeciaDoProduktowZPlikow w ścieżce {Sciezka}: {plikiLokalnePowiazania.Count}");
        }

        /// <summary>
        /// Przygotowuje słownik z kluczami do produktów w celu szybszego wyszukiwania odpowiedniego produktu
        /// </summary>
        /// <param name="produktyNaB2B">Słownik z produktami na platformie</param>
        /// <param name="PoCzymSzukacPlikow">Określa po czym ma szukać plików i na tej podstawie buduje słownik z kluczami</param>
        /// <returns>Słownik z kluczami (na podstawie propertisa PoCzymSzukacPlikow) i produktami</returns>
        public Dictionary<string, List<Produkt>> WygenerujListeKluczyDlaProduktow(IDictionary<long, Produkt> produktyNaB2B, TypyPolDoDopasowaniaZdjecia PoCzymSzukacPlikow)
        {
            Dictionary<string, List<Produkt>> produktyPogrupowaneWgSzukania = new Dictionary<string, List<Produkt>>();
               //przygotowanie kolekcji produkty
            foreach (Produkt p in produktyNaB2B.Values)
            {
                string klucz = PlikiHelper.WygenerujKluczeDopasowania(PoCzymSzukacPlikow, p);
                if (!string.IsNullOrEmpty(klucz))
                {
                    if (produktyPogrupowaneWgSzukania.ContainsKey(klucz))
                    {
                        produktyPogrupowaneWgSzukania[klucz].Add(p);
                    }
                    else
                    {
                        produktyPogrupowaneWgSzukania.Add(klucz,new List<Produkt>{p});
                    }
                    //Log.DebugFormat("produkt o id {0} i kodzie {1} będzie mapowany wg klucza {2}", p.Id, p.Kod, klucz);
                }
                else
                {
                    Log.DebugFormat(
                        $"produkt o id {p.Id} i kodzie {p.Kod} nie ma wygenrowanego klucza do mapowania - pole produktu wg ustawienia {PoCzymSzukacPlikow} jest puste. Produkt nie będzie miał przypisanych plików");
                }
            }

            return produktyPogrupowaneWgSzukania;
        }

        /// <summary>
        /// Zwraca identyfikatory towaru (może to być ID, symbol itd) z nazwy pliku ze zdjęciem - splituje po separatorze
        /// </summary>
        /// <param name="nazwapliku">Nazwa pliku ze zdjęciem</param>
        /// <param name="separator">Separator oddzielający identyfikator produktu od kolejności zdjęcia lub innych danych</param>
        /// <returns></returns>
        public virtual List<string> WyciagnijIdentyfikatorPlikuZNazwy(string nazwapliku, string separator)
        {
            List<string> identyfikatoryWNazwie = 
                (!string.IsNullOrEmpty(separator) && nazwapliku.Contains(separator)) ? nazwapliku.Split(new[] { separator[0] }, StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim()).ToList(): new List<string> { nazwapliku };

            return identyfikatoryWNazwie;
        }

        /// <summary>
        /// Metoda do zmockowania daty utworzenia pliku
        /// </summary>
        /// <param name="fi">Obiekt typu FileInfo</param>
        /// <returns></returns>
        public virtual DateTime PobierzDateUtworzeniaPliku(FileInfo fi)
        {
            return fi.CreationTime;
        }

        public List<string> PosortujZdjecia(List<string> plikiWKatalogu, SortowaniePlikow poCzymSortowac, string Separator)
        {
            switch (poCzymSortowac)
            {
                case SortowaniePlikow.NazwaAsc:
                    plikiWKatalogu = plikiWKatalogu.OrderBy(a => Path.GetFileName(a.ToLower())).ToList();
                    break;

                case SortowaniePlikow.DataUtworzeniaAsc:
                    plikiWKatalogu = plikiWKatalogu.OrderBy(a => PobierzDateUtworzeniaPliku(InformacjeOPliku(a))).ToList();
                    break;

                case SortowaniePlikow.DataUtworzeniaDesc:
                    plikiWKatalogu = plikiWKatalogu.OrderByDescending(a => PobierzDateUtworzeniaPliku(InformacjeOPliku(a))).ToList();
                    break;

                case SortowaniePlikow.OstatniEement:
                    plikiWKatalogu = plikiWKatalogu.OrderBy(a => WyciagnijIdentyfikatorPlikuZNazwy(Texthelper.OczyscNazwePliku(InformacjeOPliku(a).Name), Separator).Last()).ToList();
                    break;

                case SortowaniePlikow.PierwszyElement:
                    plikiWKatalogu = plikiWKatalogu.OrderBy(a => WyciagnijIdentyfikatorPlikuZNazwy(Texthelper.OczyscNazwePliku(InformacjeOPliku(a).Name), Separator).First()).ToList();
                    break;

                case SortowaniePlikow.Brak:
                    //nie robimy nic, sortowanie domyślne
                    break;
            }

            return plikiWKatalogu;
        }
        /// <summary>
        /// Metoda która przyjmuje wszystkie ścieżki do plików i zwraca gotowy słownik z id produktu i ścieżkami do plików danego produktu
        /// </summary>
        /// <param name="plikiWKatalogu">Kolekcja wszystkich ścieżek plików</param>
        /// <param name="produktyPogrupowaneWgSzukania">Słownik z kluczami zbudowanymi na podstawie pola wg którego ma mapowac pliki i produktami pasującymi do klucza</param>
        /// <param name="poCzymSortowac">Określa wg jakiego kryterium ma sortować pliki dla danego produktu</param>
        /// <param name="separator">Separator oddzielający poszczególne klucze w nazwie pliku</param>
        /// <param name="SposobSzukania"></param>
        /// <returns>Słownik z id produktu i ścieżkami do plików dla danego produktu</returns>
        public Dictionary<long, HashSet<string>> PobierzZdjeciaDlaProduktow(List<string> plikiWKatalogu, Dictionary<string, List<Produkt>> produktyPogrupowaneWgSzukania, SortowaniePlikow poCzymSortowac, string separator, SposobSzukania SposobSzukania)
        {
            List<string> plikiPominieteZPowoduRozmiaru = new List<string>();
            Dictionary<long, HashSet<string>> zdjeciaProduktow = new Dictionary<long, HashSet<string>>();
            List<string> posortowane = PosortujZdjecia(plikiWKatalogu, poCzymSortowac, separator);
        //    Log.InfoFormat("Początek mapowań ilosc {0}",posortowane.Count);
            foreach (string plik in posortowane)
            {
                //sprawdzanie rozmiary czy nie przekracza maksymalnego
                try
                {
                    FileInfo plikInfo = new FileInfo(plik);
                    if (plikInfo.Length > RozmiarZdjeciaMaks*1024*1024) //krytycznie istotne spradzenie rozmiaru dla kazdego pliku, nie tylko zdjeca
                    {
                        plikiPominieteZPowoduRozmiaru.Add(plik);
                        continue;
                    }
                }
                catch (Exception e)
                {
                    Logi.LogujInfo("Nie udało się sprawdzić rozmiaru pliku: {0}. Pomijam plik. Dokładny błąd w logach", plik);
                    Log.Error(e);
                    continue;
                }

                string nazwapliku = Path.GetFileNameWithoutExtension(plik);
                if (SposobSzukania == SposobSzukania.CalaSciezka)
                {
                    string ext = Path.GetExtension(plik);
                    if (string.IsNullOrEmpty(ext)) continue;
                    nazwapliku = TextHelper.PobierzInstancje.OczyscNazwePliku(plik.Replace(Sciezka, "", StringComparison.InvariantCultureIgnoreCase).Replace(ext, "").Replace("/", "\\")).Trim('-'); // test Path.GetFileNameWithoutExtension(plik);
                }
                string nazwa = nazwapliku.ToLower();
                List<string> plikiWnazwie = new List<string> {nazwa};

                if (!string.IsNullOrEmpty(SeparatorIdentyfikatorów))
                    plikiWnazwie = nazwa.Split(new[] {SeparatorIdentyfikatorów[0]}, StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim()).ToList();
         
                foreach (string identyfikatory in plikiWnazwie)
                {
                    List<string> identyfikatoryWNazwie = WyciagnijIdentyfikatorPlikuZNazwy(identyfikatory, separator);

                    switch (SzukanieIdentyfikatoraWNazwiePliku)
                    {
                        case SzukanieIdentyfikatora.PierwszyElement:
                        {
                            identyfikatoryWNazwie = new List<string>(1) {identyfikatoryWNazwie.FirstOrDefault()};
                        }
                            break;
                    }

                    foreach (var identyfikator in identyfikatoryWNazwie)
                    {
                        //Log.DebugFormat("plik {0} ma nastepnujace identyfikator produktow {1}", plik, identyfikator);
                        string oczyszczonyidentyfikator = TextHelper.PobierzInstancje.OczyscNazwePliku(identyfikator);
                      
                        List<Produkt> produkty = produktyPogrupowaneWgSzukania.ContainsKey(oczyszczonyidentyfikator)
                            ? produktyPogrupowaneWgSzukania[oczyszczonyidentyfikator]
                            : new List<Produkt>();
                        foreach (Produkt produkt in produkty)
                        {
                            if (!zdjeciaProduktow.ContainsKey(produkt.Id))
                                zdjeciaProduktow.Add(produkt.Id, new HashSet<string>());

                            zdjeciaProduktow[produkt.Id].Add(plik);
                            //Log.DebugFormat("plik {0} przypisany  do {1}, pasujacy identyfikator {2} identyfikator z nazwy pliku {3}",plik,produkt.Kod,oczyszczonyidentyfikator,identyfikator);
                        }
                    }
                }
            }

            if (plikiPominieteZPowoduRozmiaru.Any())
            {
                Logi.LogujInfo($"Pominięto plików: {plikiPominieteZPowoduRozmiaru.Count}, z powodu rozmiaru przekraczającego {RozmiarZdjeciaMaks}MB. Pominiete pliki: { string.Join(", ",plikiPominieteZPowoduRozmiaru) }" );
            }

            return zdjeciaProduktow;
        }

        /// <summary>
        /// Metoda do zmockowania która towrzy nowy plik z podanej ścieżki do pliku i przydziela mu identyfikator
        /// </summary>
        /// <param name="sciezkapliku">Ścieżka do pliku</param>
        /// <param name="nazwalokalna">Nazwa pliku (bez ścieżki)</param>
        /// <param name="licznikMinimalny">Licznik który będzie użyty do nadania identyfikatora</param>
        /// <returns>Nowy obiekt pliku</returns>
        public virtual Plik UtworzPlik(string sciezkapliku, string nazwalokalna, ref int licznikMinimalny)
        {
            FileInfo info = new FileInfo(sciezkapliku);

            Plik plik = new Plik
            {
                Data =
                    info.LastWriteTime.ToUniversalTime()
                        .AddMilliseconds(-info.LastWriteTime.ToUniversalTime().Millisecond),
                Nazwa = nazwalokalna,
                nazwaLokalna = info.Name,
                Rozmiar = (int)info.Length,
                Sciezka = info.DirectoryName + "\\",
                Id = --licznikMinimalny
            };

            return plik;
        }

        /// <summary>
        /// Metoda do zmockowania która sprawdza czy podany plik jest obrazkiem (na podstawie rozszerzenia)
        /// </summary>
        /// <param name="plik">Obiekt pliku</param>
        /// <returns>True jeśli plik jest obrazkiem i może być użyty jako zdjęcie</returns>
        public virtual bool ZweryfikujTypPliku(Plik plik)
        {
            return plik.RodzajPliku==RodzajPliku.Zdjecie;
        }

        /// <summary>
        /// Metoda która ustawia propertis CzyObrazek w obiekcie pliku żeby nie wykonywać za każdym razem logiki do sprawdzenia tego
        /// </summary>
        /// <param name="plik">Obiekt pliku</param>
        protected virtual void PRzypiszTypPliku(Plik plik)
        {
            plik.RodzajPliku = PlikBll.CzyPlikToZdjecie(plik)?RodzajPliku.Zdjecie : RodzajPliku.Zalacznik;
        }

        /// <summary>
        /// Metoda która dodaje do listy plików do wysyłki na platformę plik podany ze ścieżki
        /// </summary>
        /// <param name="plikiLokalnePowiazania">Lista wszystkich znalezionych powiązań do plików</param>
        /// <param name="plikiLokalne">Lista wszystkich znalezionych plików które mają powiązania</param>
        /// <param name="sciezkaPliku">Ścieżka do wybranego pliku</param>
        /// <param name="licznikMinimalny">Aktualny licznik dla ID pliku od którego kolejne ID są pomniejszane</param>
        /// <param name="separator">Separator w nazwie pliku</param>
        /// <param name="produktId">ID produktu dla którego będzie dodany łącznik do pliku</param>
        /// <param name="pliki">Słownik ze ściażkami plików w kluczu do sprawdzania czy plik jest zdjęciem głównym</param>
        /// <returns></returns>
        public int DodajPlikiDoProduktow(Dictionary<long,ProduktPlik> plikiLokalnePowiazania, List<Plik> plikiLokalne, string sciezkaPliku, int licznikMinimalny, string separator, long produktId, HashSet<string> pliki)
        {
            {
                FileInfo info = new FileInfo(sciezkaPliku);
                string nazwalokalna = info.Name;
                if (NazwaMapowanie)
                {
                   var path=   Path.GetPathRoot(sciezkaPliku);
                    string sciezka = sciezkaPliku.Replace(Path.GetExtension(sciezkaPliku),"");
                    if (!string.IsNullOrEmpty(path))
                    {
                        sciezka = sciezka.Replace(path, "", StringComparison.InvariantCultureIgnoreCase);
                    }
                    nazwalokalna = Texthelper.OczyscNazwePliku(sciezka)  + Path.GetExtension(info.Name);
                }
                Plik ptemp = UtworzPlik(sciezkaPliku, nazwalokalna, ref licznikMinimalny);
                PRzypiszTypPliku(ptemp);
                if (!ZweryfikujTypPliku(ptemp))
                {
                    return licznikMinimalny;
                }
                if (ptemp.Rozmiar > RozmiarZdjeciaMaks*1024*1024)
                {
                    Logi.LogujInfo($"Pomijanie pliku {info.Name} rozmiar przekracza {RozmiarZdjeciaMaks}MB");
                    return licznikMinimalny;
                }

                ptemp.PoprawNazwaPlikuDlaURL();
                Plik ptempIsteniejacy = plikiLokalne.FirstOrDefault(a => a.CzyTeSamePliki(ptemp));
                if (ptempIsteniejacy != null)
                {
                    ProduktPlik pp = new ProduktPlik
                    {
                        PlikId = ptempIsteniejacy.Id,
                        ProduktId = produktId,
                        Glowny = CzyZdjecieGlowne(pliki, ptempIsteniejacy)
                    };
                    long klucz = pp.Id;

                    if (!plikiLokalnePowiazania.ContainsKey(klucz))
                    {
                        plikiLokalnePowiazania.Add(klucz, pp);
                    }
                }
                else
                {
                    ProduktPlik pp = new ProduktPlik
                    {
                        PlikId = ptemp.Id,
                        ProduktId = produktId,
                        Glowny = CzyZdjecieGlowne(pliki, ptemp)
                    };
                    long klucz = pp.Id;
                    plikiLokalne.Add(ptemp);
                    plikiLokalnePowiazania.Add(klucz, pp);
                }
            }
            return licznikMinimalny;
        }

        /// <summary>
        /// Określa czy podany plik jest zdjęciem głównym
        /// </summary>
        /// <param name="identyfikatoryWNazwie">Hashset ze ścieżkami do plików, pierwszy wpis w słowniku to zdjęcie główne</param>
        /// <param name="p">Obiekt pliku</param>
        /// <returns>True jeśli plik jest zdjęciem głównym - jego ścieżka jest pierwszym elementem w słowniku</returns>
        private bool CzyZdjecieGlowne(HashSet<string> identyfikatoryWNazwie, Plik p)
        {
            switch (zdjecieGlowne)
            {
                case ZjecieGlowne.PierwszyWKolekcji:
                    return Path.Combine(p.Sciezka, p.nazwaLokalna) == identyfikatoryWNazwie.First();
                case ZjecieGlowne.OstatniParametr:
                    if (string.IsNullOrEmpty(Separator))
                    {
                        return true;
                    }
                    bool wartosc;
                    if (Refleksja.PrzetworzBool(p.NazwaBezRozszerzenia.Split(new[] {Separator[0]}, StringSplitOptions.RemoveEmptyEntries).Last(), out wartosc))
                    {
                        return wartosc;
                    }
                    break;
            }
            return false;
        }
    }

    public enum ZjecieGlowne
    {
        [FriendlyName("Ostatnia fraza w nazwie pliku - podzielona względem separatora")]
        OstatniParametr=1,
        [FriendlyName("Pierwszy element względem sortowania")]
        PierwszyWKolekcji =10
    }
}
