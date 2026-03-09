using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Web.Site2.AtrybutyMvc;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Controllers.API
{
    [RoutePrefix("api2/ProduktyStany")]
    [AutoryzacjaSolex]
    public class ProduktStanyController : SolexAPIController
    {



        [HttpDelete]
        [Route("usun")]
        public bool UsunProduktyStany(List<long> listaStanow)
        {
            SolexBllCalosc.PobierzInstancje.DostepDane.Usun<ProduktStan, long>(listaStanow);
            return true;
        }
    }
}
