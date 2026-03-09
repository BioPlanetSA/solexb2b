using System.Web.Mvc;
using SolEx.Hurt.Core.BLL;

namespace SolEx.Hurt.Web.Site2.AtrybutyMvc
{
    public class DoklejanieDanychSolex:ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            int jezykId = SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykiWSystemieSlownikPoSymbolu[filterContext.RouteData.Values["culture"].ToString()].Id;
            filterContext.HttpContext.Response.Filter = new StopkaFiltr(filterContext.HttpContext.Response.Filter, jezykId);
            base.OnActionExecuting(filterContext);
        }
    }
}