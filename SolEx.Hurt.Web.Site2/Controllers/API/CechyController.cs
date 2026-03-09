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
    [RoutePrefix("api2/cechy")]
    [AutoryzacjaSolex]
    public class CechyController : SolexAPIController
    {
        [HttpPost]
        [Route("")]
        [ObslugaStronnicowania]
        public List<Cecha> Cechy(HashSet<long>idCech)
        {
            SortowanieKryteria<Cecha> sortowanie = new SortowanieKryteria<Cecha>(x => x.Id, KolejnoscSortowania.asc, "Id");
            return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Cecha>(SolexHelper.AktualnyKlient, x=> x.Id>0 && (idCech == null || Sql.In(x.Id, idCech)), new List<SortowanieKryteria<Cecha>> { sortowanie }, Paczka, IloscElementow).ToList();
        }

        [HttpPost]
        [Route("cechyZatrybutem")]
        [ObslugaStronnicowania]
        public List<Cecha> CechyZAtrybutem(HashSet<int> idAtrybutow)
        {
            SortowanieKryteria<Cecha> sortowanie = new SortowanieKryteria<Cecha>(x => x.Id, KolejnoscSortowania.asc, "Id");
            return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Cecha>(SolexHelper.AktualnyKlient, x => idAtrybutow == null || Sql.In(x.AtrybutId, idAtrybutow), new List<SortowanieKryteria<Cecha>> { sortowanie }, Paczka, IloscElementow).ToList();
        }

        [HttpPost]
        [Route("aktualizuj")]
        public bool AktualizujCechy(List<Cecha> listaCech)
        {
            try { 
                SolexBllCalosc.PobierzInstancje.CechyAtrybuty.AktualizujLubZapiszCechy(listaCech);
            }
            catch (Exception e)
            {
                SolexBllCalosc.PobierzInstancje.Log.Error($"Błąd aktualizacji cech przez API: {e.Message}");
                throw new Exception("Błąd aktualizacji cech przez API - sprawdz logi na serwerze.");
            }
            return true;
        }


        [HttpDelete]
        [Route("usun")]
        public bool UsunCechy(List<long> listaCech)
        {
            SolexBllCalosc.PobierzInstancje.DostepDane.Usun<Cecha, long>(listaCech);
            return true;
        }
    }
}
