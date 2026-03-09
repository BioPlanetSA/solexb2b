using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using ServiceStack.Common;
using ServiceStack.DataAccess;
using ServiceStack.OrmLite;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using System.Collections.Concurrent;

namespace SolEx.Hurt.Core.BLL
{
    public class TresciDostep : LogikaBiznesBaza, ITresciDostep
    {
        public TresciDostep(ISolexBllCalosc calosc) : base(calosc)
        {
        }

        public void DodajDomyslneTresci()
        {
            IList<TrescBll> istniejace = Calosc.DostepDane.Pobierz<TrescBll>(null);
            List<TrescBll> nowe = new List<TrescBll>();
            if (istniejace.All(x => x.Symbol != "MenuGorne"))
            {
                nowe.Add(new TrescBll("Menu górne", "MenuGorne", Calosc.Konfiguracja.JezykIDDomyslny) { Systemowa = true, Dostep = AccesLevel.Wszyscy, Aktywny = true, PokazujWMenu = false });
            }
            if (istniejace.All(x => x.Symbol != "MenuDolne"))
            {
                nowe.Add(new TrescBll("Menu dolne", "MenuDolne", Calosc.Konfiguracja.JezykIDDomyslny) { Systemowa = true, Dostep = AccesLevel.Wszyscy, Aktywny = true, PokazujWMenu = false });
            }
            if (istniejace.All(x => x.Symbol != "StronySystemowe"))
            {
                nowe.Add(new TrescBll("Strony systemowe", "StronySystemowe", Calosc.Konfiguracja.JezykIDDomyslny) { Systemowa = true, Dostep = AccesLevel.Wszyscy, Aktywny = true, PokazujWMenu = false });
            }
            if (istniejace.All(x => x.Symbol != "MenuDodatkowe"))
            {
                nowe.Add(new TrescBll("Menu dodatkowe", "MenuDodatkowe", Calosc.Konfiguracja.JezykIDDomyslny) { Systemowa = true, Dostep = AccesLevel.Wszyscy, Aktywny = true, PokazujWMenu = false });
            }
            if (nowe.Any())
            {
                Calosc.DostepDane.AktualizujListe<TrescBll>(nowe);
                long idGlowne = nowe.First(x => x.Symbol == "MenuGorne").Id;
                nowe.Clear();
                WczotajStronyZDomyslnychSzablonow(idGlowne);
                nowe.Add(new TrescBll("Produkty", "Produkty", Calosc.Konfiguracja.JezykIDDomyslny) { Systemowa = true, Dostep = AccesLevel.Zalogowani, Aktywny = true, NadrzednaId = idGlowne });
                Calosc.DostepDane.AktualizujListe<TrescBll>(nowe);
            }
        }

        public bool SprawdzDostep(TrescBll obiekt, IKlient zadajacy)
        {
            if (!obiekt.Aktywny)
            {
                return false;
            }

            if (obiekt.Dostep != AccesLevel.Wszyscy && obiekt.Dostep != zadajacy.Dostep)
            {
                return false;
            }
            if (obiekt.Rola != null && obiekt.Rola.Any())
            {
                if (!zadajacy.Role.Intersect(obiekt.Rola).Any())
                {
                    return false;
                }
            }

            return true;
        }

        public void Sprawdz(IList<TrescBll> dane)
        {
            for (int i = 0; i < dane.Count; i++)
            {
                TrescBll tb = dane[i];

                //link alternatyny jesli jest to malymi literami ma byc
                if (!string.IsNullOrEmpty(tb.LinkAlternatywny))
                {
                    tb.LinkAlternatywny = tb.LinkAlternatywny.ToLower();
                }

                //symbol musi byc null albo jakis konkretny, zcasem z JS przychodzi Empty
                if (tb.Symbol == "")
                {
                    tb.Symbol = null;
                }

               

                if (!string.IsNullOrEmpty(tb.Symbol))
                {
                    tb.Symbol = tb.Symbol.Trim();
                    var istniejace = Calosc.DostepDane.Pobierz<TrescBll>(null, x => !string.IsNullOrEmpty(x.Symbol) && x.Symbol.Equals(tb.Symbol, StringComparison.InvariantCultureIgnoreCase) && x.Id != tb.Id);
                    if (istniejace.Any())
                    {
                        tb.Symbol += " Kopia " + istniejace.Count;
                    }
                }
                if (!string.IsNullOrEmpty(tb.TrescPokazywanaJakoLeweMenu))
                {
                    var istniejace = Calosc.DostepDane.Pobierz<TrescBll>(null, x => x.Symbol == tb.TrescPokazywanaJakoLeweMenu);
                    if (!istniejace.Any())
                    {
                        tb.TrescPokazywanaJakoLeweMenu = null;
                    }
                }
                if (!string.IsNullOrEmpty(tb.TrescPokazywanaJakoNaglowek))
                {
                    var istniejace = Calosc.DostepDane.Pobierz<TrescBll>(null, x => x.Symbol == tb.TrescPokazywanaJakoNaglowek);
                    if (!istniejace.Any())
                    {
                        tb.TrescPokazywanaJakoNaglowek = null;
                    }
                }
                if (!string.IsNullOrEmpty(tb.TrescPokazywanaJakoReklamaMenu))
                {
                    var istniejace = Calosc.DostepDane.Pobierz<TrescBll>(null, x => x.Symbol == tb.TrescPokazywanaJakoReklamaMenu);
                    if (!istniejace.Any())
                    {
                        tb.TrescPokazywanaJakoReklamaMenu = null;
                    }
                }
                if (!string.IsNullOrEmpty(tb.TrescPokazywanaJakoStopka))
                {
                    var istniejace = Calosc.DostepDane.Pobierz<TrescBll>(null, x => x.Symbol == tb.TrescPokazywanaJakoStopka);
                    if (!istniejace.Any())
                    {
                        tb.TrescPokazywanaJakoStopka = null;
                    }
                }
            }
        }

