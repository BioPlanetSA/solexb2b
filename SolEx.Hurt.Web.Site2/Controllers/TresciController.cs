using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ServiceStack.Common;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Web.Site2.Helper;
using SolEx.Hurt.Web.Site2.Models;
using SolEx.Hurt.Web.Site2.Models.DaneDoWidokow;
using SolEx.Hurt.Web.Site2.PageBases;
using System.IO;
using System.Reflection;
using System.Web.Caching;
using SolEx.Hurt.Web.Site2.AtrybutyMvc;
using SolEx.Hurt.Web.Site2.Models.DaneDoWidokow.Tresci;
using SolEx.Hurt.Web.Site2.Models.KontrolkiTresci;
using SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.Tresci;

namespace SolEx.Hurt.Web.Site2.Controllers
{
    [RoutePrefix("Tresci")] //bartek musial dodac bo sie psulo
    public class TresciController : SolexControler
    {
        public TresciController()
        {
            _sciezkaDoPlikowRozszerzen_bezwzgledna = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this._sciezkaDoPlikowRozszerzen_wzgledna);
            Directory.CreateDirectory(this._sciezkaDoPlikowRozszerzen_bezwzgledna);
        }

        /// <summary>
        /// Kategoria produktu z linku /pk{numer}
        /// </summary>
        /// <param name="kategoria">paramtr dodany tylko dla walidacji zeby byl typu LONG a nie np. string</param>
        /// <returns></returns>
        [Route("~/{nazwa}/pk{kategoria:long}")]
        public ActionResult KategoriaProduktu(long kategoria)
        {
            string symbol = SolexBllCalosc.PobierzInstancje.Konfiguracja.AdresStronyZProduktami;
            return StronaSymbol(symbol);
        }

        [Route("Powrot")]
        public PartialViewResult Powrot(string referer)
        {
            if (Request.UrlReferrer == null)
            {
                return null;
            }
            if (Request.Url == null)
            {
                return null;
            }
            if (Request.Url.Host != Request.UrlReferrer.Host)
            {
                return null;
            }
            if (Request.Url == Request.UrlReferrer)
            {
                return null;
            }
             if (string.IsNullOrEmpty(referer))
             {
                 return PartialView("Powrot", Request.UrlReferrer);
             }
             if (Request.Url.ToString() == referer)
             {
                 return null;
             }
            return PartialView("Powrot", referer);
        }

        private List<TreeItem<ElementMenu>> PrzygotujDrzewoMenu(TrescBll parent, bool pokazujReklamy, bool pokazujPodkategorie )
        {
            IList<TrescBll> wszystkieStrony = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<TrescBll>(SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient, null, new List<SortowanieKryteria<TrescBll>> { new SortowanieKryteria<TrescBll>(x => x.Kolejnosc, KolejnoscSortowania.asc, "Kolejnosc") });
            List<ElementMenu> dane = wszystkieStrony.Where(x => x.PokazujWMenu).Select(x => new ElementMenu(x,
               pokazujReklamy ? wszystkieStrony.FirstOrDefault(y => !string.IsNullOrEmpty(y.Symbol) && y.Symbol.Equals(x.TrescPokazywanaJakoReklamaMenu, StringComparison.InvariantCultureIgnoreCase)) : null)).ToList();

            List<TreeItem<ElementMenu>> drzewko = dane.GenerateTree(x => x.Id, x => x.Nadrzedna, parent.Id).ToList();

            if (!pokazujPodkategorie)
            {
                drzewko.ForEach(x => x.Children = new List<TreeItem<ElementMenu>>());
            }

            //jesli PARENT ma zaznaczone ze ma sie renderowac w menu to musimy go pokazac jako pierwsza opcja - ALE NIE jesli jest menu kolumnowe!
            if (parent.PokazujWMenu)
            {
                TreeItem<ElementMenu> noweDrzewo = new TreeItem<ElementMenu>
                {
                    Item = dane.FirstOrDefault(x => x.Id == parent.Id),
                    Children = drzewko
                };

                drzewko = new List<TreeItem<ElementMenu>>();
                drzewko.Add(noweDrzewo);
            }

            return drzewko;
        }

