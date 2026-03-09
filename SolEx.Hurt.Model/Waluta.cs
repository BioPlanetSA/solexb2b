using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
    public class Waluta: IHasLongId,IPoleJezyk
    {
        public Waluta()
        {
        }

        public Waluta(long id, string walutaB2B, string walutaErp):base()
        {
            Id = id;
            WalutaB2b = walutaB2B;
            WalutaErp = walutaErp;
        }

        public Waluta(long id, string walutaB2B, string walutaErp, decimal kurs) : base()
        {
            Id = id;
            WalutaB2b = walutaB2B;
            WalutaErp = walutaErp;
            Kurs = kurs;
        }

        [UpdateColumnKey]
        [PrimaryKey]
        [WidoczneListaAdmin(true, false, false, false)]
        [GrupaAtttribute("Dane podstawowe", 0)]
        public long Id { get; set; }
        
        [WidoczneListaAdmin(true, true, false, false)]
        [GrupaAtttribute("Dane podstawowe", 0)]
        public string WalutaErp { get; set; }
        
        [WidoczneListaAdmin(true, true, false, false)]
        [GrupaAtttribute("Dane podstawowe", 0)]
        [Lokalizowane]
        public string WalutaB2b { get; set; }

        [WidoczneListaAdmin(true, true, true, false)]
        [GrupaAtttribute("Dane podstawowe", 0)]
        public string NrKonta { get; set; }

        [WidoczneListaAdmin(true, true, false, false)]
        [GrupaAtttribute("Kurs waluty", 0)]
        public decimal? Kurs { get; set; }

        [Ignore]
        public int JezykId { get; set; }
    }
}
