using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Model.Enums
{
    /// <summary>
    /// Tryby tworzenia cech nadrzędnych
    /// </summary>
    public enum TrybPokazywaniaFiltrow
    {
        [FriendlyName("Wymuszaj ścieżkę wyboru konfiguratora po kolei")]
        WymuszajSciezke=1,
        [FriendlyName("Wszystkie możliwe atrybuty jednocześnie do wyboru")]
        WszystkieAtrybutyJednoczesnie=2
    }
    public enum SposoobSprawdzeniaFiltrow
    {
        [FriendlyName("Wybrany przynajmniej jeden")]
        PrzynajmniejJeden = 1,
        [FriendlyName("Wszystkie atrybuty muszą być wybrane")]
        Wszystkie = 10
    }
}