        [Route("menuHamburger/{id:int}/{czyZListyProduktowWywolanie:bool?}/{szukanieGlobalne?}")]
        public PartialViewResult MenuHamburger(int id, bool czyZListyProduktowWywolanie=false, string szukanieGlobalne = null)
        {
            if (Request.IsAjaxRequest())
            {
                MenuHamburger kontrolkaMenu = this.PobierzKontrolke<MenuHamburger>(id);
                return PartialView("_MenuHamburger-ajax",  new Tuple<MenuHamburger, bool,string>(kontrolkaMenu, czyZListyProduktowWywolanie, szukanieGlobalne) );
            }
           
            long idKat;
            bool czyAktualnieProdukty = this.Url.AktualnaStronaToStronaProduktow(out idKat);
            string linkDoMenuAjax = Url.ZbudujLink(string.Format("tresci/MenuHamburger/{0}/{1}", id, czyAktualnieProdukty), SolexHelper.AktualnyJezyk);
            return PartialView("_MenuHamburger", linkDoMenuAjax);
        }

        [ChildActionOnly]
        public PartialViewResult MojeDaneMenu(int id)
        {
            MojeKonto kontrolkaMenu = this.PobierzKontrolke<MojeKonto>(id);
            TrescBll drzewoMenu = null;
            //pobieramy treści z kreśloengo drzewa o ile podane
            if (!string.IsNullOrEmpty(kontrolkaMenu.SymbolKorzen))
            {
                drzewoMenu = Calosc.DostepDane.PobierzPojedynczy<TrescBll>(x => (!string.IsNullOrEmpty(x.Symbol) && x.Symbol.Equals(kontrolkaMenu.SymbolKorzen, StringComparison.OrdinalIgnoreCase)), SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient, false);
            }

            return PartialView("MojeKonto", new Tuple<MojeKonto, TrescBll>(kontrolkaMenu, drzewoMenu));
        }


        /// <summary>
        /// Kontrolka menu
        /// </summary>
        /// <param name="id">id kontrolki menu</param>
        /// <returns></returns>
        [ChildActionOnly]
        [Route("Menu")]
        [OutputCacheSolex(TypDanychDoCache.Menu)]
        public PartialViewResult Menu(int id)
        {
            Menu kontrolkaMenu = this.PobierzKontrolke<Menu>(id);

            if (string.IsNullOrEmpty(kontrolkaMenu.SymbolKorzen))
            {
                throw new Exception("Błąd - nie podano symbolu korzenia menu");
            }

            TrescBll parent = Calosc.DostepDane.PobierzPojedynczy<TrescBll>(x => x.Symbol == kontrolkaMenu.SymbolKorzen, SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient, false);

            if (parent == null)
            {
                return null;
            }

            ////Cachowanie - sprawdzanie czy są ukrywane treści
            bool? czySaUkrywaneTresci= Calosc.TresciDostep.SprawdzCzyKontrolaMenuMaTresciUkrywaneWgKlientow(id, parent);
            
            //wyliczmy klucz
            string kluczCache = Calosc.Cache.WyliczKluczDlaMenu(id, SolexHelper.AktualnyKlient.Id, SolexHelper.AktualnyJezyk.Id, czySaUkrywaneTresci??false);

            List<TreeItem<ElementMenu>> drzewoMenu = this.PrzygotujDrzewoMenu(parent, kontrolkaMenu.PokazujReklamy, kontrolkaMenu.PokazujPodkategorie);

            if (!drzewoMenu.Any())
            {
                return null;
            }

            //jesli PARENT ma zaznaczone ze ma sie renderowac w menu to musimy go pokazac jako pierwsza opcja - ALE NIE jesli jest menu kolumnowe!
            if (string.IsNullOrEmpty(kontrolkaMenu.MenuRodzajMenu))
            {
                kontrolkaMenu.MenuRodzajMenu = "MenuKlasyczneRozwijane";
            }

            string szablon = string.Format("\\RodzajeMenu\\{0}", kontrolkaMenu.MenuRodzajMenu);

            ////jesli jest kontrolka menu hamburgerowego to NIE mozne byc dropdown
            //bool pokazujJakoDropDownMenu = !(kontrolkaMenu is MenuHamburger);
            DaneDoMenu dane = new DaneDoMenu(drzewoMenu, kontrolkaMenu.PokazujPodkategorie, kontrolkaMenu.PokazujReklamy, kontrolkaMenu.MenuSzerokoscKolumny) { idKontrolkiWywolujacej = id };
            PartialViewResult menu = PartialView(szablon, dane);

            Calosc.Cache.DodajObiekt(kluczCache, menu);

            return menu;
        }

