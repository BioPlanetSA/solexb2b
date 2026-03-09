using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ServiceStack.Common;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Controllers
{
    [RoutePrefix("Adresy")]
    public class AdresyController : SolexControler
    {
        public PartialViewResult Lista(bool dodawanienowegoadresu)
        {
            if (SolexHelper.AktualnyKlient.AdresWysylkiBlokada)
            {
                return null;
            }
            return PartialView("Lista", dodawanienowegoadresu);
        }

        [Route("ListaDane")]
        public PartialViewResult ListaDane()
        {
            List<Adres> limity = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Adres>(SolexHelper.AktualnyKlient).ToList();
            HashSet<long> ids = new HashSet<long>( limity.Select(x => (long)x.Id) );
            var laczniki = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<KlientAdres>(SolexHelper.AktualnyKlient,x=>!ids.Contains(x.AdresId)).Select(x=>x.AdresId).ToList();
            var dodatkowe = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Adres>(null,x => laczniki.Contains(x.Id));
            limity.AddRange(dodatkowe);
            return PartialView("ListaDane", limity);
        }

        [Route("PobierzRegiony")]
        public JsonResult PobierzRegiony(int kraj)
        {
            IList<Region> regiony = SolexBllCalosc.PobierzInstancje.RegionyDostep.PobierzRegionyKraju(kraj, SolexHelper.AktualnyJezyk.Id);
            return Json(regiony, JsonRequestBehavior.AllowGet);
        }
        
        [Route("edycja/{id}")]
        public PartialViewResult Edycja(long id)
        {
            Adres obiekt=null;
            obiekt = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Adres>(id, SolexHelper.AktualnyJezyk.Id);
            
            if (obiekt == null)
            {
                throw new Exception($"Nie znaleziono adres o Id: {id}");
            }
            ViewBag.Kraje = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Kraje>(SolexHelper.AktualnyJezyk.Id,SolexHelper.AktualnyKlient);
            ViewBag.Regiony = SolexBllCalosc.PobierzInstancje.RegionyDostep.PobierzRegionyKraju(obiekt.KrajId.GetValueOrDefault(), SolexHelper.AktualnyJezyk.Id);
            return PartialView("Edycja", obiekt);
        }
        [Route("nowy")]
        public PartialViewResult DodajAdres()
        {

            Adres obiekt;
            obiekt = new Adres();
            obiekt.AutorId = SolexHelper.AktualnyKlient.Id;

            ViewBag.Kraje = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Kraje>(SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient);
            ViewBag.Regiony = SolexBllCalosc.PobierzInstancje.RegionyDostep.PobierzRegionyKraju(obiekt.KrajId.GetValueOrDefault(), SolexHelper.AktualnyJezyk.Id);
            return PartialView("Edycja", obiekt);
        }
        /// <summary>
        /// Dodawanie dowego adresu
        /// </summary>
        /// <param name="obiekt"></param>
        /// <param name="czyZKoszyka">określa czy request był z koszyka</param>
        /// <returns></returns>
        [HttpPost]
        [Route("zapisz")]
        public long Zapisz(Adres obiekt, bool czyZKoszyka=false)
        {
            if (obiekt.Id == 0)
            {
                obiekt.Id = obiekt.WygenerujIDObiektuSHAWersjaLong(-1);
                obiekt.DataDodania = DateTime.Now;
            }
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(obiekt);
            if (czyZKoszyka)
            {
                var k = SolexHelper.AktualnyKoszyk;
                k.AdresId = obiekt.Id;
                SolexBllCalosc.PobierzInstancje.Koszyk.UaktualnijKoszyk(k, false);
            }
            return obiekt.Id;
            //var obiekt = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<SzablonLimitow>(id, SolexHelper.AktualnyKlient);
            //return PartialView("Edycja", obiekt);
        }

        [Route("usun/{id}")]
        public PartialViewResult Usun(long id)
        {
            if (id == 0) return null;
            SolexBllCalosc.PobierzInstancje.DostepDane.UsunPojedynczy<Adres>(id);
            
            return ListaDane();
        }
    }
}