using System.Collections.Generic;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces.Sync;

namespace SolEx.Hurt.Model.Interfaces.SyncModuly
{
     [SyncJakaOperacja(ElementySynchronizacji.WyliczenieGotowychCen)]
    public interface IModulWyliczanieGotowychCen
     {
        void Przetworz(ref List<FlatCeny> wynik, Dictionary<long, Klient> dlaKogoLiczyc,  ISyncProvider aktualnyProvider);
     }
}
