using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Model.Enums
{
    /// <summary>
    /// Tryb otwierania linków url
    /// </summary>
    public enum TrybOtwierania
    {

        [FriendlyName("Nowe okno")]
        NoweOkno =0,
        [FriendlyName("Okno modalne")]
        Modal =1,
        [FriendlyName("To samo okno")]
        ObecneOkno =2
    }
}
