using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Web.Site2.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Newtonsoft.Json;
using ServiceStack.Common;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Web.Site2.Models.DaneDoWidokow.Filtry;
using SolEx.Hurt.Web.Site2.Models.DaneDoWidokow.Koszyk;
using SolEx.Hurt.Web.Site2.Models.DaneDoWidokow.ProfilKlienta;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.Hurt.Web.Site2.Helper
{
    public static class UrlExtender
    {
        public static IConfigBLL Config = SolexBllCalosc.PobierzInstancje.Konfiguracja;
        public static ISolexBllCalosc Calosc = SolexBllCalosc.PobierzInstancje;

        public static string LinkAbsolutny(this UrlHelper urlHelper, string plikSciezkaLokalna)
        {
            // string path = urlHelper.Content(plikSciezkaLokalna);
            var url = new Uri(HttpContext.Current.Request.Url, plikSciezkaLokalna);

            return url.AbsoluteUri;
        }

        /// <summary>
        /// Zmienia linki w podanej tresci z wzglednych na bezwgledne
        /// </summary>
        /// <param name="urlHelper"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string ZmienLinkiWzgledneNaAbsolutneWTresci(this UrlHelper urlHelper, string message)
        {
            string adresPlatformy = LinkAbsolutny(urlHelper, "/");

            message = message.Replace("\"../Zasoby/", "\"" + adresPlatformy + "Zasoby/", StringComparison.InvariantCultureIgnoreCase);
            message = message.Replace("\"/Zasoby/", "\"" + adresPlatformy + "Zasoby/", StringComparison.InvariantCultureIgnoreCase);
            message = message.Replace("href=\"/", "href=\"" + adresPlatformy, StringComparison.InvariantCultureIgnoreCase);
            return message;
        }

        public static string ActionLinkWithList(this UrlHelper helper, string action, string controller, object routeData)
        {
            string href = helper.Action(action, controller);

            if (routeData != null)
            {
                RouteValueDictionary rv = new RouteValueDictionary(routeData);
                List<string> urlParameters = new List<string>();
                foreach (var key in rv.Keys)
                {
                    object value = rv[key];

                    if (value is IEnumerable && !(value is string))
                    {
                        List<string> parametrykolekcji = new List<string>();
                        bool pusta = true;
                        int i = 0;
                        foreach (object val in (IEnumerable) value)
                        {
                            string wal = "";

                            if (val != null)
                            {
                                wal = val.ToString();
                            }
                            if (!string.IsNullOrEmpty(wal))
                            {
                                pusta = false;
                            }
                            parametrykolekcji.Add(string.Format("{0}[{2}]={1}", key, wal, i));
                            ++i;
                        }
                        if (!pusta)
                        {
                            urlParameters.AddRange(parametrykolekcji);
                        }
                    }
                    else if (value != null)
                    {
                        urlParameters.Add(string.Format("{0}={1}", key, value));
                    }
                }
                string paramString = string.Join("&", urlParameters.ToArray()); // ToArray not needed in 4.0
                if (!string.IsNullOrEmpty(paramString))
                {
                    href += "?" + paramString;
                }
            }
            return href;
        }

        [FriendlyName("Link do produktów z tą cechą")]
        [LinkGenerator(typeof(CechyBll))]
        [WidoczneListaAdmin(true, false, false, true)]
        public static string ZbudujLink(this UrlHelper helper, Cecha cecha, Jezyk jezyk)
        {
            AtrybutBll atrybutCechy = null;
            if (cecha is CechyBll)
            {
                atrybutCechy = (cecha as CechyBll).PobierzAtrybut();
            }
            else
            {
                atrybutCechy = Calosc.DostepDane.PobierzPojedynczy<AtrybutBll>(cecha.AtrybutId, jezyk.Id);
            }


            if (atrybutCechy == null)
            {
                throw new Exception("Nie można budować linku do cech bez atrybutów. Cecha Id: " + cecha.Id);
            }
            Dictionary<string, string> pars = new Dictionary<string, string>();
            pars.Add("filtry", $"{atrybutCechy.PoleDoBudowyLinkow}[{cecha.Nazwa}]");
            string url = ZbudujQueryString("", pars);
            string wynik = $"{SymbolJezyka(jezyk)}/p?{url}";
            return wynik;
        }

        /// <summary>
        /// Pobiera scieżke dla pliku użytkownika
        /// </summary>
        /// <param name="typ"></param>
        /// <param name="objektId"></param>
        /// <param name="nazwaPliku"></param>
        /// <param name="linkBezwzledny"></param>
        /// <returns></returns>
        public static string PobierzSciezkePlikUsera(this UrlHelper helper, Type typ, object objektId, string nazwaPliku, bool linkZewnetrzny)
        {
            if (string.IsNullOrEmpty(objektId.ToString()) || objektId.ToString() == "0")
            {
                throw new InvalidOperationException("Błąd przy próbie pobrania sciezki do pliku użytkownika. Id obiektu nie zostało ustawione. Typ: " + typ);
            }
            if (nazwaPliku.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                throw new InvalidOperationException("Nazwa pliku zawiera niedozwolone znaki");
            }
            string sciezka = "Zasoby\\" + typ.Name + "\\" + objektId + "\\" + nazwaPliku;
            if (linkZewnetrzny)
            {
                return ZbudujLinkZewnetrzny(helper, sciezka);
            }
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, sciezka);
        }

        /// <summary>
        /// Link do strony
        /// Link do strony - składany ręcznie dlatego że routing do strony jest routingiem domyślnym, nie mozna go wygenerować autoamtycznie
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="tresc"></param>
        /// <param name="klient"></param>
        /// <param name="jezyk">Język do linku</param>
        /// <param name="dodatkoweParametryDoUrl"></param>
        /// <returns></returns>
        public static string ZbudujLink(this UrlHelper helper, TrescBll tresc, IKlient klient, Jezyk jezyk, Dictionary<string, string> dodatkoweParametryDoUrl = null)
        {
            if (tresc == null)
            {
                throw new Exception("Brak treści - nie można zbudować linku");
            }

            string zCzegoLink = tresc.Symbol;

            if (!string.IsNullOrEmpty(tresc.LinkAlternatywny))
            {
                if (tresc.LinkAlternatywny.StartsWith("http:", StringComparison.InvariantCultureIgnoreCase) 
                    || tresc.LinkAlternatywny.StartsWith("https:", StringComparison.InvariantCultureIgnoreCase) 
                    || tresc.LinkAlternatywny.StartsWith("mailto:", StringComparison.InvariantCultureIgnoreCase))
                {
                    return tresc.LinkAlternatywny;
                }

                if (tresc.LinkAlternatywny.StartsWith("/"))
                {
                    throw new Exception("Linki alternatywne nie mogą zaczynać się na znak /. Skasuj z linku poczatkowy /. Strona o ID: " + tresc.Id + ". Link: " + tresc.LinkAlternatywny);
                }
                zCzegoLink = tresc.LinkAlternatywny;
            }

            if (string.IsNullOrEmpty(zCzegoLink))
            {
                return "";
            }

            StringBuilder linkBezQS = new StringBuilder(zCzegoLink);
            StringBuilder QS = null;

            if (zCzegoLink.Contains('?'))
            {
                int pozycjaQS = zCzegoLink.IndexOf("?");
                linkBezQS = new StringBuilder(zCzegoLink.Substring(0, pozycjaQS));
                QS = new StringBuilder(zCzegoLink.Substring(pozycjaQS + 1));
            }

            //czy chcemy modala lub mamy paramertry dodatkowe
            if (SolexBllCalosc.PobierzInstancje.TresciDostep.CzyTrescOtwieranaJakoModal(tresc, klient))
            {
                linkBezQS = linkBezQS.Append("/m");
            }

            if (dodatkoweParametryDoUrl != null)
            {
                if (QS == null)
                {
                    QS = new StringBuilder();
                }

                foreach (var p in dodatkoweParametryDoUrl)
                {
                    QS.Append(string.Format("&{0}={1}", p.Key, HttpUtility.UrlEncode(p.Value)));
                }
            }

            //obsluga jezyka, sprawdzenie potrzebne do momentu az nie zostanie stworzona nowa kontrolka menu mobilnego gdyz link do kategorii juz w sobie ma kulture zaszyta
            //if (linkBezQS.ToString().IndexOf(jezyk.Symbol, StringComparison.InvariantCultureIgnoreCase) != 0)
            //{
            //   throw new Exception("Link już zawiera jezyk - błąd. Link: " + linkBezQS);
            //}

            //todo: bio planet nie obsluguje jezykow - nie robimy tego     linkBezQS.Insert(0, SymbolJezyka(jezyk) + "/");
            linkBezQS.Insert(0,  "/");


            if (QS == null)
            {
                return linkBezQS.ToString();
            }
            return linkBezQS.ToString() + "?" + QS.ToString().TrimStart('&');
        }

        /// <summary>
        /// link do profilu klienta wartosci tak / nie.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="dane"></param>
        /// <param name="jezyk">Język do linku</param>
        /// <returns></returns>
        public static string ZbudujLinkDoUstWartosciBool(this UrlHelper helper, ZmianaDanychBool dane, Jezyk jezyk)
        {
            RouteValueDictionary parametry = new RouteValueDictionary();
            parametry = KulturaJezyka(jezyk, parametry);
            parametry.Add("typ", dane.Typ);
            parametry.Add("wartosc", dane.Wartosc);
            return helper.Action("UstawWartoscUstawieniaBool", "ProfilKlienta", parametry);
        }

        public static string ZbudujLinkDoDrukowaniaKataloguKoszyka(this UrlHelper helper, Jezyk jezyk)
        {
            RouteValueDictionary parametry = new RouteValueDictionary();
            parametry = KulturaJezyka(jezyk, parametry);
            return helper.Action("DrukujKatalogProduktow", "Integracja", parametry);
        }

        private static object lokZbudujLinkKategorieBll = new object();

        [FriendlyName("Link do kategorii")]
        [LinkGenerator(typeof(KategorieBLL))]
        [WidoczneListaAdmin(true, false, false, true)]
        public static string ZbudujLink(this UrlHelper helper, KategorieBLL kategoria, Jezyk jezyk)
        {
            if (kategoria == null)
            {
                throw new Exception("Kategoria jest null - nie można stworzyć linka dla null obiektu");
            }

            //linki do kategorii sa zawsze takie same dla danego jezyk wiec sa cachowane
            return Calosc.Cache.SlownikPrywatny_PobierzObiekt<string>((long x) =>
            {
                RouteValueDictionary parametry = new RouteValueDictionary();

                if (helper.RequestContext.RouteData.Values.ContainsKey("culture"))
                {
                    helper.RequestContext.RouteData.Values.Remove("culture");
                }

               
                parametry = KulturaJezyka(jezyk, parametry);

                parametry.Add("nazwa", kategoria.FriendlyLinkURL);
                parametry.Add("kategoria", kategoria.Id);

                return (object)helper.Action("KategoriaProduktu", "Tresci", parametry);
            }, jezyk.Id, kategoria.Id, "kategoriaBll_linki");
        }

        private static TrescBll _stronaZProduktem = null;

        /// <summary>
        /// strona z profuktem - czesto wykorzystywana wiec przechowujemy ja w propertisie zeby bylo szybciej. Publiczna bo w testach podmienimy -a klasa jest statyczna
        /// </summary>
        public static TrescBll StronaZProduktem
        {
            get
            {
                if (_stronaZProduktem == null)
                {                    
                    _stronaZProduktem = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<TrescBll>(x => !string.IsNullOrEmpty(x.Symbol) && x.Symbol.Equals(Config.AdresStronyZProduktem, StringComparison.InvariantCultureIgnoreCase), null);
                    if (_stronaZProduktem == null)
                    {
                        throw new Exception($"Brak strony z produktem - o symbolu: [{Config.AdresStronyZProduktem}], ustawienie AdresStronyZProduktem.");
                    }
                }
                return _stronaZProduktem;
            }
            set { _stronaZProduktem = value; }
        }

        public static object _lokLinkiProduktow = new object();

        /// <summary>
        /// Buduje link to produktu klienta.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="produkt">Obiekt produktu</param>
        /// <param name="jezyk">Język do linku</param>
        /// <param name="pelnyLinkZHttp"></param>
        /// <returns>gotowy link</returns>
        [FriendlyName("Link do produktu klienta")]
        [LinkGenerator(typeof(ProduktKlienta))]
        [WidoczneListaAdmin(true, false, false, true)]
        public static string ZbudujLink(this UrlHelper helper, IProduktKlienta produkt, Jezyk jezyk = null, bool pelnyLinkZHttp = false)
        {
            if (jezyk == null)
            {
              //  throw new Exception("Brak jezyk dla linka - bartek dodal to bo wydaje sie ze wszedzie jest jezyk ale funckaj dotychczas pozwalala na null jezyka - bez sensu chyba");
                  jezyk = SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykiWSystemie[produkt.JezykId];
            }

            //klucz cache musi byc inny dla zalogowanych i niezalogowanych bo otwiera sie w modalu albo bez modala

            //linki do produktów klienta sa zawsze takie same dla danego jezyk wiec sa cachowane
            return Calosc.Cache.SlownikPrywatny_PobierzObiekt<string>((long x) =>
            {
                RouteValueDictionary parametry = new RouteValueDictionary();
                parametry.Add("nazwaProduktu", produkt.FriendlyLinkURL);
                parametry = KulturaJezyka(jezyk, parametry);
                parametry.Add("produktId", produkt.Id);
                string url;
                if (SolexBllCalosc.PobierzInstancje.TresciDostep.CzyTrescOtwieranaJakoModal(StronaZProduktem, produkt.Klient))
                {
                    parametry.Add("modal", "m");
                }

                //dla bioplanet zmiana - bylo mase problemow z tymi linkami - dlatego w koncu proste generowanie zawsze pelnych linkow
                //if (!pelnyLinkZHttp)
                //{
                //    url = helper.Action("SzczegolyProduktu", "Tresci", parametry);
                //    url = url.Replace($"/{jezyk.Symbol}/", "/");
                //}
                //else
                //{
                    url = helper.Action("SzczegolyProduktu", "Tresci", parametry, "https");
                //}


                return (object) url;
            }, jezyk.Id, produkt.Id, $"produktKlienta_linki_{produkt.Klient.Dostep}");
        }

        //todo: parametr IDtresci do wywalenia jest jak breadcrumby porpawimy
        /// <summary>
        /// Link do bloga.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="blogWpis"></param>
        /// <param name="symbol"></param>
        /// <param name="jezyk">Język do linku</param>
        /// <returns></returns>
        public static string ZbudujLink(this UrlHelper helper, BlogWpisBll blogWpis, string symbol, Jezyk jezyk)
        {
            if (string.IsNullOrEmpty(symbol) || string.IsNullOrEmpty(blogWpis.LinkURL))
            {
                throw new Exception("Brak symbola strony z układem bloga dla wpisu bloga id: " + blogWpis.Id);
            }

            if (blogWpis.LinkAlternetywny != null && !string.IsNullOrEmpty(blogWpis.LinkAlternetywny.Url))
            {
                if (blogWpis.LinkAlternetywny.Url.StartsWith(SymbolJezyka(jezyk) + "/", StringComparison.InvariantCultureIgnoreCase) || 
                    blogWpis.LinkAlternetywny.Url.StartsWith("http:", StringComparison.InvariantCultureIgnoreCase)
                    || blogWpis.LinkAlternetywny.Url.StartsWith("https:", StringComparison.InvariantCultureIgnoreCase) )
                {
                    return blogWpis.LinkAlternetywny.Url + ((blogWpis.LinkAlternetywny.Tryb == TrybOtwierania.Modal) ? "/m" : "");
                }
                return string.Format("{0}/{1}{2}", SymbolJezyka(jezyk), blogWpis.LinkAlternetywny.Url.TrimStart('/'), (blogWpis.LinkAlternetywny.Tryb == TrybOtwierania.Modal) ? "/m" : "");
            }

            RouteValueDictionary parametry = new RouteValueDictionary();
            parametry = KulturaJezyka(jezyk, parametry);
            parametry.Add("tytul", blogWpis.LinkURL);
            parametry.Add("symbol", symbol);
            parametry.Add("blogWpisId", blogWpis.Id);
            return helper.Action("WpisyBloga", "Tresci", parametry);
        }


        /// <summary>
        /// Link do adresuUrl.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="adres"></param>
        /// <param name="jezyk">Język do linku</param>
        /// <returns></returns>
        public static string ZbudujLink(this UrlHelper helper, AdresUrl adres, Jezyk jezyk)
        {
            if (adres == null || string.IsNullOrEmpty(adres.Url)) return "";
            if (adres.Url.StartsWith(SymbolJezyka(jezyk) + "/", StringComparison.InvariantCultureIgnoreCase)
                || adres.Url.StartsWith("http:", StringComparison.InvariantCultureIgnoreCase)
                || adres.Url.StartsWith("https:", StringComparison.InvariantCultureIgnoreCase))
            {
                return adres.Url + ((adres.Tryb == TrybOtwierania.Modal) ? "/m" : "");
            }
            return string.Format("{0}/{1}{2}", SymbolJezyka(jezyk), adres.Url.TrimStart('/'), (adres.Tryb == TrybOtwierania.Modal) ? "/m" : "");
        }

        /// <summary>
        /// Przypisuje przedrostek jezykowy jesli potrzeba do gotowego url-a.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="adres">adres url w formie stringu</param>
        /// <param name="jezyk">Język do linku</param>
        /// <returns></returns>
        public static string ZbudujLink(this UrlHelper helper, string adres, Jezyk jezyk)
        {
            string symbol = (Config.WieleJezykowWSystemie ? "/" + jezyk.Symbol + "/" : "/");
            return symbol + (adres).Trim('/');
        }

        /// <summary>
        /// Buduje link do grupy produktowej.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="grupa">Grupa do której ma być link</param>
        /// <param name="jezyk">Język do linku</param>
        /// <param name="symbol">symbol tresci</param>
        /// <returns>gotowy link</returns>
        [FriendlyName("Link do grupy")]
        [LinkGenerator(typeof(GrupaBLL))]
        [WidoczneListaAdmin(true, false, false, true)]
        public static string ZbudujLink(this UrlHelper helper, GrupaBLL grupa, Jezyk jezyk, string symbol = "")
        {
            if (string.IsNullOrEmpty(symbol)) symbol = grupa.SymbolTresciOpisuGrupy;

            if (string.IsNullOrEmpty(symbol))
            {
                return "";
            }

            RouteValueDictionary parametry = new RouteValueDictionary();
            parametry = KulturaJezyka(jezyk, parametry);
            parametry.Add("symbol", symbol);
            parametry.Add("gpid", grupa.Id);
            return helper.Action("GrupaKategorie", "Tresci", parametry);
        }

        public static string ZbudujLinkDoTestowychDanychKataloguProduktow(this UrlHelper helper, PlikIntegracjiSzablon zrodlo)
        {
            RouteValueDictionary parametry = new RouteValueDictionary();
            parametry.Add("wersja", zrodlo.Wersja.First());
            parametry.Add("szablonId", zrodlo.IdSzablonu);
            return helper.Action("GenerujTestowyPlikDanychKatalogu", "Integracja", parametry);
        }

        /// <summary>
        /// Buduje link do aktualnej strony w wybranym języku - wykorzystywne w kontrolce zmiena jezyka.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="language">wybrany język</param>
        /// <param name="klient"></param>
        /// <returns>gotowy link</returns>
        public static string ZbudujLink(this UrlHelper helper, Jezyk language, IKlient klient)
        {
            RouteValueDictionary routeValueDictionary = new RouteValueDictionary(helper.RequestContext.HttpContext.Request.RequestContext.RouteData.Values);
            //Konieczne ze względy
            foreach (var key in helper.RequestContext.HttpContext.Request.QueryString.AllKeys)
            {
                routeValueDictionary[key] = helper.RequestContext.HttpContext.Request.QueryString[key];
            }


            routeValueDictionary["culture"] = language.Symbol;

            //nie chcemy smiecia idTresci w linkach
            if (routeValueDictionary.ContainsKey("idTresci"))
            {
                routeValueDictionary.Remove("idTresci");
            }

            return helper.RouteUrl(routeValueDictionary);
        }


        /// <summary>
        /// Buduje link do pliku wzorcowego importu do koszyka
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="opis">Obiekt opisujący importe</param>
        /// <returns>gotowy link</returns>
        public static string ZbudujLink(this UrlHelper helper, OpisImportera opis)
        {
            if (string.IsNullOrEmpty(opis.PlikWzorcowy)) return null;
            string link = string.Format("/HELP/PlikiImport/{0}", opis.PlikWzorcowy);
            string sciezkaPliku = AppDomain.CurrentDomain.BaseDirectory + link;
            if (File.Exists(sciezkaPliku))
            {
                return link;
            }
            return null;
        }

        /// <summary>
        /// Budowanie linka do dokumentu.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="dokument">Obiekt dokumentu</param>
        /// <returns></returns>
        [FriendlyName("Link do Dokumentu")]
        [LinkGenerator(typeof(DokumentyBll))]
        [WidoczneListaAdmin(true, false, false, true)]
        public static string ZbudujLink(this UrlHelper helper, DokumentyBll dokument, Jezyk jezyk)
        {
            RouteValueDictionary parametry = new RouteValueDictionary();
            parametry = KulturaJezyka(jezyk, parametry);
            parametry.Add("Id", dokument.Id);
            return helper.Action("Pokaz", "Dokumenty", parametry);
        }

        /// <summary>
        /// Uniwersalna metoda przyjmująca obiekt i budująca link na podstawie typu obiektu jaki otrzyma.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="dane">obiek do którego chcemy zbudować link</param>
        /// <param name="jezyk">Język do linku</param>
        /// <param name="methodName">Ewentualna nazwa metodu do tworzenia linku</param>
        /// <returns>gotowy link</returns>
        public static string ZbudujLink(this UrlHelper helper, object dane, Jezyk jezyk, string methodName = "")
        {
            if (dane is string)
            {
                string symbol = (Config.WieleJezykowWSystemie ? jezyk.Symbol + "/" : "");
                return symbol + ((string) dane).Trim('/');
            }

            if (dane is Cecha)
            {
                if (string.IsNullOrEmpty(methodName) || methodName.Equals("ZbudujLink", StringComparison.InvariantCultureIgnoreCase)) return ZbudujLink(helper, (Cecha) dane, jezyk);
                MethodInfo pobierzWartosc = typeof(UrlExtender).GetMethod(methodName, new[] {typeof(UrlHelper), dane.GetType(), typeof(Jezyk), typeof(bool)});
                if (pobierzWartosc == null) return null;
                return (string) pobierzWartosc.Invoke(helper, new[] {helper, dane, jezyk, false});
            }
            if (dane is KategorieBLL)
            {
                if (string.IsNullOrEmpty(methodName)) return ZbudujLink(helper, (KategorieBLL) dane, jezyk);
                MethodInfo pobierzWartosc = typeof(UrlExtender).GetMethod(methodName, new[] {typeof(UrlHelper), dane.GetType(), typeof(Jezyk), typeof(string)});
                return (string) pobierzWartosc.Invoke(helper, new[] {helper, dane, jezyk, ""});
            }
            if (dane is ProduktKlienta || dane is ProduktKlientaWirtualny)
            {
                if (string.IsNullOrEmpty(methodName))
                {
                    return ZbudujLink(helper, (ProduktKlienta) dane, jezyk);
                }
                MethodInfo pobierzWartosc = typeof(UrlExtender).GetMethod(methodName, new[] {typeof(UrlHelper), dane.GetType(), typeof(Jezyk), typeof(Jezyk)});
                return (string) pobierzWartosc.Invoke(helper, new[] {helper, dane, jezyk, ""});
            }
            if (dane is GrupaBLL)
            {
                if (string.IsNullOrEmpty(methodName)) return ZbudujLink(helper, (GrupaBLL) dane, jezyk);
                MethodInfo pobierzWartosc = typeof(UrlExtender).GetMethod(methodName, new[] {typeof(UrlHelper), dane.GetType(), typeof(Jezyk), typeof(string)});
                return (string) pobierzWartosc.Invoke(helper, new[] {helper, dane, jezyk, ""});
            }
            if (dane is DokumentyBll)
            {
                if (string.IsNullOrEmpty(methodName)) return ZbudujLink(helper, (DokumentyBll) dane, jezyk);
                MethodInfo pobierzWartosc = typeof(UrlExtender).GetMethod(methodName, new[] {typeof(UrlHelper), dane.GetType(), typeof(Jezyk), typeof(string)});
                return (string) pobierzWartosc.Invoke(helper, new[] {helper, dane, jezyk, ""});
            }
            if (dane is OpisImportera)
            {
                if (string.IsNullOrEmpty(methodName)) return ZbudujLink(helper, (OpisImportera) dane);
                MethodInfo pobierzWartosc = typeof(UrlExtender).GetMethod(methodName, new[] {typeof(UrlHelper), dane.GetType(), typeof(Jezyk), typeof(string)});
                return (string) pobierzWartosc.Invoke(helper, new[] {helper, dane, jezyk, ""});
            }
            if (dane is AdresUrl)
            {
                if (string.IsNullOrEmpty(methodName)) return ZbudujLink(helper, (AdresUrl) dane, jezyk);
                MethodInfo pobierzWartosc = typeof(UrlExtender).GetMethod(methodName, new[] {typeof(UrlHelper), dane.GetType(), typeof(Jezyk)});
                return (string) pobierzWartosc.Invoke(helper, new[] {helper, dane, jezyk});
            }

            throw new Exception("Nie można zbudować linka dla obiektu: " + dane);
        }

        /// <summary>
        /// Tworzy pełny adres url z HTTP
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        private static string DodajSerwerDoLinku(string link)
        {
            if( link.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) || link.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase) ) {
                return link;
            }

            string strona = "";
            if (HttpContext.Current != null)
            {
                //strona = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host +
                //         (HttpContext.Current.Request.Url.IsDefaultPort ? "" : ":" + HttpContext.Current.Request.Url.Port.ToString(CultureInfo.InvariantCulture));

                //bch: wymuszam HTTPs bo bylo masa problemow z tym http - jak byl proces osobny np. integracje robil http

                strona = "https://" + HttpContext.Current.Request.Url.Host;
            }

            link = link.Replace("\\", "/");

            if (link.EndsWith("/"))
            {
                return strona + link;
            }

            string wynik = strona + (link.StartsWith("/") ? "" : "/") + link;

            if (wynik.StartsWith("/"))
            {
                throw new Exception($"Błąd kosmiczny linku - wygenerowany link: {wynik}");
            }

            return wynik;
        }

        /// <summary>
        /// link do przelogowania dla przedstawiciela do listy klientow.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="jezyk">Język do linku</param>
        /// <param name="czyKlientAdministracyjny"></param>
        /// <returns></returns>
        public static string ZbudujLinkDoListyKontrahentow(this UrlHelper helper, Jezyk jezyk, bool czyKlientAdministracyjny)
        {
            if (czyKlientAdministracyjny)
            {
                return helper.ZbudujLinkDoAdmina(typeof(SolEx.Hurt.Core.Klient), jezyk);
            }

            string url = "/" + helper.Action("Przeloguj", "Logowanie").TrimStart('/');
            return url;
        }

        private static TrescBll _logowanieTresc = null;

        /// <summary>
        /// link do strony logowania
        /// </summary>
        /// <returns>gotowy link</returns>
        public static string LinkLogowania(this UrlHelper helper, string ReturnURL = null)
        {
            if (_logowanieTresc == null)
            {
                string symbolStronyLogowania = SolexBllCalosc.PobierzInstancje.Konfiguracja.SymbolStronylogowanie;
                _logowanieTresc = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<TrescBll>(x => symbolStronyLogowania.Equals(x.Symbol, StringComparison.InvariantCultureIgnoreCase), null);
            }

            Dictionary<string, string> dodatkoweParametry = null;

            if (ReturnURL != null)
            {
                if (ReturnURL.StartsWith("http"))
                {
                    throw new Exception("Adresy podane w ReturnURL muszą być względne. Podano adres: " + ReturnURL);
                }
                dodatkoweParametry = new Dictionary<string, string> {{"ReturnURL", ReturnURL}};
            }

            return helper.ZbudujLink(_logowanieTresc, SolexBllCalosc.PobierzInstancje.Klienci.KlientNiezalogowany(), SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykDomyslny, dodatkoweParametry);
        }


        /// <summary>
        /// link do strony logowania.
        /// </summary>
        /// <returns>gotowy link</returns>
        public static string LinkResetuHasla(this UrlHelper helper, Jezyk jezyk)
        {
            string symbolStrony = "resetowanie-hasla";
            TrescBll tresc = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<TrescBll>(x => x.Symbol == symbolStrony, null);
            if (tresc == null)
            {
                SolexBllCalosc.PobierzInstancje.Log.InfoFormat("Brak treści (lub nie jest dostępna) o symbolu: '{0}' - nie można resetować hasła (wielkość znaków ma znaczenie)", symbolStrony);
                return null;
            }
            return helper.ZbudujLink(tresc, SolexBllCalosc.PobierzInstancje.Klienci.KlientNiezalogowany(), jezyk);
        }


        private static string ZbudujQueryString(string url, Dictionary<string, string> doUstawienia, IEnumerable<string> doUsuniecia = null)
        {
            if (string.IsNullOrEmpty(url) && (doUstawienia == null || doUstawienia.IsEmpty()) && doUsuniecia == null)
            {
                throw new Exception("Brak podanych parametrów do stworzenia linku");
            }

            string[] data = url.Split('?');
            NameValueCollection col = HttpUtility.ParseQueryString("");
            if (data.Length > 1)
            {
                col = HttpUtility.ParseQueryString(data[1]);
            }

            foreach (var doUStawienia in doUstawienia)
            {
                if (string.IsNullOrEmpty(doUStawienia.Value))
                {
                    continue;
                }
                col[doUStawienia.Key] = doUStawienia.Value;
            }

            if (doUsuniecia != null)
            {
                foreach (var del in doUsuniecia)
                {
                    col.Remove(del);
                }
            }

            return col.ToString();
            //return PoprawPolskieZnakiISpecjalne(col.ToString());
        }

        private static string PoprawPolskieZnakiISpecjalne(string url)
        {
            //slownik zmiany znaków
            //{ wartość jaką chcemy zamienić, wartośc docelowa}
            Dictionary<string, string> rep = new Dictionary<string, string>
            {
                {"%u0104", "Ą"},
                {"%u0106", "Ć"},
                {"%u0118", "Ę"},
                {"%u0141", "Ł"},
                {"%u0143", "Ń"},
                {"%u00D3", "Ó"},
                {"%u015a", "Ś"},
                {"%u0179", "Ź"},
                {"%u017b", "Ż"},
                {"%u0105", "ą"},
                {"%u0107", "ć"},
                {"%u0119", "ę"},
                {"%u0142", "ł"},
                {"%u0144", "ń"},
                {"%u00f3", "ó"},
                {"%u015b", "ś"},
                {"%u017a", "ź"},
                {"%u017c", "ż"},
                {"%5b", "["},
                {"%5d", "]"}
            };
            string wynik = url;
            foreach (var zam in rep)
            {
                wynik = wynik.Replace(zam.Key, zam.Value);
            }

            return wynik;
        }


        public static string WypiszZNewslettera(this UrlHelper helper, IKlient klient)
        {
            string klucz = SolexBllCalosc.PobierzInstancje.Klienci.KluczDoKlientaWypisanieZapisaniaZNewsletera(klient);
            RouteValueDictionary parametry = new RouteValueDictionary {{"email", klient.Email}, {"klucz", klucz}};

            return helper.Action("Wypisz", "Newsletter", parametry, "https");
        }

        public static string ZapiszDoNewslettera(this UrlHelper helper, IKlient klient)
        {
            string klucz = SolexBllCalosc.PobierzInstancje.Klienci.KluczDoKlientaWypisanieZapisaniaZNewsletera(klient);

            RouteValueDictionary parametry = new RouteValueDictionary {{"email", klient.Email}, {"klucz", klucz}};
            return helper.Action("Zapisz", "Newsletter", parametry);
        }

        private static string AdresPlatformy
        {
            get { return SolexBllCalosc.PobierzInstancje.Konfiguracja.wlasciciel_AdresPlatformy; }
        }

        public static string LinkDoApiIntegracja(this UrlHelper helper, Jezyk jezyk)
        {
            return helper.Action("GenerowanieKluczaAktualnegoKlienta", "Integracja");
        }

        public static string LinkProdukty(this UrlHelper helper, Jezyk jezyk)
        {
            return SymbolJezyka(jezyk) + "/" + Config.AdresStronyZProduktami;
        }


        /// <summary>
        /// Tworzony link do produktów na podstawie parametrów produktów
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="parametry"></param>
        /// <param name="request"></param>
        /// <param name="jezyk"></param>
        /// <returns></returns>
        public static string LinkProdukty(this UrlHelper helper, ParametryPrzekazywaneDoListyProduktow parametry, HttpRequestBase request, Jezyk jezyk)
        {
            string link;
            if (parametry.kategoria.HasValue)
            {
                link = ZbudujLink(helper, parametry.KategoriaObiekt, jezyk);
            }
            else
            {
                link = helper.LinkProdukty(jezyk);
            }

            Dictionary<string, string> pars = new Dictionary<string, string>();
            if (parametry.filtry != null && parametry.filtry.Any())
            {
                StringBuilder sb = new StringBuilder();
                foreach (var f in parametry.filtry)
                {
                    AtrybutBll a = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<AtrybutBll>(f.Key, jezyk.Id);
                    if (a != null)
                    {
                        var cechy = a.ListaCech.Where(x => f.Value.Contains(x.Id)).ToArray();
                        if (cechy.Any())
                        {
                            sb.Append(a.PoleDoBudowyLinkow);
                            sb.Append("[");
                            sb.Append(cechy.Aggregate("", (x, y) => x + ";" + y.Nazwa).TrimStart(';'));
                            sb.Append("]");
                        }
                    }
                }

                if (sb.Length > 0)
                {
                    pars.Add("filtry", sb.ToString());
                }
            }

            if (!string.IsNullOrEmpty(parametry.szukane))
            {
                pars.Add("szukane", parametry.szukane);
            }

            if (!string.IsNullOrEmpty(parametry.szukanaWewnetrzne))
            {
                pars.Add("szukanaWewnetrzne", parametry.szukanaWewnetrzne);
            }

            if (parametry.strona > 1)
            {
                pars.Add("strona", parametry.strona.ToString(CultureInfo.InvariantCulture));
            }

            if (pars.Any())
            {
                link = link + "?" + ZbudujQueryString("", pars);
            }

            return link;
        }

        /// <summary>
        /// Link do pliku integracji.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="jezyk">Język do linku</param>
        /// <param name="kodowanie"></param>
        /// <param name="szablon"></param>
        /// <param name="aktualnyKlient"></param>
        /// <param name="wersja"></param>
        /// <returns></returns>
        public static string ZbudujLinkDoSzablonuIntegracji(this UrlHelper helper, Jezyk jezyk, string kodowanie, PlikIntegracjiSzablon szablon, IKlient aktualnyKlient, int wersja)
        {
            RouteValueDictionary parametry = new RouteValueDictionary();
            parametry.Add("szablonId", szablon.IdSzablonu);
            parametry.Add("wersja", wersja);
            parametry.Add("kodowanie", kodowanie);

            if (!string.IsNullOrEmpty(aktualnyKlient.KluczSesji))
            {
                parametry.Add("kluczKlienta", aktualnyKlient.KluczSesji);
            }

            if (Config.WieleJezykowWSystemie)
            {
                parametry = KulturaJezyka(jezyk, parametry);
            }
            return helper.Action("PobierzPlik", "Integracja", parametry, "https");
        }

        #region linki do stalych filtrow

        /// <summary>
        /// Buduje link lub atrubut do linku zmiany filtru
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="wybrany">jakie sa wybrane cechy</param>
        /// <param name="aktualna">cecha dla której tworzymy link</param>
        /// <param name="model">Model danych do filtru</param>
        /// <returns></returns>
        public static string ZbudujLinkLubAtrubutDoFitru(this UrlHelper helper, bool wybrany, CechyBll aktualna, DaneDoFiltrow model, Jezyk jezyk)
        {
            var cechy = model.Wszystkie.SelectMany(z => z.ListaCech).ToDictionary(x => x.Id, x => x);
            HashSet<long> data = new HashSet<long>();
            if (model.WszystkieWybraneCechy != null)
            {
                data = (model.MultiWybor) ? new HashSet<long>(model.WszystkieWybraneCechy.SelectMany(y => y.Value)) : new HashSet<long>(model.WszystkieWybraneCechy.Where(x => x.Key != aktualna.AtrybutId).SelectMany(y => y.Value));
            }

            if (Config.TrybPokazywaniaFiltrow == TrybPokazywaniaFiltrow.WymuszajSciezke)
            {
                List<CechyBll> tmpData = cechy.WhereKeyIsIn(data);
                foreach (CechyBll i in tmpData)
                {
                    if (i.CechyNadrzedne != null && i.CechyNadrzedne.Any() && (i.CechyNadrzedne.Contains(aktualna.Id) || !i.CechyNadrzedne.Overlaps(data)))
                    {
                        data.Remove(i.Id);
                    }
                }
            }

            if (wybrany)
            {
                data.Remove(aktualna.Id);
            }
            else
            {
                data.Add(aktualna.Id);
            }

            string dataFilter;

            if (model.StalyFiltr)
            {
                dataFilter = "href=" + ZbudujLinkDodajStalyFiltr(helper, aktualna, jezyk);
                if (!model.MultiWybor)
                {
                    dataFilter = "href=" + ZbudujLinkDodajStalyFiltr(helper, aktualna, jezyk, true);
                }
                if (wybrany)
                {
                    dataFilter = "href=" + ZbudujLinkUsunStalyFiltr(helper, aktualna, jezyk);
                }
            }
            else
            {
                List<CechyBll> cechylink = cechy.WhereKeyIsIn(data);
                Dictionary<int, HashSet<long>> cechycyfry = cechylink.GroupBy(z => z.AtrybutId).ToDictionary(x => x.Key.Value, x => new HashSet<long>( x.Select(y => y.Id) ));

                dataFilter = string.Format("data-filtry =\"{0}\" ", JsonConvert.SerializeObject(cechycyfry));
            }

            return dataFilter;
        }

        /// <summary>
        /// link do usunięcia wartości ze stałych filtrów w profilu klienta
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="cecha"></param>
        /// <returns></returns>
        public static string ZbudujLinkUsunStalyFiltr(this UrlHelper helper, Cecha cecha, Jezyk jezyk)
        {
            RouteValueDictionary parametry = new RouteValueDictionary();
            parametry.Add("cecha", cecha.Id);
            parametry = KulturaJezyka(jezyk, parametry);
            return helper.Action("UsunStalyFiltr", "StaleFiltry", parametry);
            //var url = "/Filtry/UsunStalyFiltr/" + cecha.Id;
            //return url;
        }

        /// <summary>
        /// link do usunięcia wszystkich stałych filtrów w profilu klienta
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        public static string ZbudujLinkUsunStalyFiltr(this UrlHelper helper, Jezyk jezyk)
        {
            RouteValueDictionary parametry = new RouteValueDictionary();
            return helper.Action("UsunStaleFiltry", "StaleFiltry", parametry);
        }


        /// <summary>
        ///  link do dodania wartości do stałych filtrów w profilu klienta
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="cecha"></param>
        /// <param name="usunPozostaleCechy">czy dodajemy czy podmieniamy cechy</param>
        /// <returns></returns>
        public static string ZbudujLinkDodajStalyFiltr(this UrlHelper helper, Cecha cecha, Jezyk jezyk, bool usunPozostaleCechy = false)
        {

            RouteValueDictionary parametry = new RouteValueDictionary();
            parametry.Add("cecha", cecha.Id);
            parametry = KulturaJezyka(jezyk, parametry);
            if (usunPozostaleCechy)
            {
                parametry.Add("zamiana", 1);
            }
            return helper.Action("DodajStalyFiltr", "StaleFiltry", parametry);
        }

        #endregion

        /// <summary>
        /// link do usunięcia wszystkich stałych filtrów w profilu klienta.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="jezyk">Język do linku</param>
        /// <returns></returns>
        public static string ZbudujLinkKoszyka(this UrlHelper helper, Jezyk jezyk)
        {

            RouteValueDictionary parametry = new RouteValueDictionary();
            parametry.Add("symbol", "koszyk");
            parametry = KulturaJezyka(jezyk, parametry);
            return helper.Action("StronaSymbol", "Tresci", parametry);
        }

        /// <summary>
        /// Buduje link do menu w panelu administratora.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="t">Typ dla którego tworzymy adres</param>
        /// <param name="jezyk">Język do adresu</param>
        /// <returns></returns>
        public static string ZbudujLinkDoAdmina(this UrlHelper helper, Type t, Jezyk jezyk)
        {
            RouteValueDictionary parametry = new RouteValueDictionary();
            parametry.Add("typ", t.PobierzOpisTypu());
            parametry = KulturaJezyka(jezyk, parametry);
            return helper.Action("Lista", "Admin", parametry);
        }

        /// <summary>
        /// Buduje ades url do przejści do treści w adminie.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="jezyk">Język do adresu</param>
        /// <returns></returns>
        public static string ZbudujLinkDoTresciAdmin(this UrlHelper helper, Jezyk jezyk)
        {
            RouteValueDictionary parametry = new RouteValueDictionary();
            parametry = KulturaJezyka(jezyk, parametry);
            return helper.Action("Tresci", "Admin", parametry);
        }

        /// <summary>
        /// Budeje adres url do zawartości treści.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="jezyk">Język do adresu</param>
        /// <returns></returns>
        public static string ZbudujLinkDoTrescZawartoscAdmin(this UrlHelper helper, int? id, Jezyk jezyk)
        {
            RouteValueDictionary parametry = new RouteValueDictionary();
            parametry = KulturaJezyka(jezyk, parametry);
            parametry.Add("id", id);
            return helper.Action("TrescZawartoscEdycja", "Admin", parametry);
        }

        /// <summary>
        /// Buduje adres url do importu w adminie. Link w postaci http://
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="typ"></param>
        /// <param name="jezyk">Język do adresu</param>
        /// <returns></returns>
        public static string ZbudujLinkDoImportuAdmin(this UrlHelper helper, Type typ, Jezyk jezyk)
        {
            RouteValueDictionary parametry = new RouteValueDictionary();
            parametry = KulturaJezyka(jezyk, parametry);
            return helper.Action("Import", "Admin", parametry, "https");
        }

        /// <summary>
        /// Buduje adres url do klonowania treści.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="kolumna"></param>
        /// <param name="jezyk">Język do adresu</param>
        /// <returns></returns>
        public static string ZbudujLinkDoKlonyjTrescAdmin(this UrlHelper helper, TrescKolumnaBll kolumna, Jezyk jezyk)
        {
            RouteValueDictionary parametry = new RouteValueDictionary();
            parametry = KulturaJezyka(jezyk, parametry);
            parametry.Add("id", kolumna.Id);
            return helper.Action("Klonuj", "Admin", parametry);
        }

        /// <summary>
        /// Buduje link do usuwania zawartości treści.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="kolumna"></param>
        /// <param name="jezyk">Język do adresu</param>
        /// <returns></returns>
        public static string ZbudujLinkDoUsunTrescAdmin(this UrlHelper helper, TrescKolumnaBll kolumna, Jezyk jezyk)
        {
            RouteValueDictionary parametry = new RouteValueDictionary();
            parametry = KulturaJezyka(jezyk, parametry);
            parametry.Add("id", kolumna.Id);
            parametry.Add("typ", kolumna.GetType().PobierzOpisTypu());
            parametry.Add("przekieruj", false);
            return helper.Action("Usuwanie", "Admin", parametry);
        }

        /// <summary>
        /// Buduje adres url do edycji kolumny zawartości treści.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="kolumna"></param>
        /// <param name="jezyk">Język do adresu</param>
        /// <returns></returns>
        public static string ZbudujLinkDoEdycjiTresciAdmin(this UrlHelper helper, TrescKolumnaBll kolumna, Jezyk jezyk)
        {
            RouteValueDictionary parametry = new RouteValueDictionary();
            parametry = KulturaJezyka(jezyk, parametry);
            parametry.Add("typ", kolumna.GetType().PobierzOpisTypu());
            parametry.Add("typNadrzednego", typeof(TrescWierszBll).PobierzOpisTypu());
            parametry.Add("nadrzedny", kolumna.TrescWierszId.ToString(CultureInfo.InvariantCulture));
            parametry.Add("id", kolumna.Id);
            parametry.Add("jezyk", jezyk.Id);
            return helper.Action("EdycjaModal", "Admin", parametry);
        }

        /// <summary>
        /// Buduje link do edycji wiersza zawartości treści.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="wiersz"></param>
        /// <param name="jezyk"></param>
        /// <returns></returns>
        public static string ZbudujLinkDoEdycjiTresciAdmin(this UrlHelper helper, TrescWierszBll wiersz, Jezyk jezyk)
        {
            RouteValueDictionary parametry = new RouteValueDictionary();
            parametry = KulturaJezyka(jezyk, parametry);
            parametry.Add("typ", wiersz.GetType().PobierzOpisTypu());
            parametry.Add("typNadrzednego", typeof(TrescWierszBll).PobierzOpisTypu());
            parametry.Add("nadrzedny", wiersz.TrescId.ToString(CultureInfo.InvariantCulture));
            parametry.Add("id", wiersz.Id);
            parametry.Add("jezyk", jezyk.Id);
            return helper.Action("EdycjaModal", "Admin", parametry);
        }

        /// <summary>
        /// Buduje adres url do podglądu szablonu maili.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="doKogo">Do kogo ma być wysłany</param>
        /// <param name="id">Id szablonu</param>
        /// <param name="jezyk">Język do adresu</param>
        /// <returns></returns>
        public static string ZbudujLinkDoPodgladuMaili(this UrlHelper helper, TypyPowiadomienia doKogo, long id, Jezyk jezyk)
        {
            RouteValueDictionary parametry = new RouteValueDictionary();
            parametry.Add("id", id);
            parametry.Add("dokogo", doKogo);
            parametry = KulturaJezyka(jezyk, parametry);
            return helper.Action("Podglad", "SzablonyMaili", parametry);
        }

        /// <summary>
        /// Buduje adres url do powrotu.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="czyToAjaxowyRequest"></param>
        /// <param name="UrlReferer"></param>
        /// <param name="aktualnyJezyk">Język do adresu</param>
        /// <param name="linkPowrotu"></param>
        /// <returns></returns>
        public static string ZbudujLinkPowrotu(this UrlHelper helper, bool czyToAjaxowyRequest, string UrlReferer, Jezyk aktualnyJezyk, string linkPowrotu = "/")
        {
            if (czyToAjaxowyRequest)
            {
                return null;
            }

            if (linkPowrotu.StartsWith("http"))
            {
                throw new Exception("Linki powrotu nie mogą być absolutne http*");
            }

            if (!string.IsNullOrEmpty(UrlReferer))
            {
                //     string referer = UrlReferer.LocalPath;

                //jesli link powrotu jest bez zmian standardowy
                //|| referer.StartsWith(linkPowrotu + "/") || referer.StartsWith(linkPowrotu + "?")
                //   || referer.StartsWith(linkPowrotu + ",") || referer.EndsWith(linkPowrotu)
                if (linkPowrotu == "/")
                {
                    if (UrlReferer.StartsWith("http"))
                    {
                        return UrlReferer;
                    }
                    linkPowrotu = UrlReferer;
                }
            }


            if (!linkPowrotu.StartsWith("/"))
            {
                linkPowrotu = "/" + linkPowrotu;
            }

            string jezyk = helper.SymbolJezyka(aktualnyJezyk);

            if (!linkPowrotu.StartsWith(jezyk))
            {
                linkPowrotu = jezyk + linkPowrotu;
            }

            if (linkPowrotu.Length > 1)
            {
                linkPowrotu = linkPowrotu.TrimEnd("/");
            }
            return linkPowrotu;

            //if (Calosc.Konfiguracja.WieleJezykowWSystemie)
            //{
            //    if (aktualnyJezyk == null)
            //    {
            //        throw new Exception("Brak jezyka podanego");
            //    }
            //    return "/" + aktualnyJezyk.Symbol + linkPowrotu;
            //}
        }

        /// <summary>
        /// Buduje adres url do zmiany hasła dla klienta.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="klient">Klient dla którego budujemy adres</param>
        /// <param name="jezyk">Język do adresu</param>
        /// <returns></returns>
        public static string ZbudujLink_ZmianyHaslaKlienta(this UrlHelper helper, IKlient klient, Jezyk jezyk)
        {
            TrescBll zmianaHasla = Calosc.DostepDane.PobierzPojedynczy<TrescBll>(x => x.Symbol == "zmiana-hasla", null);
            if (zmianaHasla == null)
            {
                string blad = "Brak tresci o symbolu \"zmiana-hasla\".";
                Calosc.Log.Error(blad);
            }
            Dictionary<string, string> slownik = null;

            string link;
            if (klient != null && !string.IsNullOrEmpty(klient.Gid))
            {
                slownik = new Dictionary<string, string> {{"gid", klient.Gid}};
            }


            link = helper.ZbudujLink(zmianaHasla, klient, jezyk, slownik);

            return link;
        }

        /// <summary>
        /// Buduje adres url do zmiany adresu IP klienta
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static string ZbudujLinkZmianaAdresuIpKlienta(this UrlHelper helper, IKlient k)
        {
            RouteValueDictionary parametry = new RouteValueDictionary();
            parametry.Add("hash", k.GidIp);
            return helper.Action("ZmianaIp", "Klienci", parametry);
        }

        /// <summary>
        /// Buduje link do strony bezwzgledny - rozpoczynajacy sie od http lub https
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="adres">adres link</param>
        /// <returns>gotowy link z doklejoną domeną</returns>
        public static string ZbudujLinkZewnetrzny(this UrlHelper helper, string adres)
        {
            return DodajSerwerDoLinku(adres).ToLower();
        }

        /// <summary>
        /// Buduje symbol języka dodanany do adresów.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="jezyk"></param>
        /// <returns></returns>
        public static string SymbolJezyka(this UrlHelper helper, Jezyk jezyk)
        {
            return SymbolJezyka(jezyk);
        }

        private static string SymbolJezyka(Jezyk jezyk)
        {
            if (Config.WieleJezykowWSystemie)
            {
                return "/" + jezyk.Symbol;
            }
            return "";
        }

        private static RouteValueDictionary KulturaJezyka(Jezyk jezyk, RouteValueDictionary parametry)
        {
            if (Config.WieleJezykowWSystemie)
            {
                parametry.Add("culture", jezyk.Symbol);
            }
            return parametry;
        }

        public static string WygenerujLinkDoPobraniaZalacznika(this UrlHelper helper, string sciezka)
        {
            if (!string.IsNullOrEmpty(sciezka))
            {
                return string.Format("<a download href=\"{0}\">Pobierz</a>", sciezka);
            }
            return "";
        }

        /// <summary>
        /// czy jestesmy na stronie z produktami
        /// </summary>
        /// <param name="slownikParametrowRoutingu">parametry routing jawnie podane - przydatne dla testow jednostkowych. Normalnie NIE podawać!</param>
        /// <returns></returns>
        [NonAction]
        public static bool AktualnaStronaToStronaProduktow(this UrlHelper helper, out long idKategoriiProduktow, RouteValueDictionary slownikParametrowRoutingu = null)
        {
            idKategoriiProduktow = 0;
            if (slownikParametrowRoutingu == null)
            {
                slownikParametrowRoutingu = helper.RequestContext.RouteData.Values;
            }

            if (slownikParametrowRoutingu.IsEmpty())
            {
                throw new Exception("Brak parametrów routingu - skąd link do sprawdzania?");
            }

            object obiekt;
            if (slownikParametrowRoutingu.TryGetValue("Kategoria", out obiekt))
            {
                if (long.TryParse(obiekt.ToString(), out idKategoriiProduktow))
                {
                    return true;
                }
            }

            //jak nie ma z parametrow routingow to z linku w ostatecznosci
            string linkDoZbadania;

            if (helper.RequestContext.HttpContext.Request.IsAjaxRequest() && helper.RequestContext.HttpContext.Request.UrlReferrer != null)
            {
                linkDoZbadania = helper.RequestContext.HttpContext.Request.UrlReferrer.LocalPath;
            }
            else
            {
                linkDoZbadania = helper.RequestContext.HttpContext.Request.Url.LocalPath;
            }

            //jeszcze link moze byc tylko do produktow glownych wszystkich
            if (linkDoZbadania.StartsWith("/" + Calosc.Konfiguracja.AdresStronyZProduktami + "?") || linkDoZbadania.Equals("/" + Calosc.Konfiguracja.AdresStronyZProduktami, StringComparison.InvariantCultureIgnoreCase) ||
                linkDoZbadania.StartsWith("/" + Calosc.Konfiguracja.AdresStronyZProduktami + "/") || linkDoZbadania.Equals("/" + Calosc.Konfiguracja.LinkAlternatywnyStronyProduktow, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            return false;
        }


    }
}