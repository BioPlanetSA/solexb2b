using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolEx.Hurt.Model.CustomSearchCriteria
{
    public class CechyProduktySearchCriteria:BaseSearchCriteria
    {
        private List<int> _id = null;
        private List<int> _produkt_id = null;
         private List<int> _cecha_id = null;
         public List<int> id { get { return _id; } }
         public List<int> produkt_id { get { return _produkt_id; } }
        public List<int> cecha_id { get { return _cecha_id; } }
        public CechyProduktySearchCriteria()
        {
            _produkt_id = new List<int>();
            _cecha_id = new List<int>();
            _id = new List<int>();
        }
    }
}
