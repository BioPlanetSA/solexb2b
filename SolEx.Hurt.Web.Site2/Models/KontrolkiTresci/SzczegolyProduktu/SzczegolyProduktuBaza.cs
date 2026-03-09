using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.SzczegolyProduktu
{
    public abstract class SzczegolyProduktuBaza : KontrolkaTresciBaza
    {
        public override string Grupa { get { return "Karta produktu"; } }

        public long ProduktId
        {
            get
            {
                return long.Parse(PobierzIdentyfikator("produktId", true).ToString());               
            }
        }

        public override string Kontroler
        {
            get { return "SzczegolyProduktu"; }
        }
    }

}