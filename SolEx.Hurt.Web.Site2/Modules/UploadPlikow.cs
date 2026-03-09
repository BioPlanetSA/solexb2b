using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Policy;
using System.Web;
using ServiceStack.ServiceClient.Web;
using SolEx.Hurt.Core;
using SolEx.Hurt.Web.Site2.Helper;
using System.Collections.Generic;

namespace SolEx.Hurt.Web.Site2.Modules
{
    class Wynik
    {
        public string Blad { get; set; }
        public string Sciezka { get; set; }
         public Wynik(string sciezka, string blad="")
         {
             Blad = blad;
             Sciezka = sciezka;
         }
    }

    public class UploadPlikow : WebSiteBaseHandler
    {

        public static string PobierzSciezke(long klient)
        {
            string sub = Podkatalog(klient);
            string katalog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,  "/Zasoby/upload/".TrimStart('/')).Replace("/", "\\");
            if (!Directory.Exists(katalog))
            {
                Directory.CreateDirectory(katalog);
            }
            string cala = Path.Combine(katalog, sub);
            if (!Directory.Exists(cala))
            {
                Directory.CreateDirectory(cala);
            }
            return cala;
        }
        private static string Podkatalog(long id)
        {
            return DateTime.Now.ToString("ddMMyyyyhhmmss") + id.ToString(CultureInfo.InvariantCulture);
        }
        public override void HandleRequest(HttpContext context)
        {
            long klientID = SolexHelper.PobierzInstancjeZCache().AktualnyKlient.Id;
            string katalog = PobierzSciezke(klientID);
            string wynik = "";
            foreach (string klucz in context.Request.Files)
            {
                HttpPostedFile x = context.Request.Files[klucz];
                if (x != null)
                {
                    string pelnasciezka = Path.Combine(katalog, x.FileName);
                    x.SaveAs(pelnasciezka);
                    wynik = "/zasoby/upload/" + Podkatalog(klientID) + "/" + x.FileName;
                    break;
                }
            }
            SendJson(context, new Wynik(wynik));
        }

        /// <summary>
        /// Metoda do zapisu plików na serwerze
        /// </summary>
        /// <param name="plik">HttpPostedFileBase</param>
        /// <param name="sciezka">Scieżka zawierająca nazwę pliku do zapisu na serwerze</param>
        /// <param name="oczekiwanaNazwa">Oczekiwana nazwa pliku</param>
        public string ZapiszPlik(HttpPostedFileBase plik, string sciezka, string oczekiwanaNazwa="")
        {
            if (string.IsNullOrEmpty(sciezka))
            {
                throw new Exception("Sciezka zapisu pliku użytkownika nie została podana dla pliku: "+plik.FileName);
            }
            if (plik != null && plik.ContentLength > 0)
            {
                var katalog = sciezka.Substring(0, sciezka.LastIndexOf("\\", StringComparison.InvariantCultureIgnoreCase));
                if (!Directory.Exists(katalog))
                {
                    Directory.CreateDirectory(katalog);
                }

                if (!string.IsNullOrEmpty(oczekiwanaNazwa))
                {
                    sciezka = sciezka.Replace(plik.FileName, oczekiwanaNazwa);
                }
                plik.SaveAs(sciezka);
                return oczekiwanaNazwa;
            }
            return null;
        }
    }
}
