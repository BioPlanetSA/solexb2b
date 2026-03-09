using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.Bazowe;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.SzczegolyBloga
{
    public class OrkuszkiBloga : WpisBlogBaza, IKontrolkaOkruszkow
    {
     public override string Nazwa
        {
            get { return "Okruszki bloga"; }
        }

        public override string Kontroler
        {
            get { return "Blog"; }
        }

        public override string Akcja
        {
            get { return "Sciezka"; }
        }

        [WidoczneListaAdmin]
        [FriendlyName("Kategoria która mna być pokazana jako poprzedni element")]
        [Niewymagane]
        [PobieranieSlownika(typeof(SlownikTresci))]
        public string SymbolStronaPoprzednia { get; set; }
    }

}