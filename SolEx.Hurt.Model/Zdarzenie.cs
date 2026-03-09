using ServiceStack.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Model
{
    [TworzDynamicznieTabele]
    public class UstawieniePowiadomienia : IHasLongId
    {
        public UstawieniePowiadomienia()
        {
            ParametryWysylania = new List<ParametryWyslania>();
        }

        [PrimaryKey]
        public long Id { get; set; }

        /// <summary>
        /// parametry wysyłania - głulpia lista dlatego że mamy ustawienia dla klienta, opikuna, drugiego opiekuna, przedstawicial itp.
        /// </summary>
        public List<ParametryWyslania> ParametryWysylania { get; set; }

        /// <summary>
        /// Określa czy klient ma możliwość wyłączenia zdarzenia
        /// </summary>
        public bool ZgodaNaZmianyPrzezKlienta { get; set; }

        public object Klucz()
        {
            return Id;
        }

        [Ignore]
        public bool BladKompilacji { get; set; }

        [Ignore]
        public bool WlaczoneDlaOdbiorcow
        {
            get { return ParametryWysylania.Any(x => x.Aktywny); }
        }
    }

    /// <summary>
    /// parametry do kogo wysyłać, bcc, czy aktywny
    /// </summary>
    public class ParametryWyslania
    {
        public ParametryWyslania()
        {
            Aktywny = true;
        }

        public TypyPowiadomienia DoKogo { get; set; }
        public bool Aktywny { get; set; }
        public string EmailBcc { get; set; }
        
    }
}

