using System;
using System.Web.Mvc;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Web.Site2.Bindowania
{
    public class SortowanieBinder : IModelBinder
    {
        /// <summary>
        /// Metoda mapująca pola z formularza na obiekt
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="bindingContext"></param>
        /// <returns></returns>
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == null)
            {
                return null;
            }
            if (string.IsNullOrEmpty(valueProviderResult.AttemptedValue))
            {
                return null;
            }

            var sort = valueProviderResult.AttemptedValue;
            Sortowanie sortowanie = new Sortowanie();
            foreach (var s in sort.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries))
            {
                var sortOpcja=  s.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
                SortowaniePole sp=new SortowaniePole();
                sp.Pole = sortOpcja[0];
                if (sortOpcja.Length > 1)
                {
                    sp.KolejnoscSortowania = (KolejnoscSortowania)Enum.Parse(typeof(KolejnoscSortowania), sortOpcja[1]);
                }
                sortowanie.Pola.Add(sp);
            }

            return sortowanie;

        }
    }
}