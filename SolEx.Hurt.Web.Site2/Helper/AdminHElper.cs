using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Web.Site2.Helper.DaneListaAdmin;
using SolEx.Hurt.Web.Site2.Helper.DaneListaAdmin.DodatkoweFunkcjeModuly;
using SolEx.Hurt.Web.Site2.Helper.DaneListaAdmin.KolorowanieWierszy;
using SolEx.Hurt.Web.Site2.Helper.DaneListaAdmin.OperacjeZbiorcze;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using FastMember;
using SolEx.Hurt.Core.DostepDane.DaneObiektu;
using SolEx.Hurt.Core.ExtensionRozszerzeniaKlas;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Web.Site2.Helper
{
    public class AdminHelper : BllBaza<AdminHelper>
    {
        private readonly Dictionary<string, GrupaMenuBind> _grupyMenu = new Dictionary<string, GrupaMenuBind>();
        private readonly Dictionary<Type, UprawnnieniaAdmin> _uprawnieAdmin = new Dictionary<Type, UprawnnieniaAdmin>();
        private readonly Dictionary<Type, EdycjaAdminPobraniePolObiektu> _pokazaniePolEdycja = new Dictionary<Type, EdycjaAdminPobraniePolObiektu>();

        /// <summary>
        /// Dodaje zakładki do menu admina
        /// </summary>
        /// <param name="nazwa">Nazwa zakładki</param>
        /// <param name="kolejnosc">Kolejność</param>
        /// <param name="walidatorWidocznosci">Walidator czy aktualny klient ma dostęp</param>
        public void DodajGrupeMenu(string nazwa, int kolejnosc = 0, Func<IKlient, bool> walidatorWidocznosci = null)
        {
            if (_grupyMenu.ContainsKey(nazwa))
            {
                throw new ArgumentException(string.Format(@"Próba ponownego dodania tego samego elementu {0}", nazwa), "nazwa");
            }
            _grupyMenu.Add(nazwa, new GrupaMenuBind(nazwa, kolejnosc, walidatorWidocznosci));
        }

        /// <summary>
        /// Dodaje własną pozycje do menu w adminie
        /// </summary>
        /// <param name="nazwa">Wyświetlana nazwa</param>
        /// <param name="link">Link jaki ma być wywołany</param>
        /// <param name="grupa">Grupa w jakiej ma się pokazać</param>
        /// <param name="kolejnosc">Kolekność</param>
        /// <param name="walidatorWidocznosci">Walidator czy aktualny klient ma dostęp</param>
        public void DodajWlasnaAkcjeDoMenu(string nazwa, string link, string grupa, Type t,int kolejnosc = 0, Func<IKlient, bool> walidatorWidocznosci = null)
        {
            if (!_grupyMenu.ContainsKey(grupa))
            {
                throw new ArgumentException(string.Format(@"Próba dodania elementu do nieistniejącej grupy {0}", grupa), "grupa");
            }
            if (_grupyMenu[grupa].Pozycje.ContainsKey(nazwa))
            {
                throw new ArgumentException(string.Format(@"Próba ponownego dodania tego samego elementu o nazwie {0} do grupy {1}", nazwa, grupa), "nazwa");
            }
            _grupyMenu[grupa].Pozycje.Add(nazwa, new OpcjaWGrupieBind(nazwa, link, kolejnosc, walidatorWidocznosci,t));
        }
        

        /// <summary>
        /// Ustawienie funkcji walidacji usuwania, edycji jest OBSOLUTE tutaj. Należy korzystać z obuektu DaneObiekt - i tak budować funkcje ktore przyjmuje rowniez obiekt dla ktorego wyliczamy dane funkcje
        /// </summary>
        /// <typeparam name="TDane"></typeparam>
        /// <param name="nazwa"></param>
        /// <param name="grupa"></param>
        /// <param name="kolejnosc"></param>
        /// <param name="walidatorWidocznosci"></param>
        /// <param name="walidatorEdycji"></param>
        /// <param name="walidatorDodawania"></param>
        /// <param name="walidatorUsuwania"></param>
        /// <param name="metodaPobieraniaPol"></param>
        /// <param name="akcjaZapisz"></param>
        /// <param name="akcjaDodaj"></param>
        public void DodajAutomatycznaListeObiektow<TDane>(string nazwa, string grupa, int kolejnosc = 0, Func<IKlient, bool> walidatorWidocznosci = null,
            Func<IKlient, bool> walidatorEdycji = null, Func<IKlient, bool> walidatorDodawania = null, Func<IKlient, bool> walidatorUsuwania = null,
            Func<object, object, IList<OpisPolaObiektu>> metodaPobieraniaPol = null, string akcjaZapisz = "ZapiszObiekt", string akcjaDodaj = "Dodaj")
        {
            Type t = typeof(TDane);
            string link = "/Admin/Lista?typ=" + t.PobierzOpisTypu();
            DodajWlasnaAkcjeDoMenu(nazwa, link, grupa,t, kolejnosc, walidatorWidocznosci);
            DodajUprawnieniaAdmin<TDane>(walidatorWidocznosci, walidatorEdycji, walidatorDodawania, walidatorUsuwania);
            DodajaEdycjePolAdmin<TDane>(metodaPobieraniaPol, akcjaZapisz, akcjaDodaj);
        }

        public void DodajUprawnieniaAdmin<TDane>(Func<IKlient, bool> walidatorWidocznosci = null, Func<IKlient, bool> walidatorEdycji = null, Func<IKlient, bool> walidatorDodawania = null, Func<IKlient, bool> walidatorUsuwania = null)
        {
            Type t = typeof(TDane);
            if (_uprawnieAdmin.ContainsKey(t))
            {
                throw new ArgumentException(string.Format(@"Próba ponownego dodania tego samego do kolekci uprawnień {0}", t.PobierzOpisTypu()));
            }

            _uprawnieAdmin.Add(t, new UprawnnieniaAdmin(walidatorEdycji, walidatorDodawania, walidatorUsuwania));
        }

        public void DodajaEdycjePolAdmin<TDane>(Func<object, object, IList<OpisPolaObiektu>> metoda, string akcjaZapisz = "ZapiszObiekt", string akcjaDodaj = "Dodaj")
        {
            Type t = typeof(TDane);
            if (_pokazaniePolEdycja.ContainsKey(t))
            {
                throw new ArgumentException(string.Format(@"Próba ponownego dodania tego samego do kolekcji metod edycji {0}", t.PobierzOpisTypu()));
            }
            if (metoda == null)
            {
                metoda = OpisObiektu.PobranieParametowObiektu;
            }
            _pokazaniePolEdycja.Add(t, new EdycjaAdminPobraniePolObiektu(metoda, akcjaZapisz, akcjaDodaj));
        }

        private EdycjaAdminPobraniePolObiektu PobierzEdycjePol(Type typ)
        {
            if (!_pokazaniePolEdycja.ContainsKey(typ))
            {
                _pokazaniePolEdycja.Add(typ, new EdycjaAdminPobraniePolObiektu(OpisObiektu.PobranieParametowObiektu, "ZapiszObiekt", "Dodaj"));
            }
            return _pokazaniePolEdycja[typ];
        }

        private Dictionary<long, List<ElementMenu>> _slownikMenuDlaKlienta;

        internal IEnumerable<ElementMenu> PobierzMenu(IKlient zadajacy)
        {
            
            List<ElementMenu> wynik;
            if (_slownikMenuDlaKlienta == null)
            {
                _slownikMenuDlaKlienta=new Dictionary<long, List<ElementMenu>>();
            }
            else if (_slownikMenuDlaKlienta.TryGetValue(zadajacy.Id, out wynik))
            {
                return wynik;
            }
            //Brak klienta w slowniku
            wynik=new List<ElementMenu>();
            foreach (GrupaMenuBind g in _grupyMenu.Values.OrderBy(x=>x.Kolejnosc))
            {
                if (g.Walidator != null)
                {
                    if (!g.Walidator(zadajacy))
                    {
                        continue;
                    }
                }
                List<ElementMenu> podrzedne = new List<ElementMenu>();
                foreach (var p in g.Pozycje.Values.OrderBy(x => x.Kolejnosc))
                {
                    if (p.Walidator != null)
                    {
                        if (!p.Walidator(zadajacy))
                        {
                            continue;
                        }
                    }
                    //W słowniku którym budujemy elementy menu przechowujemy nazwe a jako wartosc md5 danej pozycji. W warunku tym następuje sprawdzenie czy klient ma zawęzoną widocznosc jeżęli tak to czy widzi konkretny element menu
                    if (zadajacy.JakieElementyMenu != null && zadajacy.JakieElementyMenu.Any())
                    {
                        string wartoscMd5 = Tools.PobierzInstancje.GetMd5Hash(p.Link);
                        if (!zadajacy.JakieElementyMenu.Contains(wartoscMd5))
                        {
                            continue;
                        }
                    }
                    ElementMenu tmpp = new ElementMenu(p.Nazwa, p.Link, null, p.Typ);
                    podrzedne.Add(tmpp);
                }
                if (!podrzedne.Any())
                {
                    continue;
                }
                ElementMenu tmp = new ElementMenu(g.Nazwa, null, podrzedne,null);
                wynik.Add(tmp);
            }
            _slownikMenuDlaKlienta.Add(zadajacy.Id, wynik);
            return wynik;
        }

        public DaneLista PobierzDaneEdytora( IKlient zadajacy, Type typDanych, int numerStrony, int rozmiarStrony, string[] szukanie, string sortowanie, KolejnoscSortowania kierunek, IList<OpisPolaObiektuBaza> kolumny, Jezyk AktualnyJezyk, UrlHelper url)
        {
            long lacznie;
            Expression filtr = null;

            if (szukanie != null && !szukanie.All(string.IsNullOrEmpty))
            {
                filtr = SolexBllCalosc.PobierzInstancje.Szukanie.StworzWhereEpression(typDanych, kolumny.Select(x => x.NazwaPola).ToArray(), szukanie, true);
            }

            IList<object> dane = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzWgTypu(typDanych, zadajacy, filtr, sortowanie, kierunek, numerStrony, rozmiarStrony, AktualnyJezyk.Id, out lacznie);

            return PobierzDaneEdytora(dane, lacznie, zadajacy, typDanych, numerStrony, rozmiarStrony, szukanie, sortowanie, kierunek, kolumny, url);
        }


        private void PobierzSpecyficzneFunkcjeObiektu(Type typDanych, out List<DodatkoweFunkcjeBaza> moduly,out List<KolorowanieBaza> kolorowanie, out OpisObiektu opis, out UprawnnieniaAdmin uprawnieniaAdminaGlowne, out IParametrBindowaniaPobieraniaDanych parametryBindowaniaPobieraniaDanych, UrlHelper url )
        {
            var dft = Refleksja.PobierzListeKlasDziedziczacychPoKlasieBazowej(typeof(DodatkoweFunkcjeBaza));
            List<DodatkoweFunkcjeBaza> m = dft.Select(x => (DodatkoweFunkcjeBaza)Activator.CreateInstance(x)).ToList();
            //Wyciągamy wszystkie dla danego typu i domyślne 
            moduly = m.Where(x => x.OblugiwanyTyp() == typDanych || x.OblugiwanyTyp() == typeof(DaneObiekt)).ToList();
            foreach (var mod in moduly)
            {
                mod.Url = url;
            }


            var modkol = Refleksja.PobierzListeKlasDziedziczacychPoKlasieBazowej(typeof(KolorowanieBaza));
            List<KolorowanieBaza> k = modkol.Select(x => (KolorowanieBaza)Activator.CreateInstance(x)).ToList();
            kolorowanie = k.Where(x => x.OblugiwanyTyp() == typDanych).ToList();
            
            opis = OpisObiektu.StworzOpisObiektu(typDanych);

            var bindowanie = typeof(DostepDoDanych).GetMethod("PobierzBindowaniaPobieraniaDanych").MakeGenericMethod(typDanych);
            parametryBindowaniaPobieraniaDanych = bindowanie.Invoke(SolexBllCalosc.PobierzInstancje.DostepDane, new object[0]) as IParametrBindowaniaPobieraniaDanych;

            uprawnieniaAdminaGlowne = null;
            if (_uprawnieAdmin.ContainsKey(typDanych))
            {
                uprawnieniaAdminaGlowne = _uprawnieAdmin[typDanych];
            }
        }

        private DaneObiekt PobierzDaneObiektu(object obiekt, List<DodatkoweFunkcjeBaza> moduly, List<KolorowanieBaza> kolorowanie, OpisObiektu opis, UprawnnieniaAdmin uprawnieniaAdminaGlowne,
            IKlient zadajacyKlient)
        {
            string klucz = (opis.PoleKlucz.GetValue(obiekt) ?? "").ToString();
            DaneObiekt daneObiektu = new DaneObiekt(klucz);

            if (kolorowanie != null)
            {
                foreach (KolorowanieBaza m in kolorowanie)
                {
                    //daneObiektu.KolorTla = m.KolorWiersza(obiekt);
                    daneObiektu.KlasaCssWiersza = m.KlasaCssWiersza(obiekt);
                }
            }
            List<DodatkoweFunkcjeBaza> modulyDodatkowe = moduly.Where(x => x.OblugiwanyTyp() == typeof(DaneObiekt)).ToList();
            List<DodatkoweFunkcjeBaza> modulyDlaObiektu = moduly.Where(x => x.OblugiwanyTyp() == obiekt.GetType()).ToList();
            if (modulyDlaObiektu.Any())
            {
                //najpier wykonujemy moduly dla danego typu bez domyślnych
                foreach (DodatkoweFunkcjeBaza modulDodatkowy in modulyDlaObiektu)
                {
                    IList<DodatkowaFunkcja> funkcje = modulDodatkowy.PobierzDodatkoweFunkcjeDlaObiektu(obiekt);
                    if (funkcje != null)
                    {
                        daneObiektu.DodajFunkcje(funkcje);
                    }
                    daneObiektu.NazwaObiektu = modulDodatkowy.PobierzNazweObiektu(obiekt);
                    daneObiektu.PrzyjaznyOpisObiektu = modulDodatkowy.PobierzOpisObiektu(obiekt);
                    daneObiektu.Komunikaty = modulDodatkowy.KomunitatyNaEdycjiObiektu(obiekt);
                }

                bool? usuwac = modulyDlaObiektu.First().MoznaUsuwacObiekt(obiekt);
                if (usuwac.HasValue)
                {
                    daneObiektu.MoznaUsuwac = usuwac.Value;
                }

                bool? edytowac = modulyDlaObiektu.First().MoznaEdytowacObiekt(obiekt);
                if (edytowac.HasValue)
                {
                    daneObiektu.MoznaEdytowac = edytowac.Value;
                }
            }

                daneObiektu.TypObiektu = obiekt.GetType();
            //nakladanie zakazów odgórnych z listy admina- global asax ogolne dla obiektu zakazy

            if (uprawnieniaAdminaGlowne != null)
            {
                if (daneObiektu.MoznaUsuwac && uprawnieniaAdminaGlowne.UprawnieUsuwanie != null)
                {
                    daneObiektu.MoznaUsuwac = uprawnieniaAdminaGlowne.UprawnieUsuwanie(zadajacyKlient);
                }
                if (daneObiektu.MoznaEdytowac && uprawnieniaAdminaGlowne.UprawnieEdycja != null)
                {
                    daneObiektu.MoznaEdytowac = uprawnieniaAdminaGlowne.UprawnieEdycja(zadajacyKlient);
                }
            }

            //Dodaje funkcje domyślne czyli usuń i edytuj wraz z alertami i komunikatami na liście 
            //Dodaje w tym miejscu ponieważ tutaj wiadomo już czy mozna usunąć obiekt i czy można edytować obiekt 
            if (modulyDodatkowe.Any())
            {
                foreach (DodatkoweFunkcjeBaza modulDodatkowy in modulyDodatkowe)
                {
                    IList<DodatkowaFunkcja> funkcjeDomyslne = modulDodatkowy.PobierzDodatkoweFunkcjeDlaObiektu(daneObiektu);
                    if (funkcjeDomyslne != null)
                    {
                        daneObiektu.DodajFunkcje(funkcjeDomyslne);
                    }
                }
            }


            if (string.IsNullOrEmpty(daneObiektu.NazwaObiektu))
            {
                FriendlyNameAttribute opisy = obiekt.GetType().GetCustomAttribute<FriendlyNameAttribute>();
                daneObiektu.NazwaObiektu = (opisy != null) ? opisy.FriendlyName : obiekt.GetType().Name;
            }
            if (string.IsNullOrEmpty(daneObiektu.OpisObiektu()))
            {
                FriendlyNameAttribute opisy = obiekt.GetType().GetCustomAttribute<FriendlyNameAttribute>();
                daneObiektu.NazwaObiektu = (opisy != null) ? opisy.FriendlyOpis : "";
            }


            return daneObiektu;
        }

        public DaneLista PobierzDaneEdytora(IList<object> dane, long lacznie, IKlient zadajacy, Type typDanych, int numerStrony, int rozmiarStrony, string[] szukanie, 
            string sortowanie, KolejnoscSortowania kierunek, IList<OpisPolaObiektuBaza> kolumny, UrlHelper url)
        {
            List<DaneObiekt> obiekty = new List<DaneObiekt>();
            UprawnnieniaAdmin uprawnieniaAdminaGlowne = null;
            OpisObiektu opis = null;
            List<DodatkoweFunkcjeBaza> moduly = null;
            List<KolorowanieBaza> kolorowanie = null;
            List<Komunikat> komunikaty = new List<Komunikat>();
            IParametrBindowaniaPobieraniaDanych parametryBindowaniaTypu = null;

            PobierzSpecyficzneFunkcjeObiektu(typDanych, out moduly, out kolorowanie, out opis, out uprawnieniaAdminaGlowne, out parametryBindowaniaTypu, url);

            //komuniaty maja byc zawsze -nawet jak nie ma danych
            foreach (DodatkoweFunkcjeBaza dodatkoweFunkcjeBaza in moduly)
            {
                komunikaty = dodatkoweFunkcjeBaza.KomunitatyNaLiscieObiektu(typDanych);
            }

            if (dane.Any())
            {
                foreach (var k in kolumny)
                {
                    //jesli obiekt jest SQL to wylaczamy sortowanie i filtrowanie po tej kolumnie
                    if (parametryBindowaniaTypu.FiltrySql && k.PoleIgnorowaneWBazieSQL)
                    {
                        k.AdminDozwoloneFiltrowanieLiscie = false;
                        k.AdminDozwoloneSortowanieLiscie = false;
                    }
                }

                foreach (var d in dane)
                {
                    DaneObiekt daneObiektu = PobierzDaneObiektu(d, moduly, kolorowanie, opis, uprawnieniaAdminaGlowne, zadajacy);

                    foreach (OpisPolaObiektuBaza k in kolumny)
                    {
                        var obiektBazowy = new OpisPolaObiektuBaza(k.Property, daneObiektu.Klucz);
                        OpisPolaObiektu danepola = new OpisPolaObiektu(obiektBazowy,d);
                        //danepola.PobierzWartoscPolaObiektu(d);
                        OznaczPolaSynchronizowalne(danepola, d, SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDDomyslny);
                        DanePole pole = new DanePole(danepola);
                        if (kolorowanie != null)
                        {
                            foreach (KolorowanieBaza m in kolorowanie)
                            {
                               // pole.KolorTla = m.KolorPola(d, danepola);
                                pole.KlasaCssPola = m.KlasaCssPola(d);
                            }
                        }

                        daneObiektu.DodajPole(pole);
                    }
                    obiekty.Add(daneObiektu);
                }
            }

            IList<OperacjaZbiorczaBaza> modul = PobierzOperacjeZbiorcze(typDanych);

            bool czyMoznaDodawacNowe = uprawnieniaAdminaGlowne != null && uprawnieniaAdminaGlowne.UprawnieDodawanie != null && uprawnieniaAdminaGlowne.UprawnieDodawanie(zadajacy);
            DaneLista wynik = new DaneLista(typDanych, kolumny, obiekty, numerStrony, rozmiarStrony, lacznie, szukanie, sortowanie, kierunek, zadajacy, modul, czyMoznaDodawacNowe,komunikaty);
            var edycja = PobierzEdycjePol(typDanych);
            wynik.AkcjaDodaj = edycja.AkcjaDodaj;
            return wynik;
        }

        public DaneEdycjaAdmin PobierzDaneDoEdycji(object id, Type typ, Type rodzajDanychNadrzednych, string nadrzedny, int jezyk, IKlient zadajacyKlient, UrlHelper url)
        {
            //TODO: wczesniej ostani parmetr tworz gdy brak byl na 1 teraz zmienilem na 0 
            object dane = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojednczyWgTypu(typ, null, id.ToString(), jezyk, false);
            if (dane == null)
            {
                throw new Exception("Brak obiektu do edycji - podane ID: " + id);
            }
            return PobierzDaneObjektuDoEdycji(dane, id, typ, rodzajDanychNadrzednych, nadrzedny, jezyk, zadajacyKlient, url);
        }

        public DaneEdycjaAdmin PobierzDaneDoDodania(Type typ, Type rodzajDanychNadrzednych, string nadrzedny, int jezyk, IKlient zadajacyKlient, UrlHelper url)
        {
            object dane = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojednczyWgTypu(typ, null, null, jezyk, true);           
            return PobierzDaneObjektuDoEdycji(dane, null, typ, rodzajDanychNadrzednych, nadrzedny, jezyk, zadajacyKlient, url);
        }

        public List<OpisPolaObiektu> PobierzPolaObiektu(EdycjaAdminPobraniePolObiektu edycja, object dane, object id, Type typ, int jezyk, Type rodzajDanychNadrzednych, string nadrzedny)
        {
            List<OpisPolaObiektu> pola = edycja.MetodaPobieraniaPol(dane, id).ToList();
            List<OpisPolaObiektu> wynik = new List<OpisPolaObiektu>(pola.Count);
            foreach (var p in pola)
            {
                var atr = p.Property.GetCustomAttribute<IdentyfikatorObiektuNadrzednego>();
                LinkDokumentacji link = p.Property.GetCustomAttribute<LinkDokumentacji>(true);
                p.LinkDoDokumentacji = link == null || string.IsNullOrEmpty(link.Link) ? null : link.Link;
                if (!string.IsNullOrEmpty(nadrzedny) && atr != null && rodzajDanychNadrzednych != null && atr.Rodzaj == rodzajDanychNadrzednych)
                {
                    continue;   //nie okazujemy pola identyfikujące obiekt nadrzedny
                }
                wynik.Add(p);
            }

            var linki = typeof(UrlExtender).GetMethods();
            foreach (MethodInfo methodInfo in linki)
            {
                var tmp = methodInfo.GetCustomAttributes<LinkGeneratorAttribute>().Where(x => x.Typ == typ);
                if (tmp.Any())
                {
                    WidoczneListaAdminAttribute w = methodInfo.GetCustomAttribute<WidoczneListaAdminAttribute>(true);

                    OpisPolaObiektuBaza nowePole = new OpisPolaObiektuBaza(w, new GrupaAtttribute("Linki", 99));
                    var atribut = methodInfo.GetCustomAttribute<FriendlyNameAttribute>(true);
                    nowePole.NazwaPola = methodInfo.Name;
                    nowePole.OpisPola = atribut != null? atribut.FriendlyOpis:"";
                    nowePole.TypPrzechowywanejWartosci = methodInfo.ReturnType;
                    nowePole.NazwaWyswietlana = atribut != null ? atribut.FriendlyName : nowePole.NazwaPola;
                    nowePole.TylkoDoOdczytu = true;

                    wynik.Add(new OpisPolaObiektu(nowePole));
                }
            }
            wynik.ForEach(x => x.RodzajDanychObiektuNadrzednego = typ);

            if (SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDDomyslny != jezyk)
            {
                wynik.RemoveAll(x => !x.Tlumaczone);
            }

            //tylko jesli jest podane ID - jak nie ma ID, czyli dodajmy nowy obiekt to nie ma sensu sprawdzac synchronziacji
            if (id != null)
            {
                wynik.ForEach(x => OznaczPolaSynchronizowalne(x, dane, jezyk));
            }

            return wynik;
        }

        public DaneEdycjaAdmin PobierzDaneObjektuDoEdycji(object dane, object id, Type typ, Type rodzajDanychNadrzednych, string nadrzedny, int jezyk, IKlient zadajacyKlient, UrlHelper url)
        {
            EdycjaAdminPobraniePolObiektu edycja = PobierzEdycjePol(typ);
            List<OpisPolaObiektu> wynik = PobierzPolaObiektu(edycja, dane, id, typ, jezyk, rodzajDanychNadrzednych, nadrzedny);
            wynik.RemoveAll(x => !x.ParamatryWidocznosciAdmin.EdycjaWidoczny);
            List<Jezyk> jezyki = null;
            if (SolexBllCalosc.PobierzInstancje.Konfiguracja.WieleJezykowWSystemie)
            {
                if (wynik.Any(x => x.Tlumaczone))
                {
                    jezyki = SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykiWSystemie.Values.ToList();
                }
            }
            
            OpisObiektu opis;
            List<DodatkoweFunkcjeBaza> moduly;
            List<KolorowanieBaza> kolorowanie;
            UprawnnieniaAdmin uprawnieniaAdmina;
            IParametrBindowaniaPobieraniaDanych parametryBindowaniaTypu;
            PobierzSpecyficzneFunkcjeObiektu(typ, out moduly, out kolorowanie, out opis, out uprawnieniaAdmina, out parametryBindowaniaTypu, url);
            DaneObiekt daneObiektu = PobierzDaneObiektu(dane, moduly, kolorowanie, opis, uprawnieniaAdmina, zadajacyKlient);

            return new DaneEdycjaAdmin(daneObiektu)
            {
                AkcjaZapisz = edycja.AkcjaZapisz,
                KluczWartosc = id == null ? "" : id.ToString(),
                PolaObiektu = wynik.ToArray(),
                Typ = typ,
                TypNadrzednych = rodzajDanychNadrzednych,
                Nadrzedny = nadrzedny,
                JezykId = jezyk,
                Jezyki = jezyki
            };
        }

        private void OznaczPolaSynchronizowalne(OpisPolaObiektuBaza pole, object obiekt, int jezyk)
        {
            if (jezyk != SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDDomyslny)//w jezyku innym niz pl mozna zawsze edytować
            {
                return;
            }
            IPolaIDentyfikujaceRecznieDodanyObiekt ident = obiekt as IPolaIDentyfikujaceRecznieDodanyObiekt;
            if (ident != null && ident.RecznieDodany())
            {
                pole.PobieraneErp = false;
            }
            else
            {
                int? idmodultu;
                var typ = obiekt.GetType().PobierzPodstawowyTyp().PobierzBazowy();
                bool czySynchroERP = SolexBllCalosc.PobierzInstancje.ZadaniaBLL.CzyPoleJestSynchronizowane(typ, pole.NazwaPola, out idmodultu);
                pole.PobieraneErp = czySynchroERP;
                pole.PobieraneErpModul = idmodultu;
            }
        }

        public IList<OperacjaZbiorczaBaza> PobierzOperacjeZbiorcze(Type typ)
        {
            var moduly = Refleksja.PobierzListeKlasDziedziczacychPoKlasieBazowej(typeof(OperacjaZbiorczaBaza));
            List<OperacjaZbiorczaBaza> k = moduly.Select(x => (OperacjaZbiorczaBaza)Activator.CreateInstance(x)).Where(x => x.OblugiwanyTyp() == typ).ToList();
            return k;
        }

        public void WykonajZadanieZbiorcze(Type modul, OpisPolaObiektu[] data, string[] wybrane)
        {
            OperacjaZbiorczaBaza zad = (OperacjaZbiorczaBaza)Activator.CreateInstance(modul);
            Refleksja.UstawWartoscPol(zad, data.PolaNaslownik());
            zad.Wykonaj(wybrane);
        }

        /// <summary>
        /// Metoda zmienia wybrane pole w danym obiekcie
        /// </summary>
        /// <param name="typ">typ obiektu do zmiany</param>
        /// <param name="klucz">id obiektu który zmieniamy</param>
        /// <param name="polaDoZmiany">pola do zmiany w obiekcie</param>
        /// <param name="jezykId">jezyk id obiektu do zmiany</param>
        /// <param name="akcesorTypu">akcesor do zmian refleksji</param>
        /// <returns></returns>
        internal object AktualizujPoleObiektu(Type typ, string klucz, Dictionary<string, object> polaDoZmiany, int jezykId, TypeAccessor akcesorTypu)
        {
            object obiekt = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojednczyWgTypu(typ, null, klucz, jezykId, false);

            if (obiekt == null)
            {
                throw new Exception("Nie można znaleźć obiektu");
            }

            foreach (var pole in polaDoZmiany)
            {
                try
                {
                    akcesorTypu[obiekt, pole.Key] = pole.Value;
                } catch (Exception e)
                {
                    Log.Debug($"Błąd przy aktualizacji obiektu id: {klucz} i typie: {typ}, pole: [{pole.Key}], wartość: [{pole.Value}]. Błąd:  {e.Message}.", e);
                    throw new Exception($"Nie można ustawić pola: {pole.Key}");
                }
            }
            //dane.Wartosc = Uri.UnescapeDataString(dane.Wartosc.ToString());
            try
            {
                SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujWgTypu(obiekt);
            } catch (Exception e)
            {
                Log.Debug($"Błąd zapisu obiektu id: {klucz} i typie: {typ}. Błąd:  {e.Message}.", e);
                throw new Exception("Nie można zapisać zmienionego obiektu.");
            }
            //OznaczPolaSynchronizowalne(oo, obiekt, SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDDomyslny);
            return obiekt;
        }

        internal string ZapiszObiekt(DaneEdycjaAdmin dane)
        {
            object obiekt = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojednczyWgTypu(dane.Typ, null, dane.KluczWartosc, dane.JezykId, true);
            UzupelnijDane(obiekt, dane);
            object klucz = SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujWgTypu(obiekt);
            return klucz.ToString();
        }

        public void UzupelnijDane(object obiekt, DaneEdycjaAdmin dane)
        {
            List<OpisPolaObiektu> polawart = new List<OpisPolaObiektu>(dane.PolaObiektu).Where(x => !x.PobieraneErp && !x.TylkoDoOdczytu).ToList();
            if (!string.IsNullOrEmpty(dane.Nadrzedny))
            {
                //todo: kompletnie nie wiem po co to jest - BARTEK
                OpisObiektu opis = OpisObiektu.StworzOpisObiektu(dane.Typ);
                List<OpisPolaObiektuBaza> pola = opis.PolaObiektu.Where(x => x.ParamatryWidocznosciAdmin.Edytowalne).ToList();
                foreach (var p in pola)
                {
                    var atr = p.Property.GetCustomAttribute<IdentyfikatorObiektuNadrzednego>();
                    if (!string.IsNullOrEmpty(dane.Nadrzedny) && atr != null && dane.TypNadrzednych != null && atr.Rodzaj == dane.TypNadrzednych)
                    {
                        OpisPolaObiektuBaza par = new OpisPolaObiektuBaza(p.Property, dane.KluczWartosc);
                        //par.Wartosc = dane.Nadrzedny;
                        //polawart.Add(par);
                        polawart.Add(new OpisPolaObiektu(dane.Nadrzedny, par));
                    }
                }
            }
            Refleksja.UstawWartoscPol(obiekt, polawart.PolaNaslownik() );
        }

        internal void Usun(Type typdanych, string id)
        {
            SolexBllCalosc.PobierzInstancje.DostepDane.UsunWyTypu(typdanych, id);
        }

        internal IList<OpisPolaObiektu> PolaDoEdycjiTresc(object obiekt, object id)
        {
            List<OpisPolaObiektu> pars = new List<OpisPolaObiektu>(OpisObiektu.PobranieParametowObiektu(obiekt, id));
            TrescBll t = (TrescBll)obiekt;
            if (!string.IsNullOrEmpty(t.Symbol))
            {
                var wynik = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<TrescBll>(null, x =>
                    (!string.IsNullOrEmpty(x.TrescPokazywanaJakoLeweMenu) && x.TrescPokazywanaJakoLeweMenu.Equals(t.Symbol, StringComparison.InvariantCultureIgnoreCase))
                    || (!string.IsNullOrEmpty(x.TrescPokazywanaJakoNaglowek) && x.TrescPokazywanaJakoNaglowek.Equals(t.Symbol, StringComparison.InvariantCultureIgnoreCase))
                    || (!string.IsNullOrEmpty(x.TrescPokazywanaJakoReklamaMenu) && x.TrescPokazywanaJakoReklamaMenu.Equals(t.Symbol, StringComparison.InvariantCultureIgnoreCase))
                    || (!string.IsNullOrEmpty(x.TrescPokazywanaJakoStopka) && x.TrescPokazywanaJakoStopka.Equals(t.Symbol, StringComparison.InvariantCultureIgnoreCase))
                    );
                if (wynik.Any())
                {
                    pars.First(x => x.NazwaPola == "Symbol").TylkoDoOdczytu = true;
                }
            }
            return pars;
        }

        internal IList<OpisPolaObiektu> PolaDoEdycjiKolumna(object obiekt, object id)
        {
            List<OpisPolaObiektu> pars = new List<OpisPolaObiektu>(OpisObiektu.PobranieParametowObiektu(obiekt, id));
            object kontrolkja = ((IObiektPrzechowujacyKontrolke)obiekt).StworzKontrolke();
            var obiekty = OpisObiektu.PobranieParametowObiektu(kontrolkja, id);
            foreach (var o in pars)
            {
                if (obiekty.Any(x => x.NazwaPola == o.NazwaPola && x.IdentyfikatorObiektu == o.IdentyfikatorObiektu))
                {
                    continue;
                }
                obiekty.Add(o);
            }

            //pars.AddRange(OpisObiektu.PobranieParametowObiektu(kontrolkja, id));
            return obiekty;
        }

        /// <summary>
        /// Pobiera kolekcję parametrów dla ustawień
        /// </summary>
        /// <param name="obj">Ustawienie które chcemy przekształcić na listę parametrów</param>
        /// <returns></returns>
        public IList<OpisPolaObiektu> PobierzParametry(Ustawienie obj)
        {
            PropertyInfo wartprop = obj.GetType().Properties()["Wartosc"];
            PropertyInfo wartniezalogowani = obj.GetType().Properties()["WartoscDlaNiezalogowanych"];
            List<OpisPolaObiektu> parametry = new List<OpisPolaObiektu>();
            parametry.Add(new OpisPolaObiektu { NazwaPola = "Wartosc", NazwaWyswietlana = "Wartość dla zalogowanych", IdentyfikatorObiektu = obj.Id, Wartosc = obj.Wartosc??obj.WartoscDomyslna, Property = wartprop });
            parametry.Add(new OpisPolaObiektu { NazwaPola = "WartoscDlaNiezalogowanych", NazwaWyswietlana = "Wartość dla niezalogowanych", IdentyfikatorObiektu = obj.Id, Wartosc = obj.WartoscDlaNiezalogowanych??obj.PoprzedniaWartoscDlaNiezalogowanych, Property = wartniezalogowani });
            switch (obj.Typ)
            {
                case TypUstawienia.Bool:
                    parametry.ForEach(x => x.TypPrzechowywanejWartosci = typeof(bool));
                    parametry.ForEach(x => x.Wartosc = x.Wartosc is string && (((string)x.Wartosc).Equals("True",StringComparison.InvariantCultureIgnoreCase) || (string)x.Wartosc == "1"));
                    break;

                case TypUstawienia.Combo:
                    parametry.ForEach(x => x.Slownik = PobieranieParametrowKontrolek.PobierzSlownik(obj.Slownik));
                    parametry.ForEach(x => x.Multiselect = obj.Multiwartosc);
                    parametry.ForEach(x => x.TypPrzechowywanejWartosci = typeof(string));
                    break;

                case TypUstawienia.ComboRefleksja:
                    parametry.ForEach(x => x.Slownik = PobieranieParametrowKontrolek.PobierzSlownik(Type.GetType(obj.Slownik, true)));
                    parametry.ForEach(x => x.Multiselect = obj.Multiwartosc);
                    parametry.ForEach(x => x.TypPrzechowywanejWartosci = typeof(string));
                    break;

                case TypUstawienia.Enum:
                    parametry.ForEach(x => x.Slownik = PobieranieParametrowKontrolek.PobierzSlownikEnum(Type.GetType(obj.Slownik, true)));
                    parametry.ForEach(x => x.Multiselect = obj.Multiwartosc);
                    parametry.ForEach(x => x.TypPrzechowywanejWartosci = ((obj.Multiwartosc) ? typeof(string[]) : typeof(string)));
                    break;

                case TypUstawienia.Refleksja:
                    parametry.ForEach(x => x.Slownik = PobieranieParametrowKontrolek.PobierzSlownikPolaObiektu(Type.GetType(obj.Slownik, true), true));
                    parametry.ForEach(x => x.Multiselect = obj.Multiwartosc);
                    parametry.ForEach(x => x.TypPrzechowywanejWartosci = typeof(string));
                    break;

                case TypUstawienia.String:
                    parametry.ForEach(x => x.TypPrzechowywanejWartosci = typeof(string));
                    break;

                case TypUstawienia.Datetime:
                    parametry.ForEach(x => x.TypPrzechowywanejWartosci = typeof(DateTime));
                    break;

                case TypUstawienia.Decimal:
                    parametry.ForEach(x => x.TypPrzechowywanejWartosci = typeof(decimal));
                    break;

                case TypUstawienia.Password:
                    parametry.ForEach(x => x.TypPrzechowywanejWartosci = typeof(string));
                    break;

                case TypUstawienia.Int:
                    parametry.ForEach(x => x.TypPrzechowywanejWartosci = typeof(int));
                    break;

                case TypUstawienia.HTML:
                    parametry.ForEach(x => x.TypPrzechowywanejWartosci = typeof(string));
                    parametry.ForEach(x => x.WymuszonyTypEdytora = TypEdytora.EdytorTekstowy);
                    break;
            }
            parametry.ForEach(x => x.Wartosc = ((obj.Multiwartosc && x.Wartosc != null) ? ((string)x.Wartosc).Split(';') : x.Wartosc));
            return parametry;
        }

        public Dictionary<string, List<ZmianaObiektu>> PobierzHistorieZmian(Type typ, string id)
        {
            return SolexBllCalosc.PobierzInstancje.DostepDane.PobierzZmianyObiektuWgTypu(typ, id);
        }
    }
}