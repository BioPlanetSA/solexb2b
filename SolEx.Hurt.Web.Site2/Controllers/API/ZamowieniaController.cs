using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Web.Site2.AtrybutyMvc;
using SolEx.Hurt.Web.Site2.Helper;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Controllers.API
{
    [RoutePrefix("api2/zamowienia")]
    [AutoryzacjaSolex]
    public class ZamowieniaController : SolexAPIController
    {
        [HttpGet]
        [Route("")]
        public List<ZamowienieSynchronizacja> Zamowienia()
        {
            return SolexBllCalosc.PobierzInstancje.ZamowieniaDostep.PobierzZamowieniaOczekujaceNaImportDoERP(SolexHelper.AktualnyKlient).ToList();
        }



    }
}
