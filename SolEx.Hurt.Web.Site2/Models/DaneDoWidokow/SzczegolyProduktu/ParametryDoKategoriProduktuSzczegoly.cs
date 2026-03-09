using System.Collections.Generic;
using SolEx.Hurt.Core;

namespace SolEx.Hurt.Web.Site2.Models.DaneDoWidokow.SzczegolyProduktu
{
    public class ParametryDoKategoriProduktuSzczegoly
    {
        public ParametryDoKategoriProduktuSzczegoly(List<KategorieBLL> kategorie, string naglowek, string stopka)
        {
            Kategorie = kategorie;
            Naglowek = naglowek;
            Stopka = stopka;
        }

        public string Naglowek { get; set; }
        public string Stopka { get; set; }
        public List<KategorieBLL> Kategorie { get; set; }
    }
}