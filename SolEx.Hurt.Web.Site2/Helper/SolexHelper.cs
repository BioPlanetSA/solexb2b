using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using AngleSharp.Network.Default;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Web.Site2.PageBases;
using Klient = SolEx.Hurt.Core.Klient;

namespace SolEx.Hurt.Web.Site2.Helper
{
    public class SolexHelper : ISolexHelper
    {
        public string cookieLogowaniaNazwa;

        public string SzyfrujTrescCookisaLogowania(string tresc)
        {
            return tresc;
        }
        public string DeSzyfrujTrescCookisaLogowania(string tresc)
        {
            return tresc;
        }

        public ISolexBllCalosc Calosc = SolexBllCalosc.PobierzInstancje;
        public ICookieHelper CookieHelper = Helpers.CookieHelper.PobierzInstancje;

        /// <summary>
        /// Sprawdzamy czy mozna pobrac sesje z cookisa
        /// </summary>
        /// <param name="daneSesji"></param>
        /// <returns></returns>
        protected bool CzyMoznaZalogowacNaPodstawieCookisa(out Sesja daneSesji)
        {
            daneSesji = null;
            //pobranie cookisa
            var cookieWartosc = CookieHelper.GetCookieValue(cookieLogowaniaNazwa);
            if (string.IsNullOrEmpty(cookieWartosc ))
            {
                return false;
            }

            //odszyfrowanie wg. tanego klucza
            Guid sesjaID;
            if (!Guid.TryParse(DeSzyfrujTrescCookisaLogowania(cookieWartosc), out sesjaID))
            {
                return false;
            }

            //sprawdzenie sesji czy jest takowa i ustawienie jak cos danych
            daneSesji = SolexBllCalosc.PobierzInstancje.DostepDane.Select<Sesja>(x=> x.Id == sesjaID).FirstOrDefault();
            if (daneSesji == null || daneSesji.DataZakonczenia.HasValue)
            {
                //kasujemy lipne cookis
                CookieHelper.DeleteCookie(cookieLogowaniaNazwa);
                return false;
            }

            return true; //udalo sie pobrać sesje
        }

        /// <summary>
        /// sprawdzamy czy mozna pobrac sesje z headera requestu lub parametrów POST/PUT/Delete
        /// </summary>
        /// <param name="daneSesji"></param>
        /// <returns></returns>
        protected bool CzyMoznaZalogowacNaPodstawieNaglowkaRequestu(out Sesja daneSesji)
        {
            daneSesji = null;
            string sesjaGuid = HttpContext.Current.Request.Headers["SesjaGuid"];
            if (string.IsNullOrEmpty(sesjaGuid))
            {
                return false;
            }

            daneSesji = SolexBllCalosc.PobierzInstancje.DostepDane.Select<Sesja>(x=> x.Id == Guid.Parse( sesjaGuid) ).FirstOrDefault();
            if (daneSesji == null)
            {
                return false;
            }

            if (daneSesji.DataZakonczenia != null)
            {
                daneSesji = null;
                return false;
            }
            return true;
        }

        

