using System.Collections.Generic;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Core.Helper
{

    public class SlownikMagazynow : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                foreach (var k in SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Magazyn>(null))
                {
                    wynik.Add($"{k.Nazwa} [{k.Symbol}]", k.Id);
                }
                return wynik;
            }
        }
    }
}
