using System.Collections.Generic;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Web.Site2.Helper.DaneListaAdmin;

namespace SolEx.Hurt.Web.Site2.Models.Admin
{
    public class EdycjaTresciAdmin : DaneEdycjaAdmin
    {
         
        public List<TrescWierszBll> Wiersze { get; set; }

        public EdycjaTresciAdmin()
        {
            
        }

        public EdycjaTresciAdmin(DaneEdycjaAdmin baza, List<TrescWierszBll> tresci)
            : base(baza)
        {
            Wiersze = tresci;
        }
    }
}