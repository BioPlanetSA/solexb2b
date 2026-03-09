using System.Collections.Generic;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikKrajow : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                Dictionary<string,object> wynik = new Dictionary<string, object>();
                foreach (var r in SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Kraje>(null))
                {
                    wynik.Add($"{r.Nazwa} [{r.Id}]", r.Id);
                }
                return wynik;
            }
        }
    }
}
