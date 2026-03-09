using System;
using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;

namespace SolEx.Hurt.Model
{
    public class UkladKolumn : IHasLongId
    {
        [PrimaryKey]
        [AutoIncrement]
        public long Id { get; set; }
        public Type TypDanych { get; set; }
        public string Nazwa { get; set; }
        public string[] WidoczneKolumny { get; set; }

        public object Klucz()
        {
            return Id;
        }
    }
}
