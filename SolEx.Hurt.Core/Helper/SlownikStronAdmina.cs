using System.Collections.Generic;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikStronAdmina : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                //foreach (var item in CoreManager.WszystkieStronyAdmina())
                //{
                //    wynik.Add(string.Format("{0} > {1}", item.Grupa, item.Tytul), item.NazwaModuluWykrywanie);
                //}
                return wynik;
            }
        }
        
    }
}
