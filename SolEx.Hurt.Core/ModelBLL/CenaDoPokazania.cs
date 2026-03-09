using EDIFACT.BASETYPES;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Core;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Core.ModelBLL
{
    public class CenaDoPokazania
    {
        public string OpisCalosci { get; set; }

        public CenaDoPokazania() { }

        public CenaDoPokazania(WartoscLiczbowa cenaNetto, WartoscLiczbowa cenaBrutto, WartoscLiczbowa cenaNettoPrzekreslona, WartoscLiczbowa cenaBruttoPrzekreslona, string opisNetto, string cssWartosc, string cssCalosc, string cssOpis, JakieCenyPokazywac coPokazywac, string opisBrutto, string opisCalosci = null, string jednostkaMiaryOpcjonalna = null)
        {
            CenaNetto = cenaNetto;
            CenaBrutto = cenaBrutto;
            CenaNettoPrzekreslona = cenaNettoPrzekreslona;
            CenaBruttoPrzekreslona = cenaBruttoPrzekreslona;
            OpisNetto = opisNetto;
            CssWartosc = cssWartosc;
            CssOpis = cssCalosc;
            CoPokazywac = coPokazywac;
            OpisBrutto = opisBrutto;
            CssOpis = cssOpis;
            OpisCalosci = opisCalosci;
            if (!string.IsNullOrEmpty(jednostkaMiaryOpcjonalna))
            {
                JednostkaMiaryOpcjonalna = " /" + jednostkaMiaryOpcjonalna;
            }
        }

        public CenaDoPokazania(IFlatCenyBLL ceny, string opisNetto, string cssWartosc, string cssCalosc, string cssOpis, JakieCenyPokazywac coPokazywac, string opisBrutto, string opisCalosci = null, string jednostkaMiaryOpcjonalna = null)
            : this(ceny.CenaNetto, ceny.CenaBrutto, ceny.CenaNettoPrzedPromocja, ceny.CenaBruttoPrzedPromocja, opisNetto, cssWartosc, cssCalosc, cssOpis, coPokazywac, opisBrutto, opisCalosci, jednostkaMiaryOpcjonalna)
        {

        }

        public string CssSeparatorPoziomow { get; set; }

        public string JednostkaMiaryOpcjonalna { get; set; }

        public WartoscLiczbowa CenaNetto { get; set; }

        public WartoscLiczbowa CenaBrutto { get; set; }

        public WartoscLiczbowa CenaNettoPrzekreslona { get; set; }

        public WartoscLiczbowa CenaBruttoPrzekreslona { get; set; }

        public string OpisNetto { get; set; }

        public string CssOpis { get; set; }

        public string CssWartosc { get; set; }

        public string CssCalosc { get; set; }

        public JakieCenyPokazywac CoPokazywac { get; set; }

        public string OpisBrutto { get; set; }
    }
}
