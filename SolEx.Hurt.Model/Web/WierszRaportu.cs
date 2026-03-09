using System;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Core;

namespace SolEx.Hurt.Model.Web
{
    public class WierszRaportu:HistoriaDokumentuProdukt
    {
    
        public string NazwaDokumentu { get; set; }
     
        public string NazwaKlienta { get; set; }
     
        public DateTime DataUtworzenia { get; set; }
       
        public bool Zrealizowane { get; set; }
        public new WartoscLiczbowaZaokraglana ilosc {
            get {return new WartoscLiczbowaZaokraglana(base.Ilosc);}
            set { base.Ilosc = value; }
        }
      
         [FriendlyName("Nazwa klienta")]
        public string SymbolKlienta { get; set; }
      
         public string EmailKlienta { get; set; }
       
         public string Dzial { get; set; }
      
         public string Status { get; set; }
    
         public WartoscLiczbowa LimitWartosci { get; set; }
      
         public string NazwaOdbiorcy { get; set; }
              
         public string SymbolOdbiorcy { get; set; }
  
         public string EmailOdbiorcy { get; set; }
    
        [FriendlyName("Kod kreskowy")]
         public string KodKreskowy { get; set; }
    }
}
