using System;
using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;

namespace SolEx.Hurt.Model
{
    public class SzablonLimitow:IHasIntId
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public string Nazwa { get; set; }
        public int? IloscZamowien { get; set; }
        public decimal? WartoscZamowien { get; set; }
        public int IloscMiesiecy { get; set; }
        public DateTime? OdKiedy { get; set; }
        public long Tworca { get; set; }
        public decimal? MaksymalnaCenaTowaru { get; set; }

        public SzablonLimitow()
        {
            IloscMiesiecy = 1;
        }
    }
}