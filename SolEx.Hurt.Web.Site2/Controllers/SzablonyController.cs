using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ServiceStack.ServiceHost;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Controllers
{
    [RoutePrefix("Szablony")]
    public class SzablonyController : SolexControler
    {
        // GET: Szablony
        [System.Web.Mvc.Route("Pobierz")]
        public JsonResult Pobierz()
        {
            IList<SzablonyEdytorow> items = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<SzablonyEdytorow>(null);
            return Json(items.Select(x => new {title = x.Nazwa, description = x.Opis, content = x.Tresc}), JsonRequestBehavior.AllowGet);
        }
    }   
}