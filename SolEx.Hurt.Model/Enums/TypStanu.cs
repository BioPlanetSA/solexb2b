using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Model.Enums
{
    public enum TypStanu
    {
        [FriendlyName("Na stanie")]
        na_stanie = 1,
        [FriendlyName("Na wyczerpaniu")]
        na_wyczerpaniu = 4,
        [FriendlyName("Cykliczna dostawa")]
        cykliczna_dostawa = 8,
        [FriendlyName("W dostawie")]
        w_dostawie = 12,
        [FriendlyName("Na zamówienie")]
        na_zamowienie = 16,
        [FriendlyName("Dropshipping")]
        dropshiping = 20,
        [FriendlyName("Niedostępny dłuższy czas")]
        niedostepny_dluzszy_czas = 100,
        [FriendlyName("Brak")]
        brak = 200
    }
}