        public Guid ZalogujKlienta(IKlient klient, string ipAdres, string userAgent)
        {
            if (klient.Id == 0)
            {
                throw new Exception("Nie można zalogować klienta o id == 0");
            }

            //nowa sesja
            Sesja sesjaLogowania = new Sesja( Guid.NewGuid(), klient.Id, userAgent, ipAdres, DateTime.Now);          
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(sesjaLogowania);
            return sesjaLogowania.Id;

            #region stara metoda

            //bool isPersistent = true;
            //string parametryAUthCookie = "";
            //StringBuilder sb = new StringBuilder();
            //List<string> klucze = AuthCookieWartosci.Keys.ToList();
            //foreach (string klucz in klucze)
            //{
            //    AuthCookieWartosci[klucz] = null;
            //}
            //AuthCookieWartosci["klientID"] = klient.Id;
            //AuthCookieWartosci["OddzialDoJakiegoNalezyKlient"] = klient.OddzialDoJakiegoNalezyKlient;
            //if (klient.Role.Contains(RoleType.Oddzial))
            //{
            //    AuthCookieWartosci["oddzialID"] = klient.Id;
            //}

            //if (klient.Role.Contains(RoleType.Przedstawiciel))
            //{
            //    AuthCookieWartosci["przedstawicielID"] = klient.Id;
            //}

            //if (przedstawicielId.HasValue)
            //{
            //    AuthCookieWartosci["przedstawicielID"] = przedstawicielId.Value;
            //}

            //if (oddzialID.HasValue)
            //{
            //    AuthCookieWartosci["oddzialID"] = oddzialID.Value;
            //}

            //foreach (KeyValuePair<string, long?> pair in AuthCookieWartosci)
            //{
            //    sb.Append(string.Format("{0}:{1};", pair.Key, pair.Value));
            //}
            //parametryAUthCookie = sb.ToString().TrimEnd(';');

            //FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
            //    klient.Id.ToString(),
            //       DateTime.Now,
            //      DateTime.Now.AddHours(SolexBllCalosc.PobierzInstancje.Konfiguracja.CzasWaznosciLogowania),
            //       isPersistent,
            //       parametryAUthCookie,
            //       FormsAuthentication.FormsCookiePath);

            //// Encrypt the ticket.
            //string encTicket = FormsAuthentication.Encrypt(ticket);
            //// Create the cookie
            //try
            //{
            //    HttpContext.Current.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
            //}
            //catch (Exception)
            //{
            //    //zdarza się czasem wyjątek, ale nie robi problemów
            //}

            #endregion
        }

        private void ZaladujSesje(Jezyk jezyk = null)
        {
            Sesja sesjaDoZaldowania = null;
            if (!this.CzyMoznaZalogowacNaPodstawieCookisa(out sesjaDoZaldowania))
            {
                this.CzyMoznaZalogowacNaPodstawieNaglowkaRequestu(out sesjaDoZaldowania);
            }

            if(sesjaDoZaldowania == null)
            { 
                //ladowanie klienta NIEzalogowanego
                AktualnyKlient = SolexBllCalosc.PobierzInstancje.Klienci.KlientNiezalogowany();
            }
            else
            {
                //mamy to
                AktualnaSesjaID = sesjaDoZaldowania.Id;
                if (sesjaDoZaldowania.KlientId == 0)
                {
                    throw new Exception("Nie można logować klientów niezalogowanych (id==0)");
                }

                AktualnyKlient = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(sesjaDoZaldowania.KlientId);
                if (AktualnyKlient == null)
                {
                    throw new Exception("Wyczyść cache przeglądarki - logowanie na nieistniejąego klienta");
                }

                if (sesjaDoZaldowania.PrzedstawicielId.HasValue)
                {
                    AktualnyPrzedstawiciel = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(sesjaDoZaldowania.PrzedstawicielId.Value);
                    if (AktualnyKlient == null)
                    {
                        throw new Exception("Wyczyść cache przeglądarki - logowanie na nieistniejąego przedstawiciela");
                    }
                }
            }

            //kompatybilnosc wsteczna - syf do usuniecia
            HttpContext.Current.Items["klientID"] = AktualnyKlient.Id;

            UstawAktualnyJezyk(jezyk, new HttpRequestWrapper(System.Web.HttpContext.Current.Request).IsAjaxRequest());

            if (AktualnyKlient.Id != 0)
            {
                UstawAktualnyKoszyk();
            }
        //    UstawTlumaczenieWLocie();
        }

        /// <summary>
        /// NIE WOLNO uzyważ tej metody w innych celach niż testowych
        /// </summary>
        public SolexHelper()
        {
            if (HttpContext.Current != null)
            {
                throw new Exception("Nie wolno używać tego konstruktora inaczej niż do testów");
            }
        }


