using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Web.Site2.Helper;
using SolEx.Hurt.Web.Site2.Models;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Controllers
{
    public class TowaryZaPunktyController : SolexControler
    {
        // GET: TowaryZaPunkty
        public PartialViewResult Index()
        {
            decimal cena = 1m;
            var produkty = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ProduktBazowy>(SolexHelper.AktualnyJezyk.Id, null);
          //  var idp = produkty.Where(x=>x.CenaWPunktach > 0 && x.widoczny).Select(x=>x.produkt_id).ToList();
            var a = produkty.Where(x=>x.CenaWPunktach>0 && x.Widoczny).ToList();
            List<IProduktKlienta> prodKl = new List<IProduktKlienta>();
            foreach (var p in a)
            {
                //tu moze byc zonk z waluta - ale zeby dac cokolwiek dalem z klienta - chbyba przy definicji gratisow trzeba podawac walute jawnie
                IFlatCeny cenaw = new FlatCeny(SolexHelper.AktualnyKlient.Id, p.Id, cena, 0, SolexHelper.AktualnyKlient.WalutaId, 0);
                ProduktKlientZaPunktyLubGratis pk = new ProduktKlientZaPunktyLubGratis(p, new FlatCenyBLL(cenaw, p,SolexHelper.AktualnyKlient), SolexHelper.AktualnyKlient);
                prodKl.Add(pk);
            }
            ListaProduktowZKategoriam ust = new ListaProduktowZKategoriam(prodKl.OrderBy(x=>x.CenaWPunktach));
           
            return PartialView("_Lista", ust);
        }
    }
}