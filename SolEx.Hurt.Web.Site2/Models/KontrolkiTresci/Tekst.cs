using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public class Tekst : KontrolkaTresciBaza, IPoleJezyk
    {
        public Tekst()
        {
            this.DomyslneWartosciDlaNowejKontrolki = new TrescKolumna();
            this.DomyslneWartosciDlaNowejKontrolki.DodatkoweKlasyCssKolumny = new[] {"czcionka-tekstowa-paragraf"};
        }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Treść")]
        [Niewymagane]
        [Lokalizowane]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        public string Tresc { get; set; }

        public override string Nazwa
        {
            get { return "Tekst"; }
        }

        public override string Kontroler
        {
            get { return "Tresci"; }
        }

        public override string Akcja
        {
            get { return "Tekst"; }
        }

        public override string Grupa
        {
            get { return "Wygląd"; }
        }

        public int JezykId { get; set; }
    }
}