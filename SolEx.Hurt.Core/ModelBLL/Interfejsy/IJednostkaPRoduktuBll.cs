using System.Collections.Generic;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.CustomSearchCriteria;

namespace SolEx.Hurt.Core.ModelBLL.Interfejsy
{
    public interface IJednostkaProduktuBll
    {       
        string CacheNameProduktyJednostkiLista(int jezyk);
       // List<Jednostka> PobierzJednostki(JednostkiSearchCriteria criteria,int jezyk);
        Dictionary<long, Jednostka> PobierzJednostki(int jezyk);
        void UsunCache(IList<object> obj);
        void PoprawJednostkiPrzedAktualizacja(IList<Jednostka> obj);
    }
}