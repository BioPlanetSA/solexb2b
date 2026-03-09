using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.SzczegolyBloga
{
    public class GaleriaZdjec : WpisBlogBaza, INaglowekStopka, IPoleJezyk
    {
        private string _kontroler = "Blog";

        public override string Nazwa
        {
            get { return "Galeria Zdjęć"; }
        }

        public override string Kontroler
        {
            get { return _kontroler; }
        }
        public override string Grupa
        {
            get { return "Blog"; }
        }

        public override string Akcja
        {
            get { return "GaleriaZdjec"; }
        }

        public GaleriaZdjec()
        {
            RozmiarMiniatur = "ico85x90p";
            RozmiarZdjecia  = "ico570x630p";
        }

        [PobieranieSlownika(typeof(SlownikKatalogZasoby))]
        [FriendlyName("Folder do galerii zdjęć")]
        [WidoczneListaAdmin(true, false, true, false)]
        [Niewymagane]
        public string FolderZdjecDoGalerii { get; set; }

        [FriendlyName("Folder do galerii z ustawień Wpisu Bloga - jeśli nie ma wybranej opcji 'Folder do galerii zdjęć' to dopiero wybiera z tego pola.")]
        [PobieranieSlownika(typeof(SlownikPolBlogWpis))]
        [WidoczneListaAdmin(true, false, true, false)]
        [Niewymagane]
        public string[] FolderDoGaleriiZdjecZWpisu { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Rozmiar zdjęcia dużego",FriendlyOpis = "Rozmiar zdjęcia dużego")]
        [PobieranieSlownika(typeof(SlownikRozmiarZdjec))]
        public string RozmiarZdjecia { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Rozmiar miniatury",FriendlyOpis = "Rozmiar zdjęcia miniaturki")]
        [PobieranieSlownika(typeof(SlownikRozmiarZdjec))]
        public string RozmiarMiniatur { get; set; }

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

        public int JezykId{ get; set; }
    }
}