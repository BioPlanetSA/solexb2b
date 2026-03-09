using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.DesignPatterns.Model;

namespace SolEx.Hurt.Model
{
    public class Sesja:IHasGuidId
    {
        public Sesja(Guid id,long idKlietna, string nazwaUrzadzenia, string ipKlienta,DateTime dataUtworzenia)
        {
            Id = id;
            KlientId = idKlietna;
            NazwaUrzadzenia = nazwaUrzadzenia;
            IpKlienta = ipKlienta;
            DataUtworzenia = dataUtworzenia;
        }
        public Sesja() { }
        public Guid Id { get; set; }
        public long KlientId { get; set; }
        public string NazwaUrzadzenia { get; set; }
        public string IpKlienta { get; set; }
        public DateTime DataUtworzenia { get; set; }
        public DateTime? DataZakonczenia { get; set; }
        public long? PrzedstawicielId { get; set; }
    }
}
