using SolEx.Hurt.Core.ModelBLL;

namespace SolEx.Hurt.Web.Site2.Models.DaneDoWidokow
{
    public class ParametryDoWlasciciela
    {
        public ParametryDoWlasciciela( Owner wl, string pre, string so, string format)
        {
            Wlasciciel = wl;
            Prefix = pre;
            Sofix = so;
            FormatRegionu = format;
        }

        public Owner Wlasciciel { get; set; }
        public string Prefix { get; set; }
        public string Sofix { get; set; }
        public string FormatRegionu { get; set; }
    }
}