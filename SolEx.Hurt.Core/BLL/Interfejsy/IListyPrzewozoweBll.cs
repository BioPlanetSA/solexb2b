using System.Collections.Generic;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.Interfejsy
{
    public interface IListyPrzewozoweBll
    {
        List<HistoriaDokumentuListPrzewozowy> TworzenieListuPrzewozowego(int nrDokumentu, string nrListu, string format);
        void WyslijPowiadomienie(IList<HistoriaDokumentuListPrzewozowy> obj);
    }
}