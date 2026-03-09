using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Core.DostepDane.DaneObiektu;
using SolEx.Hurt.Core.ExtensionRozszerzeniaKlas;
using SolEx.Hurt.Core.Importy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Web;
using SolEx.Hurt.Web.Site2.Helper;
using SolEx.Hurt.Web.Site2.Helper.DaneListaAdmin;
using SolEx.Hurt.Web.Site2.Models;
using SolEx.Hurt.Web.Site2.Models.Admin;
using SolEx.Hurt.Web.Site2.Models.DaneDoWidokow;
using SolEx.Hurt.Web.Site2.Models.DaneDoWidokow.Koszyk;
using SolEx.Hurt.Web.Site2.Modules;
using SolEx.Hurt.Web.Site2.PageBases;
using SolEx.Hurt.Web.Site2.Providers;
using WebGrease.Css.Extensions;
using Klient = SolEx.Hurt.Core.Klient;

namespace SolEx.Hurt.Web.Site2.Controllers
{
    [Autoryzacja(new[]{RoleType.Pracownik, RoleType.Administrator } )]
    [RoutePrefix("Admin")]
    public class AdminController : SolexControler
    {
        [Route("Tlumacz")]
        [Route("Tlumacz/{fraza}")]
        public PartialViewResult Tlumacz(string fraza)
        {
            TlumaczeniePole wzorocowe = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<TlumaczeniePole>(fraza); //SystemPolaDal.Get().FirstOrDefault(x => x.Id == fraza);
            if (wzorocowe == null)
            {
                return null;
            }
            var tlumaczenia = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Tlumaczenie>(null, x => x.Pole == fraza);//LokalizacjeBLL.PobierzInstancje.PobierzTlumaczeniaDlaTypu(typeof(TlumaczeniePole)).Values.SelectMany(x => x).Where(x => x.pole == fraza).ToArray();
            List<Tlumaczenie> slowniki = new List<Tlumaczenie>();
            foreach (var j in SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykiWSystemie)
            {
                if (j.Value.Domyslny)
                {
                    continue;
                }
                var s = tlumaczenia.FirstOrDefault(x => x.JezykId == j.Key);
                if (s == null)
                {
                    s = new Tlumaczenie { JezykId = j.Key, Pole = fraza, Typ = typeof(TlumaczeniePole).PobierzOpisTypu() };
                }
                slowniki.Add(s);
            }
            return PartialView("Tlumacz", new Tlumaczenia() { TlumacznieJezykPodstawowy = wzorocowe, Lokalizacje = slowniki.ToArray() });
        }

        [HttpPost]
        public void Tlumacz(Tlumaczenia tlumacznia)
        {
            IList<TlumaczeniePole> items = new List<TlumaczeniePole> { tlumacznia.TlumacznieJezykPodstawowy };
            // SystemPolaDal.Update(items);
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe(items);
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe(tlumacznia.Lokalizacje.ToList());
            SolexBllCalosc.PobierzInstancje.Konfiguracja.ResetSystemNames();
        }


        public PartialViewResult WidocznosDlaKlientowListaKlientow(string id)
        {
            WidocznosciTypow widocznosc =  SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<WidocznosciTypow>(id);
            return PartialView("WidocznoscDlaKlientowListaKlientow", SolexBllCalosc.PobierzInstancje.WidocznosciTypowBll.PobierzKlientowSprelniajacychWarunkiSzablonu(widocznosc));
        }

        [Route("Index")]
        public ActionResult Index()
        {
            if (SolexHelper.AktualnyKlient.CzyAdministrator || SolexHelper.AktualnyKlient.Role.Contains(RoleType.Pracownik))
            {
                return View("Index");
            }
            return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Brak dostepu");
        }

        [Route("Menu")]
        public PartialViewResult Menu()
        {
            IList<Helper.DaneListaAdmin.ElementMenu> menu = AdminHelper.PobierzInstancje.PobierzMenu(SolexHelper.AktualnyKlient).ToArray();
            return PartialView("Menu", menu);
        }
        //[Route("ZarzadzaniePlikami")]
        //public ActionResult ZarzadzaniePlikami()
        //{
        //    return View("ZarzadzaniePlikami");
        //}
       
        [Autoryzacja(RoleType.Administrator)]
        [Route("Ustawienia")]
        public ActionResult Ustawienia()
        {
            return View("Ustawienia");
        }

        [Route("Tresci")]
        [Route("Tresci/{id}")]
        public ActionResult Tresci(int? id = null)
        {
            int jezyk = SolexHelper.AktualnyJezyk.Id;
            return View("Tresci", new Tuple<int?, int>(id, jezyk));
        }

        [Route("SzablonyMaili")]
        public ActionResult SzablonyMaili()
        {
            return View("SzablonyMaili");
        }

        [Route("StatusKonfiguracji")]
        public ActionResult StatusKonfiguracji()
        {
            return View("StatusKonfiguracji");
        }

        [Route("BlokadaSystemu")]
        public ActionResult BlokadaSystemu()
        {
            BlokadaSystemuViewModel model = new BlokadaSystemuViewModel();
            var pobrany = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Tresc>(x => x.Symbol == "blokada-systemu", SolexHelper.AktualnyKlient);
            if (pobrany != null)
            {
                if (!string.IsNullOrEmpty(pobrany.MetaSlowaKluczowe))
                {
                    var godzina = pobrany.MetaSlowaKluczowe.Split(';');
                    model.GodzinaStart = godzina[0];
                    model.GodzinaKoniec = godzina[1];
                    model.Powod = pobrany.MetaOpis;
                    model.Zablokowany = pobrany.Aktywny;
                }
            }
            return View("BlokadaSystemu", model);
        }

        [HttpPost]
        [Route("BlokadaSystemu")]
        public ActionResult BlokadaSystemu(BlokadaSystemuViewModel model)
        {
            var aktualizowany = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Tresc>(x => x.Symbol == "blokada-systemu", SolexHelper.AktualnyKlient);
            if (aktualizowany != null)
            {
                aktualizowany.MetaSlowaKluczowe = model.GodzinaStart + ";" + model.GodzinaKoniec + ";"; // godziny blokady
                aktualizowany.MetaOpis = model.Powod; //powód blokady
                aktualizowany.Aktywny = model.Zablokowany;

                SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(aktualizowany);
            }
            return View("BlokadaSystemu", model);
        }

        /// <summary>
        /// Akcja wykorzystywana w Testach konfiguracji
        /// </summary>
        /// <returns></returns>
        [Route("WykonajTesty")]
        public ActionResult WykonajTesty()
        {
            List<KlasaOpakowanieTesty> lista1 = SolexBllCalosc.PobierzInstancje.TestyKonfiguracji.WykonajTesty();
            List<KlasaOpakowanieTesty> lista2 = SolexBllCalosc.PobierzInstancje.TestyKonfiguracji.WykonajTestyKoszykowe();
            List<KlasaOpakowanieTesty> lista3 = SolexBllCalosc.PobierzInstancje.TestyKonfiguracji.WykonajTestyBazy();
           lista1.AddRange( SolexBllCalosc.PobierzInstancje.TestyKonfiguracji.WykonajTestySkrzynekPocztowych() ); 

            var testKontrolekWynik = new KlasaOpakowanieTesty
            {
                BladTestu = true,
                CzyMaHistorieZmian = false,
                NazwaTestu = "Parametry kontrolek na stronach (najczęściej wystarczy zapisać jeszcze raz kontrolke - jeśli nie pomoga kontaktuj się z IT",
                ListaBledow = new List<string>()
            };

            //testy konfiguracji kontrolek - musza byc w webie tutaj bo tutaj sa akcje - nie moga byc w bllu
            var wszystkieKontrolki = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<TrescKolumnaBll>(null);
            foreach (var kontrolka in wszystkieKontrolki)
            {
                try
                {
                    this.TestujZgodnoscParametrowKontrolkiIKontroleraAkcji(kontrolka.PobierzParametry(), kontrolka.Kontrolka().Kontroler, kontrolka.Kontrolka().Akcja);
                }
                catch (Exception e)
                {
                    testKontrolekWynik.ListaBledow.Add(e.Message + $"[kontrolka id: {kontrolka.Id}, nazwa: {kontrolka.Kontrolka().Nazwa}, komentarz: {kontrolka.OpisKontenera}]");
                }
            }
            if (testKontrolekWynik.ListaBledow.Any())
            {
                lista1.Add(testKontrolekWynik);
            }

            return PartialView("_TestOgolny", new Tuple<List<KlasaOpakowanieTesty>, List<KlasaOpakowanieTesty>, List<KlasaOpakowanieTesty>>(lista1, lista2, lista3));
        }

