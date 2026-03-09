using SolEx.Hurt.Model;
using System.Collections.Generic;

namespace SolEx.Hurt.Core.BLL.Interfejsy
{
    public interface IProduktyJednostkiDostep
    {
        IDictionary<long, List<JednostkaProduktu>> PobierzJednostkiProduktuWgProduktu(int jezyk);
    }
}