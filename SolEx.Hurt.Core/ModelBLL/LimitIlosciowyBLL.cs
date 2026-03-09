using System;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Core.ModelBLL
{
    public class LimitIlosciowyBLL:ProduktBazowy
    {
      public LimitIlosciowyBLL(ProduktBazowy baza) : base( baza)
      {
          
      }

        public LimitIlosciowyBLL():base( SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDPolski)
        {
        }

        [FriendlyName("Wielkość limitu")]
        
        public   decimal? IloscDostepna { get; set; }
        [FriendlyName("Ilosć wykorzystana")]
        
        public decimal  IloscWykorzystana { get; set; }
        [FriendlyName("Od kiedy")]
        
        public DateTime? Od { get; set; }
        [FriendlyName("Do kiedy")]
        
        public DateTime? Do { get; set; }

        public int LimitID { get; set; }
    }
}
