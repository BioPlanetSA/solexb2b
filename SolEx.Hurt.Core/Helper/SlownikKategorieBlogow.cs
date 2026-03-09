using System.Collections.Generic;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikKategorieBlogow : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                IList<BlogKategoria> ojcowie = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<BlogKategoria>(null);
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                foreach (var k in ojcowie)
                {
                    wynik.Add(string.Format("{0} [{1}]", k.Nazwa, k.Id), k.Id);
                }
                return wynik;
            }
        }
    }
}
