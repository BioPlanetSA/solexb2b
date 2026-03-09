using System.Collections.Generic;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikKategoriiSklepow : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                foreach (KategoriaSklepu kategorie_Klientow in SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<KategoriaSklepu>(null))
                {
                    wynik.Add(string.Format("{0} - {1}", kategorie_Klientow.Nazwa, kategorie_Klientow.Id), kategorie_Klientow.Id);
                }
                return wynik;
            }
        }

        
    }
}
