using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ServiceStack.Common;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Web.Site2.Helper;
using SolEx.Hurt.Web.Site2.Models;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Controllers
{
    [RoutePrefix("Filtry")]
    public class StaleFiltryController : SolexControler
    {
        /// <summary>
        /// Akcja pobierająca atrybuty jako stałe filtry wraz z filtrowaniem względem dostępnych produktów dla klietna - jeżeli będzie wybrany stały filtr natomiast klient nie bedzie miał widocznego produktu z danym atrybutem filtr nie zostanie pokazany
        /// </summary>
        /// <param name="listaatrybutow"></param>
        /// <returns></returns>
        [ChildActionOnly]
        public PartialViewResult StaleFiltryDoPokazania(int[] listaatrybutow)
        {
            if (listaatrybutow == null || !listaatrybutow.Any())
            {
                return null;
            }
            //List<AtrybutBll> atrybuty = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<AtrybutBll>(SolexHelper.AktualnyKlient, x => listaatrybutow.Contains(x.Id)).OrderBy(x=>x.Kolejnosc).ThenBy(x=>x.Nazwa).ToList();
            //wyciagamy tylko te atrybuty / cechy ktore maja jakiekolwiek produkty

            //Pobieramy produkty dla klienta
            IList<ProduktKlienta> produkty = SolexBllCalosc.PobierzInstancje.ProduktyKlienta.PobierzProduktyKlientaZCache(SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient);

            //przechodzimy po atrybutach i sprawdzamy czy jest chociaż jeden produkt dla klienta z atrybutem będącym stałym filtrem
            HashSet<int?> atr = new HashSet<int?>( produkty.SelectMany(x => x.Cechy.Values.Select(y => y.AtrybutId)) );
            HashSet<int> atrybutyDoPokazania = new HashSet<int>( listaatrybutow.Where(x => atr.Contains(x)));

            //Pobieramy które stałe filtry miał klient zaznaczone
            Dictionary<int, HashSet<long>> wybrane = SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzStaleFiltry(SolexHelper.AktualnyKlient);

            //Pobieramy stałe filtry które są widoczne
            List<AtrybutBll> atrybuty = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<AtrybutBll>(SolexHelper.AktualnyKlient, x => Sql.In(x.Id, atrybutyDoPokazania) && x.Widoczny).OrderBy(x => x.Kolejnosc).ThenBy(x => x.Nazwa).ToList();
            
            return PartialView("_StaleFiltry", new ParametryDoStalychFiltrow(atrybuty, wybrane, true));
        }

        /// <summary>
        /// usuwa wybrany filtr
        /// </summary>
        /// <param name="cecha"></param>
        /// <returns></returns>
        [Route("UsunStalyFiltr")]
        [Route("UsunStalyFiltr/{cecha}")]
        public ActionResult UsunStalyFiltr(long cecha) 
        {
            SolexBllCalosc.PobierzInstancje.ProfilKlienta.UsunStalyFiltr(SolexHelper.AktualnyKlient,new HashSet<long> { cecha});

            long katId;
            if (Url.AktualnaStronaToStronaProduktow(out katId))
            {
                return AkcjaPowrotu();
            }
            return AkcjaPowrotu( Url.LinkProdukty(SolexHelper.AktualnyJezyk) );
        }

        /// <summary>
        /// Usuwa wszstkkie stałe filtry z profilu klienta
        /// </summary>
        /// <returns></returns>
        [Route("UsunStaleFiltry")]
        public ActionResult UsunStaleFiltry() 
        {
            SolexBllCalosc.PobierzInstancje.ProfilKlienta.UsunWartosc(SolexHelper.AktualnyKlient, TypUstawieniaKlienta.StalyFiltr);
            long katId;

            if (Url.AktualnaStronaToStronaProduktow(out katId))
            {
                return AkcjaPowrotu();
            }
            return AkcjaPowrotu(Url.LinkProdukty(SolexHelper.AktualnyJezyk));
        }

        /// <summary>
        /// Dodaje wybraną cechę do stałych wilftów 
        /// </summary>
        /// <param name="cecha"></param>
        /// <param name="zamiana">     
        /// 0 - dodajemy cechy  do filtrów
        /// 1 - dodajemy tylko jedną cechę w ramach jednego atrubutu, atrybutów może byc wiele
        /// 2 - dodajemy tylko jeden filtry stały</param>
        /// <returns></returns>
        [Route("DodajStalyFiltr")]
        [Route("DodajStalyFiltr/{cecha}")]
        [Route("DodajStalyFiltr/{cecha}/{zamiana}")]
        public void DodajStalyFiltr(long cecha,int? zamiana=0)
        {
            // 0 - dodajemy cechy do filtrów
            // 1 - dodajemy tylko jedną cechę w ramach jednego atrubutu
            // 2 - dodajemy tylko jeden filtry stały
            if (zamiana==2)
            {
                SolexBllCalosc.PobierzInstancje.ProfilKlienta.UsunWartosc(SolexHelper.AktualnyKlient, TypUstawieniaKlienta.StalyFiltr);
            }
            SolexBllCalosc.PobierzInstancje.ProfilKlienta.DodajStatyFiltr(SolexHelper.AktualnyKlient, new HashSet<long>{cecha },zamiana==1);
        }
       
    }
}