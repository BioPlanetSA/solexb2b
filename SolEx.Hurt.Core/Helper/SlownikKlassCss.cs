using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikKlassCss : SlownikBazowy
    {
        protected override Dictionary<string, object> WygenerujSlownik
        {
            get
            {

                Dictionary<string, object> wynik = new Dictionary<string, object>();
                var sciezka1 = HttpContext.Current.Server.MapPath(@"~/Layout/css/main/CustomCSS.scss");
                var sciezka2 = HttpContext.Current.Server.MapPath(@"~/Layout/css/main/_wspolneCustomCSS.scss");

                if (!File.Exists(sciezka1) || !File.Exists(sciezka2))
                {
                    return new Dictionary<string, object>();
                }
                string zawartosc = " " + File.ReadAllText(sciezka1) + " " + File.ReadAllText(sciezka2); 

                //string strRegex = @"{(((?<Counter>{)*[^{}]*)*((?<-Counter>})*[^{}]*)*)*(?(Counter)(?!))}";  //REGEX Mateusza z neta - on nie dziala dobrze
                string strRegex = @"[\s]([\.][_A-Za-z0-9\-]+)";
                Regex myRegex = new Regex(strRegex, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant);
                foreach (Match o in myRegex.Matches(zawartosc))
                {
                        string wartosc = o.Value.Trim().TrimStart('.');
                        if (!wynik.ContainsKey(wartosc))
                        {
                            wynik.Add(wartosc, wartosc);
                        }
                }
                return wynik;
            }
        }
    }
}
