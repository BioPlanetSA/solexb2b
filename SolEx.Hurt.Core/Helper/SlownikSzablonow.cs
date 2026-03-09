using System;
using System.Collections.Generic;
using SolEx.Hurt.Core.BLL;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikSzablonow : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                throw new NotImplementedException();
                //Dictionary<string, object> wynik = new Dictionary<string, object>();
                //foreach (var sz in ImportyDostep.PobierzInstancje.Pobierz( SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDPolski))
                //{
                //    wynik.Add(string.Format("{0} - {1}", sz.nazwa, sz.id), sz.id);
                //}
                //return wynik;
            }
        }
    }
}
