using System.Reflection;
using System.Text.RegularExpressions;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FastMember;
using ServiceStack.Text;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Sync.App.Modules_.Helpers;
using SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty;


namespace SolEx.Hurt.Sync.App.Modules_.Rozne
{
    public abstract class OpisyDoProduktowZPlikowBaza : SyncModul
    {
        private IConfigSynchro _configBll = SyncManager.PobierzInstancje.Konfiguracja;

        public IConfigSynchro _ConfigBLL
        {
            get { return _configBll; }
            set { _configBll = value; }
        }

        public Tools _Tools { get; set; } = Tools.PobierzInstancje;

        public virtual LogiFormatki _LogiFormatki { get; set; } = LogiFormatki.PobierzInstancje;

        public TypeAccessor akcesorRefleksjiProduktow = typeof(Produkt).PobierzRefleksja();

        [FriendlyName("Ścieżka do katalogu z plikami opisów - np. c:\\pliki", FriendlyOpis = "Nie wolno dawać c:\\pliki\\en!!!!! tylko c:\\pliki")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Sciezka { get; set; }

        [Niewymagane]
        [FriendlyName("Pojedyńczy znak który rozdziela od siebie nazwę pliku, nazwę produktu dla tłumaczenia i cyfrę określającą kolejność opisu. np: <b>SKU123$1.txt</b> gdzie kod produktu to SKU123 - naszym separatorem jest znak $ a opis, do którego będzie dodana treść ma numer 1. Jeśli w nazwie pliku jest zawarta nazwa tłumaczenia dla pliku to nazwa może mieć format <b>SKU123$nazwa produktu dla tłumaczenia$1.txt</b>. Jeśli pole jest puste, to opis z pliku będzie dodany do pola opis na B2B")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Separator { get; set; }

        [Niewymagane]
        [FriendlyName("Uzupełniaj kolejność opisów z plików wg klucza np 1;2;3;4;5 lub A;B;C;D;E itd. Nie jest używane jeśli separator jest pusty")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string KluczOpisow { get; set; }

        //[FriendlyName("Czy usuwać z nazwy pliku separator i kod produktu?")]
        //public bool CzyUsuwacSeparatorIKodZpliku { get; set; }

        [FriendlyName("Po jakim polu z produktu dopasowywać pliki?")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public TypyPolDoDopasowaniaZdjecia PoCzymSzukacPlikow { get; set; }

        [FriendlyName("Czy Nazwa pliku ma być dokładnie dopasowana?")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool DokladneDopasowanieDoNazwyPliku { get; set; }

        [FriendlyName("Określa czy odczytywany plik ma być kodowany w UTF8 czy kodowanie będzie dopasowane automatycznie")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public Opisy.KodowanieOpisow Kodowanie { get; set; }

        [FriendlyName("Rozszerzenie plików, z których będzie odczytany opis")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Rozszerzenie { get; set; }

        [FriendlyName("Określa który opis nie będzie czyszczony przy synchronizacji")]
        [PobieranieSlownika(typeof(SlownikPolProduktowTylkoOpisy))]
        [Niewymagane]
        public List<string> IgnorowanyOpis { get; set; }

        [FriendlyName("Czy usunąć stare opisy(nawet te zaimpoprtowane z erpa) ")]
        [PobieranieSlownika(typeof(SlownikPolProduktowTylkoOpisy))]
        [Niewymagane]
        public bool CzyscStareOpisy { get; set; }

        public override string uwagi => "Przy synchronizacji produktów opisy zostaną nadpisane domyślnymi wartościami. Należy włączyć moduł dodatkowy o nazwie KtorePolaSynchronizowac i wybrać w nim pola z opisami.";

        public OpisyDoProduktowZPlikowBaza()
        {
            Sciezka = "";
            Separator = "$";
            //CzyUsuwacSeparatorIKodZpliku = false;
            PoCzymSzukacPlikow = TypyPolDoDopasowaniaZdjecia.Kod;
            KluczOpisow = "1;2;3;4;5";
            Kodowanie = Opisy.KodowanieOpisow.UTF8;
            Rozszerzenie = ".txt";
            DokladneDopasowanieDoNazwyPliku = false;
            CzyscStareOpisy = false;
        }

        private string PoprawRozszerzenie(string rozszerzenie)
        {
            if (!rozszerzenie.Contains("."))
                rozszerzenie = "." + rozszerzenie;

            return rozszerzenie;
        }


        private void DodajPlikiProduktu(string klucz, List<string> pliki, ref Dictionary<string, List<string>> slownik)
        {
            if (pliki.Count == 0 || string.IsNullOrEmpty(klucz))
                return;

            if (!slownik.ContainsKey(klucz))
                slownik.Add(klucz, new List<string>());

            slownik[klucz].AddRange(pliki.OrderBy(a => a));
        }

        public virtual string[] PobierzPliki(string Sciezka)
        {
            return Directory.GetFiles(Sciezka, "*" + PoprawRozszerzenie(Rozszerzenie), SearchOption.AllDirectories);
        }

        public void PrzetworzBaza(ref List<Produkt> produktyNaB2B, ref List<Tlumaczenie> slowniki)
        {
            if (string.IsNullOrEmpty(Sciezka) || !Directory.Exists(Sciezka))
            {
                throw new Exception($"Brak katalogu: '{Sciezka}' skonfigurowanego w module({this.Nazwa}) lub katalog nie istnieje. Sprawdź konfiguracje modułu.");
            }

            if (produktyNaB2B == null)
                produktyNaB2B = new List<Produkt>();

            Przetwarzaj(ref produktyNaB2B, ref slowniki);
        }

        public void Przetwarzaj(ref List<Produkt> produktyNaB2B, ref List<Tlumaczenie> slowniki)
        {
            string[] plikiWKatalogu = PobierzPliki(Sciezka);
          //  TypWSystemie produktTyp = _ConfigBLL.SystemTypes.FirstOrDefault(a => a.Nazwa == typeof(Produkt).FullName);
            string produktTyp = (typeof (ProduktBazowy)).PobierzOpisTypu();
            _LogiFormatki.LogujInfo("Plików z opisami: " + plikiWKatalogu.Length);
            
            Dictionary<string, string> slownikPlikow = StworzSlownikPlikow(plikiWKatalogu);
            //przygotowanie kolekji produkty
            Dictionary<string, List<string>> plikiProduktow  = DopasujPlikiDoProduktow(produktyNaB2B, slownikPlikow);


            foreach (KeyValuePair<string, List<string>> dict in plikiProduktow)
            {
                List<Produkt> produkty = null;
                switch (PoCzymSzukacPlikow)
                {
                    case TypyPolDoDopasowaniaZdjecia.KodKreskowy: produkty = produktyNaB2B.AsParallel().Where(p => p.KodKreskowy == dict.Key).ToList();
                        break;

                    case TypyPolDoDopasowaniaZdjecia.Rodzina:
                        {
                            produkty = produktyNaB2B.AsParallel().Where(p => p.Rodzina == dict.Key).ToList();
                        }
                        break;

                    case TypyPolDoDopasowaniaZdjecia.Kod: produkty = produktyNaB2B.AsParallel().Where(p => p.Kod == dict.Key).ToList();
                        break;
                    case TypyPolDoDopasowaniaZdjecia.Idproduktu:

                        int i;
                        if (int.TryParse(dict.Key, out i))
                        {
                            produkty = produktyNaB2B.AsParallel().Where(p => p.Id == i).ToList();
                        }
                        break;
                        
                    case TypyPolDoDopasowaniaZdjecia.PoleTekst1:
                        {
                            produkty = produktyNaB2B.AsParallel().Where(p => !string.IsNullOrEmpty(p.PoleTekst1) && p.PoleTekst1.Equals(dict.Key, StringComparison.InvariantCultureIgnoreCase)).ToList();
                        }
                        break;
                    case TypyPolDoDopasowaniaZdjecia.PoleTekst2:
                    {
                        produkty = produktyNaB2B.AsParallel().Where(p => !string.IsNullOrEmpty(p.PoleTekst2) && p.PoleTekst2.Equals(dict.Key, StringComparison.InvariantCultureIgnoreCase)).ToList();
                    }
                        break;
                    default:
                        break;
                }
                

                if (produkty != null && produkty.Count == 0)
                    continue;

                PropertyInfo[] prop = typeof(Produkt).GetProperties().Where(a => a.Name.StartsWith("Opis")).ToArray();

                foreach (Produkt produkt in produkty)
                {
                    if (CzyscStareOpisy)
                    {
                        //resetujemy opisy, w razie jak by plik zniknął
                        foreach (PropertyInfo info in prop)
                        {
                            if (IgnorowanyOpis == null || !IgnorowanyOpis.Contains(info.Name))
                            {
                                info.SetValue(produkt, "");
                            }
                        }
                    }

                    string[] kolejnoscOpisow = KluczOpisow.Split(new [] {";"},
                        StringSplitOptions.RemoveEmptyEntries);

                    foreach (string plikOpisu in dict.Value)
                    {
                        string symboljezyka = PobierzSymbolJezykaZeSciezki(plikOpisu, Sciezka).ToLower();
                    //    Log.DebugFormat(" plik {0} symbol jezyka {1}", plikOpisu, symboljezyka);
                        KeyValuePair<int, Jezyk> jezyk = _ConfigBLL.JezykiWSystemie.FirstOrDefault(a => a.Value.Symbol.ToLower() == symboljezyka);
                    //    Log.DebugFormat(" plik {0}   jezyk.Value {1}", plikOpisu, jezyk.Value);
                        string index = "";
                        if ((jezyk.Value != null && jezyk.Value.Domyslny) || string.IsNullOrEmpty(symboljezyka) || symboljezyka.Length > 2)
                        {
                            if (!string.IsNullOrEmpty(Separator))
                            {
                                index = ZnajdzKolejnoscOpisu(plikOpisu, kolejnoscOpisow, Separator);
                            }
                            if (index != "-1")
                                ZmienOpis(index, plikOpisu, produkt);
                        }
                        else if (jezyk.Value != null)
                        {
                            FileInfo plik = new FileInfo(plikOpisu);
                            bool czyMaKolejnosc = CzyWNazwiePlikuJestKolejnosc(plikOpisu, kolejnoscOpisow, Rozszerzenie, Separator);

                            string[] elementyPliku = plik.Name.Split(new string[] {Separator, Rozszerzenie}, StringSplitOptions.RemoveEmptyEntries);

                            //najprawdopodobniej w nazwie pliku jest tłumaczenie nazwy dla danego produktu
                            if ((czyMaKolejnosc && elementyPliku.Length >= 3) || (!czyMaKolejnosc && elementyPliku.Length == 2))
                            {
                                string nazwa = elementyPliku[1];
                                ZmienOpisWSlowniku(nazwa, produkt, ref slowniki, jezyk.Value, produktTyp, "Nazwa");
                            }

                            var opis = Tools.PobierzInstancje.PobierzZawartoscPlikuTekstowegoZFormatowaniem(plikOpisu,Kodowanie, true);
                           
                            if (!string.IsNullOrEmpty(Separator))
                            {
                                index = ZnajdzKolejnoscOpisu(plikOpisu, kolejnoscOpisow, Separator);
                            }
                            if (index != "-1")
                                ZmienOpisWSlowniku(opis, produkt, ref slowniki, jezyk.Value, produktTyp, "Opis" + index);

                        }
                    }
                }
              
            }
        }

        public Dictionary<string, List<string>> DopasujPlikiDoProduktow(List<Produkt> produktyNaB2B, Dictionary<string, string> slownikPlikow)
        {

            Dictionary<string, List<string>> plikiProduktow = new Dictionary<string, List<string>>(StringComparer.InvariantCultureIgnoreCase);
            Dictionary<string, string> slownikZOyczszczonaNazwa = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var element in slownikPlikow)
            {
                string klucz = TextHelper.PobierzInstancje.OczyscNazwePliku(element.Key);
                if (slownikZOyczszczonaNazwa.ContainsKey(klucz))
                {
                    Log.ErrorFormat($"Plik o nazwie {element.Key} znajduje się już w kolekcji, jego klucz to {klucz}");
                    Log.ErrorFormat($"Plik wcześniej dodany: {slownikZOyczszczonaNazwa[klucz].TrimStart(Sciezka.ToCharArray())}, jego klucz to {klucz}");
                }
                else
                {
                    slownikZOyczszczonaNazwa.Add(klucz, element.Value);
                }
                Log.DebugFormat($"Dodawanie do slownika plikow klucz {klucz}, warosc {element.Value}");
            }

            foreach (var produkt in produktyNaB2B)
            {
                string klucz = PlikiHelper.WygenerujKluczeDopasowania(PoCzymSzukacPlikow, produkt);
                string szukanasep = klucz + Separator;
                string szukanaroz = klucz + TextHelper.PobierzInstancje.OczyscNazwePliku(Rozszerzenie);
                string regexNazwa = @"\\"+DopasujDoRegex(klucz + Separator) + ((string.IsNullOrEmpty(Separator))?"":".*")+Rozszerzenie+"$";
                Log.DebugFormat($"produkt {klucz}, klucz sep {szukanasep}, klucz roz {szukanaroz}");
                if (string.IsNullOrEmpty(klucz))
                {
                    continue;
                }
            
                string kluczs = "";
                switch (PoCzymSzukacPlikow)
                {
                    case TypyPolDoDopasowaniaZdjecia.Kod:
                        kluczs = produkt.Kod;
                        break;
                    case TypyPolDoDopasowaniaZdjecia.Rodzina:
                        kluczs = produkt.Rodzina;
                        break;
                    case TypyPolDoDopasowaniaZdjecia.KodKreskowy:
                        kluczs = produkt.KodKreskowy;
                        break;
                    case TypyPolDoDopasowaniaZdjecia.Idproduktu:
                        kluczs = produkt.Id.ToString();
                        break;
                    case TypyPolDoDopasowaniaZdjecia.PoleTekst1:
                        kluczs = produkt.PoleTekst1;
                        break;
                    case TypyPolDoDopasowaniaZdjecia.PoleTekst2:
                        kluczs = produkt.PoleTekst2;
                        break;
                }
                List<string> listaPasujacychPlikow = slownikZOyczszczonaNazwa.Where(a => a.Key.Contains(szukanasep) || a.Key.Contains(szukanaroz)).Select(a => a.Value).ToList();
                if (DokladneDopasowanieDoNazwyPliku)
                {
                    Log.DebugFormat($"Znaleziono przed dokładnym  {listaPasujacychPlikow.Count}");
                    Regex newRegex = new Regex(regexNazwa,RegexOptions.IgnoreCase);
                    listaPasujacychPlikow = listaPasujacychPlikow.Where(a => newRegex.IsMatch(a)).ToList();
                    Log.DebugFormat($"Znaleziono {listaPasujacychPlikow.Count}");
                }
                Log.DebugFormat($"produkt {klucz}, klucz sep {szukanasep}, klucz roz {szukanaroz}");
                Log.DebugFormat($"Znaleziono {listaPasujacychPlikow.Count}");
                DodajPlikiProduktu(kluczs, listaPasujacychPlikow, ref plikiProduktow);
            }
            Log.DebugFormat($"Dopasowanych plików {plikiProduktow.Count}");
            return plikiProduktow;
        }

        public Dictionary<string, string> StworzSlownikPlikow(string[] plikiWKatalogu)
        {
            Dictionary<string, string> slownikPlikow = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
          //  int poczatekklucza = 0;//rozwala ceramizyery i nie wiem po co To jest
            foreach (string plik in plikiWKatalogu)
            {
                string klucz = new FileInfo(plik).Name.ToLower();
                if (!slownikPlikow.ContainsKey(klucz))
                {
                    slownikPlikow.Add(klucz, plik);
                }
            }
            return slownikPlikow;
        }

        public string ZnajdzKolejnoscOpisu(string plikOpisu, string[] kolejnoscOpisow, string separator)
        {
            int index = 1;
            if (kolejnoscOpisow.Length > 0 && !plikOpisu.Contains(separator + "."))
            {
                
                foreach (string ko in kolejnoscOpisow)
                {
                    if (plikOpisu.Contains(separator + ko.Trim() + "."))
                    {
                        if (index != 1)
                            return index.ToString();

                        return string.Empty;
                    }
                    ++index;
                }
            }

            //jeśli w nazwie pliku nie ma separatora tzn że wrzucamy to do głównego opisu czyli do pola opis w produkcie
            if ( /*kolejnoscOpisow.Length == 0 && */ !plikOpisu.Contains(separator + ".") && index==1 )
            {
                return string.Empty;
            }

            return "-1";
        }

        private string PobierzSymbolJezykaZeSciezki(string sciezka, string katalog)
        {
            string dir = Path.GetDirectoryName(sciezka).Replace(katalog,"").TrimStart('\\');
            if (string.IsNullOrEmpty(dir))
            {
                return string.Empty;
            }

            if (!dir.Contains('\\'))
            {
                return dir;
            }

            string symbolJezyka = dir.Substring(0,dir.IndexOf('\\'));
            return symbolJezyka;
        }

        private bool CzyWNazwiePlikuJestKolejnosc(string nazwapliku, string[] kolejnoscOpisow, string rozszerzenie, string separator)
        {
            bool wynik = false;
            foreach (string s in kolejnoscOpisow)
            {
                if (nazwapliku.EndsWith(separator + s + rozszerzenie))
                    wynik = true;
            }

            return wynik;
        }

        private void ZmienOpisWSlowniku(string tresc, Produkt produkt, ref List<Tlumaczenie> produktyTlumaczenia, Jezyk jezyk, string typ, string nazwaPola)
        {
            Tlumaczenie s = produktyTlumaczenia.FirstOrDefault(p => p.Typ == typ && p.ObiektId == produkt.Id && p.JezykId == jezyk.Id && p.Pole == nazwaPola);
            if (s == null)
            {
                Tlumaczenie nowy = new Tlumaczenie
                {
                    JezykId = jezyk.Id,
                    ObiektId = produkt.Id,
                    Pole = nazwaPola,
                    Typ = typ,
                    Wpis = tresc
                };
                produktyTlumaczenia.Add(nowy);
            }
            else s.Wpis = tresc;
        }

        private void ZmienOpis(string sindex, string plik, Produkt produkt)
        {
            Log.DebugFormat($"Zmieniam Opis dla produktu: {produkt.Kod}");
            if (produkt == null)
            {
                throw new Exception("Brak produktu");
            }
            PropertyInfo propertisy = typeof(Produkt).GetProperties().FirstOrDefault(a => a.Name.Equals("Opis" + sindex,StringComparison.InvariantCultureIgnoreCase));
            
            if (propertisy != null)
            {
                string opis = _Tools.PobierzZawartoscPlikuTekstowegoZFormatowaniem(plik, Kodowanie, true);
               
                string staryOpis = akcesorRefleksjiProduktow[produkt, propertisy.Name]?.ToString();
#if DEBUG
                if (string.IsNullOrEmpty(staryOpis))
                {
                    Log.Debug("Brak starego opisu");
                }
#endif
                if (string.IsNullOrEmpty(staryOpis) || staryOpis != opis)
                {
                    akcesorRefleksjiProduktow[produkt, propertisy.Name] = opis;
                }
            }
        }

        private string DopasujDoRegex(string nazwa)
        {
            string znakiSpecjalne = @".$^{[(|)]}*+\?";
            string nowaNazwa = "";
            foreach (char c in nazwa)
            {
                if (znakiSpecjalne.Contains(c))
                {
                    nowaNazwa += '\\';
                }
                nowaNazwa += c;
            }
            return nowaNazwa;
        }

        

        public override string Opis
        {
            get { return "Przeszukuje określony katalog i dopasowuje pliki txt z opisami do produktów. Jeśli opisy mają być wczytywane w różnych językach, należy w wybranym katalogu zrobić podfoldery o nazwach identycznych jak symbole języków w B2B np. EN. Opisy znajdujące się w takim katalogu będą wczytywane do odpowiedniego języka"; }
        }
    }
}
