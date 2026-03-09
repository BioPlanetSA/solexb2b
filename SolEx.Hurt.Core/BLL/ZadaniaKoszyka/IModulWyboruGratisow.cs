using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using System.Collections.Generic;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public interface IModulWyboruGratisow : IGrupaZadania
    {
        IEnumerable<OpisProduktuGratisowego> PobierzProdukty(IKlient klient);
    }
}