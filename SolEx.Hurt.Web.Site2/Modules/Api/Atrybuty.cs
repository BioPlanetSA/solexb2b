using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.Modules.Api
{
    /// <summary>
    /// Aktualizuje atrybuty podane w obiekcie Data typu Lista<atrybuty>
    /// </summary>
    public class AktualizujAtrybuty : ApiSessionBaseHandler
    {
        protected override object Handle() 
        {
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<AtrybutBll>((List<AtrybutBll>)Data);
            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<AtrybutBll>); }
        }
    }

    /// <summary>
    /// Pobiera atrybuty w formie Słownik<klucz int ID, wartość atrybuty>
    /// </summary>
    public class PobierzAtrybuty : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {
        protected override object Handle()
        {
            //Dictionary<int, atrybuty> levels =
                //SolexBllCalosc.PobierzInstancje.CechyAtrybuty.Pobierz((AtrybutySearchCriteria)SearchCriteriaObject, SolEx.Hurt.Core.BLL.SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDPolski).ToDictionary(x => x.atrybut_id, x => new atrybuty(x));

            //return levels;

            var ids = (HashSet<int>)Data;
            var atrybuty = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<AtrybutBll>(null, x => ids == null || ids.Contains(x.Id));
            return atrybuty.Select(x => new Atrybut(x)).ToDictionary(x => x.Id, x => x);
        }
    }

    /// <summary>
    /// Usuwa atrybuty podane w obiekcie Data jako ListaID<int>
    /// </summary>
    public class UsunAtrybuty : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            List<int> ids = (List<int>)Data;
            SolexBllCalosc.PobierzInstancje.DostepDane.Usun<AtrybutBll, int>(ids);
            //SolexBllCalosc.PobierzInstancje.CechyAtrybuty.Usun(ids);
            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<int>); }
        }
    }
}
