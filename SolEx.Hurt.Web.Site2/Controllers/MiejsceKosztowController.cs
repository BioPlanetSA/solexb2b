using System.Web.Mvc;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Controllers
{
    [RoutePrefix("MiejsceKosztow")]
    public class MiejsceKosztowController : SolexControler
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
          
            var limity = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<MiejsceKosztow>(SolexHelper.AktualnyKlient.KlientPodstawowy());
            return PartialView("ListaDane", limity);
        }
        [Route("Usun")]
        public void Usun(long id)
        {
            SolexBllCalosc.PobierzInstancje.DostepDane.UsunPojedynczy<MiejsceKosztow>(id);
        }
        [Route("Edycja")]
        public PartialViewResult Edycja(long? id = null)
        {
            MiejsceKosztow obiekt;
            if (id.HasValue)
            {
                obiekt = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<MiejsceKosztow>(id, SolexHelper.AktualnyKlient.KlientPodstawowy());
            }
            else
            {
                obiekt = new MiejsceKosztow();
                obiekt.KlientId = SolexHelper.AktualnyKlient.KlientPodstawowy().Id;
            }
       
            return PartialView("Edycja", obiekt);
        }

        [Route("Edycja")]
        [HttpPost]
        public void Edycja(MiejsceKosztow obiekt)
        {
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(obiekt);
            //var obiekt = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<SzablonLimitow>(id, SolexHelper.AktualnyKlient);
            //return PartialView("Edycja", obiekt);
        }
    }
}