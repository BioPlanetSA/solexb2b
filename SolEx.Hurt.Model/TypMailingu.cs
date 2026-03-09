using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ServiceStack.OrmLite;
using ServiceStack.DataAnnotations;

namespace SolEx.Hurt.Model
{
    [TworzDynamicznieTabele]
    public class TypMailingu
    {
        [AutoIncrement]
        public int Id { get; set; }
        public string Nazwa { get; set; }
        public bool Aktywny { get; set; }
    }
}