        public SolexHelper(Jezyk jezyk)
        {
            //tylko to uuzpelniemy cooksa name bo tu sie tworzy sesjaHelper - specjalnie tak jest, nie zmieniać tego
            cookieLogowaniaNazwa = Tools.PobierzInstancje.GetMd5Hash("solexb2b" + HttpContext.Current.Request.ServerVariables["APP_POOL_ID"]);
            this.ZaladujSesje(jezyk);
        }

       
        public static SolexHelper PobierzInstancjeZCache()
        {
            if (HttpContext.Current != null && HttpContext.Current.Items["sh"] != null)
            {
                return ((SolexHelper) HttpContext.Current.Items["sh"]);
            }
            return null;
        }

        public static void UtworzNowy(SolexHelper sh)
        {
            HttpContext.Current.Items["sh"] = sh;
        }

        public static void WyczyscObiektKoszykaZSolexHelpera(IList<KoszykBll> obj)
        {
            if (PobierzInstancjeZCache() != null)
            {
                PobierzInstancjeZCache().PrzeladujAktualnyKoszyk();
            }
        }

        public bool TlumaczenieWLocie { get; private set; }

        //BCH: komentuje - bio nie wykorzystuje tlumaczenia w locie
        //private void UstawTlumaczenieWLocie()
        //{
        //    //ustaw tlumaczenie w locie
        //    this.TlumaczenieWLocie = false;
        //    if (SolexBllCalosc.PobierzInstancje.Konfiguracja.TlumaczenieWLocie)
        //    {
        //        if (AktualnyKlient.Id != 0) //zalogowany
        //        {
        //            if (AktualnyKlient.CzyAdministrator)
        //            {
        //                if (!HttpContext.Current.Request.RawUrl.Contains("/api/"))
        //                {
        //                    this.TlumaczenieWLocie= true;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            //niezalogowni tylko w domenie solexb2b.com
        //            if (HttpContext.Current != null)
        //            {
        //                TlumaczenieWLocie = HttpContext.Current.Request.Url.Host.Contains(".solexb2b.com") || HttpContext.Current.Request.Url.Host.Contains("localhost");
        //            }
        //        }
        //    }
        //}

        public Jezyk AktualnyJezyk { get; private set; }   //propertis NIE moze byc wirtualny!

        public string cookisJezykNiezalogowanyNazwa = "solexB2BJezyk";

        private int cookiesJezykaNiezalogowanyKlientPobierz()
        {
            string wartoscCookisa = CookieHelper.GetCookieValue(this.cookisJezykNiezalogowanyNazwa);
            if (!string.IsNullOrEmpty(wartoscCookisa))
            {
                int jezykIdCookis = 0;
                if (int.TryParse(wartoscCookisa, out jezykIdCookis))
                {
                    if (Calosc.Konfiguracja.JezykiWSystemie.ContainsKey(jezykIdCookis))
                    {
                        return jezykIdCookis;
                    }
                }
            }
            return 0;
        }

