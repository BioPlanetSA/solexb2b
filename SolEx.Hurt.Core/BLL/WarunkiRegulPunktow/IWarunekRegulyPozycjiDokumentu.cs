using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Core.BLL.WarunkiRegulPunktow
{
    public interface IWarunekRegulyPozycjiDokumentu
    {
        bool OdwrocenieWarunku { get; set; }

        bool SpelniaWarunek(DokumentuPozycjaBazowa pozycja, DokumentyBll dokument);
    }
}