        [Route("Sciezka")]
        public PartialViewResult Sciezka(int? idTresci)
        {
            List<ElementMenu> tresci = new List<ElementMenu>();
            long? id = idTresci;
            while (id!=null)
            {
                TrescBll tmp = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<TrescBll>(id, SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient);
                if (tmp != null)
                {
                    if (tmp.NadrzednaId != null)
                    {
                        //jak nie linkujemy elemntu to niech sie nie pokazuje - durny mechanizm bartek komentuje - napisze testy przy gruntownej przerobce tego mechanizmu
                      //  if (!string.IsNullOrEmpty(tmp.LinkAlternatywny))
                       // {
                            tresci.Insert(0, new ElementMenu(tmp, null));
                      //  }
                    }
                    id = tmp.NadrzednaId;
                }
                else
                {
                    id = null;
                }
            }

            TrescBll pierwsza = SolexBllCalosc.PobierzInstancje.TresciDostep.PobierzStroneGlowna(SolexHelper.AktualnyKlient, SolexHelper.AktualnyJezyk.Id);
            if (tresci.All(x => x.Id != pierwsza.Id))
            {
                tresci.Insert(0, new ElementMenu(pierwsza,null));
            }
            if (tresci.Count == 1)
            {
                return null;
            }
            string aktualnylink = Request.Url.ToString();
            TrescBll tmp2 = new TrescBll(tresci.Last().TrescBll) {LinkAlternatywny = aktualnylink};

            ////jelsi to produkty to inny link alternatywny
            if (SolexBllCalosc.PobierzInstancje.Konfiguracja.AdresStronyZProduktami == tmp2.Symbol)
            {
                tmp2.LinkAlternatywny = SolexBllCalosc.PobierzInstancje.Konfiguracja.AdresStronyZProduktami; //wszystkie produkty
            }


            tresci.RemoveAt(tresci.Count - 1);
            tresci.Add(new ElementMenu(tmp2,null));
            return PartialView("_Sciezka", tresci);
        }
        [Route("Tekst")]
        public ActionResult Tekst(string tresc)
        {
            return PartialView("Tekst", tresc);
        }
        
        public ActionResult StronaGlowna()
        {
            //nie dajemy try catch - jaksie tu wywali to nie ma nic do pokazania w ogole
            TrescBll pierwsza = SolexBllCalosc.PobierzInstancje.TresciDostep.PobierzStroneGlowna(SolexHelper.AktualnyKlient, SolexHelper.AktualnyJezyk.Id);
            return Strona(pierwsza);
        }

        [Route("~/{nazwaProduktu}/p{produktId:long}/{modal:regex(m)}", Order = 1)]  //jesli okno modalne jest
        [Route("~/{nazwaProduktu}/p{produktId:long}", Order = 2)]
        public ActionResult SzczegolyProduktu()
        {
            string symbol = SolexBllCalosc.PobierzInstancje.Konfiguracja.AdresStronyZProduktem;
            return StronaSymbol(symbol);
        }

