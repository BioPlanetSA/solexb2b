namespace SolEx.Hurt.Web.Site2.Models.DaneDoWidokow.SzczegolyProduktu
{
    public class ParametryDoWymiarowProduktu
    {
        public ParametryDoWymiarowProduktu(WymiaryProduktu wymiary, int? obrazekpudelka, int? obrazekopzbiorczego, int? obrazekpalety, string jedPods,DaneNaglowekStopka naglowek)
        {
            Wymiary = wymiary;
            ObrazekOpZbiorczego = obrazekopzbiorczego;
            ObrazekPalety = obrazekpalety;
            ObrazekPudelka = obrazekpudelka;
            NaglowekStopka = naglowek;
            JednostkaPodstawowa = jedPods;
        }
        public WymiaryProduktu Wymiary { get; set; }
        public int? ObrazekPudelka { get; set; }
        public int? ObrazekOpZbiorczego { get; set; }
        public int? ObrazekPalety { get; set; }
        public DaneNaglowekStopka NaglowekStopka{ get; set; }
        public string JednostkaPodstawowa { get; set; }

    }
}