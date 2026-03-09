using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SolEx.Hurt.Model;
using SolEx.Hurt.Web.Site2.AtrybutyMvc;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Controllers.API
{
    [RoutePrefix("api2/Jezyki")]
    [AutoryzacjaSolex]
    public class JezykiController : SolexAPIController
    {
        [HttpGet]
        [Route("")]
        public Dictionary<int, Jezyk> Jezyki()
        {
            return Core.BLL.Languages.JezykiWSystemie();
        }
    }
}
