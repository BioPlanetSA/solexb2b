namespace SolEx.Hurt.Core.Helper
{
    public class SlownikWidokowSzczegolowProduktow : SlownikWidokowBaza
    {
        protected override string SciezkaWidokow
        {
            get
            {
                return "\\SzczegolyProduktu\\PolaProduktu";
            }
        }
    }
}