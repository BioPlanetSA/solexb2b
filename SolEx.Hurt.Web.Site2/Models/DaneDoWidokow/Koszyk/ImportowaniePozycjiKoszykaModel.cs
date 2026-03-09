using System.Collections.Generic;
using SolEx.Hurt.Core.BLL;

namespace SolEx.Hurt.Web.Site2.Models.DaneDoWidokow.Koszyk
{
    public class ImportowaniePozycjiKoszykaModel
    {
        public  Dictionary<string, OpisImportera> Lista { get; set; }

        public ImportowaniePozycjiKoszykaModel()
        {
            Lista = new Dictionary<string, OpisImportera>();
        }
        public string Wybrany { get; set; }
        public DaneNaglowekStopka NaglowekStopka { get; set; }
}
}
