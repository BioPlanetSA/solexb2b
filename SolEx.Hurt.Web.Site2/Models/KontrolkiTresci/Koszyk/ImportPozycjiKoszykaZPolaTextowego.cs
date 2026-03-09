using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.Koszyk
{
    public class ImportPozycjiKoszykaZPolaTextowego : KontrolkaTresciBaza, INaglowekStopka, IPoleJezyk
    {
        public ImportPozycjiKoszykaZPolaTextowego()
        {
            this.Naglowek = @"<h6 style='margin - bottom: 20px'><span class='ZnakiWKolku'>2</span>&nbsp;Wklejenie listy produkt&oacute;w</h6>
<p>W poniższe okno możesz wklei listę produktów z Twojego programu ERP, Excela, Open Office lub inych programów z których można kopiować dane.
Produkty rozpoznajemy między innymi według kodów kreskowych, symboli lub ID bazodanowych.Jedyny warunek to taki żeby w jednej lini znajdował się pojedyńczy produkt, oraz ilość jaką dodać do koszyka.</p>
<div class='c'><img alt='przyklad wklejania produktów z excela' src='../static/importCopyPaste.png' /></div>";
        }
        public override string Grupa
        {
            get { return "Koszyk"; }
        }

        public override string Nazwa
        {
            get { return "Importowanie pozycji koszyka za pomocą pola tekstowego"; }
        }

        public override string Kontroler
        {
            get { return "Koszyk"; }
        }

        public override string Akcja
        {
            get { return "ImportPozycjiZPolaTextowewgo"; }
        }

        public override string Opis
        {
            get { return "Kontrolka umożliwia import pozycji koszyka za pomocą pola tekstowego (kopiuj wklej)"; }
        }

        public override string Ikona
        {
            get { return "fa fa-import"; }
        }

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