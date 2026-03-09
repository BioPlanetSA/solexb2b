using System;
using System.Linq.Expressions;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Core.DostepDane
{
    public class SortowanieKryteria<TDaneZwracana>
    {
        public SortowanieKryteria(Expression<Func<TDaneZwracana, object>> warunek, KolejnoscSortowania kolejnosc, string poleNazwa)
        {
            WarunekExpression = warunek;
            Kierunek = kolejnosc;
            PoleNazwa = poleNazwa;
        }

        public string PoleNazwa { get; private set; }

        public KolejnoscSortowania Kierunek { get; set; }

        public Expression<Func<TDaneZwracana, object>> WarunekExpression { get; private set; }

        private Func<TDaneZwracana, object> _warunekSkompilowany = null;

        public Func<TDaneZwracana, object> Warunek
        {
            get
            {
                if (_warunekSkompilowany == null)
                {
                    _warunekSkompilowany = WarunekExpression.Compile();
                }
              
                return _warunekSkompilowany;
            }
        }
    }
}
