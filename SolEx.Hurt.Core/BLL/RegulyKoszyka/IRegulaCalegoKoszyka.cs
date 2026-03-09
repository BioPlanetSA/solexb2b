using SolEx.Hurt.Core.ModelBLL.Interfejsy;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    public interface IRegulaCalegoKoszyka
    {
        bool OdwrocenieWarunku { get; set; }

        bool KoszykSpelniaRegule(IKoszykiBLL koszyk);
    }
}