        /// <summary>
        /// Akcja wykorzystywana w Testach konfiguracji
        /// </summary>
        /// <returns></returns>
        [Route("UsunDaneZTabeli")]
        public ActionResult UsunDaneZTabeli(string nazwa)
        {
            Log.ErrorFormat("Uzytkownik o emailu: {0} o ip: {1} usunął dane z tabeli: {2}", SolexHelper.AktualnyKlient.Email, SesjaHelper.PobierzInstancje.IpKlienta, nazwa);
            //MainDAO.UsunDaneZTabeli(nazwa);
            SolexBllCalosc.PobierzInstancje.DostepDane.WyczyscTabele(nazwa);
            return RedirectToAction("StatusKonfiguracji");
        }

        /// <summary>
        /// Akcja wykorzystywana w Edytuj pliki systemowe
        /// </summary>
        /// <returns></returns>
        [Route("PlikiDoEdycji")]
        public ActionResult PlikiDoEdycji()
        {
            return View("_PlikiDoEdycji", PlikiDostep.PobierzInstancje.PobierzPlikiDoEdycjiAdmin());
        }

        /// <summary>
        /// Akcja wykorzystywana w Edytuj pliki systemowe
        /// </summary>
        /// <returns></returns>
        [Route("EdytujPlik")]
        public PartialViewResult EdytujPlik(string hash)
        {
            var pliki = PlikiDostep.PobierzInstancje.PobierzPlikiDoEdycjiAdmin();
            if (!pliki.ContainsKey(hash))
            {
                return null;
            }
            return PartialView("_EdytujPlik", pliki[hash]);
        }

        /// <summary>
        /// Akcja wykorzystywana w Edytuj pliki systemowe
        /// </summary>
        /// <returns></returns>
        [ValidateInput(false)]
        [Route("ZapiszPlik")]
        public void ZapiszPlik(PlikDoEdycji zmieniany, MapowanePola mapowane)
        {
            PlikiDostep.PobierzInstancje.ZapiszPlik(zmieniany);
        }

        /// <summary>
        /// Akcja wykorzystywana w Grupach produktów do pokazywania drzewka
        /// </summary>
        /// <returns></returns>
        [Route("ModelujDrzewko")]
        public ActionResult ModelujDrzewko(int? grupaid = null, int? jezyk = null)
        {
            if (jezyk == null)
            {
                jezyk = SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDDomyslny;
            }
            string nazwaGrupy =
                SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<GrupaBLL>(x => x.Id == grupaid, null).Nazwa;
            return View("DrzewkoGrupy", new Tuple<int?, int, string>(grupaid, jezyk.Value, nazwaGrupy));
        }

        [Route("Zamowienie/{id}")]
        public ActionResult Zamowienie(int id)
        {
            List<ZamowieniaProduktyBLL> listaProduktowZamowienia = new List<ZamowieniaProduktyBLL>();
            ZamowieniaBLL order = new ZamowieniaBLL();
            //if (id.HasValue)
            //{
            order = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ZamowieniaBLL>(id, SolexHelper.AktualnyKlient);
            listaProduktowZamowienia = order.PobierzPozycjeDokumentu().Select(x => x as ZamowieniaProduktyBLL).ToList();
            //}

            return View("ZamowieniePodglad", new Tuple<List<ZamowieniaProduktyBLL>, ZamowieniaBLL>(listaProduktowZamowienia, order));
        }

        [Route("DokumentZERP")]
        public ActionResult DokumentZERP(int? id)
        {
            List<DokumentyPozycje> listaPozycji = new List<DokumentyPozycje>();
            DokumentyBll order = new DokumentyBll();
            if (id.HasValue)
            {
                order = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<DokumentyBll>(id, SolexHelper.AktualnyKlient);
                listaPozycji = order.PobierzPozycjeDokumentu().Select(x => x as DokumentyPozycje).ToList();
            }
            return View("DokumentPodglad", new Tuple<List<DokumentyPozycje>, DokumentyBll>(listaPozycji, order));
        }

        [HttpPost]
        [Route("Zamowienie")]
        public ActionResult Zamowienie(string idDokumentu, string zamowienieDropDownList, string akcja)
        {
            if(akcja=="Anuluj")
                return Redirect("/Admin/Lista?typ=SolEx.Hurt.Core.ZamowieniaBLL,SolEx.Hurt.Core");
            if (!string.IsNullOrEmpty(idDokumentu) && !string.IsNullOrEmpty(zamowienieDropDownList) && akcja == "Zapisz")
            {
                var dokument =
                SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ZamowieniaBLL>(
                    x => x.Id == Convert.ToInt32(idDokumentu), SolexHelper.AktualnyKlient);

                int status = Convert.ToInt32(zamowienieDropDownList);

                dokument.StatusId = (StatusImportuZamowieniaDoErp)status;
                SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(dokument);
                return Redirect("/Admin/Lista?typ=SolEx.Hurt.Core.ZamowieniaBLL,SolEx.Hurt.Core");
            }
            throw new HttpException("Błędne parametry");
        }

        [Route("DrzewkoGrupyDane")]
        public JsonResult DrzewkoGrupyDane(int jezyk, int? id = null)
        {
            var kategorie = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<KategorieBLL>(jezyk, null,x=>x.GrupaId == id);
            var dane = kategorie.OrderBy(x => x.Kolejnosc).Select(x => new DrzewkoGrupyModel(x)).ToList();
            dane.ForEach(x => x.state.selected = (x.id == id.ToString()));
            return PrzeksztalcNaJson(dane);
        }

        [Route("DrzewkoGrupyAktualizuj")]
        public JsonResult DrzewkoGrupyAktualizuj(DrzewkoGrupyModel[] zmieniane)
        {
            bool nowe = false;
            List<long> ids = new List<long>();
            zmieniane.ForEach(x => { int tmp; int.TryParse(x.id, out tmp); ids.Add(tmp); });
            var tresci = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<KategorieBLL>(SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDDomyslny, null, x => ids.Contains(x.Id));
            List<KategorieBLL> aktualizacja = new List<KategorieBLL>();
            foreach (DrzewkoGrupyModel tm in zmieniane)
            {
                int id;
                int.TryParse(tm.id, out id);
                KategorieBLL t = tresci.FirstOrDefault(x => x.Id == id);
                if (t == null)
                {
                    t = new KategorieBLL {JezykId = SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDDomyslny};
                    nowe = true;
                }
                t.Nazwa = tm.text;
                if (tm.parent != "#")
                {
                    t.ParentId = int.Parse(tm.parent);
                }
                t.Kolejnosc = tm.kolejnosc;
                aktualizacja.Add(t);
            }
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<KategorieBLL>(aktualizacja);
            return Json(nowe);
        }
        [Route("DrzewkoGrupyUsun")]
        public void DrzewkoGrupyUsun(object[] ids)
        {
            SolexBllCalosc.PobierzInstancje.DostepDane.Usun<KategorieBLL, object>(ids.ToList());
        }

        [Route("TresciDane")]
        [Route("TresciDane/{id}")]
        public JsonResult TresciDane(int? id = null)
        {
            IList<TrescBll> tresci = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<TrescBll>(SolexHelper.AktualnyJezyk.Id,null);
            List<TrescModel> dane = tresci.OrderBy(x => x.Kolejnosc).Select(x => new TrescModel(x)).ToList();
            dane.ForEach(x=>x.state.selected=(x.id==id.ToString()));
            return PrzeksztalcNaJson(dane);
        }

