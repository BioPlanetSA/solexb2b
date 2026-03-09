using System.Collections.Generic;
using SolEx.Hurt.Core.ModelBLL.ObiektyMaili;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class ParametryDoRejestracji
    {
        public ParametryDoRejestracji( List<ParametryPola> pola, string odpowiedz, bool czyPokazywacCapche, List<string> bledy = null)
        {
            Pola = pola;
            Bledy = bledy;
            Odpowiedz = odpowiedz;
            CzyPokazywacCaptche = czyPokazywacCapche;
        }
        
        public List<ParametryPola> Pola{ get; set; }
        public List<string> Bledy { get; set; }
        public string Odpowiedz { get; set; }
        public bool CzyPokazywacCaptche { get; set; }
    }
}