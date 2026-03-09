using System.Collections.Generic;
using SolEx.Hurt.Core.BLL;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikWalut : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {    
                var wynik = new Dictionary<string, object>();
                foreach (var item in SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Model.Waluta>(null))
                {
                    if (!wynik.ContainsKey(item.WalutaB2b))
                    {
                        wynik.Add(item.WalutaB2b, item.Id);
                    }
                }
                return wynik;
            }
        }

    }
}
