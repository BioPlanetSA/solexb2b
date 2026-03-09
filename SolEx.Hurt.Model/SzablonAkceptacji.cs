using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;

namespace SolEx.Hurt.Model
{
    public class SzablonAkceptacji:IHasIntId
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }
        public string Nazwa { get; set; }
        public long Tworca { get; set; }
    }
}