        [Route("TrescAktualizuj")]
        public JsonResult TrescAktualizuj(TrescModel[] zmieniane)
        {
            List<long> ids=new List<long>();
            zmieniane.ForEach(x=> { int tmp; int.TryParse(x.id, out tmp); ids.Add(tmp); });
            var tresci = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<TrescBll>(SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDDomyslny, null, x => ids.Contains(x.Id));
            List<TrescBll> aktualizacja=new List<TrescBll>();
            foreach (TrescModel tm in zmieniane)
            {
                int id;
                int.TryParse(tm.id, out id);
                TrescBll t = tresci.FirstOrDefault(x => x.Id == id);
                if (t == null)
                {
                    t=new TrescBll();
                    t.JezykId = SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDDomyslny;
                    t.LinkAlternatywny = TextHelper.PobierzInstancje.OczyscNazwePliku(tm.text);
                }
                t.Nazwa = tm.text;  
                t.NadrzednaId =int.Parse( tm.parent);
                t.Kolejnosc = tm.kolejnosc;
                aktualizacja.Add(t);
            }
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<TrescBll>(aktualizacja);
            return Json(true);
        }

        [Route("DodajWiersz")]
        [Route("DodajWiersz/{id}")]
        public void DodajWiersz(int id)
        {
            TrescBll tw = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<TrescBll>(id, null);
            TrescWierszBll tp = new TrescWierszBll();
            tp.TrescId = id;
            if (tw.Wiersze.Any())
            {
                tp.Kolejnosc = tw.Wiersze.Max(x => x.Kolejnosc) + 1;
            }

            //stadnarodwe klasy css
            tp.DodatkoweKlasyCss = new[] {"wiersz-wysrodkuj-w-pionie", "wiersz-dodaj-odstepy-dla-elementow-wewnatrz"};
            
            tp.JezykId = SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDDomyslny;
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(tp);
        }

        private List<MethodInfo> wszystkieMetodyWszystkichKontrolerow = null;

        [NonAction]
        public void TestujZgodnoscParametrowKontrolkiIKontroleraAkcji(Dictionary<string, object> parametryKontrolki, string kontroler, string akcja)
        {
            if (string.IsNullOrEmpty(kontroler) || string.IsNullOrEmpty(akcja))
            {
                throw new Exception("Brak podanego kontrolera, akcji");
            }

            if (wszystkieMetodyWszystkichKontrolerow == null)
            {
                Assembly asm = Assembly.GetExecutingAssembly();
                wszystkieMetodyWszystkichKontrolerow = asm.GetTypes().Where(x => x.Name.EndsWith("Controller", StringComparison.InvariantCultureIgnoreCase)).SelectMany(type => type.GetMethods()).Where(method => method.IsPublic && !method.IsDefined(typeof(NonActionAttribute))).ToList();
            }

            MethodInfo metoda = wszystkieMetodyWszystkichKontrolerow.FirstOrDefault(method => method.DeclaringType.Name.StartsWith(kontroler, StringComparison.InvariantCultureIgnoreCase) &&  method.Name.Equals(akcja, StringComparison.InvariantCultureIgnoreCase) );

            if (metoda == null)
            {
                throw new Exception( string.Format("Brak akcji: {0}/{1} wymaganej dla kontrolki.", kontroler, akcja ));
            }

            var paramsAkcji = metoda.GetParameters();

            foreach (var p in paramsAkcji)
            {
                KeyValuePair<string, object> parametrKontrolki = parametryKontrolki.FirstOrDefault(x => x.Key.Equals(p.Name, StringComparison.InvariantCultureIgnoreCase));
                //if (parametrKontrolki.Equals(new KeyValuePair<string, object>()))
                //{
                //    continue;
                //}

                try
                {
                    if (parametrKontrolki.Value == null)
                    {
                        //bartek - nie umiem tego zrobić dobrze daltego daje continue
                        //if (p.IsNullable() || p.HasDefaultValue)
                        //{
                            continue;
                       // }
                     
                        //    throw new Exception("Brak wymaganego parametru: " + p.Name);
                    }


                    if (p.ParameterType == typeof(long) || p.ParameterType == typeof(int))
                    {
                        long.Parse(parametrKontrolki.Value.ToString());
                    }


                    if (p.ParameterType == typeof(decimal))
                    {
                        decimal.Parse(parametrKontrolki.Value.ToString());
                    }

                    if (p.ParameterType == typeof(bool))
                    {
                        bool.Parse(parametrKontrolki.Value.ToString());
                    }
                }
                catch
                {
                    throw new Exception( string.Format("Parametr: {0} dla akcji: {1}/{2} ma zły typ. Wymagany Typ: {3}, ale w kontrolce wartość: {4}", 
                        p.Name, kontroler, akcja, p.ParameterType.Name, parametrKontrolki.Value));
                }
            }
        }


        [Route("DodajKolumne")]
        public void DodajKolumne(int id, string typ)
        {
            DodajKolumne(id, typ, null);
        }
        [NonAction]
        private void DodajKolumne(int id, string typ, TrescKolumnaBll kolumnaBazowa = null)
        {
            TrescKolumnaBll tp;
            if (kolumnaBazowa != null)
            {
                tp = new TrescKolumnaBll(kolumnaBazowa);
            }
            else
            {
                tp = new TrescKolumnaBll
                {
                    TrescWierszId = id,
                    Szerokosc = 12,
                    RodzajKontrolki = typ
                };

                //zaladowanie kontrolki i z niej wartosci domyslnych dla wiersza
                Type typDoZbudowania = Type.GetType(typ, true);
                KontrolkaTresciBaza obiekt = Activator.CreateInstance(typDoZbudowania) as KontrolkaTresciBaza;

                if (obiekt == null)
                {
                    throw new Exception("Kontrolka nie dziedziczy po klasie bazowej KontrolkaTresciBaza - błąd kontrolki. Zgłoś problem do administratora");
                }

                if (obiekt.DomyslneWartosciDlaNowejKontrolki != null)
                {
                    if (obiekt.DomyslneWartosciDlaNowejKontrolki.Kolejnosc != 0)
                    {
                        tp.Szerokosc = obiekt.DomyslneWartosciDlaNowejKontrolki.Szerokosc;
                    }

                    if (obiekt.DomyslneWartosciDlaNowejKontrolki.DodatkoweKlasyCssKolumny != null && obiekt.DomyslneWartosciDlaNowejKontrolki.DodatkoweKlasyCssKolumny.Length > 0)
                    {
                        tp.DodatkoweKlasyCssKolumny = obiekt.DomyslneWartosciDlaNowejKontrolki.DodatkoweKlasyCssKolumny;
                    }

                    if (!string.IsNullOrEmpty(obiekt.DomyslneWartosciDlaNowejKontrolki.DodatkoweKlasyCssReczneKolumny))
                    {
                        tp.DodatkoweKlasyCssReczneKolumny = obiekt.DomyslneWartosciDlaNowejKontrolki.DodatkoweKlasyCssReczneKolumny;
                    }

                    tp.Dostep = obiekt.DomyslneWartosciDlaNowejKontrolki.Dostep;
                    if (obiekt.DomyslneWartosciDlaNowejKontrolki.Kolejnosc != 0)
                    {
                        tp.Kolejnosc = obiekt.DomyslneWartosciDlaNowejKontrolki.Kolejnosc;
                    }
                    if (!string.IsNullOrEmpty(obiekt.DomyslneWartosciDlaNowejKontrolki.Marginesy))
                    {
                        tp.Marginesy = obiekt.DomyslneWartosciDlaNowejKontrolki.Marginesy;
                    }
                    if (!string.IsNullOrEmpty(obiekt.DomyslneWartosciDlaNowejKontrolki.Paddingi))
                    {
                        tp.Paddingi = obiekt.DomyslneWartosciDlaNowejKontrolki.Paddingi;
                    }

                    if (!string.IsNullOrEmpty(obiekt.DomyslneWartosciDlaNowejKontrolki.OpisKontenera))
                    {
                        tp.OpisKontenera = obiekt.DomyslneWartosciDlaNowejKontrolki.OpisKontenera;
                    }
                }

                tp.JezykId = SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDDomyslny;
            }

            TrescWierszBll tw = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<TrescWierszBll>(tp.TrescWierszId, null);
                if (tw.Kolumny.Any())
                {
                
                    tp.Kolejnosc = tw.Kolumny.Max(x => x.Kolejnosc) + 1;
                }

            tp.Id = (int) SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(tp);
        }
        [Route("ZapiszKolejnosc")]
        public void ZapiszKolejnosc(TrescWierszBll[] wiersze,TrescKolumnaBll[] kolumny)
        {
            if (wiersze != null)
            {
                List<TrescWierszBll> aktualizownewiersze = new List<TrescWierszBll>();
                foreach (var w in wiersze)
                {
                    TrescWierszBll tmp = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<TrescWierszBll>(w.Id);
                    tmp.Kolejnosc = w.Kolejnosc;
                    aktualizownewiersze.Add(tmp);
                }
                SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<TrescWierszBll>(aktualizownewiersze);
            }
            if (kolumny != null)
            {
                List<TrescKolumnaBll> aktualizowneKolumny = new List<TrescKolumnaBll>();
                foreach (var k in kolumny)
                {
                    TrescKolumnaBll tmp = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<TrescKolumnaBll>(k.Id);
                    tmp.Kolejnosc = k.Kolejnosc;
                    tmp.TrescWierszId = k.TrescWierszId;
                    aktualizowneKolumny.Add(tmp);
                }
                SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<TrescKolumnaBll>(aktualizowneKolumny);
            }
        }
        [Route("TrescZawartoscEdycja")]
        [Route("TrescZawartoscEdycja/{id}")]
        public PartialViewResult TrescZawartoscEdycja(int id)
        {
            int jezyk = SolexHelper.AktualnyJezyk.Id;
            DaneEdycjaAdmin danewidokok = AdminHelper.PobierzInstancje.PobierzDaneDoEdycji(id, typeof(TrescBll), null, null, jezyk, SolexHelper.AktualnyKlient, Url);
            TrescBll tresc = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<TrescBll>(id, jezyk, null);
            EdycjaTresciAdmin dane = new EdycjaTresciAdmin(danewidokok, tresc.Wiersze);
            return PartialView("TrescEdycja", dane);
        }
        [Route("WyborKontrolkiTresci")]
        [Route("WyborKontrolkiTresci/{id}")]
        public PartialViewResult WyborKontrolkiTresci(int id)
        {
            return PartialView("TresciWyborKontrolki",new Tuple<int,  IList<KontrolkaTresciBaza>>(id, this.PobierzKontrolki()));
        }

