using SolEx.Hurt.Core.ModelBLL.Interfejsy;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    public interface IRegulaPozycji
    {
        bool OdwrocenieWarunku { get; set; }

        bool PozycjaSpelniaRegule(IKoszykPozycja pozycja, IKoszykiBLL koszyk);
    }
}