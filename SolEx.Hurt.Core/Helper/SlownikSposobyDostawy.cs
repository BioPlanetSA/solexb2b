using System.Collections.Generic;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikSposobyDostawy:SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                var zad = SolexBllCalosc.PobierzInstancje.ZadaniaBLL.PobierzZadania<ISposobDostawy, ISposobDostawy>(SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDPolski, null);
                foreach (var tmp in zad)
                {
                    wynik.Add(string.Format("{0} [{1}]", tmp.OpisDostawy, tmp.Id), tmp.Id);
                }
                return wynik;
            }
        }
    }
}