        private IList<KontrolkaTresciBaza> PobierzKontrolki()
        {
            var typy = Refleksja.PobierzListeKlasDziedziczacychPoKlasieBazowej(typeof(KontrolkaTresciBaza));
            List<KontrolkaTresciBaza> wynik = new List<KontrolkaTresciBaza>();
            foreach (var t in typy)
            {
                if (t.IsAbstract)
                {
                    continue;
                }
                KontrolkaTresciBaza kb = (KontrolkaTresciBaza)Activator.CreateInstance(t);
                wynik.Add(kb);
            }

            return wynik;
        }


        [Route("PobierzSzablonTresci/{id}")]
        public FileResult PobierzSzablonTresci(int id,bool pobierzDzieci=true)
        {
            TrescBllImport dane = (TrescBllImport)SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<TrescBll>(id, null);
         List<TrescBllImport> kolekcja = new List<TrescBllImport>();
            if (dane.NadrzednaId == null)
            {
                kolekcja.AddRange(dane.Dzieci);
            }
            else
            {
                kolekcja.Add(dane);
            }
            if (!pobierzDzieci)
            {
                kolekcja.ForEach(x=>x.Dzieci.Clear());
            }
            string danejson = JSonHelper.Serialize(kolekcja);
            return File(Encoding.UTF8.GetBytes(danejson), "text/json","SzablonListyProduktow " + dane.Nazwa + ".json");
        }
        [Route("WczytajSzablonTresci/{id}")]
        public PartialViewResult WczytajSzablonTresci(int id)
        {
            List<string> szablony = SolexBllCalosc.PobierzInstancje.TresciDostep.IstniejaceSzablony();
            return PartialView("WczytajSzablonTresci", new Tuple<int,List<string>>(id,szablony));
        }
        [Route("EksportSzablonTresci/{id}")]
        public PartialViewResult EksportSzablonTresci(int id)
        {
            TrescBll dane = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<TrescBll>(id, null);
            return PartialView("EksportSzablonTresci", dane);
        }
        [Route("ZresetujSzablonTresci")]
        public ActionResult ZresetujSzablonTresci(int id, HttpPostedFileBase szablon = null, string istniejacy = null)
        {
            if (szablon == null && istniejacy == null)
            {
                throw new ArgumentException("Nie podana oni pliku z szablonem, ani nie wybrano szablonu");
            }
            List<TrescBllImport> wzor;
            if (szablon != null)
            {
                StreamReader sb = new StreamReader(szablon.InputStream);
                string zawartosc = sb.ReadToEnd();

                wzor = JSonHelper.Deserialize<List<TrescBllImport>>(zawartosc);
            }
            else
            {
                wzor = SolexBllCalosc.PobierzInstancje.TresciDostep.WczytajSzablonDyskowy(istniejacy);
            }
            SolexBllCalosc.PobierzInstancje.TresciDostep.ResetujSzablon(id, wzor);
            return RedirectToAction("Tresci", "Admin");
        }
        [Route("UstawieniaLista")]
        public ActionResult UstawieniaLista()
        {
            List<Ustawienie> ustawienia = SolexBllCalosc.PobierzInstancje.Konfiguracja.ListaUstawien(true, null);
            return PartialView("UstawieniaLista", ustawienia.OrderBy(x=>x.Grupa).ToList());
        }
        [Route("UstawienieEdycja/{id}")]
        public ActionResult UstawienieEdycja(string id)
        {
            Ustawienie u = SolexBllCalosc.PobierzInstancje.Konfiguracja.Pobierz(id);
            var parametry = AdminHelper.PobierzInstancje.PobierzParametry(u);
            ViewBag.Ustawienie = u;
            return PartialView("UstawienieEdycja",parametry);
        }
        [ValidateInput(false)]
        [Route("ZapiszUstawienie")]
        public void ZapiszUstawienie(string id, OpisPolaObiektu[] parametry, bool wartoscDomyslna, bool wartoscDomyslnaNiezalogowani)
        {
            Ustawienie u=  SolexBllCalosc.PobierzInstancje.Konfiguracja.Pobierz(id);
            //kontrolka Html.CheckBox renderuje dwa inputy o tej samej nazwie (http://stackoverflow.com/questions/2697299/asp-net-mvc-why-is-html-checkbox-generating-an-additional-hidden-input)
            //i dla tego wartości boolowe są przesyłane jako tablica stringów gdzie druga wartość jest zawsze null
            //tutaj pomijamu drugą wartość by poprawnie zapisać ustawienie. 
            foreach (var source in parametry.Where(x=>x.TypPrzechowywanejWartosci==typeof(bool)))
            {
                string[] staraWartosc= (string[]) source.Wartosc;
                source.Wartosc = staraWartosc[0];
            }
            Refleksja.UstawWartoscPol(u,parametry.PolaNaslownik());
            if (wartoscDomyslna)
            {
                u.Wartosc = null;
            }
            if (wartoscDomyslnaNiezalogowani)
            {
                u.WartoscDlaNiezalogowanych = null;
            }
            SolexBllCalosc.PobierzInstancje.Konfiguracja.AktualizacjaUstawien(u);
            SolexBllCalosc.PobierzInstancje.Konfiguracja.RefreshData();
        }
        
