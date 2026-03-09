using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class ListaProduktowZKategoriam 
    {
        public IProduktKlienta[] Produkty { get; set; }
        public KategorieBLL[] Kategorie { get; set; }
        public HashSet<long> ProduktyId { get; set; }
    
        public ListaProduktowZKategoriam(IEnumerable<IProduktKlienta> produkty=null, IEnumerable<KategorieBLL> kategorie = null, HashSet<long> produktyid=null   )
        {
            Produkty = produkty == null ? null : produkty.ToArray();
            Kategorie = kategorie == null ? null : kategorie.ToArray();
            ProduktyId = produktyid;
        }
        public ListaProduktowZKategoriam() { }    
    }
}