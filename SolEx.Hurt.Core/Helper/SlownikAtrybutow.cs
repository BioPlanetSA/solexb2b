using System.Collections.Generic;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikAtrybutow:SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                foreach (AtrybutBll atrybut in SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<AtrybutBll>(null))
                {
                    wynik.Add(string.Format("{0} [{1}]", atrybut.Nazwa, atrybut.Id), atrybut.Id);
                }
                return wynik;
            }
        }
    }
}
