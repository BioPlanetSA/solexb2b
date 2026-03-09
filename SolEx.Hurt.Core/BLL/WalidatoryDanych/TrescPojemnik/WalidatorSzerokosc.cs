using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.WalidatoryDanych.TrescPojemnik
{
    /// <summary>
    /// Waliduje czy kod kreskowy jest poprawny - unikalny lub pusty
    /// </summary>
    public class WalidatorSzerokosc : IWalidatorDanych
    {
        /// <summary>
        /// Zwraca informację czy wpisane dane są poprawne
        /// </summary>
        /// <param name="identyfikatorObiektu">Klucz identyfikujacy obiekt dla którego przeprowadzamy test</param>

        /// <param name="fraza">Testowana fraza</param>
        public WalidacaDanychWynik Waliduj(string identyfikatorObiektu, string fraza)
        {
            string komunikat = "";
            int szerokosc;
            if (!int.TryParse(fraza, out szerokosc))
            {
                komunikat = "Nie wpisano liczby";
            }
            else
            {
                if (szerokosc < 1 || szerokosc > 12)
                {
                    komunikat = "Szerokość musi być z przedziału 1 do 12";
                }
            }

            return new WalidacaDanychWynik { Wynik = string.IsNullOrEmpty(komunikat), KomunikatBledu = komunikat };
        }
    }
}