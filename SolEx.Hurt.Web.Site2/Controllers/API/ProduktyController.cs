using System.Collections.Generic;
using System.Linq;
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
    [System.Web.Http.RoutePrefix("api2/produkty")]
    [AutoryzacjaSolex]
    public class ProduktyController : SolexAPIController
    {
        [System.Web.Http.AcceptVerbs("GET")]
        [System.Web.Http.Route("index")]
        public int[] Index()
        {
            return new int[] {4, 4};
        }


        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("")]
        [ObslugaStronnicowania]
        public List<Produkt> Produkty()
        {
            SortowanieKryteria<Produkt>sortowanie=new SortowanieKryteria<Produkt>(x=>x.Id,KolejnoscSortowania.asc, "Id");
            return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Produkt>(SolexHelper.AktualnyKlient,null,new List<SortowanieKryteria<Produkt>> {sortowanie}, Paczka,IloscElementow).ToList();
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("aktualizuj")]
        public bool AktualizujProdukty(IList<Produkt> listaProduktow)
        {
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<Produkt>(listaProduktow);
            return true;
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("ProduktyCechy")]
        [ObslugaStronnicowania]
        public List<ProduktCecha> ProduktyCechy(HashSet<long>idCech)
        {
            SortowanieKryteria<ProduktCecha> sortowanie = new SortowanieKryteria<ProduktCecha>(x => x.Id, KolejnoscSortowania.asc, "Id");
            return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ProduktCecha>(SolexHelper.AktualnyKlient, x=>idCech==null || Sql.In(x.CechaId,idCech), new List<SortowanieKryteria<ProduktCecha>> { sortowanie }, Paczka, IloscElementow).ToList();
        }
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("AktualizujProduktyCechy")]
        public bool AktualizujProduktyCechy(IList<ProduktCecha> listalacznikow)
        {
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<ProduktCecha>(listalacznikow);
            return true;
        }
        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("UsunProduktyCechy")]
        public bool UsunProduktyCechy(List<long> idLacznikow )
        {
            SolexBllCalosc.PobierzInstancje.DostepDane.Usun<ProduktCecha, long>(idLacznikow);
            return true;
        }
        
    }

}