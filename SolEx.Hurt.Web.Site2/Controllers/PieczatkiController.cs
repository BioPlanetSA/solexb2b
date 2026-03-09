using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceStack.Common.Extensions;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers.Pieczatki;
using SolEx.Hurt.Web.Site2.Modules;
using SolEx.Hurt.Web.Site2.PageBases;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Web.Site2.Helper;
using SolEx.Hurt.Web.Site2.Providers;

namespace SolEx.Hurt.Web.Site2.Controllers
{
    public class PieczatkiController : SolexControler
    {
        //1. lista typow
        public PartialViewResult Index()
        {
            //Wczytuje strone główna w ktorej generowane sa typy pieczatek
            List<PieczatkiTyp> listaTypow = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<PieczatkiTyp>(null).ToList();
            return PartialView("_ListaTypow", new Tuple<List<PieczatkiTyp>, IKlient> (listaTypow, SolexHelper.AktualnyKlient));
        }

        /// <summary>
        /// Akcja wykorzystywana w momencie ładowania edytora z pliku
        /// </summary>
        /// <param name="zalacznik"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase zalacznik)
        {
            if (zalacznik == null)
            {
                throw new HttpException(404, "Nie znaleziono pliku");
            }
            if (!zalacznik.FileName.Contains(".json"))
            {
                throw new HttpException(500, "Niepoprawny format pliku");
            }

            //odczytuje z pliku informacje o typie, rozmiarze i informacji czy jest zablokowana
            var serializer = new JsonSerializer();
            
            using (var sr = new StreamReader(zalacznik.InputStream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                var json = serializer.Deserialize(jsonTextReader).ToString();
                
                JObject o = JObject.Parse(json);
                var first = o.First.First.First;

                var typPieczatki = (string)first["typPieczatki"];
                var zablokowany = (bool)first["zablokowany"];

                int typId = SolexBllCalosc.PobierzInstancje.PieczatkiBll.PobierzTypPieczatkiPoSymbolu(typPieczatki).Id;


                //zapis wczytanego szablonu do tymczasowego katalogu

                string sciezka = Url.PobierzSciezkePlikUsera(typeof(PieczatkiSzablony), typId, zalacznik.FileName, false);     /* "/Zasoby/szablony_pieczatek/temp/"+data;*/
                new UploadPlikow().ZapiszPlik(zalacznik, sciezka);


                var typ = SolexBllCalosc.PobierzInstancje.PieczatkiBll.PobierzKoloryPieczatkiPoIdTypu(typId);
                if (typId > 0)
                {
                    var szablonView = new PieczatkiSzablonyViewModel(0, typId, "", "", "", typ.Szerokosc_mm, typ.Wysokosc_mm, "", "", "");
                    szablonView.Zablokowany = zablokowany;
                    szablonView.SymbolTypu = typPieczatki;
                    szablonView.SciezkaDoPlikuSzablonuJSON = sciezka + zalacznik.FileName;

                    return PartialView("_Edytor", new Tuple<PieczatkiTyp, PieczatkiSzablonyViewModel, IKlient>(typ, szablonView, SolexHelper.AktualnyKlient));
                }
            }
            throw new HttpException(500, "Coś poszło nie tak");
        }

        //2. lista szablonów
        [Route("pieczatki/szablony/{id?}")]
        public ActionResult Szablony(int id)
        {
            var typ = SolexBllCalosc.PobierzInstancje.PieczatkiBll.PobierzTypPieczatkiPoId(id);
            var szablony = SolexBllCalosc.PobierzInstancje.PieczatkiBll.PobierzSzablonyPoIdTypu(typ.Id);
            
            var listaSzablonow = szablony.Select(item => new PieczatkiSzablonyViewModel(item.Id, item.TypId, "", item.SciezkaDoPlikuSzablonuJSON, item.SciezkaDoPlikuSzablonuSVG,
                typ.Szerokosc_mm,typ.Wysokosc_mm, typ.SymbolTypu,item.Nazwa, item.Opis)).ToList();
            return View("_ListaSzablonow", new Tuple<PieczatkiTyp, List<PieczatkiSzablonyViewModel>, IKlient>(typ, listaSzablonow, SolexHelper.AktualnyKlient));
        }

        //3. ladowanie edytora
        [Route("pieczatki/edytor/{id?}/{typId?}/{wysokosc?}/{szerokosc?}")]
        public ActionResult Edytor(int? id, int? typId, decimal? wysokosc, decimal? szerokosc)
        {
            if (id > 0 && typId== null && wysokosc==null && szerokosc == null)
            {
                var szablon = SolexBllCalosc.PobierzInstancje.PieczatkiBll.PobierzSzablonPoId(id);
                if (szablon == null)
                {
                    throw new HttpException(404, "Nie znaleziono");
                }
                var typ = SolexBllCalosc.PobierzInstancje.PieczatkiBll.PobierzTypPieczatkiPoId(szablon.TypId);
                var szablonView = new PieczatkiSzablonyViewModel(szablon.Id, szablon.TypId, typ.Nazwa,
                    szablon.SciezkaDoPlikuSzablonuJSON, szablon.SciezkaDoPlikuSzablonuSVG, typ.Szerokosc_mm,
                    typ.Wysokosc_mm, typ.OpisHtml,szablon.Nazwa, szablon.Opis) {SymbolTypu = typ.SymbolTypu, Zablokowany = szablon.Zablokowany};
                return View("_Edytor",
                    new Tuple<PieczatkiTyp, PieczatkiSzablonyViewModel, IKlient>(typ, szablonView, SolexHelper.AktualnyKlient));
            }
            else
            {
                if (id!= null && typId.HasValue && wysokosc.HasValue && szerokosc.HasValue)
                {
                    var typ = SolexBllCalosc.PobierzInstancje.PieczatkiBll.PobierzTypPieczatkiPoId(id.Value);
                    if (typId > 0 && wysokosc > 0 && szerokosc > 0 && typ != null)
                    {
                        var szablonView = new PieczatkiSzablonyViewModel(id.Value, typId.Value, "", "", "", szerokosc.Value, wysokosc.Value, "", "","");
                        szablonView.SymbolTypu = typ.SymbolTypu;

                        return View("_Edytor",
                    new Tuple<PieczatkiTyp, PieczatkiSzablonyViewModel, IKlient>(typ, szablonView, SolexHelper.AktualnyKlient));
                    }
                }
            }
            throw new HttpException(404, "Nie znaleziono");
        }


        public ActionResult WczytajGalerie()
        {
            List<string> pliki = new List<string>();
            var directory = AppDomain.CurrentDomain.BaseDirectory + @"\Zasoby\szablony_pieczatek\galeria\";
            if (Directory.Exists(directory))
            {
                pliki = Directory.GetFiles(directory, "*.svg")
                        .Select(Path.GetFileName)
                        .ToList();

            }

            return PartialView("_Galeria", pliki);
        }

        //Dodaj do koszyka
        public ActionResult WybierzAutomatPieczatki(string typId)
        {
            var obiekt = SolexBllCalosc.PobierzInstancje.PieczatkiBll.PobierzTypPieczatkiPoId(Convert.ToInt32(typId));
                //PieczatkiBll.PobierzInstancje.PobierzTypPieczatkiPoId(Convert.ToInt32(typId));
            var produkty = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ProduktBazowy>(SolexHelper.AktualnyJezyk.Id, null, x => obiekt.PowiazaneProduktyId.Contains(x.Id)).ToList();
                //SolexBllCalosc.PobierzInstancje.ProduktyBazowe.Wszystykie().Where(x => obiekt.PowiazaneProduktyId.Contains(x.produkt_id)).ToList();
            
            //var obiekt = PieczatkiBll.PobierzInstancje.PobierzTypPieczatkiPoSymbolu(symbol); //do napisania
            return PartialView("_WybierzAutomat", produkty);
        }

        /// <summary>
        /// Ładuje kolory dla danego typu pieczątki
        /// </summary>
        /// <returns></returns>
        public JsonResult WczytajKolory(string id)
        {
            if (id == null) throw new ArgumentNullException("id");
            var kolory = SolexBllCalosc.PobierzInstancje.PieczatkiBll.PobierzKoloryPieczatkiPoIdTypu(id);
            if (kolory != null)
            {
                if (Request.IsAjaxRequest())
                {
                    if (kolory.DozwoloneKoloryCalejPieczatki == null)
                    {
                        kolory.DozwoloneKoloryCalejPieczatki = "black";
                    }
                    return Json(kolory.DozwoloneKoloryCalejPieczatki, JsonRequestBehavior.AllowGet); 
                }
            }
            return Json("", JsonRequestBehavior.AllowGet); 
        }

        // *********************** TYP ************************************//
        [Autoryzacja(RoleType.Administrator)]
        public ActionResult DodajTyp()
        {
           // var model = new PieczatkiTypProduktViewModel();
            throw new NotImplementedException();
            //model.OpisPolaObiektu.Multiselect = true;
            //model.OpisPolaObiektu.TypSlownika = typeof(SlownikProduktow);
            //model.OpisPolaObiektu.RodzajDanych = typeof(Produkt);
            //model.OpisPolaObiektu.NazwaPola = "pieczatkiProdukty";
            //model.OpisPolaObiektu.Wymagane = false;
          //  return View("_DodajTyp", model);
        }

        [HttpPost]
        [ValidateInput(false)] 
        public ActionResult DodajTyp(PieczatkiTypProduktViewModel model)
        {
            throw new NotImplementedException();
            //foreach (var item in model.ParametrModulow.PodajWartosciListy)
            //{
            //    model.PieczatkiTyp.PowiazaneProduktyId.Add(Convert.ToInt32(item));
            //}
            //SolexBllCalosc.PobierzInstancje.PieczatkiBll.DodajTyp(model.PieczatkiTyp);
            ////PieczatkiBll.PobierzInstancje.DodajTyp(model.PieczatkiTyp);
            //return RedirectToAction("Index");
        }

        [Autoryzacja(RoleType.Administrator)]
        public ActionResult EdytujTyp(int id)
        {
            throw new NotImplementedException();
            //var model = new PieczatkiTypProduktViewModel();
            ////model.OpisPolaObiektu.Multiselect = true;
            ////model.OpisPolaObiektu.TypSlownika = typeof(SlownikProduktow);
            ////model.OpisPolaObiektu.RodzajDanych = typeof(Produkt);
            ////model.OpisPolaObiektu.NazwaPola = "pieczatkiProdukty";
            ////model.OpisPolaObiektu.Wymagane = true;

            ////model.PieczatkiTyp = SolexBllCalosc.PobierzInstancje.PieczatkiBll.PobierzTypPieczatkiPoId(id);
            ////    //PieczatkiBll.PobierzInstancje.PobierzTypPieczatkiPoId(id);

            ////List<int> listaProduktowId = model.PieczatkiTyp.PowiazaneProduktyId;
            ////var produkty = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ProduktBazowy>(SolexHelper.AktualnyJezykId, null);
            ////    //SolexBllCalosc.PobierzInstancje.ProduktyBazowe.Wszystykie();
            ////var lista = produkty.Where(x => listaProduktowId.Contains(x.Id)).ToList();

            ////Dictionary<string, object> dict  = new Dictionary<string, object>();
            ////foreach (var o in lista)
            ////{
            ////    var nazwa = String.Format("{0} ( - {1})", o.Nazwa, o.Id);
            ////    dict.Add(nazwa, o.Id);

            ////    model.OpisPolaObiektu.Wartosc += o.Id + ";";
            ////}
            ////throw new NotImplementedException();
            //////if (!string.IsNullOrEmpty(model.ParametrModulow.Wartosc))
            //////{
            //////    model.ParametrModulow.Wartosc = model.ParametrModulow.Wartosc.Remove(model.ParametrModulow.Wartosc.Length - 1);
            //////}
            
            ////model.OpisPolaObiektu.Slownik = dict;
            
            //return View("_EdytujTyp", model);
        }

        [HttpPost]
        [ValidateInput(false)] 
        [Autoryzacja(RoleType.Administrator)]
        public ActionResult EdytujTyp(PieczatkiTypProduktViewModel model)
        {
            throw new NotImplementedException();
            //foreach (var item in model.ParametrModulow.PodajWartosciListy)
            //{
            //    model.PieczatkiTyp.PowiazaneProduktyId.Add(Convert.ToInt32(item));
            //}
            //SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(model.PieczatkiTyp);
            ////PieczatkiBll.PobierzInstancje.AktualizujTyp(model.PieczatkiTyp);
            //return RedirectToAction("Index");
        }

        [HttpGet]
        [Autoryzacja(RoleType.Administrator)]
        public ActionResult UsunTyp(int id)
        {
            SolexBllCalosc.PobierzInstancje.PieczatkiBll.UsunTyp(id);
            return RedirectToAction("Index");
        }

        // *********************** SzablonListyProduktow ************************************//

        /// <summary>
        /// Usuwa wybrany szablon 
        /// </summary>
        /// <param name="idStrony">po usunięciu pliku wraca do strony o id typu podanym w parametrze</param>
        /// <param name="idSzablonu">usuwa szablon o danym id</param>
        /// <returns></returns>
        [Route("pieczatki/szablony/usun/{idStrony}/{idSzablonu}")]
        [Autoryzacja(RoleType.Administrator)]
        public ActionResult UsunSzablon(int? idStrony, int? idSzablonu)
        {
            if (idStrony > 0 && idSzablonu > 0)
            {
                var szablon = SolexBllCalosc.PobierzInstancje.PieczatkiBll.PobierzSzablonPoId(idSzablonu);
                var sciezkaSerwera = Server.MapPath(@"~/");
                var sciezkaJson = szablon.SciezkaDoPlikuSzablonuJSON;
                var sciezkaSvg = szablon.SciezkaDoPlikuSzablonuSVG;

                if (!string.IsNullOrEmpty(sciezkaJson) && !string.IsNullOrEmpty(sciezkaSvg))
                {
                    sciezkaJson = szablon.SciezkaDoPlikuSzablonuJSON.Remove(0, 1);
                    sciezkaSvg = szablon.SciezkaDoPlikuSzablonuSVG.Remove(0, 1);

                    if (System.IO.File.Exists(Path.Combine(sciezkaSerwera, sciezkaJson.ReplaceAll("/", "\\"))))
                    {
                        System.IO.File.Delete(sciezkaSerwera + szablon.SciezkaDoPlikuSzablonuJSON);
                    }


                    if (System.IO.File.Exists(Path.Combine(sciezkaSerwera, sciezkaSvg)))
                    {
                        System.IO.File.Delete(sciezkaSerwera + szablon.SciezkaDoPlikuSzablonuSVG);
                    }
                }

                SolexBllCalosc.PobierzInstancje.DostepDane.UsunPojedynczy<PieczatkiSzablony>(idSzablonu);

                return Redirect("/pieczatki/szablony/" + idStrony);
            }
            throw new HttpException(404, "Nie znaleziono");
        }

        [Route("pieczatki/szablony/edytuj/{idTypu?}/{idSzablonu?}")]
        [Autoryzacja(RoleType.Administrator)]
        [HttpGet]
        public ActionResult EdytujSzablon(int? idTypu, int? idSzablonu)
        {
            var pieczec = SolexBllCalosc.PobierzInstancje.PieczatkiBll.PobierzSzablonPoId(idSzablonu);
            return View("_EdytujSzablon", pieczec);
        }

        [Route("pieczatki/szablony/edytuj/{idTypu?}/{idSzablonu?}")]
        [Autoryzacja(RoleType.Administrator)]
        [HttpPost]
        [ValidateInput(false)] 
        public ActionResult EdytujSzablon(PieczatkiSzablony szablon)
        {
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(szablon);
            return View("_EdytujSzablon", szablon);
        }

        /// <summary>
        /// Wyświetla okno modal do którego przekzywane są parametry pieczątki
        /// </summary>
        /// <param name="symbol">np. pieczatka_prostokatna</param>
        /// <param name="wysokosc">wysokosc w mm</param>
        /// <param name="szerokosc">szerokosc w mm</param>
        /// <returns>Okno modalowe</returns>
        public ActionResult ZapiszSzablonModal(string symbol, decimal wysokosc, decimal szerokosc)
        {
            var req = Request.UrlReferrer;

            int id =-1;
            
            //nowy szablon
            if (req != null && req.Segments.Length == 7)
            {
                id = Convert.ToInt32(req.Segments[3].Remove(req.Segments[3].Length - 1, 1));
            }

            //istniejący szablon
            if (req != null && req.Segments.Length == 4)
            {
                id = Convert.ToInt32(req.Segments[3]);
                var asd = SolexBllCalosc.PobierzInstancje.PieczatkiBll.PobierzSzablonPoId(id);
                id = asd.TypId;
            }

            PieczatkiSzablonyViewModel ps = new PieczatkiSzablonyViewModel();
            var typ = SolexBllCalosc.PobierzInstancje.PieczatkiBll.PobierzTypPieczatkiPoSymbolu(symbol);
            ps.Id = id;
            ps.Szerokosc_mm = szerokosc;
            ps.Wysokosc_mm = wysokosc;
            ps.NazwaTypu = typ.Nazwa;
            return PartialView("_ZapiszSzablonModal", ps);
        }

        [Autoryzacja(RoleType.Administrator)]
        public JsonResult ZapiszSzablon(string typ, string nazwa, string nazwaPliku, decimal szerokosc, decimal wysokosc, string opis, string json, string svg, string zablokowana)
        {
            PieczatkiSzablony szablon = new PieczatkiSzablony();
            string sciezka = "/Zasoby/szablony_pieczatek/";
            int typId = -1;
            if (typ == "pieczatka_prostokatna")
            {
                sciezka += "prostokatne/";
                typId = 1;
            }
            else if (typ == "pieczatka_okragla")
            {
                sciezka += "okragle/";
                typId = 2;
            }
            else if (typ == "pieczatka_owalna")
            {
                sciezka += "owalne/";
                typId = 3; 
            }
            string sciezkaSerw = Server.MapPath(@"~/") + sciezka;




            if (!Directory.Exists(sciezkaSerw))
            {
                Directory.CreateDirectory(sciezkaSerw);
            }

            var req = Request.UrlReferrer;

            int id = -1;

            //nowy szablon
            if (req != null && req.Segments.Length == 7)
            {
                id = Convert.ToInt32(req.Segments[3].Remove(req.Segments[3].Length - 1, 1));
            }
            //istniejący szablon
            if (req != null && req.Segments.Length == 4)
            {
                id = Convert.ToInt32(req.Segments[3]);
                var asd = SolexBllCalosc.PobierzInstancje.PieczatkiBll.PobierzSzablonPoId(id);
                id = asd.TypId;
            }

            szablon.TypId = id;
            szablon.Nazwa = nazwa;
            szablon.Opis = opis;
            szablon.Zablokowany = Convert.ToBoolean(zablokowana);

            string plikjson = sciezkaSerw + nazwaPliku + ".json";
            string pliksvg = sciezkaSerw + nazwaPliku + ".svg";


            if (!System.IO.File.Exists(plikjson) && !System.IO.File.Exists(pliksvg))
            {
                using (var sw = new StreamWriter(plikjson))
                {
                    sw.Write(json);
                }
                using (var sw = new StreamWriter(pliksvg))
                {
                    sw.Write(svg);
                }

                szablon.SciezkaDoPlikuSzablonuJSON = sciezka + nazwaPliku + ".json";
                szablon.SciezkaDoPlikuSzablonuSVG = sciezka + nazwaPliku + ".svg";

                if (typId != -1)
                {
                    SolexBllCalosc.PobierzInstancje.PieczatkiBll.DodajSzablon(szablon);
                    return Json("Dodano nowy szablon", JsonRequestBehavior.AllowGet);
                }
                return Json("Brak pieczątki o danym typie", JsonRequestBehavior.AllowGet);
            }
            return Json("Plik o podanej nazwie już istnieje", JsonRequestBehavior.AllowGet);
        }

        public ActionResult WybierzModel()
        {
          //  return PartialView("_WybierzModel");
            return null;
        }

        public ActionResult ZacznijOdNowaModal()
        {
          //  return PartialView("_ZacznijOdNowaModal");
            return null;
        }

        [Autoryzacja(RoleType.Klient)]
        public ActionResult RealizujZamowienie()
        {
          //  return PartialView("_WybierzModel");
            return null;
        }

        /// <summary>
        /// Tworzy pdf z danych binarnych przesłanych od klienta
        /// </summary>
        /// <param name="pdf">base64</param>
        [HttpPost]
        [Autoryzacja(RoleType.Klient)]
        public JsonResult ZapiszPdf(string pdf)
        {
            string sciekaPdf = Server.MapPath(@"~/") + "/Zasoby/pieczatki/PDF/";

            if (!Directory.Exists(sciekaPdf))
            {
                Directory.CreateDirectory(sciekaPdf);
            }
            sciekaPdf += "/" + DateTime.Now.ToString("dd-MM-yyyy");
            if (!Directory.Exists(sciekaPdf))
            {
                Directory.CreateDirectory(sciekaPdf);
            }
            //zapis do pliku
            string sciezka = sciekaPdf + "/" + SolexHelper.AktualnyKlient.Email + "_" +DateTime.Now.ToString("HH-mm-ss") + ".pdf";
            byte[] bytes = Convert.FromBase64String(pdf);
            var stream = new FileStream(sciezka, FileMode.CreateNew);
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(bytes, 0, bytes.Length);
            }
            return Json(sciezka, JsonRequestBehavior.AllowGet); 
        }
    }
}