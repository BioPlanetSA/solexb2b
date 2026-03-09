using System.Web.Mvc;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Controllers
{
    [RoutePrefix("Error")]
   
    public class ErrorController : SolexControler
    {
          [Route("Error/{kod:int}")]
        public PartialViewResult Error(int kod)
          {
            return PartialView("_Error", kod);
          }

      
    }
}