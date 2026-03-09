using System.Collections.Generic;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Web.Site2.Models.DaneDoWidokow
{
    public class DaneDoWidokuSortowania
    {
        public DaneDoWidokuSortowania(Dictionary<string, Sortowanie> dostepne, Sortowanie wybrane,string gdzie)
        {
            DostepneSortowanie  = dostepne;
            WybraneSortowanie = wybrane;
            Przeznaczenie = gdzie;
        }

        public Dictionary<string, Sortowanie> DostepneSortowanie { get; set; }
        public Sortowanie WybraneSortowanie { get; set; }
        public string Przeznaczenie { get; set; }
    }
}