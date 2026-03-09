using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Routing;

namespace SolEx.Hurt.Web.Site2
{
    public class CultureConstraint : IRouteConstraint
    {
        private readonly string defaultCulture;
        private readonly HashSet<string> dozwoloneOpcje;

        public CultureConstraint(string defaultCulture, HashSet<string> dozwoloneOpcje)
        {
            this.defaultCulture = defaultCulture;
            this.dozwoloneOpcje = dozwoloneOpcje;
        }

        public bool Match(
            HttpContextBase httpContext,
            Route route,
            string parameterName,
            RouteValueDictionary values,
            RouteDirection routeDirection)
        {
            if (routeDirection != RouteDirection.UrlGeneration || !this.defaultCulture.Equals(values[parameterName]))
            {
                string wartosc = values[parameterName].ToString();
                return dozwoloneOpcje.Contains(wartosc);
            }
            return true;
        }
    }
}