        public void ResetujSzablon(int id, List<TrescBllImport> wzor)
        {
            foreach (TrescBllImport element in wzor)
            {
                TrescBll tb = new TrescBll();
                tb.KopiujPola(element, new { element.Wiersze, element.Id });
                tb.JezykId = Calosc.Konfiguracja.JezykIDDomyslny;
                tb.NadrzednaId = id;

                Calosc.DostepDane.AktualizujPojedynczy(tb);
                foreach (var w in element.Wiersze)
                {
                    TrescWierszBll tw = new TrescWierszBll();
                    tw.KopiujPola(w, new { w.Id, w.TrescId });
                    tw.TrescId = (int)tb.Id;
                    tw.JezykId = tb.JezykId;
                    Calosc.DostepDane.AktualizujPojedynczy(tw);
                    List<TrescKolumnaBll> kolumny = new List<TrescKolumnaBll>();
                    foreach (var k in w.Kolumny)
                    {
                        TrescKolumnaBll tk = new TrescKolumnaBll();
                        tk.KopiujPola(k, new { k.Id, k.TrescWierszId });
                        tk.TrescWierszId = tw.Id;
                        tk.JezykId = tw.JezykId;
                        kolumny.Add(tk);
                    }
                    Calosc.DostepDane.AktualizujListe<TrescKolumnaBll>(kolumny);
                }
                ResetujSzablon((int)tb.Id, element.Dzieci);
            }
        }

        public List<string> IstniejaceSzablony()
        {
            string sciezka = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DomyslnaKonfiguracja\\SzablonyTresci");
            if (!Directory.Exists(sciezka))
            {
                Directory.CreateDirectory(sciezka);
            }
            return Directory.GetFiles(sciezka, "*.json").Select(Path.GetFileNameWithoutExtension).ToList();
        }

        public void WczotajStronyZDomyslnychSzablonow(long idGlowneMenu)
        {
            string sciezka = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DomyslnaKonfiguracja\\DomyslneSzablony\\");
            if (!Directory.Exists(sciezka))
            {
                Directory.CreateDirectory(sciezka);
            }
            string[] filePaths = Directory.GetFiles(sciezka, "*.json");
            foreach (var filePath in filePaths)
            {
                string tresc = File.ReadAllText(filePath);
                List<TrescBllImport> importy = JSonHelper.Deserialize<List<TrescBllImport>>(tresc);
                DodajBrakujaceSzablony(importy, idGlowneMenu);
            }
        }

        public void DodajBrakujaceSzablony(List<TrescBllImport> wzor, long id)
        {
            foreach (TrescBllImport element in wzor)
            {
                TrescBll tb = new TrescBll();
                tb.KopiujPola(element, new { element.Wiersze, element.Id });
                tb.JezykId = Calosc.Konfiguracja.JezykIDDomyslny;
                tb.NadrzednaId = id;

                Calosc.DostepDane.AktualizujPojedynczy(tb);
                foreach (var w in element.Wiersze)
                {
                    TrescWierszBll tw = new TrescWierszBll();
                    tw.KopiujPola(w, new { w.Id, w.TrescId });
                    tw.TrescId = (int)tb.Id;
                    tw.JezykId = tb.JezykId;
                    Calosc.DostepDane.AktualizujPojedynczy(tw);
                    List<TrescKolumnaBll> kolumny = new List<TrescKolumnaBll>();
                    foreach (var k in w.Kolumny)
                    {
                        TrescKolumnaBll tk = new TrescKolumnaBll();
                        tk.KopiujPola(k, new { k.Id, k.TrescWierszId });
                        tk.TrescWierszId = tw.Id;
                        tk.JezykId = tw.JezykId;
                        kolumny.Add(tk);
                    }
                    Calosc.DostepDane.AktualizujListe<TrescKolumnaBll>(kolumny);
                }
            }
        }

