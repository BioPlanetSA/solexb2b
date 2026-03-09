using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using System;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.WalidatoryDanych.Tresc
{
    /// <summary>
    /// Waliduje czy kod kreskowy jest poprawny - unikalny lub pusty
    /// </summary>
    public class WalidatorSymbol : IWalidatorDanych
    {
        /// <summary>
        /// Zwraca informację czy wpisane dane są poprawne
        /// </summary>
        /// <param name="identyfikatorObiektu">Klucz identyfikujacy obiekt dla którego przeprowadzamy test</param>

        /// <param name="fraza">Testowana fraza</param>
        public WalidacaDanychWynik Waliduj(string identyfikatorObiektu, string fraza)
        {
            bool wynik = true;
            string komunikat = "";
            string testowanaFraza = fraza;
            if (!string.IsNullOrEmpty(fraza))
            {
                testowanaFraza = fraza.Trim();
            }
            if (!string.IsNullOrEmpty(testowanaFraza))
            {
                var produkty = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<TrescBll>(null, x => !string.IsNullOrEmpty(x.Symbol) && x.Symbol.Equals(testowanaFraza, StringComparison.InvariantCultureIgnoreCase)).ToList();
                if (string.IsNullOrEmpty(identyfikatorObiektu))
                {
                    wynik = !produkty.Any();
                }
                else
                {
                    int k = (int)Convert.ChangeType(identyfikatorObiektu, typeof(int));
                    wynik = produkty.All(x => x.Id == k);
                }
            }
            if (!wynik)
            {
                komunikat = "Istnieje już treść z takim linkiem";
            }
            return new WalidacaDanychWynik { Wynik = wynik, KomunikatBledu = komunikat };
        }
    }
}