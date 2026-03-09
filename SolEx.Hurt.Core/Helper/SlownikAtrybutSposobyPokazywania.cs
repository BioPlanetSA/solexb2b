using System.Collections.Generic;
using SolEx.Hurt.Core.BLL;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikAtrybutSposobyPokazywania : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                List<string> widoki = PlikiDostep.PobierzInstancje.PobierzTypyFiltow();
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                foreach (string item in widoki)
                {
                    wynik.Add(string.Format("{0} [{1}]", item, item), item);

                }
                return wynik;
            }
        }
    }
}
