using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikUstawienieWartosc : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                var ustawienia = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Ustawienie>(null).ToList();
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                foreach (var k in ustawienia)
                {
                    if (k.Wartosc == null)
                    {
                        k.Wartosc = "";
                    }
                    wynik.Add(string.Format("{0} - {1}",k.Nazwa,k.Opis), k.Symbol);
                }
                return wynik;
            }
        }
    }
}
