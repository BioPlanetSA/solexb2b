using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikPolProduktowTylkoOpisy : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                foreach (var k in (new Produkt()).GetType().GetProperties().Where(a => a.Name.StartsWith("opis")).ToArray())
                {
                    if (k.PropertyType != typeof(string)) { continue; }
                    if (wynik.ContainsKey(k.Name))
                    {
                        continue;
                    }
                    wynik.Add(k.Name, k.Name);

                }
                return wynik;
            }
        }
    }
}
