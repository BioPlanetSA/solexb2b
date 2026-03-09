using System.Collections.Generic;
using SolEx.Hurt.Core.BLL;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikJezykow : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                var opiekunowie =  SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykiWSystemie.Values;
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                foreach (var k in opiekunowie)
                {
                    wynik.Add(string.Format("{0} [{1}]", k.Nazwa, k.Id), k.Id);
                }
                return wynik;
            }
        }
    }
}
