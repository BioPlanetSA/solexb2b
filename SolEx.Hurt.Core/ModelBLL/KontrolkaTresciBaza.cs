using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using Newtonsoft.Json;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.ModelBLL
{
    public class KontrolkaUsunieta : KontrolkaTresciBaza
    {
        public KontrolkaUsunieta()
        {
            

        }

        private string _kontrolka;
        public KontrolkaUsunieta(string kontrolka)
        {
            _kontrolka = kontrolka;
        }
        [WidoczneListaAdmin(true,true,false,false)]       
        public string Tresc { get; set; }
        public override string Nazwa
        {
            get { return "Kontrolka zastępcza dla usuniętych kontrolek. Zastępuje " + _kontrolka; }
        }

        public override string Kontroler
        {
            get { return "Tresci"; }
        }

        public override string Akcja
        {
            get { return "Tekst"; }
        }

        public override string Grupa
        {
            get { return "Wygląd"; }
        }
    }

    public abstract class KontrolkaTresciBaza : IKontrolkaTresciBaza
    {
        public IKlient AktualnyKlient { get; set; }

        /// <summary>
        /// Pobiera identyfikator obiektu
        /// </summary>
        /// <param name="symbol">Klucz obiektu</param>
        /// <param name="zwracajblad">Czy wyrzucać wyjątek jeśli nie znaleziono</param>
        /// <returns></returns>
        protected object PobierzIdentyfikator(string symbol, bool zwracajblad, string model ="")
        {
            //nie wiem po co takie zabezpieczenie zakomentowane w ramach taska 9048
            //if (HttpContext.Current.Request.Url.AbsoluteUri.Contains("Admin"))
            //{
            //    return null;
            //}
            var context = HttpContext.Current.Request.RequestContext.RouteData.Values;
            object value;
            if (!context.ContainsKey(symbol))
            {
                value = HttpContext.Current.Request.Params[symbol];
                if (value == null)
                {
                    if (zwracajblad)
                    {
                        throw new BrakIdentyfikatoraException(model, symbol);
                    }
                    return null;
                }
                //nusiałem dodać deserializacje ponieważ id Kategorii przekazywane jest jako hashset a nie jako long 
                if (value.ToString().StartsWith("["))
                {
                   //json
                    value = ((HashSet<long>)JsonConvert.DeserializeObject(value.ToString(), typeof(HashSet<long>))).FirstOrDefault();
                }
                return value;
            }
            value = context[symbol];
            return value;
        }

        public int Id { get; set; }

        [NieWysylajParametryDoAkcji]
        public abstract string Nazwa { get;  }

        [NieWysylajParametryDoAkcji]
        public virtual string Grupa { get { return "Pozostałe"; } }

        [NieWysylajParametryDoAkcji]
        public virtual string Opis { get { return ""; } }

        [NieWysylajParametryDoAkcji]
        public virtual string Ikona
        {
            get { return "fa-file-text-o"; }
        }
        [NieWysylajParametryDoAkcji]
        public abstract string Kontroler { get;}

        [NieWysylajParametryDoAkcji]
        public abstract string Akcja { get; }

        [Niewymagane]
        [WidoczneListaAdmin]
        [FriendlyName("Nazwa pojemnika")]
        [GrupaAtttribute("Zawijanie", 3)]
        public string AcordionNazwa { get; set; }

        [WidoczneListaAdmin]
        [FriendlyName("Domyślne zwinięty")]
        [GrupaAtttribute("Zawijanie", 3)]
        public bool AcordionZwiniety { get; set; }

        public RouteValueDictionary Parametry()
        {
            RouteValueDictionary wynik = new RouteValueDictionary();
             var props= GetType().GetProperties();
             foreach (var p in props)
             {                
                 if (p.GetCustomAttributes(typeof(NieWysylajParametryDoAkcjiAttribute), true).Any() )
                 {
                     continue;
                 }

                 object val = p.GetValue(this);
                 if (string.IsNullOrEmpty(val?.ToString()))
                 {
                     continue;
                 }
                 wynik.Add(p.Name, val);
            }
            return wynik;
        }

        public bool RecznieDodany()
        {
            return true;
        }
        
        public Model.TrescKolumna DomyslneWartosciDlaNowejKontrolki;

    }

    public class NieWysylajParametryDoAkcjiAttribute : Attribute
    {
    }
}
