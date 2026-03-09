using SolEx.Hurt.Model;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class ParametryDoStronicowania
    {
        public ParametryDoStronicowania()
        {
        }

        public ParametryDoStronicowania(int liczbastron, ParametryPrzekazywaneDoListyProduktow parametryStronicowania, int lacznie, bool pokazacWyborStron, int rozmiarStrony, int[] dostepneRozmiaryStron)
        {
            LiczbaStron = liczbastron;
            ParametryStronicowania = parametryStronicowania;
            PokazacWyborStron = pokazacWyborStron;
            LacznaIlosc = lacznie;
            RozmiarStrony = rozmiarStrony;
            DostepneRozmiaryStron = dostepneRozmiaryStron;
        }
        
        public int LiczbaStron { get; set; }
        public ParametryPrzekazywaneDoListyProduktow ParametryStronicowania { get; set; }
        public bool PokazacWyborStron { get; set; }
        public int LacznaIlosc { get; set; }
        public int RozmiarStrony { get; set; }
        public int[] DostepneRozmiaryStron { get; set; }
    }
}