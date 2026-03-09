using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolEx.Hurt.Model.CustomSearchCriteria
{
    public class KatalogSzablonSearchCriteria : BaseSearchCriteria
    {
        private List<int> _id = null;
        private List<string> _nazwa = null;
        private List<int> __kategorieKlientowID = null;

        public bool? Aktywny { get; set; }
        public List<int> Id { get { return _id; } }
        public List<string> nazwa { get { return _nazwa; } }
        public List<int> _kategorieKlientowID { get { return __kategorieKlientowID; } set { __kategorieKlientowID = value; }  }

        public KatalogSzablonSearchCriteria()
        {
            _id = new List<int>();
            _nazwa = new List<string>();
            __kategorieKlientowID = new List<int>();
        }
        
    }
}
