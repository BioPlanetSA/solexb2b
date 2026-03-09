using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Web.Site2.Helper
{
    public class SlownikElementowMenu : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                var k = SolexHelper.PobierzInstancjeZCache().AktualnyKlient;

                var elementy = AdminHelper.PobierzInstancje.PobierzMenu(k);
                Dictionary<string, object> wynik = new Dictionary<string, object>();
                foreach (var e in elementy)
                {
                    string nazwa = e.Nazwa;
                    if (e.Dzieci == null || !e.Dzieci.Any())
                    {
                        wynik.Add(nazwa, nazwa);
                        continue;
                    }
                    foreach (var d in e.Dzieci)
                    {
                        string linkMd5 = Tools.PobierzInstancje.GetMd5Hash(d.Link);
                        string n = string.Format("{0}>{1}", nazwa, d.Nazwa);
                        wynik.Add(n, linkMd5);
                    }
                }
                return wynik;
            }
        }
    }
}