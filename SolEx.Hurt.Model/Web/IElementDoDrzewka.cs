using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolEx.Hurt.Model.Web
{

    public class ElementDoDrzewka
    {
       public int id { get; set; }
       public string label { get; set; }
       public List<ElementDoDrzewka> branch { get; set; }
        public bool inode { get; set; }
        public bool Aktywny { get; set; }
        public ElementDoDrzewka()
        {
            branch=new List<ElementDoDrzewka>();
            inode = true;
            test = "dddd";
        }
        public string test { get; set; }
    }
    public class AktualizacjaDrzewka
    {
        public int id { get; set; }
        public int? parent { get; set; }
        public int position { get; set; }
        public bool aktywnosc { get; set; }

        public bool ZablokowanyParent { get; set; }

        public bool ZablokowanaKolejnosc { get; set; }
    }
}
