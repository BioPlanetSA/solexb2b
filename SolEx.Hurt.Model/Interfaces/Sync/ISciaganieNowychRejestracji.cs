namespace SolEx.Hurt.Model.Interfaces.Sync
{
    /// <summary>
    /// Interfejs systemu księgowego umożliwoający zapisanie klientów do systemu magazynowgo
    /// </summary>
    public interface ISciaganieNowychRejestracji
    {
       /// <summary>
       /// Dodaje klientów do systemu magazynowego
       /// </summary>
       /// <param name="klienci">Lista klientów do dodania</param>
       /// <returns>Dodani klienci</returns>
       Rejestracja ImportKlientowDoERP(Rejestracja klienci);
    }
}
