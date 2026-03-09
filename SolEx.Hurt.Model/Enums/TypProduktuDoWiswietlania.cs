using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Model.Enums
{
    /// <summary>
    /// Poziom dostępu do elementu
    /// </summary>
    public enum TypProduktuDoWiswietlania
    {
        [FriendlyName("Produkty rodzinne")]
        ProduktyWRodzinieIds = 0,
        [FriendlyName("Zamienniki jednostronne")]
        ZamiennikiJednostrone = 1,
        [FriendlyName("Zamienniki dwustronne")]
        ZamiennikiDwustronne = 2,
        [FriendlyName("Wszystkie zamienniki")]
        Zamienniki = 3,
        
        [FriendlyName("Blog wybrana lista 1")]
        WybraneIdProduktow1 = 4,
        [FriendlyName("Blog wybrana lista 2")]
        WybraneIdProduktow2 = 5,
        [FriendlyName("Blog wybrana lista 3")]
        WybraneIdProduktow3 = 6
    }
}
