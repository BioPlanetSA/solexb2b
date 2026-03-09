using System.Collections.Generic;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core
{
    [Alias("Grupa")]
    public class GrupaBLL : Grupa,IPoleJezyk
    {
        public GrupaBLL(Grupa g, int jezyk):base(g)
        {
            JezykId = jezyk;
        }
        public GrupaBLL() { }
        public GrupaBLL(int jezyk): this(null, jezyk) { }
     

        public IList<KategorieBLL> PobierzKategorie(IKlient klient, HashSet<long> produktyRozpatrywane = null)
        {
            return SolexBllCalosc.PobierzInstancje.KategorieDostep.PobierzDrzewkoKategorii(Id, JezykId, klient, produktyRozpatrywane);
        }

        [Ignore]
        public int JezykId { get; set; }

        public override string ToString()
        {
            return this.Nazwa;
        }
    }
}