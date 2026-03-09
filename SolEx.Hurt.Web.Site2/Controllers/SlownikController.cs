using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Web.Site2.PageBases;
using Szukanie = SolEx.Hurt.Core.BLL.Szukanie;

namespace SolEx.Hurt.Web.Site2.Controllers
{
    [RoutePrefix("Slownik")]
    public class SlownikController : SolexControler
    {
        [Route("PobierzWartosci")]
        public ActionResult PobierzWartosci(string typ,string szukane)
        {
            Type t = Type.GetType(typ, true);
            ISlownik pobieralnie = (ISlownik)Activator.CreateInstance(t);
            var slownik = pobieralnie.PobierzWartosci();
            //javascript nie radzi sobie z przesyłaniem longów i dla tego lepiej przesyłąć stringi, tutaj robimy podmianę longa na stringa #10442
            if (slownik.First().Value is long)
            {
                slownik = slownik.ToDictionary(x => x.Key, x => (object)x.Value.ToString());
            }
            List<KeyValuePair<string, object>> wszystkie = slownik.Select(x => x).ToList();
            var znalezione = SolexBllCalosc.PobierzInstancje.Szukanie.WyszukajObiekty(wszystkie, szukane, new List<string> { "Key" });

            return PrzeksztalcNaJson(znalezione);
        }
       
    }
}