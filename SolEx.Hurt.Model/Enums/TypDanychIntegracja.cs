using System;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Model.Enums
{
    public enum TypDanychIntegracja
    {
        Produkty,
        Sklepy,
        [FriendlyName("Produkty do katalogu")]
        ProduktyKatalogDrukowanie,
        [FriendlyName("Szablon katalogu")]
        SzablonKatalogDrukowanie
    }
}