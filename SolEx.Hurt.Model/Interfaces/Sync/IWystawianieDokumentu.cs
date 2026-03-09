using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Model.Interfaces.Sync
{
    public interface IWystawianieDokumentu
    {
        string WystawDokument(ZamowienieSynchronizacja zamowienie, int dlakogo, string dlakogoSymbol, string magazyn, TypDokumentu typ, out string braki);
    }
}
