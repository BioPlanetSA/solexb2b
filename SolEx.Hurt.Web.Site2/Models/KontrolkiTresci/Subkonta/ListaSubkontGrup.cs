using SolEx.Hurt.Core.ModelBLL;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.Subkonta
{
    public class ListaSubkontGrup : KontrolkaTresciBaza
    {
        public override string Nazwa
        {
            get { return "Lista grup subkont"; }
        }

        public override string Kontroler
        {
            get { return "SubkontaGrupy"; }
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