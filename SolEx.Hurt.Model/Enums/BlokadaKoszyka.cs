using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Model.Enums
{
    /// <summary>
    /// Dostepne tryby blokady koszyka
    /// </summary>
    public enum BlokadaKoszyka
    {
        [FriendlyName("Blokuj finalizacje gdy przekroczone są stany lub niedostępne")]
        BlokujGdyPrzekroczone =0,
        [FriendlyName("Nie blokuj finalizacji.")]
        NieBlokuj=1,
        [FriendlyName("Blokuj finalizacje gdy wszystkie pozycje są niedostępne")]
        BlokujGdyWszystkieNiedostepne=2
    }
}
