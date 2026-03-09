using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.SzczegolyProduktu
{
    public class WyswietlWymiaryProduktu : SzczegolyProduktuBaza, INaglowekStopka, IPoleJezyk
    {
        public WyswietlWymiaryProduktu()
        {
            PokazujJednostkePodstawowa = true;
            ZaokraglenieWagi = 0;
            ZaokraglenieWymirow = 0;
        }
        public override string Nazwa
        {
            get { return "Wymiary produktu"; }
        }

        public override string Opis
        {
            get { return "Wyswietla wymiary produktu."; }
        }

        public override string Akcja
        {
            get { return "WyswietlWymiaryProduktu"; }
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
        [FriendlyName("Stopka")]
        [Lokalizowane]
        public string Stopka { get; set; }

        [WidoczneListaAdmin(true, false, true, false)]
        [Niewymagane]
        [FriendlyName("Jednosta Gabarytow")]
        [GrupaAtttribute("Jednostki", 3)]
        [Lokalizowane]
        public string JednostkaGabarytow { get; set; }

        [WidoczneListaAdmin(true, false, true, false)]
        [Niewymagane]
        [FriendlyName("Jednosta objetosci")]
        [GrupaAtttribute("Jednostki", 3)]
        [Lokalizowane]
        public string JednostkaObjetosci { get; set; }

        [WidoczneListaAdmin(true, false, true, false)]
        [Niewymagane]
        [FriendlyName("Jednosta wagi")]
        [GrupaAtttribute("Jednostki", 3)]
        [Lokalizowane]
        public string JednostkaWagi { get; set; }

        [WidoczneListaAdmin(true, false, true, false)]
        [Niewymagane]
        [WymuszonyTypEdytora(TypEdytora.PoleZdjecie)]
        [FriendlyName("Obrazek pudełka")]
        [GrupaAtttribute("Obrazy", 2)]
        public int? ObrazekPudelka { get; set; }

        [WidoczneListaAdmin(true, false, true, false)]
        [Niewymagane]
        [WymuszonyTypEdytora(TypEdytora.PoleZdjecie)]
        [FriendlyName("Obrazek opakowania zbiorczego")]
        [GrupaAtttribute("Obrazy", 2)]
        public int? ObrazekOpZbiorczego { get; set; }

        [WidoczneListaAdmin(true, false, true, false)]
        [Niewymagane]
        [WymuszonyTypEdytora(TypEdytora.PoleZdjecie)]
        [FriendlyName("Obrazek palety")]
        [GrupaAtttribute("Obrazy", 2)]
        public int? ObrazekPalety { get; set; }

        [WidoczneListaAdmin(true, false, true, false)]
        [Niewymagane]
        [FriendlyName("Pokazuj jednostkę podstawową przy ilości")]
        [GrupaAtttribute("Jednostki", 3)]
        public bool PokazujJednostkePodstawowa { get; set; }

        //[WidoczneListaAdmin(true, false, true, false)]
        //[Niewymagane]
        //[FriendlyName("Czy przeliczać wagę jesli nie są uzupełnione")]
        //public bool PrzeliczajWage { get; set; }

        //[WidoczneListaAdmin(true, false, true, false)]
        //[Niewymagane]
        //[FriendlyName("Czy przeliczać objętość jesli nie jest podana na produkcie")]
        //public bool PrzeliczajObjetosc { get; set; }

        [WidoczneListaAdmin(true, false, true, false)]
        [Niewymagane]
        [FriendlyName("Do ilu miejsc po przecinku zaokraglac wymiary")]
        public int ZaokraglenieWymirow { get; set; }

        [WidoczneListaAdmin(true, false, true, false)]
        [Niewymagane]
        [FriendlyName("Do ilu miejsc po przecinku zaokraglac wage i objetosc")]
        public int ZaokraglenieWagi { get; set; }

        public int JezykId { get; set; }
    }
}