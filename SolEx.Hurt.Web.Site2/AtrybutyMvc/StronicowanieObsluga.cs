using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using SolEx.Hurt.Web.Site2.Controllers.API;
using System.Web.Mvc;
using SolEx.Hurt.Helpers;
using System.Net;
using System.Web.Http;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Web.Site2.AtrybutyMvc
{
    public class StronicowanieObsluga : System.Web.Http.Filters.ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            var posiadaAtrybutStronicowania = filterContext.ActionDescriptor.GetCustomAttributes<ObslugaStronnicowaniaAttribute>().Count>0;
            if (!posiadaAtrybutStronicowania)
            {
                if (filterContext.Request.Headers.FirstOrDefault(x => x.Key == "PageSize").Value != null)
                {
                    HttpResponseMessage m = new HttpResponseMessage(HttpStatusCode.BadRequest);
                    m.Content = new StringContent(string.Format("Próba wywaołania akcji: {0} w kontrolerze: {1}, ze stronicowaniem mimo iż nie jest obsługiwane.", filterContext.ActionDescriptor.ActionName, filterContext.ControllerContext.ControllerDescriptor.ControllerName));
                    throw new HttpResponseException(m);
                }
            }
            
            base.OnActionExecuting(filterContext);
        }
    }
}