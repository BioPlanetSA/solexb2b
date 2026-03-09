using System.Collections.Generic;

namespace SolEx.Hurt.Core.BLL.Interfejsy
{
    public interface IProduktyKategorieDostep
    {
        IDictionary<long, HashSet<long>> ProduktyKategorieGrupowanePoProdukcie { get; }
        IDictionary<long, HashSet<long>> ProduktyKategorieGrupowanePoKategorii { get; }
        void WyczyscCacheKategorii(IList<object> obj);
    }
}