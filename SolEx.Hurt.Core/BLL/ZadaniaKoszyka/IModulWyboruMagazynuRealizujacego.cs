using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using System.Collections.Generic;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public interface IModulWyboruMagazynuRealizujacego : IGrupaZadania
    {
        HashSet<string> PobierzDostepneMagazyny(IKlient klient);
    }
}