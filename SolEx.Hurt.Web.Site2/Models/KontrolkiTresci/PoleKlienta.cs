using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public class PoleKlienta : KontrolkaTresciBaza
    {
        public override string Grupa
        {
            get { return "Klienci"; }
        }

        public override string Nazwa
        {
            get { return "Pole klienta"; }
        }

        public override string Kontroler
        {
            get { return "Klienci"; }
        }

        public override string Akcja
        {
            get { return "PoleKlienta"; }
        }

        public override string Opis
        {
            get { return "Kontrolka umożliwia wyświetlenie dowolnego pola klienta"; }
        }

        [FriendlyName("Wartość do pokazania")]
        [WidoczneListaAdmin(true,true,true,true)]
        [PobieranieSlownika(typeof(SlownikPolKlienta))]
        [Niewymagane]
        public string WartoscDoPokazania { get; set; }

        [Niewymagane]
        [WidoczneListaAdmin(true, true, true, true)]
        public string Przedrostek { get; set; }

        [Niewymagane]
        [WidoczneListaAdmin(true, true, true, true)]
        public string Przyrostek { get; set; }

        [FriendlyName("Fraza do pokazania")]
        [WidoczneListaAdmin(true, true, true, true)]
        [Niewymagane]
        public string FrazaDoPokazania { get; set; }
    }
}