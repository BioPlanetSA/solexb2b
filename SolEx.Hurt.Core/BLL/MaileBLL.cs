using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL.ObiektyMaili;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Web;
using ServiceStack.Common;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using Attachment = System.Net.Mail.Attachment;

namespace SolEx.Hurt.Core.BLL
{
    public class MaileBLL : BllBazaCalosc, IMaileBLL
    {
        private List<SzablonMailaBaza> _powiadomienia = null;
        public List<SzablonMailaBaza> PobierzListeWszystkichPowiadomienMailowych()
        {
            if (_powiadomienia == null)
            {
                List<Type> typy = Refleksja.PobierzListeKlasDziedziczacychPoKlasieBazowej(typeof(SzablonMailaBaza)).Where(x => !x.IsAbstract).ToList();
                _powiadomienia = new List<SzablonMailaBaza>(typy.Count);

                foreach (var x in typy)
                {
                    try
                    {
                        SzablonMailaBaza obiekt = (SzablonMailaBaza) Activator.CreateInstance(x);
                        if (!string.IsNullOrEmpty(obiekt.OpisFormatu()))
                        {
                            _powiadomienia.Add(obiekt);
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"Błąd tworzenia powiadomienia klasy: {x.Name}", e);
                    }
                }

            }
            return _powiadomienia;
        }

        public void InicjalizacjaPowiadomien()
        {
            List<SzablonMailaBaza> listaPowiadomien = this.PobierzListeWszystkichPowiadomienMailowych();
            IList<UstawieniePowiadomienia> ustawieniaWBazie = Calosc.DostepDane.Pobierz<UstawieniePowiadomienia>(null);

            foreach (UstawieniePowiadomienia temp in ustawieniaWBazie)
            {
                if (!listaPowiadomien.Exists(x => x.Id == temp.Id))
                {
                   Calosc.DostepDane.UsunPojedynczy<UstawieniePowiadomienia>(temp.Id);
                }
            }

            IList < UstawieniePowiadomienia > nowe = new List<UstawieniePowiadomienia>();

            foreach (SzablonMailaBaza powiadomienie in listaPowiadomien)
            {
                bool aktualizowac = false;
                UstawieniePowiadomienia ustawienieZBazy = ustawieniaWBazie.FirstOrDefault(x => x.Id == powiadomienie.Id);
                if (ustawienieZBazy == null)
                {
                    ustawienieZBazy = new UstawieniePowiadomienia { ZgodaNaZmianyPrzezKlienta = powiadomienie.ZgodaNaZmianyPrzezKlienta, Id =  powiadomienie.Id};
                    aktualizowac = true;
                }

                //czyszczenie ustawien parametrow z osob do ktorych nie mozna JUŻ wysylac powiadomienia
                if (ustawienieZBazy.ParametryWysylania != null)
                {
                    List<TypyPowiadomienia> doWywaleniaTypy = ustawienieZBazy.ParametryWysylania.Where(x => !powiadomienie.ObslugiwaneRodzajePowiadomien.Contains(x.DoKogo)).Select(y => y.DoKogo).ToList();
                    if (doWywaleniaTypy.Any())
                    {
                        aktualizowac = true;
                        foreach (var parametryWyslania in doWywaleniaTypy)
                        {
                            ustawienieZBazy.ParametryWysylania.RemoveAll(x => x.DoKogo == parametryWyslania);
                        }
                    }
                }

                foreach (var rodzaj in powiadomienie.ObslugiwaneRodzajePowiadomien)
                {
                    if (ustawienieZBazy.ParametryWysylania == null)
                    {
                        aktualizowac = true;
                        ustawienieZBazy.ParametryWysylania = new List<ParametryWyslania>();
                    }
                    if (ustawienieZBazy.ParametryWysylania.FirstOrDefault(x => x.DoKogo == rodzaj) == null)
                    {
                        aktualizowac = true;
                        bool czyZawiera = powiadomienie.PowiadomieniaDomyslnieAktywne != null && powiadomienie.PowiadomieniaDomyslnieAktywne.Contains(rodzaj);
                        ustawienieZBazy.ParametryWysylania.Add(new ParametryWyslania { Aktywny = czyZawiera, DoKogo = rodzaj });
                    }
                }

                if (aktualizowac)
                {
                    nowe.Add(ustawienieZBazy);
                }
            }

            if (nowe.Any())
            {
                Calosc.DostepDane.AktualizujListe<UstawieniePowiadomienia>(nowe);
            }
        }

