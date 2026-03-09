using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Model.Interfaces
{
    public interface IObiektWidocznyDlaOkreslonychGrupKlientow: IHasLongId
    {
        [WidoczneListaAdmin]
        [FriendlyName("Widoczność")]
        [WymuszonyTypEdytora(TypEdytora.WidocznoscDlaKlientow)]
        [Ignore]
        WidocznosciTypow Widocznosc { get; set; }
    }
}
