
using System.Collections.Generic;
namespace SolEx.Hurt.Model.CustomSearchCriteria
{
    public class DokumentyPozycjeSearchCriteria: BaseSearchCriteria
    {
        private List<int> _dokument_id;
        private List<int> _id;
        public DokumentyPozycjeSearchCriteria()
        {
          
            _dokument_id = new List<int>();
            _id = new List<int>();
        
        }
        public List<int> dokument_id { get { return _dokument_id; } }
        public List<int> id { get { return _id; } }
      
    }
}
