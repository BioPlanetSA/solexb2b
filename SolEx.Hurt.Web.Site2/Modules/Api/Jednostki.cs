using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.Modules.Api
{
    /// <summary>
    /// Aktuailzuje jednostki produktów przekazane w obiekcie Data w formie Lista<Jednostki>
    /// </summary>
    public class AktualizujJednostki : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<Jednostka>((List<Jednostka>)Data);
            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<Jednostka>); }
        }
    }

    /// <summary>
    /// Pobiera jednostki produktów jako Slownik<klucz int ID,wartość Jednostki>
    /// </summary>
    [ApiUprawnioneRole(RoleType.Administrator, RoleType.Przedstawiciel)]
    public class PobierzJednostki : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {
        protected override object Handle()
        {
            Dictionary<long, Model.Jednostka> levels = SolexBllCalosc.PobierzInstancje.JednostkaProduktuBll.PobierzJednostki(SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDPolski);

            return levels;
        }
    }

    /// <summary>
    /// Usuwa jednostki produktów przekazanych w obiekcie Data jako ListaID<long>
    /// </summary>
    public class UsunJednostki : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            HashSet<long> ids = (HashSet<long>)Data;
            if (ids.Any())
            {
                SolexBllCalosc.PobierzInstancje.DostepDane.Usun<Jednostka, long>(ids.ToList());
            }
            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(HashSet<long>); }
        }
    }
}