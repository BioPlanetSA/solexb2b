using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.SzczegolyProduktu
{
    public class CenaProduktu : SzczegolyProduktuBaza, INaglowekStopka, IPoleJezyk
    {
        public CenaProduktu()
        {
            NiePokazujJesliBrakCeny = false;
            CenaZawszeWidoczna = false;
        }
        public override string Nazwa
        {
            get { return "Cena"; }
        }

        public override string Opis
        {
            get { return "Wyswietla wybrane rodzaje cen."; }
        }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Typ ceny",FriendlyOpis = "Netto, Brutto, Netto Brutto, Brutto Netto")]
        public JakieCenyPokazywac TypCeny { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Rodzaj ceny")]
        public RodzajCeny RodzajCeny { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Nie pokazuj jesli brak ceny, albo cena = 0 ")]
        public bool NiePokazujJesliBrakCeny { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Poziom cenowy do pokazania")]
        [PobieranieSlownika(typeof(SlownikPoziomuCen))]
        public int? PoziomCenowy { get; set; }

        public override string Akcja
        {
            get { return "Cena"; }
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

        [WidoczneListaAdmin(true, false, true, false)]
        [Niewymagane]
        [FriendlyName("Wyswietlana nazwa ceny dla ceny z poziomu")]
        [Lokalizowane]
        public string NazwaCeny { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czy zawsze pokazywać cenę produktu", FriendlyOpis = "To pole ma najwyższy priorytet w pokazywaniu ceny. Jeśli niezaznaczone, kontrolka będzie wyświetlać ceny wg normalnych zasad.")]
        public bool CenaZawszeWidoczna { get; set; }

        public int JezykId { get; set; }
    }
}