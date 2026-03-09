using ServiceStack.DataAnnotations;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public class Wyszukiwarka : KontrolkaTresciBaza, IPoleJezyk
    {
        public override string Nazwa
        {
            get { return "Wyszukiwarka"; }
        }

        public override string Kontroler
        {
            get { return "Produkty"; }
        }

        public override string Akcja
        {
            get { return "Szukanie"; }
        }

        public Wyszukiwarka()
        {
            Placeholder = "Szukaj produktów";
            this.DomyslneWartosciDlaNowejKontrolki = new TrescKolumna();
            DomyslneWartosciDlaNowejKontrolki.DodatkoweKlasyCssKolumny = new[] { "col-lg-3", "col-lg-last-down", "text-xs-right" };
        }

        public override string Opis
        {
            get { return "Wyszukiwarka produktów"; }
        }

        public override string Grupa
        {
            get { return "Wygląd"; }
        }

        public override string Ikona
        {
            get { return "fa fa-search"; }
        }

        [FriendlyName("Znak wodny")]
        [Lokalizowane]
        [WidoczneListaAdmin(true,true,true,true)]
        public string Placeholder { get; set; }

        [Ignore]
        public int JezykId { get; set; }
    }
}