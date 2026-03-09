using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class ParametrDoKataloguProduktow
    {
        public IKatalogSzablonModelBLL szablon;
        public IList<IProduktKlienta> ListaProduktow;
        public string OpisWpisanyPrzezKlienta;
    }
}