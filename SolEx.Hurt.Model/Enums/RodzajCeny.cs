using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Model.Enums
{
    public enum RodzajCeny
    {

        [FriendlyName("Cena detaliczna")]
        CenaDetaliczna = 1,
        [FriendlyName("Cena przed rabatem")]
        CenaPrzedRabatem = 2,
        [FriendlyName("Cena po rabacie")]
        CenaPoRabacie = 3,
        [FriendlyName("Cena w wybranym poziomie cenowym")]
        CenaWWybranymPoziomieCenowym = 4,
        [FriendlyName("Cena po rabacie bez przeliczenia (w walucie oryginalnej)")]
        CenaPoRabacieBezPrzeliczeniaWaluty = 5,
    }
}
