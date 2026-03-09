using SolEx.Hurt.Core.ModelBLL;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.Subkonta
{
    public class ListaMiejscKosztow : KontrolkaTresciBaza
    {
        public override string Nazwa
        {
            get { return "Lista miejsc powstawanie kosztow"; }
        }

        public override string Kontroler
        {
            get { return "MiejsceKosztow"; }
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