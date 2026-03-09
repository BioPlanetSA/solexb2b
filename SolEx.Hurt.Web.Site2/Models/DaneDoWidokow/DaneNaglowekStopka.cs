namespace SolEx.Hurt.Web.Site2.Models.DaneDoWidokow
{
    public class DaneNaglowekStopka
    {
        public DaneNaglowekStopka() { }
        public DaneNaglowekStopka(string naglowek, string stopka)
        {
            Naglowek = naglowek;
            Stopka = stopka;
        }

        public string Naglowek { get; set; }
        public string Stopka { get; set; }
    }
}