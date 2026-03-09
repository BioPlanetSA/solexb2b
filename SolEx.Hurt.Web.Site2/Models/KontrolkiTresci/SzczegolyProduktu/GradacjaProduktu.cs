using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.SzczegolyProduktu
{
    public class GradacjaProduktu : SzczegolyProduktuBaza, INaglowekStopka, IPoleJezyk
    {
        public GradacjaProduktu()
        {
            this.PokazywacIleJuzKupione = true;
            this.TypCenyPoRabacie = JakieCenyPokazywac.Netto;
        }
        public override string Nazwa
        {
            get { return "Gradacja"; }
        }

        public override string Akcja
        {
            get { return "Gradacje"; }
        }

        public override string Kontroler
        {
            get { return "Produkty"; }
        }

        [FriendlyName("Nagłówek sekcji")]
        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Niewymagane]
        [Lokalizowane]
        public string Naglowek { get; set; }

        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Niewymagane]
        [FriendlyName("Stopka",FriendlyOpis = "Stopka dla pola")]
        [Lokalizowane]
        public string Stopka { get; set; }
        
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Sposób pokazywania")]
        public GradacjaSposobPokazywania SposobPokazywania { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Typ ceny")]
        public JakieCenyPokazywac TypCenyPoRabacie { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy pokazywać ile już klient kupił produktu")]
        public bool PokazywacIleJuzKupione { get; set; }

        public int JezykId { get; set; }
    }
}