using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikNieKlientow : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                var opiekunowie = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Klient>(null,y => y.Role.Count(x => x != RoleType.Klient) > 0);
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
