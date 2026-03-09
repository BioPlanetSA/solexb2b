using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    [FriendlyName("Pozycja jest produktem wirtualnym.", FriendlyOpis = "Sprawdza czy pozycja koszyka jest produktem wirtualnym.")]
    public class PozycjaJestProduktemWirtualnym : RegulaKoszyka, IRegulaPozycji
    {
        public bool PozycjaSpelniaRegule(IKoszykPozycja pozycja, IKoszykiBLL koszyk)
        {
            return pozycja.ProduktId != pozycja.ProduktBazowyId;
        }
    }
}