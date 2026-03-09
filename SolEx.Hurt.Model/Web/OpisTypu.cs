using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolEx.Hurt.Model.Web
{
    public class OpisTypu
    {
        private List<OpisTypu> _Dzieci; 
        public  string Nazwa { get; set; }
        public string Opis { get; set; }
        public Type TypPola { get; set; }
        public  List<OpisTypu> Dzieci {
            get { return _Dzieci; } 
        }
        public OpisTypu()
        {
            _Dzieci=new List<OpisTypu>();
        }
    }
}
