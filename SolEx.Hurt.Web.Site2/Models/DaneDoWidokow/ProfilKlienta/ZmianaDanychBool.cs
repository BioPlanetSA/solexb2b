using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Web.Site2.Models.DaneDoWidokow.ProfilKlienta
{
    public class ZmianaDanychBool
    {
        public TypUstawieniaKlienta Typ { get; set; }
        public bool Wartosc { get; set; }
        public string Tooltip { get; set; }
       // public string DodatkowyIdentyfikator { get; set; }
        public string Ikona { get; set; }
        public string OpisPrzycisku { get; set; }
        public string klasaCss { get; set; }

        /// <summary>
        /// standardowo renderujemy przycisk w formie - aktywowanie tej opcji, powoduje że zamiast formy renderujemy link
        /// </summary>
        public bool PokazTylkoLink { get; set; }
    }
}