using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Web.Site2.Helper;
using SolEx.Hurt.Web.Site2.PageBases;
using Klient = SolEx.Hurt.Model.Klient;

namespace SolEx.Hurt.Web.Site2.Controllers
{
    [RoutePrefix("logowanie")]
    public class LogowanieController : SolexControler
    {
        [HttpGet]
        public ActionResult Logowanie( bool logowaniepokazujcaptcha=false)
        {
            if (SesjaHelper.PobierzInstancje.CzyKlientZalogowany)
            {
                return RedirectToAction("StronaGlowna", "Tresci");
            }
            return PartialView("_Logowanie", new Tuple<string, LockSystem, bool>(null, Lock, logowaniepokazujcaptcha));
        }

        private LockSystem _s;

        protected LockSystem Lock
        {
            get
            {
                if (_s == null)
                {
                    _s = new LockSystem();
                }
                return _s;
            }
        }

        [HttpPost]
        public ActionResult Logowanie(Models.Logowanie logowanie, bool logowaniepokazujcaptcha=false)
        {
            try
            {
                if (logowaniepokazujcaptcha && !SprawdzCaptcha())
                {
                    throw new Exception("Niepoprawna captcha");
                } 
                Core.Klient klientDoZalogowania = KlienciHelper.PobierzInstancje.PobierzKlientaDoZalogowania(logowanie, logowaniepokazujcaptcha);


                if (!Lock.MozeSieZalogowac(klientDoZalogowania))
                {
                    throw new Exception("System zablokowany - nie możesz się logować");
                }
                string urlDoPrzekierowania = null;

                if (Calosc.Klienci.CzyWymaganaZmianaHasla(klientDoZalogowania))
                {
                    SolexBllCalosc.PobierzInstancje.Klienci.ResetHasla(klientDoZalogowania);
                    this.Wylogowanie();
                    urlDoPrzekierowania = "/wymagana-zmiana-hasla";
                }

                if (!string.IsNullOrEmpty(klientDoZalogowania.Gid))
                {
                    urlDoPrzekierowania = Url.ZbudujLink_ZmianyHaslaKlienta(klientDoZalogowania, SolexHelper.AktualnyJezyk);
                }
                else
                {
                    klientDoZalogowania.DataOstatniegoLogowania = DateTime.Now;
                    SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(klientDoZalogowania);
                    Guid guid = SolexHelper.ZalogujKlienta(klientDoZalogowania, SesjaHelper.PobierzInstancje.IpKlienta, Request.UserAgent);


                    CookieHelper.PobierzInstancje.SetCookie(SolexHelper.cookieLogowaniaNazwa, SolexHelper.SzyfrujTrescCookisaLogowania(guid.ToString()));

                    //kasowanie cookisa dla jezyka bez logowania
                    CookieHelper.PobierzInstancje.DeleteCookie(SolexHelper.cookisJezykNiezalogowanyNazwa);

                    //dodajemy info do statystyk o poprawnym logowaniu
                    Calosc.Klienci.DodajZdarzenieLogowannie(logowanie.Uzytkownik, true);


                    Jezyk jezykKlienta = SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykiWSystemie[klientDoZalogowania.JezykId];

                    if (!string.IsNullOrEmpty(Request.QueryString["ReturnUrl"]))
                    {
                        string adres = Request.QueryString["ReturnUrl"];
                        urlDoPrzekierowania = $"{SolexBllCalosc.PobierzInstancje.Konfiguracja.wlasciciel_AdresPlatformy}/{adres.TrimStart('/')}";
                    }
                    else if (klientDoZalogowania.CzyAdministrator || klientDoZalogowania.Role.Contains(RoleType.Pracownik))
                    {
                        urlDoPrzekierowania = Url.ZbudujLinkDoAdmina(typeof(Core.Klient), jezykKlienta);
                    }
                    else if (klientDoZalogowania.Role.Contains(RoleType.Oddzial) || klientDoZalogowania.Role.Contains(RoleType.Przedstawiciel))
                    {
                        urlDoPrzekierowania = Url.ZbudujLinkDoAdmina(typeof(Core.Klient), jezykKlienta);
                    }
                    else
                    {
                        urlDoPrzekierowania = "/";
                    }
                }


                if (string.IsNullOrEmpty(urlDoPrzekierowania))
                {
                    Calosc.Klienci.DodajZdarzenieLogowannie(logowanie.Uzytkownik, false);
                }
                else
                {
                    Response.Redirect(urlDoPrzekierowania, true);
                }
            }
            catch (Exception e)
            {
                Calosc.Klienci.DodajZdarzenieLogowannie(logowanie.Uzytkownik, false);
                return PartialView("_Logowanie", new Tuple<string, LockSystem, bool>(e.Message, Lock, logowaniepokazujcaptcha));
            }
            return null; //tu nigdy nie trafimy
        }

