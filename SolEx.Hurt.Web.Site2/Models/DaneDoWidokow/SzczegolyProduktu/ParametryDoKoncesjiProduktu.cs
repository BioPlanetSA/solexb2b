using System.Collections.Generic;

namespace SolEx.Hurt.Web.Site2.Models.DaneDoWidokow.SzczegolyProduktu
{
    public class ParametryDoKoncesjiProduktu
    {
        public ParametryDoKoncesjiProduktu(List<string> koncesja, string naglowek, string stopka)
        {
            Koncesje = koncesja;
            Naglowek = naglowek;
            Stopka = stopka;
        }

        public string Naglowek { get; set; }
        public string Stopka { get; set; }
        public List<string> Koncesje { get; set; }
    }
}