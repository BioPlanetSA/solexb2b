using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public class SlajderFullWhidth : SlajderKontrolka
    {
        [Lokalizowane]
        public override string Nazwa
        {
            get { return base.Nazwa + " FullWhidth"; }
        }
        public override string Akcja
        {
            get { return "FullWidthSlider"; }
        }

        public override string Opis
        {
            get { return "Wyświetla slajder na cały ekran"; }
        }
    }
}