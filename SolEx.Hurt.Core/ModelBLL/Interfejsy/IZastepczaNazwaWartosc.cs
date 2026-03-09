using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Core.ModelBLL.Interfejsy
{
    public interface IZastepczaNazwaWartosc
    {
        [FriendlyName("Tekst jeśli nie ma danych")]
        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Niewymagane]
        string TextZastepczy { get; set; }

        [FriendlyName("Nazwa pola (jesli wybierzemy nie pokazyj nazwy to ona się pojawi)")]
        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Niewymagane]
        string NazwaZastepcza { get; set; }
    }
}