        [NonAction]
        private string PobierzSortowanie(Type typ, string sortowanie, ref KolejnoscSortowania sortowanieKolejnosc)
        {

            if (string.IsNullOrEmpty(sortowanie))
            {
                Sortowanie wart = SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzSortowanie(SolexHelper.AktualnyKlient, TypUstawieniaKlienta.ListaKolumnWAdminieSortowanie, typ.FullName);
                if (wart != null)
                {
                    sortowanie = wart.Pola.First().Pole;
                    sortowanieKolejnosc = wart.Pola.First().KolejnoscSortowania;
                }
                //bylo pobieranie pierwszej dowolnej kolumny do sortowania - wywalam to - niech user sam sobie ustawi sortowanie - BARTEK
                //if (string.IsNullOrEmpty(sortowanie))
                //{
                //    long? szablon = null;
                //    sortowanie = PobierzKolumny(typ, ref szablon).First().NazwaPola;
                //}
            }
            else
            {
                SolexBllCalosc.PobierzInstancje.ProfilKlienta.DodajWartosc(SolexHelper.AktualnyKlient, TypUstawieniaKlienta.ListaKolumnWAdminieSortowanie, sortowanie + " " + sortowanieKolejnosc, typ.FullName);
            }
            return sortowanie;
        }

        [NonAction]
        private void UstawKolumny(Type typ, string[] kolumny, long? szablonId)
        {      
           if (szablonId != null)
            {
                SolexBllCalosc.PobierzInstancje.ProfilKlienta.DodajWartosc(SolexHelper.AktualnyKlient, TypUstawieniaKlienta.ListaKolumnWAdminieSzablon, szablonId, typ.FullName);
                SolexBllCalosc.PobierzInstancje.ProfilKlienta.UsunWartosc(SolexHelper.AktualnyKlient, TypUstawieniaKlienta.ListaKolumnWAdminie, typ.FullName);
            }
            else if (kolumny != null)
            {
                SolexBllCalosc.PobierzInstancje.ProfilKlienta.DodajWartosc(SolexHelper.AktualnyKlient, TypUstawieniaKlienta.ListaKolumnWAdminie, kolumny, typ.FullName);
                SolexBllCalosc.PobierzInstancje.ProfilKlienta.UsunWartosc(SolexHelper.AktualnyKlient, TypUstawieniaKlienta.ListaKolumnWAdminieSzablon, typ.FullName);
            }
        }

        /// <summary>
        /// Pobiera kolumny widoczne dla uzytkwonika wg. jego szablonu
        /// </summary>
        /// <param name="typ"></param>
        /// <param name="szablonId"></param>
        /// <returns></returns>
        [NonAction]
        private IList<OpisPolaObiektuBaza> PobierzKolumny(Type typ, ref long? szablonId)
        {
            string[] nazwykolumn = null;
            long wart = SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzWartosc<long>(SolexHelper.AktualnyKlient, TypUstawieniaKlienta.ListaKolumnWAdminieSzablon, typ.FullName); // CookieHelper.GetCookieValue(cookie);
            if (wart != 0)
            {
                szablonId = wart;
            }
            else
            {
                 nazwykolumn = SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzWartosc<string[]>(SolexHelper.AktualnyKlient, TypUstawieniaKlienta.ListaKolumnWAdminie, typ.FullName); // CookieHelper.GetCookieValue(cookie); 
            }
          
            if (szablonId.HasValue && szablonId != 0)
            {
                var szablon = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<UkladKolumn>(szablonId.Value);
                if (szablon != null && szablon.TypDanych == typ)
                {
                    nazwykolumn = szablon.WidoczneKolumny;
                }
            }
            OpisObiektu opis = OpisObiektu.StworzOpisObiektu(typ);
            if (nazwykolumn == null)
            {
                nazwykolumn = opis.PolaObiektu.Where(x => x.ParamatryWidocznosciAdmin.DomyslnieWidoczne).Select(x => x.NazwaPola).ToArray();
            }
            return opis.PolaObiektu.Where(x => nazwykolumn.Contains(x.NazwaPola)).ToList();
        }

        [Route("Lista")]
        public ActionResult Lista(Type typ, string[] szukanie = null, int numerStrony = 1, int? rozmiarStrony = null, string sortowanie = null, KolejnoscSortowania sortowanieKolejnosc = KolejnoscSortowania.desc)
        {
            if (!RouteData.Values.ContainsKey("typ"))
            {
                RouteData.Values.Add("typ", typ.PobierzOpisTypu());
            }
            else
            {
                RouteData.Values["typ"] = typ.PobierzOpisTypu();
            }
            long? szablon = null;
            IList<OpisPolaObiektuBaza> kolumny = PobierzKolumny(typ, ref szablon);
            int rozmiar;
            if (rozmiarStrony != null)
            {
                SolexBllCalosc.PobierzInstancje.ProfilKlienta.DodajWartosc(SolexHelper.AktualnyKlient, TypUstawieniaKlienta.ListaKolumnWAdminieRozmiarStrony,rozmiarStrony, typ.FullName);
                rozmiar=(int)rozmiarStrony;
            }
            else
            {
                rozmiar = SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzWartosc<int>(SolexHelper.AktualnyKlient,TypUstawieniaKlienta.ListaKolumnWAdminieRozmiarStrony, typ.FullName);
            }
            string sortowanieOdczytane = PobierzSortowanie(typ, sortowanie, ref sortowanieKolejnosc);

            DaneLista dane = AdminHelper.PobierzInstancje.PobierzDaneEdytora(SolexHelper.AktualnyKlient, typ, numerStrony, rozmiar, szukanie, sortowanieOdczytane, sortowanieKolejnosc, kolumny, SolexHelper.AktualnyJezyk, Url);

            return View("Lista", dane);
        }

        [Route("WyslijMaile/{idKlienta}")]
        [Route("WyslijMaile")]
        public ActionResult WyslijMaile(int idKlienta=0)
        {
            var k =idKlienta==0? SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Model.Klient>(null, x => x.Aktywny && (x.Gid!=null && x.Gid!="")): SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Model.Klient>(null, x => x.Id==idKlienta);
            foreach (var klient in k)
            {
                Klient kli = new Klient(klient);
                WyslijPowiadomienia.PobierzInstancje.WyslijPowiadomienieNowyKlient(kli);
            }
            
            return RedirectToAction("Lista", new { typ = typeof(Klient).PobierzOpisTypu() });
        }
        [Route("ListaCsv")]
        public FileContentResult ListaCsv(string typ, string[] szukanie = null, string sortowanie = null, KolejnoscSortowania sortowanieKolejnosc = KolejnoscSortowania.asc)
        {
            Type typDanych = Type.GetType(typ, true);
            long? szablon = null;
            var kolumny = PobierzKolumny(typDanych, ref szablon);
            int rozmiar = int.MaxValue;
            string sortowanieOdczytene = PobierzSortowanie(typDanych, sortowanie, ref sortowanieKolejnosc);
            DaneLista dane = AdminHelper.PobierzInstancje.PobierzDaneEdytora(SolexHelper.AktualnyKlient, typDanych, 1, rozmiar, szukanie, sortowanieOdczytene, sortowanieKolejnosc, kolumny, SolexHelper.AktualnyJezyk, Url);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < dane.KolumnyWidoczne.Count; i++)
            {
                sb.Append(dane.KolumnyWidoczne[i].NazwaWyswietlana);
                if (i < dane.KolumnyWidoczne.Count - 1)
                {
                    sb.Append(";");
                }
            }
            sb.Append("\r\n");
            var obiekty = dane.ObiektyDoPokazania.ToList();
            foreach (DaneObiekt t in obiekty)
            {
                for (int j = 0; j < t.Pola.Count; j++)
                {
                    sb.Append(t.Pola[j].Wartosc.Wartosc);
                    if (j < t.Pola.Count - 1)
                    {
                        sb.Append(";");
                    }
                }
                sb.Append("\r\n");
            }
            sb.Append("\r\n");

            var data = Encoding.UTF8.GetBytes( sb.ToString() );
            var result = Encoding.UTF8.GetPreamble().Concat(data).ToArray();

            return File(result, "application/csv", typ + ".csv");
        }

        //private Dictionary<ListaKolumnModel> _slownikPobierzUkladKolumn
        //{
            
        //}


