using System.Collections.Generic;
using System.Web.Mvc;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Controllers
{
    [RoutePrefix("Komunikaty")]
    public class KomunikatyController : SolexControler
    {
        [HttpPost]
        [Route("OdpowiedzKomunikat")]
        public ActionResult OdpowiedzKomunikat(Komunikaty kom, OdpowiedzKlienta odpowiedz)
        {
            string klucz = Tools.PobierzInstancje.GetMd5Hash(kom.Id + "||" + SolexHelper.AktualnyKlient.Id + "||" + kom.OdKiedy.Value.ToShortDateString());

            if (kom.Klucz == klucz)
            {
                DzialaniaUzytkownikow ak = new DzialaniaUzytkownikow(ZdarzenieGlowne.WyswietlenieKomunikatu, null);
                ak.Parametry = new Dictionary<string, string> {{"idKomunikatu", kom.Id.ToString()}};
                if (kom.Przycisk == PrzyciskiDlaKlienta.PrzyciskOd1Do6)
                {
                    ak.Parametry.Add("OdpowiedzKlienta", ((int) odpowiedz).ToString());
                }
                else
                {
                    ak.Parametry.Add("OdpowiedzKlienta", odpowiedz.ToString());
                }
                SolexBllCalosc.PobierzInstancje.Statystyki.DodajZdarzenie(ak, SolexHelper.AktualnyKlient);
            }
            string adres = Request.UrlReferrer.AbsoluteUri;
            return Redirect(adres);
        }
    }
}