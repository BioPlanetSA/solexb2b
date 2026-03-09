namespace SolEx.Hurt.Model.Web
{
    public class DodawanieProduktuPrzyciski
    {
        public bool DodawanieTekstowe { get; set; }
        public string TekstPrzyciskBrak { get; set; }
        public string TekstPrzyciskJest { get; set; }
        public bool JuzDodany { get; set; }

        public decimal IleJuzJestWKoszyku { get; set; }
        public DodawanieProduktuPrzyciski(bool dodawanietxt, string tekstprzyciskbrak, string tekstprzyciskjest, bool jestwkoszyku, decimal ileJestWKoszyku)
        {
            // TODO: Complete member initialization
            DodawanieTekstowe = dodawanietxt;
            TekstPrzyciskBrak = tekstprzyciskbrak;
            TekstPrzyciskJest = tekstprzyciskjest;
            JuzDodany = jestwkoszyku;
            IleJuzJestWKoszyku = ileJestWKoszyku;
        }
        public DodawanieProduktuPrzyciski() { }
       
    }
}