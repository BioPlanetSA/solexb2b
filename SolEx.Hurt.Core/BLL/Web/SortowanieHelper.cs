using System;
using System.Collections.Generic;
using SolEx.Hurt.Core.ModelBLL;

namespace SolEx.Hurt.Core.BLL.Web
{
    public class SortowanieHelper : BllBaza<SortowanieHelper>
    {
        public Dictionary<string, Sortowanie> PobierzSortowanieZOpisem<T>(IEnumerable<Sortowanie> sortowanie)
        {
            Dictionary<string, Sortowanie> sortowania = new Dictionary<string, Sortowanie>();
            foreach (Sortowanie p in sortowanie)
            {
                string opiss;
                if (!string.IsNullOrEmpty(p.Opis))
                {
                    opiss = p.Opis;
                }
                else
                {
                    opiss = p.PobierzOpisNaPodstawiePol<T>("Rosnąco wg", "Malejąco wg");
                }
                sortowania.Add(opiss, p);
            }
            return sortowania;
        }
    }
}