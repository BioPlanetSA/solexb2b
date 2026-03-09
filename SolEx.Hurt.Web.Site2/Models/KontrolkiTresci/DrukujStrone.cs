using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    [FriendlyName("Drukuj stronę", FriendlyOpis = "Dozwolony clasy css to fa-2x, fa-3x ...")]
    public class DrukujStrone : KontrolkaTresciBaza, IPrefixSofix
    {
        public DrukujStrone()
        {
            SelektorHtmlElementu = ".modal-content";
        }

        public override string Nazwa
        {
            get { return "Drukuj stronę"; }
        }
        
        public override string Kontroler
        {
            get { return "Wyglad"; }
        }

        public override string Akcja
        {
            get { return "Drukuj"; }
        }
       public override string Opis
        {
            get { return "Dozwolony clasy css to fa-2x, fa-3x ..."; }
        }
        public override string Grupa
        {
            get { return "Wygląd"; }
        }

        [FriendlyName("Tekst przed")]
        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Niewymagane]
        public string Prefix { get; set; }

        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Niewymagane]
        [FriendlyName("Test za")]
        public string Sofix { get; set; }

        [FriendlyName("Selektor elementu HTML do wydrukowania (jQuery)", FriendlyOpis = "Jeśli chcesz wydrukować całą stronę wpisz 'body'. Jeśli chcesz wydrukować tylko okno modalne wpisz '.modal-content' ")]
        [WidoczneListaAdmin(true, false, true, false)]
        [Wymagane]
        public string SelektorHtmlElementu { get; set; }
    }
}