using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers.MojeDane;
using SolEx.Hurt.Model.Web;
using SolEx.Hurt.Web.Site2.PageBases;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Web.Site2.Helper;
using Klient = SolEx.Hurt.Core.Klient;

namespace SolEx.Hurt.Web.Site2.Controllers
{
    public class MojeDaneController : SolexControler
    {
        /// <summary>
        /// Pobiera dane aktualnie zalogowanego klienta i wyświetla je na stronie
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult ZmienDane(string opis, string emailNaJakiOdeslacOdpowiedz = "")
        {
            Adres adres;
            MojeDaneModel mojeDaneModel;            
            
            if (SolexHelper.AktualnyKlient.Adresy.Any(a=>a.TypAdresu==TypAdresu.Glowny))
            {
                adres = SolexHelper.AktualnyKlient.Adresy.FirstOrDefault(a => a.TypAdresu == TypAdresu.Glowny) as Adres;
            }
            else
            {
                adres = SolexHelper.AktualnyKlient.Adresy.FirstOrDefault() as Adres;
            }
            if (adres != null)
            {
                mojeDaneModel = new MojeDaneModel(SolexHelper.AktualnyKlient.Nazwa, SolexHelper.AktualnyKlient.Nip, adres.UlicaNr, adres.Miasto,
                    adres.KodPocztowy, adres.Kraj,
                    SolexHelper.AktualnyKlient.Telefon, "", SolexHelper.AktualnyKlient.Email, false, false);
            }
            else
            {
                mojeDaneModel = new MojeDaneModel(SolexHelper.AktualnyKlient.Nazwa, SolexHelper.AktualnyKlient.Nip, "", "", "","",SolexHelper.AktualnyKlient.Telefon, "", SolexHelper.AktualnyKlient.Email, false, false);
            }

            mojeDaneModel.BlokujFormularz = false;
            
            //jezeli nie wpisano adresu w ustawieniach kontrolki i nie ma opiekuna blokujemy formularz
            if (SolexHelper.AktualnyKlient.Opiekun == null  && string.IsNullOrEmpty(emailNaJakiOdeslacOdpowiedz))
            {
                mojeDaneModel.BlokujFormularz = true;
            }

            mojeDaneModel.EmailNaJakiOdeslacOdpowiedz = emailNaJakiOdeslacOdpowiedz;

            List<ParametryPola> pola = Refleksja.WygenerujParametryPol(mojeDaneModel);

            mojeDaneModel.Pola = pola;
            mojeDaneModel.Opis = opis;
            return PartialView("_ZmienDane", mojeDaneModel);
        }

        /// <summary>
        /// Wysyła prośbę na maila o zmianę danych, zwraca odpowiednią informację czy sie powiodło czy nie
        /// </summary>
        /// <param name="mojeDaneModel"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult ZmienDane(MojeDaneModel mojeDaneModel)
        {
            mojeDaneModel.Rezultat = true;
            bool result = true;
            //todo: WENTURA!!! normalnie widok zrobic do tego maila a nie takie skladanie beznaidzjne
            string title = "B2B - Zmiana danych klienta: " + SolexHelper.AktualnyKlient.Nazwa + ".";
            string content = string.Format("DANE DO FAKTURY\r\n\r\n Firma: {0}\r\n NIP: {1}\r\n "
            + "Adres: {2}\r\n Miasto: {3}\r\n Kod pocztowy: {4}\r\n Kraj: {5}\r\n Nr telefonu: {8}\r\n\r\n"
            + " KONTO\r\n\r\n email: {6}\r\nUwagi:\r\n{7}"
            , mojeDaneModel.Firma, mojeDaneModel.NIP, mojeDaneModel.Adres, mojeDaneModel.Miasto, mojeDaneModel.KodPocztowy, mojeDaneModel.Kraj,
            mojeDaneModel.Email, mojeDaneModel.Uwagi, mojeDaneModel.NrTelefonu);

            //do kogo wysylamy email, jezeli puste to do opiekuna
            //jezeli wypelnione to do adresu email podanego w ustawieniach kontroli
            string doKogoWyslacEmail = mojeDaneModel.EmailNaJakiOdeslacOdpowiedz;

            if (string.IsNullOrEmpty(doKogoWyslacEmail) && SolexHelper.AktualnyKlient.Opiekun != null)
            {
                doKogoWyslacEmail = SolexHelper.AktualnyKlient.Opiekun.Email;
            }

            if (!string.IsNullOrEmpty(doKogoWyslacEmail))
            {
                Exception ex;
                var email = new WiadomoscEmail {DoKogo = doKogoWyslacEmail, Tytul = title, TrescWiadomosci = content};
                result = SolexBllCalosc.PobierzInstancje.MaileBLL.WyslijEmaila(email, null, TypyUstawieniaSkrzynek.Ogolne, out ex);
            }
            else
            {
                return PartialView("Tekst", "Brak odbiorcy wiadomości. Skontaktuj się z administratorem w celu poprawnego skonfigurowania kontrolki dane klienta (moje dane). ");
            }

            if (result)
            {
                mojeDaneModel.Status = true;
            }

            return PartialView("_ZmienDane", mojeDaneModel);
        }

