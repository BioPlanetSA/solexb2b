using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolEx.Hurt.Model.Interfaces.Sync;

namespace SolEx.Hurt.Model.Interfaces.SyncModuly
{
        [SyncJakaOperacja(Enums.ElementySynchronizacji.ZadaniaOgolne)]
    public interface IZadaniaOglone
    {
        void Przetworz(ISyncProvider provider);
    }
}
