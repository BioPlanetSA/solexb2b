using System.Collections.Generic;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Web;
using System.Linq;
using SolEx.Hurt.Web.Site2.Models;

namespace SolEx.Hurt.Core.BLL.Web
{
    public class ParametryDoPrzeladowaniaKoszyka
    {
        public List<KoszykPozycje> Pozycje { get; set; }
        public int IdKontrolki { get; set; }

        public DodatkowePolaKoszykaPogrupowane[] PolaWlasneKoszyka { get; set; }

    }
}