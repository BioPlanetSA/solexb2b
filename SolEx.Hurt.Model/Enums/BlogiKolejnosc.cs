using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Model.Enums
{
    public enum BlogiKolejnosc
    {
        [FriendlyName("Losowa")]
        Losowa = 0,
        [FriendlyName("Od najm³odszych")]
        DataDodania = 1,
        [FriendlyName("Wzglêdem kolejnoœci")]
        Kolejnosc = 10
    }

    public enum BlogiSposobPokazaniaDaty
    {
        Brak = 0,
        PodTytulem = 1,
        JakoZnaczekNaZdjeciu = 10
    }
}