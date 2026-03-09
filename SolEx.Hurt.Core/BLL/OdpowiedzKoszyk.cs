using System.Collections.Generic;
using SolEx.Hurt.Model.Core;

namespace SolEx.Hurt.Core.BLL
{
    public class OdpowiedzKoszyk
    {
        public List<OdpowiedzKoszykaDlaPozycji> Odpowiedzi { get; set; }
        public List<AktualneIlosci> Pozycje { get; set; }
        public bool CzyModal { get; set; }
        public WartoscLiczbowa Netto { get; set; }
        public WartoscLiczbowa Brutto { get; set; }
        public string Waluta { get; set; }
        public int? IloscPozycji { get; set; }
    }

    public class OdpowiedzKoszykaDlaPozycji
    {
        public string Tekst { get; set; }
        public string Typ { get; set; }
        public int CzasWyswietlania { get; set; }
    }

    public class AktualneIlosci
    {
        public long ProduktId { get; set; }
        public decimal Ilosc { get; set; }
    }
}