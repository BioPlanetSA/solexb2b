using System.Collections.Generic;
using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;

namespace SolEx.Hurt.Model
{
    public class SzablonAkceptacjiPoziomy:IHasIntId
    {
        public SzablonAkceptacjiPoziomy()
        {
            Klienci = new List<long>();
        }
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }
        public int SzablonAkceptacjiId { get; set; }
        public List<long> Klienci { get; set; }
        public int Poziom { get; set; }
    }
}
