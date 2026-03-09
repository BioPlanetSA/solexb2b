using System.Collections.Generic;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;

namespace SolEx.Hurt.Core.Helper
{

    public class SlownikProduktow : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                foreach (var k in SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ProduktBazowy>(null,x => x.Widoczny))
                {
                    wynik.Add(string.Format("{0} ({1} - {2})", k.Nazwa, k.Kod, k.Id), k.Id);
                }
                return wynik;
            }
        }
    }
}
