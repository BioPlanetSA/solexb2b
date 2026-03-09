using System.Linq;
using System.Text;
using System.IO;
using SolEx.Hurt.Model.Core;

namespace SolEx.Hurt.Model
{
    /// <summary>
    /// Status eksportu rejestracji
    /// </summary>
    public enum RegisterExportStatus
    {
        DontExport =0,
        Export =1,
        Exported =2,
        Error=3
    }
    /// <summary>
    /// Status rejestracji
    /// </summary>
    public enum RegisterStatus
    {
        Wait = 0,
        Accepted = 1,
        Canceled = 2,
        Exists = 3,
        Error=4
    }
}
