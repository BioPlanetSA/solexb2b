using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Core.ModelBLL.Interfejsy
{
    public interface IPrefixSofix
    {
        [FriendlyName("Tekst przed")]
        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Niewymagane]
        string Prefix { get; set; }

        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Niewymagane]
        [FriendlyName("Test za")]
        string Sofix { get; set; }
    }
}
