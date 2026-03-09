using SolEx.Hurt.Model;

namespace SolEx.Hurt.Web.Site2.UI
{
    /// <summary>
    /// Reprezentuje zakładkę popupa produktu, każda kontrolka na popupie dziedzicząca po tym interfejsie, automatycznie ma wypełnione oba pola
    /// </summary>
    public interface IPopupTab
    {
        /// <summary>
        /// Obiekt produktu odczytany z flata
        /// </summary>
        Produkt FlatProduct{get;set;}
        /// <summary>
        /// Obiekt produktu odczytany na podstawie tabeli produktów
        /// </summary>
        Produkt BaseProduct{get;set;}
    }
}
