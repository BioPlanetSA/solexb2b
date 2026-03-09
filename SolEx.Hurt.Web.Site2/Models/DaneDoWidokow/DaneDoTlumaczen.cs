using System.Collections.Generic;
using SolEx.Hurt.Web.Site2.Controllers;
using SolEx.Hurt.Web.Site2.Helper.DaneListaAdmin;

namespace SolEx.Hurt.Web.Site2.Models.DaneDoWidokow
{
    public class DaneDoTlumaczen
    {
        public DaneDoTlumaczen(List<DaneDoTabow> taby, List<TlumaczenieDoEdycji> tlumaczenia)
        {
            Taby = taby;
            Tlumaczenia = tlumaczenia;
        }
        public List<DaneDoTabow> Taby { get; set; }
        public List<TlumaczenieDoEdycji> Tlumaczenia{ get; set; }
    }
}