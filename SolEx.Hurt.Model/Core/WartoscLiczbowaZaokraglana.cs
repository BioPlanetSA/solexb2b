using System;
using System.ComponentModel;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.Hurt.Model.Core
{
    [TypeConverter(typeof(WartoscLiczbowaZaokragalnaConverter))]
    public class WartoscLiczbowaZaokraglana : IComparable
    {
        private decimal _val;

        public WartoscLiczbowaZaokraglana()
        {
            _val = -1;
        }
        public decimal Wartosc
        {
            get { return _val; }
            set { _val = value; }
        }
        public String Waluta { get; set; }

        public WartoscLiczbowaZaokraglana(decimal d)
        {
            _val = d;
        }


        public WartoscLiczbowaZaokraglana(decimal p, string walutaKoszyka)
        {
            _val = p;
            Waluta = walutaKoszyka;
        }

        public static implicit operator WartoscLiczbowaZaokraglana(decimal d)
        {
            return new WartoscLiczbowaZaokraglana(d);
        }

        public static implicit operator Decimal(WartoscLiczbowaZaokraglana d)
        {
            return d._val;
        }

        public static implicit operator Decimal?(WartoscLiczbowaZaokraglana d)
        {
            if (d == null)
                return null;
            return d._val;
        }

    
        public override string ToString()
        {
            return string.Format("{0:0.####} {1}", _val, Waluta).Trim();
        }
        public static WartoscLiczbowaZaokraglana Parse(String s)
        {
            decimal wartosc;
            string[] stringi = s.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            string[] data = { "", "" };
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
                return new WartoscLiczbowaZaokraglana(wartosc,  data[1] );
            }
            throw new InvalidCastException(string.Format("Nie można rzutawać {0} na WartoscLiczbowaZaokraglana",s));
        }

        public int CompareTo(object druga)
        {
            var other = druga as WartoscLiczbowaZaokraglana;
            if (other == null) return 1;
           
            return Wartosc.CompareTo(other.Wartosc);
        }
    }
}