using System.Collections.Generic;
using SolEx.Hurt.Model.Interfaces.Sync;

namespace SolEx.Hurt.Model.Interfaces.SyncModuly
{
    [SyncJakaOperacja(Enums.ElementySynchronizacji.KategorieProduktów)]
    public interface IModulKategorieProduktow
    {
        void Przetworz(ref Dictionary<long, KategoriaProduktu> listaWejsciowa, Dictionary<long, KategoriaProduktu> listaKategoriiB2B, ISyncProvider provider, List<Grupa> grupyPRoduktow);
    }
}
