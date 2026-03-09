using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class ParametryDoPodsumowaniaLimitow
    {
        public SzablonLimitow Szablon { get; set; }
        public dynamic WykorzystanyLimit { get; set; }
        public DateTime DoKiedy { get; set; }
        public RodzajLimitu Rodzaj { get; set; }
        public ParametryDoPodsumowaniaLimitow(SzablonLimitow szablon, dynamic wykorzystanyLimit, DateTime doKiedy, RodzajLimitu rodzaj)
        {
            Szablon = szablon;
            WykorzystanyLimit = wykorzystanyLimit;
            DoKiedy = doKiedy;
            Rodzaj = rodzaj;
        }
    }
}