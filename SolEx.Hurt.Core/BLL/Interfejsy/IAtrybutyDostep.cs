using SolEx.Hurt.Model;
using System.Collections.Generic;

namespace SolEx.Hurt.Core.BLL.Interfejsy
{
    public interface IAtrybutyDostep
    {
        void AktualizujAtrybuty(List<Atrybut> data);

        List<Atrybut> PobierzAtrybuty();

        void UsunAtrybuty(HashSet<int> ids);
    }
}