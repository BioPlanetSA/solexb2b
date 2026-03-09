using System.Collections.Generic;

namespace SolEx.Hurt.Model.Interfaces.Sync
{
    public interface IPobieranieProduktowUkrytych
    {
        List<ProduktUkryty> PobierzProduktyUkryte(Dictionary<long, KategoriaProduktu> kategorie);
    }
}
