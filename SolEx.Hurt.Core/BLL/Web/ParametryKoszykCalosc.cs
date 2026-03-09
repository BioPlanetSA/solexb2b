using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.Web
{
    public class ParametryKoszykCalosc:IBindowalny
    {
        public ParametryKoszykCalosc(string url, bool opis, bool beka)
        {
            PokazujOpisPozycji = opis;
            PokazywacBelkeDostepnosci = beka;
            PoprzedniaStrona = url;
        }
        public ParametryKoszykCalosc() { }
        public bool PokazujOpisPozycji { get; set; }
        public bool PokazywacBelkeDostepnosci { get; set; }
        public bool PokazywacDateDodaniaDoKoszyka { get; set; }
        public string PoprzedniaStrona { get; set; }
        public string SzukanaFraza { get; set; }
        public string WybraneSortowanie { get; set; }
    }
}