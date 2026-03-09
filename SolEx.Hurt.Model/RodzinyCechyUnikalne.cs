using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.DataAnnotations;

namespace SolEx.Hurt.Model
{
    [Alias("vRodzinyCechyUnikalne")]
    public class RodzinyCechyUnikalne
    {
        public string rodzina { get; set; }
        public long pid { get; set; }
        public long cid { get; set; }
        public long atrybutID { get; set; }
    }
}
