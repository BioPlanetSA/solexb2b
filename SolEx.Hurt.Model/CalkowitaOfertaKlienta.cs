using ServiceStack.DataAnnotations;
using SolEx.Hurt.Model.AtrybutyKlas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolEx.Hurt.Model
{
    public class CalkowitaOfertaKlienta
    {
        [FriendlyName("Mój katalog")]
        [KolumnaDostepnaWGridAdmina(80, true, false, false)]
        public bool MojKatalog { get; set; }

      [FriendlyName("Odkryty")]
        [KolumnaDostepnaWGridAdmina(80, true, false, false)]
        public bool ProduktOdkryty { get; set; }
       
        [FriendlyName("Ukryty")]
        [KolumnaDostepnaWGridAdmina(80, true, false, false)]
        public bool ProduktUkryty { get; set; }

        [FriendlyName("Produkt ID")]
        [KolumnaDostepnaWGridAdmina(100, true, true, false)]
        [PrimaryKey]
        public int ProduktID { get; set; }
        [FriendlyName("Stała cena")]
        [KolumnaDostepnaWGridAdmina(100, true, false, false)]
        public decimal? ProduktCenaStala { get; set; }
        [FriendlyName("Nazwa")]
        [KolumnaDostepnaWGridAdmina(0, true, true, false)]
        public string ProduktNazwa { get; set; }
        [FriendlyName("Symbol")]
        [KolumnaDostepnaWGridAdmina(0, true, true, false)]
        public string ProduktSymbol { get; set; }
    }
}
