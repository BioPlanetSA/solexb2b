using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public class SlajderFullPage:SlajderKontrolka
    {
        [Lokalizowane]
        public override string Nazwa
        {
            get { return base.Nazwa + " FullPage"; }
        }
        public override string Akcja
        {
            get { return "FullPage"; }
        }

        public override string Opis
        {
            get { return "Wyświetla slajder na cały ekran"; }
        }
    }
}