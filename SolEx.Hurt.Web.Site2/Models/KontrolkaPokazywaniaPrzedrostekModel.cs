using System.Collections.Generic;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class KontrolkaPokazywaniaPrzedrostekModel
    {
        public List<string> WartoscDoPokazania { get; set; }
        public string Przedrostek { get; set; }
        public string Przyrostek { get; set; }
        public string FrazaDoPokazania { get; set; }

        public KontrolkaPokazywaniaPrzedrostekModel(){}

        public string KlasyCSS { get; set; }

        public KontrolkaPokazywaniaPrzedrostekModel(List<string> wartoscDoPokazania, string przedrostek, string przyrostek,
            string frazaDoPokazania)
        {
            WartoscDoPokazania = wartoscDoPokazania;
            Przedrostek = przedrostek;
            Przyrostek = przyrostek;
            FrazaDoPokazania = frazaDoPokazania;
        }
    }
}