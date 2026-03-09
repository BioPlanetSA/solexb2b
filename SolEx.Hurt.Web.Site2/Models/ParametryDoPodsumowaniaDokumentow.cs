using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Web.Site2.Models
{
    public class ParametryDoPodsumowaniaDokumentow
    {
        public ParametryDoPodsumowaniaDokumentow(IKlient k, bool ukryjKredyt, DocumentSummary podsum)
        {
            AktualnyKlient = k;
            UkryjkredytKupiecki = ukryjKredyt;
            Podsumowanie = podsum;
        }
        public IKlient AktualnyKlient { get; set; }
        public bool UkryjkredytKupiecki { get; set; }
        public DocumentSummary Podsumowanie { get; set; }

    }
}