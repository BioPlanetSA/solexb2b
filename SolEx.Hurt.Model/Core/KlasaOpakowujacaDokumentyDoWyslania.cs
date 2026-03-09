using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolEx.Hurt.Model.Core
{
    //Klasa potrzebna do jsona - nie radził sobie ze slownikiem Dictionary<HistoriaDokumentuProdukt,List<HistoriaDokumentuProdukt>>
    public class KlasaOpakowujacaDokumentyDoWyslania
    {
        public HistoriaDokumentu Dokument { get; set; }
        public List<HistoriaDokumentuProdukt> Produkty { get; set; }

        public KlasaOpakowujacaDokumentyDoWyslania(HistoriaDokumentu dok, List<HistoriaDokumentuProdukt> prod)
        {
            Dokument = dok;
            Produkty = prod;
        }
    }
}
