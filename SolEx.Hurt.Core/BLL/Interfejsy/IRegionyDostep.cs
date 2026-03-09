using SolEx.Hurt.Model;
using System.Collections.Generic;

namespace SolEx.Hurt.Core.BLL.Interfejsy
{
    public interface IRegionyDostep
    {
        IList<Region> PobierzRegionyKraju(int kraj, int jezyk, bool tylkoAktywne = true);
    }
}