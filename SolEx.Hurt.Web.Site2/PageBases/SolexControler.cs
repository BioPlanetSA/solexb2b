using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using log4net;
using Newtonsoft.Json;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Web.Site2.Helper;
using SolEx.Hurt.Web.Site2.Models;

namespace SolEx.Hurt.Web.Site2.PageBases
{
    /// <summary>
    /// Klasa bazowa dla wszystkich kontrolerów, udostępnia podstawowe wspólne elementy
    /// </summary>

    public abstract class SolexControler : Controller
    {
        public ISolexBllCalosc Calosc = SolexBllCalosc.PobierzInstancje;

        public SolexHelper SolexHelper { get; set; }

        [NonAction]
        protected Dictionary<int, HashSet<long>> StaleFiltryAktualnieWybrane()
        {
            return Calosc.ProfilKlienta.PobierzStaleFiltry(SolexHelper.AktualnyKlient);
        }

        [NonAction]
        protected TDane PobierzKontrolke<TDane>(int klucz)
        {
            var wynik = Calosc.DostepDane.PobierzPojedynczy<TrescKolumnaBll>(klucz).Kontrolka();
            return (TDane)wynik;
        }

        [NonAction]
        protected string SzukanaFrazaGlobalnie()
        {
            //todo:tymczasowo sprawdzmy czy obiekt produktów wyslany - docelowo trzeb to usunac
            string szukanie = Request.Form.Get("SzukanaFrazaGlobalnie");    //bartek modyfiakcja nie wiem czy zadziala
            if (szukanie != null)
            {
                return szukanie;
            }

            //czy w QS podany szukane
            szukanie = Request.QueryString.Get("szukane");
            if (szukanie != null)
            {
                return szukanie;
            }

            return null;
        }

        /// <summary>
        /// Przekształca wysłany obiekt na jsona, jeśli obiekt jest słownikiem, to pola będą w formie Key - wartośc klucz, Value= wartosc, a nie {Klucz:Wartosc}
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [NonAction]
        protected JsonResult PrzeksztalcNaJson(object data)
        {
            if (data is IDictionary<string, object>)
            {
                data = (data as IDictionary<string, object>).Select(x => x).ToList();
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [NonAction]
        protected bool SprawdzCaptcha()
        {
            var response = Request["g-recaptcha-response"];
            //secret that was generated in key value pair
            string secret =SolexBllCalosc.PobierzInstancje.Konfiguracja.GoogleCaptchaKluczPrywatny;
            if (String.IsNullOrEmpty(secret))
            {
                Log.Info("Próba uzycja captcha bez wpisania kluczy, automatyczne zatwierdzenie captcha");
                return true;
            }
            WebClient client = new WebClient();
            string reply = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));

            CaptchaResponse captchaResponse = JsonConvert.DeserializeObject<CaptchaResponse>(reply);

            //when response is false check for the error message
            return captchaResponse.Success;
        }

        [NonAction]
        protected void WyslijPlik(byte[] dane, Encoding kodowanie, string nazwa)
        {
            string typ = Path.GetExtension(nazwa);
            if (typ == null)
            {
                throw new Exception("Błąd pobierania typu");
            }
            string type = Tools.PobierzInstancje.GetMimeType(typ.ToLower());
            Response.ContentEncoding = kodowanie;
            Response.ContentType = type;
            Response.AddHeader("content-disposition", "attachment; filename=\"" + nazwa + "\"");
            Response.BinaryWrite(dane);

        }
       
        protected ILog Log
        {
            get
            {
                return LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            }
        }
        
        protected PartialViewResult StworzPolePojedynczegoWpisu(object o, string pole, string naglowek, string preset, string opakowanie, string metaTytul, string metaOpis, string metaTagi, string stopka,  string domyslnie="")
        {
            if (o == null)
            {
                return null;
            }
            var propertis = o.GetType().GetProperty(pole);
            string szablon = "PojedynczePoleTekst";
            object wartosc = null;
            List<ProduktKlienta> listaProduktow = null;
            var war = propertis.GetValue(o, null);
            if (war == null)
            {
                if (domyslnie == null)
                {
                    return null;
                }
                wartosc = domyslnie;
                szablon = "PojedynczePoleTekst";
            }
            else
            {
                if (propertis.PropertyType == typeof (IObrazek))
                {
                    IObrazek w = (IObrazek) war;
                    w.DomyslnyPreset = preset;
                    wartosc = w;
                    szablon = "PojedynczeZdjecie";
                }else if (propertis.PropertyType == typeof (IKlient))
                {
                    IKlient w = (IKlient) war;
                    string autorNazwa = w.Nazwa;
                    wartosc = autorNazwa;
                    szablon = "PojedynczePoleTekst";
                }else if (propertis.PropertyType == typeof (int[]))
                {
                    throw new Exception("Brakt co to?");
                    //var w = ((int[]) war).Select(Convert.ToInt64);
                    //IEnumerable<long> idProduktowKlienta = SolexBllCalosc.PobierzInstancje.ProduktyKlienta.PobierzIdProduktowRzeczywiscieDostepnychDlaKlienta(SolexHelper.AktualnyKlient).Where(w.Contains);
                    //listaProduktow = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ProduktKlienta>(SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient, x => idProduktowKlienta.Contains(x.Id)).ToList();
                    //szablon = "PojedynczePoleProduktu";
                }
                else
                {
                    wartosc = string.IsNullOrEmpty(opakowanie) ? war.ToString() : string.Format(opakowanie, war);
                }
            }
           
            return PartialView("PojedynczePole/"+szablon, new ParametryPrzekazywaneDoSzegolow(naglowek, wartosc, false, listaProduktow, metaTytul, metaOpis, metaTagi, stopka));
        }

        protected PartialViewResult WspolnaCzescSciezka(string adres, string nazwa, string symbolPoprzednejTresci)
        {
            List<Tuple<string,string>> model=new List<Tuple<string, string>>();

            if (!string.IsNullOrEmpty(symbolPoprzednejTresci))
            {
                TrescBll tmp = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<TrescBll>(x => x.Symbol == symbolPoprzednejTresci, SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient, false);
                if (tmp != null)
                {
                    string link = Url.ZbudujLink(tmp,SolexHelper.AktualnyKlient, SolexHelper.AktualnyJezyk);
                    //przechwyt - produkt moga byc nie klilalne z menu - wtedy jest brak linku. Na chama robimy jakiś link
                    if (string.IsNullOrEmpty(link) && !string.IsNullOrEmpty(tmp.Symbol) )
                    {
                        link = tmp.Symbol;
                    }
                    model.Add(new Tuple<string, string>(link, tmp.Nazwa));
                }
            }
            model.Add(new Tuple<string, string>(adres, nazwa));
            return PartialView("SciezkaUzupelnienieAjax", model);
        }


        /// <summary>
        /// przekierowanie na strone podaną w parametrach po wykonanej akcji
        /// </summary>
        /// <param name="powrot">strona gdzie ma być przekierowanie</param>
        /// <returns></returns>
        [NonAction]
        protected ActionResult AkcjaPowrotu(string powrot = "/")
        {
            powrot = Url.ZbudujLinkPowrotu(Request.IsAjaxRequest(), Request.UrlReferrer != null ?  Request.UrlReferrer.ToString() : null, SolexHelper.AktualnyJezyk, powrot);
            if (powrot == null)
            {
                return null;
            }
            return Redirect(powrot);
        }


    }
}