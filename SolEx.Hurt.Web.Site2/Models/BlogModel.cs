using System.Collections.Generic;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class BlogModel
    {
        public BlogiSposobPokazaniaDaty PokazujDate { get; set; }

        public bool BlogPokazujZdjecia { get; set; }

        public int BlogIloscPierwszychZnakow { get; set; }

        public BlogiKolejnosc BlogKolejnosc { get; set; }

        public int BlogIleDynamicznieZaladowacAktualnosci { get; set; }
        public int BlogIlePokazywacMaxWpisow { get; set; }

        public long[] BlogKategorieDoPokazania { get; set; }
        public List<BlogWpisBll> BlogWpisList { get; set; }
        public bool BlogPokazujTytul { get; set; }

        public string BlogPresetDoZdjec { get; set; }

        public string SymbolStronyWpisPojedynczy { get; set; }
        public BlogModel()
        {
            BlogWpisList = new List<BlogWpisBll>();
            BlogKafleUklad = "col-xs-12 col-sm-6 col-md-4 col-lg-3";
            PokazWiecej = false;
        }

        public bool PokazWiecej { get; set; }

        public string BlogUklad { get; set; }

        public int IloscJuzPokazanaKlientowi { get; set; }

        public string BlogKafleUklad { get; set; }
        public string NaglowekWpisow { get; set; }
        public string CzytajDalejTresc { get; set; }
        public string[] CssDlaPojedynczegoWpisu { get; set; }

        public string UkladPuzli { get; set; }
    }
}