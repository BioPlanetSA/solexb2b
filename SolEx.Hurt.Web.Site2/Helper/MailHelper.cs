using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL.ObiektyMaili;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Web.Site2.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using ServiceStack.Common;
using SolEx.Hurt.Model.Interfaces;
using System.Text;
using System.Web.Routing;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Helper
{
    public class MailHelper : BllBaza<MailHelper>
    {
        public ISolexBllCalosc Calosc = SolexBllCalosc.PobierzInstancje;

        public WiadomoscEmail GenerujPodgladMaila(UstawieniePowiadomienia szablon, int jezyk, TypyPowiadomienia dokogo)
        {
            SzablonMailaBaza obiekt = Calosc.MaileBLL.PobierzListeWszystkichPowiadomienMailowych().First(x => x.Id == szablon.Id);

            TworzenieFakeObiektow.UstawDaneTestowe(obiekt);
           
            obiekt.DoKogoWysylany = dokogo;
            if (dokogo == TypyPowiadomienia.Klient)
            {
                obiekt.Klient.JezykId = jezyk;
            }
            if (dokogo == TypyPowiadomienia.Opiekun)
            {
                obiekt.Klient.Opiekun.JezykId = jezyk;
            }
            if (dokogo == TypyPowiadomienia.DrugiOpiekun)
            {
                obiekt.Klient.DrugiOpiekun.JezykId = jezyk;
            }
            if (dokogo == TypyPowiadomienia.Przedstawiciel)
            {
                obiekt.Klient.Przedstawiciel.JezykId = jezyk;
            }
            Exception blad = null;
            return ParsujSzablon(obiekt, out blad, false);
        }

        /// <summary>
        /// Zapisuje szablon newsltera na dysk do pliku - żeby mvc moglo parsowac szablon
        /// </summary>
        /// <param name="news"></param>
        private void StworzSzablonNewslettera(SzablonMailaNewslettera news)
        {
            string sciekaFizyczna = PlikiDostep.PobierzInstancje.ZbudujSciezkeFizyczna(news.NazwaSzablonu());
            if (!File.Exists(sciekaFizyczna))
            {
                //katalogi i pliki
                Directory.CreateDirectory(PlikiDostep.PobierzInstancje.ZbudujSciezkeFizyczna(PlikiDostep.PobierzInstancje.KatalogSzablonowNewsletterow) );

                string wynikHTML = "@using SolEx.Hurt.Core.BLL \n" +
                                   "@using SolEx.Hurt.Web.Site2.Helper \n " +
                                   "@model SolEx.Hurt.Core.ModelBLL.ObiektyMaili.SzablonMailaNewslettera \n " +
                                   "<body>" +
                                   news.NewsletterKampania.Tresc +
                                   "</body>";

                using (FileStream fs = File.Create(sciekaFizyczna))
                {
                    UTF8Encoding enc = new UTF8Encoding(true);
                    Byte[] info = enc.GetPreamble().Concat(enc.GetBytes(wynikHTML)).ToArray();
                    fs.Write(info, 0, info.Length);
                }
            }
        }

        public WiadomoscEmail GenerujPodgladNewslettera(NewsletterKampania mailing, IKlient klient)
        {
            SzablonMailaNewslettera mail = new SzablonMailaNewslettera(mailing, klient);
            try
            {
                StworzSzablonNewslettera(mail);
                Exception blad = null;
                WiadomoscEmail wiadomoscEmail = ParsujSzablon(mail, out blad, false); //zakldamy ze style nie ida nasze - sa juz w template zrobione
                if (blad != null)
                {
                    throw blad;
                }
                return wiadomoscEmail;
            }
            finally
            {
                if (mailing.Status == StatusNewsletter.Przygotowywany)
                {
                    if (File.Exists(PlikiDostep.PobierzInstancje.ZbudujSciezkeFizyczna(mail.NazwaSzablonu())))
                    {
                        //kasujemy z dysku plik jesli jest NIE wysylane - stworzy sie pozniej sam
                        File.Delete(PlikiDostep.PobierzInstancje.ZbudujSciezkeFizyczna(mail.NazwaSzablonu()));
                    }
                }
            }
        }

        /// <summary>
        /// NIE URUCHAMIAC tej metody - tylko do uzytko wewnetrzengo, ale publiczna zeby mozna mokowac ja i testowac
        /// </summary>
        /// <param name="danemaila"></param>
        /// <param name="blad"></param>
        /// <param name="konwertujeCsSdoInline"></param>
        /// <returns></returns>
        public virtual WiadomoscEmail ParsujSzablon(SzablonMailaBaza danemaila, out Exception blad, bool konwertujeCsSdoInline = true)
        {
            blad = null;
            try
            {
                danemaila.Konfiguracja = Calosc.Konfiguracja;
                SolexControler controller = (SolexControler)Activator.CreateInstance(typeof(SzablonyMailiController));
                controller = MVCHelper.PobierzInstancje.CreateController(controller);

                string szablon = danemaila.NazwaSzablonu();
                string wynik = controller.PartialViewToString(szablon, danemaila);
                wynik = Calosc.MaileBLL.PrzetworzLinkiWTresciMaila(wynik);

                WiadomoscEmail mail = new WiadomoscEmail();
                string tekstStopki = null;

                if (danemaila is SzablonMailaNewslettera)
                {
                    SzablonMailaNewslettera newsletter = danemaila as SzablonMailaNewslettera;
                    mail.Tytul = newsletter.NewsletterKampania.Temat;

                    //niezalogowany klient nie moze sie wypisac
                    if (danemaila.Klient.Id != 0)
                    {
                        var URL = new UrlHelper(controller.Request.RequestContext);
                        string linkWypisania = URL.WypiszZNewslettera(danemaila.Klient);
                        tekstStopki = Calosc.TresciDostep.PobierzStopkeNewsletterow(danemaila.Klient, danemaila.JezykMaila(), linkWypisania);
                    }
                }
                else
                {
                    mail.Tytul = Helpers.Tools.PobierzInstancje.WytnijFrazeZTresci(ref wynik, FrazaTytul);
                    mail.DoKogo = Helpers.Tools.PobierzInstancje.WytnijFrazeZTresci(ref wynik, FrazaOdbiorca);

                    tekstStopki = Calosc.TresciDostep.PobierzStopkeMaile(danemaila.Klient, danemaila.JezykMaila());
                }

                mail.TrescWiadomosci = this.DoklejStopkeDoTresciHtml(wynik, tekstStopki).Replace("\r\n", "").Trim();

                if (konwertujeCsSdoInline)
                {
                    mail.TrescWiadomosci = PreMailer.Net.PreMailer.MoveCssInline(mail.TrescWiadomosci, false, null, CssMaili, false, true).Html;
                }
                return mail;
            }
            catch (Exception ex)
            {
                Calosc.Log.ErrorFormat("Bład parsowania szablonu mailowego nazwa szablonu {0}, powiadomienie {1}, wysyłane do {2}. Dokładny błąd w kolejnym wpisie", danemaila.NazwaSzablonu(), danemaila.Typ(), danemaila.DoKogoWysylany);
                Calosc.Log.Error(ex);
                blad = ex;
            }
            return null;
        }
        
        private string DoklejStopkeDoTresciHtml(string tresc, string tekstStopki)
        {
            string stopka = string.Format("<div class=\"stopka-solex\" style=\"padding: 20px; text-align:center; color: #919191; font-size: 11px; font-family: Arial, Helvetica, sans-serif;\">{0}</div> ", tekstStopki);
            return tresc.Replace("</body>", stopka + "</body>");
        }

        private readonly string[] FrazaTytul = { "<tytul>", "</tytul>" };
        private readonly string[] FrazaOdbiorca = { "<odbiorca>", "</odbiorca>" };

        private static string _cssMaili = "";

        /// <summary>
        /// ładowanie stylów css maili (także własnych klienta)
        /// </summary>
        private string CssMaili
        {
            get
            {
                if (string.IsNullOrEmpty(_cssMaili))
                {
                    if (!string.IsNullOrEmpty(Calosc.Konfiguracja.SzablonNiestandardowyNazwa))
                    {
                        string sciezkaDoPlikuCss = Calosc.Konfiguracja.SzablonNiestandardowySciezkaBezwzgledna + "/maile.css";
                        if (File.Exists(sciezkaDoPlikuCss))
                        {
                            _cssMaili = File.ReadAllText(sciezkaDoPlikuCss);
                        }
                    }

                    if (string.IsNullOrEmpty(_cssMaili))
                    {
                        _cssMaili = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Layout\css\maile\maile.css");
                    }

                    //doczytania INKA na góre
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\Layout\css\maile\ink.css"))
                    {
                        _cssMaili = _cssMaili.Insert(0, File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Layout\css\maile\ink.css"));
                    }
                }
                return _cssMaili;
            }
        }

        private bool CzyJestDoKogoWyslac(SzablonMailaBaza danemaila)
        {
            switch (danemaila.DoKogoWysylany)
            {
                case TypyPowiadomienia.Opiekun:
                    if (danemaila.Klient.Opiekun != null)
                    {
                        return true;
                    }
                    break;

                case TypyPowiadomienia.Przedstawiciel:
                    if (danemaila.Klient.Przedstawiciel != null)
                    {
                        return true;
                    }
                    break;

                case TypyPowiadomienia.DrugiOpiekun:
                    if (danemaila.Klient.DrugiOpiekun != null)
                    {
                        return true;
                    }
                    break;

                case TypyPowiadomienia.Klient:
                    if (danemaila.Klient == null)
                    {
                        return false;
                    }
                    return true;
            }
            return false;
        }
        //klucz typ maila, Wartosc KeyValuePair gdzie klucz to wartosc domyslna a wartość to slownik gdzie klucz to id klienta wartosc to wartosc ustawienia profilu klienta
        private Dictionary<string, KeyValuePair<bool, Dictionary<long, bool>>> _ustawienia;
        public bool CzyZezwolilNaWysylanie(TypUstawieniaKlienta typ, string typMaila, long idKlienta)
        {
            KeyValuePair<bool, Dictionary<long, bool>> ustaw;
            if (_ustawienia == null)
            {
                _ustawienia = new Dictionary<string, KeyValuePair<bool, Dictionary<long, bool>>>();
            }

            if (!_ustawienia.TryGetValue(typMaila, out ustaw))
            {
                bool wartoscDomyslna;
                Dictionary<long, bool> idKlientow = Calosc.ProfilKlienta.PobierzKlientowZWartosciaUstawienia<bool>(typ, typMaila, AccesLevel.Zalogowani);
                //Sprawdzamy czy jest wartosc domyslna jezeli jest to pobieramy i usuwamy ja ze slownika
                if (!idKlientow.TryGetValue(0, out wartoscDomyslna))
                {
                    wartoscDomyslna = true;
                }
                else
                {
                    idKlientow.Remove(0);
                }
                ustaw = new KeyValuePair<bool, Dictionary<long, bool>>(wartoscDomyslna, idKlientow);
                _ustawienia.Add(typMaila, ustaw);
            }
            //Sprawdzamy czy sa jakies profile w bazie dla danego ustawienia
            if (ustaw.Value.Any())
            {
                bool wartosc;
                if (ustaw.Value.TryGetValue(idKlienta, out wartosc))
                {
                    return wartosc;
                }
            }
            return ustaw.Key;





        }




        /// <summary>
        /// Główna metoda do wysyłania maili
        /// </summary>3
        /// <param name="danemaila"></param>
        public void WyslijMaile(SzablonMailaBaza danemaila)
        {
            if (danemaila.GetType() == typeof(SzablonMailaNewslettera) )
            {
                throw new Exception("Nie można wysyłać tą metodą newslettery!");
            }

            UstawieniePowiadomienia parametry = Calosc.DostepDane.PobierzPojedynczy<UstawieniePowiadomienia>(danemaila.Id);

            if (danemaila is Formularz)
            {
                //zarzadzanie do kogos wysylac maila odbywa sie w samym formularzu (kontrolce formularza)
                parametry = new UstawieniePowiadomienia();
                parametry.ParametryWysylania = (danemaila as Formularz).ParametryWysylania;
                parametry.Id = danemaila.Id;
            }

            if (parametry == null)
            {
                Calosc.Log.Error("Brak konfiguracji ustawien dla powiadomienia: " + danemaila.NazwaSzablonu() + ". Muszą to sprawdzić programiści (błędne ID powiadomienia?)");
                return;
            }

            foreach (var p in parametry.ParametryWysylania)
            {
                if (!p.Aktywny && string.IsNullOrEmpty(p.EmailBcc))
                {
                    //mail jest nieaktywny i nie ma nic w BCC - omijamy wysylke
                    continue;
                }
                bool czyKlienZezwolil = CzyZezwolilNaWysylanie(TypUstawieniaKlienta.PowiadomieniaMailowe, danemaila.GetType().Name, danemaila.Klient.Id);
                //sprawdzamy czy kient moze zmieniać czy chce dostawać powiadomienie
                if (p.Aktywny && p.DoKogo==TypyPowiadomienia.Klient  && parametry.ZgodaNaZmianyPrzezKlienta)
                {
                    //czyKlienZezwolil = Calosc.ProfilKlienta.PobierzWartosc<bool>(danemaila.Klient, TypUstawieniaKlienta.PowiadomieniaMailowe, danemaila.GetType().Name);
                    if (!czyKlienZezwolil && string.IsNullOrEmpty(p.EmailBcc))
                    {
                        Log.InfoFormat("Pomijam wysłanie powiadomienia mailowego: {0} do: {1} dlatego że nie klient nie chce tych powiadomień. ", danemaila.NazwaSzablonu(), p.DoKogo);
                        continue;
                    }
                }

                danemaila.DoKogoWysylany = p.DoKogo;
                if (!this.CzyJestDoKogoWyslac(danemaila))
                {
                    Log.DebugFormat("Pomijam wysłanie powiadomienia mailowego: {0} do: {1} dlatego że nie można ustalić komu wysłać maila (klient nie ma maila?)", danemaila.NazwaSzablonu(), p.DoKogo);
                    continue;
                }

                Exception bladParsowania;
                var mail = ParsujSzablon(danemaila, out bladParsowania);
                if (mail == null)
                {
                    //jak jest pusta zawartosc to znaczy ze nie wysylamy do tego delikwenta widocznie
                    continue;
                }

                //dopiero po parsowaniu moze byc w parsowaniu zmieniane sa adresy odbiorcow
                mail.KopiaBCC = p.EmailBcc;

                if (!p.Aktywny || (parametry.ZgodaNaZmianyPrzezKlienta && !czyKlienZezwolil))
                {
                    mail.DoKogo = mail.KopiaBCC;
                    mail.KopiaBCC = null;
                }

                if (string.IsNullOrEmpty(mail.DoKogo))
                {
                    Log.DebugFormat("Pomijam powtórnie wysłanie powiadomienia mailowego: {0} do: {1} dlatego że nie można ustalić komu wysłać maila (klient nie ma maila?)", danemaila.NazwaSzablonu(), p.DoKogo );
                    continue;
                }

                TypyUstawieniaSkrzynek typ = TypyUstawieniaSkrzynek.Ogolne;

                //bioplanet chca na sztywno w mailu o nowym zamowieniu dodawac BCC opiekuna i przedstawiciela
                if (danemaila is NoweZamowienie)
                {
                    if (danemaila.Klient.Opiekun != null && danemaila.Klient.OpiekunId.HasValue && !string.IsNullOrEmpty(danemaila.Klient.Opiekun.Email))
                    {
                        mail.KopiaBCC += $";{danemaila.Klient.Opiekun.Email}";
                    }

                    if (danemaila.Klient.Przedstawiciel != null && danemaila.Klient.PrzedstawicielId.HasValue && !string.IsNullOrEmpty(danemaila.Klient.Przedstawiciel.Email))
                    {
                        mail.KopiaBCC += $";{danemaila.Klient.Przedstawiciel.Email}";
                    }
                }

                Exception ex;
                bool udaloSieWyslac = Calosc.MaileBLL.WyslijEmaila(mail, danemaila.Zalaczniki, typ, out ex);
                if (!udaloSieWyslac)
                {
                    //Paweł - zakomentowałem wywalanie wyjątki gdyż bez sensu jest wywalac aplikację bo nie udało się wysłać maila. Problem był w subkontach gdy chcieliśmy dodać konto a nie było skonfigurowanej skrzynki mailowej. Jak dla mnie wystarczające w zupełności jest wrzucenie informacji do logów
                    string blad = $"Nieudana próba wysłania maila {danemaila.NazwaSzablonu()}. Błąd: {ex.Message}.";
                    Calosc.Log.Error(blad);
                    // throw new Exception(blad);
                }
            }
        }

        /// <summary>
        /// glowna metoda do wysylania kampani newsletterów
        /// </summary>
        /// <param name="idNewsletteraDoWysylki"></param>
        public void WyslijAktywneMailingi(int? idNewsletteraDoWysylki = null)
        {
            Model.NewsletterKampania kampania = null;
            if (idNewsletteraDoWysylki == null)
            {
                kampania = Calosc.DostepDane.Pobierz<Model.NewsletterKampania>(null, x => x.Status == StatusNewsletter.ZaplanowanyDoWysłania || x.Status == StatusNewsletter.Wysyłany).FirstOrDefault();
            }
            else
            {
                kampania = Calosc.DostepDane.PobierzPojedynczy<Model.NewsletterKampania>(idNewsletteraDoWysylki.Value);
                if (kampania.Status != StatusNewsletter.ZaplanowanyDoWysłania && kampania.Status != StatusNewsletter.Wysyłany)
                {
                    //nie mozemy wysylac czegos w innych statusach
                    return;
                }
            }

            if (kampania != null)
            {
                WyslijMaileZKampanii(kampania);
            }
        }

        //newsleter wysylanie
        protected void WyslijMaileZKampanii(NewsletterKampania kampania)
        {
            if (!(kampania.Status == StatusNewsletter.ZaplanowanyDoWysłania || kampania.Status == StatusNewsletter.Wysyłany))
            {
                return;
            }
            
            Dictionary<string, IKlient> wszyscyOdbiorcy = null;
            Dictionary<string, IKlient> komuWyslac =  Calosc.MaileBLL.PobierzListeKlientowDoWysylki(kampania, out wszyscyOdbiorcy);

            if (komuWyslac == null || komuWyslac.IsEmpty())
            {
                //konczenie newsltera
                kampania.Status = StatusNewsletter.Zakończony;
                Calosc.DostepDane.AktualizujPojedynczy(kampania);
                return;
            }

            if (kampania.Status != StatusNewsletter.Wysyłany)
            {
                kampania.Status = StatusNewsletter.Wysyłany;
                Calosc.DostepDane.AktualizujPojedynczy(kampania);
            }

            int ilewyslane = 0;
            int iloscbledow = 0;
            int ileWPaczce = Calosc.Konfiguracja.MailingIleNaRaz;
            int maxbledow = Calosc.Konfiguracja.MailPrzerwaPoIluBledach;

            //koniecznie przed wysylka trzeba zbudowac szablon jesli go nie ma
            SzablonMailaNewslettera sztucznyMail = new SzablonMailaNewslettera(kampania, komuWyslac.First().Value  );
            StworzSzablonNewslettera(sztucznyMail);

            List<IKlient> listaKlientowDoKtorychWyslanyMailing = new List<IKlient>();
            try
            {
                foreach (KeyValuePair<string, IKlient> odbiorca in komuWyslac)
                {
                    try
                    {
                        Exception bladWysylki = null;
                        SzablonMailaNewslettera obiekt = new SzablonMailaNewslettera(kampania, odbiorca.Value);
                        WiadomoscEmail mailDoWysylki = ParsujSzablon(obiekt, out bladWysylki);

                        mailDoWysylki.KampaniaId = (int)kampania.Id;
                        mailDoWysylki.DoKogo = odbiorca.Value.Email;

                        if (bladWysylki != null)
                        {
                            Calosc.Log.ErrorFormat("Problem z parsowaniem newslettera dla klienta o mailu: {0}. Błąd: {1}", odbiorca.Key, bladWysylki.Message);
                            throw bladWysylki;
                        }

                        if (!string.IsNullOrEmpty(obiekt.NewsletterKampania.OdpowiedzNaAdres))
                        {
                            mailDoWysylki.ReplayTo = obiekt.NewsletterKampania.OdpowiedzNaAdres;
                        }

                        bool udaloSieWyslac = Calosc.MaileBLL.WyslijEmaila(mailDoWysylki, null, TypyUstawieniaSkrzynek.Newsletter, out bladWysylki);
                        if (!udaloSieWyslac)
                        {
                            throw bladWysylki;
                        }

                        //dodajmey klienta jesli poszlo OK
                        listaKlientowDoKtorychWyslanyMailing.Add(odbiorca.Value);
                    }
                    catch (Exception ex)
                    {
                        Calosc.Log.ErrorFormat("Problem z wysłaniem newslettera dla klienta o mailu: {0}. Błąd: {1}", odbiorca.Key, ex.Message);
                        iloscbledow++;
                    }

                    if (iloscbledow >= maxbledow)
                    {
                        Calosc.Log.ErrorFormat("Kończymy wysyłanie maili z powodu dużej ilości błędów (ponad " + iloscbledow + ")");
                        break;
                    }
                    if (ilewyslane >= ileWPaczce)
                    {
                        break;
                    }
                }
            }
            finally
            {
                //tylko wpisujemy zdarzenie jak cos poszlo
                if (listaKlientowDoKtorychWyslanyMailing.Any())
                {
                    List<string> doKogoPoszloEmaile = listaKlientowDoKtorychWyslanyMailing.Select(x => x.Email).ToList();
                    Calosc.Statystyki.ZdarzenieWysylanieNewslettera(kampania, null, doKogoPoszloEmaile);
                    Calosc.DostepDane.AktualizujPojedynczy(kampania);
                }
            }
        }


    }
}