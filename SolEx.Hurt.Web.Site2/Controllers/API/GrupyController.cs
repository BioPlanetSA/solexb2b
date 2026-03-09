using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Web.Site2.AtrybutyMvc;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Controllers.API
{
    [RoutePrefix("api2/grupy")]
    [AutoryzacjaSolex]
    public class GrupyController : SolexAPIController
    {
        [HttpGet]
        [Route("")]
        public List<GrupaBLL> Grupy()
        {
            return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<GrupaBLL>(null).ToList();
        }
    }
}
