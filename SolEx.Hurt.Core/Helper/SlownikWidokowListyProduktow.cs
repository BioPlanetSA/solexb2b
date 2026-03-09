namespace SolEx.Hurt.Core.Helper
{
    public class SlownikWidokowListyProduktow : SlownikWidokowBaza
    {
        protected override string SciezkaWidokow
        {
            get
            {
                return "\\Produkty\\Widoki";
            }
        }
    }
}