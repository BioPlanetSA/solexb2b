using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public class KategoriaKlienta : KontrolkaTresciBaza
    {
        public override string Grupa
        {
            get { return "Klienci"; }
        }

        public override string Nazwa
        {
            get { return "Kategoria klienta"; }
        }

        public override string Kontroler
        {
            get { return "Klienci"; }
        }

        public override string Akcja
        {
            get { return "KategoriaKlienta"; }
        }

        [FriendlyName("Grupa Kategorii jaką chcesz pokazać")]
        [WidoczneListaAdmin(true, true, true, true)]
        [PobieranieSlownika(typeof(SlownikGrupyKategoriiKlienta))]
        [Wymagane]
        public string GrupaKategoriiDoPokazania { get; set; }

        [Niewymagane]
        [WidoczneListaAdmin(true, true, true, true)]
        public string Przedrostek { get; set; }

        [Niewymagane]
        [WidoczneListaAdmin(true, true, true, true)]
        public string Przyrostek { get; set; }

    }
}