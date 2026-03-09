using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikPolKlienta : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                Klient k = new Klient();
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                foreach (var propertyInfo in k.GetType().GetProperties())
                {
                    if (!wynik.Keys.Contains(propertyInfo.Name))
                    {
                        wynik.Add(propertyInfo.Name, propertyInfo.Name);
                    }
                }
                return wynik;
            }
        }
    }
}
