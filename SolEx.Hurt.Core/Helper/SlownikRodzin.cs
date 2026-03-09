using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikRodzin 
    {
        public static Dictionary<string, List<Produkt>> PobierzRodzinySlownik(List<Produkt> listaProduktow)
        {
            Dictionary<string, List<Produkt>> wynik = new Dictionary<string, List<Produkt>>();
            var produktyZRodzina = listaProduktow.Where(x => !string.IsNullOrEmpty(x.Rodzina));
            foreach (var produkty in produktyZRodzina)
            {
                if (wynik.ContainsKey(produkty.Rodzina))
                {
                    wynik[produkty.Rodzina].Add(produkty);
                }
                else
                {
                    wynik.Add(produkty.Rodzina, new List<Produkt>() { produkty });
                }
            }
            return wynik;
        }
    }
}