        public void UstawAktualnyJezyk(Jezyk jezyk, bool czyToZapytanieAJAX)
        {
            if (jezyk != null)
            {
                if (AktualnyKlient.Id != 0)
                {
                    //jesli jest NIE ajax to napidsujemy w bazie dla klienta - jak ajax to nie ruszamy
                    if (!czyToZapytanieAJAX && jezyk.Id != AktualnyKlient.JezykId)
                    {
                        //jak jest inny jezyk niz aktualny to zmiana jezyka - TYLKO jesli request NIE AJAX
                        AktualnyKlient.JezykId = jezyk.Id;
                        Calosc.DostepDane.AktualizujPojedynczy(AktualnyKlient as Klient);
                    }
                    //zawsze pobieramy z klienta jezyk
                    AktualnyJezyk = Calosc.Konfiguracja.JezykiWSystemie[AktualnyKlient.JezykId];
                }
                else
                {
                    //niezalogowany
                    AktualnyJezyk = jezyk;

                    //zapisanie cookisa dla niezalogowanego klienta
                    int jezykIDzCookisa = cookiesJezykaNiezalogowanyKlientPobierz();
                    if (jezykIDzCookisa != jezyk.Id)
                    {
                        CookieHelper.SetCookie(cookisJezykNiezalogowanyNazwa, jezyk.Id);
                    }
                }
                return;
            }
            
            //tu wejdzie tylko wtedy jak NIE ma jezyka podanego
                //nie ma jezyka, i klient nie zalogowany - proba pobrania z cookisa jak jest
                if (AktualnyKlient.Id == 0)
                {
                    //cookis czy jest z jezykiem - jak jest to bierzmey z niego, jak nie ma to kaplica
                    int jezykIDzCookisa = cookiesJezykaNiezalogowanyKlientPobierz();
                    if (jezykIDzCookisa != 0)
                    {
                        AktualnyJezyk = Calosc.Konfiguracja.JezykiWSystemie[jezykIDzCookisa];
                        return;
                    }
                }
                else
                {
                    AktualnyJezyk = Calosc.Konfiguracja.JezykiWSystemie[AktualnyKlient.JezykId]; 
                    return;
                }
            

            //niezalogowany lub brak jezyka na kliencie
            AktualnyJezyk = Calosc.Konfiguracja.JezykiWSystemie[Calosc.Konfiguracja.JezykIDDomyslny];
        }

        private KoszykBll _koszyk = null;

        public KoszykBll AktualnyKoszyk => _koszyk ?? (_koszyk = UstawAktualnyKoszyk());

        private KoszykBll UstawAktualnyKoszyk()
        {
            if (this.AktualnyKlient.Id == 0)
            {
                throw new HttpException(401, "Dla klienta 0 nie można pobierać koszyka");
                return null;
            }

            TypKoszyka typKoszyka = TypKoszyka.Koszyk;
            long idKoszyka = SolexBllCalosc.PobierzInstancje.ProfilKlienta.PobierzWartosc<int>(AktualnyKlient, TypUstawieniaKlienta.WybranyKoszyk, typKoszyka.ToString());
            KoszykBll koszyk = null;
            if (idKoszyka != 0)
            {
                koszyk = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<KoszykBll>(idKoszyka, AktualnyJezyk.Id, AktualnyKlient);
            }
            if (koszyk == null)
            {
                //todo: spradzic z subkontami czy jest OK
                koszyk = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<KoszykBll>(AktualnyJezyk.Id, AktualnyKlient, x => x.Typ == TypKoszyka.Koszyk && x.KlientId == AktualnyKlient.Id).FirstOrDefault();
                if (koszyk == null)
                {
                    idKoszyka = SolexBllCalosc.PobierzInstancje.Koszyk.StworzNowyKoszyk(AktualnyKlient, "", typKoszyka);
                    koszyk = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<KoszykBll>(idKoszyka, AktualnyJezyk.Id, AktualnyKlient);
                }
                SolexBllCalosc.PobierzInstancje.ProfilKlienta.DodajWartosc(AktualnyKlient, TypUstawieniaKlienta.WybranyKoszyk, koszyk.Id, typKoszyka.ToString());
            }

            koszyk.PrzedstawicielKtoryEdytujeKoszykWImieniuKlienta = this.AktualnyPrzedstawiciel;

            return koszyk;
        }

