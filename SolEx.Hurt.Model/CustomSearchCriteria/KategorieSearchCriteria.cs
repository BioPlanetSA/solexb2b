using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolEx.Hurt.Model.CustomSearchCriteria
{
    public class KategorieSearchCriteria:BaseSearchCriteria
    {
        private List<int> _kategoria_id=null;
        private List<int> _grupa_id = null;
        private List<bool> _widoczna = null;
          private List<int?> _parent_id = null;
        public List<int> kategoria_id { get {return _kategoria_id; } }
        public List<int> grupa_id { get { return _grupa_id; } }
        public List<int?> parent_id { get { return _parent_id; } }
        public List<bool> widoczna { get { return _widoczna; } }
        public KategorieSearchCriteria()
        {
            _kategoria_id = new List<int>();
            _grupa_id = new List<int>();
            _parent_id = new List<int?>();
            _widoczna=new List<bool>();
        }
    }
}
