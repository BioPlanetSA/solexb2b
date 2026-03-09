using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikGoogleTlumaczTrybyWyswietlania : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                if (wynik.Count == 0)
                {
                    wynik.Add("W pionie", "wpionie");
                    wynik.Add("W poziomie", "wpoziomie");
                    wynik.Add("Standardowe menu", "standardowe");
                }
                
                return wynik;
            }
        }
    }
}
