using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public interface IModulKoszykGratisy : IGrupaZadania
    {
        Komunikat KomunikatWarunku(IKoszykiBLL koszyk);

        bool SpelniaRegule(IKoszykiBLL koszyk);
    }
}