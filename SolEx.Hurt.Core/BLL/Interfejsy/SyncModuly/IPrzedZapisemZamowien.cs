using System.Collections.Generic;
using SolEx.Hurt.Core.BLL.Interfejsy.Sync;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Core.BLL.Interfejsy.SyncModuly
{
        [SyncJakaOperacja(Model.Enums.ElementySynchronizacji.ImportZamówień)]
    public interface IPrzedZapisemZamowien
    {
        void Przetworz(List<ZamowienieSynchronizacja> zamowieniaDoZapisania, Dictionary<int, Model.Klient> wszyscy, Dictionary<int, Produkt> produktyB2B, Dictionary<int, Jednostka> jednostki, Dictionary<string, ProduktJednostka> produktyjednostki, ISyncProvider aktualnyProvider);
    }
}
