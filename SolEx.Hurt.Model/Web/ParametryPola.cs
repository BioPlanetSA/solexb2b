
using System.Collections.Generic;
using System.Web;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model.Web
{
    public class ParametryPola
    {

        public ParametryPola()
        {
            NazwaJakoNameDlaPola = true;
        }
        public ParametryPola(string nazwa, string wyswietlanaNazwa, string typ, bool wymagane, string grupa,object wartosc=null, string sciezka=null)
        {
            // TODO: Complete member initialization
            Nazwa = nazwa;
            WyswietlanaNazwa = wyswietlanaNazwa;
            Typ = typ;
            Wymagane = wymagane;
            Grupa = grupa;
            Wartosc = wartosc;
            SciezkaZalacznika = sciezka;
            NazwaJakoNameDlaPola = true;
        }
        public string Nazwa { get; set; }
        public string Typ { get; set; }
        public object Wartosc { get; set; }

        public string WyswietlanaNazwa { get; set; }
        public bool Wymagane { get; set; }
        public string Grupa { get; set; }
        public string klasaCSS { get; set; }

        public HttpPostedFileBase Plik { get; set; }
        public string SciezkaZalacznika { get; set; }
        public bool NazwaJakoNameDlaPola { get; set; }

        public Dictionary<string, object> SlownikWartosci { get; set; }
    }
}
