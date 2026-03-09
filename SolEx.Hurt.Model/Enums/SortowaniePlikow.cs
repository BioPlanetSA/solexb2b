using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Model.Enums
{
    public enum SortowaniePlikow
    {
        [FriendlyName("Nazwa rosn¹co")]
        NazwaAsc = 0,
        [FriendlyName("Nazwa malej¹co")]
        NazwaDesc = 1,
        [FriendlyName("Data rosn¹co")]
        DataAsc = 2,
        [FriendlyName("Data malej¹co")]
        DataDesc = 3,
        [FriendlyName("Pierwszy element")]
        PierwszyElement =4,
        [FriendlyName("Ostatni element")]
        OstatniEement=5,
        [FriendlyName("Brak")]
        Brak=6,
        [FriendlyName("Data utworzenia rosn¹co")]
        DataUtworzeniaAsc =8,
        [FriendlyName("Data utworzenia malej¹co")]
        DataUtworzeniaDesc =9
    }
}