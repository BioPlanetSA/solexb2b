using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Web.Site2.AtrybutyMvc;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Controllers.API
{
    [RoutePrefix("api2/atrybuty")]
    [AutoryzacjaSolex]
    public class AtrybutyController : SolexAPIController
    {
        [HttpGet]
        [Route("")]
        public List<Atrybut> Atrybuty()
        {
            return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Atrybut>(null).ToList();
        }

        [HttpPost]
        [Route("aktualizuj")]
        public bool AktualizujAtrybuty(IList<AtrybutBll> listaAtrybutow)
        {
            try
            {
                SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<AtrybutBll>(listaAtrybutow);
            } catch (Exception e)
            {
                SolexBllCalosc.PobierzInstancje.Log.Error($"Błąd aktualizacji atrybutów przez API: {e.Message} - atrybutów do dodania: {listaAtrybutow.Count}");
                throw new Exception("Błąd aktualizacji atrybutów przez API - sprawdz logi na serwerze.");
            }
            return true;
        }

        [HttpDelete]
        [Route("usun")]
        public bool UsunAtrybuty(List<int> listaAtrybutow)
        {
            SolexBllCalosc.PobierzInstancje.DostepDane.Usun<AtrybutBll, int>(listaAtrybutow);
            return true;
        }
    }
}
