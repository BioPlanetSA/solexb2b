using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.Hurt.Model
{
    public class ProduktyZamienniki : IHasLongId
    {
        public ProduktyZamienniki()
        {
        }

        public ProduktyZamienniki(long produkt, long zamiennik, bool dwustronny= true)
        {
            ProduktId = produkt;
            ZamiennikId = zamiennik;
            Dwustronny = dwustronny;
        }

        public long Id
        {
            get { return (ProduktId + "-" + ZamiennikId).WygenerujIDObiektuSHAWersjaLong(); }
        }

        public long ProduktId { get; set; }
        public long ZamiennikId { get; set; }
        public bool Dwustronny {get; set;}
        public object PolaDoKlucza()
        {
            ProduktyZamienniki pomijanie = new ProduktyZamienniki();
            return new { pomijanie.ProduktId, pomijanie.ZamiennikId };
        }
    }
}
