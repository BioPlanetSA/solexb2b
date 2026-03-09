using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.Hurt.Model
{

    public class KlientKategoriaKlienta : IHasLongId
    {
        public  long Id
        {
            get { return (KlientId + "_" + KategoriaKlientaId).WygenerujIDObiektuSHAWersjaLong(); }
        }
   
        public long KlientId { get; set; }
        public int KategoriaKlientaId { get; set; }

        /// <summary>
        /// Tworzy nowy łacznik
        /// </summary>
        /// <param name="klient">id klienta</param>
        /// <param name="kategoria">id kategorii klientów</param>
        public KlientKategoriaKlienta(long klient, int kategoria)
        {
            KlientId = klient;
            KategoriaKlientaId = kategoria;
        }
        public KlientKategoriaKlienta() { }
    }
}
