using System;
using System.Web.Mvc;

namespace SolEx.Hurt.Web.Site2.Bindowania
{
    public class FiltrListaProduktowKolekcjaBinder : IModelBinder
    {
        /// <summary>
        /// Metoda mapująca pola z formularza na obiekt
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="bindingContext"></param>
        /// <returns></returns>
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            return null;
            throw new Exception("jednak by potrzebny ten binder");
        }
    }
}