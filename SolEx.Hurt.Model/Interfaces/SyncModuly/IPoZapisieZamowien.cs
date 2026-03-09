using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolEx.Hurt.Model.Interfaces.Sync;

namespace SolEx.Hurt.Model.Interfaces.SyncModuly
{
        [SyncJakaOperacja(Model.Enums.ElementySynchronizacji.ImportZamówień)]
    public interface IPoZapisieZamowien
    {
        void Przetworz(List<ZamowienieSynchronizacja> zapisane, List<ZamowienieSynchronizacja> zamowieniaDoZapisania, Dictionary<long, Klient> wszyscy,
            Dictionary<long, Produkt> produktyB2B, Dictionary<long, Jednostka> jednostki, Dictionary<long, ProduktJednostka> produktyjednostki, Sync.ISyncProvider aktualnyProvider);
    }
}
