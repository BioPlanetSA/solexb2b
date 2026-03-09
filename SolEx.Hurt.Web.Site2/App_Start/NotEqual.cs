using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace SolEx.Hurt.Web.Site2
{
    public class NotEqual : IRouteConstraint
    {
        private readonly List<string> _matches;

        public NotEqual(string matches)
        {
            _matches = matches.ToLower().Split(',').ToList();
        }

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            return !_matches.Contains(values[parameterName].ToString().ToLower());
        }
    }
}