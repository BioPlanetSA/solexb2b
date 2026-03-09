using System.Collections.Generic;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikStatusowDokumentow : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                foreach (StatusZamowienia status in SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<StatusZamowienia>(null))
                {
                    wynik.Add(string.Format("{0} [{1}]", status.Nazwa, status.Id), status.Id);
                }
                return wynik;
            }
        }
    }
}
