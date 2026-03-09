using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SolEx.Hurt.Core.BLL.ZadaniaDlaZamowienia;
using SolEx.Hurt.Core.DostepDane.DaneObiektu;
using SolEx.Hurt.Core.ExtensionRozszerzeniaKlas;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL
{
    public class ZadaniaBLL : LogikaBiznesBaza, IZadaniaBLL
    {
        public ZadaniaBLL(ISolexBllCalosc calosc)
            : base(calosc)
        {
        }

        /// <summary>
        /// Zwraca informacje czy określone zadanie jest aktywne.
        /// </summary>
        /// <typeparam name="T">Typ sprawdzany</typeparam>
        /// <returns></returns>
        public bool JestAktywneZadanie<T>()
        {
            string typ = typeof(T).PobierzOpisTypu();
            return Calosc.DostepDane.Pobierz<ZadanieBll>(null, x => !string.IsNullOrEmpty(x.ModulFullTypeName) && x.ModulFullTypeName == typ).Any();
        }

        public bool KlientMaDostep(ZadanieBll zadanie, IKlient klient)
        {
            if (klient == null)
            {
                return true;
            }
            if (!zadanie.Centralne && zadanie.OddzialId == null)
            {
                return true; //zadanie dla wszystkich
            }
            if (!zadanie.Centralne && zadanie.OddzialId == klient.OddzialDoJakiegoNalezyKlient)
            {
                return true;//klient należy do oddziału dla którego przypisane jest zadania
            }
            if (zadanie.Centralne && klient.OddzialDoJakiegoNalezyKlient == 0)
            {
                return true;//zadanie jest tylko dla klientów centrali i klient jest centrali
            }
            return false;//klient nie spełnia żadnej z reguł
        }

        public IList<ModulSynchronizacji> BindModulySynchronizacji(int jezyk, IKlient klient)
        {
            return Calosc.DostepDane.Pobierz<ZadanieBll>(jezyk, klient, x => x.PobierzGrupyDoJakichPasujeZadanie().Contains(TypZadania.Synchronizacja)).Select(x => new ModulSynchronizacji(x)).ToList();
        }

        public IList<ModulKoszyka> BindModulyKoszyka(int jezyk, IKlient klient)
        {
            return Calosc.DostepDane.Pobierz<ZadanieBll>(jezyk, klient, x => x.PobierzGrupyDoJakichPasujeZadanie().Contains(TypZadania.RegulaKoszyka)).Select(x => new ModulKoszyka(x)).ToList();
        }

        public IList<HarmonogramBll> BindHarmonogram(int jezyk, IKlient klient)
        {
            return
                Calosc.DostepDane.Pobierz<ZadanieBll>(jezyk, klient, x => x.NumerElementuSynchronizacji.HasValue)
                    .Select(x => new HarmonogramBll(x))
                    .ToList();
        }

        public IList<ModulPunktowy> BindModulyPunktowe(int jezyk, IKlient klient)
        {
            return Calosc.DostepDane.Pobierz<ZadanieBll>(jezyk, klient, x => x.PobierzGrupyDoJakichPasujeZadanie().Contains(TypZadania.RegulaPunktowa)).Select(x => new ModulPunktowy(x)).ToList();
        }

        public IList<ZadanieBll> BindingPoSelecie(int jezykId, IKlient zadajacy, IList<ZadanieBll> obj, object parametrDoMetodyPoSelect = null)
        {
            foreach (ZadanieBll zadaniaBll in obj.Where(x => !string.IsNullOrEmpty(x.ModulFullTypeName)))
            {
                zadaniaBll.Typ = Type.GetType(zadaniaBll.ModulFullTypeName);
            }
            return obj;
        }

        public IEnumerable<TT> PobierzZadania<T, TT>(int jezyk, IKlient klient)
        {
            //pobieranie zadań z bazy
            var zadanieWyfiltrowane = PobierzZadaniaWyfiltrowane<T, TT>(jezyk, klient);
            //tworzenie zadań 
            var wynik = zadanieWyfiltrowane.Select(x => x.Modul()).Cast<TT>();
            return wynik;
        }
        /// <summary>
        /// Pobiera zadania z bazy oraz filtruje
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TT"></typeparam>
        /// <param name="jezyk"></param>
        /// <param name="klient"></param>
        /// <returns></returns>
        public List<ZadanieBll> PobierzZadaniaWyfiltrowane<T, TT>(int jezyk, IKlient klient)
        {
            var zadania = Calosc.DostepDane.Pobierz<ZadanieBll>(jezyk, klient, x => x.Aktywne && (x.ModulFullTypeName != null && x.ModulFullTypeName != ""));
            var zadanieWyfiltrowane = zadania.Where(x => x.Typ.InheritsOrImplements<TT>() && x.Typ.InheritsOrImplements<T>()).ToList();
            return zadanieWyfiltrowane;
        }

        public IEnumerable<ZadanieCalegoKoszyka> PobierzZadaniaCalegoKoszykaKtorePasuja<T>(IKoszykiBLL koszyk) where T : IGrupaZadania
        {
            var tmp = PobierzZadania<T, ZadanieCalegoKoszyka>(koszyk.Klient.JezykId, koszyk.Klient);
            return tmp.Where(zadanieKoszyka => zadanieKoszyka.CzySpelniaKryteria(koszyk)).ToList();
        }

        public Dictionary<IKoszykPozycja, List<ZadaniePozycjiKoszyka>> PobierzZadaniaPozycjiKtorePasuja<T>(IKoszykiBLL koszykKlienta) where T : IGrupaZadania
        {
            var zadaniapozycjiwszystkie = PobierzZadania<T, ZadaniePozycjiKoszyka>(koszykKlienta.Klient.JezykId, koszykKlienta.Klient).ToList();
            Dictionary<IKoszykPozycja, List<ZadaniePozycjiKoszyka>> pozycjeZadania = new Dictionary<IKoszykPozycja, List<ZadaniePozycjiKoszyka>>();
            foreach (var kp in koszykKlienta.PobierzPozycje)
            {
                List<ZadaniePozycjiKoszyka> zadaniapozycji = zadaniapozycjiwszystkie.Where(x => x.CzySpelniaKryteria(kp, koszykKlienta)).ToList();
                if (zadaniapozycji.Any())
                {
                    pozycjeZadania.Add(kp, zadaniapozycji);
                }
            }
            return pozycjeZadania;
        }

       
        /// <summary>
        /// Zwraca datę zakończenia działania określonego modułu
        /// </summary>
        /// <param name="nazwa">Nazwa modułu</param>
        /// <returns>Data, jeśli brak to DateTime.Max, jeśli brak data to DateTime.Min</returns>
        public DateTime TerminOstatniegoUruchomienia(string nazwa)
        {
            var z = Calosc.DostepDane.Pobierz<ZadanieBll>(null, x => !string.IsNullOrEmpty(x.ModulFullTypeName) && x.ModulFullTypeName.IndexOf(nazwa, StringComparison.InvariantCultureIgnoreCase) > -1).FirstOrDefault();
            if (z == null)
            {
                return DateTime.MaxValue;
            }
            if (z.OstatnieUruchomienieKoniec.HasValue)
            {
                return z.OstatnieUruchomienieKoniec.Value;
            }
            return DateTime.MinValue;
        }

        private Dictionary<Type, IList<Tuple<string, int>>> _listaPolSynchronziacji = new Dictionary<Type, IList<Tuple<string, int>>>();

        public IList<Tuple<string, int>> SynchronizaowanePolaDlaTypu(Type typ)
        {
            if (!_listaPolSynchronziacji.ContainsKey(typ))
            {
                List<Tuple<string, int>> lista = new List<Tuple<string, int>>();
                var modultydsynchro = Calosc.DostepDane.Pobierz<ModulSynchronizacji>(null);
                foreach (ModulSynchronizacji mp in modultydsynchro)
                {
                    var modul = mp.Modul() as IModulPola;
                    if (modul == null)
                    {
                        continue;
                    }
                    var atr = modul.GetType().GetCustomAttributes(true).FirstOrDefault(a => a.GetType() == typeof(SynchronizowanePola)) as SynchronizowanePola;
                    if (atr != null)
                    {
                        if (atr.Typ != typ)
                        {
                            continue;
                        }
                        var wynik = modul.PobierzDostepnePola();
                        if (wynik == null)
                        {
                            continue;
                        }
                        wynik.ForEach(x => lista.Add(new Tuple<string, int>(x, mp.Id)));
                    }
                }
                _listaPolSynchronziacji.Add(typ, lista);
                return lista;
            }
            return _listaPolSynchronziacji[typ];
        }

        public bool CzyPoleJestSynchronizowane(Type typ, string nazwaPola, out int? id)
        {
            id = null;
            if (string.IsNullOrEmpty(nazwaPola))
                return false;

            PropertyInfo[] wartprop = typ.GetProperties();
            if (wartprop.All(x => x.Name != nazwaPola))
            {
                return false;
            }

            IList<Tuple<string, int>> lista = SynchronizaowanePolaDlaTypu(typ);
            //nie ma tego typu w ogole - to ma dostawać TRUE czyli ze jest synchronizowany
            if (!lista.Any())
            {
                return true;
            }

            id = lista.First().Item2;
            
            //jelsi jest na liscie to JEST synchronizaowany
            var elem = lista.FirstOrDefault(x => x.Item1 == nazwaPola);
            if (elem == null)
            {
                return true;
            }
            return false;
        }

        public void UsunZdublowaneModulySystemowe()
        {
            var obecneZadania = Calosc.DostepDane.Pobierz<ZadanieBll>(null, x => x.ModulWymagany).GroupBy(x => x.ModulFullTypeName).ToDictionary(x => x.Key, x => x);
            List<int> listaDoUsuniecia = new List<int>();
            foreach (var z in obecneZadania)
            {
                int id = z.Value.First().Id;
                listaDoUsuniecia.AddRange(from moduly in z.Value where moduly.Id != id select moduly.Id);
            }
            Calosc.DostepDane.Usun<ZadanieBll, int>(listaDoUsuniecia);
        }

        public void UsunCache(IList<object> obj)
        {
            Calosc.Cache.UsunGdzieKluczRozpoczynaSieOd(Calosc.DostepDane.KluczCacheTypDanych<ZadanieBll>());
        }

        public void SprawdzZadaniaSystemowe()
        {
            List<ZadanieBll> zmieniane = new List<ZadanieBll>();
            IList<ZadanieBll> obecneZadania = Calosc.DostepDane.Pobierz<ZadanieBll>(null);
            foreach (var z in obecneZadania)
            {
                if (string.IsNullOrEmpty(z.ModulFullTypeName))
                {
                    continue;
                }
                if (z.ModulFullTypeName.Count(x => x == ',') != 1)
                {
                    if (z.ModulFullTypeName.Count(x => x == 'x') > 1)
                    {
                        z.ModulFullTypeName = z.ModulFullTypeName.Substring(0, z.ModulFullTypeName.IndexOf(','));
                        zmieniane.Add(z);
                    }
                    else if (z.ModulFullTypeName.StartsWith("SolEx.Hurt.Sync.App.Modules"))
                    {
                        z.ModulFullTypeName += ",SolEx.Hurt.Sync.App.Modules_";
                        zmieniane.Add(z);
                    }
                    else if (z.ModulFullTypeName.StartsWith("SolEx.Hurt.Core"))
                    {
                        z.ModulFullTypeName += ",SolEx.Hurt.Core";
                        zmieniane.Add(z);
                    }
                }
                Type p = Type.GetType(z.ModulFullTypeName);
                if (p == null && !z.Usuniente)
                {
                    z.Usuniente = true;
                    zmieniane.Add(z);
                }
            }
            Calosc.DostepDane.AktualizujListe<ZadanieBll>(zmieniane);
            zmieniane.Clear();
            var vas = Enum.GetValues(typeof(ElementySynchronizacji));
            if (vas.Length == 0)
            {
                throw new Exception("Bład - nie odczytano typów zadań");
            }
            foreach (int e in vas)
            {
                int e1 = e;
                var zad = new HashSet<int>(obecneZadania.Where(z => z.NumerElementuSynchronizacji.HasValue && z.NumerElementuSynchronizacji.Value == e1)
                    .Select(x => x.Id));
                if (!zad.Any())
                {
                    ZadanieBll z = new ZadanieBll
                    {
                        Aktywne = true,
                        ModulKolejnosc = 99,
                        MozeDzialacOdGodziny = 0,
                        MozeDzialacDoGodziny = 24,
                        IleMinutCzekacDoKolejnegoUruchomienia = 60 * 24,
                        NumerElementuSynchronizacji = e,
                        JezykId = Calosc.Konfiguracja.JezykIDDomyslny
                    };
                    zmieniane.Add(z);
                }
            }
            Calosc.DostepDane.AktualizujListe<ZadanieBll>(zmieniane);
            zmieniane.Clear();
            obecneZadania = Calosc.DostepDane.Pobierz<ZadanieBll>(null);
            var wszystkie = Refleksja.PobierzListeKlasDziedziczacychPoKlasieBazowej(typeof(ModulStowrzonyNaPodstawieZadania));
            foreach (Type e in wszystkie)
            {
                if (e.IsAbstract)
                {
                    continue;
                }
                Type e1 = e;
                if (!e1.GetCustomAttributes(typeof(ModulStandardowy), false).Any())
                {
                    continue;
                }
                
                //tutaj juz ida tylko dodawanie automatycznych modulow

                ModulStowrzonyNaPodstawieZadania zadania = (ModulStowrzonyNaPodstawieZadania)Activator.CreateInstance(e1);
                int? idojca = null;
                if (zadania.JakiejOperacjiSynchronizacjiDotyczy != null)
                {
                    idojca = obecneZadania.FirstOrDefault(x => zadania.JakiejOperacjiSynchronizacjiDotyczy.Any(a => (int)a == x.NumerElementuSynchronizacji))?.Id ?? 0;

                }

                var zadw = obecneZadania.FirstOrDefault(x => x.ModulFullTypeName == e1.PobierzOpisTypu() && x.ModulWymagany);
                if (zadw == null)
                {
                    zadw = new ZadanieBll
                    {
                        Aktywne = true,
                        ModulWymagany = true,
                        ModulFullTypeName = e1.PobierzOpisTypu(),
                        ZadanieNadrzedne = idojca,
                        JezykId = Calosc.Konfiguracja.JezykIDDomyslny
                    };
                    var pars = OpisObiektu.PobierzParametry(e1);
                    zadw.UstawParametry(pars);
                }
                if (zadw.MozeDzialacDoGodziny != zadania.MozeDzialacDoGodziny || zadw.MozeDzialacOdGodziny != zadania.MozeDzialacOdGodziny || zadw.IleMinutCzekacDoKolejnegoUruchomienia != zadania.IleMinutCzekacDoKolejnegoUruchomienia)
                {
                    zmieniane.Add(zadw);
                }
                zadw.MozeDzialacDoGodziny = zadania.MozeDzialacDoGodziny;
                zadw.MozeDzialacOdGodziny = zadania.MozeDzialacOdGodziny;
                zadw.IleMinutCzekacDoKolejnegoUruchomienia = zadania.IleMinutCzekacDoKolejnegoUruchomienia;
            }
            Calosc.DostepDane.AktualizujListe<ZadanieBll>(zmieniane);
        }
    }
}