        [ChildActionOnly]
        public ActionResult ListaKolumn(Type typ)
        {
            var opis = OpisObiektu.StworzOpisObiektu(typ);
            long? szablon = null;
            IList<OpisPolaObiektuBaza> kolumny = PobierzKolumny(typ, ref szablon);

            if (kolumny == null)
            {
                kolumny = opis.PolaObiektu.Where(x => x.ParamatryWidocznosciAdmin.DomyslnieWidoczne).ToArray();
            }

            var listaKolumnParametry = new ListaKolumnModel()
            {
                AktywnySzablon = szablon,
                Typ = typ,
                WidoczneKolumny = kolumny.Select(x => x.NazwaPola).ToArray(),
                WszystkieKolumny = opis.PolaObiektu.Where(x => x.ParamatryWidocznosciAdmin.DostepneLista).ToList()
            };

            listaKolumnParametry.IstniejaceSzablony = Calosc.DostepDane.Pobierz<UkladKolumn>(null, x => x.TypDanych == typ).ToList();   //todo: cache

            return PartialView("ListaKolumn", listaKolumnParametry);
        }

        [Route("ListaKolumnUstaw")]
        public ActionResult ListaKolumnUstaw(string typ, string[] wybraneKolumny, long? szablon = null)
        {
            Type typDanych = Type.GetType(typ, true);
            UstawKolumny(typDanych, wybraneKolumny, szablon);
            return RedirectToAction("Lista", new { typ });
        }
        [Route("ModulZbiorczePArametry")]
        public ActionResult ModulZbiorczePArametry(Type modul)
        {
            IList<OpisPolaObiektu> parametry = OpisObiektu.PobierzParametry(modul);
            return PartialView("ModulZbiorczeParametry", parametry);
        }
        [Route("WykonajZadanieZbiorcze")]
        public ActionResult WykonajZadanieZbiorcze(Type typ, OpisPolaObiektu[] data, string[] wybrane, Type modul)
        {
            AdminHelper.PobierzInstancje.WykonajZadanieZbiorcze(modul, data, wybrane);
            return RedirectToAction("Lista", new {typ= typ.PobierzOpisTypu() });
        }
        [Route("ZapiszSzablonKolumn")]
        public ActionResult ZapiszSzablonKolumn(string typ, string[] wybraneKolumny, string nazwa)
        {
            Type typDanych = Type.GetType(typ, true);
            var ukkol = new UkladKolumn { Nazwa = nazwa, WidoczneKolumny = wybraneKolumny, TypDanych = typDanych };
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(ukkol);
            UstawKolumny(typDanych, null, ukkol.Id);
            return RedirectToAction("Lista", new { typ });

        }
        [Route("EdytujPole")]
        [ValidateInput(false)]
        public ActionResult EdytujPole(Type typ, string klucz, OpisPolaObiektu dane)
        {
            var wartosc = dane.Wartosc as string[];
            if (wartosc!=null && wartosc.Any() && dane.WymuszonyTypEdytora==TypEdytora.PoleTekstoweMultiLine)
            {
                dane.Wartosc = HttpUtility.UrlDecode(wartosc.First(), System.Text.Encoding.Default);
            }

            // dane.IdentyfikatorObiektu = klucz;
            //dane.PobierzWartoscPolaObiektu(obiekt);
            var akcesor = typ.PobierzRefleksja();
            var poZmienie = AdminHelper.PobierzInstancje.AktualizujPoleObiektu(typ, klucz, dane.PolaNaslownik(), SolexHelper.AktualnyJezyk.Id, akcesor);
            OpisPolaObiektuBaza zmienione = OpisObiektu.PobranieParametowObiektu(poZmienie, klucz).First(x => x.NazwaPola == dane.NazwaPola);

            return PartialView("EdycjaInline",  zmienione);
        }

        [Route("Dodaj")]
        public ActionResult Dodaj(string typ, string typNadrzednego = null, string nadrzedny = null)
        {
            Type typdanych = Type.GetType(typ, true);
            Type typnadrzedny = typNadrzednego != null ? Type.GetType(typNadrzednego, true) : null;
            DaneEdycjaAdmin danewidokok = AdminHelper.PobierzInstancje.PobierzDaneDoDodania(typdanych, typnadrzedny, nadrzedny, SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDDomyslny, SolexHelper.AktualnyKlient, Url);
            danewidokok.Jezyki = null;
            return View("Edycja", danewidokok);
        }
        [Route("Edycja")]
        [Route("Edycja/{id}")]
        public ActionResult Edycja(string id, Type typ, Type typNadrzednego = null, string nadrzedny = null)//, int? jezyk = null)
        {
            //if (!jezyk.HasValue)
            //{
            //    jezyk = SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDDomyslny;
            //}
            if (!RouteData.Values.ContainsKey("typ"))
            {
                RouteData.Values.Add("typ", typ.PobierzOpisTypu());
            }
            //if (typ == typeof(ProduktBazowy))
            //{
            //    typ = typeof(Produkt);
            //}
            int jezyk = SolexHelper.AktualnyJezyk.Id;
            DaneEdycjaAdmin danewidokok = AdminHelper.PobierzInstancje.PobierzDaneDoEdycji(id, typ, typNadrzednego, nadrzedny, jezyk, SolexHelper.AktualnyKlient, Url);
            danewidokok.PolaObiektu.Where(x => x.Grupa.Grupa == "Linki").ForEach(x => x.Wartosc = Url.ZbudujLink(SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojednczyWgTypu(typ, null, id.ToString(), jezyk, true), SolexHelper.AktualnyJezyk,x.NazwaPola));
            return View("Edycja", danewidokok);
        }
        [Route("EdycjaModal")]
        public ActionResult EdycjaModal(string id, string typ, string typNadrzednego = null, string nadrzedny = null, int? jezyk = null)
        {
            Type typnadrzedny = typNadrzednego != null ? Type.GetType(typNadrzednego, true) : null;
            Type typdanych = Type.GetType(typ, true);
            if (!jezyk.HasValue)
            {
                jezyk = SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDDomyslny;
            }
            DaneEdycjaAdmin danewidokok = AdminHelper.PobierzInstancje.PobierzDaneDoEdycji(id, typdanych, typnadrzedny, nadrzedny, jezyk.Value, SolexHelper.AktualnyKlient, Url);
            if (typdanych == typeof(TrescKolumnaBll))
            {
                TrescKolumnaBll element = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<TrescKolumnaBll>(id);
                Type typkontrolki = Type.GetType(element.RodzajKontrolki, true);
                var obiekt = Activator.CreateInstance(typkontrolki) as KontrolkaTresciBaza;
                if (obiekt != null)
                {
                    var licencja = typkontrolki.GetCustomAttributes(true).FirstOrDefault(p => p.GetType() == typeof(Licencja)) as Licencja;
                    if (licencja != null)
                    {
                        string komunikat = string.Empty;
                        foreach (var lic in licencja.Licencje)
                        {
                            if (!SolexBllCalosc.PobierzInstancje.Konfiguracja.GetLicense(lic))
                            {
                                komunikat = "Kontrolka wymaga licencji: " + lic;
                                break;
                            }
                        }
                        ViewBag.Komunikat = komunikat;
                    }
                    ViewBag.Naglowek = obiekt.Nazwa;
                }
            }
            return PartialView("EdycjaModal", danewidokok);
        }
        [Route("Klonuj")]
        public void Klonuj(int id)
        {
            var kolumna =  SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<TrescKolumnaBll>(id);
            DodajKolumne(id, null, kolumna);
        }

