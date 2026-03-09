using System;
using System.Collections.Generic;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using  System.Linq;
using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.Modules.Api
{
    /// <summary>
    /// Aktualizuje adresy klientów podanych w obiekcie Data w formie Lista<Adresy>
    /// </summary>
    public class AktualizujRegiony : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            List<Region> doZmiany = (List<Region>)Data;

          SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<Region>(doZmiany);
           
            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<Region>); }
        }
    }

    /// <summary>
    /// Pobiera adresy klientów w formie Słownik<klucz int ID, wartość Adresy>
    /// </summary>
    [ApiUprawnioneRole(RoleType.Administrator, RoleType.Przedstawiciel)]
    public class PobierzRegiony : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {
        protected override object Handle()
        {
            IDictionary<int, Region> levels = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Region>(null).ToDictionary(x=>x.Id,x=>x);

            return levels;
        }
    }

    /// <summary>
    /// Usuwa adresy klientów podanych w obiekcie Data jako ListaID<int>
    /// </summary>
    public class UsunRegiony : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            List<object> ids = (List<object>)Data;
            SolexBllCalosc.PobierzInstancje.DostepDane.Usun<Region, object>(ids);
            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<object>); }
        }
    }
}