        [HttpGet]
        public PartialViewResult ZmianaHasla()
        {
            string guid = Request.QueryString["gid"];
            if (string.IsNullOrEmpty(guid))
            {
                if (SolexHelper.AktualnyKlient.Id == SolexBllCalosc.PobierzInstancje.Klienci.KlientNiezalogowany().Id)
                {
                    throw new Exception("Nie można zmieniać hasła dla klienta niezalogowanego!"); //nie ma obslugi wyjatkow dla Kontrolerow
                }
            }

            var zmianaHaslaModel = new ZmianaHaslaModel { Haslo = "", Haslo2 = "", HasloZmienione = false, GUID = guid };

            //czy jest taki klient
            var k = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Model.Klient>(null, x => x.Gid == guid).FirstOrDefault();
            if (k == null)
            {
                Komunikat komunikat = new Komunikat(
                    string.Format("Nie możesz zmienić hasła - Twój kod przestał działać. Prawdopodobnie hasło już zostało zmienione." +
                                    "<br/><a href='{0}'>Kliknij tutaj aby przejść do logowania</a>", Url.LinkLogowania() ), KomunikatRodzaj.danger);
                return PartialView("Komunikat", new List<Komunikat> { komunikat });
            }

            return PartialView("_ZmianaHasla", zmianaHaslaModel);
        }
        [HttpPost]
        public PartialViewResult ZmianaHasla(ZmianaHaslaModel zmianaHaslaModel)
        {
            string guid = Request.QueryString["gid"];
            ViewBag.Blad = false;
            if (zmianaHaslaModel.Haslo != zmianaHaslaModel.Haslo2)
            {
                ViewBag.Komunikat = "Hasła nie są identyczne";
                return PartialView("_ZmianaHasla", zmianaHaslaModel);
            }
            IKlient klient = SolexHelper.AktualnyKlient;
            if (klient.Id==0)
            {
                var k = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Model.Klient>(null,x => x.Gid == guid).FirstOrDefault();

                if (k == null)
                {
                    Komunikat komunikat = new Komunikat("Nie możesz zmienić hasła - Twój kod przestał działać. Prawdopodobnie już zmieniłeś hasło.", KomunikatRodzaj.danger);
                    return PartialView("Komunikat", new List<Komunikat> { komunikat } );
                }

                klient =new Klient(k);
            }

            zmianaHaslaModel.HasloZmienione = SolexBllCalosc.PobierzInstancje.Klienci.ZmienHaslo(klient, zmianaHaslaModel.Haslo);

            //Jezeli podamy takie samo haslo jak mieliśmy metoda zwroci false
            if (zmianaHaslaModel.HasloZmienione == false)
            {
                ViewBag.Komunikat = "Nowe hasło musi się różnić od poprzedniego hasła";
            }
         
            return PartialView("_ZmianaHasla", zmianaHaslaModel);
        }

        
        [HttpGet]
        public PartialViewResult ResetHasla()
        {           
            var rhModel = new ResetHaslaModel {EmailLubLogin = "", Info = false, Blad = false};
            return PartialView("_ResetHasla", rhModel);
        }

        /// <summary>
        /// Zmiana hasła po loginie lub email
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult ResetHasla(ResetHaslaModel rhModel)
        {
            string data = rhModel.EmailLubLogin;
            Klient c = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(x=>data == x.Email,null);
            if (c == null)
            {
                c = SolexBllCalosc.PobierzInstancje.Klienci.PobierzPologinie(data);
            }
            if (c == null)
            {
                rhModel.Blad = true;
                return PartialView("_ResetHasla", rhModel);
            }

            SolexBllCalosc.PobierzInstancje.Klienci.ResetHasla(c);


            //wylogowanie klienta jesli jest zalogowany
            if (SolexHelper.AktualnyKlient.Id != 0)
            {
                SolexHelper.Wyloguj();
            }


            rhModel.Info = true;
            
            return PartialView("_ResetHasla", rhModel);
        }
    }
}