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
    public class AktualizujKraje : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            List<Kraje> doZmiany = (List<Kraje>)Data;

            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<Kraje>(doZmiany);
           
            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<Kraje>); }
        }
    }

    /// <summary>
    /// Pobiera adresy klientów w formie Słownik<klucz int ID, wartość Adresy>
    /// </summary>
    [ApiUprawnioneRole(RoleType.Administrator, RoleType.Przedstawiciel)]
    public class PobierzKraje : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {

        protected override object Handle()
        {
            IDictionary<int, Kraje> levels = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Kraje>(null).ToDictionary(x => x.Id, x => x);

            return levels;
        }
    }

    /// <summary>
    /// Usuwa adresy klientów podanych w obiekcie Data jako ListaID<int>
    /// </summary>
    public class UsunKraje : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            HashSet<int> ids = (HashSet<int>)Data;
            SolexBllCalosc.PobierzInstancje.DostepDane.Usun<Kraje, int>(ids.ToList());
            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(HashSet<int>); }
        }
    }
}
