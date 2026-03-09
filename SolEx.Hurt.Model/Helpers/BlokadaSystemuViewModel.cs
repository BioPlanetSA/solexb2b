using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolEx.Hurt.Model.Helpers
{
    public class BlokadaSystemuViewModel
    {
        public string GodzinaStart { get; set; }
        public string GodzinaKoniec { get; set; }
        public string Powod { get; set; }

        public bool Zablokowany { get; set; }
    }
}