        [Route("~/{symbol}/{tytul}/b{blogWpisId:long}")]
        public ActionResult WpisyBloga(string symbol, long blogWpisId)
        {
            //czy ten blog w ogole istnieje
            var wpis = Calosc.DostepDane.PobierzPojedynczy<BlogWpisBll>(blogWpisId);
            if (wpis == null)
            {
                //jak nie ma bloga to dajemy 404
                Response.StatusCode = 404;
                return null;
            }

            return StronaSymbol(symbol);
        }

        [Route("~/{symbol}/gpid{gpid}")]
        public ActionResult GrupaKategorie(string symbol)
        {
            return StronaSymbol(symbol);
        }

        public ActionResult StronaSymbol(string symbol)
        {



            TrescBll ob = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<TrescBll>(x => (!string.IsNullOrEmpty(x.Symbol) && x.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase)), SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient, false);

            if (ob == null)
            {
                //sprawdzamy poziomy dostepu - na wszelki wypadek
                ob = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<TrescBll>(x => (!string.IsNullOrEmpty(x.LinkAlternatywny) && x.LinkAlternatywny.Equals(symbol, StringComparison.InvariantCultureIgnoreCase))
                      || (!string.IsNullOrEmpty(x.Symbol) && x.Symbol.Equals(symbol, StringComparison.InvariantCultureIgnoreCase)), SolexHelper.AktualnyJezyk.Id, null, false);

                if (ob != null && ob.Aktywny)
                {
                    //czy klient jest niezalogowany - jak tak to trzeba upewnic sie czy moze po zalogownau by mial dostep do tego
                    if (SolexHelper.AktualnyKlient.Dostep == AccesLevel.Niezalogowani)
                    {
                        //teraz beznadizjene sprawdzenie - powinnismy sprawdzac duzo lepiej, ale narazie nie ma weny jak to przerobic - w najgroszym razie koles sie zaloguje a i tak tego nie zobaczy
                        if (ob.Dostep == AccesLevel.Zalogowani)
                        {
                            if (SolexHelper.AktualnyKlient.Dostep == AccesLevel.Niezalogowani)
                            {
                                //klient musi byc zalogowany zeby to ogladac
                                Response.Redirect(Url.LinkLogowania(Request.Url.PathAndQuery), true);
                                return null;
                            }
                        }
                    }

                    if (SolexHelper.AktualnyKlient.Dostep == AccesLevel.Zalogowani)
                    {
                        //jak jest zalogowany a treść jest niezalogowana to przensoimy na strone glowna
                        if (ob.Dostep == AccesLevel.Niezalogowani)
                        {
                            return this.StronaGlowna();
                        }
                    }
                }
            }
            
            if (ob == null)
            {
                throw new HttpException(404, "Nie znaleziono strony o symbolu  " + symbol);
            }
           
