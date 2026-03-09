using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using System;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.WalidatoryDanych.Klienci
{
    /// <summary>
    /// Waliduje czy kod kreskowy jest poprawny - unikalny lub pusty
    /// </summary>
    public class WalidatorMaila : IWalidatorDanych
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
            
            if (!string.IsNullOrEmpty(fraza))
            {
                fraza = fraza.Trim().ToLower();
                var klienci = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Klient>(null, x => (x.Email==null || x.Email!="") && x.Email.ToLower() == fraza).ToList();
                if (string.IsNullOrEmpty(identyfikatorObiektu))
                {
                    wynik = !klienci.Any();
                }
                else
                {
                    int k = (int)Convert.ChangeType(identyfikatorObiektu, typeof(int));
                    wynik = klienci.All(x => x.Id == k);
                }
                if (!wynik)
                {
                    komunikat = "Email jest już wykorzystany";
                }
            }

            return new WalidacaDanychWynik { Wynik = wynik, KomunikatBledu = komunikat };
        }
    }
}