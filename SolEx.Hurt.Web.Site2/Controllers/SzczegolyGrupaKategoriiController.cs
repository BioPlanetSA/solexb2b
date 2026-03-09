using System.Web.Mvc;
using ServiceStack.ServiceHost;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Controllers
{
    [RoutePrefix("SzczegolyGrupaKategorii")]
    public class SzczegolyGrupaKategoriiController : SolexControler
    {
        /// <summary>
        /// </summary>
        /// <param name="identyfikatorobiektu">IDentyfikuje aktualną grupę do pokazania</param>
        /// <param name="pole"></param>
        /// <param name="naglowek"></param>
        /// <param name="preset"></param>
        /// <param name="opakowanie"></param>
        /// <param name="stopka"></param>
        /// <returns></returns>
        [System.Web.Mvc.Route("PoleGrupy")]
        public PartialViewResult PoleGrupy(int identyfikatorobiektu, string pole, string naglowek, string preset, string opakowanie, string stopka)
        {
            var wpis = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<GrupaBLL>(identyfikatorobiektu);
            return StworzPolePojedynczegoWpisu(wpis, pole, naglowek, preset, opakowanie, wpis.Nazwa, "","", stopka);
        }

    }
}