            return Strona(ob);
        }

        [NonAction]
        public ActionResult Strona(TrescBll trescDoPokazania)
        {
            if (SolexHelper.AktualnyKlient.Id!=0 &&  SolexHelper.AktualnyPrzedstawiciel != null &&  SolexHelper.AktualnyKlient.Komunikaty != null && SolexHelper.AktualnyKlient.Komunikaty.Any())
            {
                return PartialView("_Komunikaty", SolexHelper.AktualnyKlient.Komunikaty.First());
            }
            if (!RouteData.Values.ContainsKey("idTresci"))
            {
                RouteData.Values.Add("idTresci", trescDoPokazania.Id);
            }
            else
            {
                RouteData.Values["idTresci"] = trescDoPokazania.Id;
            }
            
            if (trescDoPokazania == null)
            {
                throw new HttpException(404, "Pusta strona do pokazania. Błąd 404.");
            }
           
            TrescBll naglowek = null, stopka = null, lewe = null;

            //jesli jest AJAX to bez LAYOUTu - sama zawartosc np. modale
            if (Request.IsAjaxRequest())
            {
                string zawartosc = this.PartialViewToString("Strona", new TrescDoPokazania { Tresc = trescDoPokazania, Naglowek = naglowek, Stopka = stopka, LewaKolumna = lewe, Klient = SolexHelper.AktualnyKlient });
                return PartialView("StronaModal", zawartosc);
            }

            //ladowanie stadardowe z naglowkem, stopka itp

            if (!string.IsNullOrEmpty(trescDoPokazania.TrescPokazywanaJakoNaglowek))
            {
                naglowek = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<TrescBll>(x=>x.Symbol== trescDoPokazania.TrescPokazywanaJakoNaglowek, SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient,false);
            }
            if (!string.IsNullOrEmpty(trescDoPokazania.TrescPokazywanaJakoStopka))
            {
                stopka = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<TrescBll>(x => x.Symbol == trescDoPokazania.TrescPokazywanaJakoStopka, SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient, false);
            }
            if (!string.IsNullOrEmpty(trescDoPokazania.TrescPokazywanaJakoLeweMenu))
            {
                lewe = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<TrescBll>(x => x.Symbol == trescDoPokazania.TrescPokazywanaJakoLeweMenu, SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient, false);
            }
                       
            return View("Strona", new TrescDoPokazania {Tresc = trescDoPokazania, Naglowek = naglowek, Stopka = stopka, LewaKolumna = lewe, Klient = SolexHelper.AktualnyKlient});
        }

        [Route("~/PobierzTlumaczenia")]
        [HttpPost]
        public JsonResult PobierzTlumaczenia(string frazy)
        {
            if(frazy == null)
            {
                return null;
            }

            //speed up - cachujemy tlumaczenia - to jest mega czest owywolywane - lepiej cachowac niz pobierac caly czas
            string klucz = null;
            object result = null;
            if (frazy.Length < 500)
            {

                klucz = $"{SolexHelper.AktualnyJezyk.Id}_{frazy}";
                result = this.Calosc.Cache.PobierzObiekt(klucz);

                if (result != null)
                {
                    return result as JsonResult;
                }
            }

            Dictionary<string, string> wynik = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(frazy))
            {
                string[] pola = frazy.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in pola)
                {
                    if (!wynik.ContainsKey(s))
                    {
                        wynik.Add(s, SolexBllCalosc.PobierzInstancje.Konfiguracja.PobierzTlumaczenie(SolexHelper.AktualnyJezyk.Id, s));
                    }
                }
            }
            
            result = Json(wynik);

            if(klucz != null)
            {
                this.Calosc.Cache.DodajObiekt_Kluczowy(klucz, result);
            }

            return (JsonResult)result;
        }

        [NonAction]
        public Tuple< List<ListaPlikowModel>,string> PobierzListePlikowDoKontroliPlikow(string sciezkaBazowaWZasobach, string sciezkaUzytkownika, string urlDlaKatalogow, bool wyswietlajKatalogi = true)
        {
            string rodzicKatalogLink = null;
            string sciezkaDoListowaniaPlikowWzlgedna = Path.Combine("zasoby", sciezkaBazowaWZasobach).TrimStart('/');

            if (!string.IsNullOrEmpty(sciezkaUzytkownika))
            {
                sciezkaDoListowaniaPlikowWzlgedna = Path.Combine(sciezkaDoListowaniaPlikowWzlgedna, sciezkaUzytkownika);
            }

            string sciezkaBezwzgledna = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, sciezkaDoListowaniaPlikowWzlgedna);
            var katalogInfo = new DirectoryInfo(sciezkaBezwzgledna);

            //czy katalog istniej podany
            if (!Directory.Exists(sciezkaBezwzgledna))
            {
                throw new Exception($"Nie istnieje wymagany katalog do pobierania plików: '{sciezkaBezwzgledna}'. Czy katalog nie został usunięty? Popraw ustawienia kontrolki plików");
            }

            var urlHlepr = new UrlHelper();
            rodzicKatalogLink = null;
            if (sciezkaUzytkownika != null) 
            {
                if (sciezkaUzytkownika.Contains('/'))
                {
                    throw  new Exception("Niepoprawny znak / w sciezce. Powinien byc \\");
                }

                if (sciezkaUzytkownika.Contains('\\'))
                {//sa dwa poziomy rodzica
                    rodzicKatalogLink = urlDlaKatalogow + "?path=" + urlHlepr.Encode(sciezkaUzytkownika.Substring(0, sciezkaUzytkownika.LastIndexOf('\\')));
                }
                else
                {
                    //musi byc rodzic ale do poziomu jedno wyze
                    rodzicKatalogLink = urlDlaKatalogow;
                }
            }

            List<ListaPlikowModel> listaPlikow = new List<ListaPlikowModel>();

            //dodanie folderów
            if (wyswietlajKatalogi)
            {
              
                DirectoryInfo[] listaFolderow = katalogInfo.GetDirectories();
                foreach (DirectoryInfo item in listaFolderow)
                {
                    string sciezkaDoLinku = Path.Combine(sciezkaUzytkownika == null ? "" : sciezkaUzytkownika, item.Name);
                    var plik = new ListaPlikowModel
                    {
                        Typ = TypZasobu.Katalog,
                        Link = urlDlaKatalogow + "?path=" + urlHlepr.Encode(sciezkaDoLinku) ,
                        NazwaBezRoszerzenia = item.Name,
                        LinkDoIkony = this.Pliki_PobierzIkoneDlaRozszerzenia()
                    };
                    listaPlikow.Add(plik);
                }
            }

            FileInfo[] listaplikow = katalogInfo.GetFiles();

            foreach (FileInfo item in listaplikow)
            {
                var plik = new ListaPlikowModel
                {
                    Typ = TypZasobu.Plik,
                    Link = Path.Combine(sciezkaDoListowaniaPlikowWzlgedna, item.Name),
                    Roszerzenie = item.Extension,
                    NazwaBezRoszerzenia = Path.GetFileNameWithoutExtension(item.Name),
                    LinkDoIkony = this.Pliki_PobierzIkoneDlaRozszerzenia(item.Extension),
                    RozmiarMB = Math.Round((decimal)item.Length / 1024 / 1024, 2),
                    Data = item.LastWriteTime
                };

                if (plik.RozmiarMB == 0)
                {
                    plik.RozmiarMB = 0.1m;
                }

                listaPlikow.Add(plik);
            }
            return new Tuple<List<ListaPlikowModel>, string>(listaPlikow, rodzicKatalogLink);
        }

        public object lok = new object();

        [Route("Pliki")]
        public PartialViewResult Pliki(int id)
        {
            if (Request.Url == null)
            {
                throw new Exception("Url nie może być null");
            }

            PlikiDoPobrania kontrolka = this.PobierzKontrolke<PlikiDoPobrania>(id);

            string sciezkaWzglednaUzytkownika = Request["path"];

            if (sciezkaWzglednaUzytkownika != null)
            {
                sciezkaWzglednaUzytkownika = Server.UrlDecode(sciezkaWzglednaUzytkownika);
            }

         
            string urlDlaKatalogow = Request.Url.LocalPath;
            Tuple<List<ListaPlikowModel>, string> dane = null;

            try
            {
                dane = Calosc.Cache.SlownikPrywatny_PobierzObiekt<Tuple<List<ListaPlikowModel>, string>>((long x) => { return (object)this.PobierzListePlikowDoKontroliPlikow(kontrolka.KatalogPlikowWZasoby, sciezkaWzglednaUzytkownika, urlDlaKatalogow, !kontrolka.NieWyswietlajPodkatalogow); }, 1, $"{kontrolka.Id}+{sciezkaWzglednaUzytkownika}".WygenerujIDObiektuSHAWersjaLong(), "lista-plikow");
            }catch(Exception)
            {
                //jak nie zwroci danych - bo nie ma plikow / katalogiw itp. to nic nie pokazujemy
                return null;
            }

            List<ListaPlikowModel>  modelLista = dane.Item1;
            string rodzic = dane.Item2;

            if (modelLista.IsEmpty())
            {
                return null;
            }

            if (kontrolka.Sortowanie == SortowaniePlikow.NazwaAsc)
            {
                modelLista = modelLista.OrderBy(x => x.Typ).ThenBy(x => x.NazwaBezRoszerzenia).ToList();
            }
            else if (kontrolka.Sortowanie == SortowaniePlikow.NazwaDesc)
            {
                modelLista = modelLista.OrderBy(x => x.Typ).ThenByDescending(x => x.NazwaBezRoszerzenia).ToList();
            }
            else if (kontrolka.Sortowanie == SortowaniePlikow.DataAsc)
            {
                modelLista = modelLista.OrderBy(x => x.Typ).ThenBy(x => x.Data).ToList();
            }
            else
            {
                modelLista = modelLista.OrderByDescending(x => x.Data).ToList();
            }

            return PartialView("_ListaPlikow", new DaneDoWidokuPlikow(modelLista, kontrolka.PokazujDate, rodzic, kontrolka.pokazujNaglowek));
        }

        private Dictionary<string,string> _ikonyPlikowWgRozszerzen = new Dictionary<string,string>();


        //zakladanie katalogu dla rozszrzen plikow jak go nie ma
        private string _sciezkaDoPlikowRozszerzen_wzgledna = @"Zasoby\Obrazki\ikony_plikow";
        private string _sciezkaDoPlikowRozszerzen_bezwzgledna = null;
           
        public string Pliki_PobierzIkoneDlaRozszerzenia(string ext = "katalog")
        {
            string linkDoIkony;
            if (!_ikonyPlikowWgRozszerzen.TryGetValue(ext, out linkDoIkony))
            {
                string filename = ext.Replace(".", "") + ".png";

                string sciezkaPliku = Path.Combine( this._sciezkaDoPlikowRozszerzen_bezwzgledna, filename);

                if (!System.IO.File.Exists(sciezkaPliku))
                {
                    sciezkaPliku = Path.Combine(this._sciezkaDoPlikowRozszerzen_wzgledna, "plik.png");
                }
                else
                {
                    sciezkaPliku = Path.Combine(this._sciezkaDoPlikowRozszerzen_wzgledna, filename);
                }
               
                _ikonyPlikowWgRozszerzen.Add(ext, sciezkaPliku);
                return sciezkaPliku;
            }

            return linkDoIkony;
        }


        public PartialViewResult PoleZUstawienia(string wartoscDoPokazania, string przedrostek, string przyrostek)
        {
            var model = new KontrolkaPokazywaniaPrzedrostekModel { Przedrostek = przedrostek, Przyrostek = przyrostek, KlasyCSS = wartoscDoPokazania};
            var obj = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Ustawienie>(x => x.Symbol == wartoscDoPokazania, null);
            if (obj != null)
            {
                if (SolexHelper.AktualnyKlient.Id == 0)
                {
                    string wartosc = string.Empty;
                    if (obj.WartoscDlaNiezalogowanych == null)
                    {
                        if (obj.Wartosc == null)
                        {
                            wartosc = obj.WartoscDomyslna;
                        }
                        else
                        {
                            wartosc = obj.Wartosc;
                        }
                    }
                    else
                    {
                        wartosc = obj.WartoscDlaNiezalogowanych;
                    }
                    model.WartoscDoPokazania = new List<string>() { wartosc };
                }
                else
                {
                    model.WartoscDoPokazania = new List<string>() { string.IsNullOrEmpty(obj.Wartosc) ? obj.WartoscDomyslna :obj.Wartosc };
                }
            }

            return PartialView("_PoleZUstawienia", model);
        }
    }
}