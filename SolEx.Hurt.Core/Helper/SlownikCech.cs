using System.Collections.Generic;
using SolEx.Hurt.Core.BLL;
namespace SolEx.Hurt.Core.Helper
{
    public class SlownikCech : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                foreach (var c in SolexBllCalosc.PobierzInstancje.CechyAtrybuty.PobierzWszystkieCechy( SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDPolski))
                {
                    wynik.Add(c.Value.Symbol, c.Value.Id);
                }
                return wynik;
            }
        }
    }
}
