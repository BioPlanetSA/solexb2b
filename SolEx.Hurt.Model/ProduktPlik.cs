using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
    public class ProduktPlik : IHasLongId
    {
        public long Id
        {
            get
            {
                return (ProduktId + "_" + PlikId).WygenerujIDObiektuSHAWersjaLong();
            }
        }

        public long ProduktId { get; set; }
        public int PlikId { get; set; }
        public bool Glowny { get; set; }

        public ProduktPlik() { }
        public ProduktPlik(ProduktPlik bazowy)
        {
            ProduktId = bazowy.ProduktId;
            PlikId = bazowy.PlikId;
            Glowny = bazowy.Glowny;
        }

        public object PolaDoKlucza()
        {
            var cp = new ProduktPlik();
            return new {cp.PlikId, cp.ProduktId };
        }
    }
}
