using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikPanstwZSymbolem : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                foreach (var k in SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Kraje>(null).Where(x=>x.Widoczny).OrderBy(x=>x.Nazwa))
                {
                    if (string.IsNullOrEmpty(k.Symbol))
                    {
                        wynik.Add(k.Nazwa, k.Id);
                        continue;
                    }
                    wynik.Add(k.Symbol, k.Id);
                }
                return wynik;
            }
        }
    }
}
