using System;
using System.Web.Mvc;
using log4net;

namespace SolEx.Hurt.Web.Site2.Bindowania
{
    public class TypeBinder : DefaultModelBinder
    {
        protected ILog Log
        {
            get
            {
                return LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            }
        }
        /// <summary>
        /// Metoda mapuj¹ca pola z formularza na obiekt
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="bindingContext"></param>
        /// <returns></returns>
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == null)
            {
                return null;
            }
            return Type.GetType(valueProviderResult.AttemptedValue);
        }
    }
}