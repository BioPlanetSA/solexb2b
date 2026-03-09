using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SolEx.Hurt.Core.ModelBLL;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class MetkaParametry
    {
        public List<CechyBll> ListaMetek { get; set; }
        public string DodatkoweKlasy { get; set; }


        public MetkaParametry(List<CechyBll> listametek, string dodatkoweKlasy)
        {
            ListaMetek = listametek;
            DodatkoweKlasy = dodatkoweKlasy;
        }
    }
}