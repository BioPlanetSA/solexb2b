using System.Collections.Generic;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.Hurt.Web.Site2.Models.DaneDoWidokow
{
    public class DaneDoWidokuPlikow
    {
        public DaneDoWidokuPlikow(List<ListaPlikowModel> lista, bool pokazuj,string rodzic, bool pokazujNaglowek)
        {
            ListaPlikow = lista;
            PokazujDate = pokazuj;
            Rodzic = rodzic;
            PokazujNaglowek = pokazujNaglowek;
        }

        public bool PokazujNaglowek { get; set; }

        public List<ListaPlikowModel> ListaPlikow { get; set; }
        public bool PokazujDate{ get; set; }
        public string Rodzic { get; set; }
    }
}