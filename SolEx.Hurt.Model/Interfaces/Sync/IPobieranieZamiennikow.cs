using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolEx.Hurt.Model.Interfaces.Sync
{
    public interface IPobieranieZamiennikow
    {
        List<ProduktyZamienniki> PobierzZamiennikiProduktu(long produkt);
    }
}
