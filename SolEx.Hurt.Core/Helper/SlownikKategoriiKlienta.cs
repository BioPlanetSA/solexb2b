using System.Collections.Generic;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikKategoriiKlienta : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                foreach (var kategorie_Klientow in SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<KategoriaKlienta>(null))
                {
                    wynik.Add(kategorie_Klientow + " ["+ kategorie_Klientow.Id + "]", kategorie_Klientow.Id);
                }
                return wynik;
            }
        } 
    }
}
