using SolEx.Hurt.Web.Site2.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Helper
{
    public class MVCHelper
    {
        //private log4net.ILog Log
        //{
        //    get
        //    {
        //        return log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //    }
        //}

        private static readonly MVCHelper _instancja = new MVCHelper();

        public static MVCHelper PobierzInstancje
        {
            get { return _instancja; }
        }

        //public string PobierzWynikAkcji(string akcja, string kontroler, RouteValueDictionary pars)
        //{
        //    try
        //    {
        //        return FakeHtml.Action(akcja, kontroler, pars).ToHtmlString();
        //    }
        //    catch (Exception e)
        //    {
        //        if (e.InnerException != null)
        //        {
        //            Log.Error(e.InnerException);
        //        }
        //        else
        //        {
        //            Log.Error(e);
        //        }
        //        return string.Format("Bład kontroler {0} akcja {1}", kontroler, akcja, e.Message);
        //    }
        //}

        //public string PobierzWynikAkcji(string akcja, string kontroler)
        //{
        //    try
        //    {
        //        return FakeHtml.Action(akcja, kontroler).ToHtmlString();
        //    }
        //    catch (Exception e)
        //    {
        //        if (e.InnerException != null)
        //        {
        //            Log.Error(e.InnerException);
        //        }
        //        else
        //        {
        //            Log.Error(e);
        //        }
        //        return string.Format("Bład kontroler {0} akcja {1}", kontroler, akcja, e.Message);
        //    }
        //}
        
        //public HtmlHelper FakeHtml
        //{
        //    get
        //    {
        //        if (HttpContext.Current != null)
        //        {
        //            if (HttpContext.Current.Items["htmlhelper"] == null)
        //            {
        //                HtmlHelper helper = CreateHelper();
        //                HttpContext.Current.Items.Add("htmlhelper", helper);
        //                return helper;
        //            }
        //            return (HtmlHelper)HttpContext.Current.Items["htmlhelper"];
        //        }
        //        throw new Exception("Nie można stworzyć obiektu HTML w aplikacji nie webowej");
        //    }
        //}

        /// <summary>
        /// Creates an instance of an MVC controller from scratch
        /// when no existing ControllerContext is present
        /// </summary>
        /// <typeparam name="T">Type of the controller to create</typeparam>
        /// <returns></returns>
        //public T CreateController<T>(RouteData routeData = null)
        //            where T : Controller, new()
        //{
        //    T controller = new T();

        //    // Create an MVC Controller Context
        //    HttpContextBase wrapper = null;
        //    if (HttpContext.Current != null)
        //        wrapper = new HttpContextWrapper(System.Web.HttpContext.Current);
        //    //else
        //    //    wrapper = CreateHttpContextBase(writer);

        //    if (routeData == null)
        //        routeData = new RouteData();

        //    if (!routeData.Values.ContainsKey("controller") && !routeData.Values.ContainsKey("Controller"))
        //        routeData.Values.Add("controller", controller.GetType().Name
        //                                                    .ToLower()
        //                                                    .Replace("controller", ""));

        //    controller.ControllerContext = new ControllerContext(wrapper, routeData, controller);
        //    return controller;
        //}
        
        public SolexControler CreateController(SolexControler controller, RouteData routeData = null)
        {
            // Create an MVC Controller Context
            HttpContextBase wrapper = null;
            if (HttpContext.Current != null)
            {
                wrapper = new HttpContextWrapper(System.Web.HttpContext.Current);
            }
            //else
            //    wrapper = GetContextFake();

            if (routeData == null)
                routeData = new RouteData();

            if (!routeData.Values.ContainsKey("controller") && !routeData.Values.ContainsKey("Controller"))
                routeData.Values.Add("controller", controller.GetType().Name
                                                            .ToLower()
                                                            .Replace("controller", ""));

            controller.ControllerContext = new ControllerContext(wrapper, routeData, controller);
            controller.SolexHelper = SolexHelper.PobierzInstancjeZCache();
            return controller;
        }

        //public HtmlHelper CreateHelper()
        //{
        //    SolexControler c = (SolexControler)Activator.CreateInstance(typeof(DokumentyController));
        //    c = CreateController(c);
        //    HtmlHelper helper = new HtmlHelper(new ViewContext(c.ControllerContext, new WebFormView(c.ControllerContext, "Index"), new ViewDataDictionary(), new TempDataDictionary(), new StringWriter()), new ViewPage());
        //    return helper;
        //}
    }

    public static class ControllerExtensions
    {
        public static string PartialViewToString(this Controller controller, string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                viewName = controller.ControllerContext.RouteData.GetRequiredString("action");
            }

            controller.ViewData.Model = model;

            using (StringWriter stringWriter = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, stringWriter);
                viewResult.View.Render(viewContext, stringWriter);
                return stringWriter.GetStringBuilder().ToString();
            }
        }
        public static string ViewToString(this Controller controller, string viewName, string masterView , object model)
        {
            controller.ViewData.Model = model;
            using (var sw = new System.IO.StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindView(controller.ControllerContext, viewName, masterView);
                ViewContext viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(controller.ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

    }

}