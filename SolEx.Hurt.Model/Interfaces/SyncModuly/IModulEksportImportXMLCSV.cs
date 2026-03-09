using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolEx.Hurt.Model.Interfaces.SyncModuly
{
    [SyncJakaOperacja(Model.Enums.ElementySynchronizacji.ImportEksportXMLCSV)]
    public interface IModulEksportImportXMLCSV
    {
        void Przetworz();
    }
}