        public void WyslijBledneMaile()
        {
            var maile = Calosc.DostepDane.Pobierz<MaileBledneDoPonownejWysylki>(null);
            int liczbaBledow = 0;
            List<int> wyslanepoprawnie = new List<int>();
            foreach (MaileBledneDoPonownejWysylki m in maile)
            {
                WiadomoscEmail mail = new WiadomoscEmail(m.DoKogo, "", m.Bcc);
                mail.TrescWiadomosci = m.Tresc;
                mail.DataStworzenia = DateTime.Now;
                mail.Tytul = m.Tytul;
                try
                {
                    Exception ex;
                    if (!WyslijEmaila(mail, null, m.RodzajSkrzynki, out ex))
                    {
                        throw ex;
                    }
                    wyslanepoprawnie.Add(m.Id);
                }
                catch (Exception)
                {
                    liczbaBledow++;
                    if (liczbaBledow >= Calosc.Konfiguracja.MaksIleBledowPodczasPonownegoWysylaniaMaila)
                    {
                        break;
                    }
                }
            }
            if (wyslanepoprawnie.Any())
            {
                Calosc.DostepDane.Usun<MaileBledneDoPonownejWysylki, int>(wyslanepoprawnie);
            }
        }

        private void DodajEmailZBledem(WiadomoscEmail wiadomoscEmail, TypyUstawieniaSkrzynek zJakiejSkrzynkiWyslac)
        {
            MaileBledneDoPonownejWysylki tmp = new MaileBledneDoPonownejWysylki(wiadomoscEmail, zJakiejSkrzynkiWyslac);
            var ist = Calosc.DostepDane.Pobierz<MaileBledneDoPonownejWysylki>(null, x => x.Equals(tmp)).FirstOrDefault();
            if (ist != null)
            {
                tmp.IloscBledow = ist.IloscBledow;
                tmp.Id = ist.Id;
            }
            tmp.IloscBledow++;
            if (tmp.Id != 0 && tmp.IloscBledow > Calosc.Konfiguracja.MaksimumProbWyslaniaTegoSamegoMaila)
            {
                Calosc.DostepDane.UsunPojedynczy<MaileBledneDoPonownejWysylki>(tmp.Id);
                return;
            }
            Calosc.DostepDane.AktualizujPojedynczy(tmp);
        }

        public MaileBLL(ISolexBllCalosc calosc) : base(calosc)
        {
        }

        private Dictionary<TypyUstawieniaSkrzynek, UstawieniaSkrzynkiPocztowej> _ustawieniaSkrzynki;

        public Dictionary<TypyUstawieniaSkrzynek, UstawieniaSkrzynkiPocztowej> PobierzUstawieniaSkrzynki
        {
            get
            {
                if (_ustawieniaSkrzynki == null)
                {
                    _ustawieniaSkrzynki = new Dictionary<TypyUstawieniaSkrzynek, UstawieniaSkrzynkiPocztowej>(3);

                    var ustawienieOgolne = new UstawieniaSkrzynkiPocztowej
                    {
                        serwer = Calosc.Konfiguracja.EmailHost,
                        uzytkownik = Calosc.Konfiguracja.EmailNazwaUzytkownika,
                        haslo = Calosc.Konfiguracja.EmailHaslo,
                        uzywajSSL = Calosc.Konfiguracja.EmailSzyfrowanie,
                        port = Calosc.Konfiguracja.EmailPort,
                        timeout = Calosc.Konfiguracja.EmailTimeout,
                        NadawcaPrzyjaznaNazwa = Calosc.Konfiguracja.EmailFromPrzyjaznaNazwa,
                        NadawcaEmail = Calosc.Konfiguracja.EmailFrom
                    };

                    if (string.IsNullOrEmpty(ustawienieOgolne.NadawcaPrzyjaznaNazwa))
                    {
                        ustawienieOgolne.NadawcaPrzyjaznaNazwa = ustawienieOgolne.uzytkownik;
                    }

                    if (!(string.IsNullOrEmpty(ustawienieOgolne.serwer) || string.IsNullOrEmpty(ustawienieOgolne.uzytkownik) || string.IsNullOrEmpty(ustawienieOgolne.haslo)))
                    {
                        _ustawieniaSkrzynki.Add(TypyUstawieniaSkrzynek.Ogolne, ustawienieOgolne);
                    }

                    var ustawienieNewsletter = new UstawieniaSkrzynkiPocztowej
                    {
                        serwer = Calosc.Konfiguracja.MailingEmailHost,
                        uzytkownik = Calosc.Konfiguracja.MailingEmailNazwaUzytkownika,
                        haslo = Calosc.Konfiguracja.MailingEmailHaslo,
                        uzywajSSL = Calosc.Konfiguracja.MailingEmailSzyfrowanie,
                        port = Calosc.Konfiguracja.MailingEmailPort,
                        timeout = Calosc.Konfiguracja.MailingEmailTimeout,
                        NadawcaPrzyjaznaNazwa = Calosc.Konfiguracja.MailingEmailFromPrzyjaznaNazwa,
                        NadawcaEmail = Calosc.Konfiguracja.MailingEmailFrom
                    };

                    if (string.IsNullOrEmpty(ustawienieNewsletter.NadawcaPrzyjaznaNazwa))
                    {
                        ustawienieNewsletter.NadawcaPrzyjaznaNazwa = ustawienieNewsletter.uzytkownik;
                    }

                    if (!(string.IsNullOrEmpty(ustawienieNewsletter.serwer) || string.IsNullOrEmpty(ustawienieNewsletter.uzytkownik) || string.IsNullOrEmpty(ustawienieNewsletter.haslo)))
                    {
                        _ustawieniaSkrzynki.Add(TypyUstawieniaSkrzynek.Newsletter, ustawienieNewsletter);
                    }
                }
                return _ustawieniaSkrzynki;
            }
        }

