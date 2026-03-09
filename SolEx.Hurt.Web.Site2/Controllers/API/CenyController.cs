using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Web.Site2.AtrybutyMvc;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Controllers.API
{
    [RoutePrefix("api2/ceny")]
    [AutoryzacjaSolex]
    public class CenyController : SolexAPIController
    {
        [HttpPost]
        [Route("CenyPoziomy")]
        [ObslugaStronnicowania]
        public List<CenaPoziomu> CenyPoziomy(HashSet<long> ids)
        {
            return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<CenaPoziomu>(SolexHelper.AktualnyKlient, x => ids == null || Sql.In(x.Id, ids), null, Paczka, IloscElementow).ToList();
        }

        [HttpPost]
        [Route("AktualizujCenyPoziomy")]
        public bool AktualizujCenyPoziomy(List<CenaPoziomu> listaCenPoziomow)
        {
            try
            {
                SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<CenaPoziomu>(listaCenPoziomow);
            }
            catch (Exception e)
            {
                SolexBllCalosc.PobierzInstancje.Log.Error($"Błąd aktualizacji CenyPoziomu przez API: {e.Message}");
                throw new Exception("Błąd aktualizacji CenyPoziomu przez API - sprawdz logi na serwerze.");
            }
            return true;
        }


        [HttpDelete]
        [Route("UsunCenyPoziomy")]
        public bool UsunCenyPoziomy(List<long> listaCenPoziomow)
        {
            SolexBllCalosc.PobierzInstancje.DostepDane.Usun<CenaPoziomu, long>(listaCenPoziomow);
            return true;
        }
    }
}
