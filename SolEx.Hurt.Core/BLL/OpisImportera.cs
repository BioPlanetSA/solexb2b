using System.Collections.Generic;

namespace SolEx.Hurt.Core.BLL
{
    public class OpisImportera
    {
        public OpisImportera()
        {
            OpisTypu = "";
            DaneDoPomocy = null;
            CzyWybrany = false;
            PlikWzorcowy = null;
        }
        public string Nazwa { get; set; }
        public string OpisTypu { get; set; }
        public HashSet<string> DaneDoPomocy { get; set; }
        public bool CzyWybrany { get; set; }
        public string PlikWzorcowy { get; set; }
    }
}