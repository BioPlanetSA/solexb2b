using System.Collections.Generic;
using SolEx.Hurt.Core;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class ParametryDoListyKategoriiProduktow
    {
        public IList<KategorieBLL> Kategorie { get; set; }

        /// <summary>
        /// jako klasa css
        /// </summary>
        public string RozmiarKafla { get; set; }

        public string SymbolOpisGrupy { get; set; }

        public IKlient Klient { get; set; }

        public string ElementCss { get; set; }
        public bool PokazujNazwe { get; set; }
    }
}