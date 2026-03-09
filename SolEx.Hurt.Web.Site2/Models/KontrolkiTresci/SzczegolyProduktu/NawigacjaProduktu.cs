using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.SzczegolyProduktu
{
    public class NawigacjaProduktu : SzczegolyProduktuBaza
    {

        public override string Nazwa
        {
            get { return "Nawigacja między produktami"; }
        }

        public override string Akcja
        {
            get { return "Nawigacja"; }
        }

        public override string Opis
        {
            get { return "Dodaje linki do Poprzedniego/Następnego produktu z wybranej grupy"; }
        }

        [WidoczneListaAdmin(true, true, true, true)]
        [PobieranieSlownika(typeof(SlownikGrupy))]
        [Niewymagane]
        public int[] GrupaProd { get; set; }

    }

}