using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Core.BLL.WarunkiRegulPunktow
{
    public abstract class WarunekRegulyPunktowej : ModulStowrzonyNaPodstawieZadania
    {
        [FriendlyName("Warunek ma działać odwrotnie")]
        public bool OdwrocenieWarunku { get; set; }
    }
}