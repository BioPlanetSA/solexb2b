using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SolEx.Hurt.Model.Interfaces.SyncModuly
{
    [SyncJakaOperacja(Enums.ElementySynchronizacji.Dokumenty)]
    public interface IModulDokumenty
    {
        void Przetworz(ref ConcurrentDictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>> listaWejsciowa, ref List<StatusZamowienia> statusy,Dictionary<int, long> hashe,ref List<Klient> klienci  );
    }
}
