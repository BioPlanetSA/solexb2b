using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Web.Site2.Helper;
using SolEx.Hurt.Web.Site2.PageBases;
using Klient = SolEx.Hurt.Core.Klient;

namespace SolEx.Hurt.Web.Site2.Controllers
{
    [RoutePrefix("rabaty")]
    [Route("{action=index}")]
    public class RabatyController : SolexControler
    {
        [Route("WszystkiePasujaceRabaty/{produktId:int}/{klientID:int}")]
        public ActionResult PobierzWszystkiePasujaceRabaty(int produktId, int klientID)
        {
            ProduktKlienta pb = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktKlienta>(produktId, SolexHelper.AktualnyJezyk.Id, SolexHelper.AktualnyKlient);
            IKlient k = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(klientID);
            int[] kategorieKlienta = k.Kategorie;

            Dictionary<RabatTyp, RabatBLL> listaRabatow = new Dictionary<RabatTyp, RabatBLL>();
            foreach (RabatTyp typ in Enum.GetValues(typeof(RabatTyp)))
            {
                RabatBLL rabaty = SolexBllCalosc.PobierzInstancje.Rabaty.Znajdz(produktId, pb.CechyProduktuWystepujaceWRabatach, k, kategorieKlienta, new List<RabatTyp> { typ }, SolexHelper.AktualnyKlient.WalutaId);
                if (rabaty != null)
                {
                    listaRabatow.Add(typ, rabaty);
                }
            }

            return PartialView("ListaRabatow", new Tuple<IKlient, ProduktKlienta, Dictionary<RabatTyp, RabatBLL>>(k, pb, listaRabatow));
        }
    }
}