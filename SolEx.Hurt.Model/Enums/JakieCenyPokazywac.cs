using System;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Model.Enums
{
    public enum JakieCenyPokazywac
    {
        [FriendlyName("Brak")]
        Brak,
        [FriendlyName("Netto")]
        Netto,
        [FriendlyName("Brutto")]
        Brutto,
        [FriendlyName("Netto i Brutto")]
        NettoBrutto,
        [FriendlyName("Brutto i Netto")]
        BruttoNetto
    }
}
