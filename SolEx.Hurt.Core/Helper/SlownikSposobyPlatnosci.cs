using System.Collections.Generic;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikSposobyPlatnosci:SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                var zad = SolexBllCalosc.PobierzInstancje.ZadaniaBLL.PobierzZadania<ISposobPlatnosci, ISposobPlatnosci>(SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDPolski, null);
                foreach (var tmp in zad)
                {
                    wynik.Add($"{tmp.Nazwa} [{tmp.Id}]", tmp.Id);

                }
                return wynik;
            }
        }
    }
}
