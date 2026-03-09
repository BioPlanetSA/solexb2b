using System.Collections.Generic;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Model.CustomSearchCriteria
{
    public class KoszykiSearchCriteria:BaseSearchCriteria
    {
        private List<int> _id;
        private List<int> _klient_id;
        private List<TypKoszyka> _typ_id;
        public List<int> id { get { return _id; } }
        public List<int> klient_id { get { return _klient_id; } }
        public List<TypKoszyka> Typ { get { return _typ_id; } }
        public KoszykiSearchCriteria()
        {
            _id = new List<int>();
            _klient_id = new List<int>();
            _typ_id = new List<TypKoszyka>();
        }
    }
}
