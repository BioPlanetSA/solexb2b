using System.Collections.Generic;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.Modules
{
    public class RejestracjeAktualizacjaHandler : ApiSessionBaseHandler
    {
        protected override object Handle()
        {
                List<Rejestracja> items = (List<Rejestracja>)Data;
                if (items != null && items.Count > 0)
                {
                    for (int i = 0; i < items.Count; i++)
                    {
                        SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(items[i]);
                    }
                }
            return Helpers.Status.Utworz(Helpers.StatusApi.Ok);
        }
        public override System.Type PrzyjmowanyTyp
        {
            get { return typeof (List<Rejestracja>); }
        }
    }
}
