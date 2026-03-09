using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.Modules.Api
{
    /// <summary>
    /// Aktualizuje magazyny wysłane w obiekcie Data jako Lista<magazyny>
    /// </summary>
    [ApiUprawnioneRole(RoleType.Przedstawiciel, RoleType.Administrator)]
    public class AktualizujMagazyny : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<Magazyn>((List<Magazyn>)Data);
            return null;
        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<Magazyn>); }
        }
    }

    /// <summary>
    /// Usuwa magazyny wysłane w obiekcie Data jako ListaID<int>
    /// </summary>
    public class UsunMagazyny : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            SolexBllCalosc.PobierzInstancje.DostepDane.Usun<Magazyn, object>(  ((HashSet<object>)Data).ToList()  );
            return null;
        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof(HashSet<object>); }
        }
    }

    /// <summary>
    /// Pobiera magazyny jako Lista<magazyny>
    /// </summary>
    [ApiUprawnioneRole(RoleType.Przedstawiciel, RoleType.Administrator)]
    public class PobierzMagazyny : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Magazyn>(null);
        }
    }
}