        /// <summary>
        /// zmienia maila tak aby zostal przechwycony - nie wyslany
        /// </summary>
        /// <param name="wiadomoscEmail"></param>
        /// <returns>True jesli przechwycony</returns>
        public bool PrzechwycMaila(ref WiadomoscEmail wiadomoscEmail)
        {
            if (wiadomoscEmail == null)
            {
                Calosc.Log.Error("Przechwyć maila, email jest nullem");
                return false;
            }
            //jesli domena odbiorcy jest w domenie systemu to nie przechwytujemy nigdy
            string aktualnaDomena = HttpContext.Current.Request.Url.Host;
            string domenaZEmaila = string.IsNullOrWhiteSpace(wiadomoscEmail.DoKogo) ? "" : wiadomoscEmail.DoKogo.Substring(wiadomoscEmail.DoKogo.IndexOf('@') + 1);
            if (aktualnaDomena.EndsWith(domenaZEmaila))
            {
                return false;
            }

            //żeby na testówkach nie było akcji że maile są wysyłane do klientów to jest to sprawdzane po domenie - jeśli ma solex albo localhost (czyli testujemy na domenie zewnętrznej albo lokalnie) to przechwytujemy mail
            bool nieWysylacTrybTestowy = Calosc.Konfiguracja.MaileTylkoSolex || HttpContext.Current.Request.Url.AbsoluteUri.Contains("solex") ||
                                         HttpContext.Current.Request.Url.AbsoluteUri.Contains("localhost");
            //czasem jednak trzeba wyslac maile z testowki - dlatetego spradzamycz czy jest plik na dysku wysylajMaile.txt - jesli tak to tryb normalny, nie testowy
            try
            {
                if (File.Exists(HttpRuntime.AppDomainAppPath + @"wysylajMaile.txt"))
                {
                    nieWysylacTrybTestowy = false;
                }
            }
            catch (Exception ex)
            {
                Calosc.Log.Error(ex);
            }

            if (nieWysylacTrybTestowy)
            {
                wiadomoscEmail.Tytul += "Wiadomość przechwycona [pierwotnie do: " + wiadomoscEmail.DoKogo + ", temat: " + wiadomoscEmail.Tytul + " ]";
                wiadomoscEmail.DoKogoPierwotnieJesliPrzechwycony = wiadomoscEmail.DoKogo;
                if (!string.IsNullOrEmpty(wiadomoscEmail.KopiaBCC))
                {
                    wiadomoscEmail.DoKogoPierwotnieJesliPrzechwycony += ", BCC: " + wiadomoscEmail.KopiaBCC;
                }
                
                wiadomoscEmail.DoKogo = "maile-przechwytywanie@solex.net.pl";
                wiadomoscEmail.KopiaBCC = "";
                return true;
            }
            return false;
        }

