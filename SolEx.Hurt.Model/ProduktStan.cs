using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using System;
using System.Collections.Generic;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.Hurt.Model
{
    [FriendlyClassName("Stan magazynowy produktu")]
    public class ProduktStan : IEqualityComparer<ProduktStan>,IHasLongId
    {
        public ProduktStan()
        {
        }

        public ProduktStan(ProduktStan ps)
        {
            ProduktId = ps.ProduktId;
            MagazynId = ps.MagazynId;
            Stan = ps.Stan;
         // this.KopiujPola(ps);
        }


        [UpdateColumnKey()]
        public long ProduktId { get; set; }
        [UpdateColumnKey()]
        public int MagazynId { get; set; }
        [FriendlyName("Ilość")]
        public decimal Stan { get; set; }

        public bool Equals(ProduktStan x, ProduktStan y)
        {
          return x.ProduktId == y.ProduktId && x.MagazynId == y.MagazynId && x.Stan == y.Stan;
        }

        public int GetHashCode(ProduktStan obj)
        {
            return obj.GetHashCode();
        }

        public long Id
        {
            get { return (ProduktId+"_"+MagazynId).WygenerujIDObiektuSHAWersjaLong();}
        }
    }
}
