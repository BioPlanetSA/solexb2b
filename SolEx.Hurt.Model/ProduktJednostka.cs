using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.Hurt.Model
{
    public class ProduktJednostka:IHasLongId
    {
        public long Id { get { return (ProduktId + "||" + JednostkaId).WygenerujIDObiektuSHAWersjaLong(); } }
        public long JednostkaId { get; set; }
        public long ProduktId { get; set; }
        public bool Podstawowa { get; set; }
     
        /// <summary>
        /// Przelicznik względem jednostki podstawowej
        /// </summary>
        public decimal PrzelicznikIlosc { get; set; }
    }
}
