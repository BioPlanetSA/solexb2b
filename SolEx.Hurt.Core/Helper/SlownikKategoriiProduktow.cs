using System.Collections.Generic;
using SolEx.Hurt.Core.BLL;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikKategoriiProduktow: SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                foreach (var kategoria in SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<KategorieBLL>(null,x=>x.Widoczna))
                {
                    wynik.Add(string.Format("{0} [{1}]", kategoria.Nazwa, kategoria.Id), kategoria.Id);
                }
                return wynik;
            }
        }
    }
}
