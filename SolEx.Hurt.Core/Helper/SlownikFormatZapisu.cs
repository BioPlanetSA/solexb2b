using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikFormatZapisu : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                foreach (var wartosc in Enum.GetValues(typeof(KatalogFormatZapisu)))
                {
                    wynik.Add(wartosc.ToString(), (int)wartosc);
                }
                return wynik;
            }
        }
    }
}
