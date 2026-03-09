using SolEx.Hurt.Model;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Routing;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Web.Site2.Helper.DaneListaAdmin.DodatkoweFunkcjeModuly
{
    public class DodatkoweFunkcjeNewsletter : DodatkoweFunkcjeBaza
    {
        public override Type OblugiwanyTyp()
        {
            return typeof(NewsletterKampania);
        }

        public override IList<DodatkowaFunkcja> PobierzDodatkoweFunkcjeDlaObiektu(object o)
        {
            NewsletterKampania obiekt = (NewsletterKampania)o;
            List<DodatkowaFunkcja> wynik = new List<DodatkowaFunkcja>();
            wynik.Add(new DodatkowaFunkcja
            {
                Adres = Url.Action("NewsletterPodglad", "Newsletter", new { id = obiekt.Id}),
                Nazwa = "Podgląd"
            });

            if (obiekt.Status == StatusNewsletter.ZaplanowanyDoWysłania)
            {
                wynik.Add(new DodatkowaFunkcja {
                    Adres = Url.Action("WyslijNewsletter", "Newsletter", new { id = obiekt.Id }),
                    Nazwa = "Wyślij maile teraz",
                    KomunikatJavascriptCzyNapewno = new Komunikat("Czy na pewno wysłać maile <b>w tej chwili</b>? " +
                    " Nie musisz ręcznie wysyłać wiadomości - zostaną automatycznie rozesłane w nocy.", KomunikatRodzaj.danger),
                    KlasaCssPrzycisku = "btn-warning btn-sm btn"
                    }
                );
            }

            if (obiekt.Status == StatusNewsletter.Przygotowywany)
            {
                wynik.Add(new DodatkowaFunkcja {
                    Adres = Url.Action("WyslijNewsletter", "Newsletter", new { id = obiekt.Id }),
                    Nazwa = "Przekaż do wysłania",
                    KomunikatJavascriptCzyNapewno= new Komunikat("Czy na pewno zacząć wysyłanie tego newslettera? Dalsze zmiany tego newslttera nie będą możliwe!", KomunikatRodzaj.danger),
                    KlasaCssPrzycisku = "btn-danger btn-sm btn"
                });
            }

            if (obiekt.Status == StatusNewsletter.Wysyłany)
            {
                wynik.Add(new DodatkowaFunkcja
                {
                    Adres = Url.Action("ZakonczWysylanie", "Newsletter", new { id = obiekt.Id }),
                   // Adres = "/newsletterKampania/ZakonczWysylanie/" + obiekt.Id,
                    Nazwa = "Zakończ wysyłanie",
                    KomunikatJavascriptCzyNapewno = new Komunikat("Czy na pewno zakończyć wysyłanie maili? Powtórne włącznie nie będzie możliwe!", KomunikatRodzaj.danger),
                    KlasaCssPrzycisku = "btn-danger btn-sm btn"
                });
            }

            if (obiekt.Status == StatusNewsletter.Zakończony)
            {
                wynik.Add(new DodatkowaFunkcja
                {
                    Adres = Url.Action("Usuwanie", "Admin", new { id = obiekt.Id, typ = this.OblugiwanyTyp().PobierzOpisTypu() }),
                    Nazwa = "Usuń mailing",
                    KomunikatJavascriptCzyNapewno = new Komunikat("Czy na pewno usunąć kampanie? Wszystkie dane mailingu i statystyki zostaną usunięte!", KomunikatRodzaj.danger),
                    KlasaCssPrzycisku = "btn-danger btn-sm btn"
                });
            }

            return wynik;
        }

        public override string PobierzNazweObiektu(object o)
        {
            NewsletterKampania obiekt = (NewsletterKampania)o;
            if (!string.IsNullOrEmpty(obiekt.Temat)) return obiekt.Temat;
            FriendlyNameAttribute opisy = o.GetType().GetCustomAttribute<FriendlyNameAttribute>();
            return (opisy != null) ? opisy.FriendlyName : o.GetType().Name;
        }

        public override List<Komunikat> KomunitatyNaEdycjiObiektu(object o)
        {
            NewsletterKampania obiekt = (NewsletterKampania)o;
            List<Komunikat> wynik = new List<Komunikat>();
            if (obiekt.Status == StatusNewsletter.Wysyłany)
            {
                wynik.Add(new Komunikat("Nie można edytować newsletera który jest w trakcie wysyłania", KomunikatRodzaj.warning));
            }
            if (obiekt.Status == StatusNewsletter.Zakończony)
            {
                wynik.Add(new Komunikat("Nie można edytować newsletera który został już wysłany", KomunikatRodzaj.warning));
            }

            return null;
        }
        public override List<Komunikat> KomunitatyNaLiscieObiektu(Type o)
        {
            List<Komunikat> wynik = new List<Komunikat>();
            if (!SolexBllCalosc.PobierzInstancje.MaileBLL.PobierzUstawieniaSkrzynki.ContainsKey(TypyUstawieniaSkrzynek.Newsletter))
            {
                string komunikat = "Brak konfiguracji skrzynki pocztowej";
                wynik.Add(new Komunikat(komunikat,KomunikatRodzaj.danger));
            }
            return wynik;
        }
        public override bool? MoznaEdytowacObiekt(object o)
        {
            NewsletterKampania news = o as NewsletterKampania;

            if (news.Status == StatusNewsletter.Przygotowywany)
            {
                return true;
            }

            return false;
        }

        public override bool? MoznaUsuwacObiekt(object o)
        {
            NewsletterKampania news = o as NewsletterKampania;

            if (news.Status == StatusNewsletter.Przygotowywany)
            {
                return true;
            }

            return false;
        }
    }
}