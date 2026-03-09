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
    [RoutePrefix("api2/KategorieProduktu")]
    [AutoryzacjaSolex]
    public class KategorieProduktuController : SolexAPIController
    {
        [HttpGet]
        [Route("")]
        public List<KategorieBLL> Kategorie()
        {
            return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<KategorieBLL>(null).ToList();
        }
    }
}
