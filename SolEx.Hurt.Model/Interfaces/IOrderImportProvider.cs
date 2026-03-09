using System.Collections.Generic;

namespace SolEx.Hurt.Model.Interfaces
{
    public interface IOrderImportProvider
    {
        /// <summary>
        /// Zwraca listę produktów i ilości odczytanych z pliku
        /// </summary>
        /// <param name="data">Kod produktu, ilość</param>
        /// <returns>Lista produktów. Null jeśli błąd</returns>
        List<KeyValuePair<string, decimal>> GetProductsToImport(string data);

        string SearchField { get; }
    }
}
