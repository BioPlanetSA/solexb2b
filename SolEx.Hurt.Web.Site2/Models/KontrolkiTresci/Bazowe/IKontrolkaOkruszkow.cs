using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.Bazowe
{
    public interface IKontrolkaOkruszkow
    {
        [WidoczneListaAdmin]
        [FriendlyName("Kategoria która mna być pokazana jako poprzedni element")]
        [Niewymagane]
        [PobieranieSlownika(typeof(SlownikTresci))]
        string SymbolStronaPoprzednia { get; set; }
    }
}
