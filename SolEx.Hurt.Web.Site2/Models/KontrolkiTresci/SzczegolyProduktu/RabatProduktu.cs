using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.SzczegolyProduktu
{
    public class RabatProduktu : SzczegolyProduktuBaza, IZastepczaNazwaWartosc, INaglowekStopka, IPoleJezyk
    {
        public RabatProduktu()
        {
            PokazNazwe = false;
        }
        public override string Nazwa
        {
            get { return "Rabat produktu"; }
        }
        public override string Akcja
        {
            get { return "Rabat"; }
        }
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Pokazuj nazwy")]
        public bool PokazNazwe { get; set; }

        [FriendlyName("W jakim układzie pokazywać")]
        [WidoczneListaAdmin(true, true, true, true)]
        [PobieranieSlownika(typeof(SlownikWidokowSzczegolowProduktow))]
        public string Uklad { get; set; }

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
        [Niewymagane]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [FriendlyName("Tekst gdy pole nie ma wartości",FriendlyOpis = "Tekst który ma się pokazać jeśli kategoria nie ma wprowadzonej wartości do wybranego pola")]
        [Lokalizowane]
        public string TextZastepczy { get; set; }

        [FriendlyName("Nazwa pola wyświetlana")]
        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Niewymagane]
        [Lokalizowane]
        public string NazwaZastepcza { get; set; }

        public int JezykId { get; set; }
    }

}