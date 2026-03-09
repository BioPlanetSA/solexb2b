
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Web.Site2.AtrybutyMvc;

namespace SolEx.Hurt.Web.Site2.Models
{
    public abstract class PageMetaDataBase : IPageMetadata
    {

        public virtual string PageTitle
        {
            get { return ""; }
        }

        public virtual string MetaDescription
        {
            get { return ""; }
        }

        public virtual string MetaKeywords
        {
            get { return ""; }
        }

        public virtual string IdentyfikatorObiektu
        {
            get { return PageTitle.GetHashCode().ToString(); }
        }
    }
    public class TrescDoPokazania : PageMetaDataBase
    {
        public TrescBll Tresc { get; set; }

        public TrescBll Stopka { get; set; }

        public TrescBll Naglowek { get; set; }

        public TrescBll LewaKolumna { get; set; }
        public IKlient Klient { get; set; }

        public override string PageTitle
        {
            get { return Tresc.Nazwa; }
        }

        public override string MetaDescription
        {
            get { return Tresc.MetaOpis; }
        }

        public override string MetaKeywords
        {
            get { return Tresc.MetaSlowaKluczowe; }
        }
    }

    public class TrescDoPokazaniaElement
    {
        public IKlient Klient { get; set; }
        public TrescBll Tresc { get; set; }
   
    }
}