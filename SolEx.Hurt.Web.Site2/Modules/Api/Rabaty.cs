using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.Modules.Api
{
    /// <summary>
    /// Aktualizuje rabaty wysłane w obiekcie Data jako Lista<rabaty>
    /// </summary>
    public class AktualizujRabaty : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<Rabat>((List<Rabat>)Data);
            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<Rabat>); }
        }
    }

    /// <summary>
    /// Pobiera rabaty jako Słownik<klucz int ID, wartość rabaty>
    /// </summary>
    public class PobierzRabaty : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {
        protected override object Handle()
        {
            var ids = (HashSet<long>) Data;
            if (ids != null && ids.Any())
            {
                return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<RabatBLL>(null, x => Sql.In(x.Id, ids)).ToDictionary(x => x.Id, x => new Rabat(x));
            }
            return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<RabatBLL>(null).ToDictionary(x => x.Id, x => new Rabat(x));
        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof(HashSet<long>); }
        }

    }

    /// <summary>
    /// Zwraca listę ID rabatów jako ListaID<int> potrzebną do paczkowanego pobierania rabatów
    /// </summary>
    public class PobierzRabatyHash : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {
        protected override object Handle()
        {
            return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<RabatBLL>(null).Select(x=>x.Id);
        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<long>); }
        }

    }

    /// <summary>
    /// Usuwa rabaty wysłane w obiekcie Data jako ListaID<long>
    /// </summary>
    public class UsunRabaty : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            HashSet<long> ids = (HashSet<long>)Data;
            SolexBllCalosc.PobierzInstancje.DostepDane.Usun<RabatBLL, long>(ids.ToList());
            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(HashSet<long>); }
        }
    }
}