        public List<TrescBllImport> WczytajSzablonDyskowy(string nazwa)
        {
            string sciezka = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DomyslnaKonfiguracja\\SzablonyTresci\\", nazwa + ".json");
            string tresc = File.ReadAllText(sciezka);
            return JSonHelper.Deserialize<List<TrescBllImport>>(tresc);
        }

        /// <summary>
        /// Pobiera stronę główna
        /// </summary>
        /// <param name="klient">Klient dla którego pobieramy stronę główną</param>
        /// <param name="jezyk">Język w jakim chcemy stronę główną</param>
        /// <returns></returns>
        public TrescBll PobierzStroneGlowna(IKlient klient, int jezyk)
        {
            TrescBll parent = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<TrescBll>(x => x.Symbol == "MenuGorne", jezyk, klient, false);
            if (parent == null)
            {
                throw new HttpException(500, "Nie można zneleść kategorii o symbolu menugorne");
            }
            TrescBll pierwsza = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz(jezyk, klient, x => !string.IsNullOrEmpty(x.LinkAlternatywny) && x.NadrzednaId == parent.Id, new[] { new SortowanieKryteria<TrescBll>(x => x.Kolejnosc, KolejnoscSortowania.asc, "Kolejnosc") }).FirstOrDefault();
            if (pierwsza == null)
            {
                throw new HttpException(500, "Nie ma żadnej kategorii widocznej w menu głównym");
            }
            return pierwsza;
        }

        public string PobierzStopke(int jezyk)
        {
            if (CzyElementWylaczony_ZnakiFirmoweSolex("stopka"))
            {
                return "";
            }
            string domyslnaStopka = "System działa na platformie SolEx B2B";

            string tlumaczenie = Calosc.Konfiguracja.PobierzTlumaczenie(jezyk, domyslnaStopka);
            if (string.IsNullOrEmpty(tlumaczenie) || tlumaczenie.IndexOf("solex b2b",StringComparison.InvariantCultureIgnoreCase)<0)
            {
                return domyslnaStopka;
            }
            return tlumaczenie;
        }

        public string PobierzStopkeMaile(IKlient klient, int jezyk)
        {
            if (CzyElementWylaczony_ZnakiFirmoweSolex("stopka-maile"))
            {
                return "";
            }

            string domyslnaStopka = "Wiadomość wysłana z platformy SolEx B2B";

            string tlumaczenie = Calosc.Konfiguracja.PobierzTlumaczenie(jezyk, domyslnaStopka);
            if (string.IsNullOrEmpty(tlumaczenie) || tlumaczenie.IndexOf("solex b2b",StringComparison.InvariantCultureIgnoreCase)<0)
            {
                return domyslnaStopka;
            }
            return "<br/><br/> <span  style='color: #dedede'>" + tlumaczenie + "</span>";
        }

        public string PobierzStopkeNewsletterow(IKlient klient, int jezyk, string linkDoWypisania)
        {
            if (CzyElementWylaczony_ZnakiFirmoweSolex("stopka-newsletter"))
            {
                return "";
            }

            string domyslnaStopka ="Otrzymałeś/aś tę wiadomość email, ponieważ twój adres jest zapisany do naszego newslettera.<br/> <a href='{0}' style='color: #919191'>Rezygnuje z otrzymywania kolejnych emaili</a>";
       
            string tlumaczenie = Calosc.Konfiguracja.PobierzTlumaczenie(jezyk, domyslnaStopka, linkDoWypisania);

            tlumaczenie += "<br/><br/> <span  style='color: #dedede'>" + this.PobierzStopkeMaile(klient, jezyk) + "</span>";


            if (string.IsNullOrEmpty(tlumaczenie) || tlumaczenie.IndexOf("solex b2b", StringComparison.InvariantCultureIgnoreCase) < 0)
            {
                return domyslnaStopka;
            }
            return tlumaczenie;
        }

        public string PobierzAutora(int jezyk)
        {
            if (CzyElementWylaczony_ZnakiFirmoweSolex("autor"))
            {
                return "";
            }
            string domyslnyAutor = "Solex";

            string tlumaczenie = Calosc.Konfiguracja.PobierzTlumaczenie(jezyk, domyslnyAutor);
            if (string.IsNullOrEmpty(tlumaczenie) || tlumaczenie.IndexOf("solex", StringComparison.InvariantCultureIgnoreCase) < 0)
            {
                return domyslnyAutor;
            }
            return tlumaczenie;
        }

