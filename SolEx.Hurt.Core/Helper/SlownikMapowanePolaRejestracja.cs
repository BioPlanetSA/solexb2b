using System.Collections.Generic;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikMapowanePolaRejestracja : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                foreach (var item in Refleksja.WygenerujParametryPol<Rejestracja>())
                {
                    wynik.Add(item.WyswietlanaNazwa, item.Nazwa);
                }
                return wynik;
            }
        }
        
    }
}
