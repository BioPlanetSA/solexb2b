using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikSzerokosciKolumny : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                for(int i =1; i<=12; i++)
                {
                    wynik.Add(i.ToString(),i);
                }
                return wynik;               
            }
        }
    }
}
