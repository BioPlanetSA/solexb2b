using System;
using System.Collections.Generic;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.Modules.Api
{
    /// <summary>
    /// Aktualizuje pola systemowe wysłane w obiekcie Data jako Lista<system_pola>
    /// </summary>
    public class AktualizujSystemPolaHandler : ApiSessionBaseHandler, SolEx.Hurt.Model.Core.IDocumentApiVisible
    {
       
        protected override object Handle()
        {
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<TlumaczeniePole>((IList<TlumaczeniePole>)Data);// SystemPolaDal.Update((List<system_pola>)Data);
            return (IList<TlumaczeniePole>)Data;
        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<TlumaczeniePole>); }
        }
    }

    /// <summary>
    /// Pobiera pola systemowe jako Lista<system_pola>
    /// </summary>
    [ApiUprawnioneRole(RoleType.Pracownik)]
    public class PobierzSystemPolaHandlerr : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {

        protected override object Handle()
        {

            return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<TlumaczeniePole>(null);
        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof(TlumaczeniePole); }
        }
    }
}