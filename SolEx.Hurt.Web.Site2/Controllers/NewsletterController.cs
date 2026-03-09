using System;
using System.Collections.Generic;
using System.Web.Mvc;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Web.Site2.Helper;
using SolEx.Hurt.Web.Site2.PageBases;
using Klient = SolEx.Hurt.Core.Klient;

namespace SolEx.Hurt.Web.Site2.Controllers
{
    [RoutePrefix("newsletter")]
    public class NewsletterController : SolexControler
    {
        [Route("Wypisz/{email}/{klucz}")]
        public ActionResult Wypisz(string email, string klucz)
        {
            Klient klient = SolexBllCalosc.PobierzInstancje.Klienci.PobierzPologinie(email);
            Dictionary<string, string> pars = new Dictionary<string, string>();
            string kluczDoWypisania = SolexBllCalosc.PobierzInstancje.Klienci.KluczDoKlientaWypisanieZapisaniaZNewsletera(klient);
            if (klient == null || !kluczDoWypisania.Equals(klucz, StringComparison.InvariantCultureIgnoreCase))
            {
                pars.Add("wiadomosc", "Błędne dane");
            }
            else
            {
                klient.ZgodaNaNewsletter = false;
                SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(klient);
                pars.Add("wiadomosc", "Zostałeś wypisany z newslettera");
            }
            return AkcjaPowrotu();
        }

        [Route("Zapisz/{email}/{klucz}")]
        public ActionResult Zapisz(string email, string klucz)
        {
            Klient klient = SolexBllCalosc.PobierzInstancje.Klienci.PobierzPologinie(email);
     //       Dictionary<string, string> pars = new Dictionary<string, string>();
            if (klient == null || !SolexBllCalosc.PobierzInstancje.Klienci.KluczDoKlientaWypisanieZapisaniaZNewsletera(klient).Equals(klucz, StringComparison.InvariantCultureIgnoreCase))
            {
      //          pars.Add("wiadomosc", "Błędne dane");
            }
            else
            {
                klient.ZgodaNaNewsletter = true;
                SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(klient);
           //     pars.Add("wiadomosc", "Zostałeś zapisany do newslettera");
            }
            return AkcjaPowrotu();
        }

        [Route("ZgodaNaNewsletter")]
        public PartialViewResult ZgodaNaNewsletter()
        {
            if (SesjaHelper.PobierzInstancje.CzyKlientZalogowany)
            {
                return PartialView("_ZgodaNewsletter",SolexHelper.AktualnyKlient);
            }
                       
            return PartialView("_ZapiszDoNewslettera");
        }

        [HttpPost]
        [Route("ZapiszDoNewslettera")]
        public JsonResult ZapiszDoNewslettera(string adresemail)
        {
            string ip = SesjaHelper.PobierzInstancje.IpKlienta;
            string wiadomosc = "Zapisanie nie powiodło się";

            if (!string.IsNullOrEmpty(adresemail))
            {
                var newsletter = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<NewsletterZapisani>(x => x.Email == adresemail, SolexHelper.AktualnyKlient);

                //Jeżeli zapisany i nie wypisany
                if (newsletter != null && newsletter.DataWypisania == null)
                {
                    wiadomosc = "Jesteś już zapisany do newslettera.";
                }
                else
                {
                    //jesli jest - ale wypisany - to kasujemy wypisanie tylko
                    if (newsletter != null && newsletter.DataWypisania != null)
                    {
                        newsletter.DataWypisania = null;
                    }
                    //Jeśli mail nie jest w bazie 
                    if (newsletter == null)
                    {
                        newsletter = new NewsletterZapisani
                        {
                            Email = adresemail,
                            AdersIp = ip,
                            DataZapisania = DateTime.Now
                        };
                    }

                    try
                    {
                        SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(newsletter);
                        SolexBllCalosc.PobierzInstancje.Statystyki.ZdarzenieZapisDoNewslettera(newsletter);
                        wiadomosc = "<h6>Dziękujemy,</h6> za zapisanie się do naszego newslettera. Na podany adres email wysłano potwierdzenie zapisu.";
                    }
                    catch (Exception e)
                    {
                        Log.ErrorFormat("Błąd dodawania do newslettera emaila: {0}", newsletter.Email);
                        Log.Error(e);
                    }
                }
            }
            return Json(new { odp = wiadomosc });
        }

        [Route("NewsletterPodglad/{id}")]
        public ActionResult NewsletterPodglad(int id)
        {
            NewsletterKampania newsletterKampania = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<NewsletterKampania>(x => x.Id == id, SolexHelper.AktualnyKlient);
            var email = MailHelper.PobierzInstancje.GenerujPodgladNewslettera(newsletterKampania, SolexHelper.AktualnyKlient);          
            return Content(email.TrescWiadomosci);
        }

        [Route("WyslijNewsletter/{id}")]
        public ActionResult WyslijNewsletter(int id)
        {
            //jesli kampania NIE jest do wyslania to ustawiamy tylko status do wysalnie i NIE WYSYLAMY
            NewsletterKampania kampania = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<NewsletterKampania>(id, SolexHelper.AktualnyKlient);
            if (kampania.Status == StatusNewsletter.Przygotowywany)
            {
                kampania.Status = StatusNewsletter.ZaplanowanyDoWysłania;
                SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(kampania);
                return null;
            }

            if (kampania.Status == StatusNewsletter.Zakończony)
            {
                throw new Exception("Kampania już jest zakończona - nie można jej wysłać powtórnie");
            }

            MailHelper.PobierzInstancje.WyslijAktywneMailingi(id);
            return AkcjaPowrotu();
        }


        [Route("ZakonczWysylanie/{id}")]
        public ActionResult ZakonczWysylanie(int id)
        {
            //jesli kampania NIE jest do wyslania to ustawiamy tylko status do wysalnie i NIE WYSYLAMY
            NewsletterKampania kampania = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<NewsletterKampania>(id, SolexHelper.AktualnyKlient);
            if (kampania.Status != StatusNewsletter.Zakończony)
            {
                kampania.Status = StatusNewsletter.Zakończony;
                SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(kampania);
            }
            return AkcjaPowrotu();
        }


    }
}