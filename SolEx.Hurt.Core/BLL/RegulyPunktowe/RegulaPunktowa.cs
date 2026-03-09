using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Core.BLL.RegulyPunktowe
{
    public abstract class RegulaPunktowa : ModulPosiadajacyWarunki
    {
        public RegulaPunktowa()
        {
            PrzetwarzacNastepneRegulyDlaDokumentu = true;
        }

        [FriendlyName("Czy przetwarzać kolejne reguly jeśli dokument spełnił warunki tej reguły")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool PrzetwarzacNastepneRegulyDlaDokumentu { get; set; }
    }
}