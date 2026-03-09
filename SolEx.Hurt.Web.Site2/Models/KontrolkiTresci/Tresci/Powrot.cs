using SolEx.Hurt.Core.ModelBLL;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.Tresci
{
    public class Powrot : KontrolkaTresciBaza
    {
        public override string Nazwa
        {
            get { return "Kontrolka powrotu do poprzednej strony"; }
        }

        public override string Kontroler
        {
            get { return "Tresci"; }
        }

        public override string Akcja
        {
            get { return "Powrot"; }
        }
        public override string Grupa
        {
            get { return "Wygląd"; }
        }
    }
}