using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.Hurt.Core.ModelBLL
{
    public class Sortowanie
    {
        public List<SortowaniePole> Pola { get; set; }
        public string Opis { get; set; }

        public string WersjaTekstowa { get; set; }

        public Sortowanie()
        {
            Pola=new List<SortowaniePole>();
        }

        public Sortowanie(List<SortowaniePole> opcjesortowania, string p, string wersja)
        {
            Pola = opcjesortowania;
            Opis = p;
            WersjaTekstowa = wersja;
        }

        public string SortowanieFraza()
        {
            if (Pola.Count == 0)
            {
                return "";
            }
            StringBuilder sb=new StringBuilder();
            foreach (var p in Pola)
            {
                sb.AppendFormat("{0} {1},", p.Pole, p.KolejnoscSortowania);
            }
            return sb.ToString(0, sb.Length - 1);
        }

        public string PobierzOpisNaPodstawiePol<T>(string frazarosnaco, string frazamalejaco)
        {
            StringBuilder opiss = new StringBuilder();
            foreach (var kolumna in Pola)
            {
                string tekst = Refleksja.ZnajdzPropertis(typeof(T), kolumna.Pole).PobierzAtrybutPola<FriendlyNameAttribute, string>(x => x.FriendlyName) ?? kolumna.Pole;
                opiss.AppendFormat("{0} {1} ", kolumna.KolejnoscSortowania == KolejnoscSortowania.asc ? frazarosnaco : frazamalejaco, tekst);
            }
            return opiss.ToString();
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(WersjaTekstowa))
            {
                WersjaTekstowa = Pola.Select(x => x.Pole + " " + x.KolejnoscSortowania).Aggregate((a, b) => a + "," + b) + "|" + Opis + ";";
            }
            return WersjaTekstowa;
        }
    }

    public class SortowaniePole
    {
        public SortowaniePole()
        {
            
        }
        public SortowaniePole(string p,KolejnoscSortowania kolejnosc)
        {
            Pole = p;
            KolejnoscSortowania = kolejnosc;
        }
        public string Pole { get; set; }
        public KolejnoscSortowania KolejnoscSortowania { get; set; }
    }

}
