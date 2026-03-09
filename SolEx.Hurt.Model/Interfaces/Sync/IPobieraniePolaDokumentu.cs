using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolEx.Hurt.Model.Interfaces.Sync
{
    public interface IPobieraniePolaDokumentu
    {
        string PobierzPole(int dokumentId, string pole);
        Dictionary<int, Dictionary<string, string>> PobierzPole(HashSet<int> dokumentyId);
    }
}
