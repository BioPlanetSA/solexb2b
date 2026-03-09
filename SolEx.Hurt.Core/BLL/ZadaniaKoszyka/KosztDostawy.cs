using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    [FriendlyName("Koszty dostawy", FriendlyOpis = "Dodanie kosztu dostawy")]
    public class KosztDostawy : DostawaBaza
    {
        //public override string Opis
        //{
        //    get { return "Dodanie kosztu dostawy"; }
        //}

        [FriendlyName("Cena netto dostawy - Jeśli pole niewypełnione to brak kosztu dostawy")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal? CenaNetto { get; set; }

        public override bool Wykonaj(IKoszykiBLL koszyk)
        {
            return ProduktDostawy != null;
        }

        public override decimal WyliczCene(IKoszykiBLL koszyk)
        {
            return CenaNetto.HasValue ? CenaNetto.Value : 0;
        }
    }
}