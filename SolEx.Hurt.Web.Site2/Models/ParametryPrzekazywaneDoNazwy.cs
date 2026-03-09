
using System.Collections.Generic;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class ParametryPrzekazywaneDoNazwy
    {
        public ParametryPrzekazywaneDoNazwy(IProduktKlienta pk, RodzajMetki metka, string dodatkoweKlasy, string link, bool produktRodzinowy, bool pokazUlubione, HashSet<long>idUlubionych=null )
        {
            Produkt = pk;
            RodzajMetki=metka;
            DodatkoweKlasy = dodatkoweKlasy;
            Link = link;
            ProduktRodzinowy = produktRodzinowy;
            PokazUlubione = pokazUlubione;
            IdUlubionych = idUlubionych;
        }

        public IProduktKlienta Produkt { get; set; }
        public RodzajMetki RodzajMetki { get; set; }
        public string DodatkoweKlasy { get; set; }
        public string Link { get; set; }
        public bool ProduktRodzinowy { get; set; }

        public HashSet<long> IdUlubionych { get;set; }

        public bool PokazUlubione { get; set; }
    }
}