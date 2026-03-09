using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolEx.Hurt.Model.CustomSearchCriteria
{
    public class AdressSearchCriteria : BaseSearchCriteria
    {
        private List<int> _id = null;
        private List<string> _nazwa = null;
        private List<string> _ulica_nr = null;
        private List<string> _miasto = null;
        private List<string> _kod_pocztowy = null;
        private List<string> _kraj = null;
        private List<int> _klient_id = null;
        private List<bool> _domyslny = null;
        private List<bool> _glowny = null;
        public AdressSearchCriteria()
        {
            _id = new List<int>();
            _nazwa = new List<string>();
            _ulica_nr = new List<string>();
            _miasto = new List<string>();
            _kod_pocztowy = new List<string>();
            _kraj = new List<string>();
            _klient_id = new List<int>();
            _domyslny = new List<bool>();
            _glowny = new List<bool>();

        }
        public List<int> Id { get { return _id; } }
        public List<string> nazwa { get { return _nazwa; } }
        public List<string> UlicaNr { get { return _ulica_nr; } }
        public List<string> miasto { get { return _miasto; } }
        public List<string> KodPocztowy { get { return _kod_pocztowy; } }
        public List<string> kraj { get { return _kraj; } }
        public List<int> KlientId { get { return _klient_id; } }
        public List<bool> domyslny { get { return _domyslny; } }
        public List<bool> glowny { get { return _glowny; } }
    }
}
