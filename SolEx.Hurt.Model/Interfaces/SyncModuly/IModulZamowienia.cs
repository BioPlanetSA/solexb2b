using System.Collections.Generic;
using SolEx.Hurt.Model.Interfaces.Sync;

namespace SolEx.Hurt.Model.Interfaces.SyncModuly
{
    [SyncJakaOperacja(Enums.ElementySynchronizacji.ImportZamówień)]
    public interface IModulZamowienia
    {
        void Przetworz(ZamowienieSynchronizacja zamowienieWejsciowe, ref List<ZamowienieSynchronizacja> wszystkie, ISyncProvider provider, Dictionary<long, Jednostka> jednostki,
            Dictionary<long, ProduktJednostka> laczniki, Dictionary<long, Produkt> produktyB2B, List<Cecha> cechyB2B, List<ProduktCecha> lacznikiCech);
    }
}
