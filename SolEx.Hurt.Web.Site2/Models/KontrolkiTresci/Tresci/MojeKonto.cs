using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.Tresci
{
    public class MojeKonto : KontrolkaTresciBaza
    {
        public MojeKonto()
        {
            this.DomyslneWartosciDlaNowejKontrolki = new TrescKolumna();
            this.DomyslneWartosciDlaNowejKontrolki.Dostep = AccesLevel.Zalogowani;
            this.DomyslneWartosciDlaNowejKontrolki.DodatkoweKlasyCssKolumny = new[] {"hidden-md-down"};
            this.DomyslneWartosciDlaNowejKontrolki.Szerokosc = 3;

            PoleKlienta = "Email";
        }
        public override string Nazwa
        {
            get { return "Informacja o aktualnie zalogowanej osobie (moje dane)"; }
        }
        public override string Opis
        {
            get { return "Kontrolka pokazująca dane osoby aktualnie zalogowanej i menu moje dane"; }
        }
        public override string Kontroler
        {
            get { return "Tresci"; }
        }

        public override string Akcja
        {
            get { return "MojeDaneMenu"; }
        }
        public override string Grupa
        {
            get { return "Wygląd"; }
        }

        [Niewymagane]
        [PobieranieSlownika(typeof(SlownikTresci))]
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Symbol korzenia treści", FriendlyOpis = "Dodatkowe menu do pokazania po najechaniu/ kliknieciu w dane klienta")]
        [Wymagane]
        public string SymbolKorzen { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [PobieranieSlownika(typeof(SlownikPolKlienta))]
        [FriendlyName("Pole klienta",FriendlyOpis = "Pole z klienta pokazywane jako informacja o aktualnie zalogowanym kliencie.")]
        public string PoleKlienta { get; set; }
    }
}