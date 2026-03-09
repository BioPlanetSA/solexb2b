using System.Collections.Generic;
using SolEx.Hurt.Model.Interfaces.Sync;

namespace SolEx.Hurt.Model.Interfaces.SyncModuly
{
     [SyncJakaOperacja(Enums.ElementySynchronizacji.ListyPrzewozowe)]
    public interface IModulListyPrzewozowe
    {
        void Przetworz(ref List<HistoriaDokumentuListPrzewozowy> listaWejsciowa, Dictionary<int, long> dokumentyWErp, ISyncProvider provider);
    }
}
