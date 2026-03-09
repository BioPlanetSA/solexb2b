using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public class Kontakt : KontrolkaTresciBaza
    {
        public Kontakt()
        {
            PokazTytul = true;
            PokazujAdresEmailNaJakiOdeslacOdpowiedz = false;
            Tytul = "Temat wiadomości";
            Tresc = "Treść wiadomości";
            MailDoWysylki = "testb2b@solex.net.pl";
            DoOpiekuna = false;
            DoPrzedstawiciela = false;
            DoDrugiegoOpiekuna = false;
            Pola = "Przykładowy plik|Plik;Telefon|String";
        }

        public override string Nazwa
        {
            get { return "Formularz kontaktowy"; }
        }

        public override string Kontroler
        {
            get { return "Klienci"; }
        }

        public override string Akcja
        {
            get { return "FormularzZapytania"; }
        }

        public override string Opis
        {
            get { return "Kontrolka z formularzem kontaktowym"; }
        }

        [FriendlyName("Pokaż tytuł")]
        [WidoczneListaAdmin(true, true,true,true)]
        public bool PokazTytul { get; set; }

        [FriendlyName("Pokazywać email na jaki odeslać odpowiedź")]
        [WidoczneListaAdmin(true, true, true, true)]
        public bool PokazujAdresEmailNaJakiOdeslacOdpowiedz { get; set; }

        [FriendlyName("Tytuł")]
        [WidoczneListaAdmin(true, true, true, true)]
        [Lokalizowane]
        public string Tytul { get; set; }

        [FriendlyName("Treść")]
        [WidoczneListaAdmin(true, true, true, true)]
        [Lokalizowane]
        public string Tresc { get; set; }

        [FriendlyName("Mail na jaki wysłać zapytanie")]
        [WidoczneListaAdmin(true, true, true, true)]
        public string MailDoWysylki { get; set; }

        [FriendlyName("Do opiekuna")]
        [WidoczneListaAdmin(true, true, true, true)]
        public bool DoOpiekuna { get; set; }

        [FriendlyName("Do przedstawiciela")]
        [WidoczneListaAdmin(true, true, true, true)]
        public bool DoPrzedstawiciela { get; set; }

        [FriendlyName("Do drugiego opiekuna")]
        [WidoczneListaAdmin(true, true, true, true)]
        public bool DoDrugiegoOpiekuna { get; set; }

        [FriendlyName("Opcjonalne pola - w postaci np. 'Przykładowy plik|Plik;Telefon|String' ")]
        [WidoczneListaAdmin(true, true, true, true)]
        public string Pola { get; set; }
    }
}