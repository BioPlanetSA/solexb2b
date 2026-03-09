using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.Dokumenty
{
    public class Oferty : DokumentyBaza
    {
        public override RodzajDokumentu Typ
        {
            get { return RodzajDokumentu.Oferta; }
        }

        public override string Nazwa
        {
            get { return "Lista ofert"; }
        }
    }
}