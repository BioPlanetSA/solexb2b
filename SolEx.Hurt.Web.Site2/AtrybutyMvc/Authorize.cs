
using System.Web.Mvc;
using System.Web.Routing;
using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.AtrybutyMvc
{
    public class Authorize : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            SolexHelper solexHelper = SolexHelper.PobierzInstancjeZCache();
            if (solexHelper.AktualnyKlient.Id == 0)
            {
                UrlHelper url = new UrlHelper(filterContext.RequestContext);
                string adres = url.LinkLogowania(filterContext.HttpContext.Request.Url.AbsolutePath);

                filterContext.Result = new RedirectResult(adres);
            }

        }





        //public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        //{
        //    SolexHelper solexHelper = SolexHelper.PobierzInstancjeZCache();
        //    if (solexHelper.AktualnyKlient.Id == 0)
        //    {
        //        UrlHelper url = new UrlHelper(HttpContext.Current.Request.RequestContext);
        //        string adres = url.LinkLogowania(actionContext.Request.RequestUri.AbsolutePath);

        //        actionContext.re.Result = new RedirectResult(adres);

        //        base.OnActionExecuting(filterContext);
        //        //UrlExtender helper = filterContext.RequestContext.Url
        //        ////Dodajemy parametry do query stringa zeby wiadomo było ze request przyszedl po zalogowaniu (z return urla)
        //        //string adres = filterContext.Request.RequestUri.AbsolutePath + "?pozalogowaniu=true";
        //        //filterContext.RequestContext.r.Redirect(helper.LinkLogowania(adres), true);
        //        ////return RedirectToAction("logowanie", "logowanie",new RouteValueDictionary() { { "ReturnUrl", Request.Url.AbsolutePath} });
        //    }
        //}

        //public override void OnActionExecuted(ActionExecutedContext filterContext)
        //{
        //    if (HttpContext.Current.Request.QueryString.GetValues("pozalogowaniu") != null)
        //    {

        //        //UrlHelper url = new UrlHelper(HttpContext.Current.Request.RequestContext);
        //        //filterContext.Result = new RedirectResult(url.Action("StronaGlowna", "Tresci"));

        //    }
               
        //}
    }
}