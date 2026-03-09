using System;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.Extensions;
using IModelBinder = System.Web.Mvc.IModelBinder;
using ModelBindingContext = System.Web.Mvc.ModelBindingContext;

namespace SolEx.Hurt.Web.Site2.Bindowania
{
    /// <summary>
    /// Uniwersalny mapper, wypełniający propertisy obiektu na podstawie name w formularzu
    /// </summary>
    public class UniwersalnyBinder : IModelBinder
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
        public object BindModel(System.Web.Mvc.ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var request = controllerContext.HttpContext.Request;

            object o = Activator.CreateInstance(bindingContext.ModelType);
            PropertyInfo[] propertisy = bindingContext.ModelType.GetProperties();
            foreach (var p in propertisy)
            {
                string wartosc = request.Form[p.Name];
                if (request.Files[p.Name] != null && request.Files[p.Name].ContentLength > 0)
                {
                    wartosc = request.Files[p.Name].FileName;
                    //string katalog = PlikiDostep.PobierzInstancje.PobierzSciezkePlikUsera(bindingContext.ModelType,)
                    //string sciezka_Kat = PlikiDostep.PobierzInstancje.Podkatalog();
                    //string katalog = PlikiDostep.PobierzInstancje.KatalogPlikow(sciezka_Kat);
                    //wartosc = Path.Combine(sciezka_Kat, request.Files[p.Name].FileName);
                    //var sciezka = Path.Combine(katalog, request.Files[p.Name].FileName);
                    //request.Files[p.Name].SaveAs(sciezka);
                }
                try
                {
                    if (string.IsNullOrEmpty(wartosc))
                    {
                        continue;
                    }

                    if (p.PropertyType == typeof(bool) || p.PropertyType == typeof(bool?))
                    {
                        string[] potencjalne = wartosc.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
                        if (potencjalne.Any())
                        {
                            p.SetValueExt(o, potencjalne[0]);
                        }
                    }
                    else
                    {
                        p.SetValueExt(o,wartosc);
                    }
                   
                }
                catch (Exception ex)
                {
                    Log.Error(
                        new Exception(
                            string.Format("Nie udało się ustawić pola {0} na wartość {1} z powodu błędu.", p.Name,wartosc), ex));
                }
            }
            return o;
        }
    }
}