        [Route("Usuwanie/{id}")]
        [Route("Usuwanie/{id}/{przekieruj:bool}/{idNadrzednego}")]
        public ActionResult Usuwanie(string id, string typ, bool przekieruj = true, string idNadrzednego = null, string typNadrzedny =null)
        {
            Type typdanych = Type.GetType(typ, true);
            AdminHelper.PobierzInstancje.Usun(typdanych, id);
            if (przekieruj)
            {
                if (string.IsNullOrEmpty(typNadrzedny))
                {
                    return RedirectToAction("Lista", new {typ});
                }
                return RedirectToAction("Edycja", new { id = idNadrzednego, typ = typNadrzedny });
            }
            return null;
        }
        [Route("WybierzWarunek")]
        public ViewResult WybierzWarunek(int nadrzedny)
        {
            ZadanieBll z = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ZadanieBll>(nadrzedny);
            ModulPosiadajacyWarunki o = (ModulPosiadajacyWarunki)z.StworzKontrolke();
            IList<Models.Admin.OpisModulu> wynik = Refleksja.PobierzListeKlasDziedziczacychPoKlasieBazowej(o.TypWarunkow).Where(x => !x.IsAbstract).Select(x => new Models.Admin.OpisModulu(x,nadrzedny )).ToList();
            return View("WybierzModul", wynik);
         
        }
        [Route("WybierzModul")]
        public ViewResult WybierzModul(Type typ)
        {
            RepreznetowyTypModulowAttribute t = typ.GetCustomAttribute<RepreznetowyTypModulowAttribute>();
            IList<Models.Admin.OpisModulu> wynik = Refleksja.PobierzListeKlasDziedziczacychPoKlasieBazowej(t.Typ).Where(x => !x.IsAbstract).Select(x =>new Models.Admin.OpisModulu(x,null)).OrderBy(x=>x.Nazwa).ToList();
            return View("WybierzModul", wynik);
        }
         [Route("DodajModul")]
        public ActionResult DodajModul(Type typ, int? parent = null)
        {
            ZadanieBll z = new ZadanieBll();
            z.ModulFullTypeName = typ.PobierzOpisTypu();
            z.JezykId = SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDDomyslny;
            z.ZadanieNadrzedne = parent;
            IList<OpisPolaObiektu> pars = OpisObiektu.PobierzParametry(typ);
            z.UstawParametry(pars);
            object id = SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(z);
            List<Type> typy = Refleksja.PobierzListeKlasZAtrybutem<RepreznetowyTypModulowAttribute>();
            Type baza = null;
            foreach (var t in typy)
            {
                RepreznetowyTypModulowAttribute atr = t.GetCustomAttribute<RepreznetowyTypModulowAttribute>();
                if (atr != null)
                {
                    if (typ.IsSubclassOf(atr.Typ))
                    {
                        baza = t;
                    }
                }
            }
            if (parent != null)
            {
                baza = typeof(ZadanieBll);
                ZadanieBll zb = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ZadanieBll>(parent);
                Type typParent = null;
                var tk = zb.StworzKontrolke().GetType();
                foreach (var t in typy)
                {
                    RepreznetowyTypModulowAttribute atr = t.GetCustomAttribute<RepreznetowyTypModulowAttribute>();
                    if (atr != null)
                    {
                        if (tk.IsSubclassOf(atr.Typ))
                        {
                            typParent = t;
                        }
                    }
                }

                return RedirectToAction("Edycja",
                    new
                    {
                        typ = baza.PobierzOpisTypu(),
                        id,
                        nadrzedny = parent,
                        typNadrzednego = typParent.PobierzOpisTypu()
                    });
            }

            //DaneEdycjaAdmin danewidokok = new DaneEdycjaAdmin
            //{
            //    AkcjaZapisz = "ZapiszObiektBedacyPojemnikiem",
            //    PolaObiektu = pars.ToArray(),
            //    Typ = typ,
            //    NazwaObiektu = z.NazwaZadanie,
            //    PrzyjaznyOpisObiektu = z.OpisZadanie
            //};
            //return View("Edycja", danewidokok);

            return RedirectToAction("Edycja", new { typ = baza.PobierzOpisTypu(), id });
        }

        [ValidateInput(false)]
        [Route("ZapiszObiektBedacyPojemnikiem")]
        public ActionResult ZapiszObiektBedacyPojemnikiem(DaneEdycjaAdmin dane, AkcjaEdycjiObiektu akcja)
        {
            string id = "";
            if (akcja != AkcjaEdycjiObiektu.Anuluj)
            {
                int idjezykadomyslnego = SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDDomyslny;
                id= AdminHelper.PobierzInstancje.ZapiszObiekt(dane);

                IObiektPrzechowujacyKontrolke tmp = (IObiektPrzechowujacyKontrolke) SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojednczyWgTypu(dane.Typ, null, dane.KluczWartosc??id, dane.JezykId, false);
                List<string> polaobiektu = tmp.GetType().GetProperties().Select(x => x.Name).ToList();
                var polaindywidualne = dane.PolaObiektu.Where(x => !polaobiektu.Contains(x.NazwaPola));
                var istniejace = tmp.PobierzParametry();
                List<OpisPolaObiektu> polaDoslownika = new List<OpisPolaObiektu>();
                foreach (var akt in polaindywidualne)
                {

                    if (!akt.Tlumaczone || dane.JezykId == idjezykadomyslnego)
                    {
                        istniejace.Remove(akt.NazwaPola);
                        istniejace.Add(akt.NazwaPola, akt.PobierzWartosc());
                    }
                    else
                    {
                        polaDoslownika.Add(akt);
                    }
                }
                tmp.UstawParametry(istniejace);
                object ktb = tmp.StworzKontrolke();
                Refleksja.UstawWartoscPol(ktb, polaDoslownika.PolaNaslownik());
                SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujLokalizacjePojedyncze(ktb);
                SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujWgTypu(tmp);
            }
            switch (akcja)
            {
                case AkcjaEdycjiObiektu.Zastosuj:
                    return RedirectToAction("Edycja", new { typ = dane.Typ.PobierzOpisTypu(), id, nadrzedny = dane.Nadrzedny, typNadrzednego =(dane.TypNadrzednych==null?null: dane.TypNadrzednych.PobierzOpisTypu()) });
                case AkcjaEdycjiObiektu.Nic:
                    return null;
                default:
                    if (dane.TypNadrzednych!=null)
                    {
                        return RedirectToAction("Edycja", new { typ = dane.TypNadrzednych.PobierzOpisTypu(), id = dane.Nadrzedny });
                    }
                    return RedirectToAction("Lista", new { typ = dane.Typ.PobierzOpisTypu() });
            }
        }

        [ValidateInput(false)]
        [Route("ZapiszObiekt")]
        public ActionResult ZapiszObiekt(DaneEdycjaAdmin dane, AkcjaEdycjiObiektu akcja)
        {
            string id = "";
            if (akcja != AkcjaEdycjiObiektu.Anuluj)
            {
                id = AdminHelper.PobierzInstancje.ZapiszObiekt(dane);
            }
            switch (akcja)
            {
                case AkcjaEdycjiObiektu.Zastosuj:
                    return RedirectToAction("Edycja", new { typ = dane.Typ.PobierzOpisTypu(), id, nadrzedny = dane.Nadrzedny, typNadrzednego = (dane.TypNadrzednych == null ? null : dane.TypNadrzednych.PobierzOpisTypu()) });
                case AkcjaEdycjiObiektu.Nic:
                    return null;
                default:
                    if (dane.TypNadrzednych!=null)
                    {
                        return RedirectToAction("Edycja", new { typ = dane.TypNadrzednych.PobierzOpisTypu(), id = dane.Nadrzedny });
                    }
                    return RedirectToAction("Lista", new { typ = dane.Typ.PobierzOpisTypu() });
            }
        }

