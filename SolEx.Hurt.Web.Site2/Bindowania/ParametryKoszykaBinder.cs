using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using log4net;
using ServiceStack.Text;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Web;
using SolEx.Hurt.Web.Site2.Models;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Bindowania
{
    public class ParametryKoszykaBinder : DefaultModelBinder
    {
        protected ILog Log
        {
            get
            {
                return LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            }
        }


        /// <summary>
        /// Metoda mapująca pola z formularza na obiekt
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="bindingContext"></param>
        /// <returns></returns>
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            ParametryKoszyka parametry = (ParametryKoszyka)controllerContext.RouteData.Values[bindingContext.ModelName];

            //jesli juz jest w kolekcji to go wyciagamy jak nie to budujemy
            if (parametry != null) return parametry;
            parametry = new ParametryKoszyka();
            
            return parametry;
        }
    }
}