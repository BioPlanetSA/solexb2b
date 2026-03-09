using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public class Sciezka : KontrolkaTresciBaza
    {
        [Lokalizowane]
        public override string Nazwa
        {
            get { return "Ścieżka"; }
        }

        public override string Kontroler
        {
            get { return "Tresci"; }
        }

        public override string Akcja
        {
            get { return "Sciezka"; }
        }

        public override string Opis
        {
            get { return "Kontrolka pokazująca ścieżkę do aktualnej treści"; }
        }

        public override string Grupa
        {
            get { return "Wygląd"; }
        }

        public Sciezka()
        {
            this.DomyslneWartosciDlaNowejKontrolki = new TrescKolumna();
            DomyslneWartosciDlaNowejKontrolki.DodatkoweKlasyCssKolumny = new []{"belka-plywajaca-ukryj", "text-uppercase"};
        }
    }
}