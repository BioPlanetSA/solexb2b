using System.Collections.Generic;
using SolEx.Hurt.Core.BLL;
using System.IO;

namespace SolEx.Hurt.Core.Helper
{
    public abstract class SlownikPlikowBaza: SlownikBazowy
    {
        protected abstract string Sciezka { get; }
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                string sciezka = Path.Combine("Zasoby", Sciezka);
                foreach (var c in PlikiDostep.PobierzInstancje.PobierzPliki(Sciezka, null))
                {
                    wynik.Add(Path.GetFileName(c), Path.GetFileName(c));
                }
                return wynik;
            }
        }
    }
}