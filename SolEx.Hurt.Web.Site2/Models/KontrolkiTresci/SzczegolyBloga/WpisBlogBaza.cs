using SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.Bazowe;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.SzczegolyBloga
{
    public abstract class WpisBlogBaza : PokazWpisBaza
    {
        public override string SymbolIdentyfikatora
        {
            get
            {
                return "blogWpisId";
            }
        }

        public override string ModelObiektu
        {
            get { return "Blog"; }
        }
    }
}