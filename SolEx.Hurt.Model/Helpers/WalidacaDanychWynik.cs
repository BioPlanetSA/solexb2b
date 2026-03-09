namespace SolEx.Hurt.Model.Helpers
{
    /// <summary>
    /// Obiekt reprezentujący wynik walidacji danych
    /// </summary>
    public class WalidacaDanychWynik
    {
        /// <summary>
        /// Informacja czy dane są poprawne
        /// </summary>
        public bool Wynik { get; set; }
        /// <summary>
        /// Komunikat błęd, ma znaczenie tylko jeśli wynik jest False
        /// </summary>
        public string KomunikatBledu { get; set; }
    }
}
