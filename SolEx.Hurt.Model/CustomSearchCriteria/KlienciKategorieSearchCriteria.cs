using System.Collections.Generic;

namespace SolEx.Hurt.Model.CustomSearchCriteria
{
    public class KlienciKategorieSearchCriteria:BaseSearchCriteria
    {

        public  List<int> klient_id {
            get { return _klient_id; }
        }
        public List<int> kategoria_klientow_id
        {
            get { return _kategoria_klientow_id; }
        }
        public List<int> Id
        {
            get { return _id; }
        }
        private List<int> _klient_id;
        private List<int> _kategoria_klientow_id;
        private List<int> _id; 
        public KlienciKategorieSearchCriteria()
        {
            _klient_id=new List<int>();
            _kategoria_klientow_id=new List<int>();
            _id=new List<int>();
        }
    }
}
