using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.Hurt.Model.Interfaces
{
    /// <summary>
    /// Baza dla wszystkich walidatorów danych
    /// </summary>
    public interface IWalidatorDanych
    {
        /// <summary>
        /// Zwraca informację czy wpisane dane są poprawne
        /// </summary>
        /// <param name="identyfikatorObiektu">Klucz identyfikujacy obiekt dla którego przeprowadzamy test</param>
     
        /// <param name="testowane">Testowana fraza</param>
        /// <returns></returns>
        WalidacaDanychWynik Waliduj(string identyfikatorObiektu,  string testowane);
    }
}
