using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.Modules.Api
{
    /// <summary>
    /// Aktualizuje słowniki wysłane w obiekcie Data jako Lista<slowniki>
    /// </summary>
    public class AktualizujSlownikiHandler : ApiSessionBaseHandler,Model.Core.IDocumentApiVisible
    {
       
        protected override object Handle()
        {
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<Tlumaczenie>((List<Tlumaczenie>) Data);
            return true;
        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<Model.Tlumaczenie>); }
        }
    }

    public class UsunSlownikiHandler : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {

        protected override object Handle()
        {
            SolexBllCalosc.PobierzInstancje.DostepDane.Usun<Tlumaczenie,object>((List<object>)Data);
            return true;
        }

        public override Type PrzyjmowanyTyp
        {
            get { return typeof(List<object>); }
        }
    }

    /// <summary>
    /// Pobiera słowniki jako Słownik<klucz int ID , wartość slowniki/>
    /// </summary>
    public class PobierzSlownikiHandler : ApiSessionBaseHandler, Model.Core.IDocumentApiVisible
    {
        protected override object Handle()
        {
            return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Tlumaczenie>(null).ToDictionary(x => x.Id, x => x);
        }
    }
}