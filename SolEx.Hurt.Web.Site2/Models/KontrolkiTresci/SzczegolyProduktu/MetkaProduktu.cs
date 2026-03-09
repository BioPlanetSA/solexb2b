using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.SzczegolyProduktu
{
    public class MetkaProduktu : SzczegolyProduktuBaza
    {
        public override string Nazwa
        {
            get { return "Metka produktu"; }
        }

        public override string Akcja
        {
            get { return "Metka"; }
        }

        [FriendlyName("Pozycja metki")]
        [WidoczneListaAdmin(true, true, true, true)]
        public MetkaPozycjaSzczegoly Pozycja { get; set; }
    }

}