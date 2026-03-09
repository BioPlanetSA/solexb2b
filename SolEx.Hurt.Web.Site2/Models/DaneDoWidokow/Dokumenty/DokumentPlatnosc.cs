using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;

namespace SolEx.Hurt.Web.Site2.Models.DaneDoWidokow.Dokumenty
{
    public class DokumentPlatnosc
    {
        public int Dokument { get; set; }
        public decimal Wartosc { get; set; }
        public DokumentyBll DokumentObiekt { get; set; }
    }
}