using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikGrupyKategoriiKlienta:SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                foreach (var kategorie_Klientow in SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<KategoriaKlienta>(null))
                {
                    if (!wynik.Keys.Contains(kategorie_Klientow.Grupa))
                    {
                        wynik.Add(kategorie_Klientow.Grupa, kategorie_Klientow.Grupa);
                    }
                }
                return wynik;
            }
        }
    }
}
