using System.Collections.Generic;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Core.ModelBLL
{
    /// <summary>
    /// Kolekcja dokumentów
    /// </summary>
    [FriendlyClassName("Kolekcja formularzy klienta")]
    public class DocumentCollection : List<DokumentyBll>
    {
        [FriendlyNameToken("petla(Dokument)", "@foreach(var DokumentyBll in Model.DocumentCollection.Data)")]
        public IList<DokumentyBll> Data
        {
            get
            {
                return this;
            }
        }
        public DocumentCollection() : base() { }
        public DocumentCollection(IEnumerable<DokumentyBll> items) : base(items) { }


    }
}
