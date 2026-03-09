using System.Collections.Generic;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikPolStringowychProduktu : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                foreach (var k in (new Produkt()).GetType().GetProperties())
                {
                    if (k.PropertyType != typeof(string)) { continue; }
                    if (wynik.ContainsKey(k.Name))
                    {
                        continue;
                    }
                    wynik.Add(k.Name,k.Name);
                    //wynik.Add(string.Format("{0} - {1}", k.nazwa, k.email), k.klient_id);
                }
                return wynik;
            }
        }
    }
}
