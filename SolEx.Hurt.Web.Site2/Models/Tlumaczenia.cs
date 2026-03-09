using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class Tlumaczenia
    {
        public TlumaczeniePole TlumacznieJezykPodstawowy { get; set; }
        public Tlumaczenie[] Lokalizacje { get; set; }
    }
}