        /// <summary>
        /// Wylogowanie z aktualnej sesji
        /// </summary>
        /// <returns></returns>
        public ActionResult Wylogowanie()
        {
            //tylko dla zalogowanych
            if (SolexHelper.AktualnyKlient.Id != 0)
            {
                SolexHelper.Wyloguj();
                //  Session.Abandon();
                //jeszcze raz strona musi sie przeladowac zeby miec pewnosc ze wszystko jest wyczyszczone
            }

            //jesli jest treść o symbolu logout to na nia kierujemy
            TrescBll stronaWylogowanie = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<TrescBll>(x=>!string.IsNullOrEmpty(x.Symbol) && x.Symbol.Equals("logout", StringComparison.InvariantCultureIgnoreCase) && x.Aktywny && (x.Dostep == AccesLevel.Niezalogowani || x.Dostep == AccesLevel.Wszyscy) , SolexHelper.AktualnyJezyk.Id,null,false);
            // strona s treścią musi mieć link alternatywny bo jak nie to powstanie pętla bo w routingu logout kieruje do tej akcji 
            if (stronaWylogowanie == null || string.IsNullOrEmpty(stronaWylogowanie.LinkAlternatywny))
            {
                if (stronaWylogowanie != null)
                {
                    SolexBllCalosc.PobierzInstancje.Log.Error("Strona wylogowania nie posiada linku alternatywnego.");
                }
                return Redirect("/");
            }
            return Redirect( Url.ZbudujLink(stronaWylogowanie,SolexHelper.AktualnyKlient, SolexHelper.AktualnyJezyk) );
        
        }

        [Route("ListaKontrahentow")]
        public PartialViewResult ListaKontrahentow()
        {
            if (SolexHelper.AktualnyKlient.CzyAdministrator || SolexHelper.AktualnyKlient.Role.Contains(RoleType.Przedstawiciel) || SolexHelper.AktualnyKlient.Role.Contains(RoleType.Pracownik))
            {
                return PartialView("ListaKontrahentow",true);
            }

            if (SolexHelper.AktualnyPrzedstawiciel != null)
            {
                if (SolexHelper.AktualnyPrzedstawiciel.Role.Contains(RoleType.Przedstawiciel) || SolexHelper.AktualnyPrzedstawiciel.Role.Contains(RoleType.Pracownik))
                {
                    return PartialView("ListaKontrahentow",false);
                }
            }
            return null;
        }

        [Route("przeloguj")]
        public ActionResult Przeloguj()
        {
            if (SolexHelper.AktualnyPrzedstawiciel == null)
            {
                return Content("Brak uprawnień aby przelogować klienta!");
            }
            if (SolexHelper.AktualnyPrzedstawiciel.CzyAdministrator|| SolexHelper.AktualnyPrzedstawiciel.Role.Contains(RoleType.Przedstawiciel) || SolexHelper.AktualnyPrzedstawiciel.Role.Contains(RoleType.Pracownik))
            {
                SolexHelper.ZmienNaAktualnejSesjiKlienta(SolexHelper.AktualnyPrzedstawiciel.Id);
            }
            return Redirect("~/Admin/Lista?typ=SolEx.Hurt.Core.Klient,SolEx.Hurt.Core");
        }
    }
}