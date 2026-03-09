using SolEx.Hurt.Core.ModelBLL;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.Subkonta
{
    public class ListaLimitow : KontrolkaTresciBaza
    {
        public override string Nazwa
        {
            get { return "Lista limitów"; }
        }

        public override string Kontroler
        {
            get { return "SzablonyLimitow"; }
        }

        public override string Akcja
        {
            get { return "Lista"; }
        }
        public override string Grupa
        {
            get { return "Subkonta"; }
        }
    }
}