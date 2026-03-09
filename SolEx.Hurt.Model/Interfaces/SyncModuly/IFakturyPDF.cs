using System.Collections.Generic;

namespace SolEx.Hurt.Model.Interfaces.SyncModuly
{
     [SyncJakaOperacja(Enums.ElementySynchronizacji.WysylanieFakturPDF)]
    public interface IFakturyPdf
    {
        void Przetworz(ref List<StatusDokumentuPDF> dokumentyNab2B, Sync.ISyncProvider aktualnyProvider);
    }
}
 