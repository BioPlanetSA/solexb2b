namespace SolEx.Hurt.Model.CustomSearchCriteria
{
    using System;
    using System.Collections.Generic;

    public class HiddenProductsSearchCriteria : BaseSearchCriteria
    {
        private List<int> _klient_zrodlo_id = null;
        private List<int> _produkt_zrodlo_id = null;

        public HiddenProductsSearchCriteria()
        {
            this._produkt_zrodlo_id = new List<int>();
            this._klient_zrodlo_id = new List<int>();
        }

        public List<int> klient_zrodlo_id
        {
            get
            {
                return this._klient_zrodlo_id;
            }
        }

        public List<int> produkt_zrodlo_id
        {
            get
            {
                return this._produkt_zrodlo_id;
            }
        }
    }
}

