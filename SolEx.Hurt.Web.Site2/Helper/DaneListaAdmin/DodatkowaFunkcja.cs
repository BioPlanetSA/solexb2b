using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Web.Site2.Helper.DaneListaAdmin
{
    public class DodatkowaFunkcja
    {
        public DodatkowaFunkcja()
        {
            this.KlasaCssPrzycisku = "btn btn-success btn-sm";  //domyslny styl przycisku
        }
        public string KlasaCssPrzycisku { get; set; }
        public string Nazwa { get; set; }
        public string Adres { get; set; }
        public Komunikat KomunikatJavascriptCzyNapewno { get; set; }
    }
}