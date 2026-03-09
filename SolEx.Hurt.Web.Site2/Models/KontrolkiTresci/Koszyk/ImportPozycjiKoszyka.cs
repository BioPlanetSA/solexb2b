using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.Koszyk
{
    public class ImportPozycjiKoszyka : KontrolkaTresciBaza, INaglowekStopka, IPoleJezyk
    {

        public ImportPozycjiKoszyka()
        {
            this.Naglowek = "<h6><span class='ZnakiWKolku'>1</span> Import produktów z pliku</h6>";
        }
        public override string Grupa
        {
            get { return "Koszyk"; }
        }

        public override string Nazwa
        {
            get { return "Importowanie pozycji koszyka"; }
        }

        public override string Kontroler
        {
            get { return "Koszyk"; }
        }

        public override string Akcja
        {
            get { return "ImportowaniePozycjiKoszyka"; }
        }

        public override string Opis
        {
            get { return "Kontrolka umożlwia import pozycji koszyka z pliku"; }
        }

        public override string Ikona
        {
            get { return "fa fa-import"; }
        }

        [FriendlyName("Dostępne formaty importów")]
        [WidoczneListaAdmin]
        [PobieranieSlownika(typeof(SlownikSposobowImportuKoszyka))]
        public string[] DostepneImporty { get; set; }

        [FriendlyName("Nagłówek dla kontrolki")]
        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Niewymagane]
        [Lokalizowane]
        public string Naglowek { get; set; }

        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Niewymagane]
        [Lokalizowane]
        [FriendlyName("Stopka dla kontrolki")]
        public string Stopka { get; set; }

        public int JezykId { get; set; }
    }
}