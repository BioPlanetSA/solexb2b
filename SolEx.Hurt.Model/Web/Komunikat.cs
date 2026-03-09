using System;
using System.Globalization;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Model.Web
{
    public class Komunikat: IComparable
    {
        public Komunikat()
        {
        }

        public Komunikat(string wiadomosc, KomunikatRodzaj komunikatRodzaj)
        {
            Wiadomosc = wiadomosc;
            Typ = komunikatRodzaj;
            PozycjaPokazania = PokazywanieKomunikatu.NaGorze;
        }

        public Komunikat(string wiadomosc, KomunikatRodzaj komunikatRodzaj, string autor)
        {
            Wiadomosc = wiadomosc;
            Typ = komunikatRodzaj;
            Autor = autor;
            PozycjaPokazania = PokazywanieKomunikatu.NaGorze;
        }
        public Komunikat(string wiadomosc, KomunikatRodzaj komunikatRodzaj, string autor, PokazywanieKomunikatu pozycja)
        {
            Wiadomosc = wiadomosc;
            Typ = komunikatRodzaj;
            Autor = autor;
            PozycjaPokazania = pozycja;
        }
        public Komunikat(string wiadomosc, KomunikatRodzaj komunikatRodzaj, string tytul,int priorytet,string autor,PokazywanieKomunikatu pozycja)
        {
            Wiadomosc = wiadomosc;
            Typ = komunikatRodzaj;
            Autor = autor;
            Tytul = tytul;
            PozycjaPokazania = pozycja;
            Priorytet = priorytet;
        }
        public PokazywanieKomunikatu PozycjaPokazania { get; set; }
        public KomunikatRodzaj Typ { get; set; }
        public string Wiadomosc { get; set; }
        public int Priorytet { get; set; }
        public string Tytul { get; set; }
        public string Autor { get; set; }

        public override int GetHashCode()
        {
            return ( (Wiadomosc ?? "") + (Tytul ?? "") + Priorytet.ToString(CultureInfo.InvariantCulture) + Typ).GetHashCode();
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return -1;
            Komunikat objAsPart = obj as Komunikat;
            if (objAsPart == null) return -1;
            int tmp1 = (int)Typ;
            int tmp2 = (int)objAsPart.Typ;
            if (tmp1 > tmp2)
                return 1;
            if (tmp1 < tmp2) return -1;
            return 0;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Komunikat objAsPart = obj as Komunikat;
            if (objAsPart == null) return false;
            return Typ == objAsPart.Typ;
        }
    }

    public enum KomunikatRodzaj
    {
        info=0,
        danger =1,
        warning=2,
        success = 3
    }

    public delegate void DodanieKomunikatuEventHandler(object sender, DodanieKomunikatuEventArgs e);
}
