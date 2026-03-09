using System.Collections.Generic;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikPoziomuCen:SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                foreach (var item in SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<PoziomCenowy>(null))
                {
                    wynik.Add(string.Format("{0} [{1}]", item.Nazwa, item.Id), item.Id);
                }
                return wynik;
            }
        }
        
    }
}
