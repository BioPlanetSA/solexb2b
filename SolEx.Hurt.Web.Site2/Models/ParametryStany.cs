using System.Collections.Generic;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class ParametryStany
    {
        public ParametryStany(IProduktKlienta produkt)
        {
            Produkt = produkt;
        }
        public ParametryStany(IProduktKlienta produkt, StanNaMagazynie stanyNaMagazynie)
        {
            Produkt = produkt;
            StanyNaMagazynie = stanyNaMagazynie;
        }
        public IProduktKlienta Produkt { get; set; }
        public StanNaMagazynie StanyNaMagazynie { get; set; }
        
    }

   
}