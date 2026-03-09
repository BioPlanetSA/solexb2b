using System.Collections.Generic;

namespace SolEx.Hurt.Model.Interfaces.SyncModuly
{
     [SyncJakaOperacja(Enums.ElementySynchronizacji.PoziomyCenowe)]
    public interface IModulPoziomyCen
    {
         void Przetworz(ref Dictionary<int, PoziomCenowy> listaPoziomowCen, ref List< CenaPoziomu> ceny, Dictionary<int, PoziomCenowy> poziomyNaB2B, Dictionary<long, CenaPoziomu> cenyPoziomyB2B);
    }
}
