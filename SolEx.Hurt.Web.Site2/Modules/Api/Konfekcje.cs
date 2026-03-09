using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.OrmLite;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.Modules.Api
{
    /// <summary>
    /// Aktualizuje konfekcje przekazane jako Lista<Konfekcje> w obiekcie Data
    /// </summary>
    public class AktualizujKonfekcje : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<Konfekcje>((List<Konfekcje>)Data);
          // SolexBllCalosc.PobierzInstancje.Rabaty.AktualizujRegulyKonfekcji
            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<Konfekcje>); }
        }
    }

    /// <summary>
    /// Pobiera konfekcje jako lista
    /// </summary>
    public class PobierzKonfekcje : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            HashSet<long>idKonfekcji = (HashSet<long>)Data;
            if (idKonfekcji == null || !idKonfekcji.Any())
            {
                return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Konfekcje>(Customer);
            }
            return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Konfekcje>(Customer, x => Sql.In(x.Id,idKonfekcji));
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(HashSet<long>); }
        }
    }
    /// <summary>
    /// Pobiera klucze id konfekcj z serwera
    /// </summary>
    public class PobierzKonfekcjeIds : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Konfekcje>(Customer, null).Select(x => x.Id).ToList();
        }
    }
    /// <summary>
    /// Usuwa konfekcje przekazane w obiekcie Data jako ListaID<int>
    /// </summary>
    public class UsunKonfekcje : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            List<long> ids = (List<long>)Data;
            if (ids.Any())
            {
                SolexBllCalosc.PobierzInstancje.DostepDane.Usun<Konfekcje,long>(ids);
            }
            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<long>); }
        }
    }
}
