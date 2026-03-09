using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using log4net;
using ServiceStack.Common;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.Extensions;
using SolEx.Hurt.Model.Helpers;
using ModelBindingContext = System.Web.Mvc.ModelBindingContext;

namespace SolEx.Hurt.Web.Site2.Bindowania
{
    /// <summary>
    /// Uniwersalny mapper, wypełniający propertisy obiektu na podstawie name w formularzu
    /// </summary>
    public class DecimalBinder : DefaultModelBinder
    {
        protected   ILog Log
        {
            get
            {
                return LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
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
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == null)
            {
                return base.BindModel(controllerContext, bindingContext);
            }
            decimal wynik;
            TextHelper.PobierzInstancje.SprobojSparsowac(valueProviderResult.AttemptedValue, out wynik);
            return wynik;
        }
    }
}