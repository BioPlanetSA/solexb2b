using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public class AkceptacjaCiasteczek : KontrolkaTresciBaza, IPoleJezyk
    {
        public AkceptacjaCiasteczek()
        {
            Tresc = "Ten serwis wykorzystuje pliki cookies. Korzystanie z witryny oznacza zgodę na ich zapis lub odczyt wg ustawień przeglądarki.";
        }
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Treść")]
        [Niewymagane]
        [Lokalizowane]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        public string Tresc { get; set; }

        public override string Nazwa
        {
            get { return "Akceptacja ciasteczek"; }
        }

        public override string Kontroler
        {
            get { return "Wyglad"; }
        }

        public override string Akcja
        {
            get { return "Ciasteczka"; }
        }

        public override string Grupa
        {
            get { return "Wygląd"; }
        }
        public int JezykId { get; set; }
    }
}