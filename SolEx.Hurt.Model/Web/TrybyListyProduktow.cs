using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolEx.Hurt.Model.Web
{
    public class TrybListyProduktow
    {
        public string Nazwa { get; set; }
        public bool Aktywny { get; set; }
        public string Link { get; set; }
        public string Symbol { get; set; }
        public int Kolejnosc { get; set; }
        public string Ikona { get; set; }
    }
}
