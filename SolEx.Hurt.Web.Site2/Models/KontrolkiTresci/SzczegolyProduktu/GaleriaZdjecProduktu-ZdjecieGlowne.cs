using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.SzczegolyProduktu
{
    public class GaleriaZdjecProduktu_ZdjecieGlowne : SzczegolyProduktuBaza, INaglowekStopka, IPoleJezyk
    {
        public override string Nazwa
        {
            get { return "Galeria Zdjęć - zdjęcia główne"; }
        }

        public override string Akcja
        {
            get { return "GaleriaZdjecZdjecieGlowne"; }
        }

        public GaleriaZdjecProduktu_ZdjecieGlowne()
        {
            RozmiarZdjecia = "ico570x630p";
            DuzyPodglad = true;
        }
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Pokazuj pobierz zdjecie w pełnej wersji",FriendlyOpis = "Pokazuje przycisk który umożliwia pobranie zdjęcia w pełnej wersji")]
        public bool PobierzPelny { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Otwieraj duży podgląd po kliknięciu na zdjęcie")]
        public bool DuzyPodglad { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Rozmiar zdjęcia dużego")]
        [PobieranieSlownika(typeof(SlownikRozmiarZdjec))]
        public string RozmiarZdjecia { get; set; }

       
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

        public int JezykId { get; set; }
    }
}