using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    [FriendlyName("Sposób płatności", FriendlyOpis = "Czy koszyk ma wybrany określony sposób płatności")]
    public class SposobPlatnosci : RegulaKoszyka, IRegulaCalegoKoszyka, IRegulaPozycji
    {
        [FriendlyName("Sposoby płatności")]
        [PobieranieSlownika(typeof(SlownikSposobyPlatnosci))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<int> SposobPlanosciID { get; set; }

        //var zad = SolexBllCalosc.PobierzInstancje.ZadaniaBLL.PobierzZadaniaCalegoKoszyka((typeof(ISposobPlatnosci)),  SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDDomyslny, null);
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
            if (SposobPlanosciID== null || !SposobPlanosciID.Any()) return true;
            if (SposobPlanosciID.Any(x => x == koszyk.PlatnoscId))
            {
                return true;
            }
            return false;
        }
    }
}