using ServiceStack.DataAnnotations;

namespace SolEx.Hurt.Core.ModelBLL
{
    [Alias("Zadanie")]
    public class HarmonogramBll : ZadanieBll
    {
        public HarmonogramBll()
        {}

        public HarmonogramBll(ZadanieBll x) : base(x)
        {}
    }
}
