using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using SolEx.Hurt.Core;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class DrzewkoGrupyModel
    {
        public DrzewkoGrupyModel()
        {
            state =new state();
        }


        public DrzewkoGrupyModel(KategorieBLL x)
            : this()
        {
            id = x.Id.ToString(CultureInfo.InvariantCulture);
            parent = x.ParentId.HasValue ? x.ParentId.ToString() : "#";
            text = x.Nazwa;
            kolejnosc = x.Kolejnosc;
            url = x.ParentId == null ? "" : string.Format("/Admin/ModelujDrzewko?grupaid={0}&id={1}", x.GrupaId, x.Id);

        }
        public string id { get; set; }
        public string parent { get; set; }
        public string text { get; set; }
        public state state { get; set; }
        public int kolejnosc { get; set; }
        public string url { get; set; }
    }
}