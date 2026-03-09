using System;
using System.Collections.Generic;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Model.Web
{
    public class DodanieKomunikatuEventArgs : EventArgs
    {
        private Komunikat _komunikat;
        private PokazywanieKomunikatu _pozycja;
        public Komunikat Komunikat
        {
            get { return _komunikat; }
        }
        public PokazywanieKomunikatu Pozycja
        {
            get { return _pozycja; }
        }
        public DodanieKomunikatuEventArgs(string wiadomosc, KomunikatRodzaj rodzaj, string autor,PokazywanieKomunikatu pozycja)
        {
            _pozycja = pozycja;
            _komunikat=new Komunikat(wiadomosc,rodzaj,autor,pozycja);
        }
    }
    public class DodanieDoSortowaniaEventArgs:EventArgs
    {
        private List<Tuple<string, KolejnoscSortowania>> _sort;
        public bool Pasuje { get; set; }
        public List<Tuple<string, KolejnoscSortowania>> Kolumna
        {
            get { return _sort; }
        }
        public DodanieDoSortowaniaEventArgs(List<Tuple<string, KolejnoscSortowania>> sort)
        {
            _sort = sort;
            Pasuje = true;
        }
    }
}
