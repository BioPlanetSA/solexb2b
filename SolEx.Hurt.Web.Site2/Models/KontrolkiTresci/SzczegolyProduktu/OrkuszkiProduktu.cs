using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.Bazowe;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.SzczegolyProduktu
{
    public class OrkuszkiProduktu : SzczegolyProduktuBaza, IKontrolkaOkruszkow
    {
        public override string Nazwa
        {
            get { return "Ścieżka do produktu"; }
        }

        public override string Akcja
        {
            get { return "Sciezka"; }
        }

        [WidoczneListaAdmin]
        [FriendlyName("Kategoria która mna być pokazana jako poprzedni element")]
        [Niewymagane]
        [PobieranieSlownika(typeof(SlownikTresci))]
       public  string SymbolStronaPoprzednia { get; set; }
    }

}