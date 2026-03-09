using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.Modules
{
    public class RejestracjePobieranieHandler : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
            List<Rejestracja> items =  SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Rejestracja>(null, x => x.StatusEksportu == RegisterExportStatus.Export && x.OddzialId == null).ToList();
            return items;
        }
    }
}
