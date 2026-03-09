using System.Collections.Generic;
using SolEx.Hurt.Core;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Web.Site2.Models.DaneDoWidokow.Filtry
{
    public class ParametryDoAktywnychFiltrow
    {
 
        public ParametryDoAktywnychFiltrow(Dictionary<Atrybut,List<Cecha>> wybraneStale, Dictionary<Atrybut, List<Cecha>> wybraneFitry, KategorieBLL kategorie, Dictionary<string, string> szukane = null, string naglowek="", string stopka="", bool brak = true)
        {
            WybraneStaleFitry = wybraneStale;
            WybraneFitry = wybraneFitry;
            Naglowek = naglowek;
            Stopka = stopka;
            SzukaneFrazy = szukane;
            WybraneKategorie = kategorie;
            BrakProduktow = brak;
        }

        public Dictionary<Atrybut, List<Cecha>> WybraneStaleFitry { get; set; }
        public Dictionary<Atrybut, List<Cecha>> WybraneFitry { get; set; }
        public Dictionary<string,string> SzukaneFrazy { get; set; }
        public KategorieBLL WybraneKategorie { get; set; }
        public string Naglowek { get; set; }
        public string Stopka { get; set; }
        public bool BrakProduktow { get; set; }
    }
}