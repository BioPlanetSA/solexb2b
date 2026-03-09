using System;
using System.ComponentModel;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.Hurt.Model.Core
{
    [TypeConverter(typeof (WartoscLiczbowaConverter))]
    public class WartoscLiczbowa : IComparable
    {
        private decimal _val;

        public WartoscLiczbowa()
        {
            _val = -1;
        }

        public decimal Wartosc
        {
            get { return _val; }
            set { _val = value; }
        }
     
        public string Waluta { get; set; }

        public WartoscLiczbowa(decimal d)
        {
            _val = d;
        }

        public WartoscLiczbowa(decimal p, string waluta)
        {
            _val = p;
            Waluta = waluta;
        }

        public static implicit operator WartoscLiczbowa(decimal d)
        {
            return new WartoscLiczbowa(d);
        }

        public static implicit operator Decimal(WartoscLiczbowa d)
        {
            return d._val;
        }


        public override string ToString()
        {
            return $"{_val:# ### ### ### ##0.00} {Waluta}".Trim();
        }

        public static WartoscLiczbowa Parse(String s)
        {
            decimal wartosc;
            string[] stringi = s.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
           string[] data= {"",""};
            if (stringi.Length == 1)
            {
                data[0] = stringi[0];
            }
            else if (stringi[0] == "-")
            {
                data[0] = stringi[0] + stringi[1];
                if (data.Length > 2)
                {
                    stringi[1] = data[2];
                }
            }
            else
            {
                data[0] = stringi[0];
                if (data.Length > 1)
                {
                    stringi[1] = data[1];
                }
            }
            data[0] = data[0].Replace(" ", "");
            if (TextHelper.PobierzInstancje.SprobojSparsowac(data[0], out wartosc))
            {
                return new WartoscLiczbowa(wartosc,  data[1]);
            }
            throw new InvalidCastException(string.Format("Nie można rzutawać {0} na WartoscLiczbowa", s));
        
        }

        public int CompareTo(object druga)
        {
            var other = druga as WartoscLiczbowa;
            if (other == null) return 1;

            return Wartosc.CompareTo(other.Wartosc);
        }

        public static WartoscLiczbowa operator -(WartoscLiczbowa w1, WartoscLiczbowa w2)
        {
            if (string.IsNullOrEmpty(w1.Waluta) && !string.IsNullOrEmpty(w2.Waluta))
            {
                w1.Waluta=w2.Waluta;
            }
            if (string.IsNullOrEmpty(w2.Waluta) && !string.IsNullOrEmpty(w1.Waluta))
            {
                w2.Waluta = w1.Waluta;
            }
            if (string.IsNullOrEmpty(w2.Waluta) && string.IsNullOrEmpty(w1.Waluta))
            {
                w2.Waluta = w1.Waluta;
            }
            if (w1.Waluta != w2.Waluta)
            {
                throw new Exception(string.Format("Nie można odejmować wartości w dwóch różnych walutach waluta 1: {0}, waluta 2: {1}", w1.Waluta, w2.Waluta));
            }

            WartoscLiczbowa nowy = new WartoscLiczbowa(w1.Wartosc - w2.Wartosc) {Waluta = w1.Waluta};
            return nowy;
        }
    }
}
   
