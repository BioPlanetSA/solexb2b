using SolEx.Hurt.Model.Enums;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SolEx.Hurt.Core.ModelBLL;

namespace SolEx.Hurt.Core.BLL.Interfejsy
{
    public interface ISzukanie
    {
        string OczyscFrazePrzygotujDoRegexa(string fraza);

        List<Regex> PobierzWyszukiwanieRegex(string wyszukiwane);

        Regex StworzRegexDlaFrazy(string fraza);

        IList<T> WyszukajObiekty<T>(IList<T> wyszukajObiekty, string szukane, IEnumerable<string> szukanepola);

        IList<T> SortujObiekty<T>(IList<T> obiekty, Sortowanie sortowanie);
    }
}