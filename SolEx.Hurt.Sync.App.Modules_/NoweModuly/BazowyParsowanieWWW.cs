using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly
{
    public abstract class BazowyParsowanieWWW : SyncModul
    {
        [FriendlyName("Adres do pobierania danych")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string AdresHttp { get; set; }

        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string AdresLogowaniaHttp { get; set; }

        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Login { get; set; }
        
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Haslo { get; set; }

         private CookieContainer _cookies = new CookieContainer();

        public HtmlDocument PobierzStrone()
        {
            HtmlAgilityPack.HtmlWeb web = new HtmlWeb();

            NetworkCredential logowanie = null;
            if (!string.IsNullOrEmpty(Login))
            {
                logowanie = new NetworkCredential(Login, Haslo);
            }

            web.UseCookies = true;

            if (!string.IsNullOrEmpty(AdresLogowaniaHttp))
            {
                string adresWywolania = string.Format(AdresLogowaniaHttp, Login, Haslo);
                //HttpWebRequest request = (HttpWebRequest) WebRequest.Create(adresWywolania); 
                //request.Method = "POST";

                //request.CookieContainer = _cookies;

                //HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //OK psd="" PL

                web.PreRequest += DodajCookisy;

                web.PostResponse += PostResponse;

                var zwrot = web.Load(adresWywolania, "POST", null, null);
               
            }

            HtmlAgilityPack.HtmlDocument doc = web.Load(AdresHttp, "GET", null, logowanie);
           
            return doc;
        }

        private void PostResponse(HttpWebRequest request, HttpWebResponse response)
        {
            _cookies = request.CookieContainer;
        }

        private bool DodajCookisy(HttpWebRequest request)
        {
            request.CookieContainer = _cookies;
            return true;
        }
    }
}
