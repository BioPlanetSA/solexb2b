using SolEx.Hurt.Model.Web;
using System.Collections.Generic;

namespace SolEx.Hurt.Model.Helpers
{
    public class FormularzZapytanieModel 
    {
        public FormularzZapytanieModel(){}

        public FormularzZapytanieModel(bool pokazTytul, bool pokazujAdresEmailNaJakiOdeslacOdpowiedz,string mailDoOdpowiedzi,  string tytul,
            string tresc, string mailDoWysylki, bool doOpiekuna, bool doPrzedstawiciela, bool doDrugiegoOpiekuna, List<ParametryPola> dpola)
        {
            PokazTytul = pokazTytul;
            PokazujAdresEmailNaJakiOdeslacOdpowiedz = pokazujAdresEmailNaJakiOdeslacOdpowiedz;
            Tytul = tytul;
            Tresc = tresc;
            MailDoWysylki = mailDoWysylki;
            MailDoOdpowiedzi = mailDoOdpowiedzi;
            DoOpiekuna = doOpiekuna;
            DoPrzedstawiciela = doPrzedstawiciela;
            DoDrugiegoOpiekuna = doDrugiegoOpiekuna;
            MailDoOdpowiedzi = MailDoOdpowiedzi;
            DPola = dpola;
        }
       
        
        public string Tytul  { get; set; }
        public string Tresc { get; set; }
        public bool PokazTytul { get; set; }
        public bool PokazujAdresEmailNaJakiOdeslacOdpowiedz { get; set; }
        public string MailDoWysylki { get; set; }
        public string MailDoOdpowiedzi { get; set; }
        public bool DoOpiekuna { get; set; }
        public bool DoPrzedstawiciela { get; set; }
        public bool DoDrugiegoOpiekuna { get; set; }
        public string TytulWartosc { get; set; }
        public string TrescWartosc{ get; set; }
        public List<ParametryPola> DPola { get; set; }
    }
}