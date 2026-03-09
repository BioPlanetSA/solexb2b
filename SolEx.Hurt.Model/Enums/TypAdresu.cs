using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Model.Enums
{
    public enum TypAdresu
    {
        [FriendlyName("Brak")]
        Brak = 0,
        [FriendlyName("Główny")]
        Glowny = 1,
        [FriendlyName("Jednorazowy")]
        Jednorazowy = 2,
        [FriendlyName("Domyślny")]
        Domyslny = 3,
        [FriendlyName("Korespondencyjny")]
        Korespondencyjny = 4,
        [FriendlyName("Wysyłki")]
        Wysylki = 5
    }
}
