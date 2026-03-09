using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.Modules.Api
{
    /// <summary>
    /// Aktualizuje cechy podane w obiekcie Data w formie Lista<cechy>
    /// </summary>
    public class AktualizujCechy : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            SolexBllCalosc.PobierzInstancje.CechyAtrybuty.AktualizujLubZapiszCechy((List<Cecha>) Data);
            //SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<Cecha>((List<Cecha>)Data);
            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<Cecha>); }
        }
    }

    /// <summary>
    /// Pobiera cechy w formie Słownik<klucz int ID,wartość cechy>
    /// </summary>
    [ApiUprawnioneRole(RoleType.Administrator, RoleType.Przedstawiciel)]
    public class PobierzCechy : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {
        protected override object Handle()
        {
            HashSet<int> ids = (HashSet<int>) Data;
            return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Cecha>(this.Customer.JezykId, null, x => Sql.In(x.Id, ids)).ToDictionary(x => x.Id, x => x);

            //var wszystkie = SolexBllCalosc.PobierzInstancje.CechyAtrybuty.PobierzWszystkieCechy(SesjaHelper.PobierzInstancje.JezykID);
            //List<CechyBll> cechy;
            //if (ids != null)
            //{
            //    cechy = wszystkie.WhereKeyIsIn(ids);
            //}
            //else
            //{
            //    cechy = wszystkie.Values.ToList();
            //}
            //Dictionary<int, Cecha> levels = cechy.Select(a => a as Cecha).ToDictionary(x => x.Id, x => x);
            //return levels;
        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof(HashSet<int>); }
        }
    }

    /// <summary>
    /// Usuwa cechy przekazane w obiekcie Data jako ListaID<int>
    /// </summary>
    public class UsunCechy : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            HashSet<int> ids = (HashSet<int>)Data;
            SolexBllCalosc.PobierzInstancje.DostepDane.Usun<Cecha, int>(ids.ToList());
            //SolexBllCalosc.PobierzInstancje.CechyAtrybuty.UsunCechy(ids);
            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(HashSet<int>); }
        }
    }
    public class PobierzCechyHash : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {
        protected override object Handle()
        {
            return new HashSet<long>( SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Cecha>(null).Select(x => x.Id) );
        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof(HashSet<long>); }
        }

    }
}
