using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikJednostek: SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                foreach (var j in SolexBllCalosc.PobierzInstancje.JednostkaProduktuBll.PobierzJednostki( SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDPolski).Values)
                {
                    wynik.Add(string.Format("{0} [{1}]", j.Nazwa, j.Id), j.Id);
                }
                return wynik;
            }
        }
    }// public  Dictionary<int,ProduktyJednostki> PobierzWszystkieJednostkiProduktow()
}
