using SolEx.Hurt.Core.ModelBLL;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.Subkonta
{
    public class ListaSubkont : KontrolkaTresciBaza
    {
        public override string Nazwa
        {
            get { return "Lista subkont"; }
        }

        public override string Kontroler
        {
            get { return "Subkonta"; }
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