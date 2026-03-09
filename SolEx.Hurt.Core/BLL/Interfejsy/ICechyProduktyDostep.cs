using System.Collections.Generic;

namespace SolEx.Hurt.Core.BLL
{
    public interface ICechyProduktyDostep
    {
        Dictionary<long, HashSet<long>> WszystkieLacznikiWgProduktow { get; }
        HashSet<long> PobierzIdCechProduktu(long produktId);
        Dictionary<long, HashSet<long>> WszystkieLacznikiWgCech { get; }
        void CzyscCache(IList<object> kluczeDoKasowania = null);
        HashSet<long> PobierzProduktyZCechami(HashSet<long> cechyId);
    }
}