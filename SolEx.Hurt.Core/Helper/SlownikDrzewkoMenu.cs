using System.Collections.Generic;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikDrzewkoMenu : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                IList<TrescBll> ojcowie = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<TrescBll>(null,x=>!string.IsNullOrEmpty(x.Symbol));
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                foreach (var k in ojcowie)
                {
                    wynik.Add(string.Format("{0} [{1}]", k.Nazwa, k.Symbol), k.Symbol);
                }
                return wynik;
            }
        }
    }
}
