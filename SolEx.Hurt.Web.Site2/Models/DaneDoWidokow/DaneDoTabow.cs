
namespace SolEx.Hurt.Web.Site2.Models.DaneDoWidokow
{
    public class DaneDoTabow
    {
        public DaneDoTabow(string link, string nazwa, bool wybrany, string opisDlaKlienta, bool tlumacz = true, string param=null)
        {
            Link = link;
            Nazwa = nazwa;
            CzyWybrany = wybrany;
            DodatkoweParametry = param;
            Opis = opisDlaKlienta;
            TlumaczNazwe = tlumacz;
        }
        public string Link { get; set; }
        public string Nazwa { get; set; }

        public string Opis { get; set; }

        public bool CzyWybrany { get; set; }
        public bool TlumaczNazwe { get; set; }
        public string DodatkoweParametry { get; set; }
    }
}