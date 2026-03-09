using System.Collections.Generic;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikPanstw : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                foreach (var k in SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Kraje>(null))
                {
                    if (k.Widoczny)
                    {
                        wynik.Add($"{k.Nazwa} [{k.Id}]", k.Id);
                    }
                }
                return wynik;
            }
        }
    }
}
