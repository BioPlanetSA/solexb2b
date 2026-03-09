using System;
using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Model
{
    public class KlientLimitIlosciowy:IHasIntId
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }
        public int KlientId	 { get; set; }
        public int ProduktId	 { get; set; }
        public decimal Ilosc	 { get; set; }
       
        [FriendlyName("Od kiedy")]
        
        public DateTime? Od	 { get; set; }
        [FriendlyName("Do kiedy")]
        
        public DateTime? Do { get; set; }
    }
}
