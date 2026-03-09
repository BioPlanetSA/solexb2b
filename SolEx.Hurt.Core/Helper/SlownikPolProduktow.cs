using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikPolProduktow : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                Produkt p = new Produkt();
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                foreach (var propertyInfo in p.GetType().GetProperties())
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
