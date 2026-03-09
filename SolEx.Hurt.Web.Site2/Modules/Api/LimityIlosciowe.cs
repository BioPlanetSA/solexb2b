using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.Modules.Api
{
    /// <summary>
    /// Aktualizuje limity ilościowe wysłane w obiekcie Data jako Lista<KlienciLimityIlosciowe>
    /// </summary>
    public class AktualizujLimityIlosciowe : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            List<KlientLimitIlosciowy> doZmiany = (List<KlientLimitIlosciowy>)Data;
            if (doZmiany.Count > 0)
            {
                SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe(doZmiany);
            }
            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<KlientLimitIlosciowy>); }
        }
    }

    /// <summary>
    /// Pobiera limity ilościowe jako Słownik<klucz int ID, wartość KlienciLimityIlosciowe>
    /// </summary>
    class PobierzLimityIlosciowe : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<KlientLimitIlosciowy>(null).ToDictionary(x => x.Id, x => x); //Core.BLL.LimityIloscioweBLL.PobierzWszystkie();
        }
    }

    /// <summary>
    /// Usuwa limity ilościowe wysłane w obiekcie Data jako ListaID<int>
    /// </summary>
    public class UsunLimityIlosciowe : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            List<int> ids = (List<int>)Data;
            if (ids.Count > 0)
            {
                //Core.BLL.LimityIloscioweBLL.Usun(ids);
                foreach (var id in ids)
                {
                    SolexBllCalosc.PobierzInstancje.DostepDane.UsunWybrane<KlientLimitIlosciowy, int>(x => x.Id == id);
                }
            }
            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<int>); }
        }
    }
}
