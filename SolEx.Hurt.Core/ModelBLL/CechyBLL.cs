using System;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.ModelBLL
{
    [Alias("Cecha")]
    [FriendlyName("Cecha", FriendlyOpis = "")]
    public class CechyBll: Cecha, ICechyBll, IComparable<CechyBll>
    {
        private int _lang;
        public CechyBll(){}

        public CechyBll(Cecha cecha):this(cecha, SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDPolski){}
        
        public CechyBll(Cecha cecha,int lang)
            : base(cecha)
        {
            _lang = lang;
        }

        [Ignore]
        public IObrazek Ikona
        {
            get
            {
                if (ObrazekId != null)
                {
                    return SolexBllCalosc.PobierzInstancje.Pliki.PobierzObrazek(ObrazekId.Value);
                }
                return null;
            }
        }

        [Ignore]
        [FriendlyName("Nazwa Atrybutu")]
        [WidoczneListaAdmin(true, false, false, true)]
        public virtual string NazwaAtrybutu
        {
            get
            {
                AtrybutBll atr = PobierzAtrybut();
                return atr != null ? atr.Nazwa : "";
            }
        }

        private AtrybutBll _atrybutBll = null;

        /// <summary>
        /// Pobiera atrybut do którego przypisana jest cecha
        /// </summary>
        /// <returns></returns>
        public virtual AtrybutBll PobierzAtrybut()
        {
            if (_atrybutBll == null && AtrybutId != null)
            {
                _atrybutBll = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<AtrybutBll>(AtrybutId.Value, this.JezykId);
                if(_atrybutBll == null)
                {
                   SolexBllCalosc.PobierzInstancje.Log.Error($"Brak atrybutu id: {AtrybutId.Value} dla cechy id: {this.Id}" ) ;
                }
            }
            return _atrybutBll;
        }

        public string PobierzNazweCechyZNazwaAtrybutu()
        {
            return NazwaAtrybutu + ": " + PobierzWyswietlanaNazwe;
        }

        public int CompareTo(CechyBll other)
        {
            if (this.Kolejnosc == other.Kolejnosc)
            {
                return 0;
            }

            if (this.Kolejnosc > other.Kolejnosc)
            {
                return 1;
            }
            
            return -1;
        }
        
        public override string ToString()
        {
            return string.Format("Nazwa: {0}, Atrybut: {1}", Nazwa, NazwaAtrybutu);
        }

    }
}
