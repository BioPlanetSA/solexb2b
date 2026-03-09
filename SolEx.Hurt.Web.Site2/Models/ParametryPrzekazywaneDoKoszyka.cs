using System;
using System.Collections.Generic;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Web.Site2.Models
{

    public class DanewyborKoszyka
    {
        public int IdWybranego { get; set; }
        public  TypKoszyka Typ { get; set; }
        public Dictionary<long, KeyValuePair<string, KoszykBll>> SlownikKoszykow { get; set; }

        public bool PokazujDaty { get; set; }

        public bool PokazujZarzadzanie { get; set; }

    }

}