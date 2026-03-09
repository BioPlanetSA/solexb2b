using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Web.Site2.Helper.DaneListaAdmin;
using System.Reflection;
using SolEx.Hurt.Core.DostepDane;

namespace SolEx.Hurt.Web.Site2.Bindowania
{
    public class DaneEdycjaAdminBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            DaneEdycjaAdmin dane = base.BindModel(controllerContext, bindingContext) as DaneEdycjaAdmin;

            if (dane == null)
            {
                return null;
            }

            //foreach (OpisPolaObiektu pole in dane.PolaObiektu)
            //{
            //    if (pole.WymuszonyTypEdytora == TypEdytora.IndywidualizacjaCenaTablica)
            //    {
                 


            //    }

            //    if (pole.WymuszonyTypEdytora.HasValue && pole.WymuszonyTypEdytora == TypEdytora.EdytorTekstowy)
            //    {
            //        if (pole.Wartosc == null)
            //        {
            //            continue;
            //        }

            //        if (pole.Wartosc is string[])
            //        {
            //            string[] tablicaWartosci = pole.Wartosc as string[];
            //            if (tablicaWartosci.Any() && tablicaWartosci[0] == "")
            //            {
            //                tablicaWartosci[0] = null; //nullowanie
            //            }
            //        }
            //    }
            //}

            return dane;
        }
    }
}