        [Route("ZalogujJakoKlient/{id?}")]
        public ActionResult ZalogujJakoKlient(long id)
        {
            SolexHelper.ZmienNaAktualnejSesjiKlienta(id);

            string url = "/";
            if (SolexBllCalosc.PobierzInstancje.Konfiguracja.WieleJezykowWSystemie)
            {
                IKlient naKogoChcemySieZalogowac = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(id, SolexHelper.AktualnyJezyk.Id);
                string symbolJezyka = SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykiWSystemie[naKogoChcemySieZalogowac.JezykId].Symbol;
                url = "/" + symbolJezyka;
            }
        
            return Redirect(url);
        }
        [Route("PrzywrocZmiany")]
        public ActionResult PrzywrocZmiany(Type typ,string data, string znacznik,  string id)
        {

            SolexBllCalosc.PobierzInstancje.DostepDane.PrzywrocWersje(typ, data, znacznik);
            return RedirectToAction("Edycja", new {typ= typ.PobierzOpisTypu(), id});
        }
        [Route("HistoriaZmian")]
        public PartialViewResult HistoriaZmian(Type typ, string id)
        {
          Dictionary<string,List<ZmianaObiektu>> wynik=  AdminHelper.PobierzInstancje.PobierzHistorieZmian(typ, id);
          if (string.IsNullOrEmpty(id))
              return PartialView("_HistoriaZmianLista", wynik);
            return PartialView("_HistoriaZmian", new ParametryDoHistoriiZmian(wynik,typ,id));
        }
        [Route("Odswiez")]
        public ActionResult Odswiez(string adres)
        {
            HttpRuntime.UnloadAppDomain();
            return Redirect(adres);
        }
        [Route("TlumaczenieWLocie")]
        public PartialViewResult TlumaczenieWLocie(long hash)
        {
            IList<Tlumaczenie> tlumaczenia = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Tlumaczenie>(SolexHelper.AktualnyKlient,x => x.ObiektId == hash);
            Dictionary<int, string> slownikTlumaczen = new Dictionary<int, string>();
            foreach (Tlumaczenie tlumaczenie in tlumaczenia)
            {
                if (!slownikTlumaczen.ContainsKey(tlumaczenie.JezykId))
                {
                    slownikTlumaczen.Add(tlumaczenie.JezykId, tlumaczenie.Wpis);
                }
            }
          //  Dictionary<int, string> slownikTlumaczen = tlumaczenia.ToDictionary(x => x.JezykId, x => x.Wpis);
            TlumaczeniePole poleDomys = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<TlumaczeniePole>(hash);
            List<DaneDoTabow> taby = new List<DaneDoTabow>();
            List<TlumaczenieDoEdycji> daneDoEdycji = new List<TlumaczenieDoEdycji>();
            IList<Jezyk> jezyki = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Jezyk>(SolexHelper.AktualnyKlient);
            foreach (Jezyk jezyk in jezyki)
            {
                TlumaczenieDoEdycji tlumaczenie = new TlumaczenieDoEdycji();
                tlumaczenie.Jezyk = jezyk.Id;
                tlumaczenie.Hash = hash;
                string param = "data-idTlumaczenia = \"" + jezyk.Id + "\"";
                taby.Add(new DaneDoTabow(jezyk.Id.ToString(),jezyk.Nazwa,false,jezyk.Nazwa, false,param));

                if(jezyk.Domyslny)tlumaczenie.Tlumacznie= poleDomys.Nazwa;

                if (slownikTlumaczen.ContainsKey(jezyk.Id))
                {
                    tlumaczenie.Tlumacznie = slownikTlumaczen[jezyk.Id];
                }
               
                daneDoEdycji.Add(tlumaczenie);
            }
            return PartialView("TlumaczenieWLocie", daneDoEdycji);
        }

        [ValidateInput(false)]
        [Route("ZapiszTlumaczenie")]
        public ActionResult ZapiszTlumaczenie(List<TlumaczenieDoEdycji> pole)
        {
            IList< TlumaczeniePole > listaTlumaczen = new List<TlumaczeniePole>();
            foreach (TlumaczenieDoEdycji tlumaczenieDoEdycji in pole)
            {
                TlumaczeniePole poleDomys = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<TlumaczeniePole>(tlumaczenieDoEdycji.Hash);
                if (poleDomys == null)
                {
                    poleDomys = new TlumaczeniePole() {Id = tlumaczenieDoEdycji.Hash,MiejsceFrazy = MiejsceFrazy.Brak};
                }
                poleDomys.Nazwa = tlumaczenieDoEdycji.Tlumacznie;
                poleDomys.JezykId = tlumaczenieDoEdycji.Jezyk;
                listaTlumaczen.Add(poleDomys);
            }
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe(listaTlumaczen);
            SolexBllCalosc.PobierzInstancje.Konfiguracja.ResetSystemNames();
            return null;
        }
        [Route("PobierzTlumaczenia")]
        public void PobierzTlumaczenia(string frazy)
        {
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
            HttpContext.Response.ContentEncoding = Encoding.UTF8;
            HttpContext.Response.ContentType = "application/json";

            string resp = JSonHelper.Serialize(wynik);

            HttpContext.Response.Filter = new System.IO.Compression.GZipStream(HttpContext.Response.Filter, System.IO.Compression.CompressionMode.Compress);
            HttpContext.Response.AppendHeader("Content-Encoding", "gzip");

            HttpContext.Response.Write(resp);
        }

        [Route("importForm/{modal:regex(m)}")]
        public PartialViewResult ImportForm(Type typ)
        {
            return PartialView("ImportAdmin", typ);
        }

        /// <summary>
        /// Import plików 
        /// </summary>
        /// <param name="typ"></param>
        /// <returns></returns>
        [Route("Import")]
        [HttpPost]
        public ActionResult Import(Type typ)
        {
            if (Request.Files.Count == 0)
            {
                throw new Exception("Brak pliku do zaimportowania!");
            }

            HttpPostedFileBase file = Request.Files[0];
            List<Komunikat> result = new List<Komunikat>();
            if (file != null)
            {
                //zapisujemy plik w katalogu
                string katalogZapisu = Url.PobierzSciezkePlikUsera(typ, DateTime.Now.ToString("yy-MM-dd HH-mm-ss")+ SolexHelper.AktualnyKlient.Id,file.FileName,false);
                new UploadPlikow().ZapiszPlik(file, katalogZapisu);
                string sciezkaZewnetrzna =  Url.PobierzSciezkePlikUsera(typ, DateTime.Now.ToString("yy-MM-dd HH-mm-ss") + SolexHelper.AktualnyKlient.Id, file.FileName, true);
                string link = $"<a href={sciezkaZewnetrzna}>{file.FileName}</a>";

                SolexBllCalosc.PobierzInstancje.Statystyki.DodajZdarzenie(ZdarzenieGlowne.ImportPliku, "Link do pliku", link, SolexHelper.AktualnyKlient);
                string zawartosc = Tools.PobierzInstancje.GetContent(file.InputStream);

                result = WczytajObiekt(typ, zawartosc, SolexHelper.AktualnyKlient);
            }
            return PartialView("WynikImportu", new DaneDoWynikuImportu(result, file != null ? file.FileName : "", DateTime.Now));
        }    

        [NonAction]
        public static List<Komunikat> WczytajObiekt(Type typ, string dane, IKlient klient)
        {
            List<Komunikat> bledy;
            Dictionary<string, List<OpisPolaObiektu>> znalezione;
            try
            {
                ImportObiektuCsv import =  new ImportObiektuCsv();
                znalezione = import.Przetworz(typ, dane, out bledy);
            }
            catch 
            {
                Komunikat tmp = new Komunikat(SolexBllCalosc.PobierzInstancje.Konfiguracja.PobierzTlumaczenie(klient.JezykId,"Nie udało się odczytać pliku"), KomunikatRodzaj.danger, "MenagerImportow");
                return new List<Komunikat> { tmp };
            }
            List<Komunikat> dodane = new List<Komunikat>();
            if (znalezione.Any())
            {
                var akcesor = typ.PobierzRefleksja();
                foreach (var p in znalezione)
                {
                    Komunikat tmp;
                    try
                    {
                        AdminHelper.PobierzInstancje.AktualizujPoleObiektu(typ, p.Key, p.Value.PolaNaslownik(), klient.JezykId, akcesor);
                        tmp = new Komunikat($"Zaktualizowano obiekt {p.Key}", KomunikatRodzaj.success, "MenagerImportowDodano");
                       
                    } catch (Exception e)
                    {
                        tmp = new Komunikat($"{e.Message} [{p.Key}]", KomunikatRodzaj.danger, "MenagerImportowDodano");
                    }
                    dodane.Add(tmp);
                }
            }
            dodane.AddRange(bledy);
            return dodane;
        }
    }

    public class TlumaczenieDoEdycji
    {
        public int Jezyk{ get; set; }
        public string Tlumacznie { get; set; }
        public long Hash { get; set; }
    }
}