        private bool SaBledyParsowania(WiadomoscEmail wiadomoscEmail)
        {
            string[] frazy = { "parsowania szablonu", "unable to compile template" };
            foreach (string s in frazy)
            {
                if (wiadomoscEmail.TrescWiadomosci.IndexOf(s, StringComparison.InvariantCultureIgnoreCase)>=0 || wiadomoscEmail.Tytul.IndexOf(s, StringComparison.InvariantCultureIgnoreCase)>0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool WyslijEmaila(WiadomoscEmail wiadomoscEmail, IEnumerable<Attachment> files, TypyUstawieniaSkrzynek zJakiejSkrzynkiWyslac, out Exception blad, bool przechwytujWiadomosciEmail = true, bool logowac = true)
        {
            blad = null;
            bool udaloSieWyslac = false;
            if (!PobierzUstawieniaSkrzynki.ContainsKey(zJakiejSkrzynkiWyslac))
            {
                string komunikat = "Brak konfiguracji skrzynki pocztowej dla danego typu wysyłki: " + zJakiejSkrzynkiWyslac;
                blad = new Exception(komunikat);
                logowac = false;    //nie mozmey logowac jak nie ma skrzynki wysylkowej bo smieci sie pokazuja
            }
            else
            {
                if (SaBledyParsowania(wiadomoscEmail))
                {
                    przechwytujWiadomosciEmail = true;
                }
                if (NiepoprawaDomenaOdbiorcy(wiadomoscEmail))
                {
                    przechwytujWiadomosciEmail = true;
                }
                if (przechwytujWiadomosciEmail)
                {
                    PrzechwycMaila(ref wiadomoscEmail);
                }

                if (string.IsNullOrEmpty(wiadomoscEmail.DoKogo))
                {
                    throw new Exception("Brak zdefiniowanego adresu odbiorcy maila");
                }

                MailMessage msg = new MailMessage {BodyEncoding = Encoding.UTF8, SubjectEncoding = Encoding.UTF8};

                msg.From = new MailAddress(PobierzUstawieniaSkrzynki[zJakiejSkrzynkiWyslac].NadawcaEmail, PobierzUstawieniaSkrzynki[zJakiejSkrzynkiWyslac].NadawcaPrzyjaznaNazwa);
                wiadomoscEmail.OdKogo = PobierzUstawieniaSkrzynki[zJakiejSkrzynkiWyslac].NadawcaEmail;

                if (!string.IsNullOrEmpty(wiadomoscEmail.ReplayTo))
                {
                    msg.ReplyToList.Add(wiadomoscEmail.ReplayTo);
                }

                string[] mailsTo = wiadomoscEmail.DoKogo.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in mailsTo)
                {
                    if (msg.To.All(p => p.Address != s))
                        msg.To.Add(s);
                }

                if (!string.IsNullOrEmpty(wiadomoscEmail.KopiaBCC))
                {
                    string[] bccs = wiadomoscEmail.KopiaBCC.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string s in bccs)
                        if (!string.IsNullOrEmpty(s) && msg.Bcc.All(p => p.Address != s) &&
                            msg.To.All(p => p.Address != s))
                            msg.Bcc.Add(s);
                }
                msg.IsBodyHtml = wiadomoscEmail.WyslijJakoHTML;
                try
                {
                    msg.Body = PrzetworzLinkiWTresciMaila(wiadomoscEmail.WyslijJakoHTML ? wiadomoscEmail.TrescWiadomosci.ZamienZnakKoncaLiniNaWebowy(): wiadomoscEmail.TrescWiadomosci.HtmlToText());
                }
                catch (Exception ex)
                {
                    Calosc.Log.Error(ex);
                    msg.Body = wiadomoscEmail.WyslijJakoHTML ? wiadomoscEmail.TrescWiadomosci.ZamienZnakKoncaLiniNaWebowy() : wiadomoscEmail.TrescWiadomosci.HtmlToText();
                }

                msg.Subject = wiadomoscEmail.Tytul.UsunFormatowanieHTML();

                if (files != null)
                {
                    foreach (Attachment f in files)
                    {
                        if (f == null) continue;

                        msg.Attachments.Add(f);
                    }
                }

                try
                {
                    SmtpClientEx smtp = new SmtpClientEx();
                    smtp.Host = PobierzUstawieniaSkrzynki[zJakiejSkrzynkiWyslac].serwer;
                    smtp.Port = PobierzUstawieniaSkrzynki[zJakiejSkrzynkiWyslac].port;
                    smtp.UseDefaultCredentials = false; // Calosc.Konfiguracja.EmailDefaultCredentials;
                    smtp.EnableSsl = PobierzUstawieniaSkrzynki[zJakiejSkrzynkiWyslac].uzywajSSL;
                    smtp.Credentials = new NetworkCredential(PobierzUstawieniaSkrzynki[zJakiejSkrzynkiWyslac].uzytkownik, PobierzUstawieniaSkrzynki[zJakiejSkrzynkiWyslac].haslo);
                    smtp.Timeout = PobierzUstawieniaSkrzynki[zJakiejSkrzynkiWyslac].timeout;
                    
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    smtp.Send(msg);
                    udaloSieWyslac = true;
                }
                catch (Exception e)
                {
                    udaloSieWyslac = false;
                    if (!przechwytujWiadomosciEmail)
                    {
                        DodajEmailZBledem(wiadomoscEmail, zJakiejSkrzynkiWyslac);
                    }
                    Calosc.Log.Error(e);
                    blad = e;
                }
            }

            wiadomoscEmail.DataStworzenia = DateTime.Now;
            wiadomoscEmail.BylBlad = !udaloSieWyslac;
            if (blad != null)
            {
                wiadomoscEmail.BladKomunikat = blad.Message;
            }
            if (logowac)
            {
                if (!string.IsNullOrEmpty(wiadomoscEmail.KopiaBCC))
                {
                    wiadomoscEmail.DoKogo += string.Format("; {0}", wiadomoscEmail.KopiaBCC);
                }
                SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy(wiadomoscEmail);
            }
            return udaloSieWyslac;
        }

        private bool NiepoprawaDomenaOdbiorcy(WiadomoscEmail wiadomoscEmail)
        {
            string[] domenyTestowe = { "test.com", "test.pl", " example.com" };
            foreach (string s in domenyTestowe)
            {
                if (wiadomoscEmail.DoKogo.ToLower().EndsWith(s))
                {
                    return true;
                }
            }
            return false;
        }

        public string PrzetworzLinkiWTresciMaila(string message)
        {
            try
            {
                string adresPlatformy = HttpContext.Current.Request.Url.Host; //bch komentuje dla BIo nie ma portu bo nie trzeba + (HttpContext.Current.Request.Url.Port != 80 ? ":" + HttpContext.Current.Request.Url.Port.ToString(CultureInfo.InvariantCulture) : "");
                return ZamienAdresyWTekscie(message, adresPlatformy);
            }
            catch (Exception)
            {
                try
                {
                    string adresPlatformy = OperationContext.Current.RequestContext.RequestMessage.Headers.To.Host; //bch kometuje - dal bio nie ma znaczenia port
                                            //+
                                            //(OperationContext.Current.RequestContext.RequestMessage.Headers.To.Port != 80
                                            //    ? ":" + OperationContext.Current.RequestContext.RequestMessage.Headers.To.Port.ToString(CultureInfo.InvariantCulture)
                                            //    : "");
                    return ZamienAdresyWTekscie(message, adresPlatformy);
                }
                catch (Exception ex)
                {
                    Calosc.Log.Error(ex);
                }
            }

            return message;
        }

        private static string ZamienAdresyWTekscie(string message, string adresPlatformy)
        {
            message = message.Replace("\"../Zasoby/", "\"https://" + adresPlatformy + "/Zasoby/", StringComparison.InvariantCultureIgnoreCase);
            message = message.Replace("\"/Zasoby/", "\"https://" + adresPlatformy + "/Zasoby/", StringComparison.InvariantCultureIgnoreCase);
            message = message.Replace("href=\"/", "href=\"https://" + adresPlatformy + "/", StringComparison.InvariantCultureIgnoreCase);
            return message;
        }

        public void UsunStaraHistorieMaili()
        {
            DateTime data = DateTime.Now.AddDays(-3);
            Calosc.DostepDane.UsunWybrane<WiadomoscEmail, long>(x => x.DataStworzenia < data);
        }

        public void UsunCache(IList<long> obj)
        {
            Calosc.Cache.UsunGdzieKluczRozpoczynaSieOd(Calosc.DostepDane.KluczCacheTypDanych<WiadomoscEmail>());
        }

        /// <summary>
        /// Metoda zwraca do kogo jesze nalezy wyslac maila z kampani. Metoda uwzglednia juz wyslane wczesniej maile (z historii maili)
        /// </summary>
        /// <param name="k"></param>
        /// <param name="juzWyslane"></param>
        /// <param name="ListaWszystkichEmailiKampani"></param>
        /// <returns></returns>
        public Dictionary<string, IKlient> PobierzListeKlientowDoWysylki(NewsletterKampania k, out Dictionary<string, IKlient> ListaWszystkichEmailiKampani)
        {
            IList<IKlient> klienciWybranieWKampani = SolexBllCalosc.PobierzInstancje.WidocznosciTypowBll.PobierzKlientowSprelniajacychWarunkiSzablonu(k.Widocznosc);      
            Dictionary<string, IKlient> slownik = new Dictionary<string, IKlient>(100);
            foreach (var klient in klienciWybranieWKampani)
            {
                if (klient.Aktywny && klient.ZgodaNaNewsletter && !string.IsNullOrEmpty(klient.Email) )
                {
                    slownik.Add(klient.Email,klient);
                }
            }
            
            foreach (string adres in k.ListaAdresowKlientowDodatkowe)
            {
                string ad = adres.Trim();
                if (string.IsNullOrEmpty(ad) || slownik.Any(x => x.Key.Equals(ad, StringComparison.InvariantCultureIgnoreCase)))
                {
                    continue;
                }

                //proba wyciagniecia klienta wg. emaila - czy juz taki jest - jak nie to tworzy nowy - sztuczny admin bo nie ma znaczenia kto wysyla newsleter - musi wyjsc skoro juz jest zpalanowany
                IKlient klient  = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>( x=>x.Email == ad, SolexBllCalosc.PobierzInstancje.Klienci.SztucznyAdministrator() );
                if (klient == null)
                {
                    klient =  new Klient(null) { Email = ad, JezykId = Calosc.Konfiguracja.JezykIDDomyslny };
                }
               
                slownik.Add(klient.Email, klient);
            }

            ListaWszystkichEmailiKampani = slownik;

            //odejmowanie tych co już zostali wysłani
            HashSet<string> adresyGdzieMailZostalWyslany = DoKogoPoszlyJuzMaileKampanii(k);
            if (adresyGdzieMailZostalWyslany != null && adresyGdzieMailZostalWyslany.Any())
            {
                return slownik.Where(x => !adresyGdzieMailZostalWyslany.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);
            }
            return slownik;
        }

        /// <summary>
        /// szukanie po emailach a nie po zdarzeniach - jest łatwiej, byc moze jednak mozna w przyszlosci to zmienic i szukac po zdarzenaich - nie wiem
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        private HashSet<string> DoKogoPoszlyJuzMaileKampanii(NewsletterKampania k)
        {
            //todo: moze to z zdarzeń lepiej zrobić?
            var wyniki = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<WiadomoscEmail>(null, x => x.KampaniaId == k.Id && !x.BylBlad);

            var lista1 = wyniki.Select(x => x.DoKogo).ToList();
            lista1.AddRange(wyniki.Select(x => x.DoKogoPierwotnieJesliPrzechwycony).ToList());

            return new HashSet<string>( lista1 );
        }


        public void PrzedAktualizacjaNewslettera(IList<NewsletterKampania> obj)
        {
            foreach (NewsletterKampania n in obj)
            {
                //jesli jest zaplanowana data wysylki to zmienimy status na do wyslania
                if (n.DataWysylki.HasValue && n.Status == StatusNewsletter.Przygotowywany)
                {
                    n.Status = StatusNewsletter.ZaplanowanyDoWysłania;
                }

                if (n.Widocznosc == null || (n.Widocznosc.KategoriaKlientaIdKtorakolwiek == null && n.Widocznosc.KategoriaKlientaIdWszystkie == null)  )
                {
                    //jesli aktualnie status tu wysylanie to zamykamy wysylanie bo nie ma juz komu wysalc - np. usunieta kategoria jest
                    if (n.Status == StatusNewsletter.Przygotowywany)
                    {
                        throw new Exception("Nie można zapisać newslettera jeśli nie ma wybranych kategorii klientów");
                    }
                }
            }
        }

         

    }

    public class SmtpClientEx : SmtpClient
    {
        private static FieldInfo GetLocalHostNameField()
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            System.Reflection.FieldInfo result = typeof(SmtpClient).GetField("macyszyn-toys.pl", flags);
            if (null == result)
                result = typeof(SmtpClient).GetField("localHostName", flags);
            return result;
        }
    }
}