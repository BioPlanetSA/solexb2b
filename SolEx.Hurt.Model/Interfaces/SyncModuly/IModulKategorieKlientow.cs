using System.Collections.Generic;

namespace SolEx.Hurt.Model.Interfaces.SyncModuly
{
     [SyncJakaOperacja(Enums.ElementySynchronizacji.KategorieKlientów)]
    public interface IModulKategorieKlientow
    {
         void Przetworz(ref List<KategoriaKlienta> kategorie, ref List<KlientKategoriaKlienta> laczniki);
    }
}
