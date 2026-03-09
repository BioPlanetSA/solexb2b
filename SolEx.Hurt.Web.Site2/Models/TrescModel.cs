using System.Globalization;
using System.Security.Policy;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Web.Site2.Helper;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class TrescModel
    {

        public TrescModel()
        {
            state=new state();
           li_attr=new attr();
            a_attr=new attr();
        }


        public TrescModel(TrescBll x):this()
        {
            string symbolJezyk =SolexBllCalosc.PobierzInstancje.Konfiguracja.WieleJezykowWSystemie? "/"+SolexHelper.PobierzInstancjeZCache().AktualnyJezyk.Symbol:"";
            jezyk = x.JezykId;
            id = x.Id.ToString(CultureInfo.InvariantCulture);
            parent=x.NadrzednaId.HasValue ? x.NadrzednaId.ToString() : "#";
            text = x.Nazwa;
            kolejnosc = x.Kolejnosc;
            //  url = x.NadrzednaId == null ? "" : string.Format("/Admin/TrescZawartoscEdycja/{0}?jezyk={1}", x.Id,x.JezykId);
            url = x.NadrzednaId == null ? "" : string.Format(symbolJezyk+"/Admin/TrescZawartoscEdycja/{0}", x.Id);
            usuwanieurl = x.NadrzednaId == null ? "" : string.Format(symbolJezyk+"/Admin/usuwanie/{0}?przekieruj={1}&typ={2}", x.Id,false,typeof(TrescBll).PobierzOpisTypu());

            li_attr.@class = "dostep-" + x.Dostep;
            li_attr.@class += " aktywnosc-" + (x.Aktywny?"tak":"nie");
        }
        public string id { get; set; }
        public string parent { get; set; }
        public string text { get; set; }
        public state state { get; set; }
        public int kolejnosc { get; set; }
        public string url { get; set; }
        public string usuwanieurl { get; set; }
        public int jezyk { get; set; }
        public attr li_attr { get; set; }
        public attr a_attr { get; set; }
    }

    public class attr
    {
        public string @class { get; set; }
    }
    public class state
    {
        public bool opened { get; set; }
        public bool selected { get; set; }
        public bool disabled { get; set; }
    }
}