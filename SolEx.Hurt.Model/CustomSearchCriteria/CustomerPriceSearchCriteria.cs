using System.Collections.Generic;

namespace SolEx.Hurt.Model.CustomSearchCriteria
{
    public class CustomerPriceSearchCriteria:BaseSearchCriteria
    {

        private List<int> _klient_id = null;

        public List<int> klient_id
        {
            get { return _klient_id; }
        }
        public List<int> produkt_id
        {
            get { return _produkt_id; }
        }
        public List<decimal> cena_netto
        {
            get { return _cena_netto; }
        }
        public List<decimal> cena_brutto
        {
            get { return _cena_brutto; }
        }
        public List<decimal> cena_hurtowa_netto
        {
            get { return _cena_hurtowa_netto; }
        }
        public List<decimal> cena_hurtowa_brutto
        {
            get { return _cena_hurtowa_brutto; }
        }
        public List<int> typ_rabatu
        {
            get { return _typ_rabatu; }
        }
        public List<string> waluta
        {
            get { return _waluta; }
        }
        public List<decimal> rabat
        {
            get { return _rabat; }
        }

        public List<int> CenaId
        {
            get { return _cenaid; }
         
        }
        private List<int> _cenaid;
        private List<int> _produkt_id ;
        private List<decimal> _cena_netto ;
        private List<decimal> _cena_brutto ;
        private List<decimal> _cena_hurtowa_netto;
        private List<decimal> _cena_hurtowa_brutto;
        private List<int> _typ_rabatu;
        private List<string> _waluta;
        private List<decimal> _rabat;
        public CustomerPriceSearchCriteria()
        {
            _klient_id = new List<int>();
            _produkt_id = new List<int>();
            _cena_netto = new List<decimal>();
            _cena_brutto = new List<decimal>();
            _cena_hurtowa_brutto = new List<decimal>();
            _cena_hurtowa_netto = new List<decimal>();
            _typ_rabatu = new List<int>();
            _waluta = new List<string>();
            _rabat = new List<decimal>();
            _cenaid=new List<int>();
        }
    }
}
