namespace SolEx.Hurt.Model
{
    public class JednostkaProduktu: Jednostka
    {
        private ProduktJednostka _lacznik;
        public JednostkaProduktu()
        {
            _lacznik = new ProduktJednostka{PrzelicznikIlosc = 1};
        }

        public JednostkaProduktu(bool podstawowa,long id,long produkt,string nazwa,decimal przelicznik)
        {
            Nazwa = nazwa;
            Id = id;
            _lacznik = new ProduktJednostka { PrzelicznikIlosc = przelicznik,ProduktId = produkt,Podstawowa = podstawowa};
        }

        public JednostkaProduktu(JednostkaProduktu baza)
            : base(baza)
        {
            _lacznik = baza.Lacznik;
        }
        public JednostkaProduktu(Jednostka baza): base(baza)
        {
        
        }
        public JednostkaProduktu(Jednostka baza, ProduktJednostka lacznik)
            : this(baza)
              {
                  _lacznik = lacznik??new ProduktJednostka{PrzelicznikIlosc = 1};
              }

        /// <summary>
        /// przelicznik do jednostki podstawowej
        /// </summary>
        public decimal Przelicznik {
            get { return _lacznik.PrzelicznikIlosc; }
            set { _lacznik.PrzelicznikIlosc = value; }
        }

        public bool Podstawowa
        {
            get { return _lacznik.Podstawowa; }
            set { _lacznik.Podstawowa = value; }
        }

        public long ProduktId
        {
            get { return _lacznik.ProduktId; }
            set { _lacznik.ProduktId = value; }
        }

        public long JednostkaProduktuId
        {
            get { return _lacznik.Id; }
          
        }

        public ProduktJednostka Lacznik
        {
            get
            {
                _lacznik.JednostkaId = Id;
                return _lacznik;
            }
            set { _lacznik = value ?? new ProduktJednostka(); }
        }

        public override string ToString()
        {
            return string.Format("{0} ",Nazwa);
        }
    }
}
