using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;

namespace SolEx.Hurt.Model
{
    public class MiejsceKosztow:IHasLongId
    {

        [PrimaryKey]
        [AutoIncrement]
        public long Id { get; set; }

        public string Nazwa { get; set; }
        public long KlientId { get; set; }
    }
}