        private Dictionary<string, bool> wylaczenia = new Dictionary<string, bool>();

        private bool CzyElementWylaczony_ZnakiFirmoweSolex(string rodzaj)
        {
            if (!wylaczenia.ContainsKey(rodzaj))
            {
                string sciezka = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format("{0}-bez.txt", rodzaj));
                wylaczenia.Add(rodzaj, File.Exists(sciezka));
            }
            return wylaczenia[rodzaj];
        }

        public bool CzyTrescOtwieranaJakoModal(TrescBll tresc, IKlient klient)
        {
            return tresc.SposobOtwierania == SposobOtwieraniaModal.ZalogowaniModalNiezalogowaniModal
                || (klient.Dostep == AccesLevel.Zalogowani && tresc.SposobOtwierania == SposobOtwieraniaModal.ZalogowaniModalNiezalogowaniNieModal)
                || (klient.Dostep == AccesLevel.Niezalogowani && tresc.SposobOtwierania == SposobOtwieraniaModal.ZalogowaniNieModalNiezalogowaniModal);
        }

        private ConcurrentDictionary<int, bool?> _czyMenuUkrywaneTresci = new ConcurrentDictionary<int, bool?>();

        /// <summary>
        /// Metoda zwraca info czy dana kontrolka menu zbudowana jest z treści ktore sa ukrywane / poakzywane wg. klientow - maja widocznosci ustawione na cokolwiek. Metoda pomocna dla cachowania menu
        /// </summary>
        /// <param name="id">id kontrolki menu</param>
        /// <param name="parent">opcjonalny parametr tresc dla ktorych sprawdzamy dzieci</param>
        /// <returns>Zwraca tak/ nie lub null - gdzie null oznacza ze nie wiadomo bo nie podany parent</returns>
        public bool? SprawdzCzyKontrolaMenuMaTresciUkrywaneWgKlientow(int id, TrescBll parent = null)
        {
            bool? czySaUkrywaneTresci = false;

            czySaUkrywaneTresci = _czyMenuUkrywaneTresci.GetOrAdd(id, (z) => {

                if (parent == null)
                {
                    return null;
                }

               return parent.Dzieci.Any(x => x.Widocznosc != null);
            });

            return czySaUkrywaneTresci;
        }

        /// <summary>
        /// Binding po aktualizacji/dodaniu wiersza który czyści prywatna zmienna cache warszy w obiekcie tresci
        /// </summary>
        /// <param name="obj"></param>
        public void WyczyscCacheWierszy(IList<TrescWierszBll> obj)
        {
            HashSet<int> idTresci = new HashSet<int>( obj.Select(x => x.TrescId) );
            WyczyscCacheWierszyDlaTresciOId(idTresci);
        }

        /// <summary>
        /// Binding po usunieciu wiersza który czyści prywatna zmienna cache warszy w obiekcie tresci
        /// </summary>
        /// <param name="obj"></param>
        public void UsunCacheWierszy(IList<object> obj)
        {
            var tresciId = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<TrescWierszBll>(null, x => Sql.In(x.Id, obj)).Select(x=>x.TrescId);
            WyczyscCacheWierszyDlaTresciOId(tresciId);
        }

        /// <summary>
        /// Motoda czyszcząca prywatną zmienną wierszy dla treści o id przekazanych jako parametr do metody
        /// </summary>
        /// <param name="idTresci"></param>
        private void WyczyscCacheWierszyDlaTresciOId(IEnumerable<int> idTresci)
        {
            var tresci = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<TrescBll>(null, x => Sql.In(x.Id, idTresci));

            foreach (var tresc in tresci)
            {
                tresc.WyczyscCacheWierszy();
            }
        }

        //public IList<TrescBll> BindingPoSelect(int jezykID, IKlient klient, IList<TrescBll> listaTresci, object opcjonalmnyParametr)
        //{
        //    if (klient==null || klient.CzyAdministrator || klient.Dostep == AccesLevel.Niezalogowani)
        //        return listaTresci;

        //    IList<TrescBll> wszystkieTresci = Calosc.DostepDane.Pobierz<TrescBll>(jezykID, null, null, new List<SortowanieKryteria<TrescBll>> { new SortowanieKryteria<TrescBll>(x => x.Kolejnosc, KolejnoscSortowania.asc, "Kolejnosc") });
        //    foreach (TrescBll trescBll in listaTresci)
        //    {
        //        var tmp1 = wszystkieTresci.Where(x => x.NadrzednaId == trescBll.Id);
        //        trescBll.DzieciMajaUkrywanaWidocznosc =true; //wszystkieTresci.Any(x => x.NadrzednaId == trescBll.Id && Calosc.WidocznosciTypowBll.KlientMaDostepDoObiektu(klient,x));
        //    }
        //    return listaTresci;
        //}
    }
}