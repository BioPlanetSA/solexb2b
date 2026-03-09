using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class ParametryDoProduktowRodzinowych
    {
        public ParametryDoProduktowRodzinowych(ProduktKlienta pk, List<IProduktKlienta> listaProduktow, int iloscKolumn, HashSet<long>idUlubionych, ListaDane listaDane)
        {
            ProduktGlowny = pk;
            ProduktyRodzinowe = listaProduktow;
            IloscKolumn = iloscKolumn;
            IdUlubionych = idUlubionych;
            ListaDane = listaDane;
        }
        public ProduktKlienta ProduktGlowny { get; set; }
        public List<IProduktKlienta> ProduktyRodzinowe { get; set; }
        public int IloscKolumn { get; set; }
        public HashSet<long> IdUlubionych { get; set; }

        public ListaDane ListaDane { get; set; }

    }
}