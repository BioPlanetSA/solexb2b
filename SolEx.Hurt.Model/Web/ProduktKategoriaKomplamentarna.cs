using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolEx.Hurt.Model.Web
{
    public class ProduktKategoriaKomplamentarna
    {
        public int produkt_id { get; set; }
        public int? kategoria_id {get;set;}
        public string komplementarna_nazwa { get; set; }
    }
}
