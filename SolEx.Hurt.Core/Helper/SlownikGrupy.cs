using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikGrupy : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                foreach (var item in SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<GrupaBLL>(null))
                {
                    wynik.Add(item.Nazwa, item.Id);
                }
                return wynik;
            }
        }
    }
}
