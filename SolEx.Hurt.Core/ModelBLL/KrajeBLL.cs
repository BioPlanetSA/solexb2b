using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.DostepDane.DaneObiektu;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Core.ModelBLL
{
    [EdytowalnyAdmin]
    [Alias("Kraje")]
    public class KrajeBLL : Kraje
    {
        [Ignore]
        [WidoczneListaAdmin(false, false, true, false, true, new[] { typeof(KrajeBLL) })]
        [GrupaAtttribute("Regiony", 1)]
        [FriendlyName("Regiony kraju")]
        [WymuszonyTypEdytora(TypEdytora.ListaPodrzedna, typeof(Region))]
        public IList<Region> Regiony
        {
            get { return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Region>(null).Where(x => x.KrajId == Id).ToList(); }
        }
    }
}
