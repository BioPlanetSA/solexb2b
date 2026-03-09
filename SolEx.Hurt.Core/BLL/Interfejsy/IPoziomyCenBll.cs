using SolEx.Hurt.Model;
using System.Collections.Generic;

namespace SolEx.Hurt.Core.BLL.Interfejsy
{
    public interface IPoziomyCenBll
    {
        Dictionary<int, CenaPoziomu> PobierzCenyProduktu(long produkt);

        void UsunCache(IList<object> obj);
        Dictionary<int, CenaPoziomu> SztucznyPoziomCenowyZerowy { get; set; }
    }
}