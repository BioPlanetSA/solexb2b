using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Model.Enums
{
    public enum KatalogKlientaTypy
    {
        [FriendlyName("Wykluczenie",FriendlyOpis = "wykluczenie z oferty produktu dla klienta (jelsi np. domyslnie wszystkei produkty sa odkryte)")]
        Wykluczenia,
        [FriendlyName("Dostepny", FriendlyOpis = "oznacza odkrycue produktow dla klienta (np. jesli domyslnie wsystkie produkty sa ukryte)")]
        Dostepne,
        [FriendlyName("Mój katalog")]
        MojKatalog
    }
}
