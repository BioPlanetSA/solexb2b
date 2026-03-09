using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.SzczegolyProduktu
{
    public class GaleriaZdjecProduktu_Miniaturki : SzczegolyProduktuBaza, INaglowekStopka, IZastepczaNazwaWartosc, IPoleJezyk
    {
      public override string Nazwa
        {
            get { return "Galeria Zdjęć - miniaturki"; }
        }

        public override string Akcja
        {
            get { return "GaleriaZdjecMiniaturki"; }
        }

        public GaleriaZdjecProduktu_Miniaturki()
        {
            DuzyPodglad = true;
            RozmiarMiniatur = "ico85x90p";
            RozmiarZdjecia = "ico570x630p";

            DomyslneWartosciDlaNowejKontrolki = new TrescKolumna();
            DomyslneWartosciDlaNowejKontrolki.Marginesy = "22px auto auto 0";
        }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Rozmiar zdjęcia dużego",FriendlyOpis = "Rozmiar zdjęcia dużego")]
        [PobieranieSlownika(typeof(SlownikRozmiarZdjec))]
        public string RozmiarZdjecia { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Rozmiar miniatury",FriendlyOpis = "Rozmiar zdjęcia miniaturki")]
        [PobieranieSlownika(typeof(SlownikRozmiarZdjec))]
        public string RozmiarMiniatur { get; set; }

       [Niewymagane]
        public string NazwaZastepcza { get; set; }

        [FriendlyName("Tekst jeśli nie ma miniatur")]
        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Niewymagane]
        [Lokalizowane]
        public string TextZastepczy { get; set; }

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
        [FriendlyName("Otwieraj duży podgląd po kliknięciu na zdjęcie")]
        public bool DuzyPodglad { get; set; }

        public int JezykId { get; set; }
    }
}