using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public class PokazAktywneFiltry : KontrolkaTresciBaza, INaglowekStopka
    {
        public override string Nazwa
        {
            get { return "Pokaż aktywne fitry"; }
        }

        public override string Kontroler
        {
            get { return "Filtry"; }
        }

        public override string Akcja
        {
            get { return "PokazAktywneFiltry"; }
        }


        public override string Grupa
        {
            get { return "Wygląd"; }
        }
        [FriendlyName("Nagłówek sekcji")]
        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Niewymagane]
        public string Naglowek { get; set; }

        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Niewymagane]
        [FriendlyName("Stopka",FriendlyOpis = "Stopka dla pola")]
        public string Stopka { get; set; }
    }
}