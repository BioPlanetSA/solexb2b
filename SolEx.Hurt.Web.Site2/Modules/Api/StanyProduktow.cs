using System;
using System.Collections.Generic;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.Modules.Api
{
    /// <summary>
    /// Pobiera stany produktów dla magazynu wysłanego w obiekcie Data jako typ magazyn
    /// </summary>
    [ApiUprawnioneRole(RoleType.Przedstawiciel, RoleType.Administrator)]
    public class PobierzStany : ApiSessionBaseHandler
    {        
      protected override object Handle()
        {
            return SolexBllCalosc.PobierzInstancje.ProduktyStanBll.PobierzStanyProduktowNieZerowe((Magazyn)(Data));
        }

        public override Type PrzyjmowanyTyp => typeof(Magazyn);
    }

    /// <summary>
    /// Aktualizuje stany produktów wysłane w obiekcie Data jako Lista<Produkty_stany>
    /// </summary>
    public class AktualizujStany : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            var dane = (List<ProduktStan>) Data;
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<ProduktStan>(dane);
            //SolexBllCalosc.PobierzInstancje.ProduktyStanBll.StanyProduktowAktualizuj((List<ProduktStan>)Data);
           return null;
        }

        public override Type PrzyjmowanyTyp => typeof(List<ProduktStan>);
    }
}
