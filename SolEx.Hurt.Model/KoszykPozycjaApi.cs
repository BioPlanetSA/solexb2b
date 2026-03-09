using ServiceStack.DataAnnotations;

namespace SolEx.Hurt.Model
{
    public class KoszykPozycjaApi : KoszykPozycje
    {
        [Ignore]
        public string ProduktSymbol { get; set; }

        [Ignore]
        public int? ProduktID { get; set; }

        [Ignore]
        public long? IDKlienta { get; set; }

        [Ignore]
        public int? IDKoszyka { get; set; }


    }
}