using System;
using System.Collections.Generic;
using System.Web;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.Modules
{
    /// <summary>
    /// Zwraca listę tłumaczeń w aktualnie wybranym języku
    /// </summary>
   public class PobierzTlumaczenia : WebSiteBaseHandler
    {
        public override void HandleRequest(HttpContext context)
        {
            string frazy = context.Request["frazy"];
            Dictionary<string,string> wynik=new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(frazy))
            {
                string[] pola = frazy.Split(new [] {";"}, StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in pola)
                {
                    if (!wynik.ContainsKey(s))
                    {
                        wynik.Add(s,SolexBllCalosc.PobierzInstancje.Konfiguracja.PobierzTlumaczenie(1, s));
                    }
                }
            }
            SendJson(context, wynik);
        }
    }
}
