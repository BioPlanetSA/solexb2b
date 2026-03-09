using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolEx.Hurt.Model.CustomSearchCriteria
{
    public class KoszykiPozycjeSearchCriteria : BaseSearchCriteria
    {
         private List<int> _id = null;
         private List<int> _koszyk_id = null;
         private List<int> _produkt_id = null;
        public List<int> id { get { return _id; } }
        public List<int> koszyk_id { get { return _koszyk_id; } }
        public List<int> produkt_id { get { return _produkt_id; } }
        public KoszykiPozycjeSearchCriteria()
        {
            _id = new List<int>();
            _koszyk_id = new List<int>();
            _produkt_id = new List<int>();
        }
    }
}
