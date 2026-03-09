using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public class FiltryNaStronie : KontrolkaTresciBaza
    {
        public override string Grupa
        {
            get { return "Produkty"; }
        }

        public override string Nazwa
        {
            get { return "Stałe Filtry"; }
        }

        public override string Kontroler
        {
            get { return "StaleFiltry"; }
        }

        public override string Akcja
        {
            get { return "StaleFiltryDoPokazania"; }
        }

        public override string Opis
        {
            get { return "Kontrolka umożliwia wyświetlenie Filtrów"; }
        }

        [FriendlyName("Lista atrybutów z jakich mają być zbudowane filtry")]
        [WidoczneListaAdmin(true,true,true,true)]
        [PobieranieSlownika(typeof(SlownikAtrybutow))]
        [Niewymagane]
        public int[] ListaAtrybutow { get; set; }

    }
}