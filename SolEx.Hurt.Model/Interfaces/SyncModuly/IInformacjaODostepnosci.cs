using System.Collections.Generic;

namespace SolEx.Hurt.Model.Interfaces.SyncModuly
{
     [SyncJakaOperacja(Enums.ElementySynchronizacji.InformacjaODostepnosci)]
    public interface IInformacjaODostepnosci
    {
         void Przetworz(  );
    }
}
