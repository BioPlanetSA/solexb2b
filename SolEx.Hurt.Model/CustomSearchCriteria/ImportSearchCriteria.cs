using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolEx.Hurt.Model.CustomSearchCriteria
{
    public class ImportSearchCriteria:BaseSearchCriteria
    {
        private List<int> _id; 
        public List<int> id {
            get { return _id; }
        }
        private List<int> _kierunek;
        public List<int> kierunek
        {
            get { return _kierunek; }
        }
        public ImportSearchCriteria()
        {
            _id=new List<int>();
            _kierunek=new List<int>();
        }
    }
}
