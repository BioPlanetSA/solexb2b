using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Linq;
using SolEx.Hurt.Core.BLL;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikKatalogZasoby : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {
                string klucz = "SlownikZasobowCache";
                Dictionary<string, object> wynik =
                    SolexBllCalosc.PobierzInstancje.Cache.PobierzObiekt<Dictionary<string, object>>(klucz);
                if (wynik == null)
                {
                    wynik = new Dictionary<string, object>();
                    var sciezka = HttpContext.Current.Server.MapPath(@"~/Zasoby/");
                    var katalogi = Directory.GetDirectories(sciezka, "*", SearchOption.AllDirectories).ToList();

                    List<string> wykluczenia = new List<string> {"cache", "imagecache", "thumbs"};
                    katalogi.RemoveAll(x => wykluczenia.Any(y => x.IndexOf(y,StringComparison.InvariantCultureIgnoreCase)>=0));
                    foreach (var kat in katalogi)
                    {
                        string tmpKat = kat.Replace(sciezka, "");
                        wynik.Add(tmpKat, tmpKat);
                    }
                    SolexBllCalosc.PobierzInstancje.Cache.DodajObiekt(klucz, wynik);
                }
                return wynik.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            }
        }
    }
}
