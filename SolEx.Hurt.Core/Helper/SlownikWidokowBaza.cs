using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.BLL;

namespace SolEx.Hurt.Core.Helper
{
    public abstract class SlownikWidokowBaza: SlownikBazowy
    {
        protected abstract string SciezkaWidokow { get; }
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                foreach (var c in PlikiDostep.PobierzInstancje.PobierzWidokiZKatalogu(SciezkaWidokow))
                {
                    wynik.Add(c, c);
                }
                if (DodatkoweWidoki != null && DodatkoweWidoki.Any())
                {
                    foreach (var o in DodatkoweWidoki)
                    {
                        if (!wynik.ContainsKey(o.Key))
                        {
                            wynik.Add(o.Key,o.Value);
                        }
                    }
                }
                return wynik;
            }
        }

        protected virtual Dictionary<string,string> DodatkoweWidoki { get; set; }
    }
}