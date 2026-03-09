using System.Collections.Generic;
using SolEx.Hurt.Core;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class ParametryPrzekazywaneDoSzczegolow : ParametrySzczegolowProduktuBaza
    {
        public ParametryPrzekazywaneDoSzczegolow(ProduktKlienta pk, string naglowek, IList<PoleSzczegolyDane> pola, IList<PoleSzczegolyDane> atrybuty, string dodatkowyCss, bool pokarzNazwe, string stopka) : base(pk)
        {
            Naglowek = naglowek;
            Pola = pola;
            Atrybuty = atrybuty;
            DodatkowyCss = dodatkowyCss;
            PokarzNazwe = pokarzNazwe;
            Stopka = stopka;
        }

        public string Naglowek { get; set; }
        public string Stopka { get; set; }
        /// <summary>
        /// Lista z wybranymi nazwami pola i wartościami do pokazania
        /// </summary>
        public IList<PoleSzczegolyDane> Pola { get; set; }

        /// <summary>
        /// Lista z wybranymi nazwami Atrybutów i wartościami do pokazania
        /// </summary>
        public IList<PoleSzczegolyDane> Atrybuty { get; set; }
        /// <summary>
        /// Dodatkowy CSS jest wykorzystywany do renderowania CSS do cen, jako że ceny korzystają z takich samych układów jak pola to div-a trzeba dopisywać dodatkowe CSS-y.
        /// </summary>
        public string DodatkowyCss { get; set; }
        public bool PokarzNazwe { get; set; }

    }
}