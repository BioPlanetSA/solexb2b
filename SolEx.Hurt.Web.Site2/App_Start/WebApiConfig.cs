using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using SolEx.Hurt.Web.Site2.AtrybutyMvc;
using SolEx.Hurt.Web.Site2.Bindowania;

namespace SolEx.Hurt.Web.Site2
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

           

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api2/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );


            config.Routes.MapHttpRoute(
                name: "ActionApi",
                routeTemplate: "api2/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.Filters.Add(new StronicowanieObsluga());

            config.ParameterBindingRules.Insert(0, GetCustomParameterBinding);

            //GlobalFilters.Filters.Add(new StronicowanieObsluga());
        }
        public static HttpParameterBinding GetCustomParameterBinding(HttpParameterDescriptor descriptor)
        {
            if (descriptor.ParameterType == typeof(long[]) || descriptor.ParameterType == typeof(int[]))
            {
                return new ArrayNumberBinder(descriptor);
            }
            // any other types, let the default parameter binding handle
            return null;
        }
    }
}
