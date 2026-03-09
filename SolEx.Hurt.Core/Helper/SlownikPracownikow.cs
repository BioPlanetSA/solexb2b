using System.Collections.Generic;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikPracownikow : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                foreach (Klient k in SolexBllCalosc.PobierzInstancje.Klienci.PobierzKlientowRoli(RoleType.Pracownik, null)) //todo: SesjaHelper.PobierzInstancje.OddzialID
                {
                    wynik.Add(string.Format("{0} - {1}", k.Nazwa, k.Email), k.Id);
                }
                return wynik;
            }
        }
    }
}
