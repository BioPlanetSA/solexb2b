using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolEx.Hurt.Model.Interfaces
{
    public interface ISlownik
    {
        Dictionary<string, object> PobierzWartosci();
    }
}
