using System.Collections.Generic;
using System.Globalization;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikKultur:SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
               // List<string> wynik = new List<string>();
                Dictionary<string,object> wynik = new Dictionary<string, object>();
                foreach (var kultura in CultureInfo.GetCultures(CultureTypes.AllCultures))
                {
                    if (!wynik.ContainsKey(kultura.Name))
                    {
                        wynik.Add(kultura.Name,kultura.Name);
                    }
                }
                return wynik;
            }
        }
    }
}
