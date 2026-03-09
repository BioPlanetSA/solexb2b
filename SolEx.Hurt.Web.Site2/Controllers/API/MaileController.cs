using System.Web.Http;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Web.Site2.AtrybutyMvc;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Controllers.API
{
    [RoutePrefix("api2/maile")]
    [AutoryzacjaSolex]
    public class MaileController : SolexAPIController
    {
        [Route("MailNoweProdukty")]
        [AcceptVerbs("GET")]
        public bool MailNoweProdukty(long[] idCechyKoniecznej, long[] idCechyNieMozePosiadac, bool wysylajDoSubkont)
        {
            SolexBllCalosc.PobierzInstancje.MailNoweProdukty.MailONowychProduktach(idCechyKoniecznej, idCechyNieMozePosiadac, wysylajDoSubkont);
            return true;
        }

        [AcceptVerbs("GET")]
        [Route("Niezaplacone/{ileDniPrzed}/{ileDniPo}/{coIleDniPonowneWyslanie}")]
        public bool Niezaplacone(int ileDniPrzed, int ileDniPo, int? coIleDniPonowneWyslanie, int[] kategoriaKlientaNieWysylaj, int[] kategoriaKlientaWysylaj)
        {
            SolexBllCalosc.PobierzInstancje.DokumentyDostep.WyslijMailaOPrzeterminowanychFakturach(ileDniPrzed, ileDniPo, coIleDniPonowneWyslanie, kategoriaKlientaNieWysylaj, kategoriaKlientaWysylaj);
            return true;
        }

        [AcceptVerbs("GET")]
        [Route("MailProduktyPrzyjeteNaMagazyn")]
        public bool MailProduktyPrzyjeteNaMagazyn(long[] idCechKoniecznych, long[] idCechZabronionych, decimal minimalneZwiekszenieStanuPrzelicznik, decimal minimalnaIloscBrakuPrzelicznik, int[] idMagazynow)
        {
            SolexBllCalosc.PobierzInstancje.MailProduktyPrzyjeteNaMagazyn.MailOProduktachPrzyjetychNaMagazyn(idCechKoniecznych, idCechZabronionych, minimalneZwiekszenieStanuPrzelicznik, minimalnaIloscBrakuPrzelicznik, idMagazynow);
            return true;
        }

        //public bool PowiadomienieODostepnosciProduktu()
        //{
        //    return true;
        //}
    }
}
