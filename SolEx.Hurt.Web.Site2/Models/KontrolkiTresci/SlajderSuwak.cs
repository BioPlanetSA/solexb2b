using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public class SlajderSuwak : SlajderKontrolka
    {
        public SlajderSuwak()
        {
            IloscElementowWWierszu = 1;
        }
        [Lokalizowane]
        public override string Nazwa
        {
            get { return base.Nazwa+" Suwak"; }
        }
        public override string Akcja
        {
            get { return "Suwak"; }
        }

        public override string Opis
        {
            get { return "Wyświetla slajder w postaci suwaka"; }
        }

        [WidoczneListaAdmin(false, false, true, false)]
        [FriendlyName("Ilość elementów w wierszu")]
        public int IloscElementowWWierszu { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Rozmiar zdjęcia",FriendlyOpis = "Rozmiar zdjęcia dużego")]
        [PobieranieSlownika(typeof(SlownikRozmiarZdjec))]
        [Niewymagane]
        public string RozmiarZdjecia { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Dodatkowe klasy css dla poszczególnych slajdów")]
        [Niewymagane]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikKlassCss,SolEx.Hurt.Core")]
        public string[] DodatkoweKlasyCssElementyKontrolki { get; set; }
    }
}