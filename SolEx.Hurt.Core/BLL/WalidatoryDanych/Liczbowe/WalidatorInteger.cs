using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.WalidatoryDanych.Liczbowe
{
    public class WalidatorInteger : IWalidatorDanych
    {
        /// <summary>
        /// Zwraca informację czy wpisane dane są poprawne
        /// </summary>
        /// <param name="identyfikatorObiektu">Klucz identyfikujacy obiekt dla którego przeprowadzamy test</param>

        /// <param name="fraza">Testowana fraza</param>
        public WalidacaDanychWynik Waliduj(string identyfikatorObiektu, string fraza)
        {
            string komunikat = "";
            int liczba;
            if (!string.IsNullOrEmpty(fraza))
            {
                if (!int.TryParse(fraza, out liczba))
                {
                    komunikat = "Wartość musi być liczbą";
                }
            }

            return new WalidacaDanychWynik { Wynik = string.IsNullOrEmpty(komunikat), KomunikatBledu = komunikat };
        }
    }
}