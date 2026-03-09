using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolEx.Hurt.Model.Interfaces.SyncModuly
{
    [SyncJakaOperacja(Model.Enums.ElementySynchronizacji.StanyProduktów)]
    public interface IModulStany
    {
        void Przetworz(ref Dictionary<int,List<ProduktStan>> listaWejsciowa, List<Magazyn> magazyny, List<Produkt> produktyB2b );
    }
}
