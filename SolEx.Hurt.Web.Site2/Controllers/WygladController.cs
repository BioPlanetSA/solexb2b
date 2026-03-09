using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Web.Site2.Models;
using SolEx.Hurt.Web.Site2.Models.DaneDoWidokow;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Controllers
{
    [RoutePrefix("wyglad")]
    public class WygladController : SolexControler
    {
        [Route("Ciasteczka")]
        public PartialViewResult Ciasteczka(string tresc)
        {
            if (SolexHelper.AktualnyKlient.Dostep == AccesLevel.Zalogowani)
            {
                return null;
            }
            return PartialView("_Ciasteczka", tresc);
        }
        [Route("DaneWlasciciela")]
        public PartialViewResult DaneWlasciciela(string formatRegionu, string prefix, string sofix)
        {
            Owner wlasciciel = SolexBllCalosc.PobierzInstancje.Konfiguracja.GetOwner();
            return PartialView("_DaneWlasciciela",new ParametryDoWlasciciela(wlasciciel,prefix, sofix, formatRegionu));
        }
        [Route("Drukuj")]
        public PartialViewResult Drukuj(string prefix, string sofix, string selektorHtmlElementu)
        {
            return PartialView("_Drukuj",new Tuple<string, string, string>(prefix, sofix, selektorHtmlElementu));
        }

        private Dictionary<string, decimal> obslugiwaneprzedladarki = new Dictionary<string, decimal>
        {
            { "InternetExplorer", 11 },
            { "Firefox", 31 },
            { "Chrome", 36 },
            { "IE", 11 },
            { "Safari", 5.1M }
        };
        [Route("SprawdzaniePrzegladarki")]
        public PartialViewResult SprawdzaniePrzegladarki()
        {
            decimal wersja;
            if (TextHelper.PobierzInstancje.SprobojSparsowac(Request.Browser.Version, out wersja))
            {
                if (obslugiwaneprzedladarki.ContainsKey(Request.Browser.Browser))
                {
                    if (obslugiwaneprzedladarki[Request.Browser.Browser] <= wersja)
                    {
                        return null;
                    }
                }
            }
            return PartialView("_SprawdzaniePrzegladarki", obslugiwaneprzedladarki);
        }
        [Route("Translator")]
        public ActionResult Translator(string jezykWyswietlania = "",string trybWyswietlania="")
        {
            GoogleTlumaczeniaViewModel model = new GoogleTlumaczeniaViewModel();
            model.JezykWyswietlania = jezykWyswietlania;
            model.TrybWyswietlania = trybWyswietlania;
            
            return PartialView("_TlumaczGoogle", model);
        }
        [Route("Jezyki")]
        public ActionResult Jezyki(bool listaRozwijana = true)
        {
            if (!SolexBllCalosc.PobierzInstancje.Konfiguracja.KlientMozeZmianiacJezyk)
            {
                return null;
            }

            IList<Jezyk> jezykii = SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykiWSystemie.Values.Where(x => !x.UkrytyDlaKlienta).ToList();

            if (jezykii.Count < 2)
            {
                return null;
            }
           
            string url = Request.RawUrl;
            return PartialView("_Jezyki", new ParametryDoJezykow(listaRozwijana, jezykii, url));
        }
    

      
    }
}