using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    public abstract class RegulaKoszyka : ModulStowrzonyNaPodstawieZadania
    {
        [FriendlyName("Warunek ma działać odwrotnie")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool OdwrocenieWarunku { get; set; }
    }
}