using System.Collections.Generic;

namespace SolEx.Hurt.Model.Interfaces.SyncModuly
{
    [SyncJakaOperacja(Model.Enums.ElementySynchronizacji.ImportZamówień)]
    public interface IPrzedZapisemZamowien
    {
        void Przetworz(List<ZamowienieSynchronizacja> zamowieniaDoZapisania, Dictionary<long, Klient> wszyscy, Dictionary<long, Produkt> produktyB2B, 
            Dictionary<long, Jednostka> jednostki, Dictionary<long, ProduktJednostka> produktyjednostki, Sync.ISyncProvider aktualnyProvider);
    }
}
