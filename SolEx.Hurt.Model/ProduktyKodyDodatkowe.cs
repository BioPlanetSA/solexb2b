using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;

namespace SolEx.Hurt.Model
{
    public class ProduktyKodyDodatkowe : IHasIntId
    {
        [PrimaryKey]
        public int Id { get; set; }
       public long ProduktId{ get; set; }
       public string Kod{ get; set; }
       public string Nazwa { get; set; }
       public int? KlientId { get; set; }
    }
}
