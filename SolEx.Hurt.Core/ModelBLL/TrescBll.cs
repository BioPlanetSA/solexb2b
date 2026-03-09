using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.ModelBLL
{
    [Alias("tresc")]
    public class TrescBll : Tresc, IPoleJezyk
    {
        public override string ToString()
        {
            return "["+ this.Symbol + "] [" + this.Id + "]";
        }

        protected ISolexBllCalosc Calosc = SolexBllCalosc.PobierzInstancje;

        [Ignore]
        public int JezykId { get; set; }

        public TrescBll(ISolexBllCalosc solexCalosc)
        {
            Calosc = solexCalosc;
        }

        public TrescBll()
        {
            JezykId = Calosc.Konfiguracja.JezykIDDomyslny;
        }
        public TrescBll(string nazwa, string symbol,int jezyk)
        {
            Nazwa = nazwa;
            Symbol = symbol;
            JezykId = jezyk;
        }

        public TrescBll(TrescBll trescBll):base(trescBll)
        {
            Calosc = trescBll.Calosc;
            JezykId = trescBll.JezykId;
        }

        private List<TrescWierszBll> _wiersz;        
        /// <summary>
        /// Wiersze bez względu na widoczność
        /// </summary>
        [Ignore]
        public List<TrescWierszBll> Wiersze
        {
            get
            {
                return _wiersz ?? (_wiersz = Calosc.DostepDane.Pobierz(JezykId, null, x => x.TrescId == Id, new[] {new SortowanieKryteria<TrescWierszBll>(x => x.Kolejnosc, KolejnoscSortowania.asc, "Kolejnosc")}).ToList());
            }
        }

        private List<TrescBll> _dzieci;
        /// <summary>
        /// Dzieci bez względu na widoczność
        /// </summary>
        [Ignore]
        public List<TrescBll> Dzieci
        {
            get
            {
                return _dzieci ?? (_dzieci = Calosc.DostepDane.Pobierz(JezykId, null, x => x.NadrzednaId == Id, new[] {new SortowanieKryteria<TrescBll>(x => x.Kolejnosc, KolejnoscSortowania.asc, "Kolejnosc")}).ToList());
            }
        }

        private string _css;

        public string PobierzCss()
        {
            if (_css == null)
            {
                StringBuilder sb = new StringBuilder();
                if (DodatkoweKlasyCss != null)
                {
                    foreach (var d in DodatkoweKlasyCss)
                    {
                        sb.AppendFormat("{0} ", d);
                    }
                }
                sb.Append(DodatkoweKlasyCssReczne);
                _css = sb.ToString();
            }
            return _css;
        }

        public void WyczyscCacheWierszy()
        {
            _wiersz = null;
        }
    }

    public class TrescBllImport : Tresc
    {
        public List<TrescWierszBllImport> Wiersze { get; set; }
        public List<TrescBllImport> Dzieci { get; set; }


        public static explicit operator TrescBllImport(TrescBll b)  // explicit byte to digit conversion operator
        {
            TrescBllImport tbi = Konwrsja(b);
            tbi.Wiersze = b.Wiersze.Select(x => (TrescWierszBllImport)x).ToList();
            if (b.Dzieci.Any())
            {
                tbi.Dzieci = b.Dzieci.Select(x => (TrescBllImport)x).ToList();
            }
            else
            {
                tbi.Dzieci=new List<TrescBllImport>();
            }
            return tbi;
        }

        private static TrescBllImport Konwrsja(TrescBll b)
        {
            TrescBllImport tbi = new TrescBllImport();
            tbi.KopiujPola(b, new { b.Wiersze });
            return tbi;
        }
    }

  
}
