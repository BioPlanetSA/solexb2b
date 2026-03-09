using System.Collections.Generic;

namespace SolEx.Hurt.Model.Interfaces.SyncModuly
{
    [SyncJakaOperacja(Enums.ElementySynchronizacji.CechyIAtrybuty)]
    public interface IModulCechyIAtrybuty
    {
          void Przetworz(ref List<Atrybut> atrybuty, ref List<Cecha> cechy, Dictionary<long, Produkt> produktyNaB2B);
    }
}
