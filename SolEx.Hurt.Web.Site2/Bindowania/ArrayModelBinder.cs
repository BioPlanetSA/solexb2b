using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SolEx.Hurt.Web.Site2.Bindowania
{
    /// <summary>
    /// Binder do odczytu tablic podawanych rozdzielone przecinkiem
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ArrayModelBinder<T>: IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (value == null || string.IsNullOrEmpty(value.AttemptedValue))
            {
                return null;
            }

            //jest jakas wartosc - bindujemy

            var rozbiteStringi =  value.AttemptedValue.Split(new []{','},StringSplitOptions.RemoveEmptyEntries);

            if ( typeof(T) == typeof(int))
            {
                return rozbiteStringi.Select(int.Parse).ToArray();
            }

            if (typeof(T) == typeof(long))
            {
                return rozbiteStringi.Select(long.Parse).ToArray();
            }

            return null;
        }

    }


}