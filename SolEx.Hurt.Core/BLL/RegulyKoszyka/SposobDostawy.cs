using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    [FriendlyName("Sposób dostawy",FriendlyOpis = "Czy koszyk ma wybrany określony sposób dostawy")]
    public class SposobDostawy : RegulaKoszyka, IRegulaCalegoKoszyka, IRegulaPozycji
    {
        [FriendlyName("Sposoby dostawy")]
        [PobieranieSlownika(typeof(SlownikSposobyDostawy))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> SposobDostawyId { get; set; }

        public bool KoszykSpelniaRegule(IKoszykiBLL koszyk)
        {
            return Regula(koszyk);
        }

        public bool PozycjaSpelniaRegule(IKoszykPozycja pozycja, IKoszykiBLL koszyk)
        {
            return Regula(koszyk);
        }

        private bool Regula(IKoszykiBLL koszyk)
        {
            if (SposobDostawyId == null || !SposobDostawyId.Any()) return true;
            if (SposobDostawyId.Any(x => int.Parse(x) == koszyk.KosztDostawyId)) return true;
            return false;
        }
    }
}