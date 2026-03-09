using SolEx.Hurt.Core.DostepDane;
using SolEx.Hurt.Core.DostepDane.DaneObiektu;
using SolEx.Hurt.Helpers;

namespace SolEx.Hurt.Web.Site2.Helper.DaneListaAdmin
{
    public class DanePole
    {
        private readonly OpisPolaObiektu _wartosc;

        //public string KolorTla { get; set; }

        public string KlasaCssPola { get; set; }

        public OpisPolaObiektu Wartosc
        {
            get { return _wartosc; }
        }

        public DanePole(OpisPolaObiektu wartosc)
        {
            _wartosc = wartosc;
        }
    }
}