using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using ServiceStack.Common;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Web.Site2.AtrybutyMvc;
using SolEx.Hurt.Web.Site2.Controllers;
using StackExchange.Profiling.Mvc;

namespace SolEx.Hurt.Web.Site2
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes, ISolexBllCalosc calosc = null)
        {
            if (calosc == null)
            {
                calosc = SolexBllCalosc.PobierzInstancje;
            }
            HashSet<string> slownikJezykow = new HashSet<string>( calosc.Konfiguracja.JezykiWSystemie.Select(x => x.Value.Symbol) );
            
            string domyslnyJezykSymbol = calosc.Konfiguracja.JezykiWSystemie[calosc.Konfiguracja.JezykIDDomyslny].Symbol;

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{resource}.ashx/{*pathInfo}");
            routes.IgnoreRoute("api/{resource}.ashx/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new {favicon = @"(.*/)?favicon.ico(/.*)?"});

            routes.MapRoute("PodanySamJezykStronaGlowna", "{culture}", new { culture = UrlParameter.Optional, controller = "Tresci", action = "StronaGlowna"}, new { culture = new CultureConstraint( domyslnyJezykSymbol, slownikJezykow) } );

            routes.MapRoute("wylogowanie_logout", "logout", new { culture = UrlParameter.Optional, controller = "Logowanie", action = "Wylogowanie" });
            routes.MapRoute("wylogowanieZJezykiem", "{culture}/logout", new { culture = UrlParameter.Optional, controller = "Logowanie", action = "Wylogowanie" } );

            if (slownikJezykow.Count > 1)
            {
                RouteTable.Routes.MapLocalizedMvcAttributeRoutes(
                    urlPrefix: "{culture}/",
                    defaults: new { culture = domyslnyJezykSymbol },
                    constraints: new { culture = new CultureConstraint(domyslnyJezykSymbol, slownikJezykow) }
                    );
            }
            else
            {
                //~~~~~~~~~~~~~~~~
                //------ Musi być tak dziwnie dlatego ze dla testow mokujemy mvc i wtedy wywala sie, dlatgo mamy w catchu swoja implemetnacje wyszukiwania wg innego assembyl
                //~~~~~~~~~~~~~~~~~~
                try
                {
                    routes.MapMvcAttributeRoutes();
                }
                catch (Exception )
                {
                    RouteCollectionExtensions.MapMvcAttributeRoutes(routes, typeof(AdminController).Assembly);
                }
              
            }

            //dodawanie ostatnich routingow domyslnych

            if (slownikJezykow.Count > 1)
            {
                //dla wielojezyczonosci
                routes.MapRoute("stronaJakNicNieTrafia-modal", "{culture}/{symbol}/{modal:regex(m)}",
               new
               {
                   culture = domyslnyJezykSymbol,
                   controller = "Tresci",
                   action = "StronaSymbol",
                   id = UrlParameter.Optional
               }, new { culture = new CultureConstraint(domyslnyJezykSymbol, slownikJezykow) });


                routes.MapRoute("stronaJakNicNieTrafia-bezModala", "{culture}/{symbol}",
                   new
                   {
                       culture = domyslnyJezykSymbol,
                       controller = "Tresci",
                       action = "StronaSymbol",
                       id = UrlParameter.Optional
                   }, new { culture = new CultureConstraint(domyslnyJezykSymbol, slownikJezykow) });

                routes.MapRoute("Default-PelnaSciezka", "{culture}/{controller}/{action}/{id}",
                new
                {
                    culture = domyslnyJezykSymbol,
                    controller = "Tresci",
                    action = "StronaGlowna",
                    id = UrlParameter.Optional
                },
                new { culture = new CultureConstraint(domyslnyJezykSymbol, slownikJezykow) });
            }
        //    else {
                //dla jezyka jednego
                routes.MapRoute("stronaJakNicNieTrafia-brakJezyka", "{symbol}/{modal:regex(m)}",
                   new
                   {
                       culture = domyslnyJezykSymbol,
                       controller = "Tresci",
                       action = "StronaSymbol",
                       id = UrlParameter.Optional
                   });

                routes.MapRoute("stronaJakNicNieTrafia2-brakJezyka", "{symbol}",
                   new
                   {
                       culture = domyslnyJezykSymbol,
                       controller = "Tresci",
                       action = "StronaSymbol",
                       id = UrlParameter.Optional
                   });

                routes.MapRoute("Default-PelnaSciezkaBezJezyka-brakJezyka", "{controller}/{action}/{id}",
                  new
                  {
                      culture = domyslnyJezykSymbol,
                      controller = "Tresci",
                      action = "StronaGlowna",
                      id = UrlParameter.Optional
                  });
         //   }

            //routes.MapRoute("adminBezKultury-ajaxyitp", "admin/{action}/{id}", new { culture = domyslnyJezykSymbol, controller = "Admin", id = UrlParameter.Optional });
            //routes.MapRoute("SlownikDoAdminBezKultury-ajaxyitp", "Slownik/{action}", new { culture = domyslnyJezykSymbol, controller = "Slownik" });


            GlobalFilters.Filters.Add(new InjectPageMetadataAttribute());

#if DEBUG
            //miniprofiler zeby liczyl czas uruchamiania akcji mvc - tylko w debugu
            GlobalFilters.Filters.Add(new ProfilingActionFilter());
#endif
        }
    }
}

