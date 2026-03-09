using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public class DokumentyPodsumowanie : KontrolkaTresciBaza
    {
        public DokumentyPodsumowanie()
        {
            UkryjKredytKupiecki = false;
        }

        public override string Grupa
        {
            get { return "Dokumenty"; }
        }
        [Lokalizowane]
        public override string Nazwa
        {
            get { return "Dokumenty podsumowanie - kredyt kupiecki klienta"; }
        }

        public override string Kontroler
        {
            get { return "Dokumenty"; }
        }

        public override string Akcja
        {
            get { return "Podsumowanie"; }
        }

        [FriendlyName("Czy ukryć info o kredycie kupieckim na dokumentach")]
        [WidoczneListaAdmin(true, true, true, true)]
        public bool UkryjKredytKupiecki { get; set; }
    }
}