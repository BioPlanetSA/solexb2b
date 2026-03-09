using SolEx.Hurt.Model.Core;

namespace SolEx.Hurt.Model.Web
{
    public class DaneDoWykresu
    {
        public DaneDoWykresu(){}
        public DaneDoWykresu(WartoscLiczbowa cena, int ilosc)
        {
            Cena = cena;
            IloscPozycji = ilosc;
        }

        public int IloscPozycji { get; set; }
        public WartoscLiczbowa Cena { get; set; }
   
    }
}
