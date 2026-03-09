using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;

namespace SolEx.Hurt.Model
{
    public class PieczatkiTyp : IHasIntId
    {
        public PieczatkiTyp()
        {
            PowiazaneProduktyId = new List<long>();
        }

        public PieczatkiTyp(PieczatkiTyp baza)
        {
            this.KopiujPola(baza);
        }

        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }
        public string SymbolTypu { get; set; }
        public string Nazwa { get; set; }
        public List<long> PowiazaneProduktyId { get; set; }
        public string DozwoloneKoloryCalejPieczatki { get; set; }
        public string OpisHtml { get; set; }
        public decimal Wysokosc_mm { get; set; }
        public decimal Szerokosc_mm { get; set; }
    }
}
