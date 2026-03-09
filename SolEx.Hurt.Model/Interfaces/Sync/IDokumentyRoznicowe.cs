using System;
using System.Collections.Generic;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Model.Interfaces.Sync
{
    public interface IDokumentyRoznicowe
    {
        /// <summary>
        /// Lista dokumentów do wysłania na platformę
        /// </summary>
        /// <param name="dokumentyNaPlatformie"></param>
        /// <param name="from"></param>
        /// <param name="klienciNaPlatformie"></param>
        /// <returns></returns>
        Dictionary<HistoriaDokumentu,List<HistoriaDokumentuProdukt>> DokumentyDoWyslania(Dictionary<int, long> dokumentyNaPlatformie, DateTime from, List<Klient> klienciNaPlatformie);

        /// <summary>
        /// Lista id dokumentów do usunięcia z platformy
        /// </summary>
        /// <param name="dokumentyNaPlatformie"></param>
        /// <param name="klienciNaB2b"></param>
        /// <returns></returns>
        List<int> DokumentyDoUsuniecia(Dictionary<int, long> dokumentyNaPlatformie, HashSet<long>klienciNaB2b );
    }
}
