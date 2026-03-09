using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Core.Helper
{
    class SlownikSlajdow : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Slajd>(null).ToDictionary(x => string.Format("{0} - {1}", x.Nazwa, x.Id), x => (object)x.Id);
            }
        }
    }
}
