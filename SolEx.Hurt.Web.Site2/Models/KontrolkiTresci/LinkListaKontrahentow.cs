using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public class LinkListaKontrahentow : KontrolkaTresciBaza
    {
        [Lokalizowane]
        public override string Nazwa
        {
            get { return "Link do listy kontrahentów"; }
        }

        public override string Kontroler
        {
            get { return "Logowanie"; }
        }

        public override string Akcja
        {
            get { return "ListaKontrahentow"; }
        }
    }
}