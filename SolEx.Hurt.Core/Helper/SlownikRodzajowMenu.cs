using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolEx.Hurt.Core.Helper
{
    public class SlownikRodzajowMenu : SlownikWidokowBaza
    {
        protected override string SciezkaWidokow
        {
            get { return "\\Tresci\\RodzajeMenu"; }
        }
    }
}
