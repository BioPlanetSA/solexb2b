using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;

namespace SolEx.Hurt.Core.BLL.WarunkiRegulPunktow
{
    public interface IWarunekRegulyCalegoDokumentu
    {
        bool SpelniaWarunek(DokumentyBll dokument);

        bool OdwrocenieWarunku { get; set; }
    }
}