using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Web.Site2.Helper.DaneListaAdmin;
using System.Reflection;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Core.DostepDane.DaneObiektu;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Extensions;

namespace SolEx.Hurt.Web.Site2.Bindowania
{
    public class OpisPolaObiektuBinder : DefaultModelBinder
    {
        protected override void BindProperty(ControllerContext controllerContext,ModelBindingContext bindingContext,PropertyDescriptor propertyDescriptor)
        {
            if (propertyDescriptor.Name == "Wartosci")
            {
                string klucz = bindingContext.ModelName + "." + "Wartosci";

                ValueProviderResult typPrzechowywany =  bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".TypPrzechowywanejWartosci") ??
                                                        bindingContext.ValueProvider.GetValue(bindingContext.ModelName + "TypPrzechowywanejWartosci");
                Type typ = Type.GetType(typPrzechowywany.AttemptedValue);
                if (typ != null && typ.IsArray)
                {
                    Type typTablicy = typ.GetElementType();
                   
                    var tablica = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(typTablicy));

                    if (!typTablicy.IsPrimitive && typTablicy != typeof(string) && !typTablicy.IsEnum)
                    {

                        int i = 0;
                        while (true)
                        {
                            var obiekt = Activator.CreateInstance(typTablicy);
                            bool znaleziony = false;
                            foreach (var propertyInfo in typTablicy.GetProperties())
                            {
                                ValueProviderResult w = bindingContext.ValueProvider.GetValue(klucz + "[" + i + "]." + propertyInfo.Name);

                                if (w == null)
                                {
                                    continue;
                                }
                                znaleziony = true;
                                object result = null;

                                if ((w.AttemptedValue == null || string.IsNullOrEmpty(w.AttemptedValue)) && propertyInfo.PropertyType.IsNullableType())
                                {
                                    result = null;
                                }
                                else
                                {
                                    //musimy podmienic typ jesli jest nullable typ
                                    Type typJakiWyciagnac = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

                                    if (typJakiWyciagnac.IsEnum)
                                    {
                                        result = Enum.Parse(typJakiWyciagnac, w.AttemptedValue);
                                    }
                                    else
                                    {
                                        result = Convert.ChangeType(w.AttemptedValue, typJakiWyciagnac, CultureInfo.InvariantCulture);
                                    }
                                }

                                propertyInfo.SetValue(obiekt, result);
                            }
                            if (!znaleziony)
                            {
                                break;
                            }
                            tablica.Add(obiekt);
                            i++;
                        }
                        Array tab = Array.CreateInstance(typTablicy, tablica.Count);
                        tablica.CopyTo(tab, 0);

                        //koniec nie znaleziono wiecej elementow
                        propertyDescriptor.SetValue(bindingContext.Model, tab);

                    }

                }
            

            }
            else
            {
                //standardowy binder
                base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
            }
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            OpisPolaObiektuBaza dane = base.BindModel(controllerContext, bindingContext) as OpisPolaObiektuBaza;

            if (dane == null)
            {
                return null;
            }

            //if (dane.Wartosci != null)
            //{
            //    //IndywidualizacjaCena[] ceny = base.BindProperty(controllerContext, bindingContext, )
            //}

            //object o = Activator.CreateInstance(bindingContext.ModelType);
            //foreach (PropertyInfo p in bindingContext.ModelType.GetProperties())
            //{
            //    ValueProviderResult wartosc = bindingContext.ValueProvider.GetValue(p.Name);
            //}

            ////foreach (OpisPolaObiektu pole in dane.PolaObiektu)
            ////{
            ////    if (pole.WymuszonyTypEdytora.HasValue && pole.WymuszonyTypEdytora == TypEdytora.EdytorTekstowy)
            ////    {
            ////        if (pole.Wartosc == null)
            ////        {
            ////            continue;
            ////        }

            ////        if (pole.Wartosc is string[])
            ////        {
            ////            string[] tablicaWartosci = pole.Wartosc as string[];
            ////            if (tablicaWartosci.Any() && tablicaWartosci[0] == "")
            ////            {
            ////                tablicaWartosci[0] = null; //nullowanie
            ////            }
            ////        }
            ////    }
            ////}
            return dane;
        }
    }


  
}


public class ObjectArrayBinder : DefaultModelBinder
{
    public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    {
        object[] dane = base.BindModel(controllerContext, bindingContext) as object[];

        return dane;
    }
}