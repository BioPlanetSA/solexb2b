using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public class JavaScript : Tekst
    {
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("JavaScript")]
        [Niewymagane]
        [Lokalizowane]
        [WymuszonyTypEdytora(TypEdytora.PoleTekstoweMultiLine)]
        public new string Tresc { get; set; }

        public override string Nazwa
        {
            get { return "JavaScript"; }
        }

        public override string Kontroler
        {
            get { return "Tresci"; }
        }

        public override string Akcja
        {
            get { return "Tekst"; }
        }

        public override string Grupa
        {
            get { return "Wygląd"; }
        }

        public new int JezykId { get; set; }
    }
}