using SolEx.Hurt.Core.ModelBLL;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.Subkonta
{
    public class ListaSzablonowAkceptacji : KontrolkaTresciBaza
    {
        public override string Nazwa
        {
            get { return "Lista szablonów akceptacji"; }
        }

        public override string Kontroler
        {
            get { return "SzablonyAkceptacji"; }
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