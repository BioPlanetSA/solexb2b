using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public interface ISposobDostawy : IGrupaZadania, IOpisModulu
    {
        ProduktBazowy ProduktDostawy { get; }

        [FriendlyName("Cena netto dostawy")]
        decimal WyliczCene(IKoszykiBLL koszyk);

        [FriendlyName("Symbol towaru")]
        string SymbolProduktu { get; set; }

        [FriendlyName("Opis dostawy")]
        [Niewymagane]
        [Lokalizowane]
        string OpisDostawy { get; set; }

        [FriendlyName("Gdzie wyświetlać komunikat")]
        PokazywanieKomunikatu KomunikatPozycja { get; set; }

        [FriendlyName("Komunikat")]
        [Niewymagane]
        [Lokalizowane]
        string Komunikat { get; set; }

        /// <summary>
        /// Propertis określający czy był błąd podczas pobierania sposoby dostawy wykorzystywany np przy pobieraniu ceny wysyłki z Upsa
        /// </summary>
        [FriendlyName("Czy podczas pobierania sposobu dostawy wystapił błąd")]
        bool CzyWystapilBlad { get; set; }
    }
}