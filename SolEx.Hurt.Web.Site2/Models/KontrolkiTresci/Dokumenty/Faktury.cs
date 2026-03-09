using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.Dokumenty
{
    public class Faktury:DokumentyBaza
    {
        public override RodzajDokumentu Typ
        {
            get { return RodzajDokumentu.Faktura; }
        }

        public override string Nazwa
        {
            get { return "Lista faktur"; }
        }
    }
}