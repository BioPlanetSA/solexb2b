using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikKategoriiKlientaNazwa : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                foreach (var kategorie_Klientow in SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<KategoriaKlienta>(null))
                {
                    if (!wynik.Keys.Contains(kategorie_Klientow.Nazwa))
                    {
                        wynik.Add(kategorie_Klientow.Nazwa, kategorie_Klientow.Nazwa);
                    }
                }
                return wynik;
            }
        }
    }
}
