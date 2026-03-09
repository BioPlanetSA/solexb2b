using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public class PowiadomieniaMailowe : KontrolkaTresciBaza, IZastepczaNazwaWartosc
    {

        public PowiadomieniaMailowe()
        {
            TextZastepczy = "Brak powiadomień do wybrania";
        }
        public override string Nazwa
        {
            get { return "Zarządzanie powiadomieniami mailowymi klienta"; }
        }

        public override string Kontroler
        {
            get { return "ProfilKlienta"; }
        }

        public override string Akcja
        {
            get { return "PowiadomieniaMailowe"; }
        }

        [FriendlyName("Tekst jeśli nie ma danych")]
        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Niewymagane]
        public string TextZastepczy { get; set; }

        [Niewymagane]
        public string NazwaZastepcza { get; set; }
    }
}