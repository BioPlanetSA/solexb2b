using System.Web.Mvc;
using ServiceStack.ServiceHost;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Web.Site2.Helper;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Controllers
{
    [RoutePrefix("SzablonyLimitow")]
    public class SzablonyLimitowController : SolexControler
    {
        [System.Web.Mvc.Route("Lista")]
        public PartialViewResult Lista()
        {
            if (SolexHelper.AktualnyKlient.KlientNadrzednyId.HasValue && !SolexHelper.AktualnyKlient.AdministratorSubkont)
            {
                return null;
            }
            return PartialView("Lista");
        }
        [System.Web.Mvc.Route("ListaDane")]
        public PartialViewResult ListaDane()
        {
            var limity = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<SzablonLimitow>(SolexHelper.AktualnyKlient.KlientPodstawowy());
            return PartialView("ListaDane",limity);
        }
        [System.Web.Mvc.Route("Usun/{id}")]
        public void Usun(long id)
        {
            SolexBllCalosc.PobierzInstancje.DostepDane.UsunPojedynczy<SzablonLimitow>(id);
        }
        [System.Web.Mvc.Route("Edycja")]
        [HttpGet]
        public PartialViewResult Edycja(int? id=null)
        {

            SzablonLimitow obiekt;
            if (id.HasValue)
            {
                obiekt = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<SzablonLimitow>(id, SolexHelper.AktualnyKlient.KlientPodstawowy());
            }
            else
            {
                obiekt=new SzablonLimitow();
                obiekt.Tworca = SolexHelper.AktualnyKlient.KlientPodstawowy().Id;
            }
            return PartialView("Edycja", obiekt);
        }
        [HttpPost]
        [System.Web.Mvc.Route("Edycja")]
        public void Edycja(SzablonLimitow obiekt)
        {
           
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(obiekt);
            //var obiekt = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<SzablonLimitow>(id, SolexHelper.AktualnyKlient);
            //return PartialView("Edycja", obiekt);
        }
    }
}