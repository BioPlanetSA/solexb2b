using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Core;
using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.Modules.Api
{
    public class Synchronizator : ApiSessionBaseHandler, IDocumentApiVisible
    {
        public override bool WymagajAktywnejSesji
        {
            get { return false; }
        }

        protected override object Handle()
        {
            var pliki = Core.BLL.PlikiDostep.PobierzInstancje.PobierzInfoOPlikachSynchronizatora();
            return pliki;
        }
    }
}