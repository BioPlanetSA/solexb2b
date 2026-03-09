using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SolEx.Hurt.Core.BLL;

namespace SolEx.Hurt.Web.Site2.App_Start
{
    public class WidokiWeWlasnymKataloguKlienta: RazorViewEngine
    {
        public WidokiWeWlasnymKataloguKlienta()
        {
            string sciezka = SolEx.Hurt.Core.BLL.SolexBllCalosc.PobierzInstancje.Konfiguracja.SzablonNiestandardowyNazwa;
            if (!string.IsNullOrEmpty(sciezka))
            {
                List<string> wlf = new List<string>(ViewLocationFormats);
                wlf.Insert(0,string.Format("~/templates/{0}/views/{{1}}/{{0}}.cshtml", sciezka));
                ViewLocationFormats = wlf.ToArray();
                List<string> pwf = new List<string>(PartialViewLocationFormats);
                pwf.Insert(0, string.Format("~/templates/{0}/views/{{1}}/{{0}}.cshtml", sciezka));
                PartialViewLocationFormats = pwf.ToArray();

                List<string> mlf = new List<string>(MasterLocationFormats);
                mlf.Insert(0, string.Format("~/templates/{0}/views/{{1}}/{{0}}.cshtml", sciezka));
                MasterLocationFormats = pwf.ToArray();
            }
            
        }

    }
}