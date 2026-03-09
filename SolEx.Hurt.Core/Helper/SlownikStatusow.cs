using System.Collections.Generic;
using SolEx.Hurt.Core.BLL;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikStatusow : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                foreach (var k in SolexBllCalosc.PobierzInstancje.Konfiguracja.StatusyZamowien)
                {
                    wynik.Add(string.Format("{0} [{1}]", k.Value.Nazwa, k.Key), k.Key);
                }
                return wynik;
            }
        }
    }
}