        public void ZmienNaAktualnejSesjiKlienta(long nowyKlientID)
        {
            if (AktualnyKlient.Id == 0)
            {
                throw new Exception("Nikt nie jest zalogowany - nie można zmieniać sesji niczyjej");
            }

            //todo: WALIDACJa doadć 
            //zeby sie przelaczyc albo jestem przedstawicielem
            //if (AktualnyPrzedstawiciel == null)
            //{
            //    //czy koles aktualnie jest adminem lub pracownikiem co moze zmienaic 
            //    if (!(AktualnyKlient.CzyAdministrator || AktualnyKlient.Role.Contains(RoleType.Przedstawiciel)))
            //    {
            //        throw new Exception("Aktualna sesja nie jest sesją przedstawicial - tylko w takich sesjach można zmieniać klientów dynamicznie");
            //    }
            //}

            Sesja sesjaDoZmiany = this.pobranieSesjiIWalidacja(this.AktualnaSesjaID.Value);

            ////upewniamy sie ze pobrana sesja jest identyczna z tym co juz mamy
            //if (sesjaDoZmiany.KlientId != AktualnyKlient.Id || sesjaDoZmiany.PrzedstawicielId != AktualnyPrzedstawiciel.Id)
            //{
            //    throw new Exception("Sesja w bazie różni się od tej od klienta");
            //}

            sesjaDoZmiany.KlientId = nowyKlientID;

            if (sesjaDoZmiany.PrzedstawicielId == null)
            {
                sesjaDoZmiany.PrzedstawicielId = AktualnyKlient.Id; //WALIDAJCA!!!
            }

            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(sesjaDoZmiany);
               
            //nie mozna juz korzystac z tej seji - niszczenie danych
            this.ZaladujSesje(null);
        }

        private Sesja pobranieSesjiIWalidacja(Guid sesjaID)
        {
            Sesja sesjaDoZmiany = SolexBllCalosc.PobierzInstancje.DostepDane.Select<Sesja>(x => x.Id == sesjaID).FirstOrDefault();
            if (sesjaDoZmiany == null)
            {
                throw new Exception("Brak sesji w bazie!");
            }

            if (sesjaDoZmiany.DataZakonczenia.HasValue)
            {
                throw new Exception("Sesja już jest zakończona!");
            }

            return sesjaDoZmiany;
        }


        public void Wyloguj(Guid? sesjaId = null)
        {
            if (this.AktualnyKlient.Id == 0)
            {
                throw new Exception("Najpierw zaloguj się!");
            }

            //jak jest podany NULL to wylogowujemy aktualna sesje
            if (sesjaId == null)
            {
                sesjaId = this.AktualnaSesjaID;
            }

            //zamkniecie sesji
            Sesja sesjaDoZmiany = this.pobranieSesjiIWalidacja(sesjaId.Value);

            if (!this.AktualnyKlient.CzyAdministrator && sesjaDoZmiany.KlientId != AktualnyKlient.Id)
            {
                throw new Exception("Nie możesz wylogować sesji, dlatego że nie jesteś jej właścicielem");
            }

            sesjaDoZmiany.DataZakonczenia = DateTime.Now;
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(sesjaDoZmiany);

            //jesli to aktualna sesje wylogowalismy to czyscimy aktualnego cookisa
            if (sesjaId == this.AktualnaSesjaID)
            {
                CookieHelper.DeleteCookie(cookieLogowaniaNazwa);
            }
        }

        public virtual IKlient AktualnyKlient { get; private set; }

        public void PrzeladujAktualnyKoszyk()
        {
            this._koszyk = null;
            UstawAktualnyKoszyk();
        }

        public Guid? AktualnaSesjaID { get; set; }

        public IKlient AktualnyPrzedstawiciel { get; set; }        
    }

    public abstract class CustomWebViewPage : WebViewPage
    {

        public SolexHelper Solex
        {
            get
            {
                return ((SolexControler)ViewContext.Controller).SolexHelper;
                //return SolexHelper.PobierzInstancjeZCache();
                //return new SolexHelper(this.ViewContext);
            }
        }

        public override void InitHelpers()
        {
            base.InitHelpers();
            //Solex = new SolexHelper(base.ViewContext, this);
        }
    }

    public abstract class CustomWebViewPage<T> : WebViewPage<T>
    {
        public SolexHelper SolexHelper
        {
            get
            {
                return ((SolexControler) ViewContext.Controller).SolexHelper;
                //return SolexHelper.PobierzInstancjeZCache(); //base.ViewContext
            }
        }

        public override void InitHelpers()
        {
            base.InitHelpers();
            //SolexHelper = new SolexHelper(base.ViewContext, this);
        }
    }


}