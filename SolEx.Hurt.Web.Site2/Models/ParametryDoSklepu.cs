using System.Collections.Generic;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Web.Site2.Models
{
  public class ParametryDoSklepu
    {
      public ParametryDoSklepu(KontrolkiTresci.Sklepy kontrolka)
        {       
            Kontrolka = kontrolka;
        }
        public KontrolkiTresci.Sklepy Kontrolka { get; set; }
        public List<SklepyBll> Sklepy { get; set; }

        public List<KategoriaSklepu> KategorieSklepowDoPokazania { get; set; }

        public List<string> MiastaDlaKategorii { get; set; }
        public string Punkty { get; set; }
        public long? KategoriaId { get; set; }
    }
}