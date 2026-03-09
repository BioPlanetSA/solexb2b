using System.Web.Mvc;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Web.Site2.Helper;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Controllers
{
    [RoutePrefix("SubkontaGrupy")]
    public class SubkontaGrupyController : SolexControler
    {
        [Route("Lista")]
        public PartialViewResult Lista()
        {
            if (SolexHelper.AktualnyKlient.KlientNadrzednyId.HasValue && !SolexHelper.AktualnyKlient.AdministratorSubkont)
            {
                return null;
            }
            return PartialView("Lista");
        }
        [Route("ListaDane")]
        public PartialViewResult ListaDane()
        {
            var limity = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<SubkontoGrupa>(SolexHelper.AktualnyKlient.KlientPodstawowy());
            return PartialView("ListaDane", limity);
        }

        [Route("Usun")]
        public void Usun(long id)
        {
            SolexBllCalosc.PobierzInstancje.DostepDane.UsunPojedynczy<SubkontoGrupa>(id);
        }

        [Route("Edycja")]
        public PartialViewResult Edycja(long? id = null)
        {
            ViewBag.SzablonyLimitow = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<SzablonLimitow>(SolexHelper.AktualnyKlient.KlientPodstawowy());
            ViewBag.SzablonyAkceptacji = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<SzablonAkceptacjiBll>(SolexHelper.AktualnyKlient.KlientPodstawowy());
            ViewBag.MiejsceKosztow = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<MiejsceKosztow>(SolexHelper.AktualnyKlient.KlientPodstawowy());
         
            SubkontoGrupa obiekt;
            if (id.HasValue)
            {
                obiekt = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<SubkontoGrupa>(id, SolexHelper.AktualnyKlient.KlientPodstawowy());
            }
            else
            {
                obiekt = new SubkontoGrupa();
                obiekt.KlientId = SolexHelper.AktualnyKlient.KlientPodstawowy().Id;
            }
       
            return PartialView("Edycja", obiekt);
        }

        [Route("Edycja")]
        [HttpPost]
        public void Edycja(SubkontoGrupa obiekt)
        {
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(obiekt);
            //var obiekt = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<SzablonLimitow>(id, SolexHelper.AktualnyKlient);
            //return PartialView("Edycja", obiekt);
        }
    }
}