using System.Collections.Generic;

namespace SolEx.Hurt.Core.BLL.Interfejsy
{
    public interface IStaleFiltry
    {
        string SzukanaFraza { get; }

        HashSet<int> Pobierz();


        string PobierzKategorie();


        string Filtry { get; }
    }
}