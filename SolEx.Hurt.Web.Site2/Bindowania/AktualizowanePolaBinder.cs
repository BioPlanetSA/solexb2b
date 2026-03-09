using System;
using System.Reflection;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Web.Site2.Models;
using IModelBinder = System.Web.Mvc.IModelBinder;
using ModelBindingContext = System.Web.Mvc.ModelBindingContext;

namespace SolEx.Hurt.Web.Site2.Bindowania
{
    /// <summary>
    /// Maper, przechowujący info jakie pola były edyowane w formularzu
    /// </summary>
    public class AktualizowanePolaBinder : IModelBinder
    {
        protected log4net.ILog Log
        {
            get
            {
                return log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            }
        }
        /// <summary>
        /// Metoda mapująca listę pól z formularza na obiekt
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="bindingContext"></param>
        /// <returns></returns>
        public object BindModel(System.Web.Mvc.ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            MapowanePola mp=new MapowanePola();
     
            var request = controllerContext.HttpContext.Request;
            foreach (string v in request.Form.Keys)
            {
                mp.DodajPole(v);
            }
            return mp;
        }
    }
}