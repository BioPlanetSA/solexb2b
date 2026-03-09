using SolEx.Hurt.Core;

namespace SolEx.Hurt.Web.Site2.Models
{
    public abstract class ParametrySzczegolowProduktuBaza : PageMetaDataBase
    {
        public ProduktKlienta Produkt { get; set; }
        protected ParametrySzczegolowProduktuBaza(ProduktKlienta pk)
        {
            Produkt = pk;
        }
        public override string PageTitle
        {
            get { return string.IsNullOrEmpty(Produkt.Rodzina)? Produkt.Nazwa:Produkt.Rodzina; }
        }

        public override string MetaDescription
        {
            get { return Produkt.MetaOpis; }
        }

        public override string MetaKeywords
        {
            get { return Produkt.MetaSlowaKluczowe; }
        }
    }
}