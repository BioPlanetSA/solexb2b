using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.Hurt.Core
{
    [FriendlyClassName("Kolekcja produktów klienta")]
    public class ProductCollection : List<IProduktKlienta>
    {
        [FriendlyNameToken("petla(ProductBLL)", "@foreach(var ProduktBazowy in Model.ProductCollection.Data)")]
        public IList<IProduktKlienta> Data
        {
            get
            {
                return this;
            }
        }
        public ProductCollection()
        { }
        public ProductCollection(IEnumerable<IProduktKlienta> items) : base(items) { }

        public override string ToString()
        {
            return Serializacje.PobierzInstancje.SerializeList(this.Select(x=>x.Id), ';');
        }


    }
}
