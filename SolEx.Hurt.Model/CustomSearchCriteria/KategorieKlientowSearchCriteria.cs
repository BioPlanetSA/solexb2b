using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolEx.Hurt.Model.CustomSearchCriteria
{
    public class KategorieKlientowSearchCriteria:BaseSearchCriteria
    {
        private List<int> _kategoria_id=null;
        private List<string> _grupa = null;
        public List<int> Id { get {return _kategoria_id; } }
        public List<string> grupa { get { return _grupa; } }
        public KategorieKlientowSearchCriteria()
        {
            _kategoria_id = new List<int>();
            _grupa=new List<string>();
          
        }
    }
}
