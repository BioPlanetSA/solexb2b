using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.SzczegolyProduktu
{
    public class ZglosBladProduktu : SzczegolyProduktuBaza, INaglowekStopka, IPoleJezyk
    {

        public override string Nazwa
        {
            get { return "Zgłoś Błąd"; }
        }

        public override string Akcja
        {
            get { return "ZglosBledneDane"; }
        }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Adres email do zgłoszeń - pobierany tylko gdy nie ma opiekuna produktu",FriendlyOpis = "Adres na jaki zostanie wysłane zgłoszenie jeśli produkt nie ma opiekuna")]
        [Niewymagane]
        public string MailGdyBrakOpiekuna { get; set; }

        [FriendlyName("Nagłówek sekcji")]
        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Niewymagane]
        [Lokalizowane]
        public string Naglowek { get; set; }

        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Niewymagane]
        [Lokalizowane]
        [FriendlyName("Stopka",FriendlyOpis = "Stopka dla pola")]
        public string Stopka { get; set; }

        public int JezykId { get; set; }
    }

}