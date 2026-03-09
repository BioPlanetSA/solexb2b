using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.Modules.Api
{
    /// <summary>
    /// Aktualizuje adresy klientów podanych w obiekcie Data w formie Lista<Adresy>
    /// </summary>
    public class AktualizujAdresy : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            List<Adres> doZmiany = (List<Adres>)Data;
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<Adres>(doZmiany);
           
            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<Adres>); }
        }
    }

    /// <summary>
    /// Pobiera adresy klientów w formie Słownik<klucz int ID, wartość Adresy>
    /// </summary>
    [ApiUprawnioneRole(RoleType.Administrator, RoleType.Przedstawiciel)]
    public class PobierzAdresy : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {
        protected override object Handle()
        {
            Dictionary<long, Adres> levels =SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Adres>(null).ToDictionary(x => x.Id, x => x);

            return levels;
        }
    }
    /// <summary>
    /// Usuwa adresy klientów podanych w obiekcie Data jako ListaID<int>
    /// </summary>
    public class UsunAdresy : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            List<object> ids = (List<object>)Data;
            SolexBllCalosc.PobierzInstancje.DostepDane.Usun<Adres, object>(ids);
            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<object>); }
        }
    }

  
}
