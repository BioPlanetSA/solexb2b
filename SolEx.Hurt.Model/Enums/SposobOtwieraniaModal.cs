

using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Model.Enums
{
    public enum SposobOtwieraniaModal
    {
        [FriendlyName("Okno modalne zalogowani, okno modalne niezalogowani")] 
        ZalogowaniModalNiezalogowaniModal,
        [FriendlyName("Okno modalne zalogowani, nowa strona niezalogowani")] 
        ZalogowaniModalNiezalogowaniNieModal,
        [FriendlyName("Nowa strona zalogowani, nowa strona niezalogowani")] 
        ZalogowaniNieModalNiezalogowaniNieModal,
        [FriendlyName("Nowa strona zalogowani, okno modalne niezalogowani")]
        ZalogowaniNieModalNiezalogowaniModal

    }
}
