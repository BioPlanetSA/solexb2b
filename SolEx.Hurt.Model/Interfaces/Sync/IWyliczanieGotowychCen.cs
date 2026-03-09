using System.Collections.Generic;

namespace SolEx.Hurt.Model.Interfaces.Sync
{
    public interface IWyliczanieGotowychCen
    {
        List<FlatCeny> WyliczCenyKlienta(